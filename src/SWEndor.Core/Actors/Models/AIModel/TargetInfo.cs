using MTV3D65;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.Actors.Models
{
  public struct TargetInfo
  {
    //private enum TargetMode : byte { POINT, ACTOR, ACTOR_SMARTPREDICTION }

    private TV_3DVECTOR _pos;
    private int _actorID;
    private bool _smartPrediction;

    public TargetInfo(TV_3DVECTOR point)
    {
      _pos = point;
      _actorID = -1;
      _smartPrediction = false;
    }

    public TargetInfo(int actorID, bool smartPrediction)
    {
      _pos = default(TV_3DVECTOR);
      _actorID = actorID;
      _smartPrediction = smartPrediction;
    }

    public void Set(TV_3DVECTOR point)
    {
      _pos = point;
      _actorID = -1;
      _smartPrediction = false;
    }

    public void Set(int actorID, bool smartPrediction)
    {
      _actorID = actorID;
      _smartPrediction = smartPrediction;
    }

    public TV_3DVECTOR Position { get { return _pos; } }
    public int ActorID { get { return _actorID; } }

    private TV_3DVECTOR GetTargetPosFromActor(Engine e, ActorInfo a)
    {
      return e.ActorFactory.Get(_actorID)?.GetGlobalPosition() ?? _pos;
    }

    private TV_3DVECTOR GetTargetPosFromActor_SmartPrediction(Engine e, ActorInfo a)
    {
      ActorInfo tgt = e.ActorFactory.Get(_actorID);
      if (tgt != null)
      {
        float dist = DistanceModel.GetDistance(e, a, tgt);
        float d = dist / Globals.LaserSpeed + e.Game.TimeSinceRender;
        ActorInfo tgtp = tgt.ParentForCoords;
        if (tgtp == null)
          return tgt.GetRelativePositionXYZ(0, 0, tgt.MoveData.Speed * d);
        else
          return tgt.GetGlobalPosition() + tgtp.GetRelativePositionXYZ(0, 0, tgtp.MoveData.Speed * d) - tgtp.GetGlobalPosition();
      }
      return _pos;
    }

    public void Update(Engine e, ActorInfo owner)
    {
      if (_actorID >= 0)
      {
        if (_smartPrediction)
          _pos = GetTargetPosFromActor_SmartPrediction(e, owner);
        else
          _pos = GetTargetPosFromActor(e, owner);
      }
    }

    public ActorInfo GetTargetActor(Engine e)
    {
      return e.ActorFactory.Get(_actorID);
    }

    public float GetDistanceFromTarget(Engine e, ActorInfo owner)
    {
      ActorInfo a = GetTargetActor(e);
      if (a != null)
        return DistanceModel.GetDistance(e, owner, a);
      return DistanceModel.GetDistance(e.TrueVision.TVMathLibrary, owner.GetGlobalPosition(), _pos);
    }
  }
}
