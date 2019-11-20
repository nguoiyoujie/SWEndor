using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Asteroid07ATI : Groups.Asteroid
  {
    internal Asteroid07ATI(Factory owner) : base(owner, "ASTR7", "Asteroid 07")
    {
      MeshData = new MeshData(Engine, Name, @"asteroids\asteroid07.x");
    }
  }
}

