using SWEndor.Core;
using System;

namespace SWEndor.Actors.Components
{
  internal struct CycleInfo<T>
  {
    public float CyclesRemaining;
    public float CyclePeriod;
    public float CycleTime;

    public Action<T> Action;

    public CycleInfo(Action<T> action)
    {
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

    public void Process(Engine engine, T owner)
    {
      CycleTime -= engine.Game.TimeSinceRender;
      if (CyclesRemaining > 0)
      {
        if (CycleTime < 0)
        {
          CyclesRemaining--;
          CycleTime += CyclePeriod;
          Action?.Invoke(owner);
        }
      }
    }
  }
}
