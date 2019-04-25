using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower03_RuinsATI : Groups.Debris
  {
    internal Tower03_RuinsATI(Factory owner) : base(owner, "Turbolaser Tower 03 Ruins")
    {
      CullDistance = 10000;
      SourceMeshPath = Path.Combine(Globals.ModelPath, @"towers\tower_03_destroyed.x");
    }
  }
}

