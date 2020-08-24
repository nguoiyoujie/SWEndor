using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;
using SWEndor.Game.Projectiles;

namespace SWEndor.Game.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that does not move
  /// </summary>
  internal static class NoMove
  {
    internal static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time) { }
  }
}
