using MTV3D65;
using SWEndor.Core;
using Primrose.Primitives;
using SWEndor.Actors.Models;
using SWEndor.Actors;

namespace SWEndor.Models
{
  public struct CollisionModel<T>
    where T :
    IMeshObject, 
    ITransformable,
    ICollidable
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

    internal void CheckCollision(Engine engine, T actor)
    {
      IsTestingCollision = false;
      if (Collision.ActorID >= 0)
      {
        ActorInfo a = engine.ActorFactory.Get(Collision.ActorID);
        if (a != null)
          actor.DoCollide(a, ref Collision);
        Collision.ActorID = -1;
      }
      IsTestingCollision = true;
    }

    internal void TestCollision(Engine engine, T actor)
    {
      if (IsTestingCollision)
      {
        if (Collision.End.x == 0)
          Collision.End = actor.GetRelativePositionXYZ(0, 0, actor.MinDimensions.z, false) - (actor.GetGlobalPosition() - actor.GetPrevGlobalPosition());

        Collision.Start = actor.GetRelativePositionXYZ(0, 0, actor.MaxDimensions.z, false);
        //TV_3DVECTOR vmin = 
        //TV_3DVECTOR vmax = prevCollisionStart;
        //prevCollisionStart = actor.GetRelativePositionXYZ(0, 0, actor.MinDimensions.z, false) - (actor.GetGlobalPosition() - actor.GetPrevGlobalPosition());

        TestCollision(engine, actor, false, ref Collision);
        IsTestingCollision = false;
      }
      if (IsTestingProspectiveCollision)
      {
        ProspectiveCollision.Start = actor.GetRelativePositionXYZ(0, 0, actor.MaxDimensions.z + 10);
        ProspectiveCollision.End = actor.GetRelativePositionXYZ(0, 0, actor.MaxDimensions.z + 10 + ProspectiveCollisionScanDistance);

        TV_3DVECTOR proImpact = new TV_3DVECTOR();
        TV_3DVECTOR proNormal = new TV_3DVECTOR();

        int count = 0;

        IsInProspectiveCollision = false;

        TestCollision(engine, actor, true, ref ProspectiveCollision);

        if (ProspectiveCollision.ActorID >= 0)
        {
          proImpact += ProspectiveCollision.Impact;
          proNormal += ProspectiveCollision.Normal;
          count++;
          IsInProspectiveCollision = true;
          ProspectiveCollisionSafe = ProspectiveCollision.Impact + ProspectiveCollision.Normal * 10000;
        }

        if (IsInProspectiveCollision || IsAvoidingCollision)
          ProspectiveCollisionLevel = 0;
        float dist = 0;
        while (ProspectiveCollisionLevel < 5)
        {
          ProspectiveCollisionLevel++;
          for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
              if (i == 0 && j == 0)
                continue;

              ProspectiveCollision.End = actor.GetRelativePositionXYZ(i * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                                                    , j * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                                                    , actor.MaxDimensions.z + 10 + ProspectiveCollisionScanDistance);
              TestCollision(engine, actor, true, ref ProspectiveCollision);

              if (ProspectiveCollision.ActorID >= 0)
              {
                proImpact += ProspectiveCollision.Impact;
                proNormal += ProspectiveCollision.Normal;
                count++;
                float newdist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.Position, ProspectiveCollision.Impact);
                if (dist < newdist)
                {
                  dist = newdist;
                  ProspectiveCollisionSafe = actor.Position + (ProspectiveCollision.End - actor.Position) * 1000;
                  if (IsAvoidingCollision)
                    break;
                }
              }
              else
              {
                if (!IsAvoidingCollision)
                  break;
                dist = float.MaxValue;
                ProspectiveCollisionSafe = actor.Position + (ProspectiveCollision.End - actor.Position) * 1000;
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

    private static void TestCollision(Engine engine, T actor, bool isProspective, ref CollisionResultData vData)
    {
      try
      {
        vData.ActorID = -1;
        //vData.Impact = new TV_3DVECTOR();
        //vData.Normal = new TV_3DVECTOR();

        TV_COLLISIONRESULT tvcres = new TV_COLLISIONRESULT();
        bool result = false;

        /* // Octree filtering (still slower than TrueVision TVScene!)
        string oid = "";
        engine.GameScenarioManager.Octree.GetId(start, end, ref oid);

        foreach (ActorInfo a in engine.GameScenarioManager.Octree.Search(oid))
        {
          if (//engine.GameScenarioManager.Octree.Contains(sb, a.OctID)
             a.Mask.Has(ComponentMask.CAN_BECOLLIDED) 
            && a.Active 
            && !a.IsAggregateMode)
            //using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
              result = a.AdvancedCollision(start, end, ref tvcres);
          if (result)
            break;
        }
        */

        using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          actor.EnableCollision(false);
          result = engine.TrueVision.TVScene.AdvancedCollision(vData.Start, vData.End, ref tvcres, (int)CONST_TV_OBJECT_TYPE.TV_OBJECT_MESH, CONST_TV_TESTTYPE.TV_TESTTYPE_ACCURATETESTING);
          actor.EnableCollision(true);
        }
        vData.End = vData.Start;

        if (result)
        {
          if (tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_MESH)
            return;

          vData.Impact = tvcres.vCollisionImpact;
          vData.Normal = tvcres.vCollisionNormal;

          if (!isProspective)
          {
            int checkID = MeshModel.GetID(tvcres.iMeshID);
            if (checkID != -1)
            {
              ActorInfo checkActor = engine.ActorFactory.Get(checkID);
              if (actor.CanCollideWith(checkActor))
                vData.ActorID = checkID;
            }
          }
          else
          {
            int checkID = MeshModel.GetID(tvcres.iMeshID);
            if (checkID != -1)
            {
              ActorInfo checkActor = engine.ActorFactory.Get(checkID);
              if (checkActor != null)
                if (!(checkActor.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER)))
                  vData.ActorID = checkID;
            }
          }
        }
      }
      catch
      {
        vData.ActorID = -1;
        vData.Impact = new TV_3DVECTOR();
        vData.Normal = new TV_3DVECTOR();
      }
    }
  }
}
