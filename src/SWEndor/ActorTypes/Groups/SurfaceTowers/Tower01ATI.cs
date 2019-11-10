using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Tower01ATI : Groups.SurfaceTower
  {
    internal Tower01ATI(Factory owner) : base(owner, "DEFT", "Deflector Tower")
    {
      SystemData.MaxShield = 100;
      CombatData.ImpactDamage = 120;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Name, @"towers\tower_01.x");
      AddOns = new AddOnData[] { new AddOnData("ADVGUN", new TV_3DVECTOR(0, 135, 0), new TV_3DVECTOR(0, 0, 0), true) };
    }
  }
}

