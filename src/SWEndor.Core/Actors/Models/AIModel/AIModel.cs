using MTV3D65;
using Primrose.Primitives.ValueTypes;
using SWEndor.Core;
using SWEndor.Models;

namespace SWEndor.Actors.Models
{
  /// <summary>
  /// Storage of AI states per actor, transferrable across actions
  /// </summary>
  internal struct AIModel
  {
    private enum TargetMode { POINT, ACTOR, ACTOR_SMARTPREDICTION }

    internal bool CanEvade;
    internal bool CanRetaliate;
    internal int HuntWeight;

    internal float AIMinSpeed;
    internal float AIMaxSpeed;


    // CombatZone // TO-DO: Change this to int when multiple Combat Zones is possible
    public int CombatZone;

    // Targeting
    private TargetMode targetMode;
    private TV_3DVECTOR targetPos;
    private int targetActorID;
    private float targetSpeed;

    // Distance Control
    public float FollowDistance { get; private set; }
    public void SetFollowDistance(ActorInfo owner, float value) { FollowDistance = value < 0 ? owner.TypeInfo.AIData.Move_CloseEnough : value; }

    public void Reset()
    {
      CanEvade = true;
      CanRetaliate = true;
      HuntWeight = 1;
      CombatZone = -1;
      targetMode = TargetMode.POINT;
      targetActorID = -1;
      AIMinSpeed = -1;
      AIMaxSpeed = -1;
    }

    public void Init(ref ActorTypes.Components.AIData data, ref ActorTypes.Components.MoveLimitData mdata)
    {
      CanEvade = data.CanEvade;
      CanRetaliate = data.CanRetaliate;
      HuntWeight = data.HuntWeight;
      CombatZone = -1;
      targetMode = TargetMode.POINT;
      targetActorID = -1;
      AIMinSpeed = data.AIMinSpeed == -1 ? mdata.MinSpeed : data.AIMinSpeed;
      AIMaxSpeed = data.AIMaxSpeed == -1 ? mdata.MaxSpeed : data.AIMaxSpeed;
    }

    public TV_3DVECTOR GetTargetPos(Engine e, ActorInfo owner)
    {
      if (targetMode == TargetMode.ACTOR)
      {
        ActorInfo a = e.ActorFactory.Get(targetActorID);
        if (a != null)
          return a.GetGlobalPosition();
      }
      else if (targetMode == TargetMode.ACTOR_SMARTPREDICTION)
      {
        ActorInfo a = e.ActorFactory.Get(targetActorID);
        if (a != null)
        {
          float dist = DistanceModel.GetDistance(e, owner, a);
          float d = dist / Globals.LaserSpeed + e.Game.TimeSinceRender;
          ActorInfo a2 = a.ParentForCoords;
          if (a2 == null)
            return a.GetRelativePositionXYZ(0, 0, a.MoveData.Speed * d);
          else
            return a.GetGlobalPosition() + a2.GetRelativePositionXYZ(0, 0, a2.MoveData.Speed * d) - a2.GetGlobalPosition();
        }
      }
      return targetPos;
    }

    public void UpdateTargetPos(Engine e, ActorInfo owner) { targetPos = GetTargetPos(e, owner); }

    public ActorInfo GetTargetActor(Engine e, ActorInfo owner)
    {
      return (targetMode != TargetMode.POINT) ? e.ActorFactory.Get(targetActorID) : null;
    }

    public float GetDistanceToTargetActor(Engine e, ActorInfo owner)
    {
      if (targetMode != TargetMode.POINT)
      {
        ActorInfo a = e.ActorFactory.Get(targetActorID);
        if (a != null)
          return DistanceModel.GetDistance(e, owner, a);
      }
      return 0;
    }

    public void SetTarget(TV_3DVECTOR pos)
    {
      targetMode = TargetMode.POINT;
      targetPos = pos;
    }

    public void SetTarget(Engine engine, ActorInfo owner, ActorInfo target, bool prediction)
    {
      targetMode = prediction ? TargetMode.ACTOR_SMARTPREDICTION : TargetMode.ACTOR;
      targetActorID = target.ID;
      UpdateTargetPos(engine, owner);
      targetSpeed = GetDistanceToTargetActor(engine, owner) - FollowDistance;
    }

    public void SetTargetSpeed(float speed)
    {
      targetSpeed = speed;
    }

    internal float AdjustRotation(Engine engine, ActorInfo owner, float responsiveness = 10)
    {
      UpdateTargetPos(engine, owner);
      return AIUpdate.AdjustRotation(owner.Engine, owner, ref targetPos, responsiveness);
    }

    internal float AdjustSpeed(ActorInfo owner, bool useAILimit)
    {
      return AIUpdate.AdjustSpeed(owner.Engine, owner, useAILimit, ref targetSpeed);
    }
  }
}
