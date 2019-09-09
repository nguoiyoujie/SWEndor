using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid03ATI : Groups.Asteroid
  {
    internal Asteroid03ATI(Factory owner) : base(owner, "Asteroid 03")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid03.x");
    }
  }
}

