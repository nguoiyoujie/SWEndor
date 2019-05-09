using SWEndor.Actors;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Surface001_00ATI : Groups.StaticScene
  {
    internal Surface001_00ATI(Factory owner) : base(owner, "Surface001_00ATI")
    {
      //CollisionEnabled = true;
      EnableDistanceCull = true;
      CullDistance = 20000;
      Scale = 4;

      SourceMeshPath = Path.Combine(Globals.ModelPath, @"surface\surface001_00.x");
    }
  }
}

