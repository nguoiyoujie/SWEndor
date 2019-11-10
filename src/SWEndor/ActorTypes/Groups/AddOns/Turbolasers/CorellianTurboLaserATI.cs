using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class CorellianTurboLaserATI : Groups.Turbolasers
  {
    internal CorellianTurboLaserATI(Factory owner) : base(owner, "CORLLSR", "Corellian Turbolaser Tower")
    {
      SystemData.MaxShield = 30;
      SystemData.MaxHull = 45;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Name, @"turbotowers\corellian_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "CRLN_LASR" };
    }
  }
}

