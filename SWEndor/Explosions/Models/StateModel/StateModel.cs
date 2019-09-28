using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ExplosionTypes;
using SWEndor.Primitives.StateMachines;

namespace SWEndor.Explosions.Models
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

    private class ActorStateMachine : StaticStateMachine<ExplosionInfo, ActorState, ActorStateCommand>
    {
      public ActorStateMachine()
      {
        In(ActorState.NORMAL)
                        .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DYING);
        In(ActorState.DYING)
                        //.On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
        In(ActorState.DEAD)
                        .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        //.On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
      }
    }

    private class CreationStateMachine : StaticStateMachine<ExplosionInfo, CreationState, CreationStateCommand>
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

    public void Init(ExplosionInfo self, ExplosionTypeInfo type, ExplosionCreationInfo acinfo)
    {
      _actorState = acinfo.InitialState;
      _creationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime;
      ComponentMask = ComponentMask.EXPLOSION;
    }

    public void AdvanceDeathOneLevel(ExplosionInfo actor) { ASM.Fire(actor, ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL, ref _actorState); }
    public void MakeDead(ExplosionInfo actor) { ASM.Fire(actor, ActorStateCommand.DEAD, ref _actorState); }
    public void MakeDying(ExplosionInfo actor) { ASM.Fire(actor, ActorStateCommand.DYING, ref _actorState); }
    public void MakeNormal(ExplosionInfo actor) { ASM.Fire(actor, ActorStateCommand.NORMAL, ref _actorState); }

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

namespace SWEndor.Explosions
{
  public partial class ExplosionInfo
  {
    public CreationState CreationState { get { return State.CreationState; } }
    public bool Planned { get { return State.CreationState == CreationState.PLANNED; } }
    public bool Generated { get { return State.CreationState == CreationState.GENERATED; } }
    public bool Active { get { return State.CreationState == CreationState.ACTIVE; } }
    public bool Disposing { get { return State.CreationState == CreationState.DISPOSING; } }
    public bool DisposingOrDisposed { get { return State.CreationState < 0; } }
    public bool Disposed { get { return State.CreationState == CreationState.DISPOSED; } }

    public ComponentMask Mask { get { return State.ComponentMask; } }

    public void AdvanceDeathOneLevel() { State.AdvanceDeathOneLevel(this); }

    public void SetGenerated() { State.SetGenerated(); }
    public void SetActivated() { State.SetActivated(); }
    public void SetDisposing() { State.SetDisposing(); }
    public void SetDisposed() { State.SetDisposed(); }
    public void ResetPlanned() { State.ResetPlanned(); }

    public void SetState_Dead() { State.MakeDead(this); }
    public void SetState_Dying() { State.MakeDying(this); }
    public void SetState_Normal() { State.MakeNormal(this); }

    public ActorState ActorState { get { return State.ActorState; } }
    public bool IsDying { get { return State.IsDying; } }
    public bool IsDead { get { return State.IsDead; } }
    public bool IsDyingOrDead { get { return State.IsDyingOrDead; } }

    public float CreationTime { get { return State.CreationTime; } }
  }
}