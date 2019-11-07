using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower01_RuinsATI : Groups.Debris
  {
    internal Tower01_RuinsATI(Factory owner) : base(owner, "RUINS2", "Turbolaser Tower 01 Ruins")
    {
      RenderData.CullDistance = 10000;
      MeshData = new MeshData(Name, @"towers\tower_01_destroyed.x");
    }
  }
}

