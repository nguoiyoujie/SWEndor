using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class LargeShip : ActorTypeInfo
  {
    internal LargeShip(Factory owner, string name): base(owner, name)
    {
      CombatData = CombatData.DefaultShip;
      ExplodeData = new ExplodeData(0.5f, 1, "ExplosionSm", DeathExplosionTrigger.ALWAYS, 2, "ExplosionLg");

      CullDistance = 20000;

      ZTilt = 5f;
      ZNormFrac = 0.008f;
      RadarSize = 10;

      TargetType = TargetType.SHIP;
      RadarType = RadarType.RECTANGLE_GIANT;

      Mask = ComponentMask.ACTOR;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSink(0.06f, 15f, 2.5f);
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState.IsDying())
      {
        TimedLifeSystem.Activate(Engine, ainfo, 25);
        CombatSystem.Deactivate(Engine, ainfo);
      }
      else if (ainfo.ActorState.IsDead())
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Explosion Wave"));
        acinfo.Position = ainfo.GetPosition();
        ainfo.AddChild(ActorFactory.Create(acinfo));
      }
    }
  }
}

