using MTV3D65;
using SWEndor.Actors;
using SWEndor.AI;
using SWEndor.AI.Actions;
using Primrose.Primitives.ValueTypes;
using System;
using SWEndor.Primitives.Extensions;
using Primrose.Expressions;
using SWEndor.Core;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class AIFns
  {
    public static Val QueueFirst(IContext context, params Val[] ps)
    {
      int id = (int)ps[0];
      Engine e = ((Context)context).Engine;
      ActorInfo actor = e.ActorFactory.Get(id);
      if (e.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return Val.FALSE;

      actor.QueueFirst(action);
      return Val.TRUE;
    }

    public static Val QueueNext(IContext context, params Val[] ps)
    {
      int id = (int)ps[0];
      Engine e = ((Context)context).Engine;
      ActorInfo actor = e.ActorFactory.Get(id);
      if (e.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return Val.FALSE;

      actor.QueueNext(action);
      return Val.TRUE;
    }

    public static Val QueueLast(IContext context, params Val[] ps)
    {
      int id = (int)ps[0];
      Engine e = ((Context)context).Engine;
      ActorInfo actor = e.ActorFactory.Get(id);
      if (e.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      ActionInfo action = ParseAction(ps);
      if (action == null)
        return Val.FALSE;

      actor.QueueLast(action);
      return Val.TRUE;
    }

    public static Val UnlockOne(IContext context, int actorID)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo actor = e.ActorFactory.Get(actorID);
      if (e.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.UnlockOne();
      return Val.TRUE;
    }

    public static Val ClearQueue(IContext context, int actorID)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo actor = e.ActorFactory.Get(actorID);
      if (e.GameScenarioManager.Scenario == null || actor == null)
        return Val.FALSE;

      actor.ClearQueue();
      return Val.TRUE;
    }

    public static Val ForceClearQueue(IContext context, int actorID)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo actor = e.ActorFactory.Get(actorID);
      if (e.GameScenarioManager.Scenario == null || actor == null)
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
          action = Idle.GetOrCreate();
          break;

        case "hunt":
          if (ps.Length <= 2)
            action = Hunt.GetOrCreate();
          else
          {
            Models.TargetType tgttype = (Models.TargetType)Enum.Parse(typeof(Models.TargetType), (string)ps[2]);
            action = Hunt.GetOrCreate(tgttype);
          }
          break;

        case "selfdestruct":
          action = SelfDestruct.GetOrCreate();
          break;

        case "delete":
          action = Delete.GetOrCreate();
          break;

        case "lock":
          action = Lock.GetOrCreate();
          break;

        case "wait":
          if (ps.Length <= 2)
            action = Wait.GetOrCreate();
          else
            action = Wait.GetOrCreate((float)ps[2]);
          break;

        case "evade":
          if (ps.Length <= 2)
            action = Evade.GetOrCreate();
          else
            action = Evade.GetOrCreate((float)ps[2]);
          break;

        case "move":
          if (ps.Length >= 4)
          {
            TV_3DVECTOR dest = ((float3)ps[2]).ToVec3();
            float speed = (float)ps[3];

            switch (ps.Length)
            {
              case 4:
                action = Move.GetOrCreate(dest, speed);
                break;
              case 5:
                action = Move.GetOrCreate(dest, speed, (float)ps[4]);
                break;
              default:
              case 6:
                action = Move.GetOrCreate(dest, speed, (float)ps[4], (bool)ps[5]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "4", ps.Length.ToString()));
          break;

        case "forcedmove":
          if (ps.Length >= 4)
          {
            TV_3DVECTOR dest = ((float3)ps[2]).ToVec3();
            float speed = (float)ps[3];

            switch (ps.Length)
            {
              case 4:
                action = ForcedMove.GetOrCreate(dest, speed);
                break;
              case 5:
                action = ForcedMove.GetOrCreate(dest, speed, (float)ps[4]);
                break;
              default:
              case 6:
                action = ForcedMove.GetOrCreate(dest, speed, (float)ps[4], (float)ps[5]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "4", ps.Length.ToString()));
          break;

        case "rotate":
          if (ps.Length >= 4)
          {
            TV_3DVECTOR dest = ((float3)ps[2]).ToVec3();
            float speed = (float)ps[3];

            switch (ps.Length)
            {
              case 4:
                action = Rotate.GetOrCreate(dest, speed);
                break;
              case 5:
                action = Rotate.GetOrCreate(dest, speed, (float)ps[4]);
                break;
              default:
              case 6:
                action = Rotate.GetOrCreate(dest, speed, (float)ps[4], (bool)ps[5]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "4", ps.Length.ToString()));
          break;

        case "hyperspacein":
          if (ps.Length >= 3)
          {
            TV_3DVECTOR dest = ((float3)ps[2]).ToVec3();
            action = HyperspaceIn.GetOrCreate(dest);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "3", ps.Length.ToString()));
          break;

        case "hyperspaceout":
          action = HyperspaceOut.GetOrCreate();
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
                action = FollowActor.GetOrCreate(tgtid);
                break;
              case 4:
                action = FollowActor.GetOrCreate(tgtid, (float)ps[3]);
                break;
              default:
              case 5:
                action = FollowActor.GetOrCreate(tgtid, (float)ps[3], (bool)ps[4]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "3", ps.Length.ToString()));
          break;

          /*
        case "avoidcollisionrotate":
          if (ps.Length >= 4)
          {
            TV_3DVECTOR pos = ((float3)ps[2]).ToVec3();
            TV_3DVECTOR rot = ((float3)ps[3]).ToVec3();

            switch (ps.Length)
            {
              case 4:
                action = AvoidCollisionRotate.GetOrCreate(pos, rot);
                break;
              default:
              case 5:
                action = AvoidCollisionRotate.GetOrCreate(pos, rot, (float)ps[4]);
                break;
            }
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[0]).ToLower(), "4", ps.Length.ToString()));
          break;
          */

        case "setgamestateb":
          if (ps.Length >= 4)
          {
            action = SetGameStateB.GetOrCreate((string)ps[2], (bool)ps[3]);
          }
          else
            throw new Exception(string.Format("Insufficient parameters for action '{0}': required {1}, has {2}", ((string)ps[1]).ToLower(), "4", ps.Length.ToString()));
          break;

      }
      return action;
    }
  }
}
