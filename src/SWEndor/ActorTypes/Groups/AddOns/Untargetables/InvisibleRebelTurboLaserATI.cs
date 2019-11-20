using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  internal class InvisibleRebelTurboLaserATI : Groups.AddOn
  {
    internal InvisibleRebelTurboLaserATI(Factory owner) : base(owner, "INVLSR", "Laser Turret")
    {
      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.NULL;

      SystemData.MaxShield = 10;
      SystemData.MaxHull = 10;
      CombatData.ImpactDamage = 16;
      MeshData = new MeshData(Engine, Name, @"turbotowers\xq_turbolaser.x", 0.25f);
      DyingMoveData.Kill();

      //Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);

      Loadouts = new string[] { "FALC_GLSR" };
    }
  }
}

