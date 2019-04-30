using SWEndor.Actors;
using System;

namespace SWEndor.AI.Actions
{
  public class CustomAction : ActionInfo
  {
    public CustomAction(Action action) : base("Custom Action")
    {
      Action = action;
      CanInterrupt = false;
    }

    private Action Action;

    public override void Process(Engine engine, int actorID)
    {
      Action.Invoke();
      Complete = true;
    }
  }
}
