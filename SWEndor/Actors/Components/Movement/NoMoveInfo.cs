namespace SWEndor.Actors.Components
{
  public class NoMoveInfo : IMovementInfo
  {
    public static readonly NoMoveInfo Instance = new NoMoveInfo();
    private NoMoveInfo() { }

    // General
    public float Speed { get { return 0; } set { } }
    public float XTurnAngle { get { return 0; } set { } }
    public float YTurnAngle { get { return 0; } set { } }
    public float ZRoll { get { return 0; } set { } }

    // speed settings
    public float MaxSpeed { get { return 0; } set { } }
    public float MinSpeed { get { return 0; } set { } }
    public float MaxSpeedChangeRate { get { return 0; } set { } }

    // yaw settings
    public float MaxTurnRate { get { return 0; } set { } }
    public float MaxSecondOrderTurnRateFrac { get { return 0; } set { } }

    // roll settings
    public float ZTilt { get { return 0; } set { } }
    public float ZNormFrac { get { return 0; } set { } }
    public bool ApplyZBalance { get { return false; } set { } }

    public void Reset() { }
    public void ResetTurn() { }
    public void Move(ActorInfo actor) { }
  }
}
