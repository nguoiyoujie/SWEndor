using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid01ATI : Groups.Asteroid
  {
    internal Asteroid01ATI(Factory owner) : base(owner, "Asteroid 01")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid01.x");
    }
  }
}

