using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo : IDisposable
  {
    public ActorTypeInfo(Factory owner, string name = "")
    {
      ActorTypeFactory = owner;
      if (name.Length > 0) { Name = name; }

      RegenData.Reset();
      ExplodeData.Reset();
      CombatData.Reset();
      TimedLifeData.Reset();
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorDataSet ActorDataSet { get { return Engine.ActorDataSet; } }
    public ActorInfo.Factory ActorFactory { get { return Engine.ActorFactory; } }
    public ActionManager ActionManager { get { return Engine.ActionManager; } }
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
    public ExplodeData ExplodeData = new ExplodeData();
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
      RegenerationSystem.Process(Engine, ainfo.ID, Game.TimeSinceRender);

      if (ainfo.ActorState.IsDying())
      {
        ExplosionSystem.ProcessDying(Engine, ainfo.ID);
        ainfo.DyingMoveComponent?.Update(ainfo, ref ainfo.MoveData, Game.TimeSinceRender);
      }

      // sound
      if (PlayerInfo.Actor != null
        && ainfo.CreationState == CreationState.ACTIVE 
        && !ActorInfo.IsScenePlayer(Engine, ainfo.ID)
        && !(GameScenarioManager.Scenario is GSMainMenu))
      {
        foreach (SoundSourceInfo assi in SoundSources)
          assi.Process(ainfo);
      }
    }

    public virtual void ProcessNewState(ActorInfo ainfo)
    {
      switch (ainfo.ActorState)
      {
        case ActorState.DEAD:
          // Explode
          ExplosionSystem.OnDeath(Engine, ainfo.ID);

          // Debris
          if (!ActorInfo.IsAggregateMode(Engine, ainfo.ID) && !Game.IsLowFPS())
            foreach (DebrisSpawnerInfo ds in Debris)
              ds.Process(ainfo);
          break;

        case ActorState.DYING:
          ainfo.DyingMoveComponent?.Initialize(ainfo, ref ainfo.MoveData);
          break;
      }

      if (ainfo.ActorState.IsDyingOrDead())
      {
        if (ActorInfo.IsPlayer(Engine, ainfo.ID))
        {
          PlayerInfo.ActorID = -1;
          PlayerCameraInfo.LookActor = ainfo.ID;
          PlayerCameraInfo.Look.SetModeDeathCircle(DeathCamera);

          if (ainfo.ActorState.IsDying())
            ainfo.TickEvents += GameScenarioManager.Scenario.ProcessPlayerDying;

          ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
        }
      }
    }

    public virtual void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      ActorInfo hitby = ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (owner.ActorState.IsDying() 
        && ActorDataSet.CombatData[ActorFactory.GetIndex(ownerActorID)].HitWhileDyingLeadsToDeath)
        owner.ActorState = ActorState.DEAD;

      if (owner.ActorState.IsDyingOrDead()
        || hitby.TypeInfo.ImpactDamage == 0)
        return;

      if (Engine.MaskDataSet[hitbyActorID].Has(ComponentMask.IS_DAMAGE))
      {
        float str0 = Engine.SysDataSet.Strength_get(ownerActorID);
        CombatSystem.onNotify(Engine, ownerActorID, CombatEventType.DAMAGE, hitby.TypeInfo.ImpactDamage);
        float str1 = Engine.SysDataSet.Strength_get(ownerActorID);

        if (ActorInfo.IsPlayer(Engine, owner.ID))
        {
          if (str1 < (int)str0)
            PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
        }

        // scoring
        int attackerID = hitby.TopParent;
        ActorInfo attacker = ActorFactory.Get(attackerID);
        if (attacker == PlayerInfo.Actor)
        {
          if (!attacker.Faction.IsAlliedWith(owner.Faction))
            AddScore(PlayerInfo.Score, hitby, owner);
          else
            Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", owner.Name, PlayerInfo.Name)
                                            , 5
                                            , owner.Faction.Color
                                            , -1);
        }

        if (owner == PlayerInfo.Actor)
        {
          PlayerInfo.Score.AddDamage(attacker, hitby.TypeInfo.ImpactDamage * ActorDataSet.CombatData[ActorFactory.GetIndex(ownerActorID)].DamageModifier);

          if (owner.ActorState.IsDyingOrDead())
            PlayerInfo.Score.AddDeath(attacker);
        }

        if (attacker != null && !attacker.Faction.IsAlliedWith(owner.Faction))
        {
          // Fighter AI
          if ((owner.TypeInfo is Groups.Fighter))
          {
            if (owner.CanRetaliate && (owner.CurrentAction == null || owner.CurrentAction.CanInterrupt))
            {
              ActionManager.ClearQueue(ownerActorID);
              ActionManager.QueueLast(ownerActorID, new AttackActor(attackerID));
            }
            else if (owner.CanEvade && !(owner.CurrentAction is Evade))
            {
              ActionManager.QueueFirst(ownerActorID, new Evade());
            }
          }
        }

        hitby.SetLocalPosition(impact.x, impact.y, impact.z);
        hitby.ActorState = ActorState.DYING;
      }
      else if (Engine.MaskDataSet[ownerActorID].Has(ComponentMask.IS_DAMAGE))
      {
      }
      else
      {
        // Collision
        CombatSystem.onNotify(Engine, ownerActorID, CombatEventType.COLLISIONDAMAGE, hitby.TypeInfo.ImpactDamage);
        if (Engine.SysDataSet.Strength_get(ownerActorID) > 0
          && Engine.MaskDataSet[ownerActorID].Has(ComponentMask.CAN_MOVE)
          && owner.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
        {
          float repel = -owner.MoveData.Speed * 0.25f;
          owner.MoveRelative(repel, 0, 0);
        }

        int attackerID = hitby.TopParent;
        ActorInfo attacker = ActorFactory.Get(attackerID);
        if (attacker == PlayerInfo.Actor)
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
        if ((owner.TypeInfo is Groups.Fighter && owner.ActorState.IsDyingOrDead()))
        {
          owner.ActorState = ActorState.DEAD;
          if (ActorInfo.IsPlayer(Engine, owner.ID))
            PlayerInfo.Score.AddDeath(attacker);
        }
      }

      hitby.OnHitEvent(owner.ID);
    }

    private void AddScore(ScoreInfo score, ActorInfo proj, ActorInfo victim)
    {
      if (!victim.ActorState.IsDyingOrDead())
      {
        score.AddHit(victim, proj.TypeInfo.ImpactDamage * ActorDataSet.CombatData[victim.dataID].DamageModifier);
      }

      if (victim.ActorState.IsDyingOrDead())
      {
        score.AddKill(victim);
      }
    }

    public void InterpretWeapon(int ownerActorID, string sweapon, out WeaponInfo weapon, out int burst)
    {
      string s = "none";
      weapon = null;
      burst = 1;

      ActorInfo owner = ActorFactory.Get(ownerActorID);
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

    public virtual bool FireWeapon(int ownerActorID, int targetActorID, string sweapon)
    {
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      if (owner == null)
        return false;

      WeaponInfo weapon = null;
      int burst = 0;

      // AI Determination
      if (sweapon == "auto")
      {
        foreach (string ws in owner.WeaponSystemInfo.AIWeapons)
        {
          if (FireWeapon(ownerActorID, targetActorID, ws))
            return true;
        }
      }
      else
      {
        InterpretWeapon(ownerActorID, sweapon, out weapon, out burst);

        if (weapon != null)
          return weapon.Fire(Engine, ownerActorID, targetActorID, burst);
      }
      
      return false;
    }

    public virtual void FireAggregation(int ownerActorID, int targetActorID, ActorTypeInfo weapontype)
    {
      float accuracy = 1;
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      ActorInfo target = ActorFactory.Get(targetActorID);
      
      float d = ActorDistanceInfo.GetDistance(ownerActorID, targetActorID) / weapontype.MaxSpeed;
      TV_3DVECTOR angle = Utilities.GetRotation(target.GetPosition() - owner.GetPosition()) - owner.GetRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Random.NextDouble() < accuracy)
        CombatSystem.onNotify(Engine, targetActorID, CombatEventType.DAMAGE, weapontype.ImpactDamage);
    }

    public void Dispose()
    {
    }
  }
}
