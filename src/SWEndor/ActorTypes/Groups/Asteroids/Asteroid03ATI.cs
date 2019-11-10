using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Asteroid03ATI : Groups.Asteroid
  {
    internal Asteroid03ATI(Factory owner) : base(owner, "ASTR3", "Asteroid 03")
    {
      MeshData = new MeshData(Name, @"asteroids\asteroid03.x");
    }
  }
}

