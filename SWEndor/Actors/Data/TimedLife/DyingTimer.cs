using SWEndor.ActorTypes;

namespace SWEndor.Actors
{
  public struct DyingTimer
  {
    public enum TimerStates { INACTIVE, ACTIVE, EXPIRED }
    public float TimeRemaining { get; private set; }
    public TimerStates State { get; private set; }

    public void Init(ActorTypeInfo type)
    {
      TimeRemaining = type.TimedLifeData.TimedLife;
      State = type.TimedLifeData.OnTimedLife ? TimerStates.ACTIVE : TimerStates.INACTIVE;
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

    public void Tick(ActorInfo self, float time)
    {
      if (State == TimerStates.ACTIVE)
      {
        TimeRemaining -= time;

        if (TimeRemaining < 0)
        {
          State = TimerStates.EXPIRED;
          self.SetState_Dead();
        }
      }
    }
  }
}
