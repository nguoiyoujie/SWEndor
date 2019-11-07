using SWEndor.Actors.Data;
using SWEndor.Core;
using SWEndor.Projectiles;

namespace SWEndor.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that does not move
  /// </summary>
  public static class NoMove
  {
    public static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time) { }
  }
}
