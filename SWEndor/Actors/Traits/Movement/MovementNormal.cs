using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Primitives.Traits;
using System.Linq;

namespace SWEndor.Actors.Traits
{
  public class MovementNormal : ITick
  {
    public float Speed { get; set; } = 0;
    public float XTurnAngle { get; set; } = 0;
    public float YTurnAngle { get; set; } = 0;
    public float ZRoll { get; private set; } = 0;

    public float MaxSpeed { get; private set; } = 0;
    public float MinSpeed { get; private set; } = 0;
    public float MaxSpeedChangeRate { get; private set; } = 0;

    public float MaxTurnRate { get; private set; } = 0;
    public float MaxSecondOrderTurnRateFrac { get; private set; } = 0;

    public float ZTilt { get; private set; } = 0;
    public float ZNormFrac { get; private set; } = 0;
    public bool ApplyZBalance { get; private set; } = true;

    public float Zdiv { get; private set; } = 0;

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

    public void Move<A>(A self, float time)
    {
      /*
      // Hyperspace special: AI loop may not be in sync
      if (actor.ActorState == ActorState.HYPERSPACE)
      {
        if (actor.CurrentAction is HyperspaceIn)
          ((HyperspaceIn)actor.CurrentAction).ApplyMove(actor);
        else if (actor.CurrentAction is HyperspaceOut)
          ((HyperspaceOut)actor.CurrentAction).ApplyMove(actor);

        actor.MoveRelative(Speed * time, 0, 0);
        return;
      }
      */
      /*
      // Control speed
      if (actor.ActorState != ActorState.FREE
       && actor.ActorState != ActorState.HYPERSPACE)
        Speed = Speed.Clamp(MinSpeed, MaxSpeed);

      // Control rotation
      if (ApplyZBalance)
      {
        TV_3DVECTOR vec = actor.GetRotation();
        actor.SetLocalRotation(vec.x, vec.y, 0);
        actor.MoveRelative(Speed * time, 0, 0);
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
        actor.MoveRelative(Speed * time, 0, 0);
        ZRoll = vec.z;
        ZRoll -= YTurnAngle * ZTilt * time;
        float rotX2 = vec.x + XTurnAngle * time;
        rotX2 = rotX2.Clamp(-actor.TypeInfo.XLimit, actor.TypeInfo.XLimit);
        float rotY2 = vec.y + YTurnAngle * time;
        actor.SetLocalRotation(rotX2, rotY2, ZRoll);
      }
      */
    }

    void ITick.Tick<A>(A self, float time)
    {
      Move(self, time);
    }
  }
}
