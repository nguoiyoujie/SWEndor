using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.ExplosionTypes;
using SWEndor.Primitives.StateMachines;

namespace SWEndor.Models
{
  public struct StateModel<T> where T : class, INotify
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

    private class ActorStateMachine : StaticStateMachine<T, ActorState, ActorStateCommand>
    {
      public ActorStateMachine()
      {
        In(ActorState.NORMAL).ExecuteOnEntry((a, __) => { a.OnStateChangeEvent(); })
                        .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DYING);
        In(ActorState.DYING).ExecuteOnEntry((a, __) => { a.OnStateChangeEvent(); })
                        //.On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
        In(ActorState.DEAD).ExecuteOnEntry((a, __) => { a.OnStateChangeEvent(); })
                        .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                        //.On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                        .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                        .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
      }
    }

    private class CreationStateMachine : StaticStateMachine<T, CreationState, CreationStateCommand>
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

    public void Init(Engine engine, ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      _actorState = acinfo.InitialState;
      _creationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime > engine.Game.GameTime ? acinfo.CreationTime : engine.Game.GameTime;
      ComponentMask = type.Mask;
    }

    public void Init(Engine engine, ExplosionTypeInfo type, ExplosionCreationInfo acinfo)
    {
      _actorState = acinfo.InitialState;
      _creationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime > engine.Game.GameTime ? acinfo.CreationTime : engine.Game.GameTime;
      ComponentMask = ComponentMask.EXPLOSION;
    }

    public void AdvanceDeathOneLevel(T actor) { ASM.Fire(actor, ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL, ref _actorState); }
    public void MakeDead(T actor) { ASM.Fire(actor, ActorStateCommand.DEAD, ref _actorState); }
    public void MakeDying(T actor) { ASM.Fire(actor, ActorStateCommand.DYING, ref _actorState); }
    public void MakeNormal(T actor) { ASM.Fire(actor, ActorStateCommand.NORMAL, ref _actorState); }

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