using SWEndor.Actors.Data;
using SWEndor.AI.Actions;

namespace SWEndor.Actors.Components
{
  public struct MoveForwardOnly : IMoveComponent
  {
    public static readonly MoveForwardOnly Instance = new MoveForwardOnly();

    public void Move(ActorInfo actor, ref MoveData data)
    {
      float time = actor.Game.TimeSinceRender;

      // Hyperspace special: AI loop may not be in sync
      if (actor.ActorState == ActorState.HYPERSPACE)
      {
        if (actor.CurrentAction is HyperspaceIn)
          ((HyperspaceIn)actor.CurrentAction).ApplyMove(actor);
        else if (actor.CurrentAction is HyperspaceOut)
          ((HyperspaceOut)actor.CurrentAction).ApplyMove(actor);

        actor.MoveRelative(data.Speed * time, 0, 0);
        return;
      }

      // Control speed
      if (actor.ActorState != ActorState.FREE
       && actor.ActorState != ActorState.HYPERSPACE)
        data.Speed = data.Speed.Clamp(data.MinSpeed, data.MaxSpeed);

      actor.MoveRelative(Globals.LaserSpeed * time, 0, 0);
    }
  }
}
