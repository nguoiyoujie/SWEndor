using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower00_RuinsATI : Groups.Debris
  {
    internal Tower00_RuinsATI(Factory owner) : base(owner, "Turbolaser Tower 00 Ruins")
    {
      RenderData.CullDistance = 10000;
      MeshData = new MeshData(Name, @"towers\tower_00_destroyed.x");
    }
  }
}

