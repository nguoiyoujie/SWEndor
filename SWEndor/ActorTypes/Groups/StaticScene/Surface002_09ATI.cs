﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_09ATI : Groups.GroundSurface
  {
    internal Surface002_09ATI(Factory owner) : base(owner, "Surface002_09ATI")
    {
      MeshData = new MeshData(Name, @"surface\surface002_09.x");
    }
  }
}

