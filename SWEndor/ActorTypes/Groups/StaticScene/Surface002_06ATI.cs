using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_06ATI : Groups.StaticScene
  {
    internal Surface002_06ATI(Factory owner) : base(owner, "Surface002_06ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_06.x");
    }
  }
}

