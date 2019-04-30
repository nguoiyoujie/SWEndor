using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class SelfDestruct : ActionInfo
  {
    public SelfDestruct() : base("SelfDestruct")
    {
      CanInterrupt = false;
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      actor.ActorState = ActorState.DEAD;
      Complete = true;
    }
  }
}
