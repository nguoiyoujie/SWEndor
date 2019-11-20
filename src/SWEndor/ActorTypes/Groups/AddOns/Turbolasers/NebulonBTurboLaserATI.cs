using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class NebulonBTurboLaserATI : Groups.Turbolasers
  {
    internal NebulonBTurboLaserATI(Factory owner) : base(owner, "NEBLLSR", "Nebulon B Turbolaser Tower")
    {
      SystemData.MaxShield = 135;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\nebulonb_turbolaser.x");

      Loadouts = new string[] { "NEBL_LASR" };
      DyingMoveData.Kill();
    }
  }
}

