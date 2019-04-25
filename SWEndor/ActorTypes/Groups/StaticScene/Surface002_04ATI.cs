using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_04ATI : Groups.StaticScene
  {
    internal Surface002_04ATI(Factory owner) : base(owner, "Surface002_04ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_04.x");
    }
  }
}

