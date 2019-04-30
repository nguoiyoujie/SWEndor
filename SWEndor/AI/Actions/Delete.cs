using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class Delete : ActionInfo
  {
    public Delete() : base("Delete")
    {
      CanInterrupt = false;
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo.Kill(engine, actorID);
      Complete = true;
    }
  }
}
