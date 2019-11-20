using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class ArqTurboLaserATI : Groups.Turbolasers
  {
    internal ArqTurboLaserATI(Factory owner) : base(owner, "ARQTLSR", "Arquitens Turbolaser Tower")
    {
      SystemData.MaxShield = 20;
      SystemData.MaxHull = 40;
      CombatData.ImpactDamage = 16;

      MeshData = new MeshData(Engine, Name, @"turbotowers\acclamator_turbolaser.x");
      DyingMoveData.Kill();

      Loadouts = new string[] { "ARQT_LASR" };
    }
  }
}

