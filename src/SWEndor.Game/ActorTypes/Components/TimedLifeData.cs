﻿using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct TimedLifeData
  {
    [INIValue]
    public bool OnTimedLife;

    [INIValue]
    public float TimedLife;

    public TimedLifeData(bool enabled, float time)
    {
      OnTimedLife = enabled;
      TimedLife = time;
    }
  }
}