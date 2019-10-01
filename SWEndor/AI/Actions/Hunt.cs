using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;
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
      return string.Join(",", new string[]
      {
          Name
        , m_TargetType.ToString()
        , Complete.ToString()
      });
    }

    Action<Engine, ActorInfo, ActorInfo, List<ActorInfo>, TargetType> getTargets = new Action<Engine, ActorInfo, ActorInfo, List<ActorInfo>, TargetType>(
       (e, a, c, t, mt) =>
       {
         if (a != null
           && c.ID != a.ID
           && a.Active
           && !a.IsDyingOrDead
           && a.CombatData.IsCombatObject
           && (a.TypeInfo.AIData.TargetType & mt) != 0
           && !a.IsOutOfBounds(e.GameScenarioManager.MinAIBounds, e.GameScenarioManager.MaxAIBounds)
           && !c.Faction.IsAlliedWith(a.Faction) // enemy
           )
         {
           if (c.MoveData.MaxSpeed == 0) // stationary, can only target those in range
             {
             WeaponShotInfo w;
             float dist = ActorDistanceInfo.GetDistance(e, c, a, c.WeaponDefinitions.GetWeaponRange());
             c.WeaponDefinitions.SelectWeapon(e, c, a, 0, dist, out w);

             if (!w.IsNull)
             {
               for (int i = a.HuntWeight; i > 0; i--)
                 t.Add(a);
             }
           }
           else
           {
             for (int i = a.HuntWeight; i > 0; i--)
               t.Add(a);
           }
         }
       }
     );

    List<ActorInfo> targets = new List<ActorInfo>(50);
    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo currtarget = null;
      targets.Clear();

      engine.ActorFactory.DoEach(getTargets, actor, targets, m_TargetType);

      if (targets.Count > 0)
      {
        int w = engine.Random.Next(0, targets.Count);
        currtarget = targets[w];
      }

      if (currtarget != null)
        actor.QueueLast(new AttackActor(currtarget.ID));
      else
        actor.QueueLast(new Wait(1));

      Complete = true;
    }
  }
}
