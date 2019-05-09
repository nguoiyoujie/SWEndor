using SWEndor.Actors.Data;
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
      MaxSpeed = 850;
      MinSpeed = 850;
      MaxTurnRate = 75;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"projectiles\torpedo.x");
    }
  }
}

