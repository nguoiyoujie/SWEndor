using SWEndor.Actors.Data;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;
using SWEndor.Projectiles;

namespace SWEndor.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward only
  /// </summary>
  public static class MoveForwardOnly
  {
    public static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time)
    {
      // Control speed
      if (!data.FreeSpeed)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      actor.MoveRelative(data.Speed * time, 0, 0);
    }
  }
}
