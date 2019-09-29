using System;
using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Primitives;
using SWEndor.Actors.Models;
using SWEndor.Models;

namespace SWEndor.Actors.Data
{
  public struct CollisionData
  {
    public bool IsTestingCollision;
    public CollisionResultData Collision;

    public bool IsTestingProspectiveCollision;
    public bool IsInProspectiveCollision;

    public CollisionResultData ProspectiveCollision;

    public TV_3DVECTOR ProspectiveCollisionSafe;
    public float ProspectiveCollisionScanDistance;
    public float ProspectiveCollisionLevel;

    public bool IsAvoidingCollision;

    public void Init()
    {
      Reset();
    }

    public void Reset()
    {
      IsTestingCollision = false;

      Collision.Reset();

      IsTestingProspectiveCollision = false;
      IsInProspectiveCollision = false;

      ProspectiveCollision.Reset();

      ProspectiveCollisionSafe = new TV_3DVECTOR();
      ProspectiveCollisionScanDistance = 1000;
      ProspectiveCollisionLevel = 0;
      IsAvoidingCollision = false;
    }

    internal void CheckCollision(Engine engine, ActorInfo actor)
    {
      IsTestingCollision = false;
      // only check player and projectiles
      if (actor.IsPlayer
          || actor.TypeInfo is ActorTypes.Groups.Projectile
          || (actor.IsDying && actor.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER)))
      {
        if (Collision.ActorID >= 0)
        {
          ActorInfo a = engine.ActorFactory.Get(Collision.ActorID);
          if (a != null)
          {
            if (actor.IsPlayer
                  && engine.PlayerInfo.PlayerAIEnabled)
            {
              engine.Screen2D.MessageSecondaryText(string.Format("DEV WARNING: PLAYER AI COLLIDED: {0}", a.ToString()), 1.5f, new TV_COLOR(1, 0.2f, 0.2f, 1), 99999);
              Collision.ActorID = -1;
              IsTestingCollision = true;
              return;
            }

            a.TypeInfo.ProcessHit(engine, a, actor, Collision.Impact, Collision.Normal);
            actor.TypeInfo.ProcessHit(engine, actor, a, Collision.Impact, -1 * Collision.Normal);
          }
          Collision.ActorID = -1;
        }
        IsTestingCollision = true;
      }
    }

    internal void TestCollision(Engine engine, ActorInfo actor)
    {
      if (IsTestingCollision)
      {
        TV_3DVECTOR vmin = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.MeshData.MaxDimensions.z, false);
        TV_3DVECTOR vmax = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.MeshData.MinDimensions.z, false) - (actor.GetGlobalPosition() - actor.GetPrevGlobalPosition());

