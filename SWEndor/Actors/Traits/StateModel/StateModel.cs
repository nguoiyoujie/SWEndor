using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;
using System;

namespace SWEndor.Actors.Traits
{
  public class StateModel : IStateModel
  {
    public ActorState ActorState { get; private set; } = ActorState.NORMAL;
    public CreationState CreationState { get; set; } = CreationState.PLANNED;
    public float CreationTime { get; set; } = 0;
    public ComponentMask ComponentMask { get; set; } = ComponentMask.NONE;

    public void Init(ActorTypeInfo type, ActorCreationInfo acinfo)
    {
      ActorState = acinfo.InitialState;
      CreationState = CreationState.PLANNED;
      CreationTime = acinfo.CreationTime;
      ComponentMask = type.Mask;
    }

    public void AdvanceDeathOneLevel<A>(A self) where A : ITraitOwner
    {
      if (IsDead)
        return;
      else if (IsDying)
        MakeDead(self);
      else
        MakeDying(self);
    }

    public void MakeDead<A>(A self) where A : ITraitOwner
    {
      if (!IsDead)
      {
        ActorState = ActorState.DEAD;
        foreach (INotifyDead nd in self.TraitsImplementing<INotifyDead>())
          nd.Dead(self);
      }
    }

    public void MakeDying<A>(A self) where A : ITraitOwner
    {
      if (!IsDyingOrDead)
      {
        ActorState = ActorState.DYING;
        foreach (INotifyDying nd in self.TraitsImplementing<INotifyDying>())
          nd.Dying(self);
      }
    }

    public void MakeNormal<A>(A self) where A : ITraitOwner { if (!IsDyingOrDead) ActorState = ActorState.NORMAL; }

    //public void MakeFree<A>(A self) where A : ITraitOwner { if (!IsDyingOrDead) ActorState = ActorState.FREE; }

    public bool IsDying { get { return ActorState == ActorState.DYING; } }

    public bool IsDead { get { return ActorState == ActorState.DEAD; } }

    public bool IsDyingOrDead { get { return ActorState < 0; } }
  }
}
