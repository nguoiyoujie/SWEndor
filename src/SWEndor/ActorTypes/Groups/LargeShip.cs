using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  internal class LargeShip : ActorTypeInfo
  {
    internal LargeShip(Factory owner, string id, string name) : base(owner, id, name)
    {
      CombatData = CombatData.DefaultShip;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeData[]
      {
        new ExplodeData("EXPL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL01", 1, 2, ExplodeTrigger.ON_DEATH),
        new ExplodeData("EXPW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      DyingMoveData.Sink(0.06f, 15f, 2.5f);
      RenderData.CullDistance = 20000;

      MoveLimitData.ZTilt = 5f;
      MoveLimitData.ZNormFrac = 0.008f;
      RenderData.RadarSize = 10;
      TimedLifeData = new TimedLifeData(false, 25);

      ArmorData.Data.Put(DamageType.LIGHT, 1.25f);
      ArmorData.Data.Put(DamageType.HEAVY, 1.5f);

      AIData.TargetType = TargetType.SHIP;
      RenderData.RadarType = RadarType.RECTANGLE_GIANT;

      Mask = ComponentMask.ACTOR;
      AIData.HuntWeight = 5;
    }
  }
}

