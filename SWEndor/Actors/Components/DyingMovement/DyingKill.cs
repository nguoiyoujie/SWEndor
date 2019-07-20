using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;

namespace SWEndor.Actors.Components
{
  public class DyingKill : IDyingMoveComponent
  {
    public static readonly DyingKill Instance = new DyingKill();
    private DyingKill() { }

    public void Initialize(ActorInfo actor, ref MoveData data)
    {
      actor.StateModel.MakeDead(actor);
    }
    public void Update(ActorInfo actor, ref MoveData data, float time) { }
  }
}
