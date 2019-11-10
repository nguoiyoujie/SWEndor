using SWEndor.Actors.Models;
using SWEndor.ActorTypes.Components;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public SystemState GetStatus(SystemPart part)
    {
      return Systems.GetStatus(part);
    }

    internal void SetStatus(SystemPart part, SystemState newstate)
    {
      Systems.SetStatus(part, newstate);
    }

    internal void DamageRandom()
    {
      Systems.DamageRandom(Engine, this, ref TypeInfo.SystemData);
    }

    internal void DisableRandom()
    {
      Systems.DisableRandom(Engine, this, ref TypeInfo.SystemData);
    }

    internal void Distribute(ref SystemData data, float time)
    {
      Systems.Distribute(ref TypeInfo.SystemData, Engine.Game.TimeSinceRender);
    }
  }
}