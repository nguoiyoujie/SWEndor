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
      Owner = owner;
      if (name.Length > 0) { _name = name; }
      _counter = counter++;
    }

    public readonly Factory Owner;

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
        SourceMesh = Owner.Engine.TrueVision.TVGlobals.GetMesh(_name);
        if (SourceMesh == null)
        {
          SourceMesh = Owner.Engine.TrueVision.TVScene.CreateMeshBuilder(_name);
          if (SourceMeshPath.Length > 0)
            SourceMesh.LoadXFile(SourceMeshPath, true);
          SourceMesh.Enable(false);
          SourceMesh.WeldVertices();
          SourceMesh.ComputeBoundings();
        }
      }

      if (SourceFarMesh == null)
      {
        SourceFarMesh = Owner.Engine.TrueVision.TVGlobals.GetMesh(_name + "_far");
        if (SourceFarMesh == null)
        {
          if (SourceFarMeshPath.Length > 0)
          {
            SourceFarMesh = Owner.Engine.TrueVision.TVScene.CreateMeshBuilder(_name + "_far");
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
      int tex = Owner.Engine.TrueVision.TVGlobals.GetTex(name);
      if (tex == 0)
      {
        int texS = Owner.Engine.TrueVision.TVTextureFactory.LoadTexture(texpath);
        int texA = Owner.Engine.TrueVision.TVTextureFactory.LoadTexture(alphatexpath ?? texpath); //LoadAlphaTexture
        tex = Owner.Engine.TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, name);
      }
      return tex;
    }

    public int LoadTexture(string name, string texpath)
    {
      int tex = Owner.Engine.TrueVision.TVGlobals.GetTex(name);
      if (tex == 0)
        tex = Owner.Engine.TrueVision.TVTextureFactory.LoadTexture(texpath, name);
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
      && !Owner.Engine.GameScenarioManager.IsCutsceneMode)
      {
        TV_3DVECTOR location = new TV_3DVECTOR();
        TV_3DVECTOR campos = ainfo.GetRelativePositionXYZ(location.x, location.y, location.z);

        cam.SetPosition(campos.x, campos.y, campos.z);

        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, rot.z / 2);
      }
      else if (mode == CameraMode.FREEMODE
            && !Owner.Engine.GameScenarioManager.IsCutsceneMode)
      {
        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, rot.z / 2);
      }
      else
      {
        int cammode = (Owner.Engine.GameScenarioManager.IsCutsceneMode) ? 0 : (int)mode;
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
          if (Owner.Engine.GameScenarioManager.CameraTargetActor != null
            && Owner.Engine.GameScenarioManager.CameraTargetActor.Mesh != null)
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

      if (!(Owner.Engine.GameScenarioManager.Scenario is GSMainMenu))
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
      if (ainfo.LastProcessStateUpdateTime < Owner.Engine.Game.GameTime)
      {
        ainfo.LastProcessStateUpdateTime = Owner.Engine.Game.GameTime + 1;
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
      if (Owner.Engine.PlayerInfo.Actor != null
        && !(Owner.Engine.PlayerInfo.Actor.TypeInfo is InvisibleCameraATI)
        && !(Owner.Engine.PlayerInfo.Actor.TypeInfo is DeathCameraATI)
        && ainfo.CreationState == CreationState.ACTIVE 
        && !ainfo.IsScenePlayer()
        && !(Owner.Engine.GameScenarioManager.Scenario is GSMainMenu))
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
          if (!ainfo.IsAggregateMode() && !Owner.Engine.Game.IsLowFPS())
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
          if (Owner.Engine.GameScenarioManager.SceneCamera == null || !(Owner.Engine.GameScenarioManager.SceneCamera.TypeInfo is DeathCameraATI))
          {
            ActorCreationInfo camaci = new ActorCreationInfo(Owner.Engine.ActorTypeFactory.Get("Death Camera"));
            camaci.CreationTime = Owner.Engine.Game.GameTime;
            camaci.InitialState = ActorState.DYING;
            camaci.Position = ainfo.GetPosition();
            camaci.Rotation = new TV_3DVECTOR();
            ActorInfo a = ActorInfo.Create(Owner.Engine.ActorFactory, camaci);
            Owner.Engine.PlayerInfo.ActorID = a.ID;

            a.CombatInfo.Strength = 0;
            a.CameraSystemInfo.CamDeathCirclePeriod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
            a.CameraSystemInfo.CamDeathCircleRadius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
            a.CameraSystemInfo.CamDeathCircleHeight = ainfo.CameraSystemInfo.CamDeathCircleHeight;

            if (ainfo.ActorState == ActorState.DYING)
            {
              ainfo.TickEvents += Owner.Engine.GameScenarioManager.Scenario.ProcessPlayerDying;
              ainfo.DestroyedEvents += Owner.Engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
            }
            else
            {
              ainfo.DestroyedEvents += Owner.Engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
            }
          }
          else
          {
            Owner.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(ainfo.GetLocalPosition().x, ainfo.GetLocalPosition().y, ainfo.GetLocalPosition().z);
          }
        }
        else
        {
          if (Owner.Engine.GameScenarioManager.SceneCamera != null && Owner.Engine.GameScenarioManager.SceneCamera.TypeInfo is DeathCameraATI)
          {
            Owner.Engine.GameScenarioManager.SceneCamera.Kill();
            Owner.Engine.GameScenarioManager.SceneCamera = null;
          }
        }
      }
    }

    public virtual void ProcessHit(int ownerActorID, int hitbyActorID, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
      ActorInfo hitby = Owner.Engine.ActorFactory.Get(hitbyActorID);

      if (owner == null || hitby == null)
        return;

      if (owner.ActorState == ActorState.DYING && owner.CombatInfo.HitWhileDyingLeadsToDeath)
        owner.ActorState = ActorState.DEAD;

      if (hitby.TypeInfo.ImpactDamage * owner.CombatInfo.DamageModifier == 0)
        return;

      if (hitby.TypeInfo.IsDamage)
      {
        float str0 = owner.CombatInfo.Strength;
        owner.CombatInfo.Strength -= hitby.TypeInfo.ImpactDamage * owner.CombatInfo.DamageModifier;
        float str1 = owner.CombatInfo.Strength;
        if (owner.IsPlayer())
        {
          if (str1 < (int)str0)
            Owner.Engine.PlayerInfo.FlashHit(Owner.Engine.PlayerInfo.HealthColor);
          Owner.Engine.PlayerInfo.Score.DamageTaken += hitby.TypeInfo.ImpactDamage * owner.CombatInfo.DamageModifier;

          if (owner.ActorState != ActorState.DYING 
            && owner.ActorState != ActorState.DEAD 
            && owner.CombatInfo.Strength <= 0)
          {
            Owner.Engine.PlayerInfo.Score.Deaths++;
          }
        }

        List<int> attackerfamily = hitby.GetAllParents();
        foreach (int i in attackerfamily)
        {
          ActorInfo a = Owner.Engine.ActorFactory.Get(i);
          if (a != null && a.Faction != null && !a.Faction.IsAlliedWith(owner.Faction))
          {
            if (!(a.TypeInfo is Groups.AddOn))
              AddScore(a.Score, a, owner);

            if (a.IsPlayer())
              AddScore(Owner.Engine.PlayerInfo.Score, a, owner);
          }
        }

        hitby.SetLocalPosition(impact.x, impact.y, impact.z);
        hitby.ActorState = ActorState.DYING;

        if ((owner.TypeInfo is Groups.Fighter))
        {
          List<int> hparents = hitby.GetAllParents();
          if (hparents.Count > 0)
          {
            ActorInfo h = Owner.Engine.ActorFactory.Get(hparents[0]);
            if (h != null && hitby.TypeInfo.IsDamage && hparents.Count > 0)
            {
              if (owner.CurrentAction != null && owner.CurrentAction.CanInterrupt && owner.Faction != null && !owner.Faction.IsAlliedWith(h.Faction) && owner.CanRetaliate)
              {
                owner.Owner.Engine.ActionManager.ClearQueue(ownerActorID);
                owner.Owner.Engine.ActionManager.QueueLast(ownerActorID, new AttackActor(h.ID));
              }
              else if (owner.CurrentAction != null && !(owner.CurrentAction is Evade) && owner.CanEvade)
              {
                owner.Owner.Engine.ActionManager.QueueFirst(ownerActorID, new Evade());
              }
            }
          }
        }
      }
      else if (owner.TypeInfo.IsDamage)
      {
      }
      else
      {
        // Collision
        owner.CombatInfo.Strength -= hitby.TypeInfo.ImpactDamage * owner.CombatInfo.DamageModifier;
        if (owner.CombatInfo.Strength > 0 && !owner.TypeInfo.NoMove && owner.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
        {
          float repel = -owner.MovementInfo.Speed * 0.25f;
          owner.MoveRelative(repel, 0, 0);
        }

        if (owner.IsPlayer())
          Owner.Engine.PlayerInfo.FlashHit(Owner.Engine.PlayerInfo.HealthColor);

        List<int> attackerfamily = hitby.GetAllParents();
        attackerfamily.Add(hitby.ID);
        foreach (int i in attackerfamily)
        {
          ActorInfo a = Owner.Engine.ActorFactory.Get(i);
          if (a != null && a.Faction != null && !a.Faction.IsAlliedWith(owner.Faction))
          {
            if (!(a.TypeInfo is Groups.AddOn))
              AddScore(a.Score, a, owner);
          }
        }

        if (owner.CombatInfo.Strength < 0 && !(owner.TypeInfo is Groups.StarDestroyer || owner.TypeInfo is Groups.Warship))
        {
          owner.ActorState = ActorState.DEAD;
          if (owner.IsPlayer())
            Owner.Engine.PlayerInfo.Score.Deaths++;
        }
      }

      hitby.OnHitEvent(owner.ID);
    }

    private void AddScore(ScoreInfo score, ActorInfo scorer, ActorInfo victim)
    {
      score.Hits++;
      score.Score++;

      bool shielded = false;
      foreach (int i in victim.GetAllChildren())
      {
        ActorInfo shd = Owner.Engine.ActorFactory.Get(i);
        if (shd != null && shd.RegenerationInfo.ParentRegenRate > 0 || shd.RegenerationInfo.RelativeRegenRate > 0)
          shielded = true;
      }
      if (!shielded)
      {
        if (victim.ActorState != ActorState.DEAD && victim.ActorState != ActorState.DYING)
        {
          score.Score += victim.TypeInfo.Score_perStrength * scorer.TypeInfo.ImpactDamage * victim.CombatInfo.DamageModifier;
        }
      }

      if (victim.TypeInfo is Groups.Fighter)
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

    public void InterpretWeapon(int ownerActorID, string sweapon, out WeaponInfo weapon, out int burst)
    {
      string s = "none";
      weapon = null;
      burst = 1;

      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
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
      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
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
          return weapon.Fire(ownerActorID, targetActorID, burst);
      }
      
      return false;
    }

    public virtual void FireAggregation(int ownerActorID, int targetActorID, ActorTypeInfo weapontype)
    {
      float accuracy = 1;
      ActorInfo owner = Owner.Engine.ActorFactory.Get(ownerActorID);
      ActorInfo target = Owner.Engine.ActorFactory.Get(targetActorID);
      
      float d = ActorDistanceInfo.GetDistance(ownerActorID, targetActorID) / weapontype.MaxSpeed;
      TV_3DVECTOR angle = Utilities.GetRotation(target.GetPosition() - owner.GetPosition()) - owner.GetRotation();
      angle.x -= (int)((angle.x + 180) / 360) * 360;
      angle.y -= (int)((angle.y + 180) / 360) * 360;

      accuracy /= (d + 1);
      accuracy /= (Math.Abs(angle.x) + 1);
      accuracy /= (Math.Abs(angle.y) + 1);

      if (Owner.Engine.Random.NextDouble() < accuracy)
        target.CombatInfo.Strength -= weapontype.ImpactDamage * target.CombatInfo.DamageModifier;
    }

    public void Dispose()
    {
    }
  }
}
