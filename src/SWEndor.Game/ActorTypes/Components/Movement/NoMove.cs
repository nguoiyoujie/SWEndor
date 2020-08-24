using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;

namespace SWEndor.Game.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that does not move
  /// </summary>
  internal static class NoMove
  {
    internal static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time) { }
  }
}
