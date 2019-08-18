using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;

namespace SWEndor.ActorTypes.Groups
{
  public class Fighter : ActorTypeInfo
  {
    internal Fighter(Factory owner, string name): base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;

      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpS00", 0.75f, 1, ExplodeTrigger.ON_DYING | ExplodeTrigger.CREATE_ON_MESHVERTICES),
        new ExplodeInfo("ExpL00", 1, 1, ExplodeTrigger.ON_DEATH)
      };

      ZTilt = 2.5f;
      ZNormFrac = 0.01f;
      RadarSize = 2;

      CullDistance = 7500;

      TargetType = TargetType.FIGHTER;
      RadarType = RadarType.FILLED_CIRCLE_S;

      Mask = ComponentMask.ACTOR;

      CanEvade = true;
      CanRetaliate = true;
      CanCheckCollisionAhead = true;

      HuntWeight = 5;
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);
      ainfo.DyingMoveComponent = new DyingSpin(180, 270);
    }

    public override void Dying<A1>(A1 self)
    {
      base.Dying(self);
      ActorInfo ainfo = self as ActorInfo;
      if (ainfo == null)
        return;

      ainfo.MoveData.ApplyZBalance = false;

      if (ainfo.Relation.Parent != null || (ainfo.CombatData.HitWhileDyingLeadsToDeath && Engine.Random.NextDouble() < 0.3f))
        ainfo.DyingTimer.Set(0.1f).Start();
      else
        ainfo.DyingTimer.Set(5).Start();

      CombatSystem.Deactivate(Engine, ainfo);

      ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
      acinfo.Position = ainfo.GetGlobalPosition();
      ainfo.AddChild(ActorFactory.Create(acinfo));
    }
  }
}

