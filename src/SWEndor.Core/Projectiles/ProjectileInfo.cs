using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using SWEndor.ProjectileTypes;

namespace SWEndor.Projectiles
{
  public partial class ProjectileInfo :
    IEngineObject,
    IIdentity,
    INamedObject,
    ITyped<ProjectileTypeInfo>,
    ILinked<ProjectileInfo>,
    IScoped,
    IActorState,
    IActorCreateable<ProjectileCreationInfo>,
    IActorDisposable,
    INotify, 
    IMeshObject,
    IDyingTime,
    IParent<ActorInfo>, 
    ITransformable,
    ICollidable
  {
    /// <summary>The instance type of this instance</summary>
    public ProjectileTypeInfo TypeInfo { get; private set; }

    /// <summary>The source factory that created this instance</summary>
    public readonly Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo> ProjectileFactory;

    /// <summary>The game engine</summary>
    public Engine Engine { get { return ProjectileFactory.Engine; } }

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

    // Data (structs)
    internal CollisionModel<ProjectileInfo> CollisionData;
    internal MoveData MoveData;

    // Traits/Model (structs)
    private Projectiles.Models.MeshModel Meshes;
    private TimerModel<ProjectileInfo> DyingTimer;
    private StateModel<ProjectileInfo> State;
    private TransformModel<ProjectileInfo, ActorInfo> Transform;
    private ExplodeModel<ProjectileInfo> Explosions;

    // Traits (classes)


    // Standalone
    internal bool InCombat = false;

    // ILinked
    /// <summary>The previous linked instance</summary>
    public ProjectileInfo Prev { get; set; }
    /// <summary>The next linked instance</summary>
    public ProjectileInfo Next { get; set; }

    // Scope counter
    /// <summary>A scope counter determining whether the object is still in scope or safe to dispose</summary>
    public ScopeCounters.ScopeCounter Scope { get; } = new ScopeCounters.ScopeCounter();


    #region Creation Methods

    internal ProjectileInfo(Engine engine, Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo> owner, short id, ProjectileCreationInfo acinfo)
    {
      ProjectileFactory = owner;
      ID = id;

      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(Engine, ID, ref TypeInfo.MeshData);
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Transform.Init(Engine, TypeInfo.MeshData.Scale, acinfo);
      Explosions.Init(TypeInfo.ExplodeSystemData.Explodes, acinfo.CreationTime);

      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();

      InCombat = TypeInfo.CombatData.IsCombatObject;

      State.Init(Engine, TypeInfo, acinfo);

      TypeInfo.Initialize(engine, this);
    }

    /// <summary>
    /// Rebuilds the instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="id">The new ID</param>
    /// <param name="acinfo">The instance creation data</param>
    public void Rebuild(Engine engine, short id, ProjectileCreationInfo acinfo)
    {
      // Clear past resources
      ID = id;
      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(Engine, ID, ref TypeInfo.MeshData);
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Transform.Init(Engine, TypeInfo.MeshData.Scale, acinfo);
      Explosions.Init(TypeInfo.ExplodeSystemData.Explodes, acinfo.CreationTime);

      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();

      InCombat = TypeInfo.CombatData.IsCombatObject;

      // Creation
      State.Init(Engine, TypeInfo, acinfo);

      OwnerID = acinfo.OwnerID;
      TargetID = acinfo.TargetID;
      if (acinfo.LifeTime > 0)
        DyingTimerSet(acinfo.LifeTime, true);

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
    }
    #endregion

    public bool IsAggregateMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * Engine.Game.PerfCullModifier;

        return (TypeInfo.RenderData.EnableDistanceCull
          && DistanceModel.GetRoughDistance(GetGlobalPosition(), Engine.PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    #region Event Methods
    public void OnStateChangeEvent()
    {
      if (ActorState == ActorState.DYING)
        TypeInfo.Dying(Engine, this);
      else if (ActorState == ActorState.DEAD)
        TypeInfo.Dead(Engine, this);
    }
    #endregion

    public int OwnerID;
    public int TargetID;

    public ActorInfo Owner { get { return Engine.ActorFactory.Get(OwnerID); } }
    public ActorInfo Target { get { return Engine.ActorFactory.Get(TargetID); } }

    /// <summary>The parent of this object when considering coordinate positions</summary>
    public ActorInfo ParentForCoords { get { return null; } }

    /// <summary>Mark the object for disposal</summary>
    public void Delete() {
      if (!MarkedDisposing) { SetPreDispose(); ProjectileFactory.MakeDead(this); }
    }

    /// <summary>Disposes the object</summary>
    public void Destroy()
    {
      if (DisposingOrDisposed)
        return;

      Delete();
      SetDisposing();

      OwnerID = -1;
      TargetID = -1;
      Transform.Reset();

      // Final dispose
      ProjectileFactory.Remove(ID);

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
      TypeInfo.ProcessState(engine, this);
      if (!IsDead)
      {
        if (CanCollide)
          CollisionData.CheckCollision(engine, this);

        TypeInfo.MoveBehavior.Move(engine, this, ref MoveData, engine.Game.TimeSinceRender);
      }
      else
        Delete();

      DyingTimer.Tick(time);
    }
  }
}
