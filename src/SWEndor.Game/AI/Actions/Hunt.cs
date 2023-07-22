using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Factories;
using SWEndor.Game.Weapons;
using Primrose.Primitives.Cache;

namespace SWEndor.Game.AI.Actions
{
  internal class Hunt : ActionInfo
  {
    private static readonly ObjectPool<Hunt> _pool;
    static Hunt() { _pool = ObjectPool<Hunt>.CreateStaticPool(() => { return new Hunt(); }, (a) => { a.Reset(); }); }

    private Hunt() : base("Hunt") { }

    private TargetType m_TargetType;
    private TargetExclusionState m_ExcludeTargets;
    private readonly ActorWeight[] targets = new ActorWeight[124]; // assumed to sufficient selection choices

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

    public static Hunt GetOrCreate(TargetType targetType = TargetType.ANY, TargetExclusionState excludeTargets = TargetExclusionState.NONE)
    {
      Hunt h = _pool.GetNew();

      h.m_TargetType = targetType;
      h.m_ExcludeTargets = excludeTargets;
      h.IsDisposed = false;
      return h;
    }

    public override void Reset()
    {
      base.Reset();
    }

    public override void Return()
    {
      base.Return();
      _pool.Return(this);
    }

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , Enum<TargetType>.GetName(m_TargetType)
        , Enum<TargetExclusionState>.GetName(m_ExcludeTargets)
        , Complete.ToString()
      });
    }

    private static bool GetTargetsStationary(Engine e, ActorInfo a, ActorInfo c, ActorWeight[] t, TargetType mt, TargetExclusionState ext)
    {
      if (t[0].ID >= t.Length - 1)
        return false;

      if (c.TypeInfo.ID == "INT_JAM")
      {

      }

      if (isValidTarget(e, a, c, mt, ext))
      {
        // stationary, can only target those in range
        float dist = DistanceModel.GetDistance(e, c, a, c.WeaponDefinitions.GetWeaponRange());
        c.WeaponDefinitions.AISelectWeapon(e, c, a, 0, dist, out WeaponShotInfo w);

        if (!w.IsNull)
        {
          t[0].ID++;
          t[0].Weight += a.AI.HuntWeight;
          t[t[0].ID] = new ActorWeight(a.ID, a.AI.HuntWeight);
        }
      }
      return true;
    }

    private static bool GetTargetsMobile(Engine e, ActorInfo a, ActorInfo c, ActorWeight[] t, TargetType mt, TargetExclusionState ext)
    {
      if (t[0].ID >= t.Length - 1)
        return false;

      if (isValidTarget(e, a, c, mt, ext))
      {
        t[0].ID++;
        t[0].Weight += a.AI.HuntWeight;
        t[t[0].ID] = new ActorWeight(a.ID, a.AI.HuntWeight);
      }
      return true;
    }

    private static bool isValidTarget(Engine e, ActorInfo a, ActorInfo c, TargetType mt, TargetExclusionState ext)
    {
      return a != null
           && a.AI.HuntWeight > 0
           && c.ID != a.ID
           && a.Active
           && !a.IsDyingOrDead
           && a.InCombat
           && c.WeaponDefinitions.CanAITarget(a) 
           // specified exclusion
           && mt.Has(a.TargetType)
           && (ext == TargetExclusionState.NONE || !a.TargetState.Intersects(ext))
           && !a.IsOutOfBounds(e.GameScenarioManager.Scenario.State.MinAIBounds, e.GameScenarioManager.Scenario.State.MaxAIBounds)
           && !c.IsAlliedWith(a);
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      ActorInfo currtarget = null;
      targets[0] = new ActorWeight(0, 0);

      if (actor.MoveData.MaxSpeed == 0)
      {
        engine.ActorFactory.DoUntil(GetTargetsStationary, actor, targets, m_TargetType, m_ExcludeTargets);
      }
      else
      {
        engine.ActorFactory.DoUntil(GetTargetsMobile, actor, targets, m_TargetType, m_ExcludeTargets);
      }


      if (targets[0].ID >= 1)
      {
        int w = engine.Random.Next(0, targets[0].Weight);
        for (int i = 1; i <= targets[0].ID; i++)
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
        actor.QueueLast(Wait.GetOrCreate(1));

      Complete = true;
    }
  }
}
