using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.AI.Squads;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Player;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System.Collections.Concurrent;

namespace SWEndor.Actors
{
  public partial class ActorInfo :
    IEngineObject,
    IIdentity,
    INamedObject,
    ITyped<ActorTypeInfo>,
    ILinked<ActorInfo>,
    IScoped,
    IActorState,
    IActorCreateable<ActorCreationInfo>,
    IActorDisposable,
    INotify, 
    IMeshObject,
    IDyingTime,
    IParent<ActorInfo>, 
    ITransformable,
    ICollidable
  {
    /// <summary>The instance type of this instance</summary>
    public ActorTypeInfo TypeInfo { get; private set; }

    /// <summary>The source factory that created this instance</summary>
    public readonly Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo> ActorFactory;

    /// <summary>The game engine</summary>
    public Engine Engine { get { return ActorFactory.Engine; } }

    internal Session Game { get { return Engine.Game; } }
    internal PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    internal PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }

    // Identifiers
    private string _name = "New Actor";
    private string sidebar_name = "";

    /// <summary>The instance name</summary>
    public string Name { get { return _name; } }

    /// <summary>The short name as displayed on the side bar</summary>
    public string SideBarName { get { return (sidebar_name.Length == 0) ? _name : sidebar_name; } set { sidebar_name = value; } }

    /// <summary>The instance ID</summary>
    public short ID { get; private set; }

    /// <summary>The instance unique identifier</summary>
    public string Key { get; private set; }

    /// <summary>The instance unique string representation</summary>
    public override string ToString()
    {
      return "[{0},{1}]".F(_name, ID);
    }

    // Faction
    private FactionInfo _faction = FactionInfo.Neutral;

    /// <summary>The faction the instance belongs to</summary>
    public FactionInfo Faction
    {
      get { return _faction; }
      set
      {
        if (_faction != value)
        {
          if (_faction != FactionInfo.Neutral)
            _faction?.UnregisterActor(this);
          _faction = value ?? FactionInfo.Neutral;
          if (_faction != FactionInfo.Neutral)
            _faction.RegisterActor(this);
        }
      }
    }

    // Squad
    private Squadron _squad = Squadron.Neutral;

    /// <summary>The squad the instance belongs to</summary>
    public Squadron Squad
    {
      get { return _squad; }
      set
      {
        _squad?.Remove(this);
        _squad = value ?? Squadron.Neutral;
        _squad.Add(this);
      }
    }

    // Components
    internal CycleInfo<ActorInfo> CycleInfo;

    // Delegate Events
    internal ActorEvent TickEvents;
    internal ActorEvent CreatedEvents;
    internal ActorEvent DestroyedEvents;
    internal ActorStateChangeEvent ActorStateChangeEvents;
    internal ActorEvent HitEvents;

    // AI
    internal ActionInfo CurrentAction = null;

    // Data (structs)
    internal CollisionModel<ActorInfo> CollisionData;
    internal WeaponData WeaponDefinitions;
    internal MoveData MoveData;
    internal AIModel AI;

    // Traits/Model (structs)
    private SystemModel Systems;
    private MeshModel Meshes;
    private RelationModel Relation;
    private TimerModel<ActorInfo> DyingTimer;
    private StateModel<ActorInfo> State;
    private HealthModel Health;
    private TransformModel<ActorInfo, ActorInfo> Transform;
    private ArmorModel Armor;
    private ExplodeModel<ActorInfo> Explosions;
    private RegenModel Regen;

    internal SpawnerInfo SpawnerInfo = SpawnerInfo.Default;
    // Traits (classes)
    internal ConcurrentQueue<ActorInfo> SpawnQueue = new ConcurrentQueue<ActorInfo>();

    // Standalone
    internal bool InCombat = false;

    // ILinked
    /// <summary>The previous linked instance</summary>
    public ActorInfo Prev { get; set; }
    /// <summary>The next linked instance</summary>
    public ActorInfo Next { get; set; }

    // Log
#if DEBUG
    internal bool Logged
    {
      get
      {
        return !(TypeInfo.AIData.TargetType.Intersects(TargetType.MUNITION | TargetType.FLOATING));
      }
    }
#endif

    // Scope counter
    /// <summary>A scope counter determining whether the object is still in scope or safe to dispose</summary>
    public ScopeCounters.ScopeCounter Scope { get; } = new ScopeCounters.ScopeCounter();


    #region Creation Methods

