using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class MissileATI : Groups.MissileProjectile
  {
    internal MissileATI(Factory owner) : base(owner, "Missile")
    {
      // Combat
      TimedLifeData = new TimedLifeData(true, 35);

      ImpactDamage = 7.5f;
      MoveLimitData.MaxSpeed = 700;
      MoveLimitData.MinSpeed = 700;
      MoveLimitData.MaxTurnRate = 120;

      Scale = 2.5f;

      MeshData = new MeshData(Name, @"projectiles\missile.x");
    }
  }
}

