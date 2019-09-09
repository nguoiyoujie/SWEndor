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
      ArmorData = ArmorData.Default;

      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      RenderData.CullDistance = 10000;

      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.ADDON;
      RenderData.RadarType = RadarType.NULL;

      Mask = ComponentMask.ACTOR;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.DyingMoveComponent = DyingKill.Instance;
    }
  }
}

