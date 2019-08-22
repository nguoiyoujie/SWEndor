using SWEndor.Actors;
using SWEndor.AI.Actions;

namespace SWEndor.AI.Squads.Missions
{
  public class AttackActor : MissionInfo
  {
    private int TargetActor_ID;
    private float Speed;
    private float Close_enough_distance;

    public AttackActor(ActorInfo target, float speed, float close_enough_distance = -1)
    {
      TargetActor_ID = target.ID;
      Speed = speed;
      Close_enough_distance = close_enough_distance;
    }

    public override ActionInfo GetNewAction()
    {
      return new Actions.AttackActor(TargetActor_ID, Speed, Close_enough_distance);
    }

    public override void Process(Engine engine, Squadron squad)
    {
      ActorInfo target = engine.ActorFactory.Get(TargetActor_ID);
      if (target == null || target.IsDyingOrDead)
        Complete = true;
    }
  }
}
