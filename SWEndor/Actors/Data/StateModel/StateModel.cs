using SWEndor.ActorTypes;
using SWEndor.Primitives.StateMachines;

namespace SWEndor.Actors
{
  public partial class ActorInfo
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

      private class ActorStateMachine : StaticStateMachine<ActorState, ActorStateCommand>
      {
        private ActorInfo temp;
        private object locker = new object();

        public ActorStateMachine()
        {
          In(ActorState.NORMAL).ExecuteOnEntry((_) => { temp.OnStateChangeEvent(); })
                          .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                          .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                          .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                          .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DYING);
          In(ActorState.DYING).ExecuteOnEntry((_) => { temp.TypeInfo.Dying(temp); temp.OnStateChangeEvent(); })
                          //.On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                          .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                          .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                          .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
          In(ActorState.DEAD).ExecuteOnEntry((_) => { temp.TypeInfo.Dead(temp); temp.OnStateChangeEvent(); })
                          .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                          //.On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                          .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                          .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
        }

        public void Fire(ActorInfo actor, ActorStateCommand command, ref ActorState state)
        {
          //lock (locker)
          //{
            temp = actor;
            Fire(command, ref state);
          //}
        }
      }

      private class CreationStateMachine : StaticStateMachine<CreationState, CreationStateCommand>
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

      public ActorState ActorState;
      public CreationState CreationState;
      public float CreationTime { get; private set; }
      public ComponentMask ComponentMask { get; set; }

      public void Init(ActorInfo self, ActorTypeInfo type, ActorCreationInfo acinfo)
      {
        ActorState = acinfo.InitialState;
        CreationState = CreationState.PLANNED;
        CreationTime = acinfo.CreationTime;
        ComponentMask = type.Mask;
      }

      public void AdvanceDeathOneLevel(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL, ref ActorState); }
      public void MakeDead(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.DEAD, ref ActorState); }
      public void MakeDying(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.DYING, ref ActorState); }
      public void MakeNormal(ActorInfo actor) { ASM.Fire(actor, ActorStateCommand.NORMAL, ref ActorState); }

      public bool IsDying { get { return ActorState == ActorState.DYING; } }
      public bool IsDead { get { return ActorState == ActorState.DEAD; } }
      public bool IsDyingOrDead { get { return ActorState < 0; } }

      //Creation State
      public void SetGenerated() { CSM.Fire(CreationStateCommand.GENERATE, ref CreationState); }
      public void SetActivated() { CSM.Fire(CreationStateCommand.ACTIVATE, ref CreationState); }
      public void SetDisposing() { CSM.Fire(CreationStateCommand.BEGIN_DISPOSE, ref CreationState); }
      public void SetDisposed() { CSM.Fire(CreationStateCommand.END_DISPOSE, ref CreationState); }
      public void ResetPlanned() { CSM.Fire(CreationStateCommand.REVIVE, ref CreationState); }
    }

    public bool Planned { get { return State.CreationState == CreationState.PLANNED; } }
    public bool Generated { get { return State.CreationState == CreationState.GENERATED; } }
    public bool Active { get { return State.CreationState == CreationState.ACTIVE; } }
    public bool Disposing { get { return State.CreationState == CreationState.DISPOSING; } }
    public bool DisposingOrDisposed { get { return State.CreationState < 0; } }
    public bool Disposed { get { return State.CreationState == CreationState.DISPOSED; } }

    public void AdvanceDeathOneLevel() { State.AdvanceDeathOneLevel(this); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); }

    public void SetGenerated() { State.SetGenerated(); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); }
    public void SetActivated() { State.SetActivated(); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); }
    public void SetDisposing() { State.SetDisposing(); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); }
    public void SetDisposed() { State.SetDisposed(); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); }
    public void ResetPlanned() { State.ResetPlanned(); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_CREATIONSTATECHANGED, this, State.CreationState); }

    public void SetState_Dead() { State.MakeDead(this); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState); }
    public void SetState_Dying() { State.MakeDying(this); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState); }
    public void SetState_Normal() { State.MakeNormal(this); if (Logged) Log.Write(Log.DEBUG, LogType.ACTOR_ACTORSTATECHANGED, this, State.ActorState); }

    public bool IsDying { get { return State.IsDying; } }
    public bool IsDead { get { return State.IsDead; } }
    public bool IsDyingOrDead { get { return State.IsDyingOrDead; } }

    public float CreationTime { get { return State.CreationTime; } }
  }
}