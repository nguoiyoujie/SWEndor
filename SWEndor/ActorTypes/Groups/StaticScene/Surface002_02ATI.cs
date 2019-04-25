using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_02ATI : Groups.StaticScene
  {
    internal Surface002_02ATI(Factory owner) : base(owner, "Surface002_02ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_02.x");
    }
  }
}

