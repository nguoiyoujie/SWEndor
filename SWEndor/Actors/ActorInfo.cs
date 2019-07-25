using System;
using System.Collections.Generic;
using MTV3D65;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Primitives.Traits;
using SWEndor.Scenarios;
using SWEndor.Sound;
using SWEndor.Actors.Traits;
using SWEndor.Primitives.Factories;

namespace SWEndor.Actors
{
  public partial class ActorInfo : ITraitOwner
  {
    public ActorTypeInfo TypeInfo { get; private set; }
    public SpawnerInfo SpawnerInfo { get; set; }

    public readonly Factory ActorFactory;
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
    public int dataID { get { return ID % Globals.ActorLimit; } }
    public string Key { get { return "{0}:{1}".F(ID, _name); } }

    public override string ToString()
    {
#if DEBUG
      return "{0},{1}".F(Key, StateModel?.CreationState.ToString() ?? "NO STATE");
#else
      return Key;
#endif
    }

    // Faction
    private FactionInfo _faction;
    public FactionInfo Faction
    {
      get
      {
        if (_faction == null)
        {
          _faction = FactionInfo.Neutral;
          _faction.RegisterActor(this);
        }
        return _faction;
      }
      set
      {
        if (_faction != null)
          _faction.UnregisterActor(this);
        _faction = value;
        if (_faction == null)
          _faction = FactionInfo.Neutral;
        _faction.RegisterActor(this);
      }
    }

    // Components
    public IMoveComponent MoveComponent;
    public IDyingMoveComponent DyingMoveComponent;

    public CycleInfo CycleInfo;
    public WeaponSystemInfo WeaponSystemInfo;

    // Checks
    public bool EnteredCombatZone = false;

    // Delegate Events
    public ActorEvent TickEvents;
    public ActorEvent CreatedEvents;
    public ActorEvent DestroyedEvents;
    public ActorStateChangeEvent ActorStateChangeEvents;
    public HitEvent HitEvents;

    // AI
    public ActionInfo CurrentAction = null;
    public bool CanEvade = true;
    public bool CanRetaliate = true;
    public int HuntWeight = 1;

    // Data
    public MoveData MoveData;
    public CollisionData CollisionData;
    public RegenData RegenData;
    public CombatData CombatData;

    // Ownership
    public ActorInfo Prev;
    public ActorInfo Next;

    // Traits
    public IStateModel StateModel { get; private set; }
    public IMeshModel MeshModel { get; private set; }
    public IHealth Health { get; private set; }
    public DyingTimer DyingTimer { get; private set; }
    public ITransform Transform { get; private set; }
    public IRelation<ActorInfo> Relation { get; private set; }

    // 
    private readonly TraitCollection Traits;
    public List<Action<ActorInfo>> PostFrameActions = new List<Action<ActorInfo>>();



#region Creation Methods

    private ActorInfo(Factory owner, int id, ActorCreationInfo acinfo)
    {
      ActorFactory = owner;
      ID = id;

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      Traits = new TraitCollection(this);
      InitializeTraits(acinfo);

      // Init data before components
      MoveData.Init(TypeInfo, acinfo);

      CollisionData.Init();
      RegenData.CopyFrom(TypeInfo.RegenData);
      CombatData.CopyFrom(TypeInfo.CombatData);

      // Components
      MoveComponent = MoveDecorator.Create(TypeInfo);

      CycleInfo = new CycleInfo(this, null);

      WeaponSystemInfo = new WeaponSystemInfo(this);

      Faction = acinfo.Faction;

      HuntWeight = TypeInfo.HuntWeight;

      TypeInfo.Initialize(this);
    }

    private void Rebuild(ActorCreationInfo acinfo)
    {
      // Clear past resources
      Destroy();

      TypeInfo = acinfo.ActorTypeInfo;
      if (acinfo.Name?.Length > 0) { _name = acinfo.Name; }

      InitializeTraits(acinfo);

      // Init data before components
      MoveData.Init(TypeInfo, acinfo);

      CollisionData.Init();
      RegenData.CopyFrom(TypeInfo.RegenData);
      CombatData.CopyFrom(TypeInfo.CombatData);

      Faction = acinfo.Faction;

      MoveComponent = MoveDecorator.Create(TypeInfo);

      HuntWeight = TypeInfo.HuntWeight;
      TypeInfo.Initialize(this);
    }

