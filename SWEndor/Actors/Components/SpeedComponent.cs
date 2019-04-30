using MTV3D65;

namespace SWEndor.Actors.Components
{
  public class CoordComponent
  {
    // General
    public TV_3DVECTOR Position { get; set; }
    public TV_3DVECTOR Rotation { get; set; }
  }

  public class SpeedComponent
  {
    // General
    public float Speed { get; set; }
  }

  public class SpeedSettingsComponent
  {
    public float MaxSpeed { get; set; }
    public float MinSpeed { get; set; }
    public float MaxSpeedChangeRate { get; set; }
  }

  public class TurnComponent
  {
    public float XTurnAngle { get; set; } // Pitch
    public float YTurnAngle { get; set; } // Yaw
    public float ZRoll { get; set; } // Roll
  }

  public class TurnSettingsComponent
  {
    // Pitch settings
    public float PitchLimit { get; set; }

    // yaw settings
    public float MaxTurnRate { get; set; }
    public float MaxSecondOrderTurnRateFrac { get; set; }

    // roll settings
    public float ZTilt { get; set; }
    public float ZNormFrac { get; set; }
    public bool ApplyZBalance { get; set; }

    // iterates Z rotation decay, uses a while loop... the algorithm should be replaced
    public float Zdiv;
  }

  public interface IMovable
  {
    void Move(float time);
  }

  public class NoMove : IMovable
  {
    public void Move(float time) { }
  }

  public class MoveForwardOnly : IMovable
  {
    private CoordComponent _coord;
    private SpeedComponent _speed;

    public MoveForwardOnly(CoordComponent coord, SpeedComponent speed)
    { _coord = coord; _speed = speed; }

    public void Move(float time)
    {
      MoveForward(_speed.Speed * time);
    }

    public void MoveForward(float front)
    {
      _coord.Position += Moveable.GetRelativePositionFUR(front, 0, 0, _coord.Rotation);
    }
  }

  public class RotateOnly : IMovable
  {
    private CoordComponent _coord;
    private TurnComponent _turn;
    private TurnSettingsComponent _turnS;

    public RotateOnly(CoordComponent coord, TurnComponent turn, TurnSettingsComponent turnS)
    { _coord = coord; _turn = turn; _turnS = turnS; }

    public void Move(float time)
    {
      // Control rotation
      if (_turnS.ApplyZBalance)
      {
        TV_3DVECTOR vec = _coord.Rotation;
        _coord.Rotation = new TV_3DVECTOR(vec.x, vec.y, 0);
        _turn.ZRoll -= _turn.YTurnAngle * _turnS.ZTilt * time;

        // Z rotation decay.
        _turnS.Zdiv += time / 0.005f;
        while (_turnS.Zdiv > 0 && !float.IsInfinity(_turnS.Zdiv))
        {
          _turn.ZRoll *= 1 - _turnS.ZNormFrac;
          _turnS.Zdiv--;
        }

        float rotX2 = vec.x + _turn.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-_turnS.PitchLimit, _turnS.PitchLimit);
        float rotY2 = vec.y + _turn.YTurnAngle * time;
        _coord.Rotation = new TV_3DVECTOR(rotX2, rotY2, _turn.ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = _coord.Rotation;
        _turn.ZRoll = vec.z;
        _turn.ZRoll -= _turn.YTurnAngle * _turnS.ZTilt * time;
        float rotX2 = vec.x + _turn.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-_turnS.PitchLimit, _turnS.PitchLimit);
        float rotY2 = vec.y + _turn.YTurnAngle * time;
        _coord.Rotation = new TV_3DVECTOR(rotX2, rotY2, _turn.ZRoll);
      }
    }
  }

  public class Moveable : IMovable
  {
    private CoordComponent _coord;
    private SpeedComponent _speed;
    private TurnComponent _turn;
    private SpeedSettingsComponent _speedS;
    private TurnSettingsComponent _turnS;

    public Moveable(CoordComponent coord, SpeedComponent speed, TurnComponent turn, SpeedSettingsComponent speedS, TurnSettingsComponent turnS)
    { _coord = coord; _speed = speed; _turn = turn; _speedS = speedS; _turnS = turnS; }

    public void Move(float time)
    {
      // Control speed
      //if (Actor.ActorState != ActorState.FREE
      // && Actor.ActorState != ActorState.HYPERSPACE)
      //  SpeedComponent.Speed = SpeedComponent.Speed.Clamp(SpeedComponent.MinSpeed, SpeedComponent.MaxSpeed);

      // Control rotation
      if (_turnS.ApplyZBalance)
      {
        TV_3DVECTOR vec = _coord.Rotation;
        _coord.Rotation = new TV_3DVECTOR(vec.x, vec.y, 0);
        MoveForward(_speed.Speed * time);
        _turn.ZRoll -= _turn.YTurnAngle * _turnS.ZTilt * time;

        // Z rotation decay.
        _turnS.Zdiv += time / 0.005f;
        while (_turnS.Zdiv > 0 && !float.IsInfinity(_turnS.Zdiv))
        {
          _turn.ZRoll *= 1 - _turnS.ZNormFrac;
          _turnS.Zdiv--;
        }

        float rotX2 = vec.x + _turn.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-_turnS.PitchLimit, _turnS.PitchLimit);
        float rotY2 = vec.y + _turn.YTurnAngle * time;
        _coord.Rotation = new TV_3DVECTOR(rotX2, rotY2, _turn.ZRoll);
      }
      else
      {
        TV_3DVECTOR vec = _coord.Rotation;
        MoveForward(_speed.Speed * time);
        _turn.ZRoll = vec.z;
        _turn.ZRoll -= _turn.YTurnAngle * _turnS.ZTilt * time;
        float rotX2 = vec.x + _turn.XTurnAngle * time;
        rotX2 = rotX2.Clamp(-_turnS.PitchLimit, _turnS.PitchLimit);
        float rotY2 = vec.y + _turn.YTurnAngle * time;
        _coord.Rotation = new TV_3DVECTOR(rotX2, rotY2, _turn.ZRoll);
      }
    }

    public void MoveForward(float front)
    {
      _coord.Position += GetRelativePositionFUR(front, 0, 0, _coord.Rotation);
    }

    public static TV_3DVECTOR GetRelativePositionFUR(float front, float up, float right, TV_3DVECTOR rotation)
    {
      TV_3DVECTOR ret = new TV_3DVECTOR();
      TrueVision TrueVision = Globals.Engine.TrueVision;
      TrueVision.TVMathLibrary.TVVec3Rotate(ref ret
                                          , new TV_3DVECTOR(right, up, front)
                                          , rotation.y
                                          , rotation.x
                                          , rotation.z);
      return ret;
    }
  }
}
