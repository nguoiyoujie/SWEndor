using SWEndor.Game.Actors;
using SWEndor.Game.Actors.Data;
using SWEndor.Game.Core;

namespace SWEndor.Game.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward at laser speed
  /// </summary>
  internal static class MoveLaser
  {
    internal static void Move(Engine engine, ActorInfo actor, ref MoveData data, float time)
    {
      actor.MoveRelative(Globals.LaserSpeed * engine.Game.TimeSinceRender, 0, 0);
    }
  }
}
