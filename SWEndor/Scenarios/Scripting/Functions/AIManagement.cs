using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AIManagement
  {
    public static Val QueueFirst(Context context, params Val[] ps)
    {
      int id = ps[0].vI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return Val.FALSE;

      actor.QueueFirst(action);
      return Val.TRUE;
    }

    public static Val QueueNext(Context context, params Val[] ps)
    {
      int id = ps[0].vI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return Val.FALSE;

      actor.QueueNext(action);
      return Val.TRUE;
    }

    public static Val QueueLast(Context context, params Val[] ps)
    {
      int id = ps[0].vI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return Val.FALSE;

      actor.QueueLast(action);
      return Val.TRUE;
    }

    public static Val UnlockActor(Context context, params Val[] ps)
    {
      int id = ps[0].vI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.UnlockOne();
      return Val.TRUE;
    }

    public static Val ClearQueue(Context context, params Val[] ps)
    {
      int id = ps[0].vI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.ForceClearQueue();
      return Val.TRUE;
    }

    public static Val ForceClearQueue(Context context, params Val[] ps)
    {
      int id = ps[0].vI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.ForceClearQueue();
      return Val.TRUE;
    }

    private static ActionInfo ParseAction(Val[] ps)
    {
      int tgtid = -1;
      //ActorInfo tgt = null;
      ActionInfo action = null;
      switch (ps[1].vS.ToLower())
      {
        case "idle":
          action = new Idle();
          break;

        case "hunt":
          action = Hunt.GetOrCreate();
          break;

        case "selfdestruct":
          action = new SelfDestruct();
          break;

        case "delete":
          action = new Delete();
          break;

        case "lock":
          action = new Lock();
          break;

        case "wait":
          if (ps.Length <= 2)
            action = new Wait();
          else
            action = new Wait(ps[2].vF);
          break;

        case "evade":
          if (ps.Length <= 2)
            action = Evade.GetOrCreate();
          else
            action = Evade.GetOrCreate(ps[2].vF);
          break;

        case "move":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].vF, ps[3].vF, ps[4].vF);
            float speed = ps[5].vF;

            switch (ps.Length)
            {
              case 6:
                action = new Move(dest, speed);
                break;
              case 7:
                action = new Move(dest, speed, ps[6].vF);
                break;
              default:
              case 8:
                action = new Move(dest, speed, ps[6].vF, ps[7].vB);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].vS.ToLower(), "6", ps.Length.ToString()));
          break;

        case "forcedmove":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].vF, ps[3].vF, ps[4].vF);
            float speed = ps[5].vF;

            switch (ps.Length)
            {
              case 6:
                action = new ForcedMove(dest, speed);
                break;
              case 7:
                action = new ForcedMove(dest, speed, ps[6].vF);
                break;
              default:
              case 8:
                action = new ForcedMove(dest, speed, ps[6].vF, ps[7].vF);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].vS.ToLower(), "6", ps.Length.ToString()));
          break;

        case "rotate":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].vF, ps[3].vF, ps[4].vF);
            float speed = ps[5].vF;

            switch (ps.Length)
            {
              case 6:
                action = new Rotate(dest, speed);
                break;
              case 7:
                action = new Rotate(dest, speed, ps[6].vF);
                break;
              default:
              case 8:
                action = new Rotate(dest, speed, ps[6].vF, ps[7].vB);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].vS.ToLower(), "6", ps.Length.ToString()));
          break;

        case "hyperspacein":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].vF, ps[3].vF, ps[4].vF);
            action = new HyperspaceIn(dest);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].vS.ToLower(), "5", ps.Length.ToString()));
          break;

        case "hyperspaceout":
          action = new HyperspaceOut();
          break;

        case "attackactor":
          if (ps.Length >= 3)
          {
            tgtid = ps[2].vI;
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ValueS.ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = AttackActor.GetOrCreate(tgtid);
                break;
              case 4:
                action = AttackActor.GetOrCreate(tgtid, ps[3].vF);
                break;
              case 5:
                action = AttackActor.GetOrCreate(tgtid, ps[3].vF, ps[4].vF);
                break;
              case 6:
                action = AttackActor.GetOrCreate(tgtid, ps[3].vF, ps[4].vF, ps[5].vB);
                break;
              default:
              case 7:
                action = AttackActor.GetOrCreate(tgtid, ps[3].vF, ps[4].vF, ps[5].vB, ps[6].vF);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].vS.ToLower(), "3", ps.Length.ToString()));
          break;

        case "followactor":
          if (ps.Length >= 3)
          {
            tgtid = ps[2].vI;
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ValueS.ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = new FollowActor(tgtid);
                break;
              case 4:
                action = new FollowActor(tgtid, ps[3].vF);
                break;
              default:
              case 5:
                action = new FollowActor(tgtid, ps[3].vF, ps[4].vB);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].vS.ToLower(), "3", ps.Length.ToString()));
          break;

        case "avoidcollisionrotate":
          if (ps.Length >= 8)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR(ps[2].vF, ps[3].vF, ps[4].vF);
            TV_3DVECTOR rot = new TV_3DVECTOR(ps[5].vF, ps[6].vF, ps[7].vF);

            switch (ps.Length)
            {
              case 8:
                action = AvoidCollisionRotate.GetOrCreate(pos, rot);
                break;
              default:
              case 9:
                action = AvoidCollisionRotate.GetOrCreate(pos, rot, ps[8].vF);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].vS.ToLower(), "8", ps.Length.ToString()));
          break;

        case "setgamestateb":
          if (ps.Length >= 4)
          {
            action = new SetGameStateB(ps[2].vS,ps[3].vB);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].vS.ToLower(), "4", ps.Length.ToString()));
          break;

      }
      return action;
    }
  }
}
