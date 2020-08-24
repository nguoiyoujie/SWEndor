using MTV3D65;
using SWEndor.Game.AI;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Models;

namespace SWEndor.Game.Actors.Models
{
  internal struct AIDecision
  {
    public HitEvent OnAttacked;
    public ActorEvent OnOutOfBounds;
    public ActorEvent OnImmenientCollision;
    public ActorEvent OnIdleAction;

    public void Init(ref ActorTypes.Components.AIData data)
    {
      OnOutOfBounds = Default_OnOutOfBounds;
      OnImmenientCollision = Default_OnImmenientCollision;
      OnIdleAction = Default_OnIdleAction;
      OnAttacked = OnAttacked_AddThreat;

      if ((data.TargetType.Has(TargetType.FIGHTER)))
        OnAttacked += Fighter_OnAttacked;
    }

    public void Reset()
    {
      OnAttacked = null;
      OnOutOfBounds = null;
      OnImmenientCollision = null;
      OnIdleAction = null;
    }

    internal void Fighter_OnAttacked(ActorInfo owner, ActorInfo attacker)
    {
      if (
        (owner.AI.Target.GetTargetActor(owner.Engine)?.TopParent == attacker.TopParent)   // if the attacker is the same, stay on target and attempt evade instead
        || (!owner.AI.CanRetaliate)                                                       // if retaliation is not allowed, attempt evade instead
        || (!owner.CanInterruptCurrentAction)                                             // if current action cannot be interrupted, attempt evade instead
        ) 
      {
        DoEvade(owner);
        return;
      }

      if (!owner.Squad.IsNull && owner.Squad.Mission == null)
      {
        if (!attacker.Squad.IsNull)
        {
          foreach (ActorInfo a in owner.Squad.Members)
          {
            if (a.AI.CanRetaliate && a.CanInterruptCurrentAction)
            {
              ActorInfo b = attacker.Squad.GetMemberRandom(owner.Engine);  //attacker.Squad.MembersCopy.Random(owner.Engine.Random);
              if (b != null)
              {
                a.ClearQueue();
                a.QueueLast(AttackActor.GetOrCreate(b.ID));
              }
            }
          }
        }
        else
        {
          foreach (ActorInfo a in owner.Squad.Members)
          {
            if (a.AI.CanRetaliate && a.CanInterruptCurrentAction)
            {
              a.ClearQueue();
              a.QueueLast(AttackActor.GetOrCreate(attacker.ID));
            }
          }
        }
      }
      else
      {
        owner.ClearQueue();
        owner.QueueLast(AttackActor.GetOrCreate(attacker.ID));
      }
    }

    private void DoEvade(ActorInfo owner)
    {
      if (owner.AI.CanEvade && !(owner.CurrentAction is Evade))
      {
        owner.QueueFirst(Evade.GetOrCreate());
      }
    }

    internal void OnAttacked_AddThreat(ActorInfo owner, ActorInfo attacker)
    {
      if (!owner.Squad.IsNull)
      {
        if (owner.Squad.Leader == owner)
          owner.Squad.AddThreat(attacker, true);
        else
          owner.Squad.AddThreat(attacker);
      }
    }

    internal void Default_OnOutOfBounds(ActorInfo owner)
    {
      float x = owner.Engine.Random.Next((int)(owner.Engine.GameScenarioManager.Scenario.State.MinAIBounds.x * 0.65f), (int)(owner.Engine.GameScenarioManager.Scenario.State.MaxAIBounds.x * 0.65f));
      float y = owner.Engine.Random.Next(-200, 200);
      float z = owner.Engine.Random.Next((int)(owner.Engine.GameScenarioManager.Scenario.State.MinAIBounds.z * 0.65f), (int)(owner.Engine.GameScenarioManager.Scenario.State.MaxAIBounds.z * 0.65f));

      if (owner.CurrentAction is Move)
        owner.CurrentAction.Complete = true;
      owner.QueueFirst(ForcedMove.GetOrCreate(new TV_3DVECTOR(x, y, z), owner.MoveData.MaxSpeed, -1, 360 / (owner.MoveData.MaxTurnRate + 72)));
    }

    internal void Default_OnImmenientCollision(ActorInfo owner)
    {
      if (owner.CurrentAction is AvoidCollisionWait)
      {
        owner.QueueNext(AvoidCollisionWait.GetOrCreate(0.5f)); // 2nd action
        owner.QueueNext(AvoidCollisionRotate.GetOrCreate(owner.CollisionData.ProspectiveCollision.Impact, owner.CollisionData.ProspectiveCollision.Normal));
      }
      else
      {
        owner.QueueFirst(AvoidCollisionWait.GetOrCreate(0.5f)); // 2nd action
        owner.QueueFirst(AvoidCollisionRotate.GetOrCreate(owner.CollisionData.ProspectiveCollision.Impact, owner.CollisionData.ProspectiveCollision.Normal));
      }
    }

    internal void Default_OnIdleAction(ActorInfo owner)
    {
      owner.QueueLast(Hunt.GetOrCreate());
    }
  }
}
