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
      int id = ps[0].ValueI;
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
      int id = ps[0].ValueI;
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
      int id = ps[0].ValueI;
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
      int id = ps[0].ValueI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.UnlockOne();
      return Val.TRUE;
    }

    public static Val ClearQueue(Context context, params Val[] ps)
    {
      int id = ps[0].ValueI;
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.ForceClearQueue();
      return Val.TRUE;
    }

    public static Val ForceClearQueue(Context context, params Val[] ps)
    {
      int id = ps[0].ValueI;
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
      switch (ps[1].ValueS.ToLower())
      {
        case "idle":
          action = new Idle();
          break;

        case "hunt":
          action = new Hunt();
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
            action = new Wait(ps[2].ValueF);
          break;

        case "evade":
          if (ps.Length <= 2)
            action = new Evade();
          else
            action = new Evade(ps[2].ValueF);
          break;

        case "move":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].ValueF, ps[3].ValueF, ps[4].ValueF);
            float speed = ps[5].ValueF;

            switch (ps.Length)
            {
              case 6:
                action = new Move(dest, speed);
                break;
              case 7:
                action = new Move(dest, speed, ps[6].ValueF);
                break;
              default:
              case 8:
                action = new Move(dest, speed, ps[6].ValueF, ps[7].ValueB);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ValueS.ToLower(), "6", ps.Length.ToString()));
          break;

        case "forcedmove":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].ValueF, ps[3].ValueF, ps[4].ValueF);
            float speed = ps[5].ValueF;

            switch (ps.Length)
            {
              case 6:
                action = new ForcedMove(dest, speed);
                break;
              case 7:
                action = new ForcedMove(dest, speed, ps[6].ValueF);
                break;
              default:
              case 8:
                action = new ForcedMove(dest, speed, ps[6].ValueF, ps[7].ValueF);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ValueS.ToLower(), "6", ps.Length.ToString()));
          break;

        case "rotate":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].ValueF, ps[3].ValueF, ps[4].ValueF);
            float speed = ps[5].ValueF;

            switch (ps.Length)
            {
              case 6:
                action = new Rotate(dest, speed);
                break;
              case 7:
                action = new Rotate(dest, speed, ps[6].ValueF);
                break;
              default:
              case 8:
                action = new Rotate(dest, speed, ps[6].ValueF, ps[7].ValueB);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ValueS.ToLower(), "6", ps.Length.ToString()));
          break;

        case "hyperspacein":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(ps[2].ValueF, ps[3].ValueF, ps[4].ValueF);
            action = new HyperspaceIn(dest);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ValueS.ToLower(), "5", ps.Length.ToString()));
          break;

        case "hyperspaceout":
          action = new HyperspaceOut();
          break;

        case "attackactor":
          if (ps.Length >= 3)
          {
            tgtid = ps[2].ValueI;
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ValueS.ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = new AttackActor(tgtid);
                break;
              case 4:
                action = new AttackActor(tgtid, ps[3].ValueF);
                break;
              case 5:
                action = new AttackActor(tgtid, ps[3].ValueF, ps[4].ValueF);
                break;
              case 6:
                action = new AttackActor(tgtid, ps[3].ValueF, ps[4].ValueF, ps[5].ValueB);
                break;
              default:
              case 7:
                action = new AttackActor(tgtid, ps[3].ValueF, ps[4].ValueF, ps[5].ValueB, ps[6].ValueF);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ValueS.ToLower(), "3", ps.Length.ToString()));
          break;

        case "followactor":
          if (ps.Length >= 3)
          {
            tgtid = ps[2].ValueI;
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ValueS.ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = new FollowActor(tgtid);
                break;
              case 4:
                action = new FollowActor(tgtid, ps[3].ValueF);
                break;
              default:
              case 5:
                action = new FollowActor(tgtid, ps[3].ValueF, ps[4].ValueB);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ValueS.ToLower(), "3", ps.Length.ToString()));
          break;

        case "avoidcollisionrotate":
          if (ps.Length >= 8)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR(ps[2].ValueF, ps[3].ValueF, ps[4].ValueF);
            TV_3DVECTOR rot = new TV_3DVECTOR(ps[5].ValueF, ps[6].ValueF, ps[7].ValueF);

            switch (ps.Length)
            {
              case 8:
                action = new AvoidCollisionRotate(pos, rot);
                break;
              default:
              case 9:
                action = new AvoidCollisionRotate(pos, rot, ps[8].ValueF);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ValueS.ToLower(), "8", ps.Length.ToString()));
          break;

        case "setgamestateb":
          if (ps.Length >= 4)
          {
            action = new SetGameStateB(ps[2].ValueS,ps[3].ValueB);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ValueS.ToLower(), "4", ps.Length.ToString()));
          break;

      }
      return action;
    }
  }
}
