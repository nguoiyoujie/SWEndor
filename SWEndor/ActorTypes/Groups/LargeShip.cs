using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;

namespace SWEndor.ActorTypes.Groups
{
  public class LargeShip : ActorTypeInfo
  {
    internal LargeShip(Factory owner, string name): base(owner, name)
    {
      CombatData = CombatData.DefaultShip;

      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL00", 0.5f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL01", 1, 2, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW01", 1, 1, ExplodeTrigger.ON_DEATH)
      };

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

    public override void Dying<A1>(A1 self)
    {
      base.Dying(self);
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      ainfo.DyingTimer.Set(25).Start();
      CombatSystem.Deactivate(Engine, ainfo.ID);
    }

    public override void Dead<A1>(A1 self)
    {
      base.Dead(self);
      /*
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Explosion Wave"));
      acinfo.Position = ainfo.GetPosition();
      ainfo.AddChild(ActorFactory.Create(acinfo));
      */
    }
  }
}

