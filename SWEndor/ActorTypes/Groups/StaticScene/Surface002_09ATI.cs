using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface002_09ATI : Groups.StaticScene
  {
    internal Surface002_09ATI(Factory owner) : base(owner, "Surface002_09ATI")
    {
      CollisionEnabled = true;
      ImpactDamage = 200;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface002_09.x");
    }
  }
}

