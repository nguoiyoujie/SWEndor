using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  internal class Wait : ActionInfo
  {
    public Wait(float time = 5) : base("Wait")
    {
      WaitTime = time;
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
      actor.AIData.AdjustRotation(engine, actor);

      if (CheckImminentCollision(actor))
        CreateAvoidAction(actor);

      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
