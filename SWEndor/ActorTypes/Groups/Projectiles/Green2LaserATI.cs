﻿using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Green2LaserATI : Groups.LaserProjectile
  {
    internal Green2LaserATI(Factory owner) : base(owner, "LSR_G2", "Green Double Laser")
    {
      CombatData.ImpactDamage = 2;
      CombatData.ImpactCloseEnoughDistance = 35;

      MeshData = new MeshData(Name, @"projectiles\green2_laser.x", 1, "Laser");
    }
  }
}

