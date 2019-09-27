using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
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

      DyingMoveData.Kill();

      RenderData.CullDistance = 10000;

      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.ADDON;
      RenderData.RadarType = RadarType.NULL;

      Mask = ComponentMask.ACTOR;
    }
  }
}

