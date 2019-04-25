using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_11ATI : Groups.StaticScene
  {
    internal Surface002_11ATI(Factory owner) : base(owner, "Surface002_11ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_11.x");
    }
  }
}

