using SWEndor.Actors;
using SWEndor.AI.Actions;
using SWEndor.Core;

namespace SWEndor.AI.Squads.Missions
{
  public class AssistActor : MissionInfo
  {
    public readonly int Target_ActorID = -1;

    public AssistActor(ActorInfo target)
    {
      Target_ActorID = target.ID;
    }

    public override ActionInfo GetNewAction(Engine engine, Squadron squad)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);

      // eliminate threats
      if (target != null)
      {
        if (target.Squad.IsNull)
          return Actions.AttackActor.GetOrCreate(target.Squad.GetThreatFirst(engine)?.ID ?? -1);

        // help attack
        if (target.CurrentAction is AI.Actions.AttackActor)
          return Actions.AttackActor.GetOrCreate(((AI.Actions.AttackActor)target.CurrentAction).Target_ActorID);
      }

      // follow
      return new Actions.FollowActor(Target_ActorID);
    }

    public override void Process(Engine engine, Squadron squad)
    {
      ActorInfo target = engine.ActorFactory.Get(Target_ActorID);
      if (target == null || target.IsDyingOrDead)
        Complete = true;
    }
  }
}
