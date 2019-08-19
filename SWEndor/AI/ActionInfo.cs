using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
using System;

namespace SWEndor.AI.Actions
{
  public class ActionInfo
  {
    public ActionInfo(string name)
    {
      Name = name;
    }

    // parameters
    public string Name { get; private set; }
    public bool Complete = false;
    public bool CanInterrupt = true;
    public ActionInfo NextAction = null;

    // collision avoidance
    private float m_collisioncheck_time = 0;


    public override string ToString()
    {
      return string.Format("{0},{1}"
                          , Name
                          , Complete
                          );
    }

    public virtual void Process(Engine engine, ActorInfo owner)
    {
      Complete = true;
    }

    protected bool CheckBounds(ActorInfo owner)
    {
      float boundmult = 0.99f;
      if (!(owner.TypeInfo is ActorTypes.Groups.Projectile) 
        && owner.IsOutOfBounds(owner.GetEngine().GameScenarioManager.MinAIBounds * boundmult, owner.GetEngine().GameScenarioManager.MaxAIBounds * boundmult) 
        && owner.EnteredCombatZone)
      {
        float x = owner.GetEngine().Random.Next((int)(owner.GetEngine().GameScenarioManager.MinAIBounds.x * 0.65f), (int)(owner.GetEngine().GameScenarioManager.MaxAIBounds.x * 0.65f));
        float y = owner.GetEngine().Random.Next(-200, 200);
        float z = owner.GetEngine().Random.Next((int)(owner.GetEngine().GameScenarioManager.MinAIBounds.z * 0.65f), (int)(owner.GetEngine().GameScenarioManager.MaxAIBounds.z * 0.65f));

        if (owner.CurrentAction is Move)
          owner.CurrentAction.Complete = true;
        owner.GetEngine().ActionManager.QueueFirst(owner.ID, new ForcedMove(new TV_3DVECTOR(x, y, z), owner.MoveData.MaxSpeed, -1, 360 / (owner.MoveData.MaxTurnRate + 72)));
        return false;
      }
      else
      {
        if (!owner.IsOutOfBounds(owner.GetEngine().GameScenarioManager.MinAIBounds * boundmult, owner.GetEngine().GameScenarioManager.MaxAIBounds * boundmult))
        {
          owner.EnteredCombatZone = true;
        }
      }
      return true;
    }

    internal static float AdjustRotation(ActorInfo owner, TV_3DVECTOR target_Position, bool isAttacking = false, bool isAvoidCollision = false)
    {
      if (owner.TypeInfo.AlwaysAccurateRotation)
      {
        owner.LookAtPoint(target_Position);
        return 0;
      }
      if (owner.TypeInfo.MaxTurnRate == 0) // Cannot turn
      {
        return 0;
      }
      else
      {
        TV_3DVECTOR tgtdir = target_Position - owner.GetPosition();

        TV_3DVECTOR chgrot = Utilities.GetRotation(tgtdir) - owner.GetRotation();

        chgrot.x = chgrot.x.Modulus(-180, 180);
        chgrot.y = chgrot.y.Modulus(-180, 180);

        TV_3DVECTOR truechg = new TV_3DVECTOR(chgrot.x, chgrot.y, chgrot.z);

        // increased responsiveness
        chgrot *= isAvoidCollision ? 9999 : 10;

        chgrot.x = chgrot.x.Clamp(-owner.TypeInfo.MaxTurnRate, owner.TypeInfo.MaxTurnRate);
        chgrot.y = chgrot.y.Clamp(-owner.TypeInfo.MaxTurnRate, owner.TypeInfo.MaxTurnRate);

        // limit abrupt changes
        float limit = owner.TypeInfo.MaxTurnRate * owner.TypeInfo.MaxSecondOrderTurnRateFrac;
        if (Math.Abs(owner.MoveData.XTurnAngle - chgrot.x) > limit)
          owner.MoveData.XTurnAngle += limit * ((owner.MoveData.XTurnAngle > chgrot.x) ? -1 : 1);
        else
          owner.MoveData.XTurnAngle = chgrot.x;

        if (Math.Abs(owner.MoveData.YTurnAngle - chgrot.y) > limit)
          owner.MoveData.YTurnAngle += limit * ((owner.MoveData.YTurnAngle > chgrot.y) ? -1 : 1);
        else
          owner.MoveData.YTurnAngle = chgrot.y;

        

        TV_3DVECTOR vec = new TV_3DVECTOR();
        TV_3DVECTOR dir = owner.GetDirection();
        owner.GetEngine().TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, tgtdir);
        float delta = owner.GetEngine().TrueVision.TVMathLibrary.ACos(owner.GetEngine().TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));

        if (owner.IsPlayer)
          owner.GetEngine().Screen2D.MessageSecondaryText(string.Format("DELTA: {0:0.000}", delta), 1.5f, new TV_COLOR(0.5f, 0.5f, 1, 1), 0);
        return delta;

      }
    }

    internal static float AdjustSpeed(ActorInfo owner, float target_Speed)
    {
      if (owner.ActorState != ActorState.FREE && owner.ActorState != ActorState.HYPERSPACE)
      {
        target_Speed = target_Speed.Clamp(owner.MoveData.MinSpeed, owner.MoveData.MaxSpeed);
      }

      if (owner.MoveData.Speed > target_Speed)
      {
        owner.MoveData.Speed -= owner.MoveData.MaxSpeedChangeRate * owner.GetEngine().Game.TimeSinceRender;
        if (owner.MoveData.Speed < target_Speed)
          owner.MoveData.Speed = target_Speed;
      }
      else
      {
        owner.MoveData.Speed += owner.MoveData.MaxSpeedChangeRate * owner.GetEngine().Game.TimeSinceRender;
        if (owner.MoveData.Speed > target_Speed)
          owner.MoveData.Speed = target_Speed;
      }

      return (owner.MoveData.Speed - target_Speed);
    }

    protected bool CheckImminentCollision(ActorInfo owner, float scandistance)
    {
      //return false; // disable for now

      if (!owner.TypeInfo.CanCheckCollisionAhead)
        return false;

      if (!owner.Engine.MaskDataSet[owner].Has(ComponentMask.CAN_BECOLLIDED))
        return false;

      if (scandistance <= 0)
        return false;

      return CollisionSystem.ActivateImminentCollisionCheck(owner.Engine, owner, ref m_collisioncheck_time, scandistance);
    }

    public void Dispose()
    {
      NextAction = null;
      Complete = true;
    }
  }
}
