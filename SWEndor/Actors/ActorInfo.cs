using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.AI.Squads;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Sound;
using System.Linq;

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
      return string.Format("[{0},{1}:{2}]", _name, ID, dataID);
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
        _faction?.UnregisterActor(this);
        _faction = value ?? FactionInfo.Neutral;
        _faction.RegisterActor(this);
      }
    }

    // Squad
    private Squadron _squad;
    public Squadron Squad
    {
      get
      {
        if (_squad == null)
        {
          _squad = Squadron.Neutral;
          _squad.Add(this);
        }
        return _squad;
      }
      set
      {
        _squad?.Remove(this);
        _squad = value ?? Squadron.Neutral;
        _squad.Add(this);
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
    public MoveData MoveData;

    // Traits (structs)
    private MeshModel Meshes;
    private RelationModel Relation;
    private TimerModel DyingTimer; 
    private StateModel State;
    private HealthModel Health;
    private TransformModel Transform;
    private ArmorModel Armor;
    private ExplodeModel Explosions;

    // Traits (classes)


    // Ownership
    public ActorInfo Prev;
    public ActorInfo Next;

    // Log
    public bool Logged
    {
      get
      {
        return !(TypeInfo is ActorTypes.Groups.Projectile || TypeInfo is ActorTypes.Groups.Debris || TypeInfo is ActorTypes.Groups.Explosion);
      }
    }


    #region Creation Methods

    private ActorInfo(Factory owner, int id, int dataid, ActorCreationInfo acinfo)
    {
      ActorFactory = owner;
      ID = id;
      dataID = dataid;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      Meshes.Init(ID, TypeInfo);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(TypeInfo);
      Health.Init(TypeInfo, acinfo);
      Transform.Init(TypeInfo, acinfo);
      Armor.Init(TypeInfo);
      Explosions = new ExplodeModel(TypeInfo, acinfo);

      MoveData.Init(TypeInfo, acinfo);
      ActorDataSet.CollisionData[dataID].Init();
      ActorDataSet.RegenData[dataID].CopyFrom(TypeInfo.RegenData);
      ActorDataSet.CombatData[dataID].CopyFrom(TypeInfo.CombatData);
      WeaponSystemInfo.Init(TypeInfo);

      Engine.MaskDataSet[this] = TypeInfo.Mask;

      // Components
      MoveComponent = MoveDecorator.Create(TypeInfo);

      CycleInfo = new CycleInfo(this, null);

      //WeaponSystemInfo = new WeaponSystemInfo(this);


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

      Meshes.Init(ID, TypeInfo);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(TypeInfo);
      Health.Init(TypeInfo, acinfo);
      Transform.Init(TypeInfo, acinfo);
      Armor.Init(TypeInfo);
      Explosions = new ExplodeModel(TypeInfo, acinfo);

      MoveData.Init(TypeInfo, acinfo);
      ActorDataSet.CollisionData[dataID].Init();
      ActorDataSet.RegenData[dataID] = TypeInfo.RegenData;
      ActorDataSet.CombatData[dataID] = TypeInfo.CombatData;
      WeaponSystemInfo.Init(TypeInfo);

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
      SetGenerated();
      Update();
      OnCreatedEvent();

      TypeInfo.GenerateAddOns(this);
    }
    #endregion


    #region Position / Rotation
    public TV_3DVECTOR Position { get { return Transform.Position; } set { Transform.Position = value; } }
    public TV_3DVECTOR PrevPosition { get { return Transform.PrevPosition; } }
    public TV_3DVECTOR Rotation { get { return Transform.Rotation; } set { Transform.Rotation = value; } }
    public TV_3DVECTOR Direction { get { return Transform.Direction; } set { Transform.Direction = value; } }
    public float Scale { get { return Transform.Scale; } set { Transform.Scale = value; } }

    public TV_3DMATRIX GetMatrix() { return Transform.GetWorldMatrix(this, Game.GameTime); }
    public TV_3DVECTOR GetGlobalPosition() { return Transform.GetGlobalPosition(this, Game.GameTime); }
    public TV_3DVECTOR GetPrevGlobalPosition() { return Transform.GetPrevGlobalPosition(this, Game.GameTime); }
    public TV_3DVECTOR GetGlobalRotation() { return Transform.GetGlobalRotation(this, Game.GameTime); }
    public TV_3DVECTOR GetGlobalDirection() { return Transform.GetGlobalDirection(this); }

    public TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false)
    {
      return Transform.GetRelativePositionFUR(this, Game.GameTime, front, up, right, uselocal);
    }

    public TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool uselocal = false)
    {
      return Transform.GetRelativePositionXYZ(this, Game.GameTime, x, y, z, uselocal);
    }

    public void MoveRelative(float forward, float up = 0, float right = 0)
    {
      TV_3DVECTOR vec = GetRelativePositionFUR(forward, up, right, true);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
    }

    public void MoveAbsolute(float x, float y, float z)
    {
      TV_3DVECTOR vec = Transform.Position + new TV_3DVECTOR(x, y, z);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
    }


    public void LookAt(TV_3DVECTOR point) { Transform.LookAt(point); }

    #endregion

    public void SetSpawnerEnable(bool value)
    {
      if (SpawnerInfo != null)
        SpawnerInfo.Enabled = value;
    }

    // replace
    public float GetTrueSpeed()
    {
      float ret = MoveData.Speed;
      if (Relation.UseParentCoords)
      {
        using (var v = ScopedManager<ActorInfo>.Scope(Relation.Parent))
        {
          if (v.Value != null)
            ret += v.Value.GetTrueSpeed();
        }
      }
      return ret;
    }

    public void Rotate(float x, float y, float z)
    {
      Transform.Rotation += new TV_3DVECTOR(x, y, z);
    }

    #region Event Methods
    public void OnTickEvent() { TickEvents?.Invoke(new ActorEventArg(ID)); }
    public void OnHitEvent(int victimID) { HitEvents?.Invoke(new HitEventArg(ID, victimID)); }
    public void OnStateChangeEvent() { ActorStateChangeEvents?.Invoke(new ActorStateChangeEventArg(ID, State.ActorState)); }
    public void OnCreatedEvent() { CreatedEvents?.Invoke(new ActorEventArg(ID)); }
    public void OnDestroyedEvent() { DestroyedEvents?.Invoke(new ActorEventArg(ID)); }

    #endregion

    public bool IsOutOfBounds(TV_3DVECTOR minbounds, TV_3DVECTOR maxbounds)
    {
      TV_3DVECTOR pos = GetGlobalPosition();
      return (pos.x < minbounds.x)
          || (pos.x > maxbounds.x)
          || (pos.y < minbounds.y)
          || (pos.y > maxbounds.y)
          || (pos.z < minbounds.z)
          || (pos.z > maxbounds.z);
    }

    public bool IsNearlyOutOfBounds(float dx = 1000, float dy = 250, float dz = 1000)
    {
      TV_3DVECTOR pos = GetGlobalPosition();
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
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Position) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.CullDistance * 0.25f * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Position) > distcheck);
      }
    }

    public bool IsPlayer { get { return PlayerInfo.ActorID == ID; } }

    public bool IsScenePlayer { get { return IsPlayer || PlayerInfo.TempActorID == ID; } }

    public void Delete() { ActorFactory.MakeDead(this); }

    public void CreateNewSquad()
    {
      Squad = Engine.SquadronFactory.Create();
    }

    public void JoinSquad(ActorInfo other)
    {
      if (other.Squad.IsNull)
        other.CreateNewSquad();
      Squad = other.Squad;
    }

    public void MakeSquadLeader()
    {
      if (Squad.IsNull)
        CreateNewSquad();
      Squad.MakeLeader(this);
    }

    public void Destroy()
    {
      if (DisposingOrDisposed)
        return;

      SetDisposing();

      // Parent
      Relation.Parent?.RemoveChild(this);

      // Destroy Children
      foreach (ActorInfo c in Children.ToArray()) // use new list as members are deleted from the IEnumerable
      {
        if (c.TypeInfo is ActorTypes.Groups.AddOn || c.Relation.UseParentCoords)
          c.Destroy();
        else
          RemoveChild(c);
      }

      Relation.UseParentCoords = false;

      // Actions
      CurrentAction = null;
      Squad = null;

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
      MoveData.Reset();
      ActorDataSet.CollisionData[dataID].Reset();
      ActorDataSet.RegenData[dataID].Reset();
      ActorDataSet.CombatData[dataID].Reset();

      Meshes.Dispose();

      // Finally
      SetDisposed();
    }

    public void Tick(float time)
    {
      CycleInfo.Process();

      CheckState(Engine);
      if (!IsDead)
      {
        if (Engine.MaskDataSet[this].Has(ComponentMask.CAN_BECOLLIDED)
        || TypeInfo is ActorTypes.Groups.Projectile)
        {
          CollisionSystem.CheckCollision(Engine, this);
        }
        MoveComponent.Move(this, ref MoveData);
      }

      DyingTimer.Tick(this, time);
      Health.Tick(time);

      OnTickEvent();
    }
  }
}
