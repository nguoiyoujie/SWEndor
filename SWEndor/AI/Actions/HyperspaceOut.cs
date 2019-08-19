using MTV3D65;
using SWEndor.Actors;

namespace SWEndor.AI.Actions
{
  public class HyperspaceOut : ActionInfo
  {
    public HyperspaceOut() : base("HyperspaceOut")
    {
      CanInterrupt = false;
    }

    // parameters
    private TV_3DVECTOR Origin_Position = new TV_3DVECTOR();
    private static float Incre_Speed = 2500;
    private static float FarEnoughDistance = 250000;
    private ActorState prevState = ActorState.NORMAL;

    public override void Process(Engine engine, ActorInfo actor)
    {
      if (actor.ActorState != ActorState.HYPERSPACE)
      {
        prevState = actor.ActorState;
        actor.ActorState = ActorState.HYPERSPACE;
        Origin_Position = actor.GetPosition();
      }
    }

    public void ApplyMove(ActorInfo owner)
    {
      //AdjustSpeed(owner, Target_Speed);
      owner.MoveData.Speed += Incre_Speed * owner.Game.TimeSinceRender;

      float dist = owner.TrueVision.TVMathLibrary.GetDistanceVec3D(owner.GetPosition(), Origin_Position);
      if (dist >= FarEnoughDistance)
      {
        owner.ActorState = prevState;
        Complete = true;
      }
      else
      {
        Complete = false;
      }
    }
  }
}
