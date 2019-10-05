using SWEndor.ActorTypes.Components;
using System;

namespace SWEndor.Models
{
  public struct TimerModel<T>
    where T :
    IActorState
  {
    public enum TimerStates { INACTIVE, ACTIVE, EXPIRED }

    public float Interval { get; private set; }
    public float TimeRemaining { get; private set; }
    public TimerStates State { get; private set; }
    public Action<T> FireAction;
    private T _target;

    public void InitAsDyingTimer(T a, ref TimedLifeData data)
    {
      _target = a;
      Interval = data.TimedLife;
      TimeRemaining = data.TimedLife;
      State = data.OnTimedLife ? TimerStates.ACTIVE : TimerStates.INACTIVE;
      FireAction = (t) => { t.SetState_Dead(); };
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
      Interval = time;
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
          FireAction?.Invoke(_target);
        }
      }
    }
  }
}
