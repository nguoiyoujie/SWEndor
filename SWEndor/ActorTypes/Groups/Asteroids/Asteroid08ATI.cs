using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid08ATI : Groups.Asteroid
  {
    internal Asteroid08ATI(Factory owner) : base(owner, "Asteroid 08")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid08.x");
    }
  }
}

