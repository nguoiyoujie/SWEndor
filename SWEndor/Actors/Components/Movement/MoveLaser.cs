using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  /// <summary>
  /// Implementation of IMoveComponent that moves forward at laser speed
  /// </summary>
  public struct MoveLaser : IMoveComponent
  {
    public static readonly MoveLaser Instance = new MoveLaser();

    public void Move(ActorInfo actor, ref MoveData data)
    {
      actor.MoveRelative(Globals.LaserSpeed * actor.Game.TimeSinceRender, 0, 0);
    }
  }
}
