using MTV3D65;
using SWEndor.Core;
using SWEndor.Primitives.Extensions;
using System;

namespace SWEndor.Actors.Models
{
  public enum TargetMode { POINT, ACTOR, ACTOR_SMARTPREDICTION }

  /// <summary>
  /// Storage of AI states per actor, transferrable across actions
  /// </summary>
  public struct AIData
  {
    // CombatZone // TO-DO: Change this to int when multiple Combat Zones is possible
    public bool EnteredCombatZone;

    // Targeting
    private TargetMode targetMode;
    private TV_3DVECTOR targetPos;
    private int targetActor;
    private float targetSpeed;

    // Distance Control
    public float FollowDistance { get; private set; }
    public void SetFollowDistance(ActorInfo owner, float value) { FollowDistance = value < 0 ? owner.TypeInfo.AIData.Move_CloseEnough : value; }

    public TV_3DVECTOR GetTargetPos(Engine e, ActorInfo owner)
    {
      if (targetMode == TargetMode.ACTOR)
      {
        ActorInfo a = e.ActorFactory.Get(targetActor);
        if (a != null)
          return a.GetGlobalPosition();
      }
      else if (targetMode == TargetMode.ACTOR_SMARTPREDICTION)
      {
        ActorInfo a = e.ActorFactory.Get(targetActor);
        if (a != null)
        {
          float dist = ActorDistanceInfo.GetDistance(owner, a);
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
      return (targetMode != TargetMode.POINT) ? e.ActorFactory.Get(targetActor) : null;
    }

    public float GetDistanceToTargetActor(Engine e, ActorInfo owner)
    {
      if (targetMode != TargetMode.POINT)
      {
        ActorInfo a = e.ActorFactory.Get(targetActor);
        if (a != null)
          return ActorDistanceInfo.GetDistance(owner, a);
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
      targetActor = target.ID;
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

      if (owner.TypeInfo.MoveLimitData.MaxTurnRate == 0) // Cannot turn
        return 0;

      if (owner.TypeInfo.AIData.AlwaysAccurateRotation)
      {
        owner.LookAt(targetPos);
        return 0;
      }

      TV_3DVECTOR tgtdir = targetPos - owner.GetGlobalPosition();
      TV_3DVECTOR chgrot = tgtdir.ConvertDirToRot() - owner.GetGlobalRotation();

      chgrot.x = chgrot.x.Modulus(-180, 180);
      chgrot.y = chgrot.y.Modulus(-180, 180);

      TV_3DVECTOR truechg = new TV_3DVECTOR(chgrot.x, chgrot.y, chgrot.z);

      // increased responsiveness
      chgrot *= responsiveness;

      chgrot.x = chgrot.x.Clamp(-owner.TypeInfo.MoveLimitData.MaxTurnRate, owner.TypeInfo.MoveLimitData.MaxTurnRate);
      chgrot.y = chgrot.y.Clamp(-owner.TypeInfo.MoveLimitData.MaxTurnRate, owner.TypeInfo.MoveLimitData.MaxTurnRate);

      // limit abrupt changes
      float limit = owner.TypeInfo.MoveLimitData.MaxTurnRate * owner.TypeInfo.MoveLimitData.MaxSecondOrderTurnRateFrac;
      if (Math.Abs(owner.MoveData.XTurnAngle - chgrot.x) > limit)
        owner.MoveData.XTurnAngle += limit * ((owner.MoveData.XTurnAngle > chgrot.x) ? -1 : 1);
      else
        owner.MoveData.XTurnAngle = chgrot.x;

      if (Math.Abs(owner.MoveData.YTurnAngle - chgrot.y) > limit)
        owner.MoveData.YTurnAngle += limit * ((owner.MoveData.YTurnAngle > chgrot.y) ? -1 : 1);
      else
        owner.MoveData.YTurnAngle = chgrot.y;

      TV_3DVECTOR vec = new TV_3DVECTOR();
      TV_3DVECTOR dir = owner.GetGlobalDirection();
      owner.Engine.TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, tgtdir);
      return owner.Engine.TrueVision.TVMathLibrary.ACos(owner.Engine.TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));
    }

    internal float AdjustSpeed(ActorInfo owner)
    {
      if (!owner.MoveData.FreeSpeed)
        targetSpeed = targetSpeed.Clamp(owner.MoveData.MinSpeed, owner.MoveData.MaxSpeed);

      if (owner.MoveData.Speed > targetSpeed)
      {
        owner.MoveData.Speed -= owner.MoveData.MaxSpeedChangeRate * owner.Engine.Game.TimeSinceRender;
        if (owner.MoveData.Speed < targetSpeed)
          owner.MoveData.Speed = targetSpeed;
      }
      else
      {
        owner.MoveData.Speed += owner.MoveData.MaxSpeedChangeRate * owner.Engine.Game.TimeSinceRender;
        if (owner.MoveData.Speed > targetSpeed)
          owner.MoveData.Speed = targetSpeed;
      }

      return owner.MoveData.Speed - targetSpeed;
    }
  }
}
