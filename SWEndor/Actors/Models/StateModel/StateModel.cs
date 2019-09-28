using SWEndor.ActorTypes;
using SWEndor.Primitives.StateMachines;

namespace SWEndor.Actors.Models
{
  public struct StateModel
  {
    private enum ActorStateCommand
    {
      NORMAL,
      DYING,
      DEAD,
      ADVANCE_DEATH_ONE_LEVEL
    }

    private enum CreationStateCommand
    {
      GENERATE,
      ACTIVATE,
      BEGIN_DISPOSE,
      END_DISPOSE,
      REVIVE
    }

    private class ActorStateMachine : StaticStateMachine<ActorInfo, ActorState, ActorStateCommand>
    {
      public ActorStateMachine()
      {
        In(ActorState.NORMAL).ExecuteOnEntry((a, __) => { a.OnStateChangeEvent(); })
                        .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DYING);
        In(ActorState.DYING).ExecuteOnEntry((a, __) => { a.TypeInfo.Dying(a.Engine, a); a.OnStateChangeEvent(); })
                        //.On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
        In(ActorState.DEAD).ExecuteOnEntry((a, __) => { a.TypeInfo.Dead(a.Engine, a); a.OnStateChangeEvent(); })
                        .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        //.On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
      }
    }

    private class CreationStateMachine : StaticStateMachine<ActorInfo, CreationState, CreationStateCommand>
    {
      public CreationStateMachine()
      {
        In(CreationState.PLANNED)
                        .On(CreationStateCommand.GENERATE).Goto(CreationState.GENERATED)
                        .On(CreationStateCommand.BEGIN_DISPOSE).Goto(CreationState.DISPOSING);

        In(CreationState.GENERATED)
                        .On(CreationStateCommand.ACTIVATE).Goto(CreationState.ACTIVE)
                        .On(CreationStateCommand.BEGIN_DISPOSE).Goto(CreationState.DISPOSING);

        In(CreationState.ACTIVE)
                        .On(CreationStateCommand.BEGIN_DISPOSE).Goto(CreationState.DISPOSING);

        In(CreationState.DISPOSING)
                  .On(CreationStateCommand.END_DISPOSE).Goto(CreationState.DISPOSED);

        In(CreationState.DISPOSED)
                  .On(CreationStateCommand.REVIVE).Goto(CreationState.PLANNED);
      }
    }

    private static ActorStateMachine ASM = new ActorStateMachine();
    private static CreationStateMachine CSM = new CreationStateMachine();

    private ActorState _actorState;
    private CreationState _creationState;
    public ActorState ActorState { get { return _actorState; } }
    public CreationState CreationState { get { return _creationState; } }
    public float CreationTime { get; private set; }
    public ComponentMask ComponentMask { get; set; }

    public void Init(ActorInfo self, ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      _actorState = acinfo.InitialState;
      _creationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime;
      ComponentMask = type.Mask;
    }

    public void AdvanceDeathOneLevel(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL, ref _actorState); }
    public void MakeDead(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.DEAD, ref _actorState); }
    public void MakeDying(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.DYING, ref _actorState); }
    public void MakeNormal(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.NORMAL, ref _actorState); }

    public bool IsDying { get { return _actorState == ActorState.DYING; } }
    public bool IsDead { get { return _actorState == ActorState.DEAD; } }
    public bool IsDyingOrDead { get { return _actorState < 0; } }
    //Creation State
    public void SetGenerated() { CSM.Fire(null, CreationStateCommand.GENERATE, ref _creationState); }
    public void SetActivated() { CSM.Fire(null, CreationStateCommand.ACTIVATE, ref _creationState); }
    public void SetDisposing() { CSM.Fire(null, CreationStateCommand.BEGIN_DISPOSE, ref _creationState); }
    public void SetDisposed() { CSM.Fire(null, CreationStateCommand.END_DISPOSE, ref _creationState); }
    public void ResetPlanned() { CSM.Fire(null, CreationStateCommand.REVIVE, ref _creationState); }
  }
}

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public CreationState CreationState { get { return State.CreationState; } }
    public bool Planned { get { return State.CreationState == CreationState.PLANNED; } }
    public bool Generated { get { return State.CreationState == CreationState.GENERATED; } }
    public bool Active { get { return State.CreationState == CreationState.ACTIVE; } }
    public bool Disposing { get { return State.CreationState == CreationState.DISPOSING; } }
    public bool DisposingOrDisposed { get { return State.CreationState < 0; } }
    public bool Disposed { get { return State.CreationState == CreationState.DISPOSED; } }

    public ComponentMask Mask { get { return State.ComponentMask; } }

    public void AdvanceDeathOneLevel()
    {
      State.AdvanceDeathOneLevel(this);
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetGenerated()
    {
      State.SetGenerated();
#if DEBUG
  if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); 
#endif
    }

    public void SetActivated() { State.SetActivated();
#if DEBUG
     if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetDisposing() { State.SetDisposing();
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetDisposed() { State.SetDisposed();
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void ResetPlanned() { State.ResetPlanned();
#if DEBUG
     if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState);
#endif
    }

    public void SetState_Dead()
    {
      State.MakeDead(this);
#if DEBUG
    if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState);
#endif
    }

    public void SetState_Dying()
    {
      State.MakeDying(this);
#if DEBUG
    if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState);
#endif
    }

    public void SetState_Normal()
    {
      State.MakeNormal(this);
#if DEBUG
      if (Logged)
        Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState);
#endif
    }


    public ActorState ActorState { get { return State.ActorState; } }
    public bool IsDying { get { return State.IsDying; } }
    public bool IsDead { get { return State.IsDead; } }
    public bool IsDyingOrDead { get { return State.IsDyingOrDead; } }

    public float CreationTime { get { return State.CreationTime; } }
  }
}