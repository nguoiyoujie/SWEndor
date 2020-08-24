using Primrose.FileFormat.INI;

namespace SWEndor.Game.ActorTypes.Components
{
  internal struct MoveLimitData
  {
#pragma warning disable 0649 // values are filled by the attribute
    [INIValue]
    public float MaxSpeed;

    [INIValue]
    public float MinSpeed;

    [INIValue]
    public float MaxSpeedChangeRate;

    [INIValue]
    public float MaxTurnRate;

    [INIValue]
    public float MaxSecondOrderTurnRateFrac;

    [INIValue]
    public float XLimit;

    [INIValue]
    public float ZTilt;

    [INIValue]
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
