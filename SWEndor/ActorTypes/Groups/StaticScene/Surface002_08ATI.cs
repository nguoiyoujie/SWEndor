using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_08ATI : Groups.StaticScene
  {
    internal Surface002_08ATI(Factory owner) : base(owner, "Surface002_08ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_08.x");
    }
  }
}

