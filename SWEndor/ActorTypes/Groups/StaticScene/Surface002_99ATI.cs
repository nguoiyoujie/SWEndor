using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_99ATI : Groups.StaticScene
  {
    internal Surface002_99ATI(Factory owner) : base(owner, "Surface002_99ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_99.x");
    }
  }
}

