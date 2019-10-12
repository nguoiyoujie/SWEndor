using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower02ATI : Groups.SurfaceTower
  {
    internal Tower02ATI(Factory owner) : base(owner, "GUNT", "Gun Tower")
    {
      SystemData.MaxShield = 25;
      CombatData.ImpactDamage = 120;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Name, @"towers\tower_02.x");
      AddOns = new AddOnData[] { new AddOnData("TGUN", new TV_3DVECTOR(0, 50, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }
  }
}

