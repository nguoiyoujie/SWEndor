using SWEndor.Actors.Data;
using SWEndor.Core;
using SWEndor.Projectiles;

namespace SWEndor.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that does not move
  /// </summary>
  internal static class NoMove
  {
    internal static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time) { }
  }
}
