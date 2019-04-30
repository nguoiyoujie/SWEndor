using MTV3D65;
using SWEndor.Actors;

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
      return string.Format("{0},{1},{2}"
                          , Name
                          , ResumeTime - Globals.Engine.Game.GameTime
                          , Complete
                          );
    }

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      if (actor.MoveComponent.MaxTurnRate == 0)
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
          if (actor.IsNearlyOutOfBounds())
            Target_Position = new TV_3DVECTOR();
          else
            Target_Position = actor.GetRelativePositionXYZ(1000, engine.Random.Next(-500, 500), engine.Random.Next(-500, 500));
        }

        float delta_angle = AdjustRotation(actor, Target_Position);
        float delta_speed = AdjustSpeed(actor, actor.MoveComponent.Speed);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
        Complete |= (ResumeTime < engine.Game.GameTime);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(actor, actor.MoveComponent.Speed * 2.5f))
      {
        engine.ActionManager.QueueFirst(actorID, new AvoidCollisionRotate(actor.CollisionInfo.ProspectiveCollisionImpact, actor.CollisionInfo.ProspectiveCollisionNormal));
      }
    }
  }
}
