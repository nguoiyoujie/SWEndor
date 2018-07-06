using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.Actions
{
  public class Hunt : ActionInfo
  {
    public Hunt() : base("Hunt")
    {
    }

    public override void Process(ActorInfo owner)
    {
      // excludes
      /*
      if (!(owner.TypeInfo is FighterGroup 
         || owner.TypeInfo is TIEGroup 
         || owner.TypeInfo is AddOnGroup 
         || owner.TypeInfo is ProjectileGroup))
         //|| !(!owner.IsPlayer() || PlayerInfo.Instance().PlayerAIEnabled))
      {
        return;
      }
      */

      //target
      //float currdist = (owner.TypeInfo is AddOnGroup) ? owner.Attack_DistanceDelta : 7500;
      //float currdist  = 7500;
      ActorInfo currtarget = null;
      List<ActorInfo> closetargets = new List<ActorInfo>();
      List<ActorInfo> targets = new List<ActorInfo>();
      foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
      {
        if (a != null
          && owner != a
          && a.CreationState == CreationState.ACTIVE
          && a.ActorState != ActorState.DYING
          && a.ActorState != ActorState.DEAD
          && a.IsCombatObject
          && !a.IsOutOfBounds(GameScenarioManager.Instance().MinAIBounds, GameScenarioManager.Instance().MaxAIBounds)
          //&& owner.CanTarget(a)
          && !owner.Faction.IsAlliedWith(a.Faction) // enemy
          )
        {
          if (owner.MaxSpeed == 0)
          {
            WeaponInfo weap = null;
            int dummy = 0;
            float dist = ActorDistanceInfo.GetDistance(owner, a, owner.GetWeaponRange());
            owner.SelectWeapon(a, 0, dist, out weap, out dummy);

            if (weap != null)//dist < currdist)
            {
              for (int i = 0; i < a.HuntWeight; i++)
                targets.Add(a);
            }
          }
          else
          {
            for (int i = 0; i < a.HuntWeight; i++)
              targets.Add(a);
          }
        }
      }

      if (targets.Count > 0)
      {
        int i = Engine.Instance().Random.Next(0, targets.Count);
        currtarget = targets[i];
      }

      if (currtarget != null)
      {
        ActionManager.QueueLast(owner, new AttackActor(currtarget));
      }
      else
      {
        ActionManager.QueueLast(owner, new Wait(2.5f));
      }

      Complete = true;
    }
  }
}
