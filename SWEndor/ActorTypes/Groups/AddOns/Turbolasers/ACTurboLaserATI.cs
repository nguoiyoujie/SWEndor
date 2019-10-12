using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class ACTurboLaserATI : Groups.Turbolasers
  {
    internal ACTurboLaserATI(Factory owner) : base(owner, "ACCLLSR", "Acclamator Turbolaser Tower")
    {
      SystemData.MaxShield = 20;
      SystemData.MaxHull = 50;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\acclamator_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "ACCL_LASR" };
    }
  }
}

