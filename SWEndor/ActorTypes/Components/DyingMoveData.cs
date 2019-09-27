using MTV3D65;
using SWEndor.Actors;
using System;

namespace SWEndor.ActorTypes.Components
{
  public struct DyingMoveData
  {
    Action<ActorInfo> _init;
    Action<ActorInfo, float> _update;
    TV_3DVECTOR _data;

    public void Kill()
    {
      _init = (a) => a.SetState_Dead();
      _update = null;
    }

    public void Spin(int minRate, int maxRate)
    {
      _data.x = minRate + (float)Globals.Engine.Random.NextDouble() * (maxRate - minRate);
      if (Globals.Engine.Random.NextDouble() > 0.5)
        _data.x = -_data.x;

      _init = (a) => a.ApplyZBalance = false;
      _update = (a, t) =>
      {
        a.Rotate(0, 0, a.TypeInfo.DyingMoveData._data.x * t);
        a.MoveData.ResetTurn();
      };
    }

    public void Sink(float pitchRate, float forwardRate, float sinkRate)
    {
      _data.x = pitchRate;
      _data.y = forwardRate;
      _data.z = sinkRate;

      _init = null;
      _update = (a, t) =>
      {
       a.XTurnAngle += a.TypeInfo.DyingMoveData._data.x * t;
       a.MoveAbsolute(a.TypeInfo.DyingMoveData._data.y * t, -a.TypeInfo.DyingMoveData._data.z * t, 0);
      };
    }

    public void Initialize(ActorInfo actor) { _init?.Invoke(actor); }
    public void Update(ActorInfo actor, float time) { _update?.Invoke(actor, time); }
  }
}