        TestCollision(engine, actor, vmin, vmax, false, out Collision.Impact, out Collision.Normal, out Collision.ActorID);
        IsTestingCollision = false;
      }
      if (IsTestingProspectiveCollision)
      {
        int dummy;

        TV_3DVECTOR prostart = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.MeshData.MaxDimensions.z + 10);
        TV_3DVECTOR proend0 = actor.GetRelativePositionXYZ(0 //(float)Math.Sin(actor.MoveData.YTurnAngle * Globals.PI / 180) * ProspectiveCollisionScanDistance  //* actor.MoveData.Speed
                                                         , 0 //-(float)Math.Sin(actor.MoveData.XTurnAngle * Globals.PI / 180) * ProspectiveCollisionScanDistance  //* actor.MoveData.Speed
                                                         , actor.TypeInfo.MeshData.MaxDimensions.z + 10 + ProspectiveCollisionScanDistance);

        TV_3DVECTOR proImpact = new TV_3DVECTOR();
        TV_3DVECTOR proNormal = new TV_3DVECTOR();

        TV_3DVECTOR _Impact = new TV_3DVECTOR();
        TV_3DVECTOR _Normal = new TV_3DVECTOR();
        int count = 0;

        IsInProspectiveCollision = false;

        TestCollision(engine, actor, prostart, proend0, true, out _Impact, out _Normal, out ProspectiveCollision.ActorID);

        if (ProspectiveCollision.ActorID >= 0)
        {
          proImpact += _Impact;
          proNormal += _Normal;
          count++;
          IsInProspectiveCollision = true;
          ProspectiveCollisionSafe = _Impact + _Normal * 10000;
        }

        if (IsInProspectiveCollision || IsAvoidingCollision)
          ProspectiveCollisionLevel = 0;
        bool nextlevel = true;
        float dist = 0;
        while (nextlevel && ProspectiveCollisionLevel < 5)
        {
          ProspectiveCollisionLevel++;
          nextlevel = true;
          for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
              if (i == 0 && j == 0)
                continue;

              proend0 = actor.GetRelativePositionXYZ(i * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                             , j * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                             , actor.TypeInfo.MeshData.MaxDimensions.z + 10 + ProspectiveCollisionScanDistance);
              TestCollision(engine, actor, prostart, proend0, true, out _Impact, out _Normal, out dummy);

              if (dummy >= 0)
              {
                proImpact += _Impact;
                proNormal += _Normal;
                count++;
                float newdist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.Position, _Impact);
                if (dist < newdist)
                {
                  dist = newdist;
                  ProspectiveCollisionSafe = actor.Position + (proend0 - actor.Position) * 1000;
                  if (IsAvoidingCollision)
                    nextlevel = false;
                }
              }
              else
              {
                dist = float.MaxValue;
                if (!IsAvoidingCollision)
                  nextlevel = false;
                ProspectiveCollisionSafe = actor.Position + (proend0 - actor.Position) * 1000;
              }
            }
        }

        if (count > 0)
        {
          ProspectiveCollision.Impact = proImpact / count;
          ProspectiveCollision.Normal = proNormal / count;
        }
        IsTestingProspectiveCollision = false;
      }
    }

    private static void TestCollision(Engine engine, ActorInfo actor, TV_3DVECTOR start, TV_3DVECTOR end, bool isProspective, out TV_3DVECTOR vImpact, out TV_3DVECTOR vNormal, out int CollisionActorID)
    {
      try
      {
        CollisionActorID = -1;
        vImpact = new TV_3DVECTOR();
        vNormal = new TV_3DVECTOR();

        TV_COLLISIONRESULT tvcres = new TV_COLLISIONRESULT();

        bool result = false;
        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
          result = engine.TrueVision.TVScene.AdvancedCollision(start, end, ref tvcres);

        if (result)
        {
          if (tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_MESH)
            return;

          vImpact = new TV_3DVECTOR(tvcres.vCollisionImpact.x, tvcres.vCollisionImpact.y, tvcres.vCollisionImpact.z);
          vNormal = new TV_3DVECTOR(tvcres.vCollisionNormal.x, tvcres.vCollisionNormal.y, tvcres.vCollisionNormal.z);

          if (!isProspective)
          {
            int checkID = MeshModel.GetID(tvcres.iMeshID);
            if (checkID != -1)
            {
              ActorInfo checkActor = engine.ActorFactory.Get(checkID);
              if (checkActor != null
                   && actor.TopParent != checkActor.TopParent
                   && checkActor.Mask.Has(ComponentMask.CAN_BECOLLIDED)
                   && !checkActor.IsAggregateMode
                   )
              {
                CollisionActorID = checkID;
              }
            }
          }
          else
          {
            int checkID = MeshModel.GetID(tvcres.iMeshID);
            if (checkID != -1)
            {
              ActorInfo checkActor = engine.ActorFactory.Get(checkID);
              if (checkActor != null
                   && !(checkActor.TypeInfo is ActorTypes.Groups.Fighter)
                   )
              {
                CollisionActorID = checkID;
              }
            }
          }
        }
      }
      catch
      {
        CollisionActorID = -1;
        vImpact = new TV_3DVECTOR();
        vNormal = new TV_3DVECTOR();
      }
    }
  }
}
