using MTV3D65;
using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public class RotateOnlyInfo : IMovementInfo
  {
    public RotateOnlyInfo(float maxturn, float max2orderturn, float ztilt, float znormfrac)
    {
      MaxTurnRate = maxturn;
      MaxSecondOrderTurnRateFrac = max2orderturn;
      ZTilt = ztilt;
      ZNormFrac = znormfrac;
      ApplyZBalance = true;
    }

    // General
    public float Speed { get { return 0; } set { } }
    public float XTurnAngle { get; set; }
    public float YTurnAngle { get; set; }
    public float ZRoll { get; set; }

    // speed settings
    public float MaxSpeed { get { return 0; } set { } }
    public float MinSpeed { get { return 0; } set { } }
    public float MaxSpeedChangeRate { get { return 0; } set { } }

    // yaw settings
    public float MaxTurnRate { get; set; }
    public float MaxSecondOrderTurnRateFrac { get; set; }

    // roll settings
    public float ZTilt { get; set; }
    public float ZNormFrac { get; set; }
    public bool ApplyZBalance { get; set; }

    // iterates Z rotation decay, uses a while loop... the algorithm should be replaced
    private float Zdiv;

    public void Reset()
    {
      XTurnAngle = 0;
      YTurnAngle = 0;
      ZRoll = 0;
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

    public void Move(ActorInfo actor)
    {
      float time = actor.Game.TimeSinceRender;

      // Control rotation
      if (ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.GetRotation();
        actor.SetLocalRotation(vec.x, vec.y, 0);
        ZRoll -= YTurnAngle * ZTilt * time;

        // Z rotation decay.
        Zdiv += time / 0.005f;
        while (Zdiv > 0 && !float.IsInfinity(Zdiv))
        {
          ZRoll *= 1 - ZNormFrac;
          Zdiv--;
        }

        float rotX2 = vec.x + XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.XLimit, actor.TypeInfo.XLimit);
        float rotY2 = vec.y + YTurnAngle * time;

        actor.SetLocalRotation(rotX2, rotY2, ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = actor.GetRotation();
        ZRoll = vec.z;
        ZRoll -= YTurnAngle * ZTilt * time;
        float rotX2 = vec.x + XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.XLimit, actor.TypeInfo.XLimit);
        float rotY2 = vec.y + YTurnAngle * time;
        actor.SetLocalRotation(rotX2, rotY2, ZRoll);
      }
    }
  }
}
