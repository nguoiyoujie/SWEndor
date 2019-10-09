using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class Fighter : ActorTypeInfo
  {
    internal Fighter(Factory owner, string id, string name) : base(owner, id, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeData[]
      {
        new ExplodeData("ELECTRO", 1, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.ATTACH_TO_ACTOR),
        new ExplodeData("EXPS00", 0.75f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("EXPL00", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      MoveLimitData.ZTilt = 2.5f;
      MoveLimitData.ZNormFrac = 0.01f;
      RenderData.RadarSize = 2;
      DyingMoveData.Spin(180, 270);
      TimedLifeData = new TimedLifeData(false, 5);

      RenderData.CullDistance = 7500;

      AIData.TargetType = TargetType.FIGHTER;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_S;

      Mask = ComponentMask.ACTOR;

      AIData.CanEvade = true;
      AIData.CanRetaliate = true;
      AIData.CanCheckCollisionAhead = true;

      AIData.HuntWeight = 5;
    }
  }
}

