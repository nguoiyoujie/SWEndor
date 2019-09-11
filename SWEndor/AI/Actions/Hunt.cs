using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Weapons;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SWEndor.AI.Actions
{
  public class Hunt : ActionInfo
  {
    public Hunt(TargetType targetType = TargetType.ANY) : base("Hunt")
    {
      m_TargetType = targetType;
    }

    private TargetType m_TargetType;

    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , m_TargetType
                          , Complete
                          );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo currtarget = null;
      //ConcurrentQueue<ActorInfo> targets = new ConcurrentQueue<ActorInfo>();
      Queue<ActorInfo> targets = new Queue<ActorInfo>();
      int weight = 0;

      Action<Engine, ActorInfo> action = new Action<Engine, ActorInfo>(
         (_, a) =>
         {
           if (a != null
             && actor != a
             && a.Active
             && !a.IsDyingOrDead
             && engine.ActorDataSet.CombatData[a.dataID].IsCombatObject
             && (a.TypeInfo.AIData.TargetType & m_TargetType) != 0
             && !a.IsOutOfBounds(engine.GameScenarioManager.MinAIBounds, engine.GameScenarioManager.MaxAIBounds)
             && !actor.Faction.IsAlliedWith(a.Faction) // enemy
             )
           {
             if (actor.MoveData.MaxSpeed == 0) // stationary, can only target those in range
             {
               WeaponShotInfo w;
               float dist = ActorDistanceInfo.GetDistance(actor, a, actor.WeaponSystemInfo.GetWeaponRange());
               actor.WeaponSystemInfo.SelectWeapon(engine, actor, a, 0, dist, out w);

               if (!w.IsNull)
               {
                 targets.Enqueue(a);
                 weight += a.HuntWeight;
               }
             }
             else
             {
               targets.Enqueue(a);
               weight += a.HuntWeight;
             }
           }
         }
       );

      engine.ActorFactory.DoEach(action);

      if (targets.Count > 0)
      {
        int w = engine.Random.Next(0, weight);
        ActorInfo tgt = null;
        //while (targets.TryDequeue(out tgt))
        while ((tgt = targets.Dequeue()) != null)
        {
          w -= tgt.HuntWeight;
          currtarget = tgt;
          if (w < 0)
            break;
        }
      }

      if (currtarget != null)
      {
        actor.QueueLast(new AttackActor(currtarget.ID));
      }
      else
      {
        actor.QueueLast(new Wait(1));
      }

      Complete = true;
    }
  }
}
