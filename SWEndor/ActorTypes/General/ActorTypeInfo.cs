using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Primitives.Extensions;
using SWEndor.Projectiles;
using SWEndor.ProjectileTypes;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo : ITypeInfo<ActorInfo>
  {
    public static ActorTypeInfo Null = new ActorTypeInfo(Globals.Engine.ActorTypeFactory, "$NULL", "Null");

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
    public string Name;

    // Data
    public ComponentMask Mask = ComponentMask.NONE;

    // Data (structs)
    public SystemData SystemData;
    public CombatData CombatData;
    public RegenData RegenData;
    public TimedLifeData TimedLifeData;
    public ArmorData ArmorData;
    public MoveLimitData MoveLimitData = MoveLimitData.Default;
    public RenderData RenderData = RenderData.Default;
    public AIData AIData = AIData.Default;
    public MeshData MeshData = MeshData.Default;
    public DyingMoveData DyingMoveData;
    public ScoreData ScoreData;
    
    // AddOns
    public AddOnData[] AddOns = new AddOnData[0];

    // Explosionf
    public ExplodeData[] Explodes = new ExplodeData[0];

    // Weapons
    public string[] Loadouts = new string[0];
    public bool TrackerDummyWeapon = false;

    // Debris
    public DebrisSpawnerData[] Debris = new DebrisSpawnerData[0];

    public LookData[] Cameras = new LookData[0];
    public DeathCameraData DeathCamera = new DeathCameraData(350, 25, 15);

    // Sound
    public SoundSourceData[] InitialSoundSources = new SoundSourceData[0];
    public SoundSourceData[] SoundSources = new SoundSourceData[0];

    // derived
    public MoveBehavior MoveBehavior;
    internal UnfixedWeaponData cachedWeaponData;

    public void LoadFromINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ActorTypeINIDirectory, id + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Name = f.GetStringValue("General", "Name", Name);
        Mask = f.GetEnumValue("General", "Mask", Mask);

        Loadouts = f.GetStringList("General", "Loadouts", Loadouts);
        TrackerDummyWeapon = f.GetBoolValue("General", "TrackerDummyWeapon", TrackerDummyWeapon);

        CombatData.LoadFromINI(f, "CombatData");
        RegenData.LoadFromINI(f, "RegenData");
        TimedLifeData.LoadFromINI(f, "TimedLifeData");
        ArmorData.LoadFromINI(f, "ArmorData");
        MoveLimitData.LoadFromINI(f, "MoveLimitData");
        RenderData.LoadFromINI(f, "RenderData");
        AIData.LoadFromINI(f, "AIData");
        MeshData.LoadFromINI(f, "MeshData");
        DyingMoveData.LoadFromINI(f, "DyingMoveData");
        ScoreData.LoadFromINI(f, "ScoreData");

        AddOnData.LoadFromINI(f, "AddOnData", "AddOns", out AddOns);
        ExplodeData.LoadFromINI(f, "ExplodeData", "Explodes", out Explodes);
        DebrisSpawnerData.LoadFromINI(f, "DebrisSpawnerData", "Debris", out Debris);
        LookData.LoadFromINI(f, "Cameras", "Cameras", out Cameras);
        DeathCamera.LoadFromINI(f, "DeathCamera");
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "InitialSoundSources", out InitialSoundSources);
        SoundSourceData.LoadFromINI(f, "SoundSourceData", "SoundSources", out SoundSources);
      }
    }

    public void SaveToINI(string id)
    {
      ID = id;
      string filepath = Path.Combine(Globals.ActorTypeINIDirectory, id + ".ini");

      if (!File.Exists(filepath))
        File.Create(filepath).Close(); INIFile f = new INIFile(filepath);

      f.SetStringValue("General", "Name", Name);
      f.SetEnumValue("General", "Mask", Mask);

      f.SetStringList("General", "Loadouts", Loadouts);
      f.SetBoolValue("General", "TrackerDummyWeapon", TrackerDummyWeapon);

      CombatData.SaveToINI(f, "CombatData");
      RegenData.SaveToINI(f, "RegenData");
      TimedLifeData.SaveToINI(f, "TimedLifeData");
      ArmorData.SaveToINI(f, "ArmorData");
      MoveLimitData.SaveToINI(f, "MoveLimitData");
      RenderData.SaveToINI(f, "RenderData");
      AIData.SaveToINI(f, "AIData");
      MeshData.SaveToINI(f, "MeshData");
      DyingMoveData.SaveToINI(f, "DyingMoveData");
      ScoreData.SaveToINI(f, "ScoreData");

      AddOnData.SaveToINI(f, "AddOnData", "AddOns", "ADD", AddOns);
      ExplodeData.SaveToINI(f, "ExplodeData", "Explodes", "EXP", Explodes);
      LookData.SaveToINI(f, "Cameras", "Cameras", "CAM", Cameras);
      DeathCamera.SaveToINI(f, "DeathCamera");
      SoundSourceData.SaveToINI(f, "SoundSourceData", "InitialSoundSources", "ISN", InitialSoundSources);
      SoundSourceData.SaveToINI(f, "SoundSourceData", "SoundSources", "SND", SoundSources);
      f.SaveFile(filepath);
    }

    public void Init()
    {
      cachedWeaponData.Load(Engine, this);
      MoveBehavior.Load(this);
      if (SystemData.AllowSystemDamage && SystemData.Parts.Length == 0)
        SystemData.AutoParts(this);
    }

    public virtual void Initialize(Engine engine, ActorInfo ainfo)
    {
      // AI
      ainfo.CanEvade = AIData.CanEvade;
      ainfo.CanRetaliate = AIData.CanRetaliate;

      // Sound
      foreach (SoundSourceData assi in InitialSoundSources)
        assi.Process(engine, ainfo);
    }

    public void GenerateAddOns(Engine engine, ActorInfo ainfo)
    {
      foreach (AddOnData addon in AddOns)
        addon.Create(engine, ainfo);
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
        foreach (SoundSourceData assi in SoundSources)
          assi.Process(engine, ainfo);
      }

      // projectile
      if (ainfo.TypeInfo.AIData.TargetType.Contains(TargetType.MUNITION)
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
          if (target.TypeInfo.AIData.TargetType.Contains(TargetType.MUNITION))
            impdist += target.TypeInfo.CombatData.ImpactCloseEnoughDistance;

          // Anticipate
          float dist = DistanceModel.GetDistance(engine, proj, target, impdist + 1);

          if (dist < impdist)
          {
            target.TypeInfo.ProcessHit(engine, target, proj, target.GetGlobalPosition(), new TV_3DVECTOR());
            proj.TypeInfo.ProcessHit(engine, proj, target, target.GetGlobalPosition(), new TV_3DVECTOR());

            proj.OnHitEvent(target);
            target.OnHitEvent(proj);
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

      if (owner.IsDying
        && owner.TypeInfo.CombatData.HitWhileDyingLeadsToDeath)
        owner.SetState_Dead();

      if (owner.TypeInfo.AIData.TargetType.Has(TargetType.MUNITION))
      {
        // do nothing
      }
      else if (hitby.TypeInfo.AIData.TargetType.Has(TargetType.MUNITION))
      {
        ProcessHit(engine, owner, hitby.TopParent, hitby.TypeInfo.CombatData, impact);
        hitby.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
        hitby.SetState_Dead(); // projectiles die on impact
      }
      else
      {
        // Collision
        owner.InflictDamage(hitby.TypeInfo.CombatData.ImpactDamage, hitby.TypeInfo.CombatData.DamageType, impact);
        if (owner.HP > 0
          && owner.Mask.Has(ComponentMask.CAN_MOVE)
          && owner.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
        {
          float repel = -owner.MoveData.Speed * 0.25f;
          owner.MoveRelative(repel, 0, 0);
        }

        ActorInfo attacker = hitby.TopParent;
        if (attacker.IsScenePlayer)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(engine, PlayerInfo.Score, attacker, owner);
          else
            Engine.Screen2D.MessageText(string.Format("{0}: {1}, watch it!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        // Fighter Collision
        if (owner.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER)
          && owner.IsDyingOrDead)
        {
          owner.SetState_Dead();
          if (owner.IsScenePlayer)
            PlayerInfo.Score.AddDeath(engine, attacker);
        }

        hitby.OnHitEvent(owner);
      }
    }

    public void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, CombatData projData, TV_3DVECTOR impact)
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
        if (owner.TypeInfo.AIData.TargetType.Has(TargetType.MUNITION))
          owner.SetState_Dead(); // projectiles die on impact
        else
        {
          float p_hp = owner.HP;
          owner.InflictDamage(projData.ImpactDamage, projData.DamageType, impact);
          float hp = owner.HP;

          if (owner.IsPlayer)
            if (hp < (int)p_hp)
              PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
        }

        // scoring
        ActorInfo attacker = hitby?.TopParent;
        if (attacker != null && attacker.IsScenePlayer)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(engine, PlayerInfo.Score, hitby, owner);
          else
            Engine.Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        if (owner.IsScenePlayer)
        {
          PlayerInfo.Score.AddDamage(engine, attacker, projData.ImpactDamage * owner.GetArmor(projData.DamageType));

          if (owner.IsDyingOrDead)
            PlayerInfo.Score.AddDeath(engine, attacker);
        }

        if (attacker != null && !attacker.Faction.IsAlliedWith(owner.Faction))
        {
          // Fighter AI
          if ((owner.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER)))
          {
            if (owner.CanRetaliate && (owner.CurrentAction == null || owner.CurrentAction.CanInterrupt))
            {
              if (!owner.Squad.IsNull && owner.Squad.Mission == null)
              {
                if (!attacker.Squad.IsNull)
                {
                  foreach (ActorInfo a in owner.Squad.Members)
                  {
                    if (a.CanRetaliate && (a.CurrentAction == null || a.CurrentAction.CanInterrupt))
                    {
                      ActorInfo b = attacker.Squad.MembersCopy.Random(engine);
                      if (b != null)
                      {
                        a.ClearQueue();
                        a.QueueLast(AttackActor.GetOrCreate(b.ID));
                      }
                    }
                  }
                }
                else
                {
                  foreach (ActorInfo a in owner.Squad.Members)
                  {
                    if (a.CanRetaliate && (a.CurrentAction == null || a.CurrentAction.CanInterrupt))
                    {
                      a.ClearQueue();
                      a.QueueLast(AttackActor.GetOrCreate(attacker.ID));
                    }
                  }
                }
              }
              else
              {
                owner.ClearQueue();
                owner.QueueLast(AttackActor.GetOrCreate(attacker.ID));
              }
            }
            else if (owner.CanEvade && !(owner.CurrentAction is Evade))
            {
              owner.QueueFirst(Evade.GetOrCreate());
            }

            if (!owner.Squad.IsNull)
            {
              if (owner.Squad.Leader == owner)
                owner.Squad.AddThreat(attacker, true);
              else
                owner.Squad.AddThreat(attacker);
            }
          }
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
      ProcessHit(engine, owner, proj.Owner, proj.TypeInfo.CombatData, impact);
      proj.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
      proj.SetState_Dead(); // projectiles die on impact
    }

    private void AddScore(Engine engine, ScoreInfo score, ActorInfo proj, ActorInfo victim)
    {
      if (!victim.IsDyingOrDead)
      {
        score.AddHit(engine, victim, proj.TypeInfo.CombatData.ImpactDamage * victim.GetArmor(proj.TypeInfo.CombatData.DamageType));
      }

      if (victim.IsDyingOrDead)
      {
        score.AddKill(engine, victim);
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
      TV_3DVECTOR angle = (target.GetGlobalPosition() - owner.GetGlobalPosition()).ConvertDirToRot() - owner.GetGlobalRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Random.NextDouble() < accuracy)
        target.InflictDamage(weapontype.CombatData.ImpactDamage, weapontype.CombatData.DamageType, target.GetGlobalPosition());
    }

    public virtual void Dying(Engine engine, ActorInfo ainfo)
    {
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");

      DyingMoveData.Initialize(ainfo);
      ainfo.DyingTimerStart();

      if (ainfo.IsPlayer)
      {
        engine.PlayerInfo.ActorID = -1;
        engine.PlayerCameraInfo.Look.SetPosition_Actor(ainfo.ID);
        engine.PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);

        ainfo.TickEvents += engine.GameScenarioManager.Scenario.ProcessPlayerDying;
        ainfo.DestroyedEvents += engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
    }

    public virtual void Dead(Engine engine, ActorInfo ainfo)
    {
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");

      // Explode
      ainfo.TickExplosions();

      // Debris
      if (!ainfo.IsAggregateMode && !engine.Game.IsLowFPS())
        foreach (DebrisSpawnerData ds in Debris)
          ds.Process(engine, ainfo);

      if (ainfo.IsPlayer)
      {
        engine.PlayerInfo.ActorID = -1;
        engine.PlayerCameraInfo.Look.SetPosition_Actor(ainfo.ID);
        engine.PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);
        ainfo.DestroyedEvents += engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
      else
      {
        if (ainfo.UseParentCoords && ainfo.TopParent.IsPlayer)
          engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SUBSYSTEM_LOST).F(ainfo.Name), 3, new TV_COLOR(1, 0.2f, 0.2f, 1));
        else
        {
          ActorInfo p = PlayerInfo.Actor;
          if (p != null && !p.Squad.IsNull && ainfo.Squad == p.Squad && ainfo != p)
          {
            engine.Screen2D.MessageSystemsText(TextLocalization.Get(TextLocalKeys.SQUAD_MEMBER_LOST).F(ainfo.Name), 3, new TV_COLOR(1, 0.2f, 0.2f, 1));
            engine.GameScenarioManager.Scenario.Mood = MoodStates.ALLY_FIGHTER_LOST;
          }
        }
      }
    }
  }
}
