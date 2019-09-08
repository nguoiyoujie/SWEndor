using SWEndor.Actors;

namespace SWEndor.ActorTypes.Instances
{
  public class InvisibleRebelTurboLaserATI : Groups.AddOn
  {
    internal InvisibleRebelTurboLaserATI(Factory owner) : base(owner, "Invisible Rebel Turbo Laser")
    {
      RadarSize = 0;

      TargetType = TargetType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);

      Loadouts = new string[] { "FALC_LASR" };
    }
  }
}

