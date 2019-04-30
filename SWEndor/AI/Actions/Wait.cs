﻿using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class Wait : ActionInfo
  {
    public Wait(float time = 5) : base("Wait")
    {
      WaitTime = time;
    }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , ResumeTime - Globals.Engine.Game.GameTime
                          , Complete
                          );
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      if (ResumeTime == 0)
        ResumeTime = engine.Game.GameTime + WaitTime;

      AdjustRotation(actor, actor.GetRelativePositionXYZ(0, 0, 1000), false);
      if (CheckImminentCollision(actor, actor.MoveComponent.Speed * 2.5f))
      {
        engine.ActionManager.QueueFirst(actorID, new AvoidCollisionRotate(actor.CollisionInfo.ProspectiveCollisionImpact, actor.CollisionInfo.ProspectiveCollisionNormal));
      }
      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
