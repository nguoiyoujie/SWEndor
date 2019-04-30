using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Sound;
using SWEndor.Weapons;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public ActorTypeInfo TypeInfo { get; private set; }
    public SpawnerInfo SpawnerInfo { get; set; }

    public readonly Factory ActorFactory;
    public Engine Engine { get { return ActorFactory.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorTypeInfo.Factory ActorTypeFactory { get { return Engine.ActorTypeFactory; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public SoundManager SoundManager { get { return Engine.SoundManager; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

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

    // Components
    public ScoreInfo Score;
    public CombatInfo CombatInfo;

    public IMoveComponent MoveComponent;
    public IDyingMoveComponent DyingMoveComponent;

    public CycleInfo CycleInfo;
    public WeaponSystemInfo WeaponSystemInfo;
    public RegenerationInfo RegenerationInfo;
    public ExplosionInfo ExplosionInfo;
    public CollisionInfo CollisionInfo;
    public CameraSystemInfo CameraSystemInfo;

    // Checks
    public bool EnteredCombatZone = false;

    // Delegate Events
    // plan to make them true delegates instead of string
    public GameEvent TickEvents;
    public GameEvent CreatedEvents;
    public GameEvent DestroyedEvents;
    public GameEvent ActorStateChangeEvents;
    public GameEvent HitEvents;

    // AI
    public ActionInfo CurrentAction = null;
    public bool CanEvade = true;
    public bool CanRetaliate = true;
    public int HuntWeight = 1;

    // Utility
    private ActorState m_ActorState;
    public ActorState ActorState
    {
      get { return m_ActorState; }
      set
      {
        if (m_ActorState != value)
        {
          ActorState prevState = m_ActorState;
          m_ActorState = value;
          TypeInfo.ProcessNewState(this);
          OnStateChangeEvent();
        }
      }
    }

    private TV_3DVECTOR m_Scale;
    public TV_3DVECTOR Scale
    {
      get { return m_Scale; }
      set
      {
        if (!m_Scale.Equals(value))
        {
          Mesh?.SetScale(Scale.x, Scale.y, Scale.z);
          FarMesh?.SetScale(Scale.x, Scale.y, Scale.z);
          m_Scale = value;
        }
      }
    }

    public TV_3DVECTOR Position { get; set; }
    //public Cached3DVector WorldPosition;
    public TV_3DVECTOR PrevPosition { get; private set; }

    public TV_3DVECTOR Rotation { get; private set; }
    //public Cached3DVector WorldRotation;

    // Render
    public TVMesh Mesh { get; private set; }
    public TVMesh FarMesh { get; private set; }
    public bool AttachToParent = false;

    // Particle system
    public int ParticleEmitterID = -1;
    public TVParticleSystem ParticleSystem = null;

    // Ownership
    public int ParentID { get; private set; } = -1;

    #region Creation Methods

    private ActorInfo(Factory owner, int id, ActorCreationInfo acinfo)
    {
      ActorFactory = owner;
      ID = id;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      // Components
      Score = new ScoreInfo(this);
      CombatInfo = new CombatInfo(this);

      MoveComponent = MoveDecorator.Create(this, TypeInfo, acinfo);

      CycleInfo = new CycleInfo(this, null);

      WeaponSystemInfo = new WeaponSystemInfo(this);
      RegenerationInfo = new RegenerationInfo(this);
      ExplosionInfo = new ExplosionInfo(this);
      CollisionInfo = new CollisionInfo(this);
      CameraSystemInfo = new CameraSystemInfo(this);

      // Creation
      CreationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime;

      // Combat
      CombatInfo.onNotify(CombatEventType.SET_STRENGTH, (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : TypeInfo.MaxStrength);

      Faction = acinfo.Faction;
      ActorState = acinfo.InitialState;
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      Scale = acinfo.InitialScale;
      PrevPosition = Position;

      HuntWeight = TypeInfo.HuntWeight;

      TypeInfo.Initialize(this);
    }

    private void Rebuild(ActorCreationInfo acinfo)
    {
      // Clear past resources
      Destroy();

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      // Creation
      CreationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime;

      // Combat
      CombatInfo.onNotify(CombatEventType.SET_STRENGTH, (acinfo.InitialStrength > 0) ? acinfo.InitialStrength : TypeInfo.MaxStrength);

      Faction = acinfo.Faction;
      ActorState = acinfo.InitialState;
      Position = acinfo.Position;
      Rotation = acinfo.Rotation;
      Scale = acinfo.InitialScale;
      PrevPosition = Position;

      MoveComponent = MoveDecorator.Create(this, TypeInfo, acinfo);

      HuntWeight = TypeInfo.HuntWeight;
      TypeInfo.Initialize(this);
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

      CreationState = CreationState.GENERATED;
      Update();
      OnCreatedEvent(Engine, new object[] { this });

      TypeInfo.GenerateAddOns(this);
    }

    public static ActorInfo Create(Factory factory, ActorCreationInfo acinfo)
    {
      return factory.Register(acinfo);
    }
    #endregion

    #region Position / Rotation
    public TV_3DVECTOR GetPosition()
    {
      //if (WorldPosition.Time >= Game.GameTime)
      //  return WorldPosition.Value;

      TV_3DVECTOR ret = Position;
      ActorInfo a = AttachToParent ? ActorFactory.Get(ParentID) : null;
      if (a != null)
        ret = a.GetRelativePositionXYZ(ret.x, ret.y, ret.z);
      //WorldPosition.Value = ret;
      //WorldPosition.Time = Game.GameTime;
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

      TrueVision.TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(right, up, front), rot.y, rot.x, rot.z);
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

      TrueVision.TVMathLibrary.TVVec3Rotate(ref ret, new TV_3DVECTOR(x, y, z), rot.y, rot.x, rot.z);
      ret += pos;
      return ret;
    }

    public TV_3DVECTOR GetRotation()
    {
      //if (WorldRotation.Time >= Game.GameTime)
      //  return WorldRotation.Value;

      ActorInfo a = AttachToParent ? ActorFactory.Get(ParentID) : null;
      if (a != null && a.Mesh != null)
      {
        TVMathLibrary mathl = TrueVision.TVMathLibrary;
        //TV_3DVECTOR aret = a.GetRotation();

        TV_3DMATRIX pmat = new TV_3DMATRIX();
        TV_3DMATRIX pymat = new TV_3DMATRIX();
        TV_3DMATRIX pyxmat = new TV_3DMATRIX();
        TV_3DMATRIX pyxzmat = new TV_3DMATRIX();
        mathl.TVMatrixIdentity(ref pmat);
        TV_3DMATRIX xmat = new TV_3DMATRIX();
        TV_3DMATRIX ymat = new TV_3DMATRIX();
        TV_3DMATRIX zmat = new TV_3DMATRIX();
        TV_3DVECTOR front = new TV_3DVECTOR();
        TV_3DVECTOR up = new TV_3DVECTOR();
        TV_3DVECTOR right = new TV_3DVECTOR();
        a.Mesh.GetBasisVectors(ref front, ref up, ref right);

        mathl.TVMatrixRotationAxis(ref ymat, up, Rotation.y);
        mathl.TVMatrixRotationAxis(ref xmat, right, Rotation.x);
        mathl.TVMatrixRotationAxis(ref zmat, front, Rotation.z);
        mathl.TVMatrixMultiply(ref pymat, pmat, ymat);
        mathl.TVMatrixMultiply(ref pyxmat, pymat, xmat);
        mathl.TVMatrixMultiply(ref pyxzmat, pyxmat, zmat);

        TV_3DVECTOR dir = Utilities.GetDirection(a.GetRotation());
        TV_3DVECTOR rdir = new TV_3DVECTOR();
        mathl.TVVec3TransformCoord(ref rdir, dir, pyxzmat);
        TV_3DVECTOR rot = Utilities.GetRotation(rdir);

        //WorldRotation.Value = rot;
        //WorldRotation.Time = Game.GameTime;
        return rot;
      }
      else
      {
        //WorldRotation.Value = Rotation;
        //WorldRotation.Time = Game.GameTime;
        return Rotation;
      }
    }

    public void SetRotation(float x, float y, float z)
    {
      ActorInfo a = AttachToParent ? ActorFactory.Get(ParentID) : null;
      if (a != null)
      {
        //TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
        //TV_3DVECTOR ret = new TV_3DVECTOR(x, y, z);
        /*
        TV_3DVECTOR aret = a.GetRotation();
        TV_3DQUATERNION pmat = new TV_3DQUATERNION();
        TrueVision.TVMathLibrary.TVQuaternionIdentity(ref pmat);
        TrueVision.TVMathLibrary.TVQuaternionRotationYawPitchRoll(ref pmat, aret.y, aret.x, aret.z);
        TrueVision.TVMathLibrary.TVQuaternionRotationYawPitchRoll(ref pmat, -Rotation.y, -Rotation.x, -Rotation.z);
        TrueVision.TVMathLibrary.TVQuaternionNormalize(ref pmat, pmat);
        Rotation = new TV_3DVECTOR(pmat.x, pmat.y, pmat.z);
        */
        /*
        TV_3DVECTOR aret = a.GetRotation();
        TV_3DMATRIX pmat = new TV_3DMATRIX();
        TV_3DMATRIX pinmat = new TV_3DMATRIX();
        float pindet = 0;
        TV_3DMATRIX mat = new TV_3DMATRIX();
        TrueVision.TVMathLibrary.TVMatrixRotationYawPitchRoll(ref pmat, aret.y, aret.x, aret.z);
        TrueVision.TVMathLibrary.TVMatrixInverse(ref pinmat, ref pindet, pmat);
        TrueVision.TVMathLibrary.TVMatrixRotationYawPitchRoll(ref mat, y, x, z);
        TV_3DMATRIX lmat = mat * pinmat;

        TV_3DQUATERNION quad = new TV_3DQUATERNION();
        TrueVision.TVMathLibrary.TVConvertMatrixToQuaternion(ref quad, lmat);
        //TrueVision.TVMathLibrary.TVEulerAnglesFromMatrix(ref aret, lmat);
        //TrueVision.TVMathLibrary.TVVec3Rotate(ref aret, new TV_3DVECTOR(), aret.x, aret.y, aret.z);
        TrueVision.TVMathLibrary.TVQuaternionNormalize(ref quad, quad);
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
      ActorInfo a = AttachToParent ? ActorFactory.Get(ParentID) : null;
      if (a != null)
        ret += a.GetDirection();

      TV_3DVECTOR dir = new TV_3DVECTOR();
      TrueVision.TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      ActorInfo a = AttachToParent ? ActorFactory.Get(ParentID) : null;
      if (a != null)
        dir -= a.GetDirection();

      Rotation = Utilities.GetRotation(dir);
    }

    public TV_3DVECTOR GetLocalDirection()
    {
      TV_3DVECTOR ret = Utilities.GetDirection(Rotation);

      TV_3DVECTOR dir = new TV_3DVECTOR();
      TrueVision.TVMathLibrary.TVVec3Normalize(ref dir, ret);
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
      float ret = MoveComponent.Speed;
      ActorInfo a = AttachToParent ? ActorFactory.Get(ParentID) : null;
      if (a != null)
        ret += a.GetTrueSpeed();

      return ret;
    }

    public void Rotate(float x, float y, float z)
    {
      TV_3DVECTOR vec = GetRotation();
      SetRotation(vec.x + x, vec.y + y, vec.z + z);
    }

    #region Parent, Child and Relatives
    public void AddParent(int actorID)
    {
      if (ParentID >= 0)
        RemoveParent();

      ParentID = actorID;
    }

    public void RemoveParent()
    {
      ParentID = -1;
    }

    public bool HasParent(int actorID, int searchlevel = 99)
    {
      if (searchlevel < 0)
        return false;

      bool ret = false;
      if (ParentID < 0)
        return false;

      ret |= (ParentID == actorID) || (ActorFactory.Get(ParentID)?.HasParent(actorID, searchlevel - 1) ?? false);

      return ret;
    }

    public int GetTopParent(int searchlevel = 99)
    {
      if (ParentID < 0 || searchlevel <= 1)
        return ID;
      else
        return ActorFactory.Get(ParentID)?.GetTopParent(searchlevel - 1) ?? ID;
    }

    public List<int> GetAllParents(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<int>();

      List<int> ret = new List<int>();
      if (searchlevel > 1)
      {
        if (ParentID >= 0 && !ret.Contains(ParentID))
        {
          ret.Add(ParentID);
          ret.AddRange(ActorFactory.Get(ParentID)?.GetAllParents(searchlevel - 1));
        }
      }
      else
      {
        if (ParentID >= 0)
          ret.Add(ParentID);
      }
      return ret;
    }

    public List<int> GetAllChildren(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<int>();

      List<int> ret = new List<int>();

      foreach (int actorID in ActorFactory.GetHoldingList())
      {
        ActorInfo p = ActorFactory.Get(actorID);
        if (p != null && p.HasParent(ID, searchlevel))
          ret.Add(p.ID);
      }
      return ret;
    }

    public bool HasRelative(int actorID, int searchlevel = 99, List<int> alreadysearched = null)
    {
      if (searchlevel < 0)
        return false;

      if (alreadysearched == null)
        alreadysearched = new List<int>();

      ActorInfo actor = ActorFactory.Get(actorID);
      return (actor != null && actor.GetTopParent() == GetTopParent());
    }

    public List<int> GetAllRelatives(int searchlevel = 99)
    {
      if (searchlevel < 0)
        return new List<int>();

      List<int> ret = new List<int>();

      foreach (int actorID in ActorFactory.GetHoldingList())
      {
        ActorInfo p = ActorFactory.Get(actorID);
        if (p != null && p.GetTopParent() == GetTopParent())
          ret.Add(p.ID);
      }
      return ret;
    }

    #endregion

    #region Event Methods
    public void OnTickEvent(params object[] param) { TickEvents?.Invoke(ID); }
    public void OnHitEvent(int victimID) { HitEvents?.Invoke(ID, victimID); }
    public void OnStateChangeEvent(params object[] param) { ActorStateChangeEvents?.Invoke(ID, ActorState); }
    public void OnCreatedEvent(params object[] param) { CreatedEvents?.Invoke(ID); }
    public void OnDestroyedEvent(params object[] param) { DestroyedEvents?.Invoke(ID); }

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
      return (pos.x < GameScenarioManager.MinBounds.x + dx)
          || (pos.x > GameScenarioManager.MaxBounds.x - dx)
          || (pos.y < GameScenarioManager.MinBounds.y + dy)
          || (pos.y > GameScenarioManager.MaxBounds.y - dy)
          || (pos.z < GameScenarioManager.MinBounds.z + dz)
          || (pos.z > GameScenarioManager.MaxBounds.z - dz);
    }

    public static bool IsAggregateMode(Engine engine, int id)
    {
      ActorInfo cam = engine.ActorFactory.Get(engine.GameScenarioManager.SceneCameraID);
      ActorInfo actor = engine.ActorFactory.Get(id);
      ActorInfo player = (engine.PlayerInfo.Actor != null) ? engine.PlayerInfo.Actor : cam;
      float distcheck = actor.TypeInfo.CullDistance * engine.Game.PerfCullModifier;

      return (!IsPlayer(engine, id) && actor.TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetRoughDistance(id, player.ID) > distcheck);
    }

    public static bool IsFarMode(Engine engine, int id)
    {
      ActorInfo cam = engine.ActorFactory.Get(engine.GameScenarioManager.SceneCameraID);
      ActorInfo actor = engine.ActorFactory.Get(id);
      ActorInfo player = (engine.PlayerInfo.Actor != null) ? engine.PlayerInfo.Actor : cam;
      float distcheck = actor.TypeInfo.CullDistance * 0.25f * engine.Game.PerfCullModifier;

      return (!IsPlayer(engine, id) && actor.TypeInfo.EnableDistanceCull && ActorDistanceInfo.GetRoughDistance(id, player.ID) > distcheck);
    }

    public static bool IsPlayer(Engine engine, int id)
    {
      return engine.PlayerInfo.Actor?.ID == id;
    }

    public static bool IsScenePlayer(Engine engine, int id)
    {
      return IsPlayer(engine, id) || engine.PlayerInfo.TempActor?.ID == id;
    }

    public float StrengthFrac { get { return (CombatInfo.Strength / CombatInfo.MaxStrength).Clamp(0, 1); } }

    public static void Kill(Engine engine, int id)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      if (actor != null)
        engine.ActorFactory.MakeDead(actor);
    }

    public void Destroy()
    {
      //if (CreationState == CreationState.DISPOSED)
      //  return;

      // Parent
      RemoveParent();

      // Destroy Children
      foreach (int i in GetAllChildren(1))
      {
        ActorInfo child = Engine.ActorFactory.Get(i);

        if (child.TypeInfo is ActorTypes.Groups.AddOn || child.AttachToParent)
          child.Destroy();
        else
          child.RemoveParent();
      }

      // Mesh
      if (Mesh != null)
        Mesh.Destroy();
      Mesh = null;

      if (FarMesh != null)
        FarMesh.Destroy();
      FarMesh = null;

      AttachToParent = false;

      // Particle System
      if (ParticleSystem != null)
        ParticleSystem.Destroy();
      ParticleSystem = null;

      // Actions
      CurrentAction = null;
      //ActionManager.ClearQueue(this);

      // Reset components
      Score.Reset();
      CombatInfo.Reset();

      MoveComponent = NoMove.Instance;
      DyingMoveComponent = null;

      CycleInfo.Reset();
      RegenerationInfo.Reset();
      ExplosionInfo.Reset();
      WeaponSystemInfo.Reset();
      CollisionInfo.Reset();

      // Events
      OnDestroyedEvent(Engine, new object[] { this, ActorState });
      CreatedEvents = null;
      DestroyedEvents = null;
      TickEvents = null;
      HitEvents = null;
      ActorStateChangeEvents = null;

      // Player
      if (this == PlayerInfo.Actor)
        PlayerInfo.ActorID = -1;
      else if (this == PlayerInfo.TempActor)
        PlayerInfo.TempActorID = -1;

      // Final dispose
      Faction.UnregisterActor(this);
      Engine.ActorFactory.Remove(ID);

      // Finally
      CreationState = CreationState.DISPOSED;
    }
  }
}
