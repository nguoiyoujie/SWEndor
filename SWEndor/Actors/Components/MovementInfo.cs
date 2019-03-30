using MTV3D65;
using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public enum DyingMovement { NONE, SPIN, SINK }


  public class MovementInfo
  {
    private ActorInfo Actor;

    // General
    public float Speed = 0;
    public float XTurnAngle = 0; // Pitch
    public float YTurnAngle = 0; // Yaw
    public float ZRoll = 0; // Roll

    // speed settings
    public float MaxSpeed = 0;
    public float MinSpeed = 0;
    public float MaxSpeedChangeRate = 0;

    // yaw settings
    public float MaxTurnRate = 0;
    public float MaxSecondOrderTurnRateFrac = 0;

    // roll settings
    public float ZTilt = 0;
    public float ZNormFrac = 0;
    public bool ApplyZBalance = true;

    // iterates Z rotation decay, uses a while loop... the algorithm should be replaced
    private float Zdiv = 0;

    public DyingMovement DyingMovement = DyingMovement.NONE;
    // Death Spin
    public int D_spin_min_rate = 0;
    public int D_spin_max_rate = 0;
    private float D_spin_r = 0;

    public float D_sink_pitch_rate = 0;
    public float D_sink_forward_rate = 0;
    public float D_sink_down_rate = 0;

    public MovementInfo(ActorInfo actor)
    {
      Actor = actor;
    }

    public void ResetTurn()
    {
      XTurnAngle = 0;
      YTurnAngle = 0;
    }

    public void Move()
    {
      //Is fixed
      if (Actor.ActorState == ActorState.FIXED || Actor.TypeInfo.NoMove)
        return;

      // Hyperspace special: AI loop may not be in sync
      if (Actor.ActorState == ActorState.HYPERSPACE)
      {
        if (Actor.CurrentAction is HyperspaceIn)
          ((HyperspaceIn)Actor.CurrentAction).ApplyMove(Actor);
        else if (Actor.CurrentAction is HyperspaceOut)
          ((HyperspaceOut)Actor.CurrentAction).ApplyMove(Actor);

        Actor.MoveRelative(Speed * Game.Instance().TimeSinceRender, 0, 0);
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
        Actor.MoveRelative(Speed * Game.Instance().TimeSinceRender, 0, 0);
        ZRoll -= YTurnAngle * ZTilt * Game.Instance().TimeSinceRender;

        // Z rotation decay.
        Zdiv += Game.Instance().TimeSinceRender / 0.005f;
        while (Zdiv > 0 && !float.IsInfinity(Zdiv))
        {
          ZRoll *= 1 - ZNormFrac;
          Zdiv--;
        }

        float rotX2 = vec.x + XTurnAngle * Game.Instance().TimeSinceRender;
        Utilities.Clamp(ref rotX2, -Actor.TypeInfo.XLimit, Actor.TypeInfo.XLimit);
        float rotY2 = vec.y + YTurnAngle * Game.Instance().TimeSinceRender;

        Actor.SetLocalRotation(rotX2, rotY2, ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = Actor.GetRotation();
        Actor.MoveRelative(Speed * Game.Instance().TimeSinceRender, 0, 0);
        ZRoll = vec.z;
        ZRoll -= YTurnAngle * ZTilt * Game.Instance().TimeSinceRender;
        float rotX2 = vec.x + XTurnAngle * Game.Instance().TimeSinceRender;
        Utilities.Clamp(ref rotX2, -Actor.TypeInfo.XLimit, Actor.TypeInfo.XLimit);
        float rotY2 = vec.y + YTurnAngle * Game.Instance().TimeSinceRender;
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
            D_spin_r = Engine.Instance().Random.Next(D_spin_min_rate, D_spin_max_rate); // assumed D_spin_min_rate < D_spin_max_rate
            if (Engine.Instance().Random.NextDouble() > 0.5f)
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
            float rotZ = D_spin_r * Game.Instance().TimeSinceRender;
            Actor.Rotate(0, 0, rotZ);
            Actor.MovementInfo.ResetTurn(); // force only forward
            break;

          case DyingMovement.SINK:
            Actor.MovementInfo.XTurnAngle += D_sink_pitch_rate * Game.Instance().TimeSinceRender;
            Actor.MoveAbsolute(D_sink_forward_rate * Game.Instance().TimeSinceRender, -D_sink_down_rate * Game.Instance().TimeSinceRender, 0);
            break;
        }
      }
    }
  }
}
