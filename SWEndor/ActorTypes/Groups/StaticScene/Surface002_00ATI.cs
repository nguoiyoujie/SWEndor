using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_00ATI : Groups.StaticScene
  {
    internal Surface002_00ATI(Factory owner) : base(owner, "Surface002_00ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_00.x");
    }
  }
}

