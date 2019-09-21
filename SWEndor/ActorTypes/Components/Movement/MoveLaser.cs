using SWEndor.Actors;
using SWEndor.Actors.Data;

namespace SWEndor.ActorTypes.Components
{
  /// <summary>
  /// Implementation of Move behavior that moves forward at laser speed
  /// </summary>
  public static class MoveLaser
  {
    public static void Move(ActorInfo actor, ref MoveData data, float time)
    {
      actor.MoveRelative(Globals.LaserSpeed * actor.Game.TimeSinceRender, 0, 0);
    }
  }
}
