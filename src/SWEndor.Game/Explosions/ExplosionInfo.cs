using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Components;
using SWEndor.Game.Core;
using SWEndor.Game.Explosions.Models;
using SWEndor.Game.ExplosionTypes;
using SWEndor.Game.Models;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Explosions
{
  public partial class ExplosionInfo :
    IEngineObject,
    IIdentity<int>,
    ITyped<ExplosionTypeInfo>,
    ILinked<ExplosionInfo>,
    IScoped,
    IActorState,
    IActorCreateable<ExplosionCreationInfo>,
    IActorDisposable,
    INotify,
    IMeshObject,
    IDyingTime,
    IParent<ActorInfo>,
    ITransformable
  {
    /// <summary>The instance type of this instance</summary>
    public ExplosionTypeInfo TypeInfo { get; private set; }

    /// <summary>The source factory that created this instance</summary>
    public readonly Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo> ExplosionFactory;

    /// <summary>The game engine</summary>
    public Engine Engine { get { return ExplosionFactory.Engine; } }

    internal Session Game { get { return Engine.Game; } }

    // Identifiers
    private string _name = "New Actor";

    /// <summary>The instance name</summary>
    public string Name { get { return _name; } }

    /// <summary>The instance ID</summary>
    public int ID { get; private set; }

    /// <summary>The instance unique string representation</summary>
    public override string ToString()
    {
      return "[{0},{1}]".F(_name, ID);
    }

    // Components
    internal CycleInfo<ExplosionInfo> AnimInfo;

    // Traits/Model (structs)
    private MeshModel Meshes;
    private TimerModel<ExplosionInfo> DyingTimer;
    private StateModel<ExplosionInfo> State;
    private TransformModel<ExplosionInfo, ActorInfo> Transform;

    // special
    internal int AttachedActorID = -1;

    // Ownership
    /// <summary>The previous linked instance</summary>
    public ExplosionInfo Prev { get; set; }
    /// <summary>The next linked instance</summary>
    public ExplosionInfo Next { get; set; }

    // Scope counter
    /// <summary>A scope counter determining whether the object is still in scope or safe to dispose</summary>
    public ScopeCounters.ScopeCounter Scope { get; } = new ScopeCounters.ScopeCounter();


    #region Creation Methods

    internal ExplosionInfo(Engine engine, Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo> owner, short id, ExplosionCreationInfo acinfo)
    {
      ExplosionFactory = owner;
    }

    /// <summary>
    /// Rebuilds the instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="id">The new ID</param>
    /// <param name="acinfo">The instance creation data</param>
    public void Rebuild(Engine engine, short id, ExplosionCreationInfo acinfo)
    {
      // Clear past resources
      ID = id;
      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      Meshes.Init(engine.ShaderFactory, engine.ExplosionMeshTable, ID, ref TypeInfo.MeshData);
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Transform.Init(engine, TypeInfo.MeshData.Scale, acinfo);

      // Creation
      State.Init(engine, TypeInfo, acinfo);

      AttachedActorID = -1;

      TypeInfo.Initialize(engine, this);
    }

    /// <summary>
    /// Initializes the game object instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    public void Initialize(Engine engine, bool includeAddOns)
    {
      State.SetGenerated();
      Update();
    }
    #endregion

    #region Event Methods
    public void OnStateChangeEvent() { }
    #endregion

    public bool IsAggregateMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * Game.PerfCullModifier;

        return (TypeInfo.RenderData.EnableDistanceCull
          && DistanceModel.GetRoughDistance(GetGlobalPosition(), Engine.PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    /// <summary>The parent of this object when considering coordinate positions</summary>
    public ActorInfo ParentForCoords { get { return Engine.ActorFactory.Get(AttachedActorID); } }

    /// <summary>Mark the object for disposal</summary>
    public void Delete()
    {
      if (!MarkedDisposing) { SetPreDispose(); ExplosionFactory.MakeDead(this); }
    }

    /// <summary>Disposes the object</summary>
    public void Destroy()
    {
      if (DisposingOrDisposed)
        return;

      Delete();
      SetDisposing();

      Transform.Reset();

      // Reset components
      AnimInfo.Reset();
      AttachedActorID = -1;

      // Final dispose
      ExplosionFactory.Remove(ID);
      Meshes.Dispose(ref TypeInfo.MeshData);

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
      AnimInfo.Process(engine, this);
      TypeInfo.ProcessState(engine, this);
      if (IsDead)
        Delete();

      DyingTimer.Tick(time);
    }
  }
}
