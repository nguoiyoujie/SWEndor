using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class SDMissilePodATI : Groups.Turbolasers
  {
    internal SDMissilePodATI(Factory owner) : base(owner, "IMPLMPOD", "Star Destroyer Missile Pod")
    {
      CombatData.MaxStrength = 24; //32
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\star_destroyer_missilepod.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "IMPL_MISL" };
    }
  }
}

