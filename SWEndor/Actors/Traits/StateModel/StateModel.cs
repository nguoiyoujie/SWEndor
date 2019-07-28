using SWEndor.ActorTypes;
using SWEndor.Primitives.StateMachines;
using SWEndor.Primitives.Traits;
using System;

namespace SWEndor.Actors.Traits
{
  public enum ActorStateCommand
  {
    NORMAL,
    DYING,
    DEAD,
    ADVANCE_DEATH_ONE_LEVEL
  }

  public enum CreationStateCommand
  {
    GENERATE,
    ACTIVATE,
    BEGIN_DISPOSE,
    END_DISPOSE,
    REVIVE
  }

  public class ActorStateMachine : StateMachine<ActorState, ActorStateCommand>
  {
    public ActorStateMachine(ActorInfo actor, ActorState initialState = ActorState.NORMAL)
    {
      In(ActorState.NORMAL)
                      .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                      .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                      .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                      .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DYING);
      In(ActorState.DYING).ExecuteOnEntry((_, __) => { foreach (INotifyDying nd in actor.TraitsImplementing<INotifyDying>()) nd.Dying(actor); })
                      //.On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                      .On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                      .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                      .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);
      In(ActorState.DEAD).ExecuteOnEntry((_, __) => { foreach (INotifyDead nd in actor.TraitsImplementing<INotifyDead>()) nd.Dead(actor); })
                      .On(ActorStateCommand.NORMAL).Goto(ActorState.NORMAL)
                      //.On(ActorStateCommand.DYING).Goto(ActorState.DYING)
                      .On(ActorStateCommand.DEAD).Goto(ActorState.DEAD)
                      .On(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL).Goto(ActorState.DEAD);

      Initialize(initialState);
    }
  }

  public class CreationStateMachine : StateMachine<CreationState, CreationStateCommand>
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

      Initialize(CreationState.PLANNED);
    }
  }

  public class StateModel : IStateModel
  {
    private ActorStateMachine ASM;
    private CreationStateMachine CSM;

    public ActorState ActorState { get { return ASM.CurrentState; } } // private set; } = ActorState.NORMAL;
    public CreationState CreationState { get { return CSM.CurrentState; } } //; set; } = CreationState.PLANNED;
    public float CreationTime { get; set; } = 0;
    public ComponentMask ComponentMask { get; set; } = ComponentMask.NONE;

    public void Init(ActorInfo self, ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      ASM = new ActorStateMachine(self, acinfo.InitialState);
      CSM = new CreationStateMachine();
      CreationTime = acinfo.CreationTime;
      ComponentMask = type.Mask;
    }

    public void AdvanceDeathOneLevel<A>(A self) where A : ITraitOwner
    {
      ASM.Fire(ActorStateCommand.ADVANCE_DEATH_ONE_LEVEL);
    }

    public void MakeDead<A>(A self) where A : ITraitOwner
    {
      ASM.Fire(ActorStateCommand.DEAD);
    }

    public void MakeDying<A>(A self) where A : ITraitOwner
    {
      ASM.Fire(ActorStateCommand.DYING);
    }

    public void MakeNormal<A>(A self) where A : ITraitOwner
    {
      ASM.Fire(ActorStateCommand.NORMAL);
    }

    public bool IsDying { get { return ActorState == ActorState.DYING; } }

    public bool IsDead { get { return ActorState == ActorState.DEAD; } }

    public bool IsDyingOrDead { get { return ActorState < 0; } }

    //Creation State
    public void SetGenerated() { CSM.Fire(CreationStateCommand.GENERATE); }
    public void SetActivated() { CSM.Fire(CreationStateCommand.ACTIVATE); }
    public void SetDisposing() { CSM.Fire(CreationStateCommand.BEGIN_DISPOSE); }
    public void SetDisposed() { CSM.Fire(CreationStateCommand.END_DISPOSE); }
    public void ResetPlanned() { CSM.Fire(CreationStateCommand.REVIVE); }
  }
}
