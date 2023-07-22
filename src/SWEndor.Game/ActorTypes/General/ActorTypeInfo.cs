using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;
using SWEndor.Game.Models;
using SWEndor.Game.Player;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Projectiles;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.Weapons;
using System;
using System.Collections.Generic;
using System.IO;
using SWEndor.Game.Primitives.Extensions;
using SWEndor.Game.Sound;
using SWEndor.Game.Actors.Components;
using SWEndor.Game.Actors.Models;

namespace SWEndor.Game.ActorTypes
{
  public partial class ActorTypeInfo : ITypeInfo<ActorInfo>
  {
    public ActorTypeInfo(Factory owner, string id, string name)
    {
      ActorTypeFactory = owner;
      if (id?.Length > 0) { ID = id; }
      if (name?.Length > 0) { Name = name; }

      SystemData.Reset();
      CombatData.Reset();
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }

    // Basic Info
    public string ID;

    [INIValue("General", "Name")]
    public string Name;

    /// <summary>Short hand class designation</summary>
    [INIValue("General", "Designation")]
    public string Designation;

    // Data
    [INIValue("General", "Mask")]
    public ComponentMask Mask { get; set; } = ComponentMask.NONE;

    // Data (structs)
    [INIEmbedObject("System")]
    internal SystemData SystemData;

    [INIEmbedObject("Combat")]
    internal CombatData CombatData;

    [INIEmbedObject("Regen")]
    internal RegenData RegenData;

    [INIEmbedObject("TimedLife")]
    internal TimedLifeData TimedLifeData;

    [INIEmbedObject("Armor")]
    internal ArmorData ArmorData = ArmorData.Default;

    [INIEmbedObject("MoveLimit")]
    internal MoveLimitData MoveLimitData = MoveLimitData.Default;

    [INIEmbedObject("Render")]
    internal RenderData RenderData = RenderData.Default;

    [INIEmbedObject("AI")]
    internal AIData AIData = AIData.Default;

    [INIEmbedObject("Mesh")]
    internal MeshData MeshData = MeshData.Default;

    [INIEmbedObject("Explode")]
    internal ExplodeSystemData ExplodeSystemData = ExplodeSystemData.Default;

    [INIEmbedObject("Weapon")]
    internal WeapSystemData WeapSystemData = WeapSystemData.Default;

    [INIEmbedObject("DyingMove")]
    internal DyingMoveData DyingMoveData;

    [INIEmbedObject("Score")]
    internal ScoreData ScoreData;

    [INIEmbedObject("Sound")]
    internal SoundData SoundData = SoundData.Default;

    [INIEmbedObject("AddOn")]
    internal AddOnSystemData AddOnData = AddOnSystemData.Default;

    [INIEmbedObject("Debris")]
    internal DebrisSystemData DebrisData = DebrisSystemData.Default;

    [INIEmbedObject("Camera")]
    internal CameraSystemData CameraData = CameraSystemData.Default;

    [INIEmbedObject("Spawner")]
    internal SpawnerData SpawnerData = SpawnerData.Default;

    [INIEmbedObject("DamageSpecial")]
    internal DamageSpecialData DamageSpecialData = DamageSpecialData.Default;
    

    // derived
    internal MoveBehavior MoveBehavior;
    internal UnfixedWeaponData cachedWeaponData;

    public void LoadFromINI(string id, string filepath)
    {
      ID = id;
      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        var self = this;
        f.LoadByAttribute(ref self);

        MeshData.Load(Engine, ID);
      }
    }

    public void SaveToINI(string filepath)
    {
      //ID = id;
      //string filepath = Path.Combine(Globals.ActorTypeINIDirectory, id + ".ini");
      Directory.CreateDirectory(Path.GetDirectoryName(filepath));

      if (!File.Exists(filepath))
        File.Create(filepath).Close();

      INIFile f = new INIFile(filepath);
      var self = this;
      f.UpdateByAttribute(ref self);
      f.WriteToFile(filepath);
    }

    public void Init()
    {
      cachedWeaponData.Load(Engine, this);
      MoveBehavior.Load(this);
      //if (SystemData.AllowSystemDamage && SystemData.Parts.Length == 0)
      //  SystemData.AutoParts(this);
    }

    public virtual void Initialize(ActorInfo ainfo)
    {
      SoundData.ProcessInitial(ainfo.Engine, ainfo);
    }

