using SWEndor.Actors;
using SWEndor.Actors.Traits;

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
      actor.StateModel.MakeDead(actor);
      Complete = true;
    }
  }
}
