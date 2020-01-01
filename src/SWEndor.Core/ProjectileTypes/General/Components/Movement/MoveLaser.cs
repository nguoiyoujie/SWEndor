using SWEndor.Actors.Data;
using SWEndor.Core;
using SWEndor.Projectiles;

namespace SWEndor.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward at laser speed
  /// </summary>
  internal static class MoveLaser
  {
    internal static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time)
    {
      actor.MoveRelative(Globals.LaserSpeed * time, 0, 0);
    }
  }
}
