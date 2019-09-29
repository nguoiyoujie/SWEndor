using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  public class AvoidCollisionWait : ActionInfo
  {
    public AvoidCollisionWait(float time = 5) : base("AvoidCollisionWait")
    {
      WaitTime = time;
      CanInterrupt = false;
    }

    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Join(",", new string[]
      {
          Name
        , (ResumeTime - Globals.Engine.Game.GameTime).ToString()
        , Complete.ToString()
      });
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (ResumeTime == 0)
        ResumeTime = engine.Game.GameTime + WaitTime;

      actor.AIData.SetTarget(actor.GetRelativePositionXYZ(0, 0, 1000));
      actor.AIData.AdjustRotation(engine, actor, 0.5f);

      actor.AIData.SetTargetSpeed(actor.MoveData.MaxSpeed);
      actor.AIData.AdjustSpeed(actor);

      if (CheckImminentCollision(actor))
      {
        CreateAvoidAction(actor);
        Complete = true;
      }

      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
