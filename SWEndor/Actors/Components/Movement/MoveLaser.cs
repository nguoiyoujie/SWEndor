using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public struct MoveLaser : IMoveComponent
  {
    public static readonly MoveLaser Instance = new MoveLaser();

    public void Move(ActorInfo actor, ref MoveData data)
    {
      actor.MoveRelative(Globals.LaserSpeed * actor.Game.TimeSinceRender);
    }
  }
}
