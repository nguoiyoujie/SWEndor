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

      ExplodeSystemData.Explodes = new ExplodeData[] {
        new ExplodeData("EXPL00", 1, 5, ExplodeTrigger.ON_DEATH)
      };

      DyingMoveData.Kill();

      RenderData.CullDistance = 15000;

      RenderData.RadarSize = 0;
      AIData.TargetType = TargetType.ADDON;
      RenderData.RadarType = RadarType.NULL;

      ArmorData.Data.Put(DamageType.MISSILE, 1.05f);
      ArmorData.Data.Put(DamageType.TORPEDO, 1.15f);

      Mask = ComponentMask.ACTOR;
    }
  }
}

