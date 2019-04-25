using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid04ATI : Groups.Asteroid
  {
    internal Asteroid04ATI(Factory owner) : base(owner, "Asteroid 04")
    {
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"asteroids\asteroid04.x");
    }
  }
}