    public virtual void ProcessState(Engine engine, ActorInfo ainfo)
    {
      // weapons
      foreach (WeaponInfo w in ainfo.WeaponDefinitions.Weapons)
        w.Reload(engine);

      // regeneration
      ainfo.Regenerate(engine.Game.TimeSinceRender);

      // explode
      ainfo.TickExplosions();

      // dying
      if (ainfo.IsDying)
        DyingMoveData.Update(ainfo, engine.Game.TimeSinceRender);

      // sound
      if (PlayerInfo.Actor != null
        && ainfo.Active
        && !ainfo.IsScenePlayer)
      {
        SoundData.Process(engine, ainfo);
      }

      // projectile
      if (ainfo.TargetType.Has(TargetType.MUNITION)
        && !ainfo.IsDyingOrDead)
      {
        NearEnoughImpact(engine, ainfo);
      }
    }

    public void NearEnoughImpact(Engine engine, ActorInfo proj)
    {
      float impdist = CombatData.ImpactCloseEnoughDistance;
      if (impdist > 0 && proj.CurrentAction != null && proj.CurrentAction is ProjectileAttackActor)
      {
        ActorInfo target = ((ProjectileAttackActor)proj.CurrentAction).Target_Actor;
        if (target != null)
        {
          //if (target.TargetType.Intersects(TargetType.MUNITION))
          //  impdist += target.TypeInfo.CombatData.ImpactCloseEnoughDistance;

          // Anticipate
          float dist = DistanceModel.GetDistance(engine, proj, target, impdist + 1);

          if (dist < impdist)
          {
            target.TypeInfo.ProcessHit(engine, target, proj, target.GetGlobalPosition(), new TV_3DVECTOR());
            proj.TypeInfo.ProcessHit(engine, proj, target, target.GetGlobalPosition(), new TV_3DVECTOR());
          }
        }
      }
    }

    public virtual void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
#if DEBUG
      if (owner == null)
        throw new ArgumentNullException("owner");

      if (hitby == null)
        throw new ArgumentNullException("hitby");
#endif

      if (hitby.TypeInfo.CombatData.ImpactDamage == 0)
        return;

      if (owner.IsDying && owner.TypeInfo.CombatData.HitWhileDyingLeadsToDeath)
        owner.SetState_Dead();

