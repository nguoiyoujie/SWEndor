using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class SurfaceGun : ActorTypeInfo
  {
    internal SurfaceGun(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      Armor = ArmorInfo.Default;

      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      CullDistance = 10000;

      RadarSize = 0;
      TargetType = TargetType.ADDON;
      RadarType = RadarType.NULL;

      Mask = ComponentMask.ACTOR;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

