using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
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

    public virtual void Process(ActorInfo owner)
    {
      Complete = true;
    }

    protected bool CheckBounds(ActorInfo owner)
    {
      float boundmult = 0.99f;
      if (!(owner.TypeInfo is ProjectileGroup) && owner.IsOutOfBounds(GameScenarioManager.Instance().MinAIBounds * boundmult, GameScenarioManager.Instance().MaxAIBounds * boundmult) && owner.EnteredCombatZone)
      {
        float x = Engine.Instance().Random.Next((int)(GameScenarioManager.Instance().MinAIBounds.x * 0.65f), (int)(GameScenarioManager.Instance().MaxAIBounds.x * 0.65f));
        float y = Engine.Instance().Random.Next(-200, 200);
        float z = Engine.Instance().Random.Next((int)(GameScenarioManager.Instance().MinAIBounds.z * 0.65f), (int)(GameScenarioManager.Instance().MaxAIBounds.z * 0.65f));

        //ActionManager.QueueFirst(owner, new Actions.Rotate(new TV_3DVECTOR(), owner.MaxSpeed, 5, false));
        if (owner.CurrentAction is Actions.Move)
          owner.CurrentAction.Complete = true;
        ActionManager.QueueFirst(owner, new Actions.ForcedMove(new TV_3DVECTOR(x, y, z), owner.MaxSpeed, -1, 360 / (owner.MaxTurnRate + 72)));
        //owner.EnteredCombatZone = false;
        return false;
      }
      else
      {
        if (!owner.IsOutOfBounds(GameScenarioManager.Instance().MinAIBounds * boundmult, GameScenarioManager.Instance().MaxAIBounds * boundmult))
        {
          owner.EnteredCombatZone = true;
        }
      }
      return true;
    }

    //float prevXTurnAngle = 0;
    //float prevYTurnAngle = 0;
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
        TV_3DVECTOR rot = owner.GetRotation();
        TV_3DVECTOR tgtrot = Utilities.GetRotation(target_Position - owner.GetPosition());

        float chgx = tgtrot.x - rot.x;
        float chgy = tgtrot.y - rot.y;

        while (chgx < -180)
          chgx += 360;

        while (chgx > 180)
          chgx -= 360;

        while (chgy < -180)
          chgy += 360;

        while (chgy > 180)
          chgy -= 360;

        chgx *= 10; ///= Game.Instance().TimeSinceRender;
        chgy *= 10; ///= Game.Instance().TimeSinceRender;
        float truechgx = chgx;
        float truechgy = chgy;

        if (chgx > owner.TypeInfo.MaxTurnRate)
        {
          chgx = owner.TypeInfo.MaxTurnRate;
        }
        else if (chgx < -owner.TypeInfo.MaxTurnRate)
        {
          chgx = -owner.TypeInfo.MaxTurnRate;
        }

        if (chgy > owner.TypeInfo.MaxTurnRate)
        {
          chgy = owner.TypeInfo.MaxTurnRate;
        }
        else if (chgy < -owner.TypeInfo.MaxTurnRate)
        {
          chgy = -owner.TypeInfo.MaxTurnRate;
        }

        // limit abrupt changes
        float limit = owner.TypeInfo.MaxTurnRate * owner.TypeInfo.MaxSecondOrderTurnRateFrac;
        if (Math.Abs(owner.XTurnAngle - chgx) > limit)
        {
          owner.XTurnAngle += limit * ((owner.XTurnAngle > chgx) ? -1 : 1);
        }
        else
        {
          owner.XTurnAngle = chgx;
        }

        if (Math.Abs(owner.YTurnAngle - chgy) > limit)
        {
          owner.YTurnAngle += limit * ((owner.YTurnAngle > chgy) ? -1 : 1);
        }
        else
        {
          owner.YTurnAngle = chgy;
        }

        if (isAttacking)
          return (Math.Abs(truechgx - owner.XTurnAngle) + Math.Abs(truechgy - owner.YTurnAngle));
        else
          return Math.Abs(owner.XTurnAngle) + Math.Abs(owner.YTurnAngle);
        //return (Math.Abs(owner.XTurnAngle) > Math.Abs(owner.YTurnAngle)) ? Math.Abs(owner.XTurnAngle) : Math.Abs(owner.YTurnAngle);
      }
    }

    protected float AdjustSpeed(ActorInfo owner, float target_Speed)
    {
      if (owner.ActorState != ActorState.FREE && owner.ActorState != ActorState.HYPERSPACE)
      {
        if (target_Speed > owner.MaxSpeed)
          target_Speed = owner.MaxSpeed;
        else if (target_Speed < owner.MinSpeed)
          target_Speed = owner.MinSpeed;
      }

      if (owner.Speed > target_Speed)
      {
        owner.Speed -= owner.TypeInfo.MaxSpeedChangeRate * Game.Instance().TimeSinceRender;
        if (owner.Speed < target_Speed)
          owner.Speed = target_Speed;
      }
      else
      {
        owner.Speed += owner.TypeInfo.MaxSpeedChangeRate * Game.Instance().TimeSinceRender;
        if (owner.Speed > target_Speed)
          owner.Speed = target_Speed;
      }

      return (owner.Speed - target_Speed);
    }

    protected bool CheckImminentCollision(ActorInfo owner, float scandistance, out TV_3DVECTOR vImpact, out TV_3DVECTOR vNormal)
    {
      TV_3DVECTOR start = new TV_3DVECTOR();
      TV_3DVECTOR end = new TV_3DVECTOR();
      vImpact = new TV_3DVECTOR();
      vNormal = new TV_3DVECTOR();

      if (!owner.TypeInfo.CanCheckCollisionAhead)
        return false;

      if (!owner.TypeInfo.CollisionEnabled)
        return false;

      if (scandistance <= 0)
        return false;

      start = owner.GetRelativePositionXYZ(0, 0, owner.TypeInfo.max_dimensions.z + 10);
      end = owner.GetRelativePositionXYZ(0, 0, owner.TypeInfo.max_dimensions.z + 10 + scandistance); //To revise hardcode

      if (owner.IsInProspectiveCollision)
      {
        vImpact = owner.ProspectiveCollisionImpact;
        vNormal = owner.ProspectiveCollisionNormal;
      }

      if (m_collisioncheck_time < Game.Instance().GameTime)
      {
        if (owner.IsInProspectiveCollision)
        {
          m_collisioncheck_time = Game.Instance().GameTime + 1.5f; // delay should be adjusted with FPS / CPU load, ideally every ~0.5s, but not more than 2.5s. Can be slightly longer since it is already performing evasion.
        }
        else
        {
          m_collisioncheck_time = Game.Instance().GameTime + 1; // delay should be adjusted with FPS / CPU load, ideally every run (~0.1s), but not more than 2s.
        }
        owner.ProspectiveCollisionScanDistance = scandistance;
        owner.IsTestingProspectiveCollision = true;
      }
      return owner.IsInProspectiveCollision;
    }
  }
}
