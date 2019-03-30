using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.Actors.Types;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public class ActorInfo
  {
    public ActorTypeInfo TypeInfo { get; private set; }
    public SpawnerInfo SpawnerInfo { get; set; }

    // Identifiers
    private string _name = "New Actor";
    private string sidebar_name = "";

    public string Name { get { return _name; } }
    public string SideBarName { get { return (sidebar_name.Length == 0) ? _name : sidebar_name; } set { sidebar_name = value; } }
    public int ID { get; private set; }
    public string Key { get { return _name + " " + ID; } }

    public override string ToString()
    {
      return string.Format("ACTOR:{0},{1}", ID, _name);
    }

    // Creation and Disposal
    public float CreationTime = 0;
    public CreationState CreationState = CreationState.PLANNED;

    // Faction
    private FactionInfo _faction;
    public FactionInfo Faction
    {
      get
      {
        if (_faction == null)
        {
          _faction = FactionInfo.Neutral;
          _faction.RegisterActor(this);
        }
        return _faction;
      }
      set
      {
        if (_faction != null)
          _faction.UnregisterActor(this);
        _faction = value;
        if (_faction == null)
          _faction = FactionInfo.Neutral;
        _faction.RegisterActor(this);
      }
    }

    // Scoring
    public ScoreInfo Score;

    // Combat
    public readonly CombatInfo CombatInfo;

    // Movement
    public readonly MovementInfo MovementInfo;

    // Cycles
    public readonly CycleInfo CycleInfo;

    // Checks
    public bool EnteredCombatZone = false;
    public float LastProcessStateUpdateTime = 0;

    // Weapons System
    public Dictionary<string, WeaponInfo> Weapons = new Dictionary<string, WeaponInfo>();
    public string[] PrimaryWeapons = new string[0];
    public string[] SecondaryWeapons = new string[0];
    public string[] AIWeapons = new string[0];

    // Camera System
    public TV_3DVECTOR DefaultCamLocation = new TV_3DVECTOR();
    public TV_3DVECTOR DefaultCamTarget = new TV_3DVECTOR(0, 0, 2000);
    public List<TV_3DVECTOR> CamLocations = new List<TV_3DVECTOR>();
    public List<TV_3DVECTOR> CamTargets = new List<TV_3DVECTOR>();

    public float CamDeathCircleRadius = 350;
    public float CamDeathCircleHeight = 25;
    public float CamDeathCirclePeriod = 15;

    // Regeneration
    public readonly RegenerationInfo RegenerationInfo;

    // Explosion Systems
    public readonly ExplosionInfo ExplosionInfo;

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
    public bool CanEvade = true;
    public bool CanRetaliate = true;
    public int HuntWeight = 1;

    // Collision
    public bool IsTestingCollision = false;
    public bool IsInCollision = false;
    public TV_3DVECTOR CollisionImpact = new TV_3DVECTOR();
    public TV_3DVECTOR CollisionNormal = new TV_3DVECTOR();
    public ActorInfo CollisionActor = null;
    public bool IsTestingProspectiveCollision = false;
    public bool IsInProspectiveCollision = false;
    public ActorInfo ProspectiveCollisionActor = null;
    public TV_3DVECTOR ProspectiveCollisionImpact = new TV_3DVECTOR();
    public TV_3DVECTOR ProspectiveCollisionNormal = new TV_3DVECTOR();
    public TV_3DVECTOR ProspectiveCollisionSafe = new TV_3DVECTOR();
    public float ProspectiveCollisionScanDistance = 1000;
    public float ProspectiveCollisionLevel = 0;
    public bool IsAvoidingCollision = false;

    // Utility
    public ActorState ActorState { get; set; }
    public ActorState prevActorState { get; private set; }

    public TV_3DVECTOR Scale { get; set; }
    public TV_3DVECTOR prevScale { get; private set; }

    public TV_3DVECTOR Position { get; set; }
    public Cached3DVector WorldPosition;
    public TV_3DVECTOR PrevPosition { get; private set; }

    public TV_3DVECTOR Rotation { get; private set; }
    public Cached3DVector WorldRotation;


    // Render
    public TVMesh Mesh { get; private set; }
    public TVMesh FarMesh { get; private set; }
    public int AttachToMesh = -1;

    public TVParticleSystem ParticleSystem = null;

    // Ownership
    public ActorInfo Parent { get; private set; }
    private List<ActorInfo> Children = new List<ActorInfo>();

    #region Creation Methods

    private ActorInfo(int id, ActorCreationInfo acinfo)
    {
      ID = id;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name.Length > 0) { _name = acinfo.Name; }

      // Components
      Score = new ScoreInfo(_name);
      CombatInfo = new CombatInfo(this);
      MovementInfo = new MovementInfo(this);
      CycleInfo = new CycleInfo(this, null);
      RegenerationInfo = new RegenerationInfo(this);
      ExplosionInfo = new ExplosionInfo(this);

      // Creation
      CreationState = acinfo.CreationState;
      CreationTime = acinfo.CreationTime;

      // Combat
      CombatInfo.IsCombatObject = acinfo.ActorTypeInfo.IsCombatObject;
      CombatInfo.OnTimedLife = acinfo.ActorTypeInfo.OnTimedLife;
      CombatInfo.TimedLife = acinfo.ActorTypeInfo.TimedLife;
      CombatInfo.Strength = (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : acinfo.ActorTypeInfo.MaxStrength;

      Faction = acinfo.Faction;
      ActorState = acinfo.InitialState;
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      Scale = acinfo.InitialScale;

      MovementInfo.Speed = (acinfo.InitialSpeed > 0) ? acinfo.InitialSpeed : acinfo.ActorTypeInfo.MaxSpeed;
      prevScale = acinfo.InitialScale;
      PrevPosition = Position;

      HuntWeight = acinfo.ActorTypeInfo.HuntWeight;

      TypeInfo.Initialize(this);
      TypeInfo.GenerateAddOns(this);
    }

    public void Generate()
    {
      Mesh = TypeInfo.GenerateMesh();
      FarMesh = TypeInfo.GenerateFarMesh();

      SetLocalPosition(Position.x, Position.y, Position.z);
      SetLocalRotation(Rotation.x, Rotation.y, Rotation.z);

      Mesh.SetScale(Scale.x, Scale.y, Scale.z);
      Mesh.SetMeshName(ID.ToString());
      Mesh.SetTag(ID.ToString());
      Mesh.ComputeBoundings();
      //Mesh.ShowBoundingBox(true);

      FarMesh.SetScale(Scale.x, Scale.y, Scale.z);
      FarMesh.SetMeshName(ID.ToString());
      FarMesh.SetTag(ID.ToString());
      FarMesh.ComputeBoundings();

      Mesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      FarMesh.SetLightingMode(CONST_TV_LIGHTINGMODE.TV_LIGHTING_MANAGED, 8);
      Mesh.Enable(true);
      FarMesh.Enable(true);

      CreationState = CreationState.PREACTIVE;
      Update();
      OnCreatedEvent(new object[] { this });

      //SpawnerInfo = new SpawnerInfo(this);
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

        if (ActorState != ActorState.DEAD)
          Update();

        CycleInfo.Process();

        if (Mesh != null)
        {
          CheckState();
          if (ActorState != ActorState.DEAD)
          {
            if (TypeInfo.CollisionEnabled || TypeInfo is ProjectileGroup)
                CheckCollision();
            MovementInfo.Move();
          }
        }
      }
      OnTickEvent(new object[] { this, ActorState });
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
      TestCollision();
    }

    private void CheckState()
    {
      CombatInfo.Process();

      if (prevActorState == ActorState)
      {
        TypeInfo.ProcessState(this);
      }
      else
      {
        TypeInfo.ProcessNewState(this);
        OnStateChangeEvent(new object[] { this, prevActorState });
        prevActorState = ActorState;
      }

      if (ActorState == ActorState.DEAD)
        Destroy();
    }

    private void Update()
    {
      PrevPosition = Position;

      if (Mesh == null)
        return;

      TV_3DVECTOR pos = GetPosition();
      TV_3DVECTOR rot = GetRotation();

      Mesh.SetPosition(pos.x, pos.y, pos.z);
      Mesh.SetRotation(rot.x, rot.y, rot.z);
      FarMesh.SetPosition(pos.x, pos.y, pos.z);
      FarMesh.SetRotation(rot.x, rot.y, rot.z);

      Mesh.SetCollisionEnable(!IsAggregateMode() && TypeInfo.CollisionEnabled);
      FarMesh.SetCollisionEnable(false);

      if (Scale.x != prevScale.x || Scale.y != prevScale.y || Scale.z != prevScale.z)
      {
        prevScale = new TV_3DVECTOR(Scale.x, Scale.y, Scale.z);
        Mesh.SetScale(Scale.x, Scale.y, Scale.z);
        FarMesh.SetScale(Scale.x, Scale.y, Scale.z);
      }

      if (CreationState == CreationState.PREACTIVE)
      {
        CreationState = CreationState.ACTIVE;
        PrevPosition = Position;
      }

      if (ParticleSystem != null)
        ParticleSystem.Update();
    }

    public void Render()
    {
      if (TypeInfo.NoRender || CreationState != CreationState.ACTIVE)
      {
        return;
      }

      if (!IsAggregateMode())
      {
        if (!IsPlayer() || PlayerCameraInfo.Instance().CameraMode != CameraMode.FREEROTATION)
        {
          if (!IsFarMode())
          {
            if (Mesh != null && Mesh.IsVisible())
              using (new PerfElement("mesh_render"))
                Mesh.Render();
          }
          else
          {
            if (FarMesh != null && FarMesh.IsVisible())
              using (new PerfElement("mesh_render_far"))
                FarMesh.Render();
          }

          if (ParticleSystem != null)
            using (new PerfElement("particlesys_render"))
              ParticleSystem.Render();
        }
      }
    }

    private void CheckCollision()
    {
      IsTestingCollision = false;
      if (!PrevPosition.Equals(new TV_3DVECTOR()) && !GetPosition().Equals(new TV_3DVECTOR()))
      {
        // only check player and projectiles
        if (IsPlayer() || TypeInfo is ProjectileGroup || (ActorState == ActorState.DYING && TypeInfo.TargetType.HasFlag(TargetType.FIGHTER)))
        //  if ((IsPlayer() && !PlayerInfo.Instance().PlayerAIEnabled) || TypeInfo is ProjectileGroup || (ActorState == ActorState.DYING && TypeInfo.TargetType.HasFlag(TargetType.FIGHTER)))
        //if (TypeInfo.IsDamage || TypeInfo.TargetType.HasFlag(TargetType.FIGHTER))
        {
          if (IsInCollision)
          {
            if (IsPlayer() && PlayerInfo.Instance().PlayerAIEnabled)
            {
              Screen2D.Instance().MessageSecondaryText(string.Format("DEV WARNING: PLAYER AI COLLIDED: {0}", CollisionActor), 1.5f, new TV_COLOR(1, 0.2f, 0.2f, 1), 99999);
              IsInCollision = false;
              IsTestingCollision = true;
              return;
            }
            if (CollisionActor != null)
            {
              CollisionActor.TypeInfo.ProcessHit(CollisionActor, this, CollisionImpact, CollisionNormal);
              TypeInfo.ProcessHit(this, CollisionActor, CollisionImpact, -1 * CollisionNormal);
            }
            IsInCollision = false;
          }
          IsTestingCollision = true;
        }
      }
    }

    private void TestLandscapeCollision()
    {
      TV_3DVECTOR vmin = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z, false);
      TV_3DVECTOR vmax = GetRelativePositionXYZ(0, 0, TypeInfo.min_dimensions.z, false) + PrevPosition - Position;

      // check landscape only
      if (TypeInfo is ProjectileGroup || TypeInfo.TargetType != TargetType.NULL)
        if (LandInfo.Instance().Land.AdvancedCollide(vmin, vmax).IsCollision())
        {
          ActorState = ActorState.DEAD;
        }
    }

    private void TestCollision()
    {
      if (IsTestingCollision)
      {
        //using (new PerfElement("collision_combat_" + Name.PadRight(15).Substring(0, 13)))
        //{
          TV_3DVECTOR vmin = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z, false);
          TV_3DVECTOR vmax = GetRelativePositionXYZ(0, 0, TypeInfo.min_dimensions.z, false) + PrevPosition - Position;

          IsInCollision = TestCollision(vmin, vmax, true, out CollisionImpact, out CollisionNormal, out CollisionActor);
          IsTestingCollision = false;
        //}
      }
      if (IsTestingProspectiveCollision)
      {
        //using (new PerfElement("collision_prospect"))
        //{
          ActorInfo dummy;

          TV_3DVECTOR prostart = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z + 10);
          TV_3DVECTOR proend0 = GetRelativePositionXYZ(0, 0, TypeInfo.max_dimensions.z + 10 + ProspectiveCollisionScanDistance);

          TV_3DVECTOR proImpact = new TV_3DVECTOR();
          TV_3DVECTOR proNormal = new TV_3DVECTOR();

          TV_3DVECTOR _Impact = new TV_3DVECTOR();
          TV_3DVECTOR _Normal = new TV_3DVECTOR();
          int count = 0;

          IsInProspectiveCollision = false;
          
          if (TestCollision(prostart, proend0, false, out _Impact, out _Normal, out ProspectiveCollisionActor))
            {
              proImpact += _Impact;
              proNormal += _Normal;
              count++;
              IsInProspectiveCollision = true;
              ProspectiveCollisionSafe = _Impact + _Normal * 10000;
            }

            if (IsInProspectiveCollision || IsAvoidingCollision)
              ProspectiveCollisionLevel = 0;
              bool nextlevel = true;
              float dist = 0;
          while (nextlevel && ProspectiveCollisionLevel < 5)
          {
            ProspectiveCollisionLevel++;
            nextlevel = true;
            for (int i = -1; i <= 1; i++)
              for (int j = -1; j <= 1; j++)
              {
                if (i == 0 && j == 0)
                  continue;

                proend0 = GetRelativePositionXYZ(i * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                               , j * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                               , TypeInfo.max_dimensions.z + 10 + ProspectiveCollisionScanDistance);
                if (TestCollision(prostart, proend0, false, out _Impact, out _Normal, out dummy))
                {
                  proImpact += _Impact;
                  proNormal += _Normal;
                  count++;
                  float newdist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(Position, _Impact);
                  if (dist < newdist)
                  {
                    dist = newdist;
                    ProspectiveCollisionSafe = Position + (proend0 - Position) * 1000;
                    if (IsAvoidingCollision)
                      nextlevel = false;
                  }
                }
                else
                {
                  dist = float.MaxValue;
                  if (!IsAvoidingCollision)
                    nextlevel = false;
                  ProspectiveCollisionSafe = Position + (proend0 - Position) * 1000;
                }
              }
          }

          if (count > 0)
          {
            ProspectiveCollisionImpact = proImpact / count;
            ProspectiveCollisionNormal = proNormal / count;
          }
          IsTestingProspectiveCollision = false;
        }
      //}
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
            if (IsPlayer())
            {
              Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
              Engine.Instance().TVScreen2DImmediate.Draw_Line3D(start.x
                                                , start.y
                                                , start.z
                                                , end.x
                                                , end.y
                                                , end.z
                                                , new TV_COLOR(0.5f, 1, 0.2f, 1).GetIntColor()
                                                );
              Engine.Instance().TVScreen2DImmediate.Action_End2D();
            }

            if (tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_MESH && tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              return false;

            vImpact = new TV_3DVECTOR(tvcres.vCollisionImpact.x, tvcres.vCollisionImpact.y, tvcres.vCollisionImpact.z);
            vNormal = new TV_3DVECTOR(tvcres.vCollisionNormal.x, tvcres.vCollisionNormal.y, tvcres.vCollisionNormal.z);

            if (getActorInfo)
            {
              if (tvcres.eCollidedObjectType == CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              {
                ActorState = ActorState.DEAD;
                return true;
              }

              TVMesh tvm = Engine.Instance().TVGlobals.GetMeshFromID(tvcres.iMeshID);
              if (tvm != null) // && tvm.IsVisible())
              {
                 int n = 0;
                if (int.TryParse(tvm.GetTag(), out n))
                {
                  actor = ActorFactory.Instance().GetActor(n);
                  return (actor != null 
                       && actor != this 
                       && !HasRelative(actor) 
                       && actor.TypeInfo.CollisionEnabled 
                       && !actor.IsAggregateMode()
                       && actor.CreationState == CreationState.ACTIVE);
                }
              }
              return false;
            }
            else
            {
              if (tvcres.eCollidedObjectType == CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              {
                return true;
              }

              TVMesh tvm = Engine.Instance().TVGlobals.GetMeshFromID(tvcres.iMeshID);
              if (tvm != null) // && tvm.IsVisible())
              {
                int n = 0;
                if (int.TryParse(tvm.GetTag(), out n))
                {
                  actor = ActorFactory.Instance().GetActor(n);
                  return true;
                }
              }
              return false;
            }
          }
          else
          {
            if (IsPlayer())
            {
              Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
              Engine.Instance().TVScreen2DImmediate.Draw_Line3D(start.x
                                                , start.y
                                                , start.z
                                                , end.x
                                                , end.y
                                                , end.z
                                                , new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor()
                                                );
              Engine.Instance().TVScreen2DImmediate.Action_End2D();
            }
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
        TypeInfo.FireWeapon(this, target, weapon);
    }

    #region Position / Rotation
    public TV_3DVECTOR GetPosition()
    {
      //if (WorldPosition.Time >= Game.Instance().GameTime)
      //  return WorldPosition.Value;

      TV_3DVECTOR ret = Position;
      ActorInfo a = AttachToMesh > 0 ? Parent : null;
      if (a != null)
          ret = a.GetRelativePositionXYZ(ret.x, ret.y, ret.z);
      WorldPosition.Value = ret;
      WorldPosition.Time = Game.Instance().GameTime;
      return ret;
    }

    public TV_3DVECTOR GetLocalPosition()
    {
      return Position;
    }

    public void SetLocalPosition(float x, float y, float z)
    {
      Position = new TV_3DVECTOR(x, y, z);
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
      //if (WorldRotation.Time >= Game.Instance().GameTime)
      //  return WorldRotation.Value;

      ActorInfo a = AttachToMesh > 0 ? Parent : null;
      if (a != null && a.Mesh != null)
      {
        //TV_3DVECTOR aret = a.GetRotation();

        TV_3DMATRIX pmat = new TV_3DMATRIX();
        TV_3DMATRIX pymat = new TV_3DMATRIX();
        TV_3DMATRIX pyxmat = new TV_3DMATRIX();
        TV_3DMATRIX pyxzmat = new TV_3DMATRIX();
        Engine.Instance().TVMathLibrary.TVMatrixIdentity(ref pmat);
        TV_3DMATRIX xmat = new TV_3DMATRIX();
        TV_3DMATRIX ymat = new TV_3DMATRIX();
        TV_3DMATRIX zmat = new TV_3DMATRIX();
        TV_3DVECTOR front = new TV_3DVECTOR();
        TV_3DVECTOR up = new TV_3DVECTOR();
        TV_3DVECTOR right = new TV_3DVECTOR();
        a.Mesh.GetBasisVectors(ref front, ref up, ref right);

        Engine.Instance().TVMathLibrary.TVMatrixRotationAxis(ref ymat, up, Rotation.y);
        Engine.Instance().TVMathLibrary.TVMatrixRotationAxis(ref xmat, right, Rotation.x);
        Engine.Instance().TVMathLibrary.TVMatrixRotationAxis(ref zmat, front, Rotation.z);
        Engine.Instance().TVMathLibrary.TVMatrixMultiply(ref pymat, pmat, ymat);
        Engine.Instance().TVMathLibrary.TVMatrixMultiply(ref pyxmat, pymat, xmat);
        Engine.Instance().TVMathLibrary.TVMatrixMultiply(ref pyxzmat, pyxmat, zmat);

        TV_3DVECTOR dir = Utilities.GetDirection(a.GetRotation());
        TV_3DVECTOR rdir = new TV_3DVECTOR();
        Engine.Instance().TVMathLibrary.TVVec3TransformCoord(ref rdir, dir, pyxzmat);
        TV_3DVECTOR rot = Utilities.GetRotation(rdir);

        WorldRotation.Value = rot;
        WorldRotation.Time = Game.Instance().GameTime;
        return rot;
      }
      else
      {
        WorldRotation.Value = Rotation;
        WorldRotation.Time = Game.Instance().GameTime;
        return Rotation;
      }
    }

    public void SetRotation(float x, float y, float z)
    {
      ActorInfo a = AttachToMesh > 0 ? Parent : null;
      if (a != null)
      {
        //TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
        //TV_3DVECTOR ret = new TV_3DVECTOR(x, y, z);
        /*
        TV_3DVECTOR aret = a.GetRotation();
        TV_3DQUATERNION pmat = new TV_3DQUATERNION();
        Engine.Instance().TVMathLibrary.TVQuaternionIdentity(ref pmat);
        Engine.Instance().TVMathLibrary.TVQuaternionRotationYawPitchRoll(ref pmat, aret.y, aret.x, aret.z);
        Engine.Instance().TVMathLibrary.TVQuaternionRotationYawPitchRoll(ref pmat, -Rotation.y, -Rotation.x, -Rotation.z);
        Engine.Instance().TVMathLibrary.TVQuaternionNormalize(ref pmat, pmat);
        Rotation = new TV_3DVECTOR(pmat.x, pmat.y, pmat.z);
        */
        /*
        TV_3DVECTOR aret = a.GetRotation();
        TV_3DMATRIX pmat = new TV_3DMATRIX();
        TV_3DMATRIX pinmat = new TV_3DMATRIX();
        float pindet = 0;
        TV_3DMATRIX mat = new TV_3DMATRIX();
        Engine.Instance().TVMathLibrary.TVMatrixRotationYawPitchRoll(ref pmat, aret.y, aret.x, aret.z);
        Engine.Instance().TVMathLibrary.TVMatrixInverse(ref pinmat, ref pindet, pmat);
        Engine.Instance().TVMathLibrary.TVMatrixRotationYawPitchRoll(ref mat, y, x, z);
        TV_3DMATRIX lmat = mat * pinmat;

        TV_3DQUATERNION quad = new TV_3DQUATERNION();
        Engine.Instance().TVMathLibrary.TVConvertMatrixToQuaternion(ref quad, lmat);
        //Engine.Instance().TVMathLibrary.TVEulerAnglesFromMatrix(ref aret, lmat);
        //Engine.Instance().TVMathLibrary.TVVec3Rotate(ref aret, new TV_3DVECTOR(), aret.x, aret.y, aret.z);
        Engine.Instance().TVMathLibrary.TVQuaternionNormalize(ref quad, quad);
        Rotation = new TV_3DVECTOR(quad.x, quad.y, quad.z);
        */
        //Rotation = aret;
        //Rotation = Utilities.GetRotation(ret);
        Rotation = new TV_3DVECTOR(x, y, z);
      }
      else
        Rotation = new TV_3DVECTOR(x, y, z);
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
      ActorInfo a = AttachToMesh > 0 ? Parent : null;
      if (a != null)
        ret += a.GetDirection();

      TV_3DVECTOR dir = new TV_3DVECTOR();
      Engine.Instance().TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      ActorInfo a = AttachToMesh > 0 ? Parent : null;
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

    public void SetSpawnerEnable(bool value)
    {
      if (SpawnerInfo != null)
        SpawnerInfo.Enabled = value;
    }


    public float GetTrueSpeed()
    {
      float ret = MovementInfo.Speed;
      ActorInfo a = AttachToMesh > 0 ? Parent : null;
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

    public int GetVertexCount()
    {
      if (Mesh == null)
        return 0;
      return Mesh.GetVertexCount();
    }

    public TV_3DVECTOR GetRandomVertex()
    {
      if (Mesh == null)
        return new TV_3DVECTOR();

      int r = Engine.Instance().Random.Next(0, Mesh.GetVertexCount());

      float x = 0;
      float y = 0;
      float z = 0;
      float dummy = 0;
      int dumint = 0;
      Mesh.GetVertex(r, ref x, ref y, ref z, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dummy, ref dumint);
      return new TV_3DVECTOR(x, y, z);
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

    //private Mutex m_childlist = new Mutex();

    public void AddParent(ActorInfo actor)
    {
      if (Parent != null)
        RemoveParent();

      Parent = actor;
      actor.Children.Add(this);
    }

    public void RemoveParent()
    {
      if (Parent != null)
        Parent.Children.Remove(this);
      Parent = null;
    }

    public void AddChild(ActorInfo actor)
    {
      //m_childlist.WaitOne();
      Children.Add(actor);
      //m_childlist.ReleaseMutex();
      actor.AddParent(this);
    }

    public void RemoveChild(ActorInfo actor)
    {
      //m_childlist.WaitOne();
      Children.Remove(actor);
      //m_childlist.ReleaseMutex();
      actor.RemoveParent();
    }

    public bool HasParent(ActorInfo a, int searchlevel = 99)
    {
      if (searchlevel < 0)
        return false;

      bool ret = false;
      if (Parent == null)
        return false;

      ret |= (Parent == a) || (Parent.HasParent(a, searchlevel - 1));

      return ret;
    }

    public ActorInfo GetTopParent(int searchlevel = 99)
    {
      if (Parent == null || searchlevel <= 1)
        return this;
      else
        return Parent.GetTopParent(searchlevel - 1);
    }

    public List<ActorInfo> GetAllParents(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<ActorInfo>();

      List<ActorInfo> ret = new List<ActorInfo>();
      if (searchlevel > 1)
      {
          if (Parent != null && !ret.Contains(Parent))
          {
            ret.Add(Parent);
            ret.AddRange(Parent.GetAllParents(searchlevel - 1));
          }
      }
      else
      {
        if (Parent != null)
          ret.Add(Parent);
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
        ret |= (p == a) || (p.HasChild(a, searchlevel - 1));

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

      if (searchlevel > 1)
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
      //m_childlist.WaitOne();
      List<ActorInfo> cs = new List<ActorInfo>(Children);
      //m_childlist.ReleaseMutex();

      if (Parent != null && !alreadysearched.Contains(Parent))
      {
        alreadysearched.Add(Parent);
        ret |= (Parent == a) || (Parent.HasRelative(a, searchlevel - 1, alreadysearched));
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
      //m_childlist.WaitOne();
      List<ActorInfo> cs = new List<ActorInfo>(Children);
      //m_childlist.ReleaseMutex();

      if (searchlevel > 1)
      {
        if (Parent != null && !ret.Contains(Parent))
        {
          ret.Add(Parent);
          ret.AddRange(Parent.GetAllRelatives(searchlevel - 1));
        }
      }
      else
      {
        if (Parent != null)
          ret.Add(Parent);
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
    public void OnTickEvent(object[] param) { foreach (string name in TickEvents) GameEvent.RunEvent(name, param); }
    public void OnHitEvent(object[] param) { foreach (string name in HitEvents) GameEvent.RunEvent(name, param); }
    public void OnStateChangeEvent(object[] param) { foreach (string name in ActorStateChangeEvents) GameEvent.RunEvent(name, param); }
    public void OnCreatedEvent(object[] param) { foreach (string name in CreatedEvents) GameEvent.RunEvent(name, param); }
    public void OnDestroyedEvent(object[] param) { foreach (string name in DestroyedEvents) GameEvent.RunEvent(name, param); }

    #endregion

    public bool IsOutOfBounds(TV_3DVECTOR minbounds, TV_3DVECTOR maxbounds)
    {
      TV_3DVECTOR pos = GetPosition();
      return (pos.x < minbounds.x)
          || (pos.x > maxbounds.x)
          || (pos.y < minbounds.y)
          || (pos.y > maxbounds.y)
          || (pos.z < minbounds.z)
          || (pos.z > maxbounds.z);
    }

    public bool IsNearlyOutOfBounds(float dx = 1000, float dy = 250, float dz = 1000)
    {
      TV_3DVECTOR pos = GetPosition();
      return (pos.x < GameScenarioManager.Instance().MinBounds.x + dx)
          || (pos.x > GameScenarioManager.Instance().MaxBounds.x - dx)
          || (pos.y < GameScenarioManager.Instance().MinBounds.y + dy)
          || (pos.y > GameScenarioManager.Instance().MaxBounds.y - dy)
          || (pos.z < GameScenarioManager.Instance().MinBounds.z + dz)
          || (pos.z > GameScenarioManager.Instance().MaxBounds.z - dz);
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
      foreach (string ws in AIWeapons)
      {
        TypeInfo.InterpretWeapon(this, ws, out weap, out burst);

        if (weap != null)
        {
          if ((delta_angle < weap.AngularRange
            && delta_angle > -weap.AngularRange)
            && (delta_distance < weap.Range
            && delta_distance > -weap.Range)
            && weap.CanTarget(this, target)
            && (weap.MaxAmmo == -1 || weap.Ammo > 0))
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
    }

    public bool IsFarMode()
    {
      ActorInfo a = (PlayerInfo.Instance().Actor != null) ? PlayerInfo.Instance().Actor : GameScenarioManager.Instance().SceneCamera;
      float distcheck = TypeInfo.CullDistance * 0.25f * Game.Instance().PerfCullModifier;

      return (!IsPlayer() && TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetRoughDistance(this, a) > distcheck);
    }

    public bool IsPlayer()
    {
      return this == PlayerInfo.Instance().Actor;
    }

    public bool IsScenePlayer()
    {
      return IsPlayer() || this == PlayerInfo.Instance().TempActor;
    }

    public float StrengthFrac
    {
      get
      {
        float f = CombatInfo.Strength / TypeInfo.MaxStrength;
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
        // Parent
        RemoveParent();

        // Destroy Children
        while (Children.Count > 0)
          if (Children[0].TypeInfo is AddOnGroup || Children[0].AttachToMesh == this.ID)
            Children[0].Destroy();
          else
            RemoveChild(Children[0]);

        // Mesh
        if (Mesh != null)
          Mesh.Destroy();
        Mesh = null;

        if (FarMesh != null)
          FarMesh.Destroy();
        FarMesh = null;

        // Particle System
        if (ParticleSystem != null)
          ParticleSystem.Destroy();
        ParticleSystem = null;

        // Events
        OnDestroyedEvent(new object[] { this, ActorState });
        CreatedEvents.Clear();
        DestroyedEvents.Clear();
        TickEvents.Clear();
        HitEvents.Clear();
        ActorStateChangeEvents.Clear();

        // Final dispose
        CreationState = CreationState.DISPOSED;
        Faction.UnregisterActor(this);
        ActorFactory.Instance().RemoveActor(ID);
      }
    }
  }
}
