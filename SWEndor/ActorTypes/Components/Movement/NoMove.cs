using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that does not move
  /// </summary>
  public static class NoMove
  {
    public static void Move(ActorInfo actor, ref MoveData data) { }
  }
}
