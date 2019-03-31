using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class SelfDestruct : ActionInfo
  {
    public SelfDestruct() : base("SelfDestruct")
    {
      CanInterrupt = false;
    }

    public override void Process(ActorInfo owner)
    {
      owner.ActorState = ActorState.DEAD;
      Complete = true;
    }
  }
}
