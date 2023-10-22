using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives;
using Primrose.Primitives.Extensions;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.ParticleTypes;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.Game.Particles
{
  public partial class ParticleInfo :
    IEngineObject,
    IIdentity<int>,
    ITyped<ParticleTypeInfo>,
    ILinked<ParticleInfo>,
    IScoped,
    IActorState,
    IActorCreateable<ParticleCreationInfo>,
    IActorDisposable,
    INotify, 
    IDyingTime,
    IParent<ActorInfo>, 
    ITransformable
  {
    /// <summary>The instance type of this instance</summary>
    public ParticleTypeInfo TypeInfo { get; private set; }

    /// <summary>The source factory that created this instance</summary>
    public readonly Factory<ParticleInfo, ParticleCreationInfo, ParticleTypeInfo> ParticleFactory;

    /// <summary>The game engine</summary>
    public Engine Engine { get { return ParticleFactory.Engine; } }

    // Identifiers
    private string _name = "New Particle";

    /// <summary>The instance name</summary>
    public string Name { get { return _name; } }

    /// <summary>The instance ID</summary>
    public int ID { get; private set; }

    /// <summary>The instance unique identifier</summary>
    public string Key { get; private set; }

    /// <summary>The instance unique string representation</summary>
    public override string ToString()
    {
      return "[{0},{1}]".F(_name, ID);
    }

    internal ParticleSystemModel ParticleModel;

    // Traits/Model (structs)
    private TimerModel<ParticleInfo> DyingTimer;
    private StateModel<ParticleInfo> State;
    private TransformModel<ParticleInfo, ActorInfo> Transform;

    // special
    internal int AttachedActorID = -1;

    // ILinked
    /// <summary>The previous linked instance</summary>
    public ParticleInfo Prev { get; set; }
    /// <summary>The next linked instance</summary>
    public ParticleInfo Next { get; set; }

    // Scope counter
    /// <summary>A scope counter determining whether the object is still in scope or safe to dispose</summary>
    public ScopeCounters.ScopeCounter Scope { get; } = new ScopeCounters.ScopeCounter();


    #region Creation Methods

    internal ParticleInfo(Engine engine, Factory<ParticleInfo, ParticleCreationInfo, ParticleTypeInfo> owner, short id, ParticleCreationInfo acinfo)
    {
      ParticleFactory = owner;
    }

    /// <summary>
    /// Rebuilds the instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    /// <param name="id">The new ID</param>
    /// <param name="acinfo">The instance creation data</param>
    public void Rebuild(Engine engine, short id, ParticleCreationInfo acinfo)
    {
      // Clear past resources
      ID = id;
      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      DyingTimerSet(TypeInfo.TimedLifeData.TimedLife + TypeInfo.ParticleSystemData.ParticleLifeTime, true);
      Transform.Init(engine, new float3(1, 1, 1), acinfo);

      State.Init(Engine, TypeInfo, acinfo);
      AttachedActorID = -1;

      TypeInfo.Initialize(engine, this);
    }

    /// <summary>
    /// Initializes the game object instance
    /// </summary>
    /// <param name="engine">The game engine</param>
    public void Initialize(Engine engine, bool includeAddOns)
    {
      SetGenerated();
      ParticleModel.Init(Engine.ShaderFactory, GetGlobalPosition(), ref TypeInfo.ParticleSystemData);
      Update();
    }
    #endregion

    //public bool IsAggregateMode
    //{
    //  get
    //  {
    //    float distcheck = TypeInfo.RenderData.CullDistance * Engine.Game.PerfCullModifier;
    //
    //    return (TypeInfo.RenderData.EnableDistanceCull
    //      && DistanceModel.GetRoughDistance(GetGlobalPosition(), Engine.PlayerCameraInfo.Camera.GetPosition()) > distcheck);
    //  }
    //}

    #region Event Methods
    public void OnStateChangeEvent() { }
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

    /// <summary>The parent of this object when considering coordinate positions</summary>
    public ActorInfo ParentForCoords { get { return Engine.ActorFactory.Get(AttachedActorID); } }

    /// <summary>Mark the object for disposal</summary>
    public void Delete() {
      if (!MarkedDisposing) 
      {
        SetPreDispose();
        ParticleModel.Dispose();
        ParticleFactory.MakeDead(this);
      }
    }

    /// <summary>Disposes the object</summary>
    public void Destroy()
    {
      if (DisposingOrDisposed)
        return;

      Delete();
      SetDisposing();
      Transform.Reset();
      AttachedActorID = -1;

      // Final dispose
      ParticleFactory.Remove(ID);

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
      if (IsDead)
        Delete();

      DyingTimer.Tick(time);
      ParticleModel.Update(this);
    }
  }
}