    public void InitializeTraits(ActorCreationInfo acinfo)
    {
      Traits.State = TraitCollectionState.INIT;
      Traits.Add(TypeInfo);
      Traits.Add(Engine.TraitPoolCollection.GetTrait<StateModel>()).Init(TypeInfo, acinfo);
      Traits.Add(Engine.TraitPoolCollection.GetTrait<Health>()).Init(TypeInfo, acinfo);
      Traits.Add(Engine.TraitPoolCollection.GetTrait<DyingTimer>()).Init(TypeInfo);
      Traits.Add(Engine.TraitPoolCollection.GetTrait<MeshModel>()).Init(ID, TypeInfo);
      Traits.Add(Engine.TraitPoolCollection.GetTrait<Transform>()).Init(TypeInfo, acinfo);
      Traits.Add(Engine.TraitPoolCollection.GetTrait<Relation<ActorInfo>>()).Init();
      Traits.Add(Engine.TraitPoolCollection.GetTrait<Explodes>()).Init(TypeInfo, acinfo);

      StateModel = Traits.Get<IStateModel>();
      MeshModel = Traits.Get<IMeshModel>();
      Health = Traits.Get<IHealth>();
      DyingTimer = Traits.Get<DyingTimer>();
      Transform = Traits.Get<ITransform>();
      Relation = Traits.Get<IRelation<ActorInfo>>();

      Traits.State = TraitCollectionState.ACTIVE;
    }

    public void Initialize()
    {
      // Update State
      StateModel.CreationState = CreationState.GENERATED;
      Update();
      OnCreatedEvent();

      // Generate AddOns.
      TypeInfo.GenerateAddOns(this);
    }
#endregion

#region Position / Rotation
    public TV_3DMATRIX GetMatrix()
    {
      return Transform.GetWorldMatrix(this, Game.GameTime);
    }

    public TV_3DVECTOR GetGlobalPosition()
    {
      return Transform.GetGlobalPosition(this, Game.GameTime);
    }

    public TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, bool uselocal = false)
    {
      return Transform.GetRelativePositionFUR(this, Game.GameTime, front, up, right, uselocal);
    }

    public TV_3DVECTOR GetRelativePositionXYZ(float x, float y, float z, bool uselocal = false)
    {
      return Transform.GetRelativePositionXYZ(this, Game.GameTime, x, y, z, uselocal);
    }

    public void MoveRelative(float forward, float up = 0, float right = 0)
    {
      TV_3DVECTOR vec = GetRelativePositionFUR(forward, up, right, true);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
    }

    public void MoveAbsolute(float x, float y, float z)
    {
      TV_3DVECTOR vec = Transform.Position + new TV_3DVECTOR(x, y, z);
      Transform.Position = new TV_3DVECTOR(vec.x, vec.y, vec.z);
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
      ActorInfo a = Relation.UseParentCoords ? Relation.Parent : null;
      if (a != null)
        ret += a.GetTrueSpeed();

      return ret;
    }

    public void Rotate(float x, float y, float z)
    {
      Transform.Rotation += new TV_3DVECTOR(x, y, z);
    }

#region Parent, Child and Relatives

    public void AddChild(ActorInfo c)
    {
      Relation.AddChild(this, c);

      /*
      c.Parent = this;

      if (NumberOfChildren == 0)
      {
        FirstChild = c;
      }
      else
      {
        LastChild.NextSibling = c;
        c.PrevSibling = LastChild;
      }
      LastChild = c;
      NumberOfChildren++;
      */
    }

    public void RemoveChild(ActorInfo c)
    {
      Relation.RemoveChild(this, c);

      /*
      if (c.Parent == this)
      {
        NumberOfChildren--;
        if (FirstChild == c)
        {
          FirstChild = c.NextSibling;
        }
        else if (LastChild == c)
        {
          LastChild = c.PrevSibling;
        }
        else
        {
          c.PrevSibling.NextSibling = c.NextSibling;
          c.NextSibling.PrevSibling = c.PrevSibling;
        }

        c.Parent = null;
      }
      */
    }

    public IEnumerable<ActorInfo> Children
    {
      get
      {
        return Relation.Children;

        /*
        ActorInfo[] ret = new ActorInfo[NumberOfChildren];
        ActorInfo child = FirstChild;
        for (int i = 0; i < NumberOfChildren; i++)
        {
          ret[i] = child;
          child = child.NextSibling;
        }
        return ret;
        */
      }
    }

