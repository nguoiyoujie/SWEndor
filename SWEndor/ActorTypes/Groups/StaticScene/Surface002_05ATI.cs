using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_05ATI : Groups.StaticScene
  {
    internal Surface002_05ATI(Factory owner) : base(owner, "Surface002_05ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_05.x");
    }
  }
}

