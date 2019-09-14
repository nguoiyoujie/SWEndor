namespace SWEndor.ActorTypes.Components
{
  public struct TimedLifeData
  {
    public readonly bool OnTimedLife;
    public readonly float TimedLife;

    public TimedLifeData(bool enabled, float time)
    {
      OnTimedLife = enabled;
      TimedLife = time;
    }
  }
}
