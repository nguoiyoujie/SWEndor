using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid05ATI : Groups.Asteroid
  {
    internal Asteroid05ATI(Factory owner) : base(owner, "Asteroid 05")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid05.x");
    }
  }
}

