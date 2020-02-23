using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct TimedLifeData
  {
    private const string sTimedLife = "TimedLife";

    [INIValue(sTimedLife, "OnTimedLife")]
    public bool OnTimedLife;

    [INIValue(sTimedLife, "TimedLife")]
    public float TimedLife;

    public TimedLifeData(bool enabled, float time)
    {
      OnTimedLife = enabled;
      TimedLife = time;
    }
  }
}
