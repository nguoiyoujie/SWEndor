using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class LargeShip : ActorTypeInfo
  {
    internal LargeShip(Factory owner, string name) : base(owner, name)
    {
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 2, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      DyingMoveData.Sink(0.06f, 15f, 2.5f);
      RenderData.CullDistance = 20000;

      MoveLimitData.ZTilt = 5f;
      MoveLimitData.ZNormFrac = 0.008f;
      RenderData.RadarSize = 10;

      AIData.TargetType = TargetType.SHIP;
      RenderData.RadarType = RadarType.RECTANGLE_GIANT;

      Mask = ComponentMask.ACTOR;
      AIData.HuntWeight = 5;
    }

    public override void Dying(Engine engine, ActorInfo ainfo)
    {
      base.Dying(engine, ainfo);

      ainfo.DyingTimerSet(25, true);
    }
  }
}

