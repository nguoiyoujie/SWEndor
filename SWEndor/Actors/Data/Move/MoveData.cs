using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public struct MoveData
  {
    // General
    public float Speed;
    public float XTurnAngle;
    public float YTurnAngle;
    public float ZRoll;

    // speed settings
    public float MaxSpeed;
    public float MinSpeed;
    public float MaxSpeedChangeRate;

    // yaw settings
    public float MaxTurnRate;
    public float MaxSecondOrderTurnRateFrac;

    // roll settings
    public float ZTilt;
    public float ZNormFrac;
    public bool ApplyZBalance;

    // iterates Z rotation decay, uses a while loop... the algorithm should be replaced
    public float Zdiv;

    public void Init(ActorTypeInfo atype, ActorCreationInfo acreate)
    {
      Speed = acreate.InitialSpeed;
      XTurnAngle = 0;
      YTurnAngle = 0;
      ZRoll = 0;
      MaxSpeed = atype.MaxSpeed;
      MinSpeed = atype.MinSpeed;
      MaxSpeedChangeRate = atype.MaxSpeedChangeRate;
      MaxTurnRate = atype.MaxTurnRate;
      MaxSecondOrderTurnRateFrac = atype.MaxSecondOrderTurnRateFrac;
      ZTilt = atype.ZTilt;
      ZNormFrac = atype.ZNormFrac;
      ApplyZBalance = true;
      Zdiv = 0;
    }


    public void Reset()
    {
      Speed = 0;
      XTurnAngle = 0;
      YTurnAngle = 0;
      ZRoll = 0;
      MaxSpeed = 0;
      MinSpeed = 0;
      MaxSpeedChangeRate = 0;
      MaxTurnRate = 0;
      MaxSecondOrderTurnRateFrac = 0;
      ZTilt = 0;
      ZNormFrac = 0;
      ApplyZBalance = true;
      Zdiv = 0;
    }

    public void ResetTurn()
    {
      XTurnAngle = 0;
      YTurnAngle = 0;
    }
  }
}
