using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower00_RuinsATI : Groups.Debris
  {
    internal Tower00_RuinsATI(Factory owner) : base(owner, "Turbolaser Tower 00 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_00_destroyed.x");
    }
  }
}

