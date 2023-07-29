using SWEndor.Game.Actors.Components;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Actors.Models;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.AI;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.AI.Squads;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using System.Collections.Concurrent;

namespace SWEndor.Game.Actors
{
  public partial class ActorInfo :
    IEngineObject,
    IIdentity<int>,
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
    ICollidable,
    IStunnable
  {
    /// <summary>The instance type of this instance</summary>
    public ActorTypeInfo TypeInfo { get; private set; }

    /// <summary>The source factory that created this instance</summary>
    public readonly Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo> ActorFactory;

    /// <summary>The game engine</summary>
    public Engine Engine { get { return ActorFactory.Engine; } }

    // Identifiers
    private string _name = "New Actor";
    private string sidebar_name = "";

    /// <summary>The instance name</summary>
    public string Name { get { return _name; } set { _name = value; } }

    /// <summary>The short name as displayed on the side bar</summary>
    public string SideBarName { get { return (sidebar_name.Length == 0) ? _name : sidebar_name; } set { sidebar_name = value; } }

    /// <summary>The instance ID</summary>
    public int ID { get; private set; }

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
          _faction?.UnregisterActor(this);
          _faction = value ?? FactionInfo.Neutral;
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
    internal ActorEvent CargoScannedEvents;
    internal ActorStateChangeEvent ActorStateChangeEvents;
    internal ActorAttackedEvent HitEvents; // I was hit
    internal ActorAttackEvent DeathEvents; // I was killed
    internal ActorAttackEvent RegisterHitEvents; // I hit someone (friendly + enemy)
    internal ActorAttackEvent RegisterKillEvents; // I killed someone (friendly + enemy)

    // AI
    internal ActionInfo CurrentAction = null;

    // Cargo
    internal bool CargoScanned = false; // Whether the cargo has been scanned by the player
    internal string Cargo = null;
    internal float CargoScanDistance = 0;
    internal float CargoVisibleDistance = 200;

    // Data (structs)
    internal CollisionModel<ActorInfo> CollisionData;
    internal WeaponData WeaponDefinitions;
    internal MoveData MoveData;
    internal AIModel AI;
    internal AIDecision AIDecision;
    internal SpecialData SpecialData;

    // Traits/Model (structs)
    private SystemModel Systems;
    private MeshModel Meshes;
    private RelationModel Relation;
    private TimerModel<ActorInfo> DyingTimer;
    private StateModel<ActorInfo> State;
    internal HealthModel Health;
    private TransformModel<ActorInfo, ActorInfo> Transform;
    private ArmorModel Armor;
    private ExplodeModel<ActorInfo, ActorInfo> Explosions;
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
        return !(TargetType.Intersects(TargetType.MUNITION | TargetType.FLOATING));
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
      if (acinfo.Name?.Length > 0) 
      {
        if (!string.IsNullOrEmpty(TypeInfo.Designation) && acinfo.Name != TypeInfo.Name)
        {
          _name = "{0} {1}".F(TypeInfo.Designation, acinfo.Name);
        }
        else
        {
          _name = acinfo.Name;
        }
      }
      Key = "{0} {1}".F(_name, ID);

      Systems.Init(ref TypeInfo.SystemData);
      Meshes.Init(engine.ShaderFactory, engine.ActorMeshTable, ID, ref TypeInfo.MeshData);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Health.Init(ref TypeInfo.CombatData, ref TypeInfo.SystemData, acinfo);
      Transform.Init(engine, TypeInfo.MeshData.Scale, acinfo);
      Armor.Init(ref TypeInfo.ArmorData);
      Explosions.Init(TypeInfo.ExplodeSystemData.Explodes, TypeInfo.ExplodeSystemData.Particles, acinfo.CreationTime);
      Regen.Init(ref TypeInfo.RegenData);
      AI.Init(ref TypeInfo.AIData, ref TypeInfo.MoveLimitData);
      AIDecision.Init(ref TypeInfo.AIData);
      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();
      SpecialData.Init();
      SpawnerInfo.Init(ref TypeInfo.SpawnerData);
      WeaponDefinitions.Init(engine.WeaponRegistry, ref TypeInfo.cachedWeaponData);

      InCombat = TypeInfo.CombatData.IsCombatObject;

      State.Init(engine, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      TypeInfo.Initialize(this);
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
      if (acinfo.Name?.Length > 0)
      {
        _name = LookUpString.GetActorName(TypeInfo, acinfo.Name);
      }
      //Key = "{0} {1}".F(_name, ID);

      Systems.Init(ref TypeInfo.SystemData);
      Meshes.Init(engine.ShaderFactory, engine.ActorMeshTable, ID, ref TypeInfo.MeshData);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Health.Init(ref TypeInfo.CombatData, ref TypeInfo.SystemData, acinfo);
      Transform.Init(engine, TypeInfo.MeshData.Scale, acinfo);
      Armor.Init(ref TypeInfo.ArmorData);
      Explosions.Init(TypeInfo.ExplodeSystemData.Explodes, TypeInfo.ExplodeSystemData.Particles, acinfo.CreationTime);
      Regen.Init(ref TypeInfo.RegenData);
      AI.Init(ref TypeInfo.AIData, ref TypeInfo.MoveLimitData);
      AIDecision.Init(ref TypeInfo.AIData);
      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();
      SpecialData.Init();
      SpawnerInfo.Init(ref TypeInfo.SpawnerData);
      WeaponDefinitions.Init(engine.WeaponRegistry, ref TypeInfo.cachedWeaponData);

      InCombat = TypeInfo.CombatData.IsCombatObject;

      // Creation
      State.Init(engine, TypeInfo, acinfo);

      Faction = acinfo.Faction;
      TypeInfo.Initialize(this);
    }

    /// <summary>
    /// Initializes the game object instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    public void Initialize(Engine engine, bool includeAddOns)
    {
      SetGenerated();
      Update();
      OnCreatedEvent();

      TypeInfo.AddOnData.Create(engine, this);
    }
    #endregion

    #region Event Methods
    public void OnTickEvent() { TickEvents?.Invoke(this); }
    public void OnHitEvent(ActorInfo attacker) { HitEvents?.Invoke(this, attacker); }
    public void OnDeathEvent(ActorInfo attacker) { DeathEvents?.Invoke(this, attacker); }
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
    public void OnCargoScannedEvent() { CargoScannedEvents?.Invoke(this);  }
    public void OnRegisterHitEvent(ActorInfo victim) { RegisterHitEvents?.Invoke(this, victim); }
    public void OnRegisterKillEvent(ActorInfo victim) { RegisterKillEvents?.Invoke(this, victim); }
    #endregion


    public bool IsAggregateMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * Engine.Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.RenderData.EnableDistanceCull
          && DistanceModel.GetRoughDistance(GetGlobalPosition(), Engine.PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * 0.25f * Engine.Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.RenderData.EnableDistanceCull
          && DistanceModel.GetRoughDistance(GetGlobalPosition(), Engine.PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    /// <summary>Checks if the object is the player</summary>
    public bool IsPlayer { get { return Engine.PlayerInfo.ActorID == ID; } }

    /// <summary>Sets the object as the player</summary>
    public void SetPlayer() { Engine.PlayerInfo.ActorID = ID; }

    /// <summary>Checks if the object is the player or the designated camera target</summary>
    public bool IsScenePlayer { get { return IsPlayer || Engine.PlayerInfo.TempActorID == ID; } }

    /// <summary>Checks if the object is allied with a faction</summary>
    public bool IsAlliedWith(FactionInfo faction) { return Faction.IsAlliedWith(faction); }

    /// <summary>Checks if the object is allied with another object</summary>
    public bool IsAlliedWith(ActorInfo actor) { return Faction.IsAlliedWith(actor.Faction); }

    /// <summary>Queries if the actor's current action can be interrupted</summary>
    public bool CanInterruptCurrentAction
    {
      get
      {
        ActionInfo action = CurrentAction;
        return action == null || action.CanInterrupt;
      }
    }

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

    /// <summary>The effective target type, after instance states are applied</summary>
    public TargetType TargetType { get { return TypeInfo.AIData.TargetType; } }

    /// <summary>The effective target type, after instance states are applied</summary>
    public TargetExclusionState TargetState
    {
      get
      {
        TargetExclusionState state = TargetExclusionState.NONE;
        if (IsStunned)
          state |= TargetExclusionState.STUNNED;

        if (!Engine.ActorFactory.DoUntil(MissileCheck, this))
          if (TypeInfo.AIData.TargetType == TargetType.FIGHTER)
            state |= TargetExclusionState.FIGHTER_MISSILE_LOCKED;
          else if (TypeInfo.AIData.TargetType == TargetType.SHIP)
            state |= TargetExclusionState.SHIP_MISSILE_LOCKED;
        return state;
      }
    }

    public static bool MissileCheck(Engine engine, ActorInfo a, ActorInfo actor)
    {
      if (a.TypeInfo.Mask == ComponentMask.GUIDED_PROJECTILE)
        if (a.CurrentAction is AI.Actions.ProjectileAttackActor attack)
          if (attack.Target_Actor != null && attack.Target_Actor.TopParent == actor)
            return false;
      return true; // continue the function
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
      AIDecision.Reset();

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
      DeathEvents = null;
      RegisterHitEvents = null;
      RegisterKillEvents = null;
      CargoScannedEvents = null;
      ActorStateChangeEvents = null;

      // Final dispose
      Faction.UnregisterActor(this);
      Faction = FactionInfo.Neutral;
      ActorFactory.Remove(ID);

      // Kill data
      MoveData.Reset();
      CollisionData.Reset();
      SpecialData.Reset();
      Meshes.Dispose(Engine.ActorMeshTable);

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

        TypeInfo.MoveBehavior.Move(engine, this, ref MoveData, Engine.Game.TimeSinceRender);
      }
      else
        Delete();

      DyingTimer.Tick(time);
      Health.Tick(time);
      Systems.Tick(engine, this, time);

      OnTickEvent();
    }
  }
}
