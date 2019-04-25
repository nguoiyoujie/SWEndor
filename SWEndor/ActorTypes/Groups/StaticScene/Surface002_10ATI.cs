using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_10ATI : Groups.StaticScene
  {
    internal Surface002_10ATI(Factory owner) : base(owner, "Surface002_10ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_10.x");
    }
  }
}

