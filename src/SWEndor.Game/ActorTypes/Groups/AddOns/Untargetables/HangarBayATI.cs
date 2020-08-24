using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;

namespace SWEndor.Game.ActorTypes.Instances
{
  internal class HangarBayATI : Groups.AddOn
  {
    internal HangarBayATI(Factory owner) : base(owner, "HANGAR", "Hangar Bay")
    {
      RenderData.RadarSize = 0;

      AIData.TargetType = TargetType.NULL;
      RenderData.RadarType = RadarType.NULL;

      Mask &= ~(ComponentMask.CAN_BECOLLIDED | ComponentMask.CAN_BETARGETED);
    }

    public override void ProcessState(Engine engine, ActorInfo ainfo)
    {
      base.ProcessState(engine, ainfo);

      ActorInfo p = ainfo.TopParent;
      p.SpawnerInfo.Process(engine, ainfo, p);
    }
  }
}


