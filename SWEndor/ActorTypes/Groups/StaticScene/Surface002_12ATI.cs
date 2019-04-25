using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_12ATI : Groups.StaticScene
  {
    internal Surface002_12ATI(Factory owner) : base(owner, "Surface002_12ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_12.x");
    }
  }
}

