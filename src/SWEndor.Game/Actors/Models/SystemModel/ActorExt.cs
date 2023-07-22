using SWEndor.Game.Actors.Models;
using SWEndor.Game.ActorTypes.Components;
using SWEndor.Game.Core;
using System.Collections.Generic;

namespace SWEndor.Game.Actors
{
  public partial class ActorInfo
  {
    internal IList<SystemInstrument> GetInstruments()
    {
      return Systems.Instruments;
    }

    public SystemState GetStatus(SystemPart part)
    {
      return Systems.GetStatus(Engine, part);
    }

    public bool IsSystemOperational(SystemPart part)
    {
      SystemState state = Systems.GetStatus(Engine, part);
      return !Systems.IsStunned(Engine) && (state == SystemState.PASSIVE || state == SystemState.ACTIVE);
    }

    public bool IsStunned
    {
      get { return Systems.IsStunned(Engine); }
    }

    public float StunDurationRemaining
    {
      get { return Systems.StunRecoverTime - Engine.Game.GameTime; }
    }

    public void InflictStun(float stunduration)
    {
      Systems.InflictStun(Engine, stunduration);
    }

    internal void SetStatus(SystemPart part, SystemState newstate)
    {
      Systems.SetStatus(part, newstate);
    }

    internal void DamageRandom(float chanceModifier)
    {
      Systems.DamageRandom(Engine, this, chanceModifier, ref TypeInfo.SystemData);
    }

    internal void DisableRandom()
    {
      Systems.DisableRandom(Engine, this, ref TypeInfo.SystemData);
    }

    internal void Distribute(ref SystemData data, float time)
    {
      Systems.Distribute(Engine, ref TypeInfo.SystemData, Engine.Game.TimeSinceRender);
    }
  }
}