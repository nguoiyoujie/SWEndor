﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.FileFormat.INI;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo
  {
    public ActorTypeInfo(Factory owner, string name = "")
    {
      ActorTypeFactory = owner;
      if (name.Length > 0) { Name = name; }

      CombatData.Reset();
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorDataSet ActorDataSet { get { return Engine.ActorDataSet; } }
    public ActorInfo.Factory ActorFactory { get { return Engine.ActorFactory; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

    // Basic Info
    public string Name;

    // Data
    public ComponentMask Mask = ComponentMask.NONE;
    public RegenData RegenData;
    public CombatData CombatData;
    public TimedLifeData TimedLifeData;
    public ArmorData ArmorData;
    public MoveLimitData MoveLimitData = MoveLimitData.Default;
    public RenderData RenderData = RenderData.Default;
    public AIData AIData = AIData.Default;
    public ScoreData ScoreData;
    
    // Mesh Data
    public float Scale = 1;

    //Sys Data
    public float MaxStrength = 1.0f;

    //Combat Data
    public float ImpactDamage = 1.0f;
    public DamageType DamageType = DamageType.COLLISION;


    public MeshData MeshData = MeshData.Default;

    // Performance Savings
    public bool IsLaser = false;

    // AddOns
    public AddOnData[] AddOns = new AddOnData[0];

    // Explosionf
    public ExplodeInfo[] Explodes = new ExplodeInfo[0];

    // Weapons
    public string[] Loadouts = new string[0];
    public bool TrackerDummyWeapon = false;
    internal WeaponData cacheWeapon;

    // Debris
    public DebrisSpawnerData[] Debris = new DebrisSpawnerData[0];

    public LookData[] Cameras = new LookData[0];
    public DeathCameraData DeathCamera = new DeathCameraData(350, 25, 15);

    // Sound
    public SoundSourceData[] InitialSoundSources = new SoundSourceData[0];
    public SoundSourceData[] SoundSources = new SoundSourceData[0];

    public void LoadFromINI()
    {
      string filepath = Path.Combine(Globals.ActorTypeINIDirectory, Name + ".ini");

      if (File.Exists(filepath))
      {
        INIFile f = new INIFile(filepath);
        RenderData.LoadFromINI(f, "RenderData");
        AIData.LoadFromINI(f, "AIData");
        ScoreData.LoadFromINI(f, "ScoreData");
        RegenData = new RegenData(f, "RegenData");
        MoveLimitData.LoadFromINI(f, "MoveLimitData");
      }
      else
      {
        File.Create(filepath).Close();
        INIFile f = new INIFile(filepath);
        RenderData.SaveToINI(f, "RenderData");
        AIData.SaveToINI(f, "AIData");
        ScoreData.SaveToINI(f, "ScoreData");
        RegenData.SaveToINI(f, "RegenData");
        MoveLimitData.SaveToINI(f, "MoveLimitData");
        f.SaveFile(filepath);
      }
    }

    public void Init()
    {
      cacheWeapon.Init(new UnfixedWeaponData(this));
    }

    public virtual void Initialize(ActorInfo ainfo)
    {
      // AI
      ainfo.CanEvade = AIData.CanEvade;
      ainfo.CanRetaliate = AIData.CanRetaliate;

      // Sound
      if (!(GameScenarioManager?.Scenario is GSMainMenu))
        foreach (SoundSourceData assi in InitialSoundSources)
          assi.Process(ainfo);
    }

    public void GenerateAddOns(ActorInfo ainfo)
    {
      foreach (AddOnData addon in AddOns)
        addon.Create(Engine, ainfo);
    }

    public virtual void ProcessState(ActorInfo ainfo)
    {
      // weapons
      foreach (WeaponInfo w in ainfo.WeaponSystemInfo.Weapons)
        w.Reload(Engine);

      // regeneration
      ainfo.Regenerate(Game.TimeSinceRender);

      ainfo.TickExplosions();

      if (ainfo.IsDying)
        ainfo.DyingMoveComponent?.Update(ainfo, ref ainfo.MoveData, Game.TimeSinceRender);

      // sound
      if (PlayerInfo.Actor != null
        && ainfo.Active 
        && !ainfo.IsScenePlayer
        && !(GameScenarioManager?.Scenario is GSMainMenu))
      {
        foreach (SoundSourceData assi in SoundSources)
          assi.Process(ainfo);
      }
    }

    public virtual void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      if (hitby.TypeInfo.ImpactDamage == 0)
        return;

      if (owner.IsDying 
        && ActorDataSet.CombatData[owner.dataID].HitWhileDyingLeadsToDeath)
        owner.SetState_Dead();
      
      if (Engine.MaskDataSet[hitby].Has(ComponentMask.IS_DAMAGE))
      {
        if (!owner.IsDyingOrDead)
        {
          float p_hp = owner.HP;
          owner.InflictDamage(hitby, hitby.TypeInfo.ImpactDamage, DamageType.NORMAL, impact);
          float hp = owner.HP;

          if (owner.IsPlayer)
            if (hp < (int)p_hp)
              PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
        }

        // scoring
        ActorInfo attacker = hitby.TopParent;
        if (attacker.IsScenePlayer)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(PlayerInfo.Score, hitby, owner);
          else
            Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        if (owner.IsScenePlayer)
        {
          PlayerInfo.Score.AddDamage(Engine, attacker, hitby.TypeInfo.ImpactDamage * owner.GetArmor(DamageType.NORMAL));

          if (owner.IsDyingOrDead)
            PlayerInfo.Score.AddDeath(Engine, attacker);
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
                      ActorInfo b = attacker.Squad.Members.ToArray().Random(Engine);
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

        hitby.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
        hitby.SetState_Dead(); // projectiles die on impact
      }
      else if (Engine.MaskDataSet[owner].Has(ComponentMask.IS_DAMAGE))
      {
      }
      else
      {
        // Collision
        owner.InflictDamage(hitby, hitby.TypeInfo.ImpactDamage, DamageType.COLLISION, impact);
        if (owner.HP > 0
          && Engine.MaskDataSet[owner].Has(ComponentMask.CAN_MOVE)
          && owner.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
        {
          float repel = -owner.MoveData.Speed * 0.25f;
          owner.MoveRelative(repel, 0, 0);
        }

        ActorInfo attacker = hitby.TopParent;
        if (attacker.IsScenePlayer)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(PlayerInfo.Score, attacker, owner);
          else
            Screen2D.MessageText(string.Format("{0}: {1}, watch it!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        // Fighter Collision
        if ((owner.TypeInfo is Groups.Fighter && owner.IsDyingOrDead))
        {
          owner.SetState_Dead();
          if (owner.IsScenePlayer)
            PlayerInfo.Score.AddDeath(Engine, attacker);
        }
      }

      hitby.OnHitEvent(owner);
    }

    private void AddScore(ScoreInfo score, ActorInfo proj, ActorInfo victim)
    {
      if (!victim.IsDyingOrDead)
      {
        score.AddHit(Engine, victim, proj.TypeInfo.ImpactDamage * victim.GetArmor(DamageType.NORMAL));
      }

      if (victim.IsDyingOrDead)
      {
        score.AddKill(Engine, victim);
      }
    }

    public virtual bool FireWeapon(ActorInfo owner, ActorInfo target, WeaponShotInfo sweapon)
    {
      if (owner == null)
        return false;

      // AI Determination
      if (EqualityComparer<WeaponShotInfo>.Default.Equals(sweapon, WeaponShotInfo.Automatic))
      {
        foreach (WeaponShotInfo ws in owner.WeaponSystemInfo.AIWeapons)
          if (FireWeapon(owner, target, ws))
            return true;
      }
      else
      {
        return sweapon.Fire(Engine, owner, target);
      }
      
      return false;
    }

    public virtual void FireAggregation(ActorInfo owner, ActorInfo target, ActorTypeInfo weapontype)
    {
      float accuracy = 1;

      float d = ActorDistanceInfo.GetDistance(owner, target) / weapontype.MoveLimitData.MaxSpeed;
      TV_3DVECTOR angle = Utilities.GetRotation(target.GetGlobalPosition() - owner.GetGlobalPosition()) - owner.GetGlobalRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Random.NextDouble() < accuracy)
        target.InflictDamage(owner, weapontype.ImpactDamage, DamageType.NORMAL, target.GetGlobalPosition());
    }

    public virtual void Dying(ActorInfo ainfo)
    {
      if (ainfo == null)
        return;

      ainfo.DyingMoveComponent?.Initialize(ainfo, ref ainfo.MoveData);

      if (ainfo.IsPlayer)
      {
        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.Look.SetPosition_Actor(ainfo.ID);
        PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);

        ainfo.TickEvents += GameScenarioManager.Scenario.ProcessPlayerDying;
        ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
    }

    public virtual void Dead(ActorInfo ainfo)
    {
      if (ainfo == null)
        return;

      // Explode
      ainfo.TickExplosions();

      // Debris
      if (!ainfo.IsAggregateMode && !Game.IsLowFPS())
        foreach (DebrisSpawnerData ds in Debris)
          ds.Process(ainfo);

      if (ainfo.IsPlayer)
      {
        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.Look.SetPosition_Actor(ainfo.ID);
        PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);
        ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
      else
      {
        if (ainfo.UseParentCoords && ainfo.TopParent.IsPlayer)
          Screen2D.MessageSystemsText("WARNING: Subsystem [{0}] lost.".F(ainfo.Name), 3, new TV_COLOR(1, 0.2f, 0.2f, 1));
      }
    }
  }
}
