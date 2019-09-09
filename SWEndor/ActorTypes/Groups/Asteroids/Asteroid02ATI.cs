using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid02ATI : Groups.Asteroid
  {
    internal Asteroid02ATI(Factory owner) : base(owner, "Asteroid 02")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid02.x");
    }
  }
}

