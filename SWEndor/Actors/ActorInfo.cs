﻿using MTV3D65;
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
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios;

namespace SWEndor.Actors
{
  public partial class ActorInfo :
    IEngineObject,
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
    public ActorTypeInfo TypeInfo { get; private set; }
    internal SpawnerInfo SpawnerInfo;

    public readonly Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo> ActorFactory;
    public Engine Engine { get { return ActorFactory.Engine; } }

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
    internal CollisionModel<ActorInfo> CollisionData;
    internal WeaponData WeaponDefinitions;
    internal MoveData MoveData;
    internal AIData AIData;

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

    // Traits (classes)


    // Standalone
    internal bool InCombat = false;

    // ILinked
    public ActorInfo Prev { get; set; }
    public ActorInfo Next { get; set; }

    // Log
#if DEBUG
    public bool Logged
    {
      get
      {
        return !(TypeInfo.AIData.TargetType.Contains(TargetType.MUNITION | TargetType.FLOATING));
      }
    }
#endif

    // Scope counter
    public ScopeCounterManager.ScopeCounter Scope { get; } = new ScopeCounterManager.ScopeCounter();


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

      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();
      WeaponDefinitions.Init(Engine.WeaponFactory, ref TypeInfo.cachedWeaponData);

      InCombat = TypeInfo.CombatData.IsCombatObject;

      State.Init(Engine, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      HuntWeight = TypeInfo.AIData.HuntWeight;

      TypeInfo.Initialize(engine, this);
    }

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

      MoveData.Init(ref TypeInfo.MoveLimitData, acinfo.FreeSpeed, acinfo.InitialSpeed);
      CollisionData.Init();
      WeaponDefinitions.Init(Engine.WeaponFactory, ref TypeInfo.cachedWeaponData);

      InCombat = TypeInfo.CombatData.IsCombatObject;

      // Creation
      State.Init(Engine, TypeInfo, acinfo);

      Faction = acinfo.Faction;

      HuntWeight = TypeInfo.AIData.HuntWeight;
      TypeInfo.Initialize(engine, this);
    }

    public void Initialize(Engine engine)
    {
      SetGenerated();
      Update();
      OnCreatedEvent();

      TypeInfo.GenerateAddOns(engine, this);
    }
    #endregion

    public void SetSpawnerEnable(bool value)
    {
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

    public bool IsNearlyOutOfBounds(GameScenarioManager mgr, float dx = 1000, float dy = 250, float dz = 1000)
    {
      TV_3DVECTOR pos = GetGlobalPosition();
      return (pos.x < mgr.MinBounds.x + dx)
          || (pos.x > mgr.MaxBounds.x - dx)
          || (pos.y < mgr.MinBounds.y + dy)
          || (pos.y > mgr.MaxBounds.y - dy)
          || (pos.z < mgr.MinBounds.z + dz)
          || (pos.z > mgr.MaxBounds.z - dz);
    }

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

    public bool IsPlayer { get { return PlayerInfo.ActorID == ID; } }

    public void SetPlayer() { PlayerInfo.ActorID = ID; }

    public bool IsScenePlayer { get { return IsPlayer || PlayerInfo.TempActorID == ID; } }

    public void Delete()
    {
      if (!MarkedDisposing) { SetPreDispose(); ActorFactory.MakeDead(this); }
    }

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

    public void JoinSquad(string squadname)
    {
      Squadron s = Engine.SquadronFactory.GetByName(squadname);
      if (!s.IsNull)
        JoinSquad(s.Leader);
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

      Delete();
      SetDisposing();

      Relation.Dispose(this);
      Transform.Reset();
      AIData.Reset();

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
      ActorFactory.Remove(ID);

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
