using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Asteroid02ATI : Groups.Asteroid
  {
    internal Asteroid02ATI(Factory owner) : base(owner, "ASTR2", "Asteroid 02")
    {
      MeshData = new MeshData(Engine, Name, @"asteroids\asteroid02.x");
    }
  }
}

