using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.AI.Actions
{
  public class Evade : ActionInfo
  {
    public Evade(float time = 2.5f) : base("Evade")
    {
      WaitTime = time;
      CanInterrupt = false;
    }

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughAngle = 0.1f;
    private float WaitTime = 0;
    private float ResumeTime = 0;
    private bool poschecked = false;


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
      if (actor.MoveData.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (ResumeTime == 0)
      {
        ResumeTime = engine.Game.GameTime + WaitTime;
      }

      if (CheckBounds(actor))
      {
        if (!poschecked)
        {
          poschecked = true;
          if (actor.IsNearlyOutOfBounds(engine.GameScenarioManager))
            // TO-DO: use the center of the Bound volume or a designated center point instead of origin.
            actor.AIData.SetTarget(new TV_3DVECTOR());
          else
            actor.AIData.SetTarget(actor.GetRelativePositionXYZ(1000, engine.Random.Next(-500, 500), engine.Random.Next(-500, 500)));
        }

        actor.AIData.SetTargetSpeed(actor.MoveData.MaxSpeed);
        float delta_angle = actor.AIData.AdjustRotation(engine, actor, 20);
        float delta_speed = actor.AIData.AdjustSpeed(actor);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
        Complete |= (ResumeTime < engine.Game.GameTime);
      }

      if (CheckImminentCollision(actor))
        CreateAvoidAction(actor);
    }
  }
}
