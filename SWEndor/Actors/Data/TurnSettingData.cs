namespace SWEndor.Actors.Data
{
  public class TurnSettingData
  {
    // Pitch settings
    public float PitchLimit { get; set; }

    // yaw settings
    public float MaxTurnRate { get; set; }
    public float MaxSecondOrderTurnRateFrac { get; set; }

    // roll settings
    public float ZTilt { get; set; }
    public float ZNormFrac { get; set; }
    public bool ApplyZBalance { get; set; }

    // iterates Z rotation decay, uses a while loop... the algorithm should be replaced
    internal float Zdiv;
  }
}
