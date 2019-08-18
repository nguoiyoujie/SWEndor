﻿using SWEndor.ActorTypes;
using SWEndor.Primitives.Traits;

namespace SWEndor.Actors.Traits
{
  public class DyingTimer : ITimer<DyingTimer>, ITick
  {
    public enum TimerStates { INACTIVE, ACTIVE, EXPIRED }

    public float TimeRemaining { get; private set; }
    public TimerStates State { get; private set; }

    // To replace with something that does not reference ActorTypeInfo
    public void Init(ActorTypeInfo type)
    {
      TimeRemaining = type.TimedLifeData.TimedLife;
      State = type.TimedLifeData.OnTimedLife ? TimerStates.ACTIVE : TimerStates.INACTIVE;
    }

    public DyingTimer Start()
    {
      if (State == TimerStates.INACTIVE)
        State = TimerStates.ACTIVE;

      return this;
    }

    public DyingTimer Pause()
    {
      if (State == TimerStates.ACTIVE)
        State = TimerStates.INACTIVE;

      return this;
    }

    public DyingTimer Set(float time)
    {
      TimeRemaining = time;
      return this;
    }

    public void Tick<A>(A self, float time)
      where A : class, ITraitOwner
    {
      if (State == TimerStates.ACTIVE)
      {
        TimeRemaining -= time;

        if (TimeRemaining < 0)
        {
          State = TimerStates.EXPIRED;
          self.Trait<IStateModel>().MakeDead(self);
        }
      }
    }
  }
}
