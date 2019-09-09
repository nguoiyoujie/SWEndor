using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid06ATI : Groups.Asteroid
  {
    internal Asteroid06ATI(Factory owner) : base(owner, "Asteroid 06")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid06.x");
    }
  }
}

