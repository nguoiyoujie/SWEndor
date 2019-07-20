using System;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  // preserved due to use for ActorTypeInfo
  public struct TimedLifeData
  {
    public bool OnTimedLife;
    public float TimedLife;

    public TimedLifeData(bool enabled = false, float time = 100)
    {
      OnTimedLife = enabled;
      TimedLife = time;
    }
  }
}
