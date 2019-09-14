using SWEndor.ActorTypes;

namespace SWEndor.Actors.Data
{
  public struct MoveData
  {
    // General
    public float Speed;

    public bool FreeSpeed;

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
      FreeSpeed = acreate.FreeSpeed;
      Speed = acreate.InitialSpeed;
      XTurnAngle = 0;
      YTurnAngle = 0;
      ZRoll = 0;
      MaxSpeed = atype.MoveLimitData.MaxSpeed;
      MinSpeed = atype.MoveLimitData.MinSpeed;
      MaxSpeedChangeRate = atype.MoveLimitData.MaxSpeedChangeRate;
      MaxTurnRate = atype.MoveLimitData.MaxTurnRate;
      MaxSecondOrderTurnRateFrac = atype.MoveLimitData.MaxSecondOrderTurnRateFrac;
      ZTilt = atype.MoveLimitData.ZTilt;
      ZNormFrac = atype.MoveLimitData.ZNormFrac;
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

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public bool ApplyZBalance { get { return MoveData.ApplyZBalance; }  set { MoveData.ApplyZBalance = value; } }
    public float XTurnAngle { get { return MoveData.XTurnAngle; } set { MoveData.XTurnAngle = value; } }
  }
}