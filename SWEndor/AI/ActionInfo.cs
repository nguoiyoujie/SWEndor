using MTV3D65;
using SWEndor.Actors;
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

    public virtual void Process(Engine engine, int ownerID)
    {
      Complete = true;
    }

    protected bool CheckBounds(ActorInfo owner)
    {
      float boundmult = 0.99f;
      if (!(owner.TypeInfo is ActorTypes.Group.Projectile) && owner.IsOutOfBounds(owner.GetEngine().GameScenarioManager.MinAIBounds * boundmult, owner.GetEngine().GameScenarioManager.MaxAIBounds * boundmult) && owner.EnteredCombatZone)
      {
        float x = owner.GetEngine().Random.Next((int)(owner.GetEngine().GameScenarioManager.MinAIBounds.x * 0.65f), (int)(owner.GetEngine().GameScenarioManager.MaxAIBounds.x * 0.65f));
        float y = owner.GetEngine().Random.Next(-200, 200);
        float z = owner.GetEngine().Random.Next((int)(owner.GetEngine().GameScenarioManager.MinAIBounds.z * 0.65f), (int)(owner.GetEngine().GameScenarioManager.MaxAIBounds.z * 0.65f));

        if (owner.CurrentAction is Move)
          owner.CurrentAction.Complete = true;
        owner.GetEngine().ActionManager.QueueFirst(owner.ID, new ForcedMove(new TV_3DVECTOR(x, y, z), owner.MovementInfo.MaxSpeed, -1, 360 / (owner.MovementInfo.MaxTurnRate + 72)));
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

    protected float AdjustRotation(ActorInfo owner, TV_3DVECTOR target_Position, bool isAttacking = false)
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
        if (this is AvoidCollisionRotate)
          chgrot *= 9999;
        else
          chgrot *= 10;

        chgrot.x = chgrot.x.Clamp(-owner.TypeInfo.MaxTurnRate, owner.TypeInfo.MaxTurnRate);
        chgrot.y = chgrot.y.Clamp(-owner.TypeInfo.MaxTurnRate, owner.TypeInfo.MaxTurnRate);

        // limit abrupt changes
        float limit = owner.TypeInfo.MaxTurnRate * owner.TypeInfo.MaxSecondOrderTurnRateFrac;
        if (Math.Abs(owner.MovementInfo.XTurnAngle - chgrot.x) > limit)
          owner.MovementInfo.XTurnAngle += limit * ((owner.MovementInfo.XTurnAngle > chgrot.x) ? -1 : 1);
        else
          owner.MovementInfo.XTurnAngle = chgrot.x;

        if (Math.Abs(owner.MovementInfo.YTurnAngle - chgrot.y) > limit)
          owner.MovementInfo.YTurnAngle += limit * ((owner.MovementInfo.YTurnAngle > chgrot.y) ? -1 : 1);
        else
          owner.MovementInfo.YTurnAngle = chgrot.y;

        

        TV_3DVECTOR vec = new TV_3DVECTOR();
        TV_3DVECTOR dir = owner.GetDirection();
        owner.GetEngine().TrueVision.TVMathLibrary.TVVec3Normalize(ref vec, tgtdir);
        float delta = owner.GetEngine().TrueVision.TVMathLibrary.ACos(owner.GetEngine().TrueVision.TVMathLibrary.TVVec3Dot(dir, vec));

        if (ActorInfo.IsPlayer(owner.GetEngine(), owner.ID))
          owner.GetEngine().Screen2D.MessageSecondaryText(string.Format("DELTA: {0:0.000}", delta), 1.5f, new TV_COLOR(0.5f, 0.5f, 1, 1), 0);
        return delta;

        /*
        if (isAttacking)
        {
          return (Math.Abs(truechgx - owner.XTurnAngle) + Math.Abs(truechgy - owner.YTurnAngle));
        }
        else
          return Math.Abs(owner.XTurnAngle) + Math.Abs(owner.YTurnAngle);
        */
      }
    }

    protected float AdjustSpeed(ActorInfo owner, float target_Speed)
    {
      if (owner.ActorState != ActorState.FREE && owner.ActorState != ActorState.HYPERSPACE)
      {
        target_Speed = target_Speed.Clamp(owner.MovementInfo.MinSpeed, owner.MovementInfo.MaxSpeed);
      }

      if (owner.MovementInfo.Speed > target_Speed)
      {
        owner.MovementInfo.Speed -= owner.MovementInfo.MaxSpeedChangeRate * owner.GetEngine().Game.TimeSinceRender;
        if (owner.MovementInfo.Speed < target_Speed)
          owner.MovementInfo.Speed = target_Speed;
      }
      else
      {
        owner.MovementInfo.Speed += owner.MovementInfo.MaxSpeedChangeRate * owner.GetEngine().Game.TimeSinceRender;
        if (owner.MovementInfo.Speed > target_Speed)
          owner.MovementInfo.Speed = target_Speed;
      }

      return (owner.MovementInfo.Speed - target_Speed);
    }

    protected bool CheckImminentCollision(ActorInfo owner, float scandistance)
    {
      return false; // disable for now

      if (!owner.TypeInfo.CanCheckCollisionAhead)
        return false;

      if (!owner.TypeInfo.CollisionEnabled)
        return false;

      if (scandistance <= 0)
        return false;

      if (m_collisioncheck_time < owner.GetEngine().Game.GameTime)
      {
        if (owner.CollisionInfo.IsInProspectiveCollision)
        {
          m_collisioncheck_time = owner.GetEngine().Game.GameTime + 0.25f; // delay should be adjusted with FPS / CPU load, ideally every ~0.5s, but not more than 2.5s. Can be slightly longer since it is already performing evasion.
        }
        else
        {
          m_collisioncheck_time = owner.GetEngine().Game.GameTime + 0.1f; // delay should be adjusted with FPS / CPU load, ideally every run (~0.1s), but not more than 2s.
        }
        owner.CollisionInfo.ProspectiveCollisionScanDistance = scandistance;
        owner.CollisionInfo.IsTestingProspectiveCollision = true;
      }
      return owner.CollisionInfo.IsInProspectiveCollision;
    }

    public void Dispose()
    {
      NextAction = null;
      Complete = true;
    }
  }
}
