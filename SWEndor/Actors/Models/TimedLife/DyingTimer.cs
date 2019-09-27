using SWEndor.ActorTypes;

namespace SWEndor.Actors.Models
{
  public enum TimerStates
  {
    INACTIVE,
    ACTIVE,
    EXPIRED
  }

  public struct TimerModel
  {
    public float TimeRemaining { get; private set; }
    public TimerStates State { get; private set; }

    public void InitAsDyingTimer(ActorTypeInfo type)
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

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public float DyingTimeRemaining { get { return DyingTimer.TimeRemaining; } }
    public void DyingTimerStart() { DyingTimer.Start(); }
    public void DyingTimerPause() { DyingTimer.Pause(); }
    public void DyingTimerSet(float time, bool startnow) { DyingTimer.Set(time, startnow); }
  }
}
