using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Projectiles;

namespace SWEndor.Game.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward only
  /// </summary>
  internal static class MoveForwardOnly
  {
    internal static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time)
    {
      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      actor.MoveRelative(data.Speed * time, 0, 0);
    }
  }
}
