using System;

namespace SWEndor.Actors.Components
{
  public class CycleInfo
  {
    private ActorInfo Actor;
    public float CyclesRemaining = 1;
    public float CyclePeriod = 1;
    public float CycleTime = 0;

    public Action Action;

    public CycleInfo(ActorInfo actor, Action action)
    {
      Actor = actor;
      Action = action;
    }

    public void Process()
    {
      CycleTime -= Game.Instance().TimeSinceRender;
      if (CyclesRemaining > 0)
      {
        if (CycleTime < 0)
        {
          CyclesRemaining--;
          CycleTime += CyclePeriod;

          Action?.Invoke();
        }
      }
    }
  }
}
