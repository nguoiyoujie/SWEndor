﻿using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid06ATI : Groups.Asteroid
  {
    private static Asteroid06ATI _instance;
    public static Asteroid06ATI Instance()
    {
      if (_instance == null) { _instance = new Asteroid06ATI(); }
      return _instance;
    }

    private Asteroid06ATI() : base("Asteroid 06")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid06.x");
    }
  }
}

