namespace SWEndor.Actors.Components
{
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

    public void Initialize(ActorInfo actor) { }
    public void Update(ActorInfo actor, float time)
    {
      actor.MoveComponent.XTurnAngle += PitchRate * time;
      actor.MoveAbsolute(ForwardRate * time, -SinkRate * time, 0);
    }
  }
}
