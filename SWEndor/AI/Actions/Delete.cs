using SWEndor.Actors;

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
      actor.Kill();
      Complete = true;
    }
  }
}
