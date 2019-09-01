using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  /// <summary>
  /// Implementation of IMoveComponent that does not move
  /// </summary>
  public struct NoMove : IMoveComponent
  {
    public static readonly NoMove Instance = new NoMove();

    public void Move(ActorInfo actor, ref MoveData data) { }
  }
}
