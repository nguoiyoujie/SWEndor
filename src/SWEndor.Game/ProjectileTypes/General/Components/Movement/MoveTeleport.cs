using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;
using SWEndor.Game.Projectiles;

namespace SWEndor.Game.ProjectileTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that teleports to target
  /// </summary>
  internal static class MoveTeleport
  {
    public static void Move(Engine engine, ProjectileInfo actor, ref MoveData data, float time)
    {
      if (actor.Target != null)
      {
        actor.Position = actor.Target.Position;
      }
    }
  }
}
