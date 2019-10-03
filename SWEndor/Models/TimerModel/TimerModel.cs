using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Explosions;
using SWEndor.ExplosionTypes;
using System;

namespace SWEndor.Models
{
  public struct TimerModel
  {
    public enum TimerStates { INACTIVE, ACTIVE, EXPIRED }

    public float TimeRemaining { get; private set; }
    public TimerStates State { get; private set; }
    public Action FireAction;

    public void InitAsDyingTimer(ActorInfo a, ActorTypeInfo type)
    {
      TimeRemaining = type.TimedLifeData.TimedLife;
      State = type.TimedLifeData.OnTimedLife ? TimerStates.ACTIVE : TimerStates.INACTIVE;
      FireAction = () => { a.SetState_Dead(); };
    }

    public void InitAsDyingTimer(ExplosionInfo expl, ExplosionTypeInfo type)
    {
      TimeRemaining = type.TimedLifeData.TimedLife;
      State = type.TimedLifeData.OnTimedLife ? TimerStates.ACTIVE : TimerStates.INACTIVE;
      FireAction = () => { expl.SetState_Dead(); };
    }

    public void Start()
    {
      if (State == TimerStates.INACTIVE)
        State = TimerStates.ACTIVE;
    }

    public void Pause()
    {
      if (State == TimerStates.ACTIVE)
        State = TimerStates.INACTIVE;
    }

    public void Set(float time, bool startnow)
    {
      TimeRemaining = time;
      if (startnow)
        Start();
    }

    public void Tick(float time)
    {
      if (State == TimerStates.ACTIVE)
      {
        TimeRemaining -= time;

        if (TimeRemaining < 0)
        {
          State = TimerStates.EXPIRED;
          FireAction?.Invoke();
        }
      }
    }
  }
}
