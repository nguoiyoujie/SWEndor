using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_03ATI : Groups.StaticScene
  {
    internal Surface002_03ATI(Factory owner) : base(owner, "Surface002_03ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_03.x");
    }
  }
}

