using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Tower01_RuinsATI : Groups.Debris
  {
    internal Tower01_RuinsATI(Factory owner) : base(owner, "RUINS1", "Turbolaser Tower 01 Ruins")
    {
      RenderData.CullDistance = 15000;
      MeshData = new MeshData(Engine, Name, @"towers\tower_01_destroyed.x");
    }
  }
}