      if (owner.TargetType.Has(TargetType.MUNITION))
      {
        // do nothing
      }
      else if (hitby.TargetType.Has(TargetType.MUNITION))
      {
        ProcessHit(engine, owner, hitby.TopParent, hitby.TypeInfo.CombatData, hitby.TypeInfo.DamageSpecialData, impact);
        hitby.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
        hitby.SetState_Dead(); // projectiles die on impact
      }
      else
      {
        // Collision
        bool victimIsPlayer = owner.IsScenePlayer; // in process of dealing damage, the player actor may become unassigned.
        float damage = owner.InflictDamage(hitby.TypeInfo.CombatData.ImpactDamage, hitby.TypeInfo.CombatData.DamageType, impact, hitby.TypeInfo.DamageSpecialData);
        if (owner.HP > 0
          && owner.Mask.Has(ComponentMask.CAN_MOVE)
          && owner.TargetType.Has(TargetType.FIGHTER))
        {
          float repel = -owner.MoveData.Speed * 0.25f;
          owner.MoveRelative(repel, 0, 0);
        }

        ActorInfo attacker = hitby.TopParent;
        if (attacker.IsScenePlayer)
        {
          if (!attacker.IsAlliedWith(owner))
          {
            ScoreHit(engine, PlayerInfo.Score, owner, damage);
          }
          else
          {
            ScoreFriendlyHit(engine, PlayerInfo.Score, owner, damage, true);
          }
        }
        
        // Fighter Collision
        if (owner.TargetType.Has(TargetType.FIGHTER)
          && owner.IsDyingOrDead)
        {
          owner.SetState_Dead();
          if (victimIsPlayer)
          {
            ScoreGetHit(engine, PlayerInfo.Score, owner, attacker, damage);
          }
        }

        owner.OnHitEvent(attacker);
        if (owner.IsDyingOrDead)
        {
          owner.OnDeathEvent(attacker);
        }
      }
    }

    internal void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, CombatData projData, DamageSpecialData dmgData, TV_3DVECTOR impact)
    {
#if DEBUG
      if (owner == null)
        throw new ArgumentNullException("owner");

#endif
      if (projData.ImpactDamage == 0)
        return;

      if (owner.IsDying
        && owner.TypeInfo.CombatData.HitWhileDyingLeadsToDeath)
        owner.SetState_Dead();

      if (!owner.IsDyingOrDead)
      {
        float damage = 0;
        bool victimIsPlayer = owner.IsScenePlayer; // in process of dealing damage, the player actor may become unassigned.
        //if (owner.TargetType.Has(TargetType.MUNITION))
        //{
        //  owner.SetState_Dead(); // projectiles die on impact
        //}
        //else
        {
          float p_hp = owner.HP;
          damage = owner.InflictDamage(projData.ImpactDamage, projData.DamageType, impact, dmgData);
          float hp = owner.HP;

          if (owner.IsPlayer)
            if (hp < (int)p_hp)
              PlayerInfo.FlashHit(PlayerInfo.StrengthColor);

          ActorInfo attr = hitby?.TopParent;
          if (attr != null)
          {
            attr.OnRegisterHitEvent(owner);
            if (owner.IsDyingOrDead)
            {
              attr.OnRegisterKillEvent(owner);
            }
          }
        }

        DamageSpecialData.ProcessHit(engine, owner);
        // scoring
        // warning: hitby may be null if the owner of the projectile is killed before his hit lands.
        ActorInfo attacker = hitby?.TopParent;
        if (attacker != null)
        {
          if (attacker.IsScenePlayer)
          {
            if (!attacker.IsAlliedWith(owner))
              ScoreHit(engine, PlayerInfo.Score, owner, damage);
            else
              ScoreFriendlyHit(engine, PlayerInfo.Score, owner, damage, false);
          }

          if (!attacker.IsAlliedWith(owner))
          {
            // AI
            owner.AIDecision.OnAttacked?.Invoke(owner, attacker);
          }
        }

        owner.OnHitEvent(attacker); // allow null
        if (owner.IsDyingOrDead)
        {
          owner.OnDeathEvent(attacker);
        }

        if (victimIsPlayer)
        {
          ScoreGetHit(engine, PlayerInfo.Score, owner, attacker, damage);
        }
      }
    }

    public virtual void ProcessHit(Engine engine, ActorInfo owner, ProjectileInfo proj, TV_3DVECTOR impact)
    {
#if DEBUG
      if (owner == null)
        throw new ArgumentNullException("owner");

      if (proj == null)
        throw new ArgumentNullException("proj");
#endif
      // proj.Owner may read rubbish data if the actual owner is destroyed and sent to the pool. For now this is handled by forcing projectiles to hold their owner's Scopes to delay erasure of data.
      ProcessHit(engine, owner, proj.Owner, proj.TypeInfo.CombatData, proj.TypeInfo.DamageSpecialData, impact);

      if (!proj.TypeInfo.DamageSpecialData.NeverDisappear)
      {
        proj.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
        proj.SetState_Dead();
      }
    }

    private void ScoreHit(Engine engine, ScoreInfo score, ActorInfo victim, float damage)
    {
      if (damage > 0)
      {
        //score.AddHit(engine, victim, damage);
        score.AddEntry(new ScoreInfo.Entry(engine.Game.GameTime, ScoreInfo.EntryType.HIT, victim.Name, victim.TypeInfo, damage, (int)(victim.TypeInfo.ScoreData.PerStrength * damage)));

        if (engine.PlayerInfo.Actor != null)
          if (engine.PlayerInfo.Actor.IsSystemOperational(SystemPart.SCANNER))
            engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.ENEMY_HIT)
                                           , 0.5f
                                           , engine.PlayerInfo.FactionColor
                                           , -99);
      }

      if (victim.IsDyingOrDead)
      {
        //score.AddKill(engine, victim);
        engine.SoundManager.SetMoodFromKill(victim.TargetType);
        score.AddEntry(new ScoreInfo.Entry(engine.Game.GameTime, ScoreInfo.EntryType.KILL, victim.Name, victim.TypeInfo, 0, victim.TypeInfo.ScoreData.DestroyBonus));

        if (victim.TargetType.Has(TargetType.FIGHTER))
          engine.SoundManager.SetMood(MoodState.DESTROY_FIGHTER);
        else if (victim.TargetType.Has(TargetType.SHIP))
          engine.SoundManager.SetMood(MoodState.DESTROY_SHIP);

        if (engine.PlayerInfo.Actor != null)
          if (engine.PlayerInfo.Actor.IsSystemOperational(SystemPart.SCANNER))
            engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.ENEMY_DESTROYED).F(victim.Name)
                                           , 2
                                           , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL));
      }
    }

    private void ScoreFriendlyHit(Engine engine, ScoreInfo score, ActorInfo victim, float damage, bool isCollision)
    {
      if (damage > 0)
      {
        score.AddEntry(new ScoreInfo.Entry(engine.Game.GameTime, ScoreInfo.EntryType.FRIENDLY_HIT, victim.Name, victim.TypeInfo, damage, (int)(victim.TypeInfo.ScoreData.PerStrength * damage)));

        if (!victim.IsDyingOrDead)
        {
          if (!victim.TargetType.Has(TargetType.MUNITION)) // shooting a missile does not raise alarm
          {
            ActorInfo victim_owner = victim.Parent ?? victim;
            // we should do an actual pilot check
            Engine.Screen2D.MessageText(string.Format("{0}: {1}, watch {2}!", victim_owner.Name, PlayerInfo.Name, isCollision ? "it" : "your fire")
                                            , 5
                                            , victim_owner.Faction.Color
                                            , -1
                                            , false);
          }
        }
      }

      if (victim.IsDyingOrDead)
      {
        score.AddEntry(new ScoreInfo.Entry(engine.Game.GameTime, ScoreInfo.EntryType.FRIENDLY_KILL, victim.Name, victim.TypeInfo, 0, victim.TypeInfo.ScoreData.DestroyBonus));
      }
    }

    private void ScoreGetHit(Engine engine, ScoreInfo score, ActorInfo victim, ActorInfo attacker, float damage)
    {
      // victim is the player
      if (damage > 0)
      {
        score.AddEntry(new ScoreInfo.Entry(engine.Game.GameTime, ScoreInfo.EntryType.DAMAGE_TAKEN, attacker?.Name, attacker?.TypeInfo, damage, (int)(victim.TypeInfo.ScoreData.PerStrength * damage)));
      }

      if (victim.IsDyingOrDead)
      {
        score.AddEntry(new ScoreInfo.Entry(engine.Game.GameTime, ScoreInfo.EntryType.DEATH, attacker?.Name, attacker?.TypeInfo, 0, victim.TypeInfo.ScoreData.DestroyBonus));
      }
    }

    public virtual bool FireWeapon(Engine engine, ActorInfo owner, ActorInfo target, WeaponShotInfo sweapon)
    {
      if (owner == null)
        return false;

      // AI Determination
      if (EqualityComparer<WeaponShotInfo>.Default.Equals(sweapon, WeaponShotInfo.Automatic))
      {
        foreach (WeaponShotInfo ws in owner.WeaponDefinitions.AIWeapons)
          if (FireWeapon(engine, owner, target, ws))
            return true;
      }
      else
      {
        return sweapon.Fire(engine, owner, target);
      }
      
      return false;
    }

    public virtual void FireAggregation(ActorInfo owner, ActorInfo target, ProjectileTypeInfo weapontype)
    {
      float accuracy = 1;

      float d = DistanceModel.GetDistance(Engine, owner, target) / weapontype.MoveLimitData.MaxSpeed;
      TV_3DVECTOR angle = (target.GetGlobalPosition() - owner.GetGlobalPosition()).ConvertDirToRot(Engine.TrueVision.TVMathLibrary) - owner.GetGlobalRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Random.NextDouble() < accuracy)
        target.InflictDamage(weapontype.CombatData.ImpactDamage, weapontype.CombatData.DamageType, target.GetGlobalPosition(), weapontype.DamageSpecialData);
    }

    public virtual void Dying(Engine engine, ActorInfo ainfo)
    {
#if DEBUG
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");
#endif

      DyingMoveData.Initialize(engine, ainfo);
      ainfo.DyingTimerStart();

      if (ainfo.IsPlayer)
      {
        engine.PlayerInfo.ActorID = -1;
        engine.PlayerCameraInfo.DeathLook.SetPosition_Actor(ainfo.ID, CameraData.DeathCamera);
        engine.PlayerCameraInfo.SetDeathLook();

        ainfo.TickEvents += engine.GameScenarioManager.Scenario.ProcessPlayerDying;
        ainfo.DestroyedEvents += engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
    }

    public virtual void Dead(Engine engine, ActorInfo ainfo)
    {
#if DEBUG
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");
#endif

      // Explode
      ainfo.TickExplosions();

      // Debris
      if (!ainfo.IsAggregateMode && !engine.Game.IsLowFPS())
        DebrisData.Process(engine, ainfo);

      if (ainfo.IsPlayer)
      {
        engine.PlayerInfo.ActorID = -1;

        engine.PlayerCameraInfo.DeathLook.SetPosition_Point(ainfo.GetGlobalPosition(), CameraData.DeathCamera);
        engine.PlayerCameraInfo.SetDeathLook();

        ainfo.DestroyedEvents += engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
      else
      {
        if (ainfo.UseParentCoords && ainfo.TopParent.IsPlayer)
          engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SUBSYSTEM_LOST).F(ainfo.Name)
                                           , 3
                                           , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_WARNING));
        else
        {
          ActorInfo p = PlayerInfo.Actor;
          if (p != null && !p.Squad.IsNull && ainfo.Squad == p.Squad && ainfo != p)
          {
            engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SQUAD_MEMBER_LOST).F(ainfo.Name)
                                             , 3
                                             , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_WARNING));
            engine.SoundManager.SetMood(MoodState.ALLY_FIGHTER_LOST);
          }
        }
      }
    }
  }
}
