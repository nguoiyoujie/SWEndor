using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower02_RuinsATI : Groups.Debris
  {
    internal Tower02_RuinsATI(Factory owner) : base(owner, "Turbolaser Tower 02 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_02_destroyed.x");
    }
  }
}

