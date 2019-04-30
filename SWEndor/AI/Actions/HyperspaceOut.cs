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
    private float Target_Speed = 1000;
    private float FarEnoughDistance = 50000;
    private ActorState prevState = ActorState.NORMAL;
    private bool m_switch = false;

    public override void Process(Engine engine, int actorID)
    {
      ActorInfo actor = engine.ActorFactory.Get(actorID);
      if (actor.ActorState != ActorState.HYPERSPACE && !m_switch)
      {
        prevState = actor.ActorState;
        actor.ActorState = ActorState.HYPERSPACE;
        Origin_Position = actor.GetPosition();
      }
    }

    public void ApplyMove(ActorInfo owner)
    {
      //AdjustSpeed(owner, Target_Speed);
      owner.MovementInfo.Speed += Target_Speed;

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
