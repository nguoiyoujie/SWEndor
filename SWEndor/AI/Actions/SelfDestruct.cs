using SWEndor.Actors;
using SWEndor.Core;

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
      actor.SetState_Dead();
      Complete = true;
    }
  }
}