    internal ActorInfo(Engine engine, Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo> owner, short id, ActorCreationInfo acinfo)
    {
      ActorFactory = owner;
      ID = id;

      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Systems.Init(ref TypeInfo.SystemData);
      Meshes.Init(Engine, ID, ref TypeInfo.MeshData);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Health.Init(ref TypeInfo.CombatData, ref TypeInfo.SystemData, acinfo);
      Transform.Init(TypeInfo.MeshData.Scale, acinfo);
      Armor.Init(ref TypeInfo.ArmorData);
      Explosions.Init(TypeInfo.Explodes, acinfo.CreationTime);
      Regen.Init(ref TypeInfo.RegenData);
      AI.Init(ref TypeInfo.AIData);
      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();
      WeaponDefinitions.Init(Engine.WeaponFactory, ref TypeInfo.cachedWeaponData);

      InCombat = TypeInfo.CombatData.IsCombatObject;

      State.Init(Engine, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      TypeInfo.Initialize(engine, this);
    }

    /// <summary>
    /// Rebuilds the instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="id">The new ID</param>
    /// <param name="acinfo">The instance creation data</param>
    public void Rebuild(Engine engine, short id, ActorCreationInfo acinfo)
    {
      // Clear past resources
      ID = id;
      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Systems.Init(ref TypeInfo.SystemData);
      Meshes.Init(Engine, ID, ref TypeInfo.MeshData);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Health.Init(ref TypeInfo.CombatData, ref TypeInfo.SystemData, acinfo);
      Transform.Init(TypeInfo.MeshData.Scale, acinfo);
      Armor.Init(ref TypeInfo.ArmorData);
      Explosions.Init(TypeInfo.Explodes, acinfo.CreationTime);
      Regen.Init(ref TypeInfo.RegenData);
      AI.Init(ref TypeInfo.AIData);
      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();
      WeaponDefinitions.Init(Engine.WeaponFactory, ref TypeInfo.cachedWeaponData);

      InCombat = TypeInfo.CombatData.IsCombatObject;

      // Creation
      State.Init(Engine, TypeInfo, acinfo);

      Faction = acinfo.Faction;
      TypeInfo.Initialize(engine, this);
    }

    /// <summary>
    /// Initializes the game object instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    public void Initialize(Engine engine)
    {
      SetGenerated();
      Update();
      OnCreatedEvent();

      TypeInfo.GenerateAddOns(engine, this);
    }
    #endregion

    #region Event Methods
    public void OnTickEvent() { TickEvents?.Invoke(this); }
    public void OnHitEvent() { HitEvents?.Invoke(this); }
    public void OnStateChangeEvent()
    {
      if (ActorState == ActorState.DYING)
        TypeInfo.Dying(Engine, this);
      else if (ActorState == ActorState.DEAD)
        TypeInfo.Dead(Engine, this);

      ActorStateChangeEvents?.Invoke(this, ActorState);
    }
    public void OnCreatedEvent() { CreatedEvents?.Invoke(this); }
    public void OnDestroyedEvent() { DestroyedEvents?.Invoke(this); }
    #endregion


    public bool IsAggregateMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.RenderData.EnableDistanceCull
          && DistanceModel.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * 0.25f * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.RenderData.EnableDistanceCull
          && DistanceModel.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    /// <summary>Checks if the object is the player</summary>
    public bool IsPlayer { get { return PlayerInfo.ActorID == ID; } }

    /// <summary>Sets the object as the player</summary>
    public void SetPlayer() { PlayerInfo.ActorID = ID; }

    /// <summary>Checks if the object is the player or the designated camera target</summary>
    public bool IsScenePlayer { get { return IsPlayer || PlayerInfo.TempActorID == ID; } }

    /// <summary>Checks if the object is allied with a faction</summary>
    public bool IsAlliedWith(FactionInfo faction) { return Faction.IsAlliedWith(faction); }

    /// <summary>Checks if the object is allied with another object</summary>
    public bool IsAlliedWith(ActorInfo actor) { return Faction.IsAlliedWith(actor.Faction); }

    /// <summary>Mark the object for disposal</summary>
    public void Delete()
    {
      if (!MarkedDisposing) { SetPreDispose(); ActorFactory.MakeDead(this); }
    }

    /// <summary>Creates a new squad with the object as its first member</summary>
    public void CreateNewSquad()
    {
      Squad = Engine.SquadronFactory.Create();
    }

    /// <summary>Joins another squad</summary>
    public void JoinSquad(ActorInfo other)
    {
      if (other.Squad.IsNull)
        other.CreateNewSquad();
      Squad = other.Squad;
    }

    /// <summary>Joins another squad</summary>
    public void JoinSquad(string squadname)
    {
      Squadron s = Engine.SquadronFactory.GetByName(squadname);
      if (!s.IsNull)
        JoinSquad(s.Leader);
    }

    /// <summary>Makes this object the squad leader</summary>
    public void MakeSquadLeader()
    {
      if (Squad.IsNull)
        CreateNewSquad();
      Squad.MakeLeader(this);
    }

    /// <summary>Disposes the object</summary>
    public void Destroy()
    {
      if (DisposingOrDisposed)
        return;

      Delete();
      SetDisposing();

      Relation.Dispose(this);
      SpawnerInfo.DiscardQueuedFighters(this);
      Transform.Reset();
      AI.Reset();

      // Actions
      this.ForceClearQueue();
      Squad = null;

      // Reset components
      CycleInfo.Reset();
      WeaponDefinitions.Reset();
      Systems.Reset();

      // Events
      OnDestroyedEvent();
      CreatedEvents = null;
      DestroyedEvents = null;
      TickEvents = null;
      HitEvents = null;
      ActorStateChangeEvents = null;

      // Final dispose
      Faction.UnregisterActor(this);
      Faction = FactionInfo.Neutral;
      ActorFactory.Remove(ID);

      // Kill data
      MoveData.Reset();
      CollisionData.Reset();
      Meshes.Dispose();

      InCombat = false;

      // Finally
      SetDisposed();
    }

    /// <summary>
    /// Processes the object for one game tick
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="time">The game time</param>
    public void Tick(Engine engine, float time)
    {
      CycleInfo.Process(engine, this);
      TypeInfo.ProcessState(engine, this);
      if (!IsDead)
      {
        if (CanCollide)
          CollisionData.CheckCollision(engine, this);

        TypeInfo.MoveBehavior.Move(engine, this, ref MoveData, Game.TimeSinceRender);
      }
      else
        Delete();

      DyingTimer.Tick(time);
      Health.Tick(time);

      OnTickEvent();
    }
  }
}
