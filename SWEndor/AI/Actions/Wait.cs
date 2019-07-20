using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;

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

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (ResumeTime == 0)
        ResumeTime = engine.Game.GameTime + WaitTime;

      AdjustRotation(actor, actor.GetRelativePositionXYZ(0, 0, 1000), false);
      if (CheckImminentCollision(actor))
      {
        CollisionSystem.CreateAvoidAction(engine, actor);
      }
      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
