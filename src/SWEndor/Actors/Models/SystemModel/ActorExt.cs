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

    public void SetStatus(SystemPart part, SystemState newstate)
    {
      Systems.SetStatus(part, newstate);
    }

    public void DamageRandom()
    {
      Systems.DamageRandom(Engine, this, ref TypeInfo.SystemData);
    }

    public void DisableRandom()
    {
      Systems.DisableRandom(Engine, this, ref TypeInfo.SystemData);
    }

    public void Distribute(ref SystemData data, float time)
    {
      Systems.Distribute(ref TypeInfo.SystemData, Engine.Game.TimeSinceRender);
    }
  }
}