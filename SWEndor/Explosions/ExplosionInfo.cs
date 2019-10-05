using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Core;
using SWEndor.Explosions.Models;
using SWEndor.ExplosionTypes;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo :
    IEngineObject,
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
    public ExplosionTypeInfo TypeInfo { get; private set; }

    public readonly Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo> ExplosionFactory;
    public Engine Engine { get { return ExplosionFactory.Engine; } }

    public Session Game { get { return Engine.Game; } }

    // Identifiers
    private string _name = "New Actor";
    public string Name { get { return _name; } }
    public int ID { get; private set; }
    public int dataID = -1;
    public string Key { get; private set; }

    public override string ToString()
    {
      return "[{0},{1}:{2}]".F(_name, ID, dataID);
    }

    // Components
    internal CycleInfo<ExplosionInfo> CycleInfo;

    // Traits/Model (structs)
    private MeshModel Meshes;
    private TimerModel<ExplosionInfo> DyingTimer;
    private StateModel<ExplosionInfo> State;
    private TransformModel<ExplosionInfo, ActorInfo> Transform;

    // special
    internal int AttachedActorID = -1;

    // Ownership
    public ExplosionInfo Prev { get; set; }
    public ExplosionInfo Next { get; set; }

    // Scope counter
    public ScopeCounterManager.ScopeCounter Scope { get; } = new ScopeCounterManager.ScopeCounter();


    #region Creation Methods

    internal ExplosionInfo(Engine engine, Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo> owner, int id, int dataid, ExplosionCreationInfo acinfo)
    {
      ExplosionFactory = owner;
      ID = id;
      dataID = dataid;

      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(Engine, ID, ref TypeInfo.MeshData);
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Transform.Init(TypeInfo, acinfo);

      State.Init(Engine, TypeInfo, acinfo);

      AttachedActorID = -1;

      TypeInfo.Initialize(engine, this);
    }

    public void Rebuild(Engine engine, int id, ExplosionCreationInfo acinfo)
    {
      // Clear past resources
      ID = id;
      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(Engine, ID, ref TypeInfo.MeshData);
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Transform.Init(TypeInfo, acinfo);

      // Creation
      State.Init(Engine, TypeInfo, acinfo);

      AttachedActorID = -1;

      TypeInfo.Initialize(engine, this);
    }

    public void Initialize(Engine engine)
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
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), Engine.PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * 0.25f * Game.PerfCullModifier;

        return (TypeInfo.RenderData.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), Engine.PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    public ActorInfo ParentForCoords { get { return Engine.ActorFactory.Get(AttachedActorID); } }

    public void Delete() { ExplosionFactory.MakeDead(this); }


    public void Destroy()
    {
      if (DisposingOrDisposed)
        return;

      State.SetDisposing();

       // Reset components
      CycleInfo.Reset();
      AttachedActorID = -1;

      // Final dispose
      ExplosionFactory.Remove(ID);
      Meshes.Dispose();

      // Finally
      State.SetDisposed();
    }

    public void Tick(Engine engine, float time)
    {
      CycleInfo.Process(engine, this);
      TypeInfo.ProcessState(engine, this);
      if (IsDead)
        Delete();

      DyingTimer.Tick(time);
    }
  }
}
