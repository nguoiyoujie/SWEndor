using SWEndor.Models;

namespace SWEndor.ActorTypes.Instances
{
  public class InvisibleRebelTurboLaserATI : Groups.AddOn
  {
    internal InvisibleRebelTurboLaserATI(Factory owner) : base(owner, "INVLSR", "Invisible Rebel Turbo Laser")
    {
      RenderData.RadarSize = 0;

      AIData.TargetType = TargetType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);

      Loadouts = new string[] { "FALC_LASR" };
    }
  }
}

