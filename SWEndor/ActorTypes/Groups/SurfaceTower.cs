using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;

namespace SWEndor.ActorTypes.Groups
{
  public class SurfaceTower : ActorTypeInfo
  {
    internal SurfaceTower(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;

      Explodes = new ExplodeInfo[] {
        new ExplodeInfo("ExpL00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      CullDistance = 10000;

      RadarSize = 1.5f;

      TargetType = TargetType.ADDON | TargetType.STRUCTURE;
      RadarType = RadarType.FILLED_SQUARE;

      Mask = ComponentMask.STATIC_ACTOR;
    }
  }
}

