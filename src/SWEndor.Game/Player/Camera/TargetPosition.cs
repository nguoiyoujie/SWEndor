using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Player
{
  public struct TargetPosition
  {
    public TV_3DVECTOR Position;
    public TV_3DVECTOR PositionRelative;
    public int TargetActorID;

    private TV_3DVECTOR _lastPos;
    public TV_3DVECTOR GetGlobalPosition(Engine engine)
    {
      if (TargetActorID >= 0)
      {
        ActorInfo tgt = engine.ActorFactory.Get(TargetActorID);
        if (tgt != null && tgt.Active)
          _lastPos = Position + Utilities.GetRelativePositionXYZ(engine, tgt.GetGlobalPosition(), tgt.GetGlobalRotation(), PositionRelative.x, PositionRelative.y, PositionRelative.z);

        return _lastPos;
      }
      else
      {
        return Position;
      }
    }

    public void ApproachPosition(TVMathLibrary math, TV_3DVECTOR pos, float distance)
    {
      if (distance == 0 || TargetActorID >= 0) // if locked to Actor, skip
        return;

      float dist = DistanceModel.GetDistance(math, pos, Position);

      if (dist != 0)
        Position = DistanceModel.Lerp(math, Position, pos, (distance / dist).Clamp(-100, 1));
    }
  }
}
