using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class ActorTypeInfo : IDisposable
  {
    private static ThreadSafeDictionary<string, ActorTypeInfo> list = new ThreadSafeDictionary<string, ActorTypeInfo>();
    public static ThreadSafeDictionary<string, ActorTypeInfo> GetList() { return list; }
    public static ActorTypeInfo Get(string key) { return list.GetItem(key); }

    public ActorTypeInfo(string name = "")
    {
      if (name.Length > 0) { _name = name; }
      _counter = counter++;
      list.AddItem(Key, this);
    }

    // Basic Info
    private string _name = "New ActorType";
    private long _counter = 0;
    private static long counter = 0;
    public string Key { get { return _name + " " + _counter; } }

    public string Name
    {
      get { return _name; }
      set { list.RemoveItem(Key); _name = value; _counter = counter++; list.AddItem(Key, this); }
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
    public float CullDistance = 15000.0f;

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
    public bool IsTargetable = false;
    public bool IsShip = false;
    public bool IsFighter = false;
    public bool IsHardPointAddon = false;

    public int HuntWeight = 1;


    // Meshs
    public string SourceMeshPath = "";
    public string SourceFarMeshPath = "";
    public TVMesh SourceMesh = null;
    public TVMesh SourceFarMesh = null;

    // Collision Test
    //public List<TV_3DVECTOR> CollisionPoints = new List<TV_3DVECTOR>();
    //public List<TV_3DVectorRay> CollisionRays = new List<TV_3DVectorRay>();
    public List<TV_3DVECTOR> Vertices { get; private set; }
    public float z_displacement = 0;

    public TV_3DVECTOR max_dimensions = new TV_3DVECTOR();
    public TV_3DVECTOR min_dimensions = new TV_3DVECTOR();

    // Render
    public float AnimationCyclePeriod = 1.0f;
    public float RadarSize = 0;
    public bool AlwaysShowInRadar = false;

    // Performance Savings
    public bool NoRender = false;
    public bool NoProcess = false;
    public bool NoMove = false;
    public bool NoAI = false;

    public virtual void RegisterModel()
    {
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVGlobals.GetMesh(Name);
        if (SourceMesh == null)
        {
          SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Name);
          if (SourceMeshPath.Length > 0)
            SourceMesh.LoadXFile(SourceMeshPath, true);
          SourceMesh.Enable(false);
        }
      }

      if (SourceFarMesh == null)
      {
        SourceFarMesh = Engine.Instance().TVGlobals.GetMesh(Name + "_FAR");
        if (SourceFarMesh == null)
        {
          SourceFarMesh = Engine.Instance().TVScene.CreateMeshBuilder(Name + "_FAR");
          if (SourceFarMeshPath.Length > 0)
            SourceFarMesh.LoadXFile(SourceFarMeshPath, true);
          SourceFarMesh.Enable(false);
        }
      }
    }

    public virtual TVMesh GenerateMesh()
    {
      if (SourceMesh == null)
      {
        RegisterModel();
      }

      if (SourceMesh == null)
      {
        throw new NotImplementedException("Attempted to generate empty model");
      }
      SourceMesh.Enable(false);
      SourceMesh.SetCollisionEnable(false);

      if (Vertices == null)
      {
        Vertices = new List<TV_3DVECTOR>();
        float x = 0;
        float y = 0;
        float z = 0;
        float dummy = 0;
        int dumint = 0;
        for (int r = 0; r < SourceMesh.GetVertexCount(); r += 1 + SourceMesh.GetVertexCount() / 100)
        {
          SourceMesh.GetVertex(r, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
          Vertices.Add(new TV_3DVECTOR(x, y, z));
        }
      }

      TVMesh ret = SourceMesh.Duplicate();
      SourceMesh.GetBoundingBox(ref min_dimensions, ref max_dimensions);

      ret.SetCollisionEnable(CollisionEnabled);
      return ret;
    }

    public virtual TVMesh GenerateFarMesh()
    {
      if (SourceFarMesh == null)
      {
        SourceFarMesh = SourceMesh.Duplicate();
      }

      if (SourceFarMesh == null)
      {
        throw new NotImplementedException("Attempted to generate empty model");
      }
      SourceFarMesh.Enable(false);
      SourceFarMesh.SetCollisionEnable(false);

      TVMesh ret = SourceFarMesh.Duplicate();

      ret.SetCollisionEnable(CollisionEnabled);
      return ret;
    }


    /*
    public virtual TVCamera GenerateCamera(TVMesh mesh)
    {
      TVCamera cm = Engine.Instance().TVCameraFactory.CreateCamera(Key);
      return cm;
    }
    */

    public virtual void ChaseCamera(ActorInfo ainfo)
    {
      TVCamera cam = PlayerInfo.Instance().Camera;
      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        float circleperiod = ainfo.CamDeathCirclePeriod;
        float angularphase = (Game.Instance().GameTime % circleperiod) * (2 * Globals.PI / circleperiod);
        float radius = ainfo.CamDeathCircleRadius;

        cam.SetPosition(ainfo.GetPosition().x + radius * (float)Math.Cos(angularphase)
                      , ainfo.GetPosition().y + ainfo.CamDeathCircleHeight
                      , ainfo.GetPosition().z + radius * (float)Math.Sin(angularphase));

        cam.SetLookAt(ainfo.GetPosition().x
                      , ainfo.GetPosition().y
                      , ainfo.GetPosition().z);

      }
      else
      {
        // defaults
        TV_3DVECTOR defaultcam = new TV_3DVECTOR();
        TV_3DVECTOR defaulttgt = new TV_3DVECTOR();
        switch (PlayerInfo.Instance().CameraMode)
        {
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

        int cammode = (int)PlayerInfo.Instance().CameraMode;
        TV_3DVECTOR location = new TV_3DVECTOR();
        TV_3DVECTOR target = new TV_3DVECTOR();

        location = (cammode < ainfo.CamLocations.Count) ? ainfo.CamLocations[cammode] : defaultcam;
        target = (cammode < ainfo.CamTargets.Count) ? ainfo.CamTargets[cammode] : defaulttgt;

        //location = (cammode < ainfo.CamLocations.Count) ? ainfo.CamLocations[cammode] : ainfo.DefaultCamLocation;
        //target = (cammode < ainfo.CamTargets.Count) ? ainfo.CamTargets[cammode] : ainfo.DefaultCamTarget;

        TV_3DVECTOR campos = ainfo.GetRelativePositionXYZ(location.x, location.y, location.z);
        TV_3DVECTOR camview = ainfo.GetRelativePositionXYZ(target.x, target.y, target.z);

        cam.SetPosition(campos.x, campos.y, campos.z);
        cam.SetLookAt(camview.x, camview.y, camview.z);

        TV_3DVECTOR rot = cam.GetRotation();
        cam.SetRotation(rot.x, rot.y, ainfo.GetRotation().z / 2);
      }
    }

    public virtual void Initialize(ActorInfo ainfo)
    {
      ainfo.MaxStrength = MaxStrength;
      ainfo.MaxSpeed = MaxSpeed;
      ainfo.MinSpeed = MinSpeed;
      ainfo.MaxSpeedChangeRate = MaxSpeedChangeRate;
      ainfo.MaxTurnRate = MaxTurnRate;
      ainfo.MaxSecondOrderTurnRateFrac = MaxSecondOrderTurnRateFrac;
      ainfo.ZTilt = ZTilt;
      ainfo.ZNormFrac = ZNormFrac;
      ainfo.CanEvade = CanEvade;
      ainfo.CanRetaliate = CanRetaliate;
    }

    public void GenerateAddOns(ActorInfo ainfo)
    {
      //return;

      int n = 0;
      string name = "";
      float pos_x = 0;
      float pos_y = 0;
      float pos_z = 0;
      float rot_x = 0;
      float rot_y = 0;
      float rot_z = 0;
      bool attach = false;

      while (ainfo.IsStateSDefined("AddOn_"+ n))
      {
        //Format: Name,pos_x,pos_y,pos_z,rot_x,rot_y,rot_z,AttachToMesh)
        string[] tokens = ainfo.GetStateS("AddOn_" + n).Split(',');
        if (tokens.GetLength(0) > 0)
        {
          name = tokens[0].Trim();

          if (tokens.GetLength(0) < 2 || !float.TryParse(tokens[1].Trim(), out pos_x))
            pos_x = 0;

          if (tokens.GetLength(0) < 3 || !float.TryParse(tokens[2].Trim(), out pos_y))
            pos_y = 0;

          if (tokens.GetLength(0) < 4 || !float.TryParse(tokens[3].Trim(), out pos_z))
            pos_z = 0;

          if (tokens.GetLength(0) < 5 || !float.TryParse(tokens[4].Trim(), out rot_x))
            rot_x = 0;

          if (tokens.GetLength(0) < 6 || !float.TryParse(tokens[5].Trim(), out rot_y))
            rot_y = 0;

          if (tokens.GetLength(0) < 7 || !float.TryParse(tokens[6].Trim(), out rot_z))
            rot_z = 0;

          if (tokens.GetLength(0) < 8 || !bool.TryParse(tokens[7].Trim(), out attach))
            attach = false;

          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType(name));
          acinfo.InitialState = ActorState.NORMAL;
          acinfo.Faction = ainfo.Faction;
          if (attach)
            acinfo.Position = new TV_3DVECTOR(pos_x * ainfo.Scale.x, pos_y * ainfo.Scale.y, pos_z * ainfo.Scale.z - z_displacement);
          else
            acinfo.Position = ainfo.GetRelativePositionFUR(pos_x * ainfo.Scale.x, pos_y * ainfo.Scale.y, pos_z * ainfo.Scale.z - z_displacement);

          acinfo.InitialScale = ainfo.Scale;
          acinfo.Rotation = new TV_3DVECTOR(rot_x, rot_y, rot_z);
          acinfo.CreationTime = ainfo.CreationTime;

          ActorInfo a = ActorInfo.Create(acinfo);
          a.AddParent(ainfo);

          if (attach)
            a.AttachToMesh = ainfo.ID;

          ainfo.SetStateS("AddOn_" + n, "");
        }
        n++;
      }

    }

    public virtual void ProcessState(ActorInfo ainfo)
    {
      if (ainfo.LastProcessStateUpdateTime < Game.Instance().Time)
      {
        ainfo.LastProcessStateUpdateTime = Game.Instance().Time + 1;
        foreach (WeaponInfo w in ainfo.Weapons.Values)
        {
          w.Reload();
        }

        // Regen
        if (ainfo.AllowRegen && ainfo.ActorState != ActorState.DYING && ainfo.ActorState != ActorState.DEAD)
        {
          Regenerate(ainfo, ainfo.SelfRegenRate); //* Game.Instance().TimeSinceRender);
        }

        if (ainfo.ParentRegenRate != 0)
        {
          List<ActorInfo> parents = ainfo.GetAllParents();
          foreach (ActorInfo p in parents)
          {
            Regenerate(p, ainfo.ParentRegenRate); // * Game.Instance().TimeSinceRender);
          }
        }

        if (ainfo.ChildRegenRate != 0)
        {
          List<ActorInfo> children = ainfo.GetAllChildren();
          foreach (ActorInfo c in children)
          {
            Regenerate(c, ainfo.ChildRegenRate); // * Game.Instance().TimeSinceRender);
          }
        }

        if (ainfo.RelativeRegenRate != 0)
        {
          List<ActorInfo> relatives = ainfo.GetAllRelatives();
          foreach (ActorInfo r in relatives)
          {
            Regenerate(r, ainfo.RelativeRegenRate); // * Game.Instance().TimeSinceRender);
          }
        }
      }
      // Dying enable explosions
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.EnableExplosions = true;
      }

      // Explosion
      if (ainfo.EnableExplosions && !Game.Instance().IsLowFPS())
      {
        if (ainfo.ExplosionCooldown < Game.Instance().GameTime - 5f)
        {
          ainfo.ExplosionCooldown =  Game.Instance().GameTime;
        }

        List<TV_3DVECTOR> vert = ainfo.GetVertices();
        while (ainfo.ExplosionCooldown < Game.Instance().GameTime && vert.Count > 0)
        {
          ainfo.ExplosionCooldown = ainfo.ExplosionCooldown + (float)Engine.Instance().Random.NextDouble() * ainfo.ExplosionRate;

          int r = Engine.Instance().Random.Next(0,vert.Count);

          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType(ainfo.ExplosionType));
          acinfo.InitialState = ActorState.NORMAL;
          acinfo.Position = ainfo.GetRelativePositionXYZ(vert[r].x * ainfo.Scale.x, vert[r].y * ainfo.Scale.y, vert[r].z * ainfo.Scale.z);
          acinfo.InitialScale = new TV_3DVECTOR(ainfo.ExplosionSize * (ainfo.Scale.x + ainfo.Scale.y + ainfo.Scale.z) / 3, ainfo.ExplosionSize * (ainfo.Scale.x + ainfo.Scale.y + ainfo.Scale.z) / 3, 1);
          ActorInfo.Create(acinfo);

          vert.RemoveAt(r);
        }
      }
      else
      {
        ainfo.ExplosionCooldown = Game.Instance().GameTime;
      }
    }

    private void Regenerate(ActorInfo ainfo, float amount)
    {
      if (ainfo.AllowRegen && ainfo.ActorState != ActorState.DYING && ainfo.ActorState != ActorState.DEAD)
      {
        ainfo.Strength += amount ;
        if (ainfo.Strength > ainfo.TypeInfo.MaxStrength)
          ainfo.Strength = ainfo.TypeInfo.MaxStrength;
      }
    }

    public virtual void ProcessNewState(ActorInfo ainfo)
    {
      if (ainfo.ActorState == ActorState.DEAD)
      {
        if (ainfo.EnableDeathExplosion)
        {
          ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Instance().GetActorType(ainfo.DeathExplosionType));
          acinfo.Position = ainfo.GetPosition();
          acinfo.InitialScale = new TV_3DVECTOR(ainfo.DeathExplosionSize * (ainfo.Scale.x + ainfo.Scale.y + ainfo.Scale.z) / 3, ainfo.DeathExplosionSize * (ainfo.Scale.x + ainfo.Scale.y + ainfo.Scale.z) / 3, 1);

          ActorInfo.Create(acinfo);
        }
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
            PlayerInfo.Instance().Actor.Strength = 0;

            a.CamDeathCirclePeriod = ainfo.CamDeathCirclePeriod;
            a.CamDeathCircleRadius = ainfo.CamDeathCircleRadius;
            a.CamDeathCircleHeight = ainfo.CamDeathCircleHeight;

            if (ainfo.ActorState == ActorState.DYING)
            {
              ainfo.TickEvents.Add("Common_ProcessPlayerDying");
              ainfo.DestroyedEvents.Add("Common_ProcessPlayerKilled");
            }
            else
            {
              GameScenarioManager.Instance().Scenario.ProcessPlayerKilled(new object[] { ainfo });
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
            GameScenarioManager.Instance().SceneCamera.Destroy();
            GameScenarioManager.Instance().SceneCamera = null;
          }
        }
      }
    }

    public virtual void ProcessHit(ActorInfo ainfo, ActorInfo hitby, TV_3DVECTOR impact, TV_3DVECTOR normal)
    {
      if (hitby.TypeInfo.ImpactDamage * ainfo.DamageModifier == 0)
        return;

      if (hitby.TypeInfo.IsDamage)
      {
        float str0 = ainfo.Strength;
        ainfo.Strength -= hitby.TypeInfo.ImpactDamage * ainfo.DamageModifier;
        float str1 = ainfo.Strength;
        if (ainfo.IsPlayer())
        {
          if (str1 < (int)str0)
          {
            PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
          }
          PlayerInfo.Instance().Score.DamageTaken += hitby.TypeInfo.ImpactDamage * ainfo.DamageModifier;
        }

        if (ainfo.ActorState != ActorState.DEAD && ainfo.ActorState != ActorState.DEAD && ainfo.Strength <= 0)
        {
          if (ainfo.IsPlayer())
          {
            PlayerInfo.Instance().Score.Deaths++;
          }
        }

        List<ActorInfo> attackerfamily = hitby.GetAllParents();
        foreach (ActorInfo a in attackerfamily)
        {
          if (a.Faction != null && !a.Faction.IsAlliedWith(ainfo.Faction))
          {
            AddScore(a.Score, a, ainfo);
            if (a.IsPlayer())
            {
              AddScore(PlayerInfo.Instance().Score, a, ainfo);
            }
          }
        }

        hitby.SetLocalPosition(impact.x, impact.y, impact.z);
        hitby.ActorState = ActorState.DYING;

        //ainfo.AI.ProcessHit(ainfo, hitby);
        if ((ainfo.TypeInfo is FighterGroup || ainfo.TypeInfo is TIEGroup))
        {
          List<ActorInfo> hparents = hitby.GetAllParents();
          if (hitby.TypeInfo.IsDamage && hparents.Count > 0)
          {
            if (ainfo.CurrentAction != null && ainfo.CurrentAction.CanInterrupt && ainfo.Faction != null && !ainfo.Faction.IsAlliedWith(hparents[0].Faction) && ainfo.CanRetaliate)
            {
              ActionManager.ClearQueue(ainfo);
              ActionManager.QueueLast(ainfo, new Actions.AttackActor(hparents[0]));
            }
            else if (ainfo.CurrentAction != null && !(ainfo.CurrentAction is Actions.Evade) && ainfo.CanEvade)
            {
              ActionManager.QueueFirst(ainfo, new Actions.Evade());
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
        if (ainfo.ActorState != ActorState.FIXED && ainfo.TypeInfo.IsFighter)
        {
          float repel = -ainfo.Speed * 0.25f; //* hitby.Weight / (ainfo.Weight + hitby.Weight);
                                              //TV_3DVECTOR disp = new TV_3DVECTOR();
                                              //Engine.Instance().TVMathLibrary.TVVec3Normalize(ref disp, hitby.Position - ainfo.Position);
                                              //ainfo.Mesh.SetPosition(
                                              //  ainfo.Position.x + repel * disp.x,
                                              //  ainfo.Position.y + repel * disp.y,
                                              //  ainfo.Position.z + repel * disp.z);
          ainfo.MoveRelative(repel, 0, 0);
        }
        ainfo.Strength -= hitby.TypeInfo.ImpactDamage * ainfo.DamageModifier;
        if (ainfo.Strength < 0 && !(ainfo.TypeInfo is StarDestroyerGroup || ainfo.TypeInfo is WarshipGroup))
        {
          ainfo.ActorState = ActorState.DEAD;
        }
        if (ainfo.IsPlayer())
        {
          PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
        }

        List<ActorInfo> attackerfamily = hitby.GetAllParents();
        foreach (ActorInfo a in attackerfamily)
        {
          if (a.Faction != null && !a.Faction.IsAlliedWith(ainfo.Faction))
          {
            AddScore(a.Score, a, ainfo);
            if (a.IsPlayer())
            {
              AddScore(PlayerInfo.Instance().Score, a, ainfo);
            }
          }
        }
      }

      ainfo.OnHitEvent(new object[] { ainfo, hitby });
    }

    private void AddScore(ScoreInfo score, ActorInfo scorer, ActorInfo victim)
    {
      score.Hits++;
      score.Score++;

      bool shielded = false;
      foreach (ActorInfo shd in victim.GetAllChildren())
      {
        if (shd.ParentRegenRate > 0 || shd.RelativeRegenRate > 0)
          shielded = true;
      }
      if (!shielded)
      {
        if (victim.ActorState != ActorState.DEAD && victim.ActorState != ActorState.DYING)
        {
          score.Score += victim.TypeInfo.Score_perStrength * scorer.TypeInfo.ImpactDamage * victim.DamageModifier;
        }
      }

      if (victim.TypeInfo is FighterGroup || victim.TypeInfo is TIEGroup)
      {
        score.HitsOnFighters++;
      }
      if (victim.ActorState != ActorState.DEAD && victim.ActorState != ActorState.DYING && victim.Strength <= 0)
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

      if (ainfo.Weapons.ContainsKey(s))
      {
        weapon = ainfo.Weapons[s];
      }
    }

    public virtual bool FireWeapon(ActorInfo ainfo, ActorInfo target, string sweapon)
    {
      WeaponInfo weapon = null;
      int burst = 0;

      // AI Determination
      if (sweapon == "auto")
      {
        foreach (string ws in ainfo.AIWeapons)
        {
          if (FireWeapon(ainfo, target, ws))
            return true;
        }
      }
      else
      {
        InterpretWeapon(ainfo, sweapon, out weapon, out burst);

        if (weapon != null)
        {
          return weapon.Fire(ainfo, target, burst);
          //return weapon.Fire(ainfo, ainfo.AI.Master.Move_TargetActor, burst);
        }
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
        target.Strength -= weapontype.ImpactDamage * target.DamageModifier;
    }

    public void Dispose()
    {
      list.RemoveItem(Key);
    }
  }
}
