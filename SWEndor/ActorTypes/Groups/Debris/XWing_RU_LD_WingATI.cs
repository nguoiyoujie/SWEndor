using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Instances
{
  public class XWing_RU_LD_WingATI : Groups.SpinningDebris
  {
    internal XWing_RU_LD_WingATI(Factory owner) : base(owner, "XWRULD", "XWing_RU_LD_WingATI")
    {
      MeshData = new MeshData(Name, @"xwing\xwing_rightup_leftdown_wing.x", 1, "Burn");
    }
  }
}

