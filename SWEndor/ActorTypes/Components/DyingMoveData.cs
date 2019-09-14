using SWEndor.Actors;
using System;

namespace SWEndor.ActorTypes.Components
{
  public struct DyingMoveData
  {
    Action<ActorInfo> _init;
    Action<ActorInfo, float> _update;

    public void Kill()
    {
      _init = (a) => a.SetState_Dead();
      _update = null;
    }

    public void Spin(int minRate, int maxRate)
    {
      float rate = minRate + (float)Globals.Engine.Random.NextDouble() * (maxRate - minRate);
      if (Globals.Engine.Random.NextDouble() > 0.5)
        rate = -rate;

      _init = (a) => a.ApplyZBalance = false;
      _update = (a, t) =>
      {
        a.Rotate(0, 0, rate * t);
        a.MoveData.ResetTurn();
      };
    }

    public void Sink(float pitchRate, float forwardRate, float sinkRate)
    {
      _init = null;
      _update = (a, t) =>
     {
       a.XTurnAngle += pitchRate * t;
       a.MoveAbsolute(forwardRate * t, -sinkRate * t, 0);
     };
    }

    public void Initialize(ActorInfo actor) { _init?.Invoke(actor); }
    public void Update(ActorInfo actor, float time) { _update?.Invoke(actor, time); }
  }
}
