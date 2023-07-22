
using SWEndor.Game.Core;

namespace SWEndor.Game.Projectiles.Models
{
  internal struct JammingModel
  {
    // not sure how we will use this. All homing projectiles are ActorInfo
    // live data
    public float StunRecoverTime;

    public void Reset()
    {
      StunRecoverTime = 0;
    }

    public bool IsStunned(Engine engine)
    {
      return StunRecoverTime > engine.Game.GameTime;
    }

    public void InflictStun(Engine engine, float duration)
    {
      StunRecoverTime = engine.Game.GameTime + duration;
    }
  }
}