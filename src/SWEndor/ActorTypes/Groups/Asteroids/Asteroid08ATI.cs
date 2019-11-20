using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Asteroid08ATI : Groups.Asteroid
  {
    internal Asteroid08ATI(Factory owner) : base(owner, "ASTR8", "Asteroid 08")
    {
      MeshData = new MeshData(Engine, Name, @"asteroids\asteroid08.x");
    }
  }
}

