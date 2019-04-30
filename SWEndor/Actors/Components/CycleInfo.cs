using System;

namespace SWEndor.Actors.Components
{
  public class CycleInfo
  {
    private readonly ActorInfo Actor;
    public float CyclesRemaining;
    public float CyclePeriod;
    public float CycleTime;

    public Action Action;

    public CycleInfo(ActorInfo actor, Action action)
    {
      Actor = actor;
      Action = action;

      CyclesRemaining = 1;
      CyclePeriod = 1;
      CycleTime = 0;
    }

    public void Reset()
    {
      Action = null;
      CyclesRemaining = 1;
      CyclePeriod = 1;
      CycleTime = 0;
    }

    public void Process()
    {
      CycleTime -= Actor.Game.TimeSinceRender;
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
