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
    public ActorTypeInfo(string name = "")
    {
      if (name.Length > 0) { _name = name; }
      _counter = counter++;
    }

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
        SourceMesh = Engine.Instance().TVGlobals.GetMesh(_name);
        if (SourceMesh == null)
        {
          SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(_name);
          if (SourceMeshPath.Length > 0)
            SourceMesh.LoadXFile(SourceMeshPath, true);
          SourceMesh.Enable(false);
          SourceMesh.WeldVertices();
          SourceMesh.ComputeBoundings();
        }
      }

      if (SourceFarMesh == null)
      {
        SourceFarMesh = Engine.Instance().TVGlobals.GetMesh(_name + "_far");
        if (SourceFarMesh == null)
        {
          if (SourceFarMeshPath.Length > 0)
          {
            SourceFarMesh = Engine.Instance().TVScene.CreateMeshBuilder(_name + "_far");
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
        RegisterModel();

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
      int tex = Engine.Instance().TVGlobals.GetTex(name);
      if (tex == 0)
      {
        int texS = Engine.Instance().TVTextureFactory.LoadTexture(texpath);
        int texA = Engine.Instance().TVTextureFactory.LoadTexture(alphatexpath ?? texpath); //LoadAlphaTexture
        tex = Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, name);
      }
      return tex;
    }

    public int LoadTexture(string name, string texpath)
    {
      int tex = Engine.Instance().TVGlobals.GetTex(name);
      if (tex == 0)
        tex = Engine.Instance().TVTextureFactory.LoadTexture(texpath, name);
      return tex;
    }

  public virtual void ChaseCamera(ActorInfo ainfo)
    {
      TVCamera cam = PlayerCameraInfo.Instance().Camera;
      CameraMode mode = PlayerCameraInfo.Instance().CameraMode;

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
      && !GameScenarioManager.Instance().IsCutsceneMode)
      {
        TV_3DVECTOR location = new TV_3DVECTOR();
        TV_3DVECTOR campos = ainfo.GetRelativePositionXYZ(location.x, location.y, location.z);

        cam.SetPosition(campos.x, campos.y, campos.z);

        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, rot.z / 2);
      }
      else if (mode == CameraMode.FREEMODE
            && !GameScenarioManager.Instance().IsCutsceneMode)
      {
        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, rot.z / 2);
      }
      else
      {
        int cammode = (GameScenarioManager.Instance().IsCutsceneMode) ? 0 : (int)mode;
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
          if (GameScenarioManager.Instance().CameraTargetActor != null
            && GameScenarioManager.Instance().CameraTargetActor.Mesh != null)
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

      // Movement
      ainfo.MovementInfo.MaxSpeed = MaxSpeed;
      ainfo.MovementInfo.MinSpeed = MinSpeed;
      ainfo.MovementInfo.MaxSpeedChangeRate = MaxSpeedChangeRate;
      ainfo.MovementInfo.MaxTurnRate = MaxTurnRate;
      ainfo.MovementInfo.MaxSecondOrderTurnRateFrac = MaxSecondOrderTurnRateFrac;
      ainfo.MovementInfo.ZTilt = ZTilt;
      ainfo.MovementInfo.ZNormFrac = ZNormFrac;

      // AI
      ainfo.CanEvade = CanEvade;
      ainfo.CanRetaliate = CanRetaliate;

      if (!(GameScenarioManager.Instance().Scenario is GSMainMenu))
        foreach (SoundSourceInfo assi in InitialSoundSources)
          assi.Process(ainfo);
    }

    public void GenerateAddOns(ActorInfo ainfo)
    {
      foreach (AddOnInfo addon in AddOns)
        addon.Create(ainfo);
    }

    public virtual void ProcessState(ActorInfo ainfo)
    {
      // only per second.
      if (ainfo.LastProcessStateUpdateTime < Game.Instance().GameTime)
      {
        ainfo.LastProcessStateUpdateTime = Game.Instance().GameTime + 1;
        // weapons
        foreach (WeaponInfo w in ainfo.WeaponSystemInfo.Weapons.Values)
          w.Reload();

        // regeneration
        ainfo.RegenerationInfo.Process();
      }

      // Dying enable explosions
      ainfo.ExplosionInfo.ProcessDying();
      ainfo.MovementInfo.ExecuteDyingMovement();

      // sound
      if (PlayerInfo.Instance().Actor != null
        && !(PlayerInfo.Instance().Actor.TypeInfo is InvisibleCameraATI)
        && !(PlayerInfo.Instance().Actor.TypeInfo is DeathCameraATI)
        && ainfo.CreationState == CreationState.ACTIVE 
        && !ainfo.IsScenePlayer()
        && !(GameScenarioManager.Instance().Scenario is GSMainMenu))
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
          if (!ainfo.IsAggregateMode() && !Game.Instance().IsLowFPS())
            foreach (DebrisSpawnerInfo ds in Debris)
              ds.Process(ainfo);
          break;

        case ActorState.DYING:
          ainfo.MovementInfo.GenerateDyingMovement();
          break;
      }

      if (ainfo.IsPlayer() && !(ainfo.TypeInfo is DeathCameraATI))
      {
        if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
        {
          if (GameScenarioManager.Instance().SceneCamera == null || !(GameScenarioManager.Instance().SceneCamera.TypeInfo is DeathCameraATI))
          {
            ActorCreationInfo camaci = new ActorCreationInfo(DeathCameraATI.Instance());
            camaci.CreationTime = Game.Instance().GameTime;
            camaci.InitialState = ActorState.DYING;
            camaci.Position = ainfo.GetPosition();
            camaci.Rotation = new TV_3DVECTOR();
            ActorInfo a = ActorInfo.Create(camaci);
            PlayerInfo.Instance().Actor = a;
            PlayerInfo.Instance().Actor.CombatInfo.Strength = 0;

            a.CameraSystemInfo.CamDeathCirclePeriod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
            a.CameraSystemInfo.CamDeathCircleRadius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
            a.CameraSystemInfo.CamDeathCircleHeight = ainfo.CameraSystemInfo.CamDeathCircleHeight;

            if (ainfo.ActorState == ActorState.DYING)
            {
              ainfo.TickEvents += GameScenarioManager.Instance().Scenario.ProcessPlayerDying;
              ainfo.DestroyedEvents += GameScenarioManager.Instance().Scenario.ProcessPlayerKilled;
            }
            else
            {
              ainfo.OnDestroyedEvent(null);
            }
          }
          else
          {
            GameScenarioManager.Instance().SceneCamera.SetLocalPosition(ainfo.GetLocalPosition().x, ainfo.GetLocalPosition().y, ainfo.GetLocalPosition().z);
          }
        }
        else
        {
          if (GameScenarioManager.Instance().SceneCamera != null && GameScenarioManager.Instance().SceneCamera.TypeInfo is DeathCameraATI)
          {
            GameScenarioManager.Instance().SceneCamera.Kill();
            GameScenarioManager.Instance().SceneCamera = null;
          }
        }
      }
    }

    public virtual void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (hitby.TypeInfo.ImpactDamage * ainfo.CombatInfo.DamageModifier == 0)
        return;

      if (hitby.TypeInfo.IsDamage)
      {
        float str0 = ainfo.CombatInfo.Strength;
        ainfo.CombatInfo.Strength -= hitby.TypeInfo.ImpactDamage * ainfo.CombatInfo.DamageModifier;
        float str1 = ainfo.CombatInfo.Strength;
        if (ainfo.IsPlayer())
        {
          if (str1 < (int)str0)
            PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
          PlayerInfo.Instance().Score.DamageTaken += hitby.TypeInfo.ImpactDamage * ainfo.CombatInfo.DamageModifier;
        }

        if (ainfo.ActorState != ActorState.DEAD && ainfo.ActorState != ActorState.DEAD && ainfo.CombatInfo.Strength <= 0)
        {
          if (ainfo.IsPlayer())
            PlayerInfo.Instance().Score.Deaths++;
        }

        List<ActorInfo> attackerfamily = hitby.GetAllParents();
        foreach (ActorInfo a in attackerfamily)
        {
          if (a.Faction != null && !a.Faction.IsAlliedWith(ainfo.Faction))
          {
            if (!(a.TypeInfo is AddOnGroup))
              AddScore(a.Score, a, ainfo);
            else
            { }
            //if (a.IsPlayer())
            //  AddScore(PlayerInfo.Instance().Score, a, ainfo);
          }
        }

        hitby.SetLocalPosition(impact.x, impact.y, impact.z);
        hitby.ActorState = ActorState.DYING;

        if ((ainfo.TypeInfo is FighterGroup))
        {
          List<ActorInfo> hparents = hitby.GetAllParents();
          if (hitby.TypeInfo.IsDamage && hparents.Count > 0)
          {
            if (ainfo.CurrentAction != null && ainfo.CurrentAction.CanInterrupt && ainfo.Faction != null && !ainfo.Faction.IsAlliedWith(hparents[0].Faction) && ainfo.CanRetaliate)
            {
              ActionManager.ClearQueue(ainfo);
              ActionManager.QueueLast(ainfo, new AttackActor(hparents[0]));
            }
            else if (ainfo.CurrentAction != null && !(ainfo.CurrentAction is Evade) && ainfo.CanEvade)
            {
              ActionManager.QueueFirst(ainfo, new Evade());
            }
          }
        }
      }
      else if (ainfo.TypeInfo.IsDamage)
      {
      }
      else
      {
        // Collision
        ainfo.CombatInfo.Strength -= hitby.TypeInfo.ImpactDamage * ainfo.CombatInfo.DamageModifier;
        if (ainfo.CombatInfo.Strength > 0 && !ainfo.TypeInfo.NoMove && ainfo.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
        {
          float repel = -ainfo.MovementInfo.Speed * 0.25f;
          ainfo.MoveRelative(repel, 0, 0);
        }

        if (ainfo.IsPlayer())
          PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);

        List<ActorInfo> attackerfamily = hitby.GetAllParents();
        attackerfamily.Add(hitby);
        foreach (ActorInfo a in attackerfamily)
        {
          if (a.Faction != null && !a.Faction.IsAlliedWith(ainfo.Faction))
          {
            if (!(a.TypeInfo is AddOnGroup))
              AddScore(a.Score, a, ainfo);
          }
        }

        if (ainfo.CombatInfo.Strength < 0 && !(ainfo.TypeInfo is StarDestroyerGroup || ainfo.TypeInfo is WarshipGroup))
        {
          ainfo.ActorState = ActorState.DEAD;
          if (ainfo.IsPlayer())
            PlayerInfo.Instance().Score.Deaths++;
        }
      }

      hitby.OnHitEvent(ainfo);
    }

    private void AddScore(ScoreInfo score, ActorInfo scorer, ActorInfo victim)
    {
      score.Hits++;
      score.Score++;

      bool shielded = false;
      foreach (ActorInfo shd in victim.GetAllChildren())
      {
        if (shd.RegenerationInfo.ParentRegenRate > 0 || shd.RegenerationInfo.RelativeRegenRate > 0)
          shielded = true;
      }
      if (!shielded)
      {
        if (victim.ActorState != ActorState.DEAD && victim.ActorState != ActorState.DYING)
        {
          score.Score += victim.TypeInfo.Score_perStrength * scorer.TypeInfo.ImpactDamage * victim.CombatInfo.DamageModifier;
        }
      }

      if (victim.TypeInfo is FighterGroup)
      {
        score.HitsOnFighters++;
      }
      if (victim.ActorState != ActorState.DEAD && victim.ActorState != ActorState.DYING && victim.CombatInfo.Strength <= 0)
      {
        score.Kills++;
        score.Score += victim.TypeInfo.Score_DestroyBonus;
        string t = victim.TypeInfo.Name;
        if (!score.KillsByType.ContainsKey(t))
          score.KillsByType.Add(t, 1);
        else
          score.KillsByType[t]++;
      }
    }

    public void InterpretWeapon(ActorInfo ainfo, string sweapon, out WeaponInfo weapon, out int burst)
    {
      string s = "none";
      weapon = null;
      burst = 1;

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

      if (ainfo.WeaponSystemInfo.Weapons.ContainsKey(s))
      {
        weapon = ainfo.WeaponSystemInfo.Weapons[s];
      }
    }

    public virtual bool FireWeapon(ActorInfo ainfo, ActorInfo target, string sweapon)
    {
      WeaponInfo weapon = null;
      int burst = 0;

      // AI Determination
      if (sweapon == "auto")
      {
        foreach (string ws in ainfo.WeaponSystemInfo.AIWeapons)
        {
          if (FireWeapon(ainfo, target, ws))
            return true;
        }
      }
      else
      {
        InterpretWeapon(ainfo, sweapon, out weapon, out burst);

        if (weapon != null)
          return weapon.Fire(ainfo, target, burst);
      }
      
      return false;
    }

    public virtual void FireAggregation(ActorInfo ainfo, ActorInfo target, ActorTypeInfo weapontype)
    {
      float accuracy = 1;

      float d = ActorDistanceInfo.GetDistance(ainfo, target) / weapontype.MaxSpeed;
      TV_3DVECTOR angle = Utilities.GetRotation(target.GetPosition() - ainfo.GetPosition()) - ainfo.GetRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Engine.Instance().Random.NextDouble() < accuracy)
        target.CombatInfo.Strength -= weapontype.ImpactDamage * target.CombatInfo.DamageModifier;
    }

    public void Dispose()
    {
    }
  }
}
