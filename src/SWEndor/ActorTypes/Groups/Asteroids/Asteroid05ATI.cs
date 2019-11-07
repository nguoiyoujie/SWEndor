using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid05ATI : Groups.Asteroid
  {
    internal Asteroid05ATI(Factory owner) : base(owner, "ASTR5", "Asteroid 05")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid05.x");
    }
  }
}

