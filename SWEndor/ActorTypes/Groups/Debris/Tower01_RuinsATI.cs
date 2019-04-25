using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower01_RuinsATI : Groups.Debris
  {
    internal Tower01_RuinsATI(Factory owner) : base(owner, "Turbolaser Tower 01 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_01_destroyed.x");
    }
  }
}

