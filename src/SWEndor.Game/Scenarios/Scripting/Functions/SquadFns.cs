using SWEndor.Game.Actors;
using Primrose.Expressions;
using SWEndor.Game.Core;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class SquadFns
  {
    public static Val JoinSquad(IContext context, int actorID1, int actorID2)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo a1 = e.ActorFactory.Get(actorID1);
      ActorInfo a2 = e.ActorFactory.Get(actorID2);
      if (e.GameScenarioManager.Scenario == null || a1 == null || a2 == null)
        return Val.FALSE;

      a2.JoinSquad(a1);
      return Val.TRUE;
    }

    public static Val RemoveFromSquad(IContext context, int actorID)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo a1 = e.ActorFactory.Get(actorID);
      if (e.GameScenarioManager.Scenario == null || a1 == null)
        return Val.FALSE;

      a1.Squad = null;

      return Val.TRUE;
    }

    public static Val MakeSquadLeader(IContext context, int actorID)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo a1 = e.ActorFactory.Get(actorID);
      if (e.GameScenarioManager.Scenario == null || a1 == null)
        return Val.FALSE;

      a1.MakeSquadLeader();
      return Val.TRUE;
    }
  }
}