    public ActorInfo TopParent
    {
      get
      {
        return Relation.GetTopParent(this);
        //return Parent?.TopParent ?? this;
      }
    }

    public IEnumerable<ActorInfo> Siblings
    {
      get
      {
        return Relation.Siblings;
        //return Parent?.Children ?? new ActorInfo[0];
      }
    }

    // To be replaced by ITraitOwner-compatible Faction
    ITraitOwner ITraitOwner.Owner
    {
      get
      {
        throw new NotImplementedException();
      }
    }
#endregion

#region Event Methods
    public void OnTickEvent() { TickEvents?.Invoke(new ActorEventArg(ID)); }
    public void OnHitEvent(int victimID) { HitEvents?.Invoke(new HitEventArg(ID, victimID)); }
    public void OnStateChangeEvent() { ActorStateChangeEvents?.Invoke(new ActorStateChangeEventArg(ID, StateModel.ActorState)); }
    public void OnCreatedEvent() { CreatedEvents?.Invoke(new ActorEventArg(ID)); }
    public void OnDestroyedEvent() { DestroyedEvents?.Invoke(new ActorEventArg(ID)); }

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
        float distcheck = TypeInfo.CullDistance * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Position) > distcheck);
      }
    }

    public bool IsFarMode
    {
      get
      {
        float distcheck = TypeInfo.CullDistance * 0.25f * Game.PerfCullModifier;

        return (!IsPlayer
          && TypeInfo.EnableDistanceCull
          && ActorDistanceInfo.GetRoughDistance(GetGlobalPosition(), PlayerCameraInfo.Position) > distcheck);
      }
    }

    public bool IsPlayer { get { return PlayerInfo.Actor == this; } }

    public bool IsScenePlayer { get { return IsPlayer || PlayerInfo.TempActor == this; } }

    public void Kill()
    {
      //StateModel.CreationState = CreationState.DISPOSING;
      ActorFactory.MakeDead(this);
    }

    public void Destroy()
    {
      if (StateModel.CreationState == CreationState.DISPOSING
        || StateModel.CreationState == CreationState.DISPOSED)
        return;

      StateModel.CreationState = CreationState.DISPOSING;

      // Parent
      Relation.Parent?.RemoveChild(this);

      // Destroy Children
      foreach (ActorInfo c in Children)
      {
        if (c.TypeInfo is ActorTypes.Groups.AddOn || c.Relation.UseParentCoords)
          c.Destroy();
        else
          RemoveChild(c);
      }

      Relation.UseParentCoords = false;

      // Actions
      CurrentAction = null;

      // Reset components
      MoveComponent = NoMove.Instance;
      DyingMoveComponent = null;

      CycleInfo.Reset();
      WeaponSystemInfo.Reset();

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

      Faction.UnregisterActor(this);

      // Dispose Traits
      foreach (IDisposableTrait t in TraitsImplementing<IDisposableTrait>())
      {
        t.Dispose();
        Engine.TraitPoolCollection.ReturnTrait(t);
      }

      StateModel.CreationState = CreationState.DISPOSED;
      Traits.State = TraitCollectionState.DISPOSED;
      Traits.Clear();

      // Kill data
      MoveData.Reset();

      CollisionData.Reset();
      RegenData.Reset();
      CombatData.Reset();

      // Final dispose
      Engine.ActorFactory.Remove(ID);
    }


    public T Trait<T>() where T : ITrait { return Traits.Get<T>(); }

    public bool TryGetTrait<T>(out T trait) where T : ITrait { return Traits.TryGet(out trait); }

    public T TraitOrDefault<T>() where T : ITrait { return Traits.GetOrDefault<T>(); }

    public IEnumerable<T> TraitsImplementing<T>() where T : ITrait { return Traits.TraitsImplementing<T>(); }

    public T AddTrait<T>(T trait) where T : ITrait { return Traits.Add(trait); }

    public void Dispose()
    {
      Destroy();
    }

    public bool Planned { get { return StateModel != null && StateModel.CreationState == CreationState.PLANNED; } }

    public bool Active { get { return StateModel != null && StateModel.CreationState == CreationState.ACTIVE; } }

    public bool Disposed { get { return StateModel == null || StateModel.CreationState == CreationState.DISPOSED; } }
  }
}
