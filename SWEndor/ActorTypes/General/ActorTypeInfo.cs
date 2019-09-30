using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo : ITypeInfo<ActorInfo>
  {
    public ActorTypeInfo(Factory owner, string name = "")
    {
      ActorTypeFactory = owner;
      if (name.Length > 0) { Name = name; }

      CombatData.Reset();
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }

    // Basic Info
    public string Name;

    // Data
    public ComponentMask Mask = ComponentMask.NONE;
    public float MaxStrength = 1.0f;
    public float ImpactDamage = 1.0f;
    public DamageType DamageType = DamageType.COLLISION;
    public bool IsLaser = false;

    // Data (structs)
    public RegenData RegenData;
    public CombatData CombatData;
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

    public void LoadFromINI(string name)
    {
      Name = name;
      string filepath = Path.Combine(Globals.ActorTypeINIDirectory, name + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        Mask = f.GetEnumValue("General", "Mask", Mask);
        MaxStrength = f.GetFloatValue("General", "MaxStrength", MaxStrength);
        ImpactDamage = f.GetFloatValue("General", "ImpactDamage", ImpactDamage);
        DamageType = f.GetEnumValue("General", "DamageType", DamageType);
        IsLaser = f.GetBoolValue("General", "IsLaser", IsLaser);
        Loadouts = f.GetStringList("General", "Loadouts", Loadouts);
        TrackerDummyWeapon = f.GetBoolValue("General", "TrackerDummyWeapon", TrackerDummyWeapon);

        RegenData.LoadFromINI(f, "RegenData");
        CombatData.LoadFromINI(f, "CombatData");
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
      else
      {
        File.Create(filepath).Close();
        INIFile f = new INIFile(filepath);

        f.SetEnumValue("General", "Mask", Mask);
        f.SetFloatValue("General", "MaxStrength", MaxStrength);
        f.SetFloatValue("General", "ImpactDamage", ImpactDamage);
        f.SetEnumValue("General", "DamageType", DamageType);
        f.SetBoolValue("General", "IsLaser", IsLaser);
        f.SetStringList("General", "Loadouts", Loadouts);
        f.SetBoolValue("General", "TrackerDummyWeapon", TrackerDummyWeapon);

        RegenData.SaveToINI(f, "RegenData");
        CombatData.SaveToINI(f, "CombatData");
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
    }

    public void Init()
    {
      cachedWeaponData.Load(this);
      MoveBehavior.Load(this);
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

      ainfo.TickExplosions();

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
    }

    public virtual void ProcessHit(Engine engine, ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      if (hitby.TypeInfo.ImpactDamage == 0)
        return;

      if (owner.IsDying 
        && owner.CombatData.HitWhileDyingLeadsToDeath)
        owner.SetState_Dead();
      
      if (hitby.Mask.Has(ComponentMask.IS_DAMAGE))
      {
        if (!owner.IsDyingOrDead)
        {
          float p_hp = owner.HP;
          owner.InflictDamage(hitby, hitby.TypeInfo.ImpactDamage, DamageType.NORMAL, impact);
          float hp = owner.HP;

          if (owner.IsPlayer)
            if (hp < (int)p_hp)
              PlayerInfo.FlashHit(PlayerInfo.StrengthColor);

          // scoring
          ActorInfo attacker = hitby.TopParent;
          if (attacker.IsScenePlayer)
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
            PlayerInfo.Score.AddDamage(engine, attacker, hitby.TypeInfo.ImpactDamage * owner.GetArmor(DamageType.NORMAL));

            if (owner.IsDyingOrDead)
              PlayerInfo.Score.AddDeath(engine, attacker);
          }

          if (attacker != null && !attacker.Faction.IsAlliedWith(owner.Faction))
          {
            // Fighter AI
            if ((owner.TypeInfo is Groups.Fighter))
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
                          a.QueueLast(new AttackActor(b.ID));
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
                        a.QueueLast(new AttackActor(attacker.ID));
                      }
                    }
                  }
                }
                else
                {
                  owner.ClearQueue();
                  owner.QueueLast(new AttackActor(attacker.ID));
                }
              }
              else if (owner.CanEvade && !(owner.CurrentAction is Evade))
              {
                owner.QueueFirst(new Evade());
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

        hitby.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
        hitby.SetState_Dead(); // projectiles die on impact
      }
      else if (owner.Mask.Has(ComponentMask.IS_DAMAGE))
      {
      }
      else
      {
        // Collision
        owner.InflictDamage(hitby, hitby.TypeInfo.ImpactDamage, DamageType.COLLISION, impact);
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
        if ((owner.TypeInfo is Groups.Fighter && owner.IsDyingOrDead))
        {
          owner.SetState_Dead();
          if (owner.IsScenePlayer)
            PlayerInfo.Score.AddDeath(engine, attacker);
        }
      }

      hitby.OnHitEvent(owner);
    }

    private void AddScore(Engine engine, ScoreInfo score, ActorInfo proj, ActorInfo victim)
    {
      if (!victim.IsDyingOrDead)
      {
        score.AddHit(engine, victim, proj.TypeInfo.ImpactDamage * victim.GetArmor(DamageType.NORMAL));
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

    public virtual void FireAggregation(ActorInfo owner, ActorInfo target, ActorTypeInfo weapontype)
    {
      float accuracy = 1;

      float d = ActorDistanceInfo.GetDistance(owner, target) / weapontype.MoveLimitData.MaxSpeed;
      TV_3DVECTOR angle = (target.GetGlobalPosition() - owner.GetGlobalPosition()).ConvertDirToRot() - owner.GetGlobalRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Random.NextDouble() < accuracy)
        target.InflictDamage(owner, weapontype.ImpactDamage, DamageType.NORMAL, target.GetGlobalPosition());
    }

    public virtual void Dying(Engine engine, ActorInfo ainfo)
    {
      if (ainfo == null)
        throw new ArgumentNullException("ainfo");

      DyingMoveData.Initialize(ainfo);

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
