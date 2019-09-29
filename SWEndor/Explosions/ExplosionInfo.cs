using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Core;
using SWEndor.Explosions.Models;
using SWEndor.ExplosionTypes;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo : ILinked<ExplosionInfo>, IScoped, IExplosion
  {
    public ExplosionTypeInfo TypeInfo { get; private set; }

    public readonly Factory<ExplosionInfo> ExplosionFactory;
    public Engine Engine { get { return ExplosionFactory.Engine; } }

    public Session Game { get { return Engine.Game; } }

    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }

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
    private TimerModel DyingTimer;
    private StateModel State;
    private TransformModel Transform;

    // special
    internal int AttachedActorID = -1;

    // Ownership
    public ExplosionInfo Prev { get; set; }
    public ExplosionInfo Next { get; set; }

    // Scope counter
    public ScopeCounterManager.ScopeCounter Scope { get; } = new ScopeCounterManager.ScopeCounter();


    #region Creation Methods

    internal ExplosionInfo(Engine engine, Factory<ExplosionInfo> owner, int id, int dataid, ExplosionCreationInfo acinfo)
    {
      ExplosionFactory = owner;
      ID = id;
      dataID = dataid;

      TypeInfo = acinfo.ExplosionTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(ID, TypeInfo);
      DyingTimer.InitAsDyingTimer(TypeInfo);
      Transform.Init(TypeInfo, acinfo);

      State.Init(this, TypeInfo, acinfo);

      AttachedActorID = -1;

      TypeInfo.Initialize(engine, this);
    }

    public void Rebuild(Engine engine, int id, ExplosionCreationInfo acinfo)
    {
      // Clear past resources
      ID = id;
      TypeInfo = acinfo.ExplosionTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(ID, TypeInfo);
      DyingTimer.InitAsDyingTimer(TypeInfo);
      Transform.Init(TypeInfo, acinfo);

      // Creation
      State.Init(this, TypeInfo, acinfo);

      AttachedActorID = -1;

      TypeInfo.Initialize(engine, this);
    }

    public void Initialize(Engine engine)
    {
      State.SetGenerated();
      Update();
    }
    #endregion

    public bool IsAggregateMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * Game.PerfCullModifier;

        return (TypeInfo.RenderData.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * 0.25f * Game.PerfCullModifier;

        return (TypeInfo.RenderData.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }
    
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
      CycleInfo.Process(this);
      TypeInfo.ProcessState(engine, this);
      if (IsDead)
        Delete();

      DyingTimer.Tick(this, time);
    }
  }
}
