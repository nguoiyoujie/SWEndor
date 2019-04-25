using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid05ATI : Groups.Asteroid
  {
    internal Asteroid05ATI(Factory owner) : base(owner, "Asteroid 05")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid05.x");
    }
  }
}

