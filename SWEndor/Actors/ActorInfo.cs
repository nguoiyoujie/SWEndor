using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Scenarios;
using SWEndor.Sound;
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

    public ActorDataSet ActorDataSet { get { return Engine.ActorDataSet; } }
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
    public int dataID = -1;
    public string Key { get { return _name + " " + ID; } }

    public override string ToString()
    {
      return string.Format("ACTOR:{0},{1}", ID, _name);
    }

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
    public IMoveComponent MoveComponent;
    public IDyingMoveComponent DyingMoveComponent;

    public CycleInfo CycleInfo;
    public WeaponSystemInfo WeaponSystemInfo;

    // Checks
    public bool EnteredCombatZone = false;

    // Delegate Events
    public ActorEvent TickEvents;
    public ActorEvent CreatedEvents;
    public ActorEvent DestroyedEvents;
    public ActorStateChangeEvent ActorStateChangeEvents;
    public HitEvent HitEvents;

    // AI
    public ActionInfo CurrentAction = null;
    public bool CanEvade = true;
    public bool CanRetaliate = true;
    public int HuntWeight = 1;

    // Data (structs)
    public CoordData CoordData;
    public MoveData MoveData;

    // Traits (structs)
    public Relation Relation;

    // Traits (classes)
    private StateModel State;

    // Ownership
    public ActorInfo Prev;
    public ActorInfo Next;


    #region Creation Methods

    private ActorInfo(Factory owner, int id, int dataid, ActorCreationInfo acinfo)
    {
      ActorFactory = owner;
      ID = id;
      dataID = dataid;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      // Init data before components
      CoordData.Init(acinfo);
      //ActorDataSet.CoordData[dataID].Init(acinfo);

      Relation.Init();

      MoveData.Init(TypeInfo, acinfo);
      Engine.SysDataSet.Init(this, TypeInfo, acinfo);
      Engine.MeshDataSet.Init(this, TypeInfo, acinfo);
      ActorDataSet.CollisionData[dataID].Init();
      ActorDataSet.ExplodeData[dataID].CopyFrom(TypeInfo.ExplodeData);
      ActorDataSet.RegenData[dataID].CopyFrom(TypeInfo.RegenData);
      ActorDataSet.CombatData[dataID].CopyFrom(TypeInfo.CombatData);
      Engine.TimedLifeDataSet.Init(this, TypeInfo);

      Engine.MaskDataSet[this] = TypeInfo.Mask;

      // Components
      MoveComponent = MoveDecorator.Create(TypeInfo);

      CycleInfo = new CycleInfo(this, null);

      WeaponSystemInfo = new WeaponSystemInfo(this);


      // Creation
      State.Init(this, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      HuntWeight = TypeInfo.HuntWeight;

      TypeInfo.Initialize(this);
    }

    private void Rebuild(ActorCreationInfo acinfo)
    {
      // Clear past resources
      Destroy();

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }


      // Init data before components
      CoordData.Init(acinfo);

      Relation.Init();

      MoveData.Init(TypeInfo, acinfo);
      Engine.SysDataSet.Init(this, TypeInfo, acinfo);
      Engine.MeshDataSet.Init(this, TypeInfo, acinfo);
      ActorDataSet.CollisionData[dataID].Init();
      ActorDataSet.ExplodeData[dataID] = TypeInfo.ExplodeData;
      ActorDataSet.RegenData[dataID] = TypeInfo.RegenData;
      ActorDataSet.CombatData[dataID] = TypeInfo.CombatData;
      Engine.TimedLifeDataSet.Init(this, TypeInfo);

      Engine.MaskDataSet[this] = TypeInfo.Mask;

      // Creation
      State.Init(this, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      MoveComponent = MoveDecorator.Create(TypeInfo);

      HuntWeight = TypeInfo.HuntWeight;
      TypeInfo.Initialize(this);
    }

    public void Initialize()
    {
      Engine.MeshDataSet.Mesh_generate(this, TypeInfo);

      SetGenerated();
      Update();
      OnCreatedEvent();

      TypeInfo.GenerateAddOns(this);
    }
    #endregion

    #region Position / Rotation
    public TV_3DVECTOR GetPosition()
    {
      TV_3DVECTOR ret = CoordData.Position;
      ActorInfo a = Relation.ParentForCoords;
      if (a != null)
        ret = a.GetRelativePositionXYZ(ret.x, ret.y, ret.z);

      return ret;
    }

    public TV_3DVECTOR GetLocalPosition()
    {
      return CoordData.Position;
    }

    public void SetLocalPosition(float x, float y, float z)
    {
      CoordData.Position = new TV_3DVECTOR(x, y, z);
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
      ActorInfo a = Relation.ParentForCoords;
      if (a != null && Engine.MeshDataSet.Mesh_get(a) != null)
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
        Engine.MeshDataSet.Mesh_getBasisVectors(a, ref front, ref up, ref right);

        mathl.TVMatrixRotationAxis(ref ymat, up, CoordData.Rotation.y);
        mathl.TVMatrixRotationAxis(ref xmat, right, CoordData.Rotation.x);
        mathl.TVMatrixRotationAxis(ref zmat, front, CoordData.Rotation.z);
        mathl.TVMatrixMultiply(ref pymat, pmat, ymat);
        mathl.TVMatrixMultiply(ref pyxmat, pymat, xmat);
        mathl.TVMatrixMultiply(ref pyxzmat, pyxmat, zmat);

        TV_3DVECTOR dir = Utilities.GetDirection(a.GetRotation());
        TV_3DVECTOR rdir = new TV_3DVECTOR();
        mathl.TVVec3TransformCoord(ref rdir, dir, pyxzmat);
        TV_3DVECTOR rot = Utilities.GetRotation(rdir);

        return rot;
      }
      else
      {
        return CoordData.Rotation;
      }
    }

    public void SetRotation(float x, float y, float z)
    {
      ActorInfo a = Relation.ParentForCoords;
      if (a != null)
      {
        CoordData.Rotation = new TV_3DVECTOR(x, y, z);
      }
      else
        CoordData.Rotation = new TV_3DVECTOR(x, y, z);
    }

    public TV_3DVECTOR GetLocalRotation()
    {
      return CoordData.Rotation;
    }

    public void SetLocalRotation(float x, float y, float z)
    {
      CoordData.Rotation = new TV_3DVECTOR(x, y, z);
    }

    public TV_3DVECTOR GetDirection()
    {
      TV_3DVECTOR ret = Utilities.GetDirection(CoordData.Rotation);
      ActorInfo a = Relation.ParentForCoords;
      if (a != null)
        ret += a.GetDirection();

      TV_3DVECTOR dir = new TV_3DVECTOR();
      TrueVision.TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      ActorInfo a = Relation.ParentForCoords;
      if (a != null)
        dir -= a.GetDirection();

      CoordData.Rotation = Utilities.GetRotation(dir);
    }

    public TV_3DVECTOR GetLocalDirection()
    {
      TV_3DVECTOR ret = Utilities.GetDirection(CoordData.Rotation);

      TV_3DVECTOR dir = new TV_3DVECTOR();
      TrueVision.TVMathLibrary.TVVec3Normalize(ref dir, ret);
      return dir;
    }

    public void SetLocalDirection(float x, float y, float z)
    {
      TV_3DVECTOR dir = new TV_3DVECTOR(x, y, z);
      CoordData.Rotation = Utilities.GetRotation(dir);
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

    public void MoveRelative(float front, float up = 0, float right = 0)
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
      float ret = MoveData.Speed;
      ActorInfo a = Relation.ParentForCoords;
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

    public void AddChild(ActorInfo a) { Relation.AddChild(this, a); }
    public void RemoveChild(ActorInfo a) { Relation.RemoveChild(this, a); }

    public IEnumerable<ActorInfo> Children { get { return Relation.Children; } }

    public ActorInfo TopParent { get { return Relation.GetTopParent(this); } }

    public IEnumerable<ActorInfo> Siblings { get { return Relation.Siblings; } }

    #endregion

    #region Event Methods
    public void OnTickEvent() { TickEvents?.Invoke(new ActorEventArg(ID)); }
    public void OnHitEvent(int victimID) { HitEvents?.Invoke(new HitEventArg(ID, victimID)); }
    public void OnStateChangeEvent() { ActorStateChangeEvents?.Invoke(new ActorStateChangeEventArg(ID, State.ActorState)); }
    public void OnCreatedEvent() { CreatedEvents?.Invoke(new ActorEventArg(ID)); }
    public void OnDestroyedEvent() { DestroyedEvents?.Invoke(new ActorEventArg(ID)); }

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

    public bool IsAggregateMode
    {
      get
      {
        float distcheck = TypeInfo.CullDistance * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetPosition(), PlayerCameraInfo.Position) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.CullDistance * 0.25f * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetPosition(), PlayerCameraInfo.Position) > distcheck);
      }
    }

    public bool IsPlayer { get { return PlayerInfo.ActorID == ID; } }

    public bool IsScenePlayer { get { return IsPlayer || PlayerInfo.TempActorID == ID; } }

    public void Kill() { ActorFactory.MakeDead(this); }

    public void Destroy()
    {
      if (DisposingOrDisposed)
        return;

      SetDisposing();

      // Parent
      Relation.Parent?.RemoveChild(this);

      // Destroy Children
      foreach (ActorInfo c in new List<ActorInfo>(Children)) // use new list as members are deleted from the IEnumerable
      {
        if (c.TypeInfo is ActorTypes.Groups.AddOn || c.Relation.UseParentCoords)
          c.Destroy();
        else
          RemoveChild(c);
      }

      Relation.UseParentCoords = false;

      // Actions
      CurrentAction = null;

      // Reset components

      MoveComponent = NoMove.Instance;
      DyingMoveComponent = null;

      CycleInfo.Reset();
      WeaponSystemInfo.Reset();

      // Events
      OnDestroyedEvent();
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

      // Kill data
      CoordData.Reset();
      //ActorDataSet.CoordData[dataID].Reset();
      MoveData.Reset();
      Engine.SysDataSet.Reset(this);
      Engine.MeshDataSet.Reset(this);
      ActorDataSet.CollisionData[dataID].Reset();
      ActorDataSet.RegenData[dataID].Reset();
      ActorDataSet.ExplodeData[dataID].Reset();
      ActorDataSet.CombatData[dataID].Reset();
      Engine.TimedLifeDataSet.Reset(this);

      // Finally
      SetDisposed();
    }
  }
}
