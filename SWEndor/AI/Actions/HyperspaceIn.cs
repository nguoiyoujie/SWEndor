using MTV3D65;
using SWEndor.Actors;
using SWEndor.Primitives;

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
    private bool hyperspace = false;
    private float prevdist = 9999999;

    public override string ToString()
    {
      return "{0},{1},{2}".F(Name
                          , Utilities.ToString(Target_Position)
                          , Complete
                          );
    }

    public override void Process(Engine engine, ActorInfo actor) { }

    public void ApplyMove(ActorInfo owner)
    {
      float dist = owner.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetGlobalPosition(), Target_Position);

      if (dist <= CloseEnoughDistance || prevdist < dist)
      {
        owner.MoveData.Speed = owner.MoveData.MaxSpeed;
        Complete = true;
      }
      else
      {
        if (!hyperspace)
        {
          hyperspace = true;
          owner.Transform.LookAt(Target_Position);
        }

        owner.MoveData.Speed = owner.MoveData.MaxSpeed + dist * SpeedDistanceFactor;
        if (owner.MoveData.Speed > Max_Speed)
          owner.MoveData.Speed = Max_Speed;

      }
      prevdist = dist;
    }
  }
}
