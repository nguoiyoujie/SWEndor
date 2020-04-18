using Primitives.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct MoveLimitData
  {
    private const string sMoveLimit = "MoveLimit";

#pragma warning disable 0649 // values are filled by the attribute
    [INIValue(sMoveLimit, "MaxSpeed")]
    public float MaxSpeed;

    [INIValue(sMoveLimit, "MinSpeed")]
    public float MinSpeed;

    [INIValue(sMoveLimit, "MaxSpeedChangeRate")]
    public float MaxSpeedChangeRate;

    [INIValue(sMoveLimit, "MaxTurnRate")]
    public float MaxTurnRate;

    [INIValue(sMoveLimit, "MaxSecondOrderTurnRateFrac")]
    public float MaxSecondOrderTurnRateFrac;

    [INIValue(sMoveLimit, "XLimit")]
    public float XLimit;

    [INIValue(sMoveLimit, "ZTilt")]
    public float ZTilt;

    [INIValue(sMoveLimit, "ZNormFrac")]
    public float ZNormFrac;
#pragma warning restore 0649

    public readonly static MoveLimitData Default =
        new MoveLimitData
        {
          MaxSecondOrderTurnRateFrac = 0.2f,
          XLimit = 75.0f,
          ZNormFrac = 0.025f,
        };
  }
}
