﻿using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid07ATI : Groups.Asteroid
  {
    internal Asteroid07ATI(Factory owner) : base(owner, "Asteroid 07")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid07.x");
    }
  }
}

