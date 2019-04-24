using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class ForcedMove : ActionInfo
  {
    public ForcedMove(TV_3DVECTOR target_position, float speed, float close_enough_distance = -1, float expire_time = 999999) : base("ForcedMove")
    {
      Target_Position = target_position;
      Target_Speed = speed;
      CloseEnoughDistance = close_enough_distance;
      WaitTime = expire_time;
      CanInterrupt = false;
    }

    // parameters
    public TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    public float Target_Speed = 0;
    public float CloseEnoughDistance = -1;
    private float WaitTime = 0;
    private float ResumeTime = 0;

    public override string ToString()
    {
      return string.Format("{0},{1},{2},{3},{4},{5},{6}"
                          , Name
                          , Utilities.ToString(Target_Position)
                          , Target_Speed
                          , CloseEnoughDistance
                          , ResumeTime - Globals.Engine.Game.GameTime
                          , CanInterrupt
                          , Complete
                          );
    }

    public override void Process(ActorInfo owner)
    {
      if (owner.MovementInfo.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (ResumeTime == 0)
      {
        ResumeTime = Globals.Engine.Game.GameTime + WaitTime;
      }

      if (CloseEnoughDistance < 0)
        CloseEnoughDistance = owner.TypeInfo.Move_CloseEnough;

      AdjustRotation(owner, Target_Position);
      AdjustSpeed(owner, Target_Speed);

      float dist = Globals.Engine.TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Target_Position);
      Complete |= (dist <= CloseEnoughDistance);
      Complete |= (ResumeTime < Globals.Engine.Game.GameTime);

      //TV_3DVECTOR vNormal = new TV_3DVECTOR();
      //TV_3DVECTOR vImpact = new TV_3DVECTOR();
      //if (CheckImminentCollision(owner, owner.MovementInfo.Speed * 2.5f))
      //{
      //  ActionManager.QueueFirst(owner, new AvoidCollisionRotate(owner.ProspectiveCollisionImpact, owner.CollisionInfo.ProspectiveCollisionNormal));
      //}
    }
  }
}
