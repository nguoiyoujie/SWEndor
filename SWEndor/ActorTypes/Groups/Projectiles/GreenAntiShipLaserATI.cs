﻿using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class GreenAntiShipLaserATI : Groups.LaserProjectile
  {
    internal GreenAntiShipLaserATI(Factory owner) : base(owner, "Green Anti-Ship Laser")
    {
      TimedLifeData = new TimedLifeData(true, 5);
      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH | ExplodeTrigger.ONLY_WHEN_DYINGTIME_NOT_EXPIRED)
      };

      ImpactDamage = 5;
      ImpactCloseEnoughDistance = 100;

      MeshData = new MeshData(Name, @"projectiles\green3_laser.x");
    }
  }
}

