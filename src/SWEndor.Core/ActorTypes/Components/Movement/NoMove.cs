using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that does not move
  /// </summary>
  internal static class NoMove
  {
    internal static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time) { }
  }
}
