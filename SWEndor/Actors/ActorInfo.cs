using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.AI.Squads;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using SWEndor.Sound;

namespace SWEndor.Actors
{
  public partial class ActorInfo : ILinked<ActorInfo>, IScoped, IActor
  {
    public ActorTypeInfo TypeInfo { get; private set; }
    public SpawnerInfo SpawnerInfo { get; set; }

    public readonly Factory<ActorInfo> ActorFactory;
    public Engine Engine { get { return ActorFactory.Engine; } }

    public Game Game { get { return Engine.Game; } }
    public GameScenarioManager GameScenarioManager { get { return Engine.GameScenarioManager; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }

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
    public string Key { get; private set; }//{ get { return _name + " " + ID; } }

    public override string ToString()
    {
      return "[{0},{1}:{2}]".F(_name, ID, dataID);
    }

    // Faction
    private FactionInfo _faction = FactionInfo.Neutral;
    public FactionInfo Faction
    {
      get { return _faction; }
      set
      {
        _faction?.UnregisterActor(this);
        _faction = value ?? FactionInfo.Neutral;
        _faction.RegisterActor(this);
      }
    }

    // Squad
    private Squadron _squad = Squadron.Neutral;
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
    internal HitEvent HitEvents;

    // AI
    internal ActionInfo CurrentAction = null;
    internal bool CanEvade = true;
    internal bool CanRetaliate = true;
    internal int HuntWeight = 1;

    // Data (structs)
    internal CollisionData CollisionData;
    internal CombatData CombatData;
    internal WeaponData WeaponDefinitions;
    internal MoveData MoveData;
    internal AIData AIData;

    // Traits/Model (structs)
    private MeshModel Meshes;
    private RelationModel Relation;
    private TimerModel DyingTimer;
    private StateModel State;
    private HealthModel Health;
    private TransformModel Transform;
    private ArmorModel Armor;
    private ExplodeModel Explosions;
    private RegenModel Regen;

    // Traits (classes)


    // Ownership
    public ActorInfo Prev { get; set; }
    public ActorInfo Next { get; set; }

    // Log
    public bool Logged
    {
      get
      {
        return !(TypeInfo is ActorTypes.Groups.Projectile || TypeInfo is ActorTypes.Groups.Debris || TypeInfo is ActorTypes.Groups.Explosion);
      }
    }

    // Scope counter
    public ScopeCounterManager.ScopeCounter Scope { get; } = new ScopeCounterManager.ScopeCounter();


    #region Creation Methods

    internal ActorInfo(Factory<ActorInfo> owner, int id, int dataid, ActorCreationInfo acinfo)
    {
      ActorFactory = owner;
      ID = id;
      dataID = dataid;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(ID, TypeInfo);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(TypeInfo);
      Health.Init(TypeInfo, acinfo);
      Transform.Init(TypeInfo, acinfo);
      Armor.Init(TypeInfo);
      Explosions.Init(TypeInfo, acinfo);
      Regen.Init(TypeInfo);

      MoveData.Init(TypeInfo, acinfo);
      CollisionData.Init();
      CombatData.CopyFrom(TypeInfo.CombatData);
      WeaponDefinitions.Init(TypeInfo);

      State.Init(this, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      HuntWeight = TypeInfo.AIData.HuntWeight;

      TypeInfo.Initialize(this);
    }

    public void Rebuild(int id, ActorCreationInfo acinfo)
    {
      // Clear past resources
      //Destroy(); // redundant
      ID = id;
      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }
      Key = "{0} {1}".F(_name, ID);

      Meshes.Init(ID, TypeInfo);
      Relation.Init();
      DyingTimer.InitAsDyingTimer(TypeInfo);
      Health.Init(TypeInfo, acinfo);
      Transform.Init(TypeInfo, acinfo);
      Armor.Init(TypeInfo);
      Explosions.Init(TypeInfo, acinfo);
      Regen.Init(TypeInfo);

      MoveData.Init(TypeInfo, acinfo);
      CollisionData.Init();
      CombatData = TypeInfo.CombatData;
      WeaponDefinitions.Init(TypeInfo);

      // Creation
      State.Init(this, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      HuntWeight = TypeInfo.AIData.HuntWeight;
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
    public void OnTickEvent() { TickEvents?.Invoke(this); }
    public void OnHitEvent(ActorInfo victim) { HitEvents?.Invoke(this, victim); }
    public void OnStateChangeEvent() { ActorStateChangeEvents?.Invoke(this, ActorState); }
    public void OnCreatedEvent() { CreatedEvents?.Invoke(this); }
    public void OnDestroyedEvent() { DestroyedEvents?.Invoke(this); }

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
        float distcheck = TypeInfo.RenderData.CullDistance * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.RenderData.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Camera.GetPosition()) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.RenderData.CullDistance * 0.25f * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.RenderData.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Camera.GetPosition()) > distcheck);
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

      Relation.Dispose(this);

      // Actions
      CurrentAction = null;
      Squad = null;

      // Reset components
      CycleInfo.Reset();
      WeaponDefinitions.Reset();

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
      CollisionData.Reset();
      CombatData.Reset();

      Meshes.Dispose();

      // Finally
      SetDisposed();
    }

    public void Tick(float time)
    {
      CycleInfo.Process(this);
      TypeInfo.ProcessState(this);
      if (!IsDead)
      {
        if (Mask.Has(ComponentMask.CAN_BECOLLIDED)
        || TypeInfo is ActorTypes.Groups.Projectile)
        {
          CollisionData.CheckCollision(Engine, this);
        }
        TypeInfo.MoveBehavior.Move(this, ref MoveData, Game.TimeSinceRender);
      }
      else
        Delete();

      DyingTimer.Tick(this, time);
      Health.Tick(time);

      OnTickEvent();
    }
  }
}
