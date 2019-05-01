using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public struct MoveData
  {
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
  }
}
