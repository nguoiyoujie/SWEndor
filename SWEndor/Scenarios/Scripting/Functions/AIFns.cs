using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AIFns
  {
    public static Val QueueFirst(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
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
      int id = (int)ps[0];
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
      int id = (int)ps[0];
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
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.UnlockOne();
      return Val.TRUE;
    }

    public static Val ClearQueue(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
      ActorInfo actor = context.Engine.ActorFactory.Get(id);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.ForceClearQueue();
      return Val.TRUE;
    }

    public static Val ForceClearQueue(Context context, params Val[] ps)
    {
      int id = (int)ps[0];
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
      switch (((string)ps[1]).ToLower())
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
            action = new Wait((float)ps[2]);
          break;

        case "evade":
          if (ps.Length <= 2)
            action = Evade.GetOrCreate();
          else
            action = Evade.GetOrCreate((float)ps[2]);
          break;

        case "move":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR((float)ps[2], (float)ps[3], (float)ps[4]);
            float speed = (float)ps[5];

            switch (ps.Length)
            {
              case 6:
                action = new Move(dest, speed);
                break;
              case 7:
                action = new Move(dest, speed, (float)ps[6]);
                break;
              default:
              case 8:
                action = new Move(dest, speed, (float)ps[6], (bool)ps[7]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "6", ps.Length.ToString()));
          break;

        case "forcedmove":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR((float)ps[2], (float)ps[3], (float)ps[4]);
            float speed = (float)ps[5];

            switch (ps.Length)
            {
              case 6:
                action = new ForcedMove(dest, speed);
                break;
              case 7:
                action = new ForcedMove(dest, speed, (float)ps[6]);
                break;
              default:
              case 8:
                action = new ForcedMove(dest, speed, (float)ps[6], (float)ps[7]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "6", ps.Length.ToString()));
          break;

        case "rotate":
          if (ps.Length >= 6)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR((float)ps[2], (float)ps[3], (float)ps[4]);
            float speed = (float)ps[5];

            switch (ps.Length)
            {
              case 6:
                action = new Rotate(dest, speed);
                break;
              case 7:
                action = new Rotate(dest, speed, (float)ps[6]);
                break;
              default:
              case 8:
                action = new Rotate(dest, speed, (float)ps[6], (bool)ps[7]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "6", ps.Length.ToString()));
          break;

        case "hyperspacein":
          if (ps.Length >= 5)
          {
            TV_3DVECTOR dest = new TV_3DVECTOR((float)ps[2], (float)ps[3], (float)ps[4]);
            action = new HyperspaceIn(dest);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "5", ps.Length.ToString()));
          break;

        case "hyperspaceout":
          action = new HyperspaceOut();
          break;

        case "attackactor":
          if (ps.Length >= 3)
          {
            tgtid = (int)ps[2];
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ValueS.ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = AttackActor.GetOrCreate(tgtid);
                break;
              case 4:
                action = AttackActor.GetOrCreate(tgtid, (float)ps[3]);
                break;
              case 5:
                action = AttackActor.GetOrCreate(tgtid, (float)ps[3], (float)ps[4]);
                break;
              case 6:
                action = AttackActor.GetOrCreate(tgtid, (float)ps[3], (float)ps[4], (bool)ps[5]);
                break;
              default:
              case 7:
                action = AttackActor.GetOrCreate(tgtid, (float)ps[3], (float)ps[4], (bool)ps[5], (float)ps[6]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "3", ps.Length.ToString()));
          break;

        case "followactor":
          if (ps.Length >= 3)
          {
            tgtid = (int)ps[2];
            //tgt = ActorFactory.Get(tgtid);
            //if (tgt == null)
            //  throw new Exception(string.Format("Target Actor (ID {1}) for action '{0}' not found!", ps[1].ValueS.ToLower(), ps[2].ToString().ToLower()));

            switch (ps.Length)
            {
              case 3:
                action = new FollowActor(tgtid);
                break;
              case 4:
                action = new FollowActor(tgtid, (float)ps[3]);
                break;
              default:
              case 5:
                action = new FollowActor(tgtid, (float)ps[3], (bool)ps[4]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "3", ps.Length.ToString()));
          break;

        case "avoidcollisionrotate":
          if (ps.Length >= 8)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR((float)ps[2], (float)ps[3], (float)ps[4]);
            TV_3DVECTOR rot = new TV_3DVECTOR((float)ps[5], (float)ps[6], (float)ps[7]);

            switch (ps.Length)
            {
              case 8:
                action = AvoidCollisionRotate.GetOrCreate(pos, rot);
                break;
              default:
              case 9:
                action = AvoidCollisionRotate.GetOrCreate(pos, rot, (float)ps[8]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[0]).ToLower(), "8", ps.Length.ToString()));
          break;

        case "setgamestateb":
          if (ps.Length >= 4)
          {
            action = new SetGameStateB((string)ps[2], (bool)ps[3]);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "4", ps.Length.ToString()));
          break;

      }
      return action;
    }
  }
}
