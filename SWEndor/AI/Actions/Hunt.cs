using SWEndor.Actors;
using SWEndor.Scenarios;
using SWEndor.Weapons;
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

    public override void Process(ActorInfo owner)
    {
      ActorInfo currtarget = null;
      List<ActorInfo> targets = new List<ActorInfo>();
      int weight = 0;

      foreach (ActorInfo a in ActorFactory.Instance().GetHoldingList())
      {
        if (a != null
          && owner != a
          && a.CreationState == CreationState.ACTIVE
          && a.ActorState != ActorState.DYING
          && a.ActorState != ActorState.DEAD
          && a.CombatInfo.IsCombatObject
          && (a.TypeInfo.TargetType & m_TargetType) != 0
          && !a.IsOutOfBounds(GameScenarioManager.Instance().MinAIBounds, GameScenarioManager.Instance().MaxAIBounds)
          && !owner.Faction.IsAlliedWith(a.Faction) // enemy
          )
        {
          if (owner.MovementInfo.MaxSpeed == 0) // stationary, can only target those in range
          {
            WeaponInfo weap = null;
            int dummy = 0;
            float dist = ActorDistanceInfo.GetDistance(owner, a, owner.GetWeaponRange());
            owner.SelectWeapon(a, 0, dist, out weap, out dummy);

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

      if (targets.Count > 0)
      {
        int w = Engine.Instance().Random.Next(0, weight);
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
        ActionManager.QueueLast(owner, new AttackActor(currtarget));
      }
      else
      {
        ActionManager.QueueLast(owner, new Wait(1));
      }

      Complete = true;
    }
  }
}
