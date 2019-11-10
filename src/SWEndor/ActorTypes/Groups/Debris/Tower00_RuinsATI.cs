using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Tower00_RuinsATI : Groups.Debris
  {
    internal Tower00_RuinsATI(Factory owner) : base(owner, "RUINS0", "Turbolaser Tower 00 Ruins")
    {
      RenderData.CullDistance = 10000;
      MeshData = new MeshData(Name, @"towers\tower_00_destroyed.x");
    }
  }
}

