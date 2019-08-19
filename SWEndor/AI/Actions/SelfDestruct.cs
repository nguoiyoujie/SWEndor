using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class SelfDestruct : ActionInfo
  {
    public SelfDestruct() : base("SelfDestruct")
    {
      CanInterrupt = false;
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      actor.ActorState = ActorState.DEAD;
      Complete = true;
    }
  }
}
