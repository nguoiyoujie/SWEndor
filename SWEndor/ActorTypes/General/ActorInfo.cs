using MTV3D65;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace SWEndor
{
  public class ActorInfo
  {
    public ActorTypeInfo TypeInfo { get; private set; }

    // Identifiers
    private string _name = "New Actor";
    private string sidebar_name = "";

    public string Name { get { return _name; } }
    public string SideBarName { get { return (sidebar_name.Length == 0) ? _name: sidebar_name; } set { sidebar_name = value; } }
    public int ID { get; private set; }
    public string Key { get { return _name + " " + ID; } }

    // Creation and Disposal
    public float CreationTime = 0;
    public CreationState CreationState = CreationState.PLANNED;

    // Faction
    private FactionInfo _faction;
    public FactionInfo Faction { get { if (_faction == null) _faction = FactionInfo.Neutral; return _faction; } set { _faction = value; } }

    // Scoring
    public ScoreInfo Score = new ScoreInfo();

    // Combat
    public bool IsCombatObject = false;
    public bool OnTimedLife = false;
    public float TimedLife = 100;
    public float Strength = 1;
    public float MaxStrength = 1;
    public float DamageModifier = 1;

    // Movement
    public float Speed = 0;
    public float MaxSpeed = 0;
    public float MinSpeed = 0;
    public float XTurnAngle = 0;
    public float YTurnAngle = 0;
    public float ZTilt = 0;
    public float ZNormFrac = 0;
    public float MaxSpeedChangeRate = 0;
    public float MaxTurnRate = 0;
    public float MaxSecondOrderTurnRateFrac = 0;
    public bool CanEvade = true;
    public bool CanRetaliate = true;
    public bool ApplyZBalance = true;

    // Checks
    public bool EnteredCombatZone = false;
    public float LastProcessStateUpdateTime = 0;

    // Weapons
    public Dictionary<string, WeaponInfo> Weapons = new Dictionary<string, WeaponInfo>();
    public List<string> PrimaryWeapons = new List<string>();
    public List<string> SecondaryWeapons = new List<string>();
    public List<string> AIWeapons = new List<string>();

    public List<TV_3DVECTOR> WeaponPositions = new List<TV_3DVECTOR>();

    // Camera System
    public TV_3DVECTOR DefaultCamLocation = new TV_3DVECTOR();
    public TV_3DVECTOR DefaultCamTarget = new TV_3DVECTOR(0, 0, 2000);
    public List<TV_3DVECTOR> CamLocations = new List<TV_3DVECTOR>();
    public List<TV_3DVECTOR> CamTargets = new List<TV_3DVECTOR>();

    public float CamDeathCircleRadius = 350;
    public float CamDeathCircleHeight = 25;
    public float CamDeathCirclePeriod = 15;

    // Regeneration
    public bool AllowRegen = true;
    public float SelfRegenRate = 0;
    public float ParentRegenRate = 0;
    public float ChildRegenRate = 0;
    public float RelativeRegenRate = 0;

    // Explosion Systems
    public bool EnableExplosions = false;
    public float ExplosionCooldown = Game.Instance().GameTime;
    public float ExplosionRate = 0.5f;
    public float ExplosionSize = 1;
    public string ExplosionType = "Explosion";
    public bool EnableDeathExplosion = false;
    public string DeathExplosionType = "ExplosionSm";
    public float DeathExplosionSize = 1;

    // States
    private Dictionary<string, float> StatesF = new Dictionary<string, float>();
    private Dictionary<string, string> StatesS = new Dictionary<string, string>();
    private Dictionary<string, bool> StatesB = new Dictionary<string, bool>();

    // Delegate Events
    public List<string> TickEvents = new List<string>();
    public List<string> CreatedEvents = new List<string>();
    public List<string> DestroyedEvents = new List<string>();
    public List<string> ActorStateChangeEvents = new List<string>();
    public List<string> HitEvents = new List<string>();

    // AI
    public ActionInfo CurrentAction = null;

    // Collision
    public bool IsTestingCollision = false;
    public bool IsInCollision = false;
    public TV_3DVECTOR CollisionStart = new TV_3DVECTOR();
    public TV_3DVECTOR CollisionEnd = new TV_3DVECTOR();
    public TV_3DVECTOR CollisionImpact = new TV_3DVECTOR();
    public TV_3DVECTOR CollisionNormal = new TV_3DVECTOR();
    public ActorInfo CollisionActor = null;
    public bool IsTestingProspectiveCollision = false;
    public bool IsInProspectiveCollision = false;
    public TV_3DVECTOR ProspectiveCollisionImpact = new TV_3DVECTOR();
    public TV_3DVECTOR ProspectiveCollisionNormal = new TV_3DVECTOR();
    public float ProspectiveCollisionScanDistance = 1000;

    // Utility
    public ActorState ActorState { get; set; }
    public ActorState prevActorState { get; private set; }

    public TV_3DVECTOR Scale { get; set; }
    public TV_3DVECTOR prevScale { get; private set; }

    public TV_3DVECTOR Position { get; set; }
    public TV_3DVECTOR PrevPosition { get; private set; }

    public TV_3DVECTOR Rotation { get; private set; }
    public TV_3DVECTOR PrevRotation { get; private set; }

    // Render
    public TVMesh Mesh { get; private set; }
    public TVMesh FarMesh { get; private set; }
    public int AttachToMesh = -1;

    public List<TVParticleSystem> ParticleSystems = new List<TVParticleSystem>();

    // Ownership
    private List<ActorInfo> Parent = new List<ActorInfo>();
    private List<ActorInfo> Children = new List<ActorInfo>();

    #region Creation Methods

    private ActorInfo(int id, ActorCreationInfo acinfo)
    {
      this.ID = id;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name.Length > 0) { _name = acinfo.Name; }

      IsCombatObject = acinfo.ActorTypeInfo.IsCombatObject;
      OnTimedLife = acinfo.ActorTypeInfo.OnTimedLife;
      TimedLife = acinfo.ActorTypeInfo.TimedLife;

      CreationState = acinfo.CreationState;
      CreationTime = acinfo.CreationTime;

      Faction = acinfo.Faction;
      ActorState = acinfo.InitialState;
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      Scale = acinfo.InitialScale;

      Strength = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : acinfo.ActorTypeInfo.MaxStrength;
      Speed = (acinfo.InitialSpeed > 0) ? acinfo.InitialSpeed : acinfo.ActorTypeInfo.MaxSpeed;

      PrevRotation = Rotation;
      prevScale = acinfo.InitialScale;
      PrevPosition = Position;

      TypeInfo.Initialize(this);
      TypeInfo.GenerateAddOns(this);
    }

    public void Generate()
    {
      Mesh = TypeInfo.GenerateMesh();
      //FarMesh = TypeInfo.GenerateFarMesh();

      SetLocalPosition(Position.x, Position.y, Position.z);
      SetLocalRotation(Rotation.x, Rotation.y, Rotation.z);

      Mesh.SetScale(Scale.x, Scale.y, Scale.z);
      Mesh.SetMeshName(ID.ToString());
      Mesh.SetTag(ID.ToString());
      Mesh.ComputeBoundings();
      //Mesh.ShowBoundingBox(true);

      //FarMesh.SetScale(Scale.x, Scale.y, Scale.z);
      //FarMesh.SetMeshName(ID.ToString());
      //FarMesh.SetTag(ID.ToString());
      //FarMesh.ComputeBoundings();

      Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      //FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      Mesh.Enable(true);

      Update();
      CreationState = CreationState.PREACTIVE;
      OnCreatedEvent( new object[] { this });
    }

    public static ActorInfo Create(ActorCreationInfo acinfo)
    {
      ActorInfo a;
      int ret = ActorFactory.Instance().Register(acinfo, out a);
      return a;
    }

    internal static ActorInfo FactoryCreate(int id, ActorCreationInfo acinfo)
    {
      return new ActorInfo(id, acinfo);
    }

    #endregion

    public void Process()
    {
      using (new PerfElement("actor_process"))
      {
        if (TypeInfo.NoProcess)
          return;

        if (Mesh != null)
        {
          using (new PerfElement("actor_process_checkalive"))
          {
            CheckAlive();
          }
          if (ActorState != ActorState.DEAD)
          {
            using (new PerfElement("actor_process_update"))
            {
              Update();
            }
            if (TypeInfo.CollisionEnabled || TypeInfo is ProjectileGroup)
            {
              using (new PerfElement("actor_process_collision"))
              {
                CheckCollision();
              }
            }
            using (new PerfElement("actor_process_move"))
            {
              Move();
            }
          }
        }
      }
      using (new PerfElement("actor_process_tick"))
      {
          OnTickEvent(new object[] { this, ActorState });
      }
    }

    public void ProcessAI()
    {
      if (TypeInfo.NoProcess 
        || TypeInfo.NoAI 
        || CreationState != CreationState.ACTIVE 
        || ActorState == ActorState.DYING 
        || ActorState == ActorState.DEAD
        || (IsPlayer() && !PlayerInfo.Instance().PlayerAIEnabled)
        )
        return;

      ActionManager.Run(this, CurrentAction);
    }

    public void ProcessCollision()
    {
      using (new PerfElement("testcollision"))
      {
        TestCollision();
      }
    }

    private void CheckAlive()
    {
      if (ActorState == ActorState.DEAD)
      {
        Destroy();
      }

      if (OnTimedLife)
      {
        if (TimedLife < 0f)
        {
          if (ActorState != ActorState.DYING && ActorState != ActorState.DEAD)
          {
            ActorState = ActorState.DYING;
          }
          else if (ActorState == ActorState.DYING)
          {
            ActorState = ActorState.DEAD;
          }
        }
        TimedLife -= Game.Instance().TimeSinceRender;
      }

      if (IsCombatObject)
      {
        if (Strength <= 0f)
        {
          if (ActorState != ActorState.DYING && ActorState != ActorState.DEAD)
          {
            ActorState = ActorState.DYING;
          }
          else if (ActorState == ActorState.DYING)
          {
            ActorState = ActorState.DEAD;
          }
        }
      }

      if (prevActorState == ActorState)
      {
        using (new PerfElement("actor_process_state"))
        {
          TypeInfo.ProcessState(this);
        }
      }
      else
      {
        using (new PerfElement("actor_process_newstate"))
        {
          TypeInfo.ProcessNewState(this);
          OnStateChangeEvent(new object[] { this, prevActorState });
          prevActorState = ActorState;
        }
      }
    }

    private void Update()
    {
      if (TypeInfo is TowerGunATI)
      {

      }

      PrevPosition = Position;
      PrevRotation = Rotation;

      //if (!IsAggregateMode())
      //{
        TV_3DVECTOR pos = GetPosition();
        TV_3DVECTOR rot = GetRotation();
        //if (!IsFarMode())
        //{
          Mesh.SetPosition(pos.x, pos.y, pos.z);
          Mesh.SetRotation(rot.x, rot.y, rot.z);
        //  Mesh.Enable(true);
        //}
        //else
        //{
        //  FarMesh.SetPosition(pos.x, pos.y, pos.z);
        //  FarMesh.SetRotation(rot.x, rot.y, rot.z);
        //  FarMesh.Enable(true);
        //}

        if (Scale.x != prevScale.x || Scale.y != prevScale.y || Scale.z != prevScale.z)
        {
          prevScale = new TV_3DVECTOR(Scale.x, Scale.y, Scale.z);
          Mesh.SetScale(Scale.x, Scale.y, Scale.z);
          //FarMesh.SetScale(Scale.x, Scale.y, Scale.z);
        }
      //}

      if (CreationState == CreationState.PREACTIVE)
        CreationState = CreationState.ACTIVE;
    }

    public void Render()
    {
      if (TypeInfo.NoRender || CreationState != CreationState.ACTIVE)
      {
        return;
      }

      if (!IsAggregateMode())
      {

        using (new PerfElement("actor_render"))
        {
          //if (!IsFarMode())
          //{
          if (Mesh != null && Mesh.IsVisible())
          {
            using (new PerfElement("mesh_render"))
            {
              Mesh.Render();
            }
          }
          //}
          //else
          //{
          //  if (FarMesh != null)
          //  {
          //    FarMesh.Render();
          //  }
          //}

          foreach (TVParticleSystem p in ParticleSystems)
            p.Render();
        }
      }
    }

    private void CheckCollision()
    {
      IsTestingCollision = false;
      if (!PrevPosition.Equals(new TV_3DVECTOR()) && !GetPosition().Equals(new TV_3DVECTOR()))
      {
        // only check player and projectiles
        if ((IsPlayer() && !PlayerInfo.Instance().PlayerAIEnabled) || TypeInfo is ProjectileGroup || (ActorState == ActorState.DYING && TypeInfo.IsFighter))
        {
          if (IsInCollision)
          {
            CollisionActor.TypeInfo.ProcessHit(CollisionActor, this, CollisionImpact, CollisionNormal);
            TypeInfo.ProcessHit(this, CollisionActor, CollisionImpact, -1 * CollisionNormal);
            IsInCollision = false;
          }

          IsTestingCollision = true;
          CollisionEnd = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z);
          CollisionStart = GetRelativePositionXYZ(0, 0, TypeInfo.min_dimensions.z) + PrevPosition - Position;
        }
      }
    }

    private void TestCollision()
    {
      if (IsTestingCollision)
      {
        using (new PerfElement("collision_combat_" + Name.PadRight(15).Substring(0, 13)))
        {
          // only check player and projectiles
          //if ((IsPlayer() && !PlayerInfo.Instance().PlayerAIEnabled) || TypeInfo is ProjectileGroup)
          {
            TV_3DVECTOR vmin = new TV_3DVECTOR();
            TV_3DVECTOR vmax = new TV_3DVECTOR();

            vmax = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z);
            vmin = GetRelativePositionXYZ(0, 0, TypeInfo.min_dimensions.z) + PrevPosition - Position;

            IsInCollision = TestCollision(vmin, vmax, true, out CollisionImpact, out CollisionNormal, out CollisionActor);
          }
          IsTestingCollision = false;
        }
      }
      if (IsTestingProspectiveCollision)
      {
        using (new PerfElement("collision_prospect"))
        {
          ActorInfo dummy;

          TV_3DVECTOR prostart = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z + 10);
          TV_3DVECTOR proend0 = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z + 10 + ProspectiveCollisionScanDistance);

          IsInProspectiveCollision = TestCollision(prostart
                                                 , proend0
                                                 , false, out ProspectiveCollisionImpact, out ProspectiveCollisionNormal, out dummy);
          /*
          if (IsInProspectiveCollision)
          {
          //bool proc = false;
            float distcheck = 0;
            TV_3DVECTOR safepos = new TV_3DVECTOR();
            TV_3DVECTOR impactvec;
            TV_3DVECTOR dummyvec;
            for (int x = -1; x <= 1; x++)
              for (int y = -1; y <= 1; y++)
              {
                TV_3DVECTOR proend = GetRelativePositionXYZ(x * ProspectiveCollisionScanDistance * 0.5f, y * ProspectiveCollisionScanDistance * 0.5f, TypeInfo.max_dimensions.z + 10 + ProspectiveCollisionScanDistance * 5);
                if (TestCollision(prostart
                                , proend
                                , false, out impactvec, out dummyvec, out dummy))
                {
                  float d = ActorDistanceInfo.GetRoughDistance(prostart, proend);
                  if (distcheck < d)
                  {
                    safepos = proend;
                    distcheck = d;
                  //proc = true;
                  }
                }
                else
                {
                  safepos = proend;
                  distcheck = 999999;
                  x = 10;
                  y = 10;
                }
              }
            ProspectiveCollisionGoodLocation = safepos;
          //IsInProspectiveCollision = proc;
          }
          */
          IsTestingProspectiveCollision = false;
        }
      }
    }

    private bool TestCollision(TV_3DVECTOR start, TV_3DVECTOR end, bool getActorInfo, out TV_3DVECTOR vImpact, out TV_3DVECTOR vNormal, out ActorInfo actor)
    {
      using (new PerfElement("fn_testcollision"))
      {
        try
        {
          actor = null;
          vImpact = new TV_3DVECTOR();
          vNormal = new TV_3DVECTOR();

          TV_COLLISIONRESULT tvcres = new TV_COLLISIONRESULT();

          if (Engine.Instance().TVScene.AdvancedCollision(start, end, ref tvcres))
          {
            vImpact = new TV_3DVECTOR(tvcres.vCollisionImpact.x, tvcres.vCollisionImpact.y, tvcres.vCollisionImpact.z);
            vNormal = new TV_3DVECTOR(tvcres.vCollisionNormal.x, tvcres.vCollisionNormal.y, tvcres.vCollisionNormal.z);

            if (getActorInfo)
            {
              TVMesh tvm = Engine.Instance().TVGlobals.GetMeshFromID(tvcres.iMeshID);
              if (tvm != null)
              {
                int n = 0;
                if (int.TryParse(tvm.GetTag(), out n))
                {
                  actor = ActorFactory.Instance().GetActor(n);
                  return (actor != null && actor != this && !HasRelative(actor) && actor.TypeInfo.CollisionEnabled);
                }
              }
            }
            return true;
          }
          return false;
        }
        catch
        {
          actor = null;
          vImpact = new TV_3DVECTOR();
          vNormal = new TV_3DVECTOR();
          return false;
        }
      }
    }

    public void FireWeapon(ActorInfo target, string weapon)
    {
      if (ActorState != ActorState.DYING && ActorState != ActorState.DEAD)
      {
        TypeInfo.FireWeapon(this, target, weapon);
      }
    }

    private float lastZrot = 0;
    private float Zdiv = 0;
    private void Move()
    {
      //Is fixed
      if (ActorState == ActorState.FIXED || TypeInfo.NoMove)
        return;

      if (TypeInfo is TowerGunATI)
      {

      }

      // Hyperspace special: AI loop may not be in sync
      if (ActorState == ActorState.HYPERSPACE)
      {
        if (CurrentAction is Actions.HyperspaceIn)
        {
          ((Actions.HyperspaceIn)CurrentAction).ApplyMove(this);
          /*
          LookAtPoint(AI.Master.Move_TargetPosition);
          float d = Engine.Instance().TVMathLibrary.GetDistanceVec3D(GetPosition(), AI.Master.Move_TargetPosition);

          if (d > 5000)
            Speed = d * 2.5f;
          else if (d > 1000)
            Speed = d * (0.4f + (2.1f * (d - 1000) / 4000));
          else
            Speed = d * 0.4f; //* 2.5f;
          if (Speed < TypeInfo.MaxSpeed)
            Speed = TypeInfo.MaxSpeed;
          */
        }
        else if (CurrentAction is Actions.HyperspaceOut)
        {
          ((Actions.HyperspaceOut)CurrentAction).ApplyMove(this);
          /*
          LookAtPoint(AI.Master.Move_TargetPosition);
          Speed += 20000 * Game.Instance().TimeSinceRender;
          if (Speed > 20000)
          {
            Speed = 20000;
          }
          */
        }
        MoveRelative(Speed * Game.Instance().TimeSinceRender, 0, 0);
        return;
      }

      // Limit speed
      if (ActorState != ActorState.FREE && ActorState != ActorState.HYPERSPACE)
      {
        if (Speed > MaxSpeed)
          Speed = MaxSpeed;
        else if (Speed < MinSpeed)
          Speed = MinSpeed;
      }

      if (ApplyZBalance)
      {
        TV_3DVECTOR vec = GetRotation();
        SetRotation(vec.x, vec.y, 0);
        MoveRelative(Speed * Game.Instance().TimeSinceRender, 0, 0);
        lastZrot -= YTurnAngle * ZTilt * Game.Instance().TimeSinceRender;
        Zdiv += Game.Instance().TimeSinceRender / 0.005f;
        while (Zdiv > 0)
        {
          lastZrot *= 1 - ZNormFrac;
          Zdiv--;
        }
        float rotX2 = vec.x + XTurnAngle * Game.Instance().TimeSinceRender;
        if (rotX2 > TypeInfo.XLimit)
          rotX2 = TypeInfo.XLimit;
        else if (rotX2 < -TypeInfo.XLimit)
          rotX2 = -TypeInfo.XLimit;

        float rotY2 = vec.y + YTurnAngle * Game.Instance().TimeSinceRender;

        //LocalRotation = new TV_3DVECTOR(rotX2, rotY2, lastZrot);

        SetRotation(rotX2, rotY2, lastZrot);
      }
      else
      {
        TV_3DVECTOR vec = GetRotation();
        MoveRelative(Speed * Game.Instance().TimeSinceRender, 0, 0);
        lastZrot = vec.z;
        lastZrot -= YTurnAngle * ZTilt * Game.Instance().TimeSinceRender;
        float rotX2 = vec.x + XTurnAngle * Game.Instance().TimeSinceRender;
        if (rotX2 > TypeInfo.XLimit)
          rotX2 = TypeInfo.XLimit;
        else if (rotX2 < -TypeInfo.XLimit)
          rotX2 = -TypeInfo.XLimit;

        float rotY2 = vec.y + YTurnAngle * Game.Instance().TimeSinceRender;
        SetRotation(rotX2, rotY2, lastZrot);
      }
    }

    #region Position / Rotation
    public TV_3DVECTOR GetPosition()
    {
      TV_3DVECTOR ret = Position;
      ActorInfo a = ActorFactory.Instance().GetActor(AttachToMesh);
      if (a != null)
        ret = a.GetRelativePositionXYZ(ret.x, ret.y, ret.z);

      return ret;
    }

    /*
    public void SetLocalPosition(float x, float y, float z) /// WRONG IMPLEMENTATION
    {
      TV_3DVECTOR pos = new TV_3DVECTOR(x, y, z);
      if (AttachToMesh > -1)
        pos -= ActorFactory.Instance().GetActor(AttachToMesh).GetPosition();

      Position = pos;
    }
    */

    public TV_3DVECTOR GetLocalPosition()
    {
      return Position;
    }

    public void SetLocalPosition(float x, float y, float z)
    {
      TV_3DVECTOR pos = new TV_3DVECTOR(x, y, z);
      Position = pos;
    }

    public TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false)
    {
      TV_3DVECTOR pos = GetPosition();
      TV_3DVECTOR rot = GetRotation();
      if (uselocal)
      {
        pos = GetLocalPosition();
        rot = GetLocalRotation();
      }

      TV_3DVECTOR ret = new TV_3DVECTOR();

      Engine.Instance().TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, front), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool uselocal = false)
    {
      TV_3DVECTOR pos = GetPosition();
      TV_3DVECTOR rot = GetRotation();
      if (uselocal)
      {
        pos = GetLocalPosition();
        rot = GetLocalRotation();
      }

      TV_3DVECTOR ret = new TV_3DVECTOR();

      Engine.Instance().TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(x, y, z), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRotation()
    {
      TV_3DVECTOR ret = Rotation;
      ActorInfo a = ActorFactory.Instance().GetActor(AttachToMesh);
      if (a != null)
        ret += a.GetRotation();

      return ret;
    }

    public void SetRotation(float x, float y, float z)
    {
      TV_3DVECTOR rot = new TV_3DVECTOR(x, y, z);
      ActorInfo a = ActorFactory.Instance().GetActor(AttachToMesh);
      if (a != null)
        rot -= a.GetRotation();

      Rotation = rot;
    }

    public TV_3DVECTOR GetLocalRotation()
    {
      return Rotation;
    }

    public void SetLocalRotation(float x, float y, float z)
    {
      Rotation = new TV_3DVECTOR(x, y, z);
    }

    public TV_3DVECTOR GetDirection()
    {
      TV_3DVECTOR ret = Utilities.GetDirection(Rotation);
      ActorInfo a = ActorFactory.Instance().GetActor(AttachToMesh);
      if (a != null)
        ret += a.GetDirection();

      TV_3DVECTOR dir = new TV_3DVECTOR();
      Engine.Instance().TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      ActorInfo a = ActorFactory.Instance().GetActor(AttachToMesh);
      if (a != null)
        dir -= a.GetDirection();

      Rotation = Utilities.GetRotation(dir);
    }

    public TV_3DVECTOR GetLocalDirection()
    {
      TV_3DVECTOR ret = Utilities.GetDirection(Rotation);

      TV_3DVECTOR dir = new TV_3DVECTOR();
      Engine.Instance().TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetLocalDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      Rotation = Utilities.GetRotation(dir);
    }

    public void LookAtPoint(TV_3DVECTOR target, bool preserveZrotation = false)
    {
      float zrot = GetRotation().z;
      TV_3DVECTOR dir = target - GetPosition();
      SetDirection(dir.x, dir.y, dir.z);

      if (preserveZrotation)
      {
        SetRotation(GetRotation().x, GetRotation().y, zrot);
      }
    }

    public void MoveRelative(float front, float up, float right)
    {
      TV_3DVECTOR vec = GetRelativePositionFUR(front, up, right, true);
      SetLocalPosition(vec.x, vec.y, vec.z);
    }

    public void MoveAbsolute(float x, float y, float z)
    {
      TV_3DVECTOR vec = GetLocalPosition() + new TV_3DVECTOR(x, y, z);
      SetLocalPosition(vec.x, vec.y, vec.z);
    }

    #endregion

    public float GetTrueSpeed()
    {
      float ret = Speed;
      ActorInfo a = ActorFactory.Instance().GetActor(AttachToMesh);
      if (a != null)
        ret += a.GetTrueSpeed();

      return ret;
    }

    public void GetBoundingBox(ref TV_3DVECTOR min, ref TV_3DVECTOR max)
    {
      if (Mesh == null)
        return;

      Mesh.GetBoundingBox(ref min, ref max);
    }

    public void GetBoundingBox(ref TV_3DVECTOR min, ref TV_3DVECTOR max, bool localmode)
    {
      if (Mesh == null)
        return;

      Mesh.GetBoundingBox(ref min, ref max, localmode);
    }

    public void GetBoundingSphere(ref TV_3DVECTOR retCenter, ref float radius)
    {
      if (Mesh == null)
        return;

      Mesh.GetBoundingSphere(ref retCenter, ref radius);
    }

    public void GetBoundingSphere(ref TV_3DVECTOR retCenter, ref float radius, bool localmode)
    {
      if (Mesh == null)
        return;

      Mesh.GetBoundingSphere(ref retCenter, ref radius, localmode);
    }

    public void SetTexture(int iTexture)
    {
      if (Mesh == null)
        return;

      Mesh.SetTexture(iTexture);
    }

    public List<TV_3DVECTOR> GetVertices()
    {
      if (Mesh == null)
        return new List<TV_3DVECTOR>();

      List<TV_3DVECTOR> Vertices = new List<TV_3DVECTOR>();
      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;
      for (int r = 0; r < Mesh.GetVertexCount(); r += 1 + Mesh.GetVertexCount() / 100) //??
      {
        Mesh.GetVertex(r, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
        Vertices.Add(new TV_3DVECTOR(x, y, z));
      }

      return Vertices;
    }

    public void Rotate(float x, float y, float z)
    {
      TV_3DVECTOR vec = GetRotation();
      SetRotation(vec.x + x, vec.y + y, vec.z + z);
    }




    #region CustomStates
    public bool IsStateFDefined(string key)
    {
      return StatesF.ContainsKey(key);
    }

    public string[] GetStateFKeys()
    {
      string[] ret = new string[StatesF.Count];
      StatesF.Keys.CopyTo(ret, 0);
      return ret;
    }

    public void SetStateF(string key, float value)
    {
      if (StatesF.ContainsKey(key))
      {
        StatesF[key] = value;
      }
      else
      {
        StatesF.Add(key, value);
      }
    }

    public float GetStateF(string key, float defaultvalue = 0)
    {
      return (StatesF.ContainsKey(key)) ? StatesF[key] : defaultvalue;
    }

    public bool IsStateBDefined(string key)
    {
      return StatesB.ContainsKey(key);
    }

    public string[] GetStateBKeys()
    {
      string[] ret = new string[StatesB.Count];
      StatesB.Keys.CopyTo(ret, 0);
      return ret;
    }

    public void SetStateB(string key, bool value)
    {
      if (StatesB.ContainsKey(key))
      {
        StatesB[key] = value;
      }
      else
      {
        StatesB.Add(key, value);
      }
    }

    public bool GetStateB(string key, bool defaultvalue = false)
    {
      return (StatesB.ContainsKey(key)) ? StatesB[key] : defaultvalue;
    }

    public bool IsStateSDefined(string key)
    {
      return StatesS.ContainsKey(key);
    }

    public string[] GetCustomStateSKeys()
    {
      string[] ret = new string[StatesS.Count];
      StatesS.Keys.CopyTo(ret, 0);
      return ret;
    }

    public void SetStateS(string key, string value)
    {
      if (StatesS.ContainsKey(key))
      {
        StatesS[key] = value;
      }
      else
      {
        StatesS.Add(key, value);
      }
    }

    public string GetStateS(string key, string defaultvalue = "")
    {
      return (StatesS.ContainsKey(key)) ? StatesS[key] : defaultvalue;
    }

    #endregion

    #region Parent, Child and Relatives

    private Mutex m_parentlist = new Mutex();
    private Mutex m_childlist = new Mutex();

    public void AddParent(ActorInfo actor)
    {
      //m_parentlist.WaitOne();
      Parent.Add(actor);
      actor.Children.Add(this);
      //m_parentlist.ReleaseMutex();
    }

    public void RemoveParent(ActorInfo actor)
    {
      //m_parentlist.WaitOne();
      Parent.Remove(actor);
      actor.Children.Remove(this);
      //m_parentlist.ReleaseMutex();
    }

    public void AddChild(ActorInfo actor)
    {
      //m_childlist.WaitOne();
      Children.Add(actor);
      actor.Parent.Add(this);
      //m_childlist.ReleaseMutex();
    }

    public void RemoveChild(ActorInfo actor)
    {
      //m_childlist.WaitOne();
      Children.Remove(actor);
      actor.Parent.Remove(this);
      //m_childlist.ReleaseMutex();
    }

    public bool HasParent(ActorInfo a, int searchlevel = 99)
    {
      if (searchlevel < 0)
        return false;

      bool ret = false;
      //m_parentlist.WaitOne();
      List<ActorInfo> ps = new List<ActorInfo>(Parent);
      //m_parentlist.ReleaseMutex();

      foreach (ActorInfo p in ps)
        ret |= (p == a) || (p.HasParent(a, searchlevel-1));

      return ret;
    }

    public List<ActorInfo> GetAllParents(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<ActorInfo>();

      List<ActorInfo> ret = new List<ActorInfo>();
      //m_parentlist.WaitOne();
      List<ActorInfo> ps = new List<ActorInfo>(Parent);
      //m_parentlist.ReleaseMutex();

      if (searchlevel < 1)
      {
        foreach (ActorInfo p in ps)
        {
          if (!ret.Contains(p))
          {
            ret.Add(p);
            ret.AddRange(p.GetAllParents(searchlevel - 1));
          }
        }
      }
      else
      {
        ret.AddRange(ps);
      }
      return ret;
    }

    public bool HasChild(ActorInfo a, int searchlevel = 99)
    {
      if (searchlevel < 0)
        return false;

      bool ret = false;
      //m_childlist.WaitOne();
      List<ActorInfo> cs = new List<ActorInfo>(Children);
      //m_childlist.ReleaseMutex();

      foreach (ActorInfo p in cs)
        ret |= (p == a) || (p.HasChild(a, searchlevel-1));

      return ret;
    }

    public List<ActorInfo> GetAllChildren(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<ActorInfo>();

      List<ActorInfo> ret = new List<ActorInfo>();
      //m_childlist.WaitOne();
      List<ActorInfo> cs = new List<ActorInfo>(Children);
      //m_childlist.ReleaseMutex();

      if (searchlevel < 1)
      {
        foreach (ActorInfo p in cs)
        {
          if (!ret.Contains(p))
          {
            ret.Add(p);
            ret.AddRange(p.GetAllChildren(searchlevel - 1));
          }
        }
      }
      else
      {
        ret.AddRange(cs);
      }
      return ret;
    }

    public bool HasRelative(ActorInfo a, int searchlevel = 99, List<ActorInfo> alreadysearched = null)
    {
      if (searchlevel < 0)
        return false;

      if (alreadysearched == null)
        alreadysearched = new List<ActorInfo>();

      bool ret = false;
      //m_parentlist.WaitOne();
      List<ActorInfo> ps = new List<ActorInfo>(Parent);
      //m_parentlist.ReleaseMutex();
      //m_childlist.WaitOne();
      List<ActorInfo> cs = new List<ActorInfo>(Children);
      //m_childlist.ReleaseMutex();

      foreach (ActorInfo p in ps)
      {
        if (!alreadysearched.Contains(p))
        {
          alreadysearched.Add(p);
          ret |= (p == a) || (p.HasRelative(a, searchlevel - 1, alreadysearched));
        }
      }
      foreach (ActorInfo p in cs)
      {
        if (!alreadysearched.Contains(p))
        {
          alreadysearched.Add(p);
          ret |= (p == a) || (p.HasRelative(a, searchlevel - 1, alreadysearched));
        }
      }
      return ret;
    }

    public List<ActorInfo> GetAllRelatives(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<ActorInfo>();

      List<ActorInfo> ret = new List<ActorInfo>();
      //m_parentlist.WaitOne();
      List<ActorInfo> ps = new List<ActorInfo>(Parent);
      //m_parentlist.ReleaseMutex();
      //m_childlist.WaitOne();
      List<ActorInfo> cs = new List<ActorInfo>(Children);
      //m_childlist.ReleaseMutex();

      if (searchlevel < 1)
      {
        foreach (ActorInfo p in ps)
        {
          if (!ret.Contains(p))
          {
            ret.Add(p);
            ret.AddRange(p.GetAllRelatives(searchlevel - 1));
          }
        }
      }
      else
      {
        ret.AddRange(ps);
      }

      if (searchlevel < 1)
      {
        foreach (ActorInfo p in cs)
        {
          if (!ret.Contains(p))
          {
            ret.Add(p);
            ret.AddRange(p.GetAllRelatives(searchlevel - 1));
          }
        }
      }
      else
      {
        ret.AddRange(cs);
      }
      return ret;
    }

    #endregion

    #region Event Methods
    public void OnTickEvent(object[] param)
    {
      foreach (string name in TickEvents)
      {
        GameEvent.RunEvent(name, param);
      }
    }

    public void OnHitEvent(object[] param)
    {
      foreach (string name in HitEvents)
      {
        GameEvent.RunEvent(name, param);
      }
    }

    public void OnStateChangeEvent(object[] param)
    {
      foreach (string name in ActorStateChangeEvents)
      {
        GameEvent.RunEvent(name, param);
      }
    }

    public void OnCreatedEvent(object[] param)
    {
      foreach (string name in CreatedEvents)
      {
        GameEvent.RunEvent(name, param);
      }
    }

    public void OnDestroyedEvent(object[] param)
    {
      foreach (string name in DestroyedEvents)
      {
        GameEvent.RunEvent(name, param);
      }
    }

    #endregion

    public bool IsOutOfBounds(TV_3DVECTOR minbounds, TV_3DVECTOR maxbounds)
    {
      if (GetPosition().x < minbounds.x)
      {
        return true;
      }
      else if (GetPosition().x > maxbounds.x)
      {
        return true;
      }

      if (GetPosition().y < minbounds.y)
      {
        return true;
      }
      else if (GetPosition().y > maxbounds.y)
      {
        return true;
      }

      if (GetPosition().z < minbounds.z)
      {
        return true;
      }
      else if (GetPosition().z > maxbounds.z)
      {
        return true;
      }
      return false;
    }

    public bool IsNearlyOutOfBounds(float dx = 1000, float dy = 250, float dz = 1000)
    {
      if (GetPosition().x < GameScenarioManager.Instance().MinBounds.x + dx)
      {
        return true;
      }
      else if (GetPosition().x > GameScenarioManager.Instance().MaxBounds.x - dx)
      {
        return true;
      }

      if (GetPosition().y < GameScenarioManager.Instance().MinBounds.y + dy)
      {
        return true;
      }
      else if (GetPosition().y > GameScenarioManager.Instance().MaxBounds.y - dy)
      {
        return true;
      }

      if (GetPosition().z < GameScenarioManager.Instance().MinBounds.z + dz)
      {
        return true;
      }
      else if (GetPosition().z > GameScenarioManager.Instance().MaxBounds.z - dz)
      {
        return true;
      }
      return false;
    }


    public bool CanTarget(ActorInfo target, bool checkRange, bool checkAngularRange)
    {
      if (IsPlayer())
      {
        return true;
      }

      WeaponInfo weapon = null;
      int burst = 0;
      foreach (string ws in AIWeapons)
      {
        TypeInfo.InterpretWeapon(this, ws, out weapon, out burst);

        if (weapon != null)
        {
          if (weapon.CanTarget(this, target))
          {
            return true;
          }
        }
      }
      return false;
    }

    public float GetWeaponRange()
    {
      float ret = 0;
      foreach (WeaponInfo w in Weapons.Values)
      {
        if (ret < w.Range)
          ret = w.Range;
      }
      return ret;
    }

    public bool SelectWeapon(ActorInfo target, float delta_angle, float delta_distance, out WeaponInfo weapon, out int burst)
    {
      weapon = null;
      burst = 0;
      WeaponInfo weap = null;
      //float dist = ActorDistanceInfo.GetDistance(owner, target);
      foreach (string ws in AIWeapons)
      {
        TypeInfo.InterpretWeapon(this, ws, out weap, out burst);

        if (weap != null)
        {
          if ((delta_angle < weap.AngularRange
            && delta_angle > -weap.AngularRange)
            && (delta_distance < weap.Range
            && delta_distance > -weap.Range)
            && weap.CanTarget(this, target))
          {
            weapon = weap;
            return true;
          }
        }
      }
      return false;
    }

    public bool IsAggregateMode()
    {
      ActorInfo a = (PlayerInfo.Instance().Actor != null) ? PlayerInfo.Instance().Actor : GameScenarioManager.Instance().SceneCamera;
      float distcheck = TypeInfo.CullDistance * Game.Instance().PerfCullModifier;

      return (!IsPlayer() && TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetRoughDistance(this, a) > distcheck);
      //return (!IsPlayer() && TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetDistance(this, a, distcheck + 1) > distcheck);
    }

    public bool IsFarMode()
    {
      ActorInfo a = (PlayerInfo.Instance().Actor != null) ? PlayerInfo.Instance().Actor : GameScenarioManager.Instance().SceneCamera;
      float distcheck = TypeInfo.CullDistance * 0.5f *Game.Instance().PerfCullModifier;

      return (!IsPlayer() && TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetRoughDistance(this, a) > distcheck);
      //return (!IsPlayer() && TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetDistance(this, a, distcheck + 1) > distcheck);
    }

    public bool IsPlayer()
    {
      return this == PlayerInfo.Instance().Actor;
    }

    public float StrengthFrac
    {
      get
      {
        float f = Strength / TypeInfo.MaxStrength;
        if (f > 1)
          f = 1;
        else if (f < 0)
          f = 0;
        return f;
      }
    }

    public void Destroy()
    {
      using (new PerfElement("actor_process_destroy"))
      {
        while (Parent.Count > 0)
        {
          RemoveParent(Parent[0]);
        }
        while (Children.Count > 0)
        {
          if (Children[0].TypeInfo is AddOnGroup || Children[0].AttachToMesh == this.ID)
          {
            Children[0].Destroy();
          }
          else
          {
            RemoveChild(Children[0]);
          }
        }

        if (Mesh != null)
        {
          Mesh.Destroy();
        }

        Mesh = null;

        OnDestroyedEvent(new object[] { this, ActorState });

        CreatedEvents.Clear();
        DestroyedEvents.Clear();
        TickEvents.Clear();
        HitEvents.Clear();
        ActorStateChangeEvents.Clear();
        CreationState = CreationState.DISPOSED;
        ActorFactory.Instance().RemoveActor(ID);
      }
    }
  }
}
