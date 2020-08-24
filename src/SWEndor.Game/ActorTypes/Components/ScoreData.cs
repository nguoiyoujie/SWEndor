﻿using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct ScoreData
  {
    [INIValue]
    public int PerStrength;

    [INIValue]
    public int DestroyBonus;

    public ScoreData(int perStrength, int destroyBonus)
    {
      PerStrength = perStrength;
      DestroyBonus = destroyBonus;
    }
  }
}
