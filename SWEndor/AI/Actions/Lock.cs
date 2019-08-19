using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class Lock : ActionInfo
  {
    public Lock() : base("Lock")
    {
      CanInterrupt = false;
    }

    public override void Process(Engine engine, ActorInfo actor)
    { 
      actor.MoveData.ResetTurn();
    }
  }
}
