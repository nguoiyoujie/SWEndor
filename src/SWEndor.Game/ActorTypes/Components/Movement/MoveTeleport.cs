using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;

namespace SWEndor.Game.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that teleports to target. Usually used for invisible targets
  /// </summary>
  internal static class MoveTeleport
  {
    internal static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
    {
      actor.Position = actor.AI.Target.Position;
    }
  }
}
