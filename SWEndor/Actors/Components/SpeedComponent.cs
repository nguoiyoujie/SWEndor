using MTV3D65;
using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{








  public interface IMovable
  {
    void Move(float time);
  }

  public class AAANoMove : IMovable
  {
    public void Move(float time) { }
  }

  public class AAAMoveForwardOnly : IMovable
  {
    private CoordData _coord;
    private SpeedData _speed;

    public AAAMoveForwardOnly(CoordData coord, SpeedData speed)
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

  public class AAARotateOnly : IMovable
  {
    private CoordData _coord;
    private TurnData _turn;
    private TurnSettingData _turnS;

    public AAARotateOnly(CoordData coord, TurnData turn, TurnSettingData turnS)
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
    private CoordData _coord;
    private SpeedData _speed;
    private TurnData _turn;
    private SpeedSettingData _speedS;
    private TurnSettingData _turnS;

    public Moveable(CoordData coord, SpeedData speed, TurnData turn, SpeedSettingData speedS, TurnSettingData turnS)
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
