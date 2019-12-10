using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class SurfaceGun : ActorTypeInfo
  {
    internal SurfaceGun(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;

      Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      DyingMoveData.Kill();

      RenderData.CullDistance = 10000;

      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.ADDON;
      RenderData.RadarType = RadarType.NULL;

      ArmorData.Data.Put(DamageType.LIGHT, 2);
      ArmorData.Data.Put(DamageType.HEAVY, 1.5f);

      Mask = ComponentMask.ACTOR;
    }
  }
}

