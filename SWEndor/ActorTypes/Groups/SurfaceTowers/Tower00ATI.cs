using MTV3D65;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class Tower00ATI : Groups.SurfaceTower
  {
    internal Tower00ATI(Factory owner) : base(owner, "ADVT", "Advanced Turbolaser Tower")
    {
      MaxStrength = 120;
      ImpactDamage = 120;

      RenderData.RadarSize = 2.5f;

      ScoreData = new ScoreData(50, 5000);

      MeshData = new MeshData(Name, @"towers\tower_00.x");

      AddOns = new AddOnData[] 
      {
        new AddOnData("TGUN", new TV_3DVECTOR(95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
        , new AddOnData("TGUN", new TV_3DVECTOR(-95, 155, 0), new TV_3DVECTOR(0, 0, 0), true)
      };
    }
  }
}

