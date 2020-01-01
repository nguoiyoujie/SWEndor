﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.AI.Actions
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
      if (owner.TypeInfo.AIData.TargetType.Intersects(TargetType.MUNITION))
        return true;

      float boundmult = 0.99f;
      bool outofbounds = owner.IsOutOfBounds(owner.Engine.GameScenarioManager.Scenario.State.MinAIBounds * boundmult, owner.Engine.GameScenarioManager.Scenario.State.MaxAIBounds * boundmult);
      if (outofbounds && owner.AI.EnteredCombatZone)
      {
        float x = owner.Engine.Random.Next((int)(owner.Engine.GameScenarioManager.Scenario.State.MinAIBounds.x * 0.65f), (int)(owner.Engine.GameScenarioManager.Scenario.State.MaxAIBounds.x * 0.65f));
        float y = owner.Engine.Random.Next(-200, 200);
        float z = owner.Engine.Random.Next((int)(owner.Engine.GameScenarioManager.Scenario.State.MinAIBounds.z * 0.65f), (int)(owner.Engine.GameScenarioManager.Scenario.State.MaxAIBounds.z * 0.65f));

        if (owner.CurrentAction is Move)
          owner.CurrentAction.Complete = true;
        owner.QueueFirst(ForcedMove.GetOrCreate(new TV_3DVECTOR(x, y, z), owner.MoveData.MaxSpeed, -1, 360 / (owner.MoveData.MaxTurnRate + 72)));
        return false;
      }
      else
        if (!owner.AI.EnteredCombatZone && !outofbounds)
          owner.AI.EnteredCombatZone = true;

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
      if (actor.CurrentAction is AvoidCollisionWait)
      {
        actor.QueueNext(AvoidCollisionWait.GetOrCreate(0.5f)); // 2nd action
        actor.QueueNext(AvoidCollisionRotate.GetOrCreate(actor.CollisionData.ProspectiveCollision.Impact, actor.CollisionData.ProspectiveCollision.Normal));
      }
      else
      {
        actor.QueueFirst(AvoidCollisionWait.GetOrCreate(0.5f)); // 2nd action
        actor.QueueFirst(AvoidCollisionRotate.GetOrCreate(actor.CollisionData.ProspectiveCollision.Impact, actor.CollisionData.ProspectiveCollision.Normal));
      }
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