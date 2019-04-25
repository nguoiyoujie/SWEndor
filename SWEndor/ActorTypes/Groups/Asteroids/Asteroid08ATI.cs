using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid08ATI : Groups.Asteroid
  {
    internal Asteroid08ATI(Factory owner) : base(owner, "Asteroid 08")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid08.x");
    }
  }
}

