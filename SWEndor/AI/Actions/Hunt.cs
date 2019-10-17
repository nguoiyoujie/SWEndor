using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives;
using SWEndor.Primitives.Factories;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;

namespace SWEndor.AI.Actions
{
  public class Hunt : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<Hunt> _pool = new ObjectPool<Hunt>(() => { return new Hunt(); }, (a) => { a.Reset(); });

    private Hunt() : base("Hunt") { }

    private TargetType m_TargetType;
    private List<ActorInfo> targets = new List<ActorInfo>(50);

    public static Hunt GetOrCreate(TargetType targetType = TargetType.ANY)
    {
      Hunt h = _pool.GetNew();
      _count++;
      h.m_TargetType = targetType;
      h.IsDisposed = false;
      return h;
    }

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , m_TargetType.GetEnumName()
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
           && a.InCombat
           && a.TypeInfo.AIData.TargetType.Contains(mt)
           && !a.IsOutOfBounds(e.GameScenarioManager.MinAIBounds, e.GameScenarioManager.MaxAIBounds)
           && !c.Faction.IsAlliedWith(a.Faction) // enemy
           )
         {
           if (c.MoveData.MaxSpeed == 0) // stationary, can only target those in range
           {
             WeaponShotInfo w;
             float dist = DistanceModel.GetDistance(e, c, a, c.WeaponDefinitions.GetWeaponRange());
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
        actor.QueueLast(AttackActor.GetOrCreate(currtarget.ID));
      else
        actor.QueueLast(new Wait(1));

      Complete = true;
    }

    public override void Reset()
    {
      base.Reset();
      m_TargetType = TargetType.ANY;
      targets.Clear();
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }
  }
}
