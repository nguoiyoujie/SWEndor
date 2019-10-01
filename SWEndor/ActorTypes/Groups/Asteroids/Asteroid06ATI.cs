﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid06ATI : Groups.Asteroid
  {
    internal Asteroid06ATI(Factory owner) : base(owner, "ASTR6", "Asteroid 06")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid06.x");
    }
  }
}

