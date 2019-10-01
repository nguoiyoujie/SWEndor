﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower02_RuinsATI : Groups.Debris
  {
    internal Tower02_RuinsATI(Factory owner) : base(owner, "RUINS2", "Turbolaser Tower 02 Ruins")
    {
      RenderData.CullDistance = 10000;
      MeshData = new MeshData(Name, @"towers\tower_02_destroyed.x");
    }
  }
}

