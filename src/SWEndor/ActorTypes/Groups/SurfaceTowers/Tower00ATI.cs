using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  internal class Tower00ATI : Groups.SurfaceTower
  {
    internal Tower00ATI(Factory owner) : base(owner, "ADVT", "Advanced Turbolaser Tower")
    {
      SystemData.MaxShield = 120;
      CombatData.ImpactDamage = 120;

      RenderData.RadarSize = 2.5f;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Engine, Name, @"towers\tower_00.x");

      AddOnData.AddOns = new AddOnData[] 
      {
        new AddOnData("TGUN", new TV_3DVECTOR(95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("TGUN", new TV_3DVECTOR(-95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
      };
    }
  }
}

