using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.Game.AI.Actions
{
  public class ActionInfo
  {
    public ActionInfo(string name)
    {
      Name = name;
    }

    // parameters
    public string Name { get; private set; }
    public bool Complete = false;
    public bool CanInterrupt = true;
    public ActionInfo NextAction = null;

    public override string ToString()
    {
      return "{0},{1}".F(Name
                       , Complete
                       );
    }

    public virtual void Process(Engine engine, ActorInfo owner)
    {
      Complete = true;
    }

    protected static bool CheckBounds(ActorInfo owner)
    {
      if (owner.TargetType.Has(TargetType.MUNITION))
        return true;

      float boundmult = 0.99f;
      bool outofbounds = owner.IsOutOfBounds(owner.Engine.GameScenarioManager.Scenario.State.MinAIBounds * boundmult, owner.Engine.GameScenarioManager.Scenario.State.MaxAIBounds * boundmult);
      if (outofbounds && owner.AI.CombatZone >= 0)
      {
        owner.AIDecision.OnOutOfBounds?.Invoke(owner);
        return false;
      }
      else
        if (owner.AI.CombatZone < 0 && !outofbounds)
          owner.AI.CombatZone = 0;

      return true;
    }

    protected static bool CheckImminentCollision(ActorInfo owner)
    {
      if (!owner.TypeInfo.AIData.CanCheckCollisionAhead)
        return false;

      if (!owner.Mask.Has(ComponentMask.CAN_BECOLLIDED))
        return false;

      float dist = owner.MoveData.Speed * Globals.ImminentCollisionFactor * (1 + (Math.Abs(owner.MoveData.XTurnAngle) + Math.Abs(owner.MoveData.YTurnAngle)) / 30f) ; // turning plays a role

      if (dist <= 0)
        return false;

      owner.CollisionData.ProspectiveCollisionScanDistance = dist;
      owner.CollisionData.IsTestingProspectiveCollision = true;
      return owner.CollisionData.IsInProspectiveCollision;
    }

    protected static void CreateAvoidAction(ActorInfo actor)
    {
      actor.AIDecision.OnImmenientCollision?.Invoke(actor);
    }

    public virtual void Return() { }

    public virtual void Reset()
    {
      Complete = false;
      CanInterrupt = true;
      NextAction = null;
    }

    public bool IsDisposed = false;
    public void Dispose()
    {
      if (IsDisposed)
        return;

      IsDisposed = true;
      NextAction = null;
      Complete = true;
      Return();
    }
  }
}
