using SWEndor.Actors;
using SWEndor.Actors.Data;
using SWEndor.Core;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward at laser speed
  /// </summary>
  public static class MoveLaser
  {
    public static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
    {
      actor.MoveRelative(Globals.LaserSpeed * actor.Game.TimeSinceRender, 0, 0);
    }
  }
}
