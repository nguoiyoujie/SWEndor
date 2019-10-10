using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using SWEndor.ProjectileTypes;

namespace SWEndor.Projectiles
{
  public partial class ProjectileInfo :
    IEngineObject,
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
    public ProjectileTypeInfo TypeInfo { get; private set; }

    public readonly Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo> ProjectileFactory;
    public Engine Engine { get { return ProjectileFactory.Engine; } }

    public Session Game { get { return Engine.Game; } }

    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }

    // Identifiers
    private string _name = "New Actor";
    private string sidebar_name = "";

    public string Name { get { return _name; } }
    public string SideBarName { get { return (sidebar_name.Length == 0) ? _name : sidebar_name; } set { sidebar_name = value; } }
    public short ID { get; private set; }
    public string Key { get; private set; }

    public override string ToString()
    {
      return "[{0},{1}]".F(_name, ID);
    }

    // Faction
    private FactionInfo _faction = FactionInfo.Neutral;
    public FactionInfo Faction
    {
      get { return _faction; }
      set
      {
        _faction = value ?? FactionInfo.Neutral;
      }
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
    public ProjectileInfo Prev { get; set; }
    public ProjectileInfo Next { get; set; }

    // Log
#if DEBUG
    public bool Logged
    {
      get
      {
        return false;
      }
    }
#endif

    // Scope counter
    public ScopeCounterManager.ScopeCounter Scope { get; } = new ScopeCounterManager.ScopeCounter();


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
      Transform.Init(TypeInfo.MeshData.Scale, acinfo);
      Explosions.Init(TypeInfo.Explodes, acinfo.CreationTime);

      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();

      InCombat = TypeInfo.CombatData.IsCombatObject;

      State.Init(Engine, TypeInfo, acinfo);

      TypeInfo.Initialize(engine, this);
    }

    public void Rebuild(Engine engine, short id, ProjectileCreationInfo acinfo)
    {
      // Clear past resources
      ID = id;
      TypeInfo = acinfo.TypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(Engine, ID, ref TypeInfo.MeshData);
      DyingTimer.InitAsDyingTimer(this, ref TypeInfo.TimedLifeData);
      Transform.Init(TypeInfo.MeshData.Scale, acinfo);
      Explosions.Init(TypeInfo.Explodes, acinfo.CreationTime);

      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();

      InCombat = TypeInfo.CombatData.IsCombatObject;

      // Creation
      State.Init(Engine, TypeInfo, acinfo);

      TypeInfo.Initialize(engine, this);
    }

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
        float distcheck = TypeInfo.RenderData.CullDistance * Game.PerfCullModifier;

        return (TypeInfo.RenderData.EnableDistanceCull
          && DistanceModel.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }


    public void Rotate(float x, float y, float z)
    {
      Transform.Rotation += new TV_3DVECTOR(x, y, z);
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

    public ActorInfo ParentForCoords { get { return null; } }

    public void Delete() {
      if (!MarkedDisposing) { SetPreDispose(); ProjectileFactory.MakeDead(this); }
    }

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

    public void Tick(Engine engine, float time)
    {
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
    }
  }
}
