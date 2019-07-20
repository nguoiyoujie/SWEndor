﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;
using SWEndor.Primitives.Traits;
using System.Collections.Generic;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo : IDisposable, INotifyDead, INotifyDying, INotifyDamage, INotifyAppliedDamage
  {
    public ActorTypeInfo(Factory owner, string name = "")
    {
      ActorTypeFactory = owner;
      if (name.Length > 0) { Name = name; }

      RegenData.Reset();
      //ExplodeData.Reset();
      CombatData.Reset();
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    //public ActorDataSet ActorDataSet { get { return Engine.ActorDataSet; } }
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
    public RegenData RegenData = new RegenData();

    public CombatData CombatData = new CombatData();
    public TimedLifeData TimedLifeData = new TimedLifeData();

    // Mesh Data
    public float Scale = 1;

    //Sys Data
    public float MaxStrength = 1.0f;

    //Combat Data
    public float ImpactDamage = 1.0f;

    //Move Data
    public float MaxSpeed = 0.0f;
    public float MinSpeed = 0.0f;
    public float MaxSpeedChangeRate = 0.0f;
    public float MaxTurnRate = 0.0f;
    public float MaxSecondOrderTurnRateFrac = 0.2f;
    public float XLimit = 75.0f;
    public float ZTilt = 0.0f;
    public float ZNormFrac = 0.025f;


    public bool EnableDistanceCull = true;
    public float CullDistance = 20000.0f;

    // AI
    public float Attack_AngularDelta = 5f;
    public float Attack_HighAccuracyAngularDelta = 1f;
    public float Move_CloseEnough = 500;

    public bool AggressiveTracker = false;
    public bool AlwaysAccurateRotation = false;

    public int Score_perStrength = 0;
    public int Score_DestroyBonus = 0;

    // AI
    public bool CanEvade = false;
    public bool CanRetaliate = false;
    public bool CanCheckCollisionAhead = false;

    // Targeting
    public TargetType TargetType = TargetType.NULL;
    public int HuntWeight = 1;

    // Meshs
    public string SourceMeshPath = "";
    public string SourceFarMeshPath = "";
    public TVMesh SourceMesh = null;
    public TVMesh SourceFarMesh = null;

    public TV_3DVECTOR max_dimensions = new TV_3DVECTOR();
    public TV_3DVECTOR min_dimensions = new TV_3DVECTOR();

    // Render
    public float RadarSize = 0;
    public RadarType RadarType = RadarType.NULL;
    public bool AlwaysShowInRadar = false;

    // Performance Savings
    public bool IsLaser = false;

    // Misc
    public bool IsExplosion = false;

    // AddOns
    public AddOnInfo[] AddOns = new AddOnInfo[0];

    // Explosionf
    public ExplodeInfo[] Explodes = new ExplodeInfo[0];

    // Debris
    public DebrisSpawnerInfo[] Debris = new DebrisSpawnerInfo[0];

    public ActorCameraInfo[] Cameras = new ActorCameraInfo[0];
    public DeathCameraInfo DeathCamera = new DeathCameraInfo(350, 25, 15);

    // Sound
    public SoundSourceInfo[] InitialSoundSources = new SoundSourceInfo[0];
    public SoundSourceInfo[] SoundSources = new SoundSourceInfo[0];


    public virtual void RegisterModel()
    {
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
        if (SourceMesh == null)
        {
          SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);
          //SourceMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_BUMPMAPPING_TANGENTSPACE, 8);

          if (SourceMeshPath.Length > 0)
            SourceMesh.LoadXFile(SourceMeshPath, true);
          SourceMesh.Enable(false);
          SourceMesh.WeldVertices();
          SourceMesh.ComputeBoundings();
        }
      }

      if (SourceFarMesh == null)
      {
        SourceFarMesh = TrueVision.TVGlobals.GetMesh(Name + "_far");
        if (SourceFarMesh == null)
        {
          if (SourceFarMeshPath.Length > 0)
          {
            SourceFarMesh = TrueVision.TVScene.CreateMeshBuilder(Name + "_far");
            SourceFarMesh.LoadXFile(SourceFarMeshPath, true);
            SourceFarMesh.Enable(false);
            SourceFarMesh.WeldVertices();
            SourceFarMesh.ComputeBoundings();
          }
          else
            SourceFarMesh = SourceMesh.Duplicate();
        }
      }
    }

    public virtual TVMesh GenerateMesh()
    {
      if (SourceMesh == null)
        throw new NotImplementedException("Attempted to generate empty model");

      SourceMesh.Enable(false);
      SourceMesh.SetCollisionEnable(false);

      TVMesh ret = SourceMesh.Duplicate();
      SourceMesh.GetBoundingBox(ref min_dimensions, ref max_dimensions);
      return ret;
    }

    public virtual TVMesh GenerateFarMesh()
    {
      if (SourceFarMesh == null)
        SourceFarMesh = SourceMesh.Duplicate();

      if (SourceFarMesh == null)
        throw new NotImplementedException("Attempted to generate empty model");

      SourceFarMesh.Enable(false);
      SourceFarMesh.SetCollisionEnable(false);

      TVMesh ret = SourceFarMesh.Duplicate();
      return ret;
    }

    public int LoadAlphaTexture(string name, string texpath, string alphatexpath = null)
    {
      int tex = TrueVision.TVGlobals.GetTex(name);
      if (tex == 0)
      {
        int texS = TrueVision.TVTextureFactory.LoadTexture(texpath);
        int texA = TrueVision.TVTextureFactory.LoadTexture(alphatexpath ?? texpath); //LoadAlphaTexture
        tex = TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, name);
      }
      return tex;
    }

    public int LoadTexture(string name, string texpath)
    {
      int tex = TrueVision.TVGlobals.GetTex(name);
      if (tex == 0)
        tex = TrueVision.TVTextureFactory.LoadTexture(texpath, name);
      return tex;
    }

    public virtual void Initialize(ActorInfo ainfo)
    {
      // AI
      ainfo.CanEvade = CanEvade;
      ainfo.CanRetaliate = CanRetaliate;

      if (!(GameScenarioManager.Scenario is GSMainMenu))
        foreach (SoundSourceInfo assi in InitialSoundSources)
          assi.Process(ainfo);
    }

    public void GenerateAddOns(ActorInfo ainfo)
    {
      foreach (AddOnInfo addon in AddOns)
        addon.Create(Engine, ainfo);
    }

    public virtual void ProcessState(ActorInfo ainfo)
    {
      // weapons
      foreach (WeaponInfo w in ainfo.WeaponSystemInfo.Weapons.Values)
        w.Reload(Engine);

      // regeneration
      RegenerationSystem.Process(Engine, ainfo, Game.TimeSinceRender);

      if (ainfo.StateModel.IsDying)
      {
        //ExplosionSystem.ProcessDying(Engine, ainfo.ID);
        ainfo.DyingMoveComponent?.Update(ainfo, ref ainfo.MoveData, Game.TimeSinceRender);
      }

      // sound
      if (PlayerInfo.Actor != null
        && ainfo.Active
        && !ainfo.IsScenePlayer
        && !(GameScenarioManager.Scenario is GSMainMenu))
      {
        foreach (SoundSourceInfo assi in SoundSources)
          assi.Process(ainfo);
      }
    }

    public virtual void ProcessNewState(ActorInfo ainfo)
    {

    }

    public virtual void ProcessHit(ActorInfo owner, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (owner == null || hitby == null)
        return;

      IStateModel s = owner.StateModel;
      IHealth h = owner.Health;

      if (s.IsDying
        && owner.CombatData.HitWhileDyingLeadsToDeath)
        s.MakeDead(owner);

      if (s.IsDyingOrDead
        || hitby.TypeInfo.ImpactDamage == 0)
        return;

      if (hitby.StateModel.ComponentMask.Has(ComponentMask.IS_DAMAGE))
      {
        float str0 = h.HP;
        h.InflictDamage(owner, new DamageInfo<ActorInfo>(hitby, hitby.TypeInfo.ImpactDamage, DamageType.NORMAL));
        float str1 = h.HP; 

        if (owner.IsPlayer)
        {
          if (str1 < (int)str0)
            PlayerInfo.FlashHit();
        }

        // scoring
        ActorInfo attacker = hitby.TopParent;
        if (attacker == PlayerInfo.Actor)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(PlayerInfo.Score, hitby, owner);
          else
            Screen2D.MessageText("{0}: {1}, watch your fire!".F(owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        if (owner == PlayerInfo.Actor)
        {
          PlayerInfo.Score.AddDamage(attacker, hitby.TypeInfo.ImpactDamage * owner.CombatData.DamageModifier);

          if (s.IsDyingOrDead)
            PlayerInfo.Score.AddDeath(attacker);
        }

        if (attacker != null && !attacker.Faction.IsAlliedWith(owner.Faction))
        {
          // Fighter AI
          if ((owner.TypeInfo is Groups.Fighter))
          {
            if (owner.CanRetaliate && (owner.CurrentAction == null || owner.CurrentAction.CanInterrupt))
            {
              owner.ClearQueue();
              owner.QueueLast(new AttackActor(attacker));
            }
            else if (owner.CanEvade && !(owner.CurrentAction is Evade))
            {
              owner.QueueFirst(new Evade());
            }
          }
        }

        hitby.Transform.Position = new TV_3DVECTOR(impact.x, impact.y, impact.z);
        hitby.StateModel.MakeDead(hitby); // projectiles die on impact
      }
      else if (owner.StateModel.ComponentMask.Has(ComponentMask.IS_DAMAGE))
      {
      }
      else
      {
        // Collision
        h.InflictDamage(owner, new DamageInfo<ActorInfo>(hitby, hitby.TypeInfo.ImpactDamage, DamageType.COLLISION));

        // Repulsion
        if (h.HP > 0
          && owner.StateModel.ComponentMask.Has(ComponentMask.CAN_MOVE)
          && owner.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
        {
          float repel = -owner.MoveData.Speed * 0.25f;
          owner.MoveRelative(repel);
        }

        ActorInfo attacker = hitby.TopParent;
        if (attacker == PlayerInfo.Actor)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(PlayerInfo.Score, attacker, owner);
          else
            Screen2D.MessageText("{0}: {1}, watch it!".F(owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        // Fighter Collision
        if ((owner.TypeInfo is Groups.Fighter && s.IsDyingOrDead))
        {
          owner.StateModel.MakeDead(owner);
          if (owner.IsPlayer)
            PlayerInfo.Score.AddDeath(attacker);
        }
      }

      hitby.OnHitEvent(owner.ID);
    }

    private void AddScore(ScoreInfo score, ActorInfo proj, ActorInfo victim)
    {
      if (victim.StateModel.IsDyingOrDead)
        score.AddKill(victim);
      else
        score.AddHit(victim, proj.TypeInfo.ImpactDamage * victim.CombatData.DamageModifier);
    }

    public void InterpretWeapon(ActorInfo owner, string sweapon, out WeaponInfo weapon, out int burst)
    {
      string s = "none";
      weapon = null;
      burst = 1;

      if (owner == null)
        return;

      int seperatorpos = sweapon.IndexOf(':');
      if (seperatorpos > -1)
      {
        s = sweapon.Substring(seperatorpos + 1).Trim();
        int.TryParse(sweapon.Substring(0, seperatorpos), out burst);
      }
      else
      {
        s = sweapon.Trim();
      }

      if (owner.WeaponSystemInfo.Weapons.ContainsKey(s))
      {
        weapon = owner.WeaponSystemInfo.Weapons[s];
      }
    }

    public virtual bool FireWeapon(ActorInfo owner, ActorInfo target, string sweapon)
    {
      if (owner == null)
        return false;

      WeaponInfo weapon = null;
      int burst = 0;

      // AI Determination
      if (sweapon == "auto")
      {
        foreach (string ws in owner.WeaponSystemInfo.AIWeapons)
        {
          if (FireWeapon(owner, target, ws))
            return true;
        }
      }
      else
      {
        InterpretWeapon(owner, sweapon, out weapon, out burst);

        if (weapon != null)
          return weapon.Fire(owner, target, burst);
      }
      
      return false;
    }

    public virtual void FireAggregation(ActorInfo owner, ActorInfo target, ActorTypeInfo weapontype)
    {
      float accuracy = 1;
      
      float d = ActorDistanceInfo.GetDistance(owner, target) / weapontype.MaxSpeed;
      TV_3DVECTOR angle = Utilities.GetRotation(target.Transform.Position - owner.Transform.Position) - owner.Transform.Rotation;
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Random.NextDouble() < accuracy)
        target.Health.InflictDamage(target, new DamageInfo<ActorInfo>(owner, weapontype.ImpactDamage, DamageType.NORMAL));
    }

    public void Dispose()
    {
    }

    public virtual void Dying<A1>(A1 self) where A1 : ITraitOwner
    {
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      ainfo.DyingMoveComponent?.Initialize(ainfo, ref ainfo.MoveData);

      if (ainfo.IsPlayer)
      {
        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.LookActor = ainfo.ID;
        PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);

        if (ainfo.StateModel.IsDying)
          ainfo.TickEvents += GameScenarioManager.Scenario.ProcessPlayerDying;

        ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
    }

    public virtual void Dead<A1>(A1 self) where A1 : ITraitOwner
    {
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      // Debris
      if (!ainfo.IsAggregateMode && !Game.IsLowFPS())
        foreach (DebrisSpawnerInfo ds in Debris)
          ds.Process(ainfo);

      if (ainfo.IsPlayer)
      {
        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.LookActor = ainfo.ID;
        PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);
        ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
      }
    }

    public virtual void Damaged<A1, A2>(A1 self, DamageInfo<A2> e)
      where A1 : ITraitOwner
      where A2 : ITraitOwner
    {

    }

    public virtual void AppliedDamage<A1, A2>(A2 attacker, A1 target, DamageInfo<A2> e)
      where A1 : ITraitOwner
      where A2 : ITraitOwner
    {

    }
  }
}
