using SWEndor.Actors;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SquadFns
  {
    public static Val JoinSquad(Context context, int actorID1, int actorID2)
    {
      ActorInfo a1 = context.Engine.ActorFactory.Get(actorID1);
      ActorInfo a2 = context.Engine.ActorFactory.Get(actorID2);
      if (context.Engine.GameScenarioManager.Scenario == null || a1 == null || a2 == null)
        return Val.FALSE;

      a2.JoinSquad(a1);
      return Val.TRUE;
    }

    public static Val RemoveFromSquad(Context context, int actorID)
    {
      ActorInfo a1 = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || a1 == null)
        return Val.FALSE;

      a1.Squad = null;

      return Val.TRUE;
    }

    public static Val MakeSquadLeader(Context context, int actorID)
    {
      ActorInfo a1 = context.Engine.ActorFactory.Get(actorID);
      if (context.Engine.GameScenarioManager.Scenario == null || a1 == null)
        return Val.FALSE;

      a1.MakeSquadLeader();
      return Val.TRUE;
    }
  }
}
