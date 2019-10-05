﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Transport_Box1ATI : Groups.SpinningDebris
  {
    internal Transport_Box1ATI(Factory owner) : base(owner, "BOX1", "Transport Box 1")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 12);

      MoveLimitData.MaxSpeed = 500;
      MoveLimitData.MinSpeed = 100;

      MeshData = new MeshData(Name, @"transport\transport_box1.x", 1, "Burn");
      DyingMoveData.Spin(100, 450);
    }
  }
}

