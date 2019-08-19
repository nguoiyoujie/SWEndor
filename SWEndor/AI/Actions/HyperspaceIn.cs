using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class HyperspaceIn : ActionInfo 
  {
    public HyperspaceIn(TV_3DVECTOR target_position) : base("HyperspaceIn")
    {
      Target_Position = target_position;
      CanInterrupt = false;
    }

    // parameters
    private TV_3DVECTOR Target_Position = new TV_3DVECTOR();
    private static float Max_Speed = 25000;
    private static float SpeedDistanceFactor = 2.5f;
    private static float CloseEnoughDistance = 500;
    private ActorState prevState = ActorState.NORMAL;
    private float prevdist = 9999999;

    public override string ToString()
    {
      return string.Format("{0},{1},{2}"
                          , Name
                          , Utilities.ToString(Target_Position)
                          , Complete
                          );
    }

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.ActorState != ActorState.HYPERSPACE && !Complete)
      {
        prevState = actor.ActorState;
        actor.ActorState = ActorState.HYPERSPACE;
        actor.LookAtPoint(Target_Position);
      }

      //Complete = (owner.ActorState != ActorState.HYPERSPACE);
    }

    public void ApplyMove(ActorInfo owner)
    {
      float dist = owner.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Target_Position);

      if (dist <= CloseEnoughDistance || prevdist < dist)
      {
        owner.ActorState = prevState;
        owner.MoveData.Speed = owner.MoveData.MaxSpeed;
        //owner.SetLocalPosition(Target_Position.x, Target_Position.y, Target_Position.z);
        Complete = true;
      }
      else
      {
        if (owner.ActorState != ActorState.HYPERSPACE)
        {
          prevState = owner.ActorState;
          owner.ActorState = ActorState.HYPERSPACE;
          owner.LookAtPoint(Target_Position);
        }

        owner.MoveData.Speed = owner.MoveData.MaxSpeed + dist * SpeedDistanceFactor;
        if (owner.MoveData.Speed > Max_Speed)
          owner.MoveData.Speed = Max_Speed;

        //AdjustSpeed(owner, owner.MovementInfo.Speed);

        Complete = false;
      }

      prevdist = dist;
    }
  }
}
