﻿using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class TorpedoATI : Groups.MissileProjectile
  {
    internal TorpedoATI(Factory owner) : base(owner, "Torpedo")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 25);

      ImpactDamage = 12;
      MoveLimitData.MaxSpeed = 850;
      MoveLimitData.MinSpeed = 850;
      MoveLimitData.MaxTurnRate = 75;

      MeshData = new MeshData(Name, @"projectiles\torpedo.x");
    }
  }
}

