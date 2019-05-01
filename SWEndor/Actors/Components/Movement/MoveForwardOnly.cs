using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public struct MoveForwardOnly : IMoveComponent
  {
    public MoveForwardOnly(float speed, float maxspeed, float minspeed, float changerate)
    {
      Speed = speed;
      MaxSpeed = maxspeed;
      MinSpeed = minspeed;
      MaxSpeedChangeRate = changerate;
    }

    // General
    public float Speed { get; set; }
    public float XTurnAngle { get { return 0; } set { } }
    public float YTurnAngle { get { return 0; } set { } }
    public float ZRoll { get { return 0; } set { } }

    // speed settings
    public float MaxSpeed { get; set; }
    public float MinSpeed { get; set; }
    public float MaxSpeedChangeRate { get; set; }

    // yaw settings
    public float MaxTurnRate { get { return 0; } set { } }
    public float MaxSecondOrderTurnRateFrac { get { return 0; } set { } }

    // roll settings
    public float ZTilt { get { return 0; } set { } }
    public float ZNormFrac { get { return 0; } set { } }
    public bool ApplyZBalance { get { return false; } set { } }

    public void Reset() { }
    public void ResetTurn() { }
    public void Move(ActorInfo actor)
    {
      float time = actor.Game.TimeSinceRender;

      // Hyperspace special: AI loop may not be in sync
      if (actor.ActorState == ActorState.HYPERSPACE)
      {
        if (actor.CurrentAction is HyperspaceIn)
          ((HyperspaceIn)actor.CurrentAction).ApplyMove(actor);
        else if (actor.CurrentAction is HyperspaceOut)
          ((HyperspaceOut)actor.CurrentAction).ApplyMove(actor);

        actor.MoveRelative(Speed * time, 0, 0);
        return;
      }

      // Control speed
      if (actor.ActorState != ActorState.FREE
       && actor.ActorState != ActorState.HYPERSPACE)
        Speed = Speed.Clamp(MinSpeed, MaxSpeed);

      actor.MoveRelative(Globals.LaserSpeed * time, 0, 0);
    }
  }
}
