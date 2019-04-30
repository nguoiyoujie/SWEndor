namespace SWEndor.Actors.Components
{
  public interface IMoveComponent
  {
    // General
    float Speed { get; set; }
    float XTurnAngle { get; set; } // Pitch
    float YTurnAngle { get; set; } // Yaw
    float ZRoll { get; set; } // Roll

    // speed settings
    float MaxSpeed { get; set; }
    float MinSpeed { get; set; }
    float MaxSpeedChangeRate { get; set; }

    // yaw settings
    float MaxTurnRate { get; set; }
    float MaxSecondOrderTurnRateFrac { get; set; }

    // roll settings
    bool ApplyZBalance { get; set; }

    void Reset();
    void ResetTurn();
    void Move(ActorInfo actor);
  }
}
