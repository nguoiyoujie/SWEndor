using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class LargeShip : ActorTypeInfo
  {
    internal LargeShip(Factory owner, string name) : base(owner, name)
    {
      CombatData = CombatData.DefaultShip;
      Armor = ArmorInfo.Default;
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 2, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      CullDistance = 20000;

      ZTilt = 5f;
      ZNormFrac = 0.008f;
      RadarSize = 10;

      TargetType = TargetType.SHIP;
      RadarType = RadarType.RECTANGLE_GIANT;

      Mask = ComponentMask.ACTOR;
      HuntWeight = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSink(0.06f, 15f, 2.5f);
    }

    public override void Dying(ActorInfo ainfo)
    {
      base.Dying(ainfo);

      ainfo.DyingTimerSet(25, true);
    }
  }
}

