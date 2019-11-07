using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.AI.Actions;
using SWEndor.Core;
using Primrose.Primitives.Extensions;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward only
  /// </summary>
  public static class MoveForwardOnly
  {
    public static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
    {
      // Hyperspace special: AI loop may not be in sync
      if (actor.CurrentAction is HyperspaceIn)
      {
        ((HyperspaceIn)actor.CurrentAction).ApplyMove(engine, actor);
        actor.MoveRelative(data.Speed * time);
        return;
      }
      else if (actor.CurrentAction is HyperspaceOut)
      {
        ((HyperspaceOut)actor.CurrentAction).ApplyMove(engine, actor);
        actor.MoveRelative(data.Speed * time);
        return;
      }

      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      actor.MoveRelative(Globals.LaserSpeed * time, 0, 0);
    }
  }
}
