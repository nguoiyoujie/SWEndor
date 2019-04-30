using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public static class MovementDecorator
  {
    public static IMovementInfo Create(ActorInfo actor, ActorTypeInfo atype, ActorCreationInfo acreate)
    {
      if (atype.NoMove)
        return NoMoveInfo.Instance;

      else if (atype.MaxTurnRate == 0 && atype.MaxSpeed == Globals.LaserSpeed)
        return LaserMoveInfo.Instance;

      else if (atype.MaxTurnRate == 0)
        return new ForwardMoveInfo(acreate.InitialSpeed, atype.MaxSpeed, atype.MinSpeed, atype.MaxSpeedChangeRate);

      else if (atype.MaxSpeed == 0)
        return new RotateOnlyInfo(atype.MaxTurnRate, atype.MaxSecondOrderTurnRateFrac, atype.ZTilt, atype.ZNormFrac);

      return new MovementInfo(atype, acreate);
    }

  }

  public class MovementInfo : IMovementInfo
  {
    // General
    public float Speed { get; set; }
    public float XTurnAngle { get; set; }
    public float YTurnAngle { get; set; }
    public float ZRoll { get; set; }

    // speed settings
    public float MaxSpeed { get; set; }
    public float MinSpeed { get; set; }
    public float MaxSpeedChangeRate { get; set; }

    // yaw settings
    public float MaxTurnRate { get; set; }
    public float MaxSecondOrderTurnRateFrac { get; set; }

    // roll settings
    public float ZTilt { get; set; }
    public float ZNormFrac { get; set; }
    public bool ApplyZBalance { get; set; }

    // iterates Z rotation decay, uses a while loop... the algorithm should be replaced
    private float Zdiv;

    public MovementInfo(ActorTypeInfo atype, ActorCreationInfo acreate)
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

    public void Move(ActorInfo actor)
    {
      float time = actor.Game.TimeSinceRender;

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
    }
  }

  public class DyingKillInfo : IDyingMovementInfo
  {
    public static readonly DyingKillInfo Instance = new DyingKillInfo();
    private DyingKillInfo() { }

    public void Initialize(ActorInfo actor) { actor.ActorState = ActorState.DEAD; }
    public void Update(ActorInfo actor, float time) { }
  }

  public class DyingSinkInfo : IDyingMovementInfo
  {
    public float PitchRate;
    public float ForwardRate;
    public float SinkRate;

    public DyingSinkInfo(float pitchRate, float forwardRate, float sinkRate)
    {
      PitchRate = pitchRate;
      ForwardRate = forwardRate;
      SinkRate = sinkRate;
    }

    public void Initialize(ActorInfo actor) { }
    public void Update(ActorInfo actor, float time)
    {
      actor.MovementInfo.XTurnAngle += PitchRate * time;
      actor.MoveAbsolute(ForwardRate * time, -SinkRate * time, 0);
    }
  }

  public class DyingSpinInfo : IDyingMovementInfo
  {
    public int MinRate;
    public int MaxRate;
    private float D_spin_r;

    public DyingSpinInfo(int minRate, int maxRate)
    {
      MinRate = minRate;
      MaxRate = maxRate;
    }

    public void Initialize(ActorInfo actor)
    {
      actor.MovementInfo.ApplyZBalance = false;
      D_spin_r = actor.Engine.Random.Next(MinRate, MaxRate); // assumed D_spin_min_rate < D_spin_max_rate
      if (actor.Engine.Random.NextDouble() > 0.5f)
        D_spin_r = -D_spin_r;
    }

    public void Update(ActorInfo actor, float time)
    {
      float rotZ = D_spin_r * time;
      actor.Rotate(0, 0, rotZ);
      actor.MovementInfo.ResetTurn(); // force only forward
    }
  }
}
