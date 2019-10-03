using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Components;

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


    public void Init(ref MoveLimitData data, ActorCreationInfo acreate)
    {
      FreeSpeed = acreate.FreeSpeed;
      Speed = acreate.InitialSpeed;
      XTurnAngle = 0;
      YTurnAngle = 0;
      ZRoll = 0;
      MaxSpeed = data.MaxSpeed;
      MinSpeed = data.MinSpeed;
      MaxSpeedChangeRate = data.MaxSpeedChangeRate;
      MaxTurnRate = data.MaxTurnRate;
      MaxSecondOrderTurnRateFrac = data.MaxSecondOrderTurnRateFrac;
      ZTilt = data.ZTilt;
      ZNormFrac = data.ZNormFrac;
      ApplyZBalance = true;
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