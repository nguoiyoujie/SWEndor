using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Asteroid01ATI : Groups.Asteroid
  {
    internal Asteroid01ATI(Factory owner) : base(owner, "ASTR1", "Asteroid 01")
    {
      MeshData = new MeshData(Engine, Name, @"asteroids\asteroid01.x");
    }
  }
}

