using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;

namespace SWEndor.ActorTypes
{
  public partial class ActorTypeInfo : IDisposable
  {
    public ActorTypeInfo(Factory owner, string name = "")
    {
      ActorTypeFactory = owner;
      if (name.Length > 0) { _name = name; }
      _counter = counter++;
    }

    public readonly Factory ActorTypeFactory;
    public Engine Engine { get { return ActorTypeFactory.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorInfo.Factory ActorFactory { get { return Engine.ActorFactory; } }
    public ActionManager ActionManager { get { return Engine.ActionManager; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

    // Basic Info
    private string _name = "New ActorType";
    private long _counter = 0;
    private static long counter = 0;
    public string Key { get { return _name + " " + _counter; } }

    public string Name
    {
      get { return _name; }
      set { _name = value; _counter = counter++; }
    }

    // Combat
    public bool CollisionEnabled = false;
    public bool IsCombatObject = false;
    public bool IsSelectable = false;
    public bool IsDamage = false;
    public float MaxStrength = 1.0f;
    public float ImpactDamage = 1.0f;

    public bool OnTimedLife = false;
    public float TimedLife = 100;
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

    public float Attack_AngularDelta = 5f;
    public float Attack_HighAccuracyAngularDelta = 1f;
    public float Move_CloseEnough = 500;

    public bool AggressiveTracker = false;
    public bool AlwaysAccurateRotation = false;

    public int Score_perStrength = 0;
    public int Score_DestroyBonus = 0;

    // Movement
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
    public bool AlwaysShowInRadar = false;

    // Performance Savings
    public bool NoRender = false;
    public bool NoProcess = false;
    public bool NoMove = false;
    public bool NoRotate = false;
    public bool IsLaser = false;
    public bool NoAI = false;

    // Misc
    public bool IsExplosion = false;

    // AddOns
    public AddOnInfo[] AddOns = new AddOnInfo[0];

    // Debris
    public DebrisSpawnerInfo[] Debris = new DebrisSpawnerInfo[0];

    // Sound
    public SoundSourceInfo[] InitialSoundSources = new SoundSourceInfo[0];
    public SoundSourceInfo[] SoundSources = new SoundSourceInfo[0];


    public virtual void RegisterModel()
    {
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVGlobals.GetMesh(_name);
        if (SourceMesh == null)
        {
          SourceMesh = TrueVision.TVScene.CreateMeshBuilder(_name);
          if (SourceMeshPath.Length > 0)
            SourceMesh.LoadXFile(SourceMeshPath, true);
          SourceMesh.Enable(false);
          SourceMesh.WeldVertices();
          SourceMesh.ComputeBoundings();
        }
      }

      if (SourceFarMesh == null)
      {
        SourceFarMesh = TrueVision.TVGlobals.GetMesh(_name + "_far");
        if (SourceFarMesh == null)
        {
          if (SourceFarMeshPath.Length > 0)
          {
            SourceFarMesh = TrueVision.TVScene.CreateMeshBuilder(_name + "_far");
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
      //if (SourceMesh == null)
      //  RegisterModel(engine);

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

  public virtual void ChaseCamera(ActorInfo ainfo)
    {
      TVCamera cam = PlayerCameraInfo.Camera;
      CameraMode mode = PlayerCameraInfo.CameraMode;

      // defaults
      TV_3DVECTOR defaultcam = new TV_3DVECTOR();
      TV_3DVECTOR defaulttgt = new TV_3DVECTOR();
      switch (mode)
      {
        case CameraMode.FREEROTATION:
        case CameraMode.FREEMODE:
        case CameraMode.FIRSTPERSON:
          defaultcam = new TV_3DVECTOR(0, 0, ainfo.TypeInfo.max_dimensions.z + 10);
          defaulttgt = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDPERSON:
          defaultcam = new TV_3DVECTOR(0, ainfo.TypeInfo.max_dimensions.y * 5, ainfo.TypeInfo.min_dimensions.z * 8);
          defaulttgt = new TV_3DVECTOR(0, 0, 20000);
          break;
        case CameraMode.THIRDREAR:
          defaultcam = new TV_3DVECTOR(0, ainfo.TypeInfo.max_dimensions.y * 3, ainfo.TypeInfo.max_dimensions.z * 8);
          defaulttgt = new TV_3DVECTOR(0, 0, -20000);
          break;
      }

      if (mode == CameraMode.FREEROTATION
      && !GameScenarioManager.IsCutsceneMode)
      {
        TV_3DVECTOR location = new TV_3DVECTOR();
        TV_3DVECTOR campos = ainfo.GetRelativePositionXYZ(location.x, location.y, location.z);

        cam.SetPosition(campos.x, campos.y, campos.z);

        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, rot.z / 2);
      }
      else if (mode == CameraMode.FREEMODE
            && !GameScenarioManager.IsCutsceneMode)
      {
        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, rot.z / 2);
      }
      else
      {
        int cammode = (GameScenarioManager.IsCutsceneMode) ? 0 : (int)mode;
        TV_3DVECTOR location = new TV_3DVECTOR();
        TV_3DVECTOR target = new TV_3DVECTOR();

        location = (cammode < ainfo.CameraSystemInfo.CamLocations.Length) ? ainfo.CameraSystemInfo.CamLocations[cammode] : defaultcam;
        target = (cammode < ainfo.CameraSystemInfo.CamTargets.Length) ? ainfo.CameraSystemInfo.CamTargets[cammode] : defaulttgt;

        TV_3DVECTOR campos = ainfo.GetRelativePositionXYZ(location.x, location.y, location.z);
        TV_3DVECTOR camview = ainfo.GetRelativePositionXYZ(target.x, target.y, target.z);
        /*
        if (ainfo.Mesh != null 
          && !(ainfo.TypeInfo is InvisibleCameraATI) 
          && !(ainfo.TypeInfo is DeathCameraATI))
        {
          if (GameScenarioManager.CameraTargetActor != null
            && GameScenarioManager.CameraTargetActor.Mesh != null)
          {
            cam.SetPosition(campos.x, campos.y, campos.z);
            cam.SetLookAt(camview.x, camview.y, camview.z);
          }
          else
          {
            cam.ChaseCamera(ainfo.Mesh, location, target, 100, true);
          }
        }
        else
        {
          cam.SetPosition(campos.x, campos.y, campos.z);
          cam.SetLookAt(camview.x, camview.y, camview.z);
        }
        */
        cam.SetPosition(campos.x, campos.y, campos.z);
        cam.SetLookAt(camview.x, camview.y, camview.z);

        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, ainfo.GetRotation().z / 2);
      }
    }

    public virtual void Initialize(ActorInfo ainfo)
    {
      // Combat
      ainfo.CombatInfo.MaxStrength = MaxStrength;
      ainfo.CombatInfo.IsCombatObject = IsCombatObject;
      ainfo.CombatInfo.OnTimedLife = OnTimedLife;
      ainfo.CombatInfo.TimedLife = TimedLife;

      // Movement
      //ainfo.MovementInfo.MaxSpeed = MaxSpeed;
      //ainfo.MovementInfo.MinSpeed = MinSpeed;
      //ainfo.MovementInfo.MaxSpeedChangeRate = MaxSpeedChangeRate;
      //ainfo.MovementInfo.MaxTurnRate = MaxTurnRate;
      //ainfo.MovementInfo.MaxSecondOrderTurnRateFrac = MaxSecondOrderTurnRateFrac;
      //ainfo.MovementInfo.ZTilt = ZTilt;
      //ainfo.MovementInfo.ZNormFrac = ZNormFrac;

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
      ainfo.RegenerationInfo.Process(Game.TimeSinceRender);

      if (ainfo.ActorState.IsDying())
      {
        ainfo.ExplosionInfo.ProcessDying();
        ainfo.DyingMoveComponent?.Update(ainfo, Game.TimeSinceRender);
      }

      // sound
      if (PlayerInfo.Actor != null
        && !(PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
        && !(PlayerInfo.Actor.TypeInfo is DeathCameraATI)
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
          ainfo.ExplosionInfo.OnDeath();

          // Debris
          if (!ActorInfo.IsAggregateMode(Engine, ainfo.ID) && !Game.IsLowFPS())
            foreach (DebrisSpawnerInfo ds in Debris)
              ds.Process(ainfo);
          break;

        case ActorState.DYING:
          ainfo.DyingMoveComponent?.Initialize(ainfo);
          break;
      }

      if (ActorInfo.IsPlayer(Engine, ainfo.ID) && !(ainfo.TypeInfo is DeathCameraATI))
      {
        ActorInfo cam = ainfo.GetEngine().ActorFactory.Get(GameScenarioManager.SceneCameraID);
        if (ainfo.ActorState.IsDyingOrDead())
        {
          if (cam == null || !(cam.TypeInfo is DeathCameraATI))
          {
            ActorCreationInfo camaci = new ActorCreationInfo(ActorTypeFactory.Get("Death Camera"));
            camaci.CreationTime = Game.GameTime;
            camaci.InitialState = ActorState.DYING;
            camaci.Position = ainfo.GetPosition();
            camaci.Rotation = new TV_3DVECTOR();
            ActorInfo a = ActorInfo.Create(ActorFactory, camaci);
            PlayerInfo.ActorID = a.ID;

            a.CameraSystemInfo.CamDeathCirclePeriod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
            a.CameraSystemInfo.CamDeathCircleRadius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
            a.CameraSystemInfo.CamDeathCircleHeight = ainfo.CameraSystemInfo.CamDeathCircleHeight;

            if (ainfo.ActorState.IsDying())
            {
              ainfo.TickEvents += GameScenarioManager.Scenario.ProcessPlayerDying;
              ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
            }
            else
            {
              ainfo.DestroyedEvents += GameScenarioManager.Scenario.ProcessPlayerKilled;
            }
          }
          else
          {
            cam.SetLocalPosition(ainfo.GetLocalPosition().x, ainfo.GetLocalPosition().y, ainfo.GetLocalPosition().z);
          }
        }
        else
        {
          if (cam != null && cam.TypeInfo is DeathCameraATI)
          {
            ActorInfo.Kill(Engine, cam.ID);
            GameScenarioManager.SceneCameraID = -1;
          }
        }
      }
    }

    public virtual void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = ActorFactory.Get(ownerActorID);
      ActorInfo hitby = ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (owner.ActorState.IsDying() && owner.CombatInfo.HitWhileDyingLeadsToDeath)
        owner.ActorState = ActorState.DEAD;

      if (owner.ActorState.IsDyingOrDead()
        || hitby.TypeInfo.ImpactDamage == 0)
        return;

      if (hitby.TypeInfo.IsDamage)
      {
        float str0 = owner.CombatInfo.Strength;
        owner.CombatInfo.onNotify(Actors.Components.CombatEventType.DAMAGE, hitby.TypeInfo.ImpactDamage);
        float str1 = owner.CombatInfo.Strength;


        if (ActorInfo.IsPlayer(Engine, owner.ID))
        {
          if (str1 < (int)str0)
            PlayerInfo.FlashHit(PlayerInfo.HealthColor);


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
          PlayerInfo.Score.AddDamage(attacker, hitby.TypeInfo.ImpactDamage * owner.CombatInfo.DamageModifier);

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
      else if (owner.TypeInfo.IsDamage)
      {
      }
      else
      {
        // Collision
        owner.CombatInfo.onNotify(Actors.Components.CombatEventType.COLLISIONDAMAGE, hitby.TypeInfo.ImpactDamage);
        if (owner.CombatInfo.Strength > 0
          && !owner.TypeInfo.NoMove
          && owner.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
        {
          float repel = -owner.MoveComponent.Speed * 0.25f;
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
      bool shielded = false;
      foreach (int i in victim.Children)
      {
        ActorInfo shd = ActorFactory.Get(i);
        if (shd != null && (shd.RegenerationInfo.ParentRegenRate > 0 || shd.RegenerationInfo.RelativeRegenRate > 0))
          shielded = true;
      }
      if (!shielded)
      {
        if (!victim.ActorState.IsDyingOrDead())
        {
          score.AddHit(victim, proj.TypeInfo.ImpactDamage * victim.CombatInfo.DamageModifier);
        }
      }

      if (!victim.ActorState.IsDyingOrDead() 
        && victim.CombatInfo.Strength <= 0)
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
        target.CombatInfo.onNotify(Actors.Components.CombatEventType.DAMAGE, weapontype.ImpactDamage);
    }

    public void Dispose()
    {
    }
  }
}
