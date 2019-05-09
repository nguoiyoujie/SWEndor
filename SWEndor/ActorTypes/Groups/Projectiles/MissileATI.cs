using SWEndor.Actors.Data;
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
      MaxSpeed = 700;
      MinSpeed = 700;
      MaxTurnRate = 120;

      Scale = 2.5f;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\missile.x");
    }
  }
}

