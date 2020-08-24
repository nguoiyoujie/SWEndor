﻿using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct DebrisSystemData
  {
    private static DebrisSpawnerData[] NullDebris = new DebrisSpawnerData[0];

    [INISubSectionList(SubsectionPrefix = "DEBR")]
    internal DebrisSpawnerData[] Debris;

    public static DebrisSystemData Default { get { return new DebrisSystemData(NullDebris); } }

    public DebrisSystemData(DebrisSpawnerData[] debris)
    {
      Debris = debris;
    }

    public void Process(Engine e, ActorInfo a)
    {
      foreach (DebrisSpawnerData ds in Debris)
        ds.Process(e, a);
    }
  }
}