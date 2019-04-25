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

    public override void Process(ActorInfo owner)
    {
      if (owner.MovementInfo.MaxTurnRate == 0)
      {
        Complete = true;
        return;
      }

      if (ResumeTime == 0)
      {
        ResumeTime = Globals.Engine.Game.GameTime + WaitTime;
      }

      if (CheckBounds(owner))
      {
        if (!poschecked)
        {
          poschecked = true;
          if (owner.IsNearlyOutOfBounds())
            Target_Position = new TV_3DVECTOR();
          else
            Target_Position = owner.GetRelativePositionXYZ(1000, Globals.Engine.Random.Next(-500, 500), Globals.Engine.Random.Next(-500, 500));
        }

        float delta_angle = AdjustRotation(owner, Target_Position);
        float delta_speed = AdjustSpeed(owner, owner.MovementInfo.Speed);

        Complete |= (delta_angle <= CloseEnoughAngle && delta_angle >= -CloseEnoughAngle && delta_speed == 0);
        Complete |= (ResumeTime < Globals.Engine.Game.GameTime);
      }

      TV_3DVECTOR vNormal = new TV_3DVECTOR();
      TV_3DVECTOR vImpact = new TV_3DVECTOR();
      if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
      {
        owner.Owner.Engine.ActionManager.QueueFirst(owner.ID, new AvoidCollisionRotate(owner.CollisionInfo.ProspectiveCollisionImpact, owner.CollisionInfo.ProspectiveCollisionNormal));
      }
    }
  }
}
