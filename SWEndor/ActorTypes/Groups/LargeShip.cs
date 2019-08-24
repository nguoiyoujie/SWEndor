using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes.Components;

namespace SWEndor.ActorTypes.Groups
{
  public class LargeShip : ActorTypeInfo
  {
    internal LargeShip(Factory owner, string name) : base(owner, name)
    {
      CombatData = CombatData.DefaultShip;
      Armor = ArmorInfo.Default;
      ExplodeData = new ExplodeData(0.5f, 1, "ExplosionSm", DeathExplosionTrigger.ALWAYS, 2, "ExplosionLg");

      CullDistance = 20000;

      ZTilt = 5f;
      ZNormFrac = 0.008f;
      RadarSize = 10;

      TargetType = TargetType.SHIP;
      RadarType = RadarType.RECTANGLE_GIANT;

      Mask = ComponentMask.ACTOR;
      HuntWeight = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSink(0.06f, 15f, 2.5f);
    }

    public override void Dying(ActorInfo ainfo)
    {
      base.Dying(ainfo);

      ainfo.DyingTimerSet(25, true);
      CombatSystem.Deactivate(Engine, ainfo);
    }

    public override void Dead(ActorInfo ainfo)
    {
      base.Dead(ainfo);

      ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Explosion Wave"));
      acinfo.Position = ainfo.GetGlobalPosition();
      ainfo.AddChild(ActorFactory.Create(acinfo));
    }
  }
}

