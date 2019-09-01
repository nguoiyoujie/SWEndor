using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  /// <summary>
  /// Implementation of IDyingMoveComponent that moves and rotates the actor downwards
  /// </summary>
  public class DyingSink : IDyingMoveComponent
  {
    public float PitchRate;
    public float ForwardRate;
    public float SinkRate;

    public DyingSink(float pitchRate, float forwardRate, float sinkRate)
    {
      PitchRate = pitchRate;
      ForwardRate = forwardRate;
      SinkRate = sinkRate;
    }

    public void Initialize(ActorInfo actor, ref MoveData data) { }
    public void Update(ActorInfo actor, ref MoveData data, float time)
    {
      data.XTurnAngle += PitchRate * time;
      actor.MoveAbsolute(ForwardRate * time, -SinkRate * time, 0);
    }
  }
}
