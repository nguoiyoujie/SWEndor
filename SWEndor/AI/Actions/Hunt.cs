using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Weapons;
using System;
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
      List<ActorInfo> targets = new List<ActorInfo>();
      int weight = 0;

      Action<Engine, ActorInfo> action = new Action<Engine, ActorInfo>(
         (_, a) =>
         {
           if (a != null
             && actor != a
             && a.CreationState == CreationState.ACTIVE
             && !a.ActorState.IsDyingOrDead()
             && engine.ActorDataSet.CombatData[a.dataID].IsCombatObject
             && (a.TypeInfo.TargetType & m_TargetType) != 0
             && !a.IsOutOfBounds(engine.GameScenarioManager.MinAIBounds, engine.GameScenarioManager.MaxAIBounds)
             && !actor.Faction.IsAlliedWith(a.Faction) // enemy
             )
           {
             if (actor.MoveData.MaxSpeed == 0) // stationary, can only target those in range
             {
               WeaponInfo weap = null;
               int dummy = 0;
               float dist = ActorDistanceInfo.GetDistance(actor, a, actor.WeaponSystemInfo.GetWeaponRange());
               actor.WeaponSystemInfo.SelectWeapon(a, 0, dist, out weap, out dummy);

               if (weap != null)
               {
                 targets.Add(a);
                 weight += a.HuntWeight;
               }
             }
             else
             {
               targets.Add(a);
               weight += a.HuntWeight;
             }
           }
         }
       );

      engine.ActorFactory.DoEach(action);

      if (targets.Count > 0)
      {
        int w = engine.Random.Next(0, weight);
        for (int i = 0; i < targets.Count; i++)
        {
          w -= targets[i].HuntWeight;
          currtarget = targets[i];
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
