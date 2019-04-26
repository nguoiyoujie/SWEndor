using MTV3D65;
using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public enum DyingMovement { NONE, SPIN, SINK }

  public struct MovementInfo
  {
    private readonly ActorInfo Actor;

    // General
    public float Speed;
    public float XTurnAngle; // Pitch
    public float YTurnAngle; // Yaw
    public float ZRoll; // Roll

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
    private float Zdiv;

    public DyingMovement DyingMovement;
    // Death Spin
    public int D_spin_min_rate;
    public int D_spin_max_rate;
    private float D_spin_r;

    public float D_sink_pitch_rate;
    public float D_sink_forward_rate;
    public float D_sink_down_rate;

    public MovementInfo(ActorInfo actor)
    {
      Actor = actor;

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
      DyingMovement = DyingMovement.NONE;
      D_spin_min_rate = 0;
      D_spin_max_rate = 0;
      D_spin_r = 0;
      D_sink_pitch_rate = 0;
      D_sink_forward_rate = 0;
      D_sink_down_rate = 0;
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
      DyingMovement = DyingMovement.NONE;
      D_spin_min_rate = 0;
      D_spin_max_rate = 0;
      D_spin_r = 0;
      D_sink_pitch_rate = 0;
      D_sink_forward_rate = 0;
      D_sink_down_rate = 0;
    }

    public void ResetTurn()
    {
      XTurnAngle = 0;
      YTurnAngle = 0;
    }

    public void Move()
    {
      if (Actor.TypeInfo.NoMove)
        return;

      // Hyperspace special: AI loop may not be in sync
      if (Actor.ActorState == ActorState.HYPERSPACE)
      {
        if (Actor.CurrentAction is HyperspaceIn)
          ((HyperspaceIn)Actor.CurrentAction).ApplyMove(Actor);
        else if (Actor.CurrentAction is HyperspaceOut)
          ((HyperspaceOut)Actor.CurrentAction).ApplyMove(Actor);

        Actor.MoveRelative(Speed * Actor.GetEngine().Game.TimeSinceRender, 0, 0);
        return;
      }

      // Control speed
      if (Actor.ActorState != ActorState.FREE 
       && Actor.ActorState != ActorState.HYPERSPACE)
        Utilities.Clamp(ref Speed, MinSpeed, MaxSpeed);

      // Control rotation
      if (ApplyZBalance)
      {
        TV_3DVECTOR vec = Actor.GetRotation();
        Actor.SetLocalRotation(vec.x, vec.y, 0);
        Actor.MoveRelative(Speed * Actor.GetEngine().Game.TimeSinceRender, 0, 0);
        ZRoll -= YTurnAngle * ZTilt * Actor.GetEngine().Game.TimeSinceRender;

        // Z rotation decay.
        Zdiv += Actor.GetEngine().Game.TimeSinceRender / 0.005f;
        while (Zdiv > 0 && !float.IsInfinity(Zdiv))
        {
          ZRoll *= 1 - ZNormFrac;
          Zdiv--;
        }

        float rotX2 = vec.x + XTurnAngle * Actor.GetEngine().Game.TimeSinceRender;
        Utilities.Clamp(ref rotX2, -Actor.TypeInfo.XLimit, Actor.TypeInfo.XLimit);
        float rotY2 = vec.y + YTurnAngle * Actor.GetEngine().Game.TimeSinceRender;

        Actor.SetLocalRotation(rotX2, rotY2, ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = Actor.GetRotation();
        Actor.MoveRelative(Speed * Actor.GetEngine().Game.TimeSinceRender, 0, 0);
        ZRoll = vec.z;
        ZRoll -= YTurnAngle * ZTilt * Actor.GetEngine().Game.TimeSinceRender;
        float rotX2 = vec.x + XTurnAngle * Actor.GetEngine().Game.TimeSinceRender;
        Utilities.Clamp(ref rotX2, -Actor.TypeInfo.XLimit, Actor.TypeInfo.XLimit);
        float rotY2 = vec.y + YTurnAngle * Actor.GetEngine().Game.TimeSinceRender;
        Actor.SetLocalRotation(rotX2, rotY2, ZRoll);
      }
    }

    public void GenerateDyingMovement()
    {
      if (Actor.ActorState == ActorState.DYING)
      {
        switch (DyingMovement)
        {
          case DyingMovement.SPIN:
            ApplyZBalance = false;
            D_spin_r = Actor.GetEngine().Random.Next(D_spin_min_rate, D_spin_max_rate); // assumed D_spin_min_rate < D_spin_max_rate
            if (Actor.GetEngine().Random.NextDouble() > 0.5f)
              D_spin_r = -D_spin_r;
            break;
        }
      }
    }

    public void ExecuteDyingMovement()
    {
      if (Actor.ActorState == ActorState.DYING)
      {
        switch (DyingMovement)
        {
          case DyingMovement.SPIN:
            float rotZ = D_spin_r * Actor.GetEngine().Game.TimeSinceRender;
            Actor.Rotate(0, 0, rotZ);
            Actor.MovementInfo.ResetTurn(); // force only forward
            break;

          case DyingMovement.SINK:
            Actor.MovementInfo.XTurnAngle += D_sink_pitch_rate * Actor.GetEngine().Game.TimeSinceRender;
            Actor.MoveAbsolute(D_sink_forward_rate * Actor.GetEngine().Game.TimeSinceRender, -D_sink_down_rate * Actor.GetEngine().Game.TimeSinceRender, 0);
            break;
        }
      }
    }
  }
}
