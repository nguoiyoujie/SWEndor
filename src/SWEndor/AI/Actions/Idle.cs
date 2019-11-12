﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Idle : ActionInfo
  {
    public Idle() : base("Idle")
    {
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (CheckBounds(actor))
      {
        //AdjustRotation(engine, actor, actor.GetGlobalPosition());
        actor.AI.SetTargetSpeed(actor.MoveData.MinSpeed);
        actor.AI.AdjustSpeed(actor);

        if (NextAction == null)
          actor.QueueLast(Hunt.GetOrCreate());

        Complete = true;

        TV_3DVECTOR vNormal = new TV_3DVECTOR();
        TV_3DVECTOR vImpact = new TV_3DVECTOR();
        if (CheckImminentCollision(actor))
        {
          CreateAvoidAction(actor);
        }
      }
    }
  }
}