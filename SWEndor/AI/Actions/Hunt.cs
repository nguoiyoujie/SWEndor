using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives;
using SWEndor.Primitives.Factories;
using SWEndor.Weapons;
using System;
namespace SWEndor.AI.Actions
{
  public class Hunt : ActionInfo
  {
    internal static int _count = 0;
    internal static ObjectPool<Hunt> _pool = new ObjectPool<Hunt>(() => { return new Hunt(); }, (a) => { a.Reset(); });

    private Hunt() : base("Hunt") { }

    private TargetType m_TargetType;
    private ActorWeight[] targets = new ActorWeight[124]; // assumed to sufficient selection choices

    public struct ActorWeight
    {
      public int ID;
      public int Weight;

      public ActorWeight(int id, int weight)
      {
        ID = id;
        Weight = weight;
      }
    }

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

    Func<Engine, ActorInfo, ActorInfo, ActorWeight[], TargetType, bool> getTargetsStationary = new Func<Engine, ActorInfo, ActorInfo, ActorWeight[], TargetType, bool>(
       (e, a, c, t, mt) =>
       {
         if (t[0].ID >= t.Length - 1)
           return false;

         if (a != null
           && a.HuntWeight > 0
           && c.ID != a.ID
           && a.Active
           && !a.IsDyingOrDead
           && a.InCombat
           && a.TypeInfo.AIData.TargetType.Contains(mt)
           && !a.IsOutOfBounds(e.GameScenarioManager.MinAIBounds, e.GameScenarioManager.MaxAIBounds)
           && !c.IsAlliedWith(a) // enemy
           )
         {
           // stationary, can only target those in range
           WeaponShotInfo w;
           float dist = DistanceModel.GetDistance(e, c, a, c.WeaponDefinitions.GetWeaponRange());
           c.WeaponDefinitions.SelectWeapon(e, c, a, 0, dist, out w);

           if (!w.IsNull)
           {
             t[0].ID++;
             t[0].Weight += a.HuntWeight;
             t[t[0].ID] = new ActorWeight(a.ID, a.HuntWeight);
           }
         }
         return true;
       }
     );

    Func<Engine, ActorInfo, ActorInfo, ActorWeight[], TargetType, bool> getTargets = new Func<Engine, ActorInfo, ActorInfo, ActorWeight[], TargetType, bool>(
       (e, a, c, t, mt) =>
       {
         if (t[0].ID >= t.Length - 1)
           return false;

         if (a != null
           && a.HuntWeight > 0
           && c.ID != a.ID
           && a.Active
           && !a.IsDyingOrDead
           && a.InCombat
           && a.TypeInfo.AIData.TargetType.Contains(mt)
           && !a.IsOutOfBounds(e.GameScenarioManager.MinAIBounds, e.GameScenarioManager.MaxAIBounds)
           && !c.IsAlliedWith(a) // enemy
           )
         {
           t[0].ID++;
           t[0].Weight += a.HuntWeight;
           t[t[0].ID] = new ActorWeight(a.ID, a.HuntWeight);
         }
         return true;
       }
     );

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo currtarget = null;
      targets[0] = new ActorWeight(0, 0);

      engine.ActorFactory.DoUntil(
        (actor.MoveData.MaxSpeed == 0) ? getTargetsStationary : getTargets
        , actor
        , targets
        , m_TargetType);

      if (targets[0].ID >= 1)
      {
        int w = engine.Random.Next(0, targets[0].Weight);
        for (int i = 1; i < targets[0].ID; i++)
        {
          w -= targets[i].Weight;
          if (w <= 0)
          {
            currtarget = engine.ActorFactory.Get(targets[i].ID);
            break;
          }
        }
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
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
      _count--;
    }
  }
}
