using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  // LEGACY AI CODE
  public enum AIType
  {
    NONE,
    LOCK,
    AUTO,
    IDLE,
    MOVE,
    MOVE_ACTOR,
    ATTACK,
    ATTACK_ACTOR,
    ATTACK_LOCK_ACTOR,
    ROTATE,
    HYPERSPACE_IN,
    HYPERSPACE_OUT,
    SELFDESTRUCT,
    DELETE
  }

  public class AIElement
  {
    public AIType AIType = AIType.NONE;
    public TV_3DVECTOR TargetPosition;
    public ActorInfo TargetActor;
    public bool Complete = false;
  }

  public class AIMaster
  {
    // Collision Evasion
    public bool IsCollisionEvident = false;
    public float CollisionEvasion_ActorScan = 2f;
    public float CollisionEvasion_VertexScan = 2.5f;
    public ActorInfo CollisionEvasion_BlockingActor = null;
    public TV_3DVECTOR CollisionEvasion_TargetPosition = new TV_3DVECTOR();

    // Move / Follow
    public bool IsFollowingTarget = false;
    public bool IsHyperSpace = false;
    public bool RotateOnly = false;
    public ActorInfo Move_TargetActor = null;
    public TV_3DVECTOR Move_TargetPosition = new TV_3DVECTOR();
    public bool IsUsingAlt = false;
    public TV_3DVECTOR Move_AltPosition = new TV_3DVECTOR();
    public float Move_AltCloseEnough = 500;

    // Attack
    public bool IsAttacking = false;
    public float Attack_DistanceDelta = 4000;
    public float Attack_AngularDelta = 10f;
    public float Attack_HighAccuracyDistanceDelta = 1500;
    public float Attack_HighAccuracyAngularDelta = 2.5f;

    public AIType CurrAI = AIType.NONE;
    public bool IsActive;
  }

  public class AI
  {
    private AIMaster _cmaster = null;
    public AIMaster Master
    {
      get
      {
        if (_cmaster == null)
          _cmaster = new AIMaster();
        return _cmaster;
      }
      set
      {
        _cmaster = value;
      }
    }

    public Queue<AIElement> Orders = new Queue<AIElement>();
    private AIElement _corder = null;
    public AIElement CurrentOrder
    {
      get
      {
        if (_corder == null)
          _corder = new AIElement { Complete = true };
        return _corder;
      }
      set
      {
        _corder = value;
      }
    }

    private TV_3DVECTOR target_Direction = new TV_3DVECTOR();
    private float target_Speed = 0;

    private List<TV_3DVECTOR> proximityVertices = new List<TV_3DVECTOR>();
    private TV_3DVECTOR proximitySum = new TV_3DVECTOR();
    private bool proximitySafe = true;
    private float scantime = 0;

    public void Process(ActorInfo ainfo)
    {
      // Non-combat units ignore
      if (!ainfo.IsCombatObject)
        return;

      // Retrieve next order
      while ((CurrentOrder.Complete || CurrentOrder.AIType == AIType.NONE || CurrentOrder.AIType == AIType.IDLE) && Orders.Count > 0)
      {
        CurrentOrder = Orders.Dequeue();
      }

      TranslateOrders(ainfo);
      if (CanUseAI(ainfo))
      {
        Fire(ainfo);
        Scan(ainfo);
        CalculateDir(ainfo);
        CalculateSpd(ainfo);
        Steer(ainfo);
        Speed(ainfo);
      }
    }


    private static float outerScan = 1.2f;
    private static float acceptableouterScan = 2f;
    private static float innerScan = 2.5f;

    public void ScanFront(ActorInfo ainfo)
    {
      //if (ainfo.Mesh == null)
      return;

      proximitySafe = true;

      ActorInfo[] al = ActorFactory.Instance().GetActorList();
      TV_3DVECTOR start = new TV_3DVECTOR();
      float rad = 0;
      TV_3DVECTOR tstart = new TV_3DVECTOR();
      float trad = 0;
      ainfo.GetBoundingSphere(ref start, ref rad);
      proximityVertices.Clear();
      foreach (ActorInfo a in al)
      {
        if (a != ainfo && !a.HasChild(ainfo) && !a.HasParent(ainfo) && a.TypeInfo.CollisionEnabled && a.CreationState == CreationState.ACTIVE)
        {
          a.GetBoundingSphere(ref tstart, ref trad);
          float d = ActorDistanceInfo.GetDistance(ainfo, a);
          if (d < outerScan * (rad + trad))
          {
            float cdotp = 0;
            float dotp = 0;
            foreach (TV_3DVECTOR av in a.TypeInfo.Vertices)
            {
              TV_3DVECTOR nv = a.GetRelativePositionXYZ(av.x, av.y, av.z) - ainfo.GetPosition();
              d = Engine.Instance().TVMathLibrary.GetDistanceVec3D(ainfo.GetPosition(), a.GetRelativePositionXYZ(av.x, av.y, av.z));
              if (d < innerScan * ainfo.Speed)
              {
                proximityVertices.Add(nv / d / d / d);// * a.Weight);

                TV_3DVECTOR v = new TV_3DVECTOR();
                TV_3DVECTOR dir = new TV_3DVECTOR();

                Engine.Instance().TVMathLibrary.TVVec3Normalize(ref v, nv);
                //Engine.Instance().TVMathLibrary.TVVec3Normalize(ref dir, ainfo.ProspectivePosition - ainfo.GetPosition());
                dotp = Engine.Instance().TVMathLibrary.VDotProduct(dir, v);
                if (dotp > 0.75)
                {
                  proximitySafe = false;
                  proximitySum += nv / d / d / d;

                  if (dotp > cdotp)
                  {
                    cdotp = dotp;
                    Master.CollisionEvasion_BlockingActor = a;
                  }
                }
              }
            }
          }
        }
      }
    }

    public bool IsFrontSafe(ActorInfo ainfo)
    {
      if (ainfo != PlayerInfo.Instance().Actor || !(ainfo.TypeInfo is ProjectileGroup))
      {
        return true;
      }

      if (ainfo.TypeInfo is AddOnGroup)
      {
        return true;
      }

      if (!proximitySafe)
      {
        return false;
      }

      if (Master.CollisionEvasion_BlockingActor != null
          && Master.CollisionEvasion_BlockingActor.CreationState == CreationState.ACTIVE
          )
      {
        float dist = ActorDistanceInfo.GetDistance(ainfo, Master.CollisionEvasion_BlockingActor);

        TV_3DVECTOR start = new TV_3DVECTOR();
        float rad = 0;
        TV_3DVECTOR tstart = new TV_3DVECTOR();
        float trad = 0;
        ainfo.GetBoundingSphere(ref start, ref rad);
        Master.CollisionEvasion_BlockingActor.GetBoundingSphere(ref tstart, ref trad);

        if (Master.IsCollisionEvident && dist < acceptableouterScan * (rad + trad) + 1000)
        {
          return false;
        }
      }
      return true;
    }

    public TV_3DVECTOR FindSafeDirection()
    {
      TV_3DVECTOR v = new TV_3DVECTOR();
      Engine.Instance().TVMathLibrary.TVVec3Normalize(ref v, proximitySum);
      if (v.y > 0.25f) v.y = 0.25f;
      else if (v.y < -0.25f) v.y = -0.25f;
      return -1 * v;
    }

    public bool IsMaxSpeedSafe(ActorInfo ainfo)
    {
      TV_3DVECTOR v = new TV_3DVECTOR();
      TV_3DVECTOR vp = new TV_3DVECTOR();
      Engine.Instance().TVMathLibrary.TVVec3Normalize(ref v, FindSafeDirection());
      Engine.Instance().TVMathLibrary.TVVec3Normalize(ref vp, ainfo.GetDirection());
      float dotv = Engine.Instance().TVMathLibrary.VDotProduct(vp, v);

      return (dotv > 0.5);
    }

    private bool CanUseAI(ActorInfo ainfo)
    {
      return (Master != null
              && Master.IsActive
              && (ainfo != PlayerInfo.Instance().Actor || PlayerInfo.Instance().PlayerAIEnabled)
              && ainfo.CreationState == CreationState.ACTIVE);
              //&& ainfo.Mesh != null);
    }

    private float m_autoRefreshTime = 0;
    public void TranslateOrders(ActorInfo ainfo)
    {
      // special case
      if (CurrentOrder.AIType == AIType.SELFDESTRUCT)
      {
        Master.IsActive = false;
        ainfo.ActorState = ActorState.DEAD;
      }
      else if (CurrentOrder.AIType == AIType.DELETE)
      {
        Master.IsActive = false;
        ainfo.Destroy();
        return;
      }

      if (ainfo.Faction != null 
        && ainfo.Faction.AutoAI 
        && (ainfo != PlayerInfo.Instance().Actor || PlayerInfo.Instance().PlayerAIEnabled)
        && (m_autoRefreshTime < Game.Instance().GameTime || CurrentOrder.AIType == AIType.IDLE))
      {
        switch (CurrentOrder.AIType)
        {
          case AIType.ATTACK_ACTOR:
            m_autoRefreshTime = Game.Instance().GameTime + ((ainfo.TypeInfo is AddOnGroup) ? 4f : 25f);
            CurrentOrder.AIType = AIType.AUTO;
            break;

          case AIType.AUTO:
          case AIType.IDLE:
            m_autoRefreshTime = Game.Instance().GameTime + ((ainfo.TypeInfo is AddOnGroup) ? 2f : 5f);
            CurrentOrder.AIType = AIType.AUTO;
            break;

          case AIType.NONE:
          case AIType.LOCK:
          case AIType.MOVE:
          case AIType.MOVE_ACTOR:
          case AIType.ATTACK_LOCK_ACTOR:
          case AIType.ROTATE:
          case AIType.ATTACK:
          case AIType.HYPERSPACE_IN:
          case AIType.HYPERSPACE_OUT:
          default:
            break; // Do not interrupt
        }
      }

      switch (CurrentOrder.AIType)
      {
        case AIType.NONE:
          Master.IsActive = false;
          break;
        case AIType.LOCK:
          Master.IsActive = false;
          ainfo.XTurnAngle = 0;
          ainfo.YTurnAngle = 0;
          break;
        case AIType.AUTO:
          AutoOrder(ainfo);
          break;
        case AIType.IDLE:
          Master.IsActive = true;
          Master.IsAttacking = false;
          Master.IsFollowingTarget = false;
          Master.RotateOnly = false;
          Master.IsHyperSpace = false;
          if (Master.CurrAI != AIType.IDLE)
          {
            Master.Move_TargetPosition = ainfo.GetPosition();
          }
          break;
        case AIType.MOVE:
          Master.IsActive = true;
          Master.IsAttacking = false;
          Master.IsFollowingTarget = false;
          Master.RotateOnly = false;
          Master.IsHyperSpace = false;
          if (Master.CurrAI != AIType.MOVE)
          {
            Master.Move_TargetPosition = CurrentOrder.TargetPosition;
          }
          break;
        case AIType.MOVE_ACTOR:
          Master.IsActive = true;
          Master.IsAttacking = false;
          Master.IsFollowingTarget = true;
          Master.RotateOnly = false;
          Master.IsHyperSpace = false;
          if (Master.CurrAI != AIType.MOVE_ACTOR)
          {
            Master.Move_TargetActor = CurrentOrder.TargetActor;
          }
          else
          {
            if (Master.Move_TargetActor == null
              || Master.Move_TargetActor.CreationState != CreationState.ACTIVE
              || Master.Move_TargetActor.ActorState == ActorState.DEAD
              || Master.Move_TargetActor.ActorState == ActorState.DYING)
            {
              CurrentOrder.Complete = true;
              Orders.Enqueue(new AIElement { AIType = AIType.IDLE });
            }
          }
          break;
        case AIType.ROTATE:
          Master.IsActive = true;
          Master.IsAttacking = false;
          Master.IsFollowingTarget = false;
          Master.RotateOnly = true;
          Master.IsHyperSpace = false;
          if (Master.CurrAI != AIType.ROTATE)
          {
            Master.Move_TargetPosition = CurrentOrder.TargetPosition;
          }
          break;
        case AIType.HYPERSPACE_IN:
        case AIType.HYPERSPACE_OUT:
          Master.IsActive = true;
          Master.IsAttacking = false;
          Master.IsFollowingTarget = false;
          Master.RotateOnly = false;
          Master.IsHyperSpace = true;
          if (Master.CurrAI != AIType.HYPERSPACE_IN || Master.CurrAI != AIType.HYPERSPACE_OUT)
          {
            if (CurrentOrder.AIType == AIType.HYPERSPACE_IN)
            {
              Master.Move_TargetPosition = CurrentOrder.TargetPosition;
            }
            else
            {
              Master.Move_TargetPosition = ainfo.GetRelativePositionXYZ(0, 0, 50000);
            }
          }
          break;
        case AIType.ATTACK:
          Master.IsActive = true;
          Master.IsAttacking = true;
          Master.IsFollowingTarget = false;
          Master.RotateOnly = false;
          Master.IsHyperSpace = false;
          Master.Move_TargetPosition = CurrentOrder.TargetPosition;
          break;
        case AIType.ATTACK_ACTOR:
        case AIType.ATTACK_LOCK_ACTOR:
          Master.IsActive = true;
          Master.IsAttacking = true;
          Master.IsFollowingTarget = true;
          Master.RotateOnly = false;
          Master.IsHyperSpace = false;
          if (Master.CurrAI != AIType.ATTACK_ACTOR && Master.CurrAI != AIType.ATTACK_LOCK_ACTOR)
          {
            Master.Move_TargetActor = CurrentOrder.TargetActor;
          }
          else
          {
            if (Master.Move_TargetActor == null
              || Master.Move_TargetActor.CreationState != CreationState.ACTIVE
              || Master.Move_TargetActor.ActorState == ActorState.DEAD
              || Master.Move_TargetActor.ActorState == ActorState.DYING)
            {
              Master.IsAttacking = false;
              Master.IsUsingAlt = false;
              m_autoRefreshTime = 0;
              CurrentOrder.Complete = true;
              Orders.Enqueue(new AIElement { AIType = AIType.IDLE });
            }
          }
          break;
      }

      if (ainfo.TypeInfo is TIE_X1_ATI)
      {

      }

      Master.Attack_AngularDelta = ainfo.TypeInfo.Attack_AngularDelta;
      Master.Attack_HighAccuracyAngularDelta = ainfo.TypeInfo.Attack_HighAccuracyAngularDelta;

      if (!Master.IsUsingAlt)
      {
        Master.Move_AltCloseEnough = ainfo.TypeInfo.Move_CloseEnough;
      }

      if (Master.CurrAI != CurrentOrder.AIType)
      {
        Master.CurrAI = CurrentOrder.AIType;
      }


      //is out of bounds?
      if (!(ainfo.TypeInfo is ProjectileGroup) && ainfo.IsOutOfBounds(GameScenarioManager.Instance().MinBounds * 0.75f, GameScenarioManager.Instance().MaxBounds * 0.75f) && ainfo.EnteredCombatZone)
      {
        float x = Engine.Instance().Random.Next((int)(GameScenarioManager.Instance().MinBounds.x * 0.65f), (int)(GameScenarioManager.Instance().MaxBounds.x * 0.65f));
        float y = Engine.Instance().Random.Next(-200, 200);
        float z = Engine.Instance().Random.Next((int)(GameScenarioManager.Instance().MinBounds.z * 0.65f), (int)(GameScenarioManager.Instance().MaxBounds.z * 0.65f));

        Master.Move_AltPosition = new TV_3DVECTOR(x, y, z);
        Master.CurrAI = AIType.MOVE;
        CurrentOrder.TargetPosition = new TV_3DVECTOR(x, y, z);
        CurrentOrder.AIType = AIType.MOVE;
      }
      else
      {
        if (!ainfo.IsOutOfBounds(GameScenarioManager.Instance().MinBounds * 0.75f, GameScenarioManager.Instance().MaxBounds * 0.75f))
        {
          ainfo.EnteredCombatZone = true;
        }
      }
    }

    public void AutoOrder(ActorInfo ainfo)
    {
      // excludes
      if (!(ainfo.TypeInfo is FighterGroup || ainfo.TypeInfo is TIEGroup || ainfo.TypeInfo is AddOnGroup || ainfo.TypeInfo is ProjectileGroup) || !(PlayerInfo.Instance().Actor != ainfo || PlayerInfo.Instance().PlayerAIEnabled))
      {
        return;
      }

      Master.IsUsingAlt = false;

      //target
      float currdist = (ainfo.TypeInfo is AddOnGroup) ? Master.Attack_DistanceDelta : 7500;
      ActorInfo currtarget = null;
      List<ActorInfo> targets = new List<ActorInfo>();
      foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
      {
        if (a != null
          && ainfo != a
          && a.CreationState == CreationState.ACTIVE
          && a.ActorState != ActorState.DYING
          && a.ActorState != ActorState.DEAD
          && a.IsCombatObject
          && !ainfo.Faction.IsAlliedWith(a.Faction) // enemy
          )
        {
          if (ainfo.TypeInfo is AddOnGroup)
          {
            float dist = ActorDistanceInfo.GetDistance(ainfo, a);

            if (dist < currdist)
            {
              targets.Add(a);
              //currtarget = a;
              //currdist = dist;
            }
          }
          else
          {
            targets.Add(a);
          }
        }
      }

      if (targets.Count > 0)
      {
        int i = Engine.Instance().Random.Next(0, targets.Count);
        currtarget = targets[i];
      }

      if (currtarget != null)
      {
        AIElement neworder = new AIElement
        {
          AIType = AIType.ATTACK_ACTOR,
          TargetActor = currtarget
        };
        CurrentOrder.Complete = true;
        Orders.Enqueue(neworder);
      }
    }

    public void Scan(ActorInfo ainfo)
    {
      if (Master.IsHyperSpace)
      {
        return;
      }
      if (scantime < Game.Instance().GameTime)
      {
        ScanFront(ainfo);
        scantime = Game.Instance().GameTime + 0.25f + (float)Engine.Instance().Random.NextDouble() * 0.5f;
        if (!IsFrontSafe(ainfo))
        {
          Master.IsCollisionEvident = true;
          Master.CollisionEvasion_TargetPosition = ainfo.GetPosition() + FindSafeDirection() * 1500;
        }
        else
        {
          Master.IsCollisionEvident = false;
        }
      }
    }

    public void CalculateDir(ActorInfo ainfo)
    {
      if (Master.IsHyperSpace)
      {
        return;
      }
      if (Master.IsCollisionEvident)
      {
        target_Direction = Master.CollisionEvasion_TargetPosition - ainfo.GetPosition();

        //float d = Engine.Instance().TVMathLibrary.GetDistanceVec3D(ainfo.Position, Master.CollisionEvasion_TargetPosition);

        //if (d < 750)
        //{
        //  Master.IsCollisionEvident = false;
        //}
      }
      else if (Master.IsUsingAlt)
      {
        float d = Engine.Instance().TVMathLibrary.GetDistanceVec3D(ainfo.GetPosition(), Master.Move_AltPosition);

        target_Direction = Master.Move_AltPosition - ainfo.GetPosition();

        if (d < Master.Move_AltCloseEnough)
        {
          Master.IsUsingAlt = false;
        }
        else
        {
          Master.Move_AltCloseEnough += 100 * Game.Instance().TimeSinceRender;
        }
      }
      else if (Master.IsFollowingTarget && Master.Move_TargetActor != null)
      {
        if (Master.IsAttacking && Master.Move_TargetActor.CreationState == CreationState.ACTIVE)
        {
          // Anticipate
          float dist = ActorDistanceInfo.GetDistance(ainfo, Master.Move_TargetActor);
          float d = dist / Globals.LaserSpeed; // Laser Speed

          TV_3DVECTOR target = Master.Move_TargetActor.GetPosition();
          //TV_3DVECTOR target = Master.Move_TargetActor.GetRelativePositionXYZ(0, 0, ((Master.Move_TargetActor.AttachToMesh > -1 && Master.Move_TargetActor.Parent.Count > 0) ? Master.Move_TargetActor.Parent[0].Speed : Master.Move_TargetActor.Speed) * d);
          target_Direction = target - ainfo.GetPosition();

          if (dist < 2f * ainfo.TypeInfo.MaxSpeed)
          {
            Master.IsUsingAlt = true;
            float x = ainfo.GetPosition().x + Engine.Instance().Random.Next((int)(-4f * ainfo.TypeInfo.MaxSpeed), (int)(4f * ainfo.TypeInfo.MaxSpeed));
            if ((int)(4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().x > GameScenarioManager.Instance().MaxBounds.x * 0.75f)
            {
              x -= (int)(4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().x - GameScenarioManager.Instance().MaxBounds.x * 0.75f;
            }
            else if ((int)(-4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().x < GameScenarioManager.Instance().MinBounds.x * 0.75f)
            {
              x += (int)(-4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().x - GameScenarioManager.Instance().MinBounds.x * 0.75f;
            }

            float y = Engine.Instance().Random.Next(-200, 200);

            float z = ainfo.GetPosition().z + Engine.Instance().Random.Next((int)(-4f * ainfo.TypeInfo.MaxSpeed), (int)(4f * ainfo.TypeInfo.MaxSpeed));
            if ((int)(4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().z > GameScenarioManager.Instance().MaxBounds.z * 0.75f)
            {
              z -= (int)(4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().z - GameScenarioManager.Instance().MaxBounds.z * 0.75f;
            }
            else if ((int)(-4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().z < GameScenarioManager.Instance().MinBounds.z * 0.75f)
            {
              z += (int)(-4f * ainfo.TypeInfo.MaxSpeed) + ainfo.GetPosition().z - GameScenarioManager.Instance().MinBounds.z * 0.75f;
            }

            Master.Move_AltPosition = new TV_3DVECTOR(x, y, z);
          }
          else
          {
            TV_3DVECTOR rot = ainfo.GetRotation();
            TV_3DVECTOR tgtrot = Utilities.GetRotation(target_Direction);

            float chgx = tgtrot.x - rot.x;
            float chgy = tgtrot.y - rot.y;

            while (chgy < -180)
              chgy += 360;

            while (chgy > 180)
              chgy -= 360;

            /*if (chgy > 90f || chgy < -90f)
            {
              Master.IsUsingAlt = true;
              Master.Move_AltPosition = Master.Move_TargetActor.Mesh.GetWorldPosition(new TV_3DVECTOR(Engine.Instance().Random.Next(-50, 50), Engine.Instance().Random.Next(-50, 50), -2.5f * ainfo.TypeInfo.MaxSpeed));
              target_Direction = Master.Move_AltPosition;
            }
            */
          }
        }
        else
        {
          target_Direction = Master.Move_TargetActor.GetPosition() - ainfo.GetPosition();
        }
      }
      else
      {
        target_Direction = Master.Move_TargetPosition - ainfo.GetPosition();
      }

      /*
      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_Line3D(
        ainfo.Position.x,
        ainfo.Position.y,
        ainfo.Position.z,
        (ainfo.Position + target_Direction * 10).x,
        (ainfo.Position + target_Direction * 10).y,
        (ainfo.Position + target_Direction * 10).z,
        new TV_COLOR((Master.IsCollisionEvident) ? 1 : 0
                      , (Master.IsFollowingTarget) ? 1 : 0
                      , (Master.IsUsingAlt) ? 1 : 0
                      , 1).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();
      */
    }

    public void CalculateSpd(ActorInfo ainfo)
    {
      if (Master.IsHyperSpace)
      {
        float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(Master.Move_TargetPosition, ainfo.GetPosition());

        if (dist < Master.Move_AltCloseEnough)
        {
          ainfo.ActorState = ActorState.NORMAL;
          CurrentOrder.Complete = true;
          Orders.Enqueue(new AIElement { AIType = AIType.IDLE });
        }
        else
        {
          ainfo.ActorState = ActorState.HYPERSPACE;
        }
      }
      else if (Master.IsCollisionEvident)
      {
        if (IsMaxSpeedSafe(ainfo))
        {
          target_Speed = ainfo.TypeInfo.MaxSpeed;
        }
        else
        {
          target_Speed = ainfo.TypeInfo.MinSpeed;
        }
      }
      else if (Master.IsUsingAlt)
      {
        target_Speed = ainfo.TypeInfo.MaxSpeed;
      }
      else if (Master.IsFollowingTarget && Master.Move_TargetActor != null)
      {
        if (Master.IsAttacking)
        {
          if ((ainfo.YTurnAngle > 60f && ainfo.YTurnAngle < 120f) || (ainfo.YTurnAngle < -60f && ainfo.YTurnAngle > -120f))
          {
            target_Speed = ainfo.TypeInfo.MinSpeed;
          }
          else
          {
            target_Speed = ainfo.TypeInfo.MaxSpeed;
          }
        }
        else
        {
          target_Speed = ActorDistanceInfo.GetDistance(Master.Move_TargetActor, ainfo);
          if (target_Speed > ainfo.TypeInfo.MaxSpeed)
            target_Speed = ainfo.TypeInfo.MaxSpeed;

          if (target_Speed < ainfo.TypeInfo.MinSpeed)
            target_Speed = ainfo.TypeInfo.MinSpeed;
        }
      }
      else
      {
        if (Master.RotateOnly || Master.CurrAI == AIType.IDLE)
        {
          target_Speed = ainfo.TypeInfo.MinSpeed;
        }
        else
        {
          target_Speed = Engine.Instance().TVMathLibrary.GetDistanceVec3D(Master.Move_TargetPosition, ainfo.GetPosition());
        }

        if (target_Speed > ainfo.TypeInfo.MaxSpeed)
          target_Speed = ainfo.TypeInfo.MaxSpeed;

        if (target_Speed < ainfo.TypeInfo.MinSpeed)
          target_Speed = ainfo.TypeInfo.MinSpeed;

        //if (ainfo.TypeInfo is WarshipGroup || ainfo.TypeInfo is StarDestroyerGroup)
        //{
          float dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(Master.Move_TargetPosition, ainfo.GetPosition());
          if (dist < Master.Move_AltCloseEnough)
          {
            CurrentOrder.Complete = true;
            Orders.Enqueue(new AIElement { AIType = AIType.IDLE });
            //target_Speed = ainfo.TypeInfo.MinSpeed;
            //ainfo.XTurnAngle = 0;
            //ainfo.YTurnAngle = 0;
          }
        //}
      }
    }

    public void Steer(ActorInfo ainfo)
    {
      if (ainfo.TypeInfo.MaxTurnRate == 0)
      {
        return;
      }

      TV_3DVECTOR rot = ainfo.GetRotation();
      //TVMesh temp = Engine.Instance().TVScene.CreateMeshBuilder();
      //temp.SetPosition(ainfo.Position.x, ainfo.Position.y, ainfo.Position.z);
      //temp.LookAtPoint(target_Direction + ainfo.Position);
      TV_3DVECTOR tgtrot = Utilities.GetRotation(target_Direction); //temp.GetRotation();
      //temp.Destroy();

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

      if (chgx > ainfo.TypeInfo.MaxTurnRate)
        chgx = ainfo.TypeInfo.MaxTurnRate;
      else if (chgx < -ainfo.TypeInfo.MaxTurnRate)
        chgx = -ainfo.TypeInfo.MaxTurnRate;

      if (chgy > ainfo.TypeInfo.MaxTurnRate)
        chgy = ainfo.TypeInfo.MaxTurnRate;
      else if (chgy < -ainfo.TypeInfo.MaxTurnRate)
        chgy = -ainfo.TypeInfo.MaxTurnRate;

      // limit abrupt changes
      float limit = ainfo.TypeInfo.MaxTurnRate * ainfo.TypeInfo.MaxSecondOrderTurnRateFrac;
      if (Math.Abs(ainfo.XTurnAngle - chgx) > limit)
      {
        ainfo.XTurnAngle += limit * ((ainfo.XTurnAngle > chgx) ? -1 : 1);
      }
      else
      {
        ainfo.XTurnAngle = chgx;
      }

      if (Math.Abs(ainfo.YTurnAngle - chgy) > limit)
      {
        ainfo.YTurnAngle += limit * ((ainfo.YTurnAngle > chgy) ? -1 : 1);
      }
      else
      {
        ainfo.YTurnAngle = chgy;
      }

      if (Master.RotateOnly && Math.Abs(chgx) < 0.1f && Math.Abs(chgy) < 0.1f && ainfo.Speed == ainfo.TypeInfo.MinSpeed)
      {
        CurrentOrder.Complete = true;
        Orders.Enqueue(new AIElement { AIType = AIType.IDLE });
      }
    }

    public void Speed(ActorInfo ainfo)
    {
      if (Master.IsHyperSpace)
      {
        return;
      }
      if (ainfo.Speed == 0)
      {
        return;
      }

      if (ainfo.Speed > target_Speed)
      {
        ainfo.Speed -= ainfo.TypeInfo.MaxSpeedChangeRate * Game.Instance().TimeSinceRender;
        if (ainfo.Speed < target_Speed)
          ainfo.Speed = target_Speed;
      }
      else
      {
        ainfo.Speed += ainfo.TypeInfo.MaxSpeedChangeRate * Game.Instance().TimeSinceRender;
        if (ainfo.Speed > target_Speed)
          ainfo.Speed = target_Speed;
      }
    }

    public void Fire(ActorInfo ainfo)
    {
      if (Master.IsHyperSpace)
      {
        return;
      }
      //if (!ainfo.IsWeaponReady())
      //{
      //  return;
      //}

      if (Master.IsAttacking && ((!Master.IsUsingAlt && !Master.IsCollisionEvident) || ainfo.TypeInfo.AggressiveTracker))
      {
        float dist = 0;
        float angle_x = 0;
        float angle_y = 0;

        if (ainfo.TypeInfo is AddOnGroup)
        {
          //Master.Move_TargetPosition = target_Direction;
        }

        ActorInfo temp = Master.Move_TargetActor;
        if (Master.IsFollowingTarget)
        {
          if (ainfo.TypeInfo.AggressiveTracker)
          {
            foreach (ActorInfo a in ActorFactory.Instance().GetActorList())
            {
              if (a != null
                  && ainfo != a
                  && a.CreationState == CreationState.ACTIVE
                  && a.ActorState != ActorState.DYING
                  && a.ActorState != ActorState.DEAD
                  && a.IsCombatObject
                  && !ainfo.Faction.IsAlliedWith(a.Faction))
              {
                Master.Move_TargetActor = a;
                dist = ActorDistanceInfo.GetDistance(Master.Move_TargetActor, ainfo);
                TV_3DVECTOR rot = ainfo.GetRotation();
                TV_3DVECTOR tgtrot = Utilities.GetRotation(Master.Move_TargetActor.GetPosition() - ainfo.GetPosition());

                angle_x = tgtrot.x - rot.x;
                angle_y = tgtrot.y - rot.y;

                while (angle_x < -180)
                  angle_x += 360;

                while (angle_x > 180)
                  angle_x -= 360;

                while (angle_y < -180)
                  angle_y += 360;

                while (angle_y > 180)
                  angle_y -= 360;

                if (dist < Master.Attack_HighAccuracyDistanceDelta && Math.Abs(angle_x) < Master.Attack_HighAccuracyAngularDelta && Math.Abs(angle_y) < Master.Attack_HighAccuracyAngularDelta)
                {
                  ainfo.FireWeapon(Master.Move_TargetActor, "auto");
                  Master.Move_TargetActor = temp;
                  return;
                }
                else if (dist < Master.Attack_DistanceDelta && Math.Abs(angle_x) < Master.Attack_AngularDelta && Math.Abs(angle_y) < Master.Attack_AngularDelta)
                {
                  ainfo.FireWeapon(Master.Move_TargetActor, "auto");
                  Master.Move_TargetActor = temp;
                  return;
                }
              }
            }
          }
          else
          {
            dist = ActorDistanceInfo.GetDistance(Master.Move_TargetActor, ainfo);

            if (dist < Master.Attack_HighAccuracyDistanceDelta && Math.Abs(ainfo.XTurnAngle) < Master.Attack_HighAccuracyAngularDelta && Math.Abs(ainfo.YTurnAngle) < Master.Attack_HighAccuracyAngularDelta)
            {
              ainfo.FireWeapon(Master.Move_TargetActor, "auto");
            }
            else if (dist < Master.Attack_DistanceDelta && Math.Abs(ainfo.XTurnAngle) < Master.Attack_AngularDelta && Math.Abs(ainfo.YTurnAngle) < Master.Attack_AngularDelta)
            {
              ainfo.FireWeapon(Master.Move_TargetActor, "auto");
            }
          }
        }
        else
        {
          dist = Engine.Instance().TVMathLibrary.GetDistanceVec3D(Master.Move_TargetPosition, ainfo.GetPosition());

          if (dist < Master.Attack_HighAccuracyDistanceDelta && Math.Abs(ainfo.XTurnAngle) < Master.Attack_HighAccuracyAngularDelta && Math.Abs(ainfo.YTurnAngle) < Master.Attack_HighAccuracyAngularDelta)
          {
            ainfo.FireWeapon(Master.Move_TargetActor, "auto");
            return;
          }
          else if (dist < Master.Attack_DistanceDelta && Math.Abs(ainfo.XTurnAngle) < Master.Attack_AngularDelta && Math.Abs(ainfo.YTurnAngle) < Master.Attack_AngularDelta)
          {
            ainfo.FireWeapon(Master.Move_TargetActor, "auto");
            return;
          }
        }
      }
    }

    /*
    public void ProcessHit(ActorInfo ainfo, ActorInfo hitby)
    {
      if (ainfo.TypeInfo.IsDamage)
      {
        return;
      }

      if (!(ainfo.TypeInfo is FighterGroup || ainfo.TypeInfo is TIEGroup))
      {
        return;
      }

      if (hitby.TypeInfo.IsDamage && hitby.Parent.Count > 0)
      {
        if (Master.CurrAI == AIType.ATTACK
          || Master.CurrAI == AIType.ATTACK_ACTOR
          || Master.CurrAI == AIType.IDLE
          || Master.CurrAI == AIType.AUTO)
        {
          if (ainfo.Faction != null && !ainfo.Faction.IsAlliedWith(hitby.Parent[0].Faction))
          {
            foreach (AIElement ai in ainfo.AI.Orders)
            {
              ai.Complete = true;
            }
            ainfo.AI.Orders.Enqueue(new AIElement { AIType = AIType.ATTACK_ACTOR, TargetActor = hitby.Parent[0] });

            m_autoRefreshTime = Game.Instance().Time + 15f;
          }
        }
      }
    }
    */
  }
}
