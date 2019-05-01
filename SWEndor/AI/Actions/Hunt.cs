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

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      ActorInfo currtarget = null;
      List<ActorInfo> targets = new List<ActorInfo>();
      int weight = 0;

      Action<Engine, int> action = new Action<Engine, int>(
         (_, aID) =>
         {
           ActorInfo a = engine.ActorFactory.Get(aID);
           if (a != null
             && actor != a
             && a.CreationState == CreationState.ACTIVE
             && !a.ActorState.IsDyingOrDead()
             && a.CombatInfo.IsCombatObject
             && (a.TypeInfo.TargetType & m_TargetType) != 0
             && !a.IsOutOfBounds(engine.GameScenarioManager.MinAIBounds, engine.GameScenarioManager.MaxAIBounds)
             && !actor.Faction.IsAlliedWith(a.Faction) // enemy
             )
           {
             if (actor.MoveComponent.MaxSpeed == 0) // stationary, can only target those in range
             {
               WeaponInfo weap = null;
               int dummy = 0;
               float dist = ActorDistanceInfo.GetDistance(actorID, aID, actor.WeaponSystemInfo.GetWeaponRange());
               actor.WeaponSystemInfo.SelectWeapon(a.ID, 0, dist, out weap, out dummy);

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
        int w = Globals.Engine.Random.Next(0, weight);
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
        engine.ActionManager.QueueLast(actorID, new AttackActor(currtarget.ID));
      }
      else
      {
        engine.ActionManager.QueueLast(actorID, new Wait(1));
      }

      Complete = true;
    }
  }
}
