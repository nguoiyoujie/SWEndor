using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.ActorTypes.Groups
{
  public class Fighter : ActorTypeInfo
  {
    internal Fighter(Factory owner, string name) : base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      ArmorData = ArmorData.Default;
      Explodes = new ExplodeData[]
      {
        new ExplodeData("Electro", 1, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.ATTACH_TO_ACTOR),
        new ExplodeData("ExpS00", 0.75f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeData("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      MoveLimitData.ZTilt = 2.5f;
      MoveLimitData.ZNormFrac = 0.01f;
      RenderData.RadarSize = 2;
      DyingMoveData.Spin(180, 270);

      RenderData.CullDistance = 7500;

      AIData.TargetType = TargetType.FIGHTER;
      RenderData.RadarType = RadarType.FILLED_CIRCLE_S;

      Mask = ComponentMask.ACTOR;

      AIData.CanEvade = true;
      AIData.CanRetaliate = true;
      AIData.CanCheckCollisionAhead = true;

      AIData.HuntWeight = 5;
    }

    public override void Dying(Engine engine, ActorInfo ainfo)
    {
      base.Dying(engine, ainfo);

      ainfo.MoveData.ApplyZBalance = false;

      if (ainfo.Parent != null || (ainfo.CombatData.HitWhileDyingLeadsToDeath && Engine.Random.NextDouble() < 0.3f))
        ainfo.DyingTimerSet(0.1f, true);
      else
        ainfo.DyingTimerSet(5, true);
    }
  }
}

