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
    public static object QueueFirst(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (Globals.Engine.GameScenarioManager.Scenario == null || actor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      context.Engine.ActionManager.QueueFirst(actor.ID, action);
      return true;
    }

    public static object QueueNext(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (Globals.Engine.GameScenarioManager.Scenario == null || actor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      context.Engine.ActionManager.QueueNext(actor.ID, action);
      return true;
    }

    public static object QueueLast(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (Globals.Engine.GameScenarioManager.Scenario == null || actor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      context.Engine.ActionManager.QueueLast(actor.ID, action);
      return true;
    }

    public static object UnlockActor(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (Globals.Engine.GameScenarioManager.Scenario == null || actor == null)
        return false;

      context.Engine.ActionManager.UnlockOne(actor.ID);
      return true;
    }

    public static object ClearQueue(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (Globals.Engine.GameScenarioManager.Scenario == null || actor == null)
        return false;

      context.Engine.ActionManager.ClearQueue(actor.ID);
      return true;
    }

    public static object ForceClearQueue(Context context, params object[] ps)
    {
      int id = Convert.ToInt32(ps[0].ToString());
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (Globals.Engine.GameScenarioManager.Scenario == null || actor == null)
        return false;

      context.Engine.ActionManager.ForceClearQueue(actor.ID);
      return true;
    }

    private static ActionInfo ParseAction(object[] ps)
    {
      int tgtid = -1;
      //ActorInfo tgt = null;
      ActionInfo action = null;
      switch (ps[1].ToString().ToLower())
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
            action = new Wait(Convert.ToSingle(ps[2].ToString()));
          break;

        case "evade":
          if (ps.Length <= 2)
            action = new Evade();
          else
            action = new Evade(Convert.ToSingle(ps[2].ToString()));
          break;

        case "move":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()));
            float speed = Convert.ToSingle(ps[5].ToString());

            switch (ps.Length)
            {
              case 6:
                action = new Move(dest, speed);
                break;
              case 7:
                action = new Move(dest, speed, Convert.ToSingle(ps[6].ToString()));
                break;
              default:
              case 8:
                action = new Move(dest, speed, Convert.ToSingle(ps[6].ToString()), Convert.ToBoolean(ps[7].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ToString().ToLower(), 6, ps.Length));
          break;

        case "forcedmove":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()));
            float speed = Convert.ToSingle(ps[5].ToString());

            switch (ps.Length)
            {
              case 6:
                action = new ForcedMove(dest, speed);
                break;
              case 7:
                action = new ForcedMove(dest, speed, Convert.ToSingle(ps[6].ToString()));
                break;
              default:
              case 8:
                action = new ForcedMove(dest, speed, Convert.ToSingle(ps[6].ToString()), Convert.ToSingle(ps[7].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ToString().ToLower(), 6, ps.Length));
          break;

        case "rotate":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()));
            float speed = Convert.ToSingle(ps[5].ToString());

            switch (ps.Length)
            {
              case 6:
                action = new Rotate(dest, speed);
                break;
              case 7:
                action = new Rotate(dest, speed, Convert.ToSingle(ps[6].ToString()));
                break;
              default:
              case 8:
                action = new Rotate(dest, speed, Convert.ToSingle(ps[6].ToString()), Convert.ToBoolean(ps[7].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ToString().ToLower(), 6, ps.Length));
          break;

        case "hyperspacein":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()));
            action = new HyperspaceIn(dest);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ToString().ToLower(), 5, ps.Length));
          break;

        case "hyperspaceout":
          action = new HyperspaceOut();
          break;

        case "attackactor":
          if (ps.Length >= 3)
          {
            tgtid = Convert.ToInt32(ps[2].ToString());
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ToString().ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = new AttackActor(tgtid);
                break;
              case 4:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[3].ToString()));
                break;
              case 5:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()));
                break;
              case 6:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()), Convert.ToBoolean(ps[5].ToString()));
                break;
              default:
              case 7:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()), Convert.ToBoolean(ps[5].ToString()), Convert.ToSingle(ps[6].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ToString().ToLower(), 3, ps.Length));
          break;

        case "followactor":
          if (ps.Length >= 3)
          {
            tgtid = Convert.ToInt32(ps[2].ToString());
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ToString().ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = new FollowActor(tgtid);
                break;
              case 4:
                action = new FollowActor(tgtid, Convert.ToSingle(ps[3].ToString()));
                break;
              default:
              case 5:
                action = new FollowActor(tgtid, Convert.ToSingle(ps[3].ToString()), Convert.ToBoolean(ps[4].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ToString().ToLower(), 3, ps.Length));
          break;

        case "avoidcollisionrotate":
          if (ps.Length >= 8)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR(Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToSingle(ps[4].ToString()));
            TV_3DVECTOR rot = new TV_3DVECTOR(Convert.ToSingle(ps[5].ToString()), Convert.ToSingle(ps[6].ToString()), Convert.ToSingle(ps[7].ToString()));

            switch (ps.Length)
            {
              case 8:
                action = new AvoidCollisionRotate(pos, rot);
                break;
              default:
              case 9:
                action = new AvoidCollisionRotate(pos, rot, Convert.ToSingle(ps[8].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 8, ps.Length));
          break;

        case "setgamestateb":
          if (ps.Length >= 4)
          {
            action = new SetGameStateB(ps[2].ToString(), Convert.ToBoolean(ps[3].ToString()));
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[1].ToString().ToLower(), 4, ps.Length));
          break;

      }
      return action;
    }
  }
}
