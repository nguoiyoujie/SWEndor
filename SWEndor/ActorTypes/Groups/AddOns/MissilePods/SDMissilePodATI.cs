using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class SDMissilePodATI : Groups.Turbolasers
  {
    internal SDMissilePodATI(Factory owner) : base(owner, "Star Destroyer Missile Pod")
    {
      MaxStrength = 24; //32
      ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_missilepod.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "IMPL_MISL" };
    }
  }
}

