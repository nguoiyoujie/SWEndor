using SWEndor.ActorTypes.Components;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower03_RuinsATI : Groups.Debris
  {
    internal Tower03_RuinsATI(Factory owner) : base(owner, "Turbolaser Tower 03 Ruins")
    {
      RenderData.CullDistance = 10000;
      MeshData = new MeshData(Name, @"towers\tower_03_destroyed.x");
    }
  }
}

