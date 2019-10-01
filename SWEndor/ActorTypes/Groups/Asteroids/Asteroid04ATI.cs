using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Asteroid04ATI : Groups.Asteroid
  {
    internal Asteroid04ATI(Factory owner) : base(owner, "ASTR4", "Asteroid 04")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid04.x");
    }
  }
}

