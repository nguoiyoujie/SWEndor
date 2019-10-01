using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class NebulonBMissilePodATI : Groups.Turbolasers
  {
    internal NebulonBMissilePodATI(Factory owner) : base(owner, "NEBLMPOD", "Nebulon B Missile Pod")
    {
      CombatData.MaxStrength = 140; //32
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\nebulonb_missilepod.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "NEBL_MISL" };
    }
  }
}

