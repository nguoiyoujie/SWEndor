using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Tower03_RuinsATI : Groups.Debris
  {
    internal Tower03_RuinsATI(Factory owner) : base(owner, "RUINS3", "Turbolaser Tower 03 Ruins")
    {
      RenderData.CullDistance = 10000;
      MeshData = new MeshData(Name, @"towers\tower_03_destroyed.x");
    }
  }
}

