using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  public class Delete : ActionInfo
  {
    public Delete() : base("Delete")
    {
      CanInterrupt = false;
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      actor.Delete();
      Complete = true;
    }
  }
}
