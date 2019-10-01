﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box4ATI : Groups.SpinningDebris
  {
    internal Transport_Box4ATI(Factory owner) : base(owner, "BOX4", "Transport Box 4")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 100;

      MeshData = new MeshData(Name, @"transport\transport_box4.x");
      DyingMoveData.Spin(100, 450);
    }
  }
}

