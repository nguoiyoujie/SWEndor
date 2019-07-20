using MTV3D65;
using SWEndor.Actors;
using SWEndor.Primitives;

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
      return "{0},{1},{2},{3},{4},{5},{6}".F(Name
                                          , Utilities.ToString(Target_Position)
                                          , Target_Speed
                                          , CloseEnoughDistance
                                          , ResumeTime - Globals.Engine.Game.GameTime
                                          , CanInterrupt
                                          , Complete
                                          );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.MoveData.MaxSpeed == 0)
      {
        Complete = true;
        return;
      }

      if (ResumeTime == 0)
      {
        ResumeTime = engine.Game.GameTime + WaitTime;
      }

      if (CloseEnoughDistance < 0)
        CloseEnoughDistance = actor.TypeInfo.Move_CloseEnough;

      AdjustRotation(actor, Target_Position);
      AdjustSpeed(actor, Target_Speed);

      float dist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.GetGlobalPosition(), Target_Position);
      Complete |= (dist <= CloseEnoughDistance);
      Complete |= (ResumeTime < engine.Game.GameTime);
    }
  }
}
