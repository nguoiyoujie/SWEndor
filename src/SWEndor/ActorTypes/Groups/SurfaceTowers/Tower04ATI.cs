using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Tower04ATI : Groups.SurfaceTower
  {
    internal Tower04ATI(Factory owner) : base(owner, "SUPT", "Super Deflector Tower")
    {
      SystemData.MaxShield = 100;
      CombatData.ImpactDamage = 120;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Engine, Name, @"towers\tower_01.x");
      AddOnData.AddOns = new AddOnData[] { new AddOnData("SUPGUN", new TV_3DVECTOR(0, 135, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }
  }
}

