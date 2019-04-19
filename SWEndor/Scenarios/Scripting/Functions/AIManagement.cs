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
    public static object Actor_QueueFirst(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      ActionManager.QueueFirst(GameScenarioManager.Instance().Scenario.ActiveActor.ID, action);
      return true;
    }

    public static object Actor_QueueNext(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      ActionManager.QueueNext(GameScenarioManager.Instance().Scenario.ActiveActor.ID, action);
      return true;
    }

    public static object Actor_QueueLast(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return false;

      ActionManager.QueueLast(GameScenarioManager.Instance().Scenario.ActiveActor.ID, action);
      return true;
    }

    public static object Actor_UnlockActor(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionManager.UnlockOne(GameScenarioManager.Instance().Scenario.ActiveActor.ID);
      return true;
    }

    public static object Actor_ClearQueue(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionManager.ClearQueue(GameScenarioManager.Instance().Scenario.ActiveActor.ID);
      return true;
    }

    public static object Actor_ForceClearQueue(Context context, params object[] ps)
    {
      if (GameScenarioManager.Instance().Scenario == null || GameScenarioManager.Instance().Scenario.ActiveActor == null)
        return false;

      ActionManager.ForceClearQueue(GameScenarioManager.Instance().Scenario.ActiveActor.ID);
      return true;
    }

    private static ActionInfo ParseAction(object[] ps)
    {
      int tgtid = -1;
      ActorInfo tgt = null;
      ActionInfo action = null;
      switch (ps[0].ToString().ToLower())
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
          if (ps.Length <= 1)
            action = new Wait();
          else
            action = new Wait(Convert.ToSingle(ps[1].ToString()));
          break;

        case "evade":
          if (ps.Length <= 1)
            action = new Evade();
          else
            action = new Evade(Convert.ToSingle(ps[1].ToString()));
          break;

        case "move":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            float speed = Convert.ToSingle(ps[4].ToString());

            switch (ps.Length)
            {
              case 5:
                action = new Move(dest, speed);
                break;
              case 6:
                action = new Move(dest, speed, Convert.ToSingle(ps[5].ToString()));
                break;
              default:
              case 7:
                action = new Move(dest, speed, Convert.ToSingle(ps[5].ToString()), Convert.ToBoolean(ps[6].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 5, ps.Length));
          break;

        case "forcedmove":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            float speed = Convert.ToSingle(ps[4].ToString());

            switch (ps.Length)
            {
              case 5:
                action = new ForcedMove(dest, speed);
                break;
              case 6:
                action = new ForcedMove(dest, speed, Convert.ToSingle(ps[5].ToString()));
                break;
              default:
              case 7:
                action = new ForcedMove(dest, speed, Convert.ToSingle(ps[5].ToString()), Convert.ToSingle(ps[6].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 5, ps.Length));
          break;

        case "rotate":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            float speed = Convert.ToSingle(ps[4].ToString());

            switch (ps.Length)
            {
              case 5:
                action = new Rotate(dest, speed);
                break;
              case 6:
                action = new Rotate(dest, speed, Convert.ToSingle(ps[5].ToString()));
                break;
              default:
              case 7:
                action = new Rotate(dest, speed, Convert.ToSingle(ps[5].ToString()), Convert.ToBoolean(ps[6].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 5, ps.Length));
          break;

        case "hyperspacein":
          if (ps.Length >= 4)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            action = new HyperspaceIn(dest);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 4, ps.Length));
          break;

        case "hyperspaceout":
          action = new HyperspaceOut();
          break;

        case "attackactor":
          if (ps.Length >= 2)
          {
            tgtid = Convert.ToInt32(ps[1].ToString());
            tgt = ActorInfo.Factory.Get(tgtid);
            if (tgt == null)
              throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[0].ToString().ToLower(), ps[1].ToString().ToLower()));

            switch (ps.Length)
            {
              case 2:
                action = new AttackActor(tgtid);
                break;
              case 3:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[2].ToString()));
                break;
              case 4:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
                break;
              case 5:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToBoolean(ps[4].ToString()));
                break;
              default:
              case 6:
                action = new AttackActor(tgtid, Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), Convert.ToBoolean(ps[4].ToString()), Convert.ToSingle(ps[5].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 2, ps.Length));
          break;

        case "followactor":
          if (ps.Length >= 2)
          {
            tgtid = Convert.ToInt32(ps[1].ToString());
            tgt = ActorInfo.Factory.Get(tgtid); if (tgt == null)
              throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[0].ToString().ToLower(), ps[1].ToString().ToLower()));

            switch (ps.Length)
            {
              case 2:
                action = new FollowActor(tgtid);
                break;
              case 3:
                action = new FollowActor(tgtid, Convert.ToSingle(ps[2].ToString()));
                break;
              default:
              case 4:
                action = new FollowActor(tgtid, Convert.ToSingle(ps[2].ToString()), Convert.ToBoolean(ps[3].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 2, ps.Length));
          break;

        case "avoidcollisionrotate":
          if (ps.Length >= 7)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()));
            TV_3DVECTOR rot = new TV_3DVECTOR(Convert.ToSingle(ps[4].ToString()), Convert.ToSingle(ps[5].ToString()), Convert.ToSingle(ps[6].ToString()));

            switch (ps.Length)
            {
              case 7:
                action = new AvoidCollisionRotate(pos, rot);
                break;
              default:
              case 8:
                action = new AvoidCollisionRotate(pos, rot, Convert.ToSingle(ps[7].ToString()));
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 7, ps.Length));
          break;

        case "setgamestateb":
          if (ps.Length >= 3)
          {
            action = new SetGameStateB(ps[1].ToString(), Convert.ToBoolean(ps[2].ToString()));
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ps[0].ToString().ToLower(), 3, ps.Length));
          break;

      }
      return action;
    }
  }
}
