using SWEndor.Game.Actors;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;

namespace SWEndor.Game.AI.Squads.Missions
{
  public class AttackActor : MissionInfo
  {
    public readonly int Target_ActorID = -1;
    private float Speed;
    private float Close_enough_distance;

    public AttackActor(ActorInfo target, float speed, float close_enough_distance = -1)
    {
      Target_ActorID = target.ID;
      Speed = speed;
      Close_enough_distance = close_enough_distance;
    }

    public override ActionInfo GetNewAction(Engine engine, Squadron squad)
    {
      return Actions.AttackActor.GetOrCreate(Target_ActorID, Speed, Close_enough_distance);
    }

    public override void Process(Engine engine, Squadron squad)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || target.IsDyingOrDead)
        Complete = true;
    }
  }
}
