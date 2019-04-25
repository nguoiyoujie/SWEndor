using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_07ATI : Groups.StaticScene
  {
    internal Surface002_07ATI(Factory owner) : base(owner, "Surface002_07ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_07.x");
    }
  }
}

