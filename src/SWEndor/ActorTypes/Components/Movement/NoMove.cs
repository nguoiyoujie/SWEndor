using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that does not move
  /// </summary>
  public static class NoMove
  {
    public static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time) { }
  }
}
