using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Groups
{
  public class Fighter : ActorTypeInfo
  {
    internal Fighter(Factory owner, string name): base(owner, name)
    {
      // Combat
      CombatData = CombatData.DefaultFighter;
      ExplodeData = new ExplodeData(explosionRate: 0.75f, deathTrigger: DeathExplosionTrigger.ALWAYS);

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

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState.IsDying())
      {
        ainfo.MoveData.ApplyZBalance = false;

        if (ainfo.ParentID >= 0 || (ainfo.ActorDataSet.CombatData[ainfo.dataID].HitWhileDyingLeadsToDeath && Engine.Random.NextDouble() < 0.3f))
        {
          TimedLifeSystem.Activate(Engine, ainfo.ID, 0.1f);
          CombatSystem.Deactivate(Engine, ainfo.ID);
        }
        else
        {
          TimedLifeSystem.Activate(Engine, ainfo.ID, 5);
          CombatSystem.Deactivate(Engine, ainfo.ID);
        }

        ActorCreationInfo acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Electro"));
        acinfo.Position = ainfo.GetPosition();
        ainfo.AddChild(ActorInfo.Create(ActorFactory, acinfo).ID);
      }
    }
  }
}

