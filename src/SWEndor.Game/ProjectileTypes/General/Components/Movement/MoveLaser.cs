using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;
using SWEndor.Game.Projectiles;

namespace SWEndor.Game.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward at laser speed
  /// </summary>
  internal static class MoveLaser
  {
    internal static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time)
    {
      actor.MoveRelative(data.Speed * time, 0, 0);
    }
  }
}
