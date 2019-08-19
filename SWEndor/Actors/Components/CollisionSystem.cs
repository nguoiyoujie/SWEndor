using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using System;

namespace SWEndor.Actors.Components
{
  public static class CollisionSystem
  {
    internal static bool ActivateImminentCollisionCheck(Engine engine, ActorInfo actor, ref float check_time, float scandistance)
    {
      return ActivateImminentCollisionCheck(engine, actor, ref check_time, scandistance, ref engine.ActorDataSet.CollisionData[actor.dataID]);
    }

    private static bool ActivateImminentCollisionCheck(Engine engine, ActorInfo actor, ref float check_time, float scandistance, ref CollisionData data)
    {
      if (check_time < engine.Game.GameTime)
      {
        if (data.IsInProspectiveCollision)
        {
          check_time = engine.Game.GameTime + 0.25f; // delay should be adjusted with FPS / CPU load, ideally every ~0.5s, but not more than 2.5s. Can be slightly longer since it is already performing evasion.
        }
        else
        {
          check_time = engine.Game.GameTime + 0.1f; // delay should be adjusted with FPS / CPU load, ideally every run (~0.1s), but not more than 2s.
        }
        data.ProspectiveCollisionScanDistance = scandistance;
        data.IsTestingProspectiveCollision = true;
      }
      return data.IsInProspectiveCollision;
    }

    internal static void CreateAvoidAction(Engine engine, ActorInfo actor)
    {
      actor.QueueFirst(new AvoidCollisionRotate(engine.ActorDataSet.CollisionData[actor.dataID].ProspectiveCollision.Impact, engine.ActorDataSet.CollisionData[actor.dataID].ProspectiveCollision.Normal));
    }

    internal static void CheckCollision(Engine engine, ActorInfo actor)
    {
      CheckCollision(engine, actor, ref engine.ActorDataSet.CollisionData[actor.dataID]);
    }

    private static void CheckCollision(Engine engine, ActorInfo actor, ref CollisionData data)
    {
      data.IsTestingCollision = false;
      // only check player and projectiles
      if (actor.IsPlayer
          || actor.TypeInfo is ActorTypes.Groups.Projectile
          || (actor.ActorState.IsDying() && actor.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER)))
      {
        if (data.Collision.ActorID >= 0)
        {
          ActorInfo a = engine.ActorFactory.Get(data.Collision.ActorID);
          if (actor.IsPlayer
                && engine.PlayerInfo.PlayerAIEnabled)
          {
            engine.Screen2D.MessageSecondaryText(string.Format("DEV WARNING: PLAYER AI COLLIDED: {0}", a.ToString()), 1.5f, new TV_COLOR(1, 0.2f, 0.2f, 1), 99999);
            data.Collision.ActorID = -1;
            data.IsTestingCollision = true;
            return;
          }

          a.TypeInfo.ProcessHit(a, actor, data.Collision.Impact, data.Collision.Normal);
          actor.TypeInfo.ProcessHit(actor, a, data.Collision.Impact, -1 * data.Collision.Normal);

          data.Collision.ActorID = -1;
        }
        data.IsTestingCollision = true;
      }
    }

    internal static void TestCollision(Engine engine, ActorInfo actor)
    {
      TestCollision(engine, actor, ref engine.ActorDataSet.CollisionData[actor.dataID]);
    }

    private static void TestCollision(Engine engine, ActorInfo actor, ref CollisionData data)
    {
      if (data.IsTestingCollision)
      {
        TV_3DVECTOR vmin = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.max_dimensions.z, false);
        TV_3DVECTOR vmax = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.min_dimensions.z, false) - actor.CoordData.LastTravelled;

        TestCollision(engine, actor, vmin, vmax, false, out data.Collision.Impact, out data.Collision.Normal, out data.Collision.ActorID);
        data.IsTestingCollision = false;
      }
      if (data.IsTestingProspectiveCollision)
      {
        int dummy;

        TV_3DVECTOR prostart = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.max_dimensions.z + 10);
        TV_3DVECTOR proend0 = actor.GetRelativePositionXYZ((float)Math.Sin(actor.MoveData.YTurnAngle * Globals.PI / 180) * data.ProspectiveCollisionScanDistance  //* actor.MoveData.Speed
                                                         , -(float)Math.Sin(actor.MoveData.XTurnAngle * Globals.PI / 180) * data.ProspectiveCollisionScanDistance  //* actor.MoveData.Speed
        //TV_3DVECTOR proend0 = actor.GetRelativePositionXYZ((float)Math.Sin(actor.MoveData.YTurnAngle * Globals.PI / 180) * data.ProspectiveCollisionScanDistance
        //                                                , -(float)Math.Sin(actor.MoveData.XTurnAngle * Globals.PI / 180) * data.ProspectiveCollisionScanDistance
                                                         , actor.TypeInfo.max_dimensions.z + 10 + data.ProspectiveCollisionScanDistance);

        TV_3DVECTOR proImpact = new TV_3DVECTOR();
        TV_3DVECTOR proNormal = new TV_3DVECTOR();

        TV_3DVECTOR _Impact = new TV_3DVECTOR();
        TV_3DVECTOR _Normal = new TV_3DVECTOR();
        int count = 0;

        data.IsInProspectiveCollision = false;

        TestCollision(engine, actor, prostart, proend0, true, out _Impact, out _Normal, out data.ProspectiveCollision.ActorID);

        if (data.ProspectiveCollision.ActorID >= 0)
        {
          proImpact += _Impact;
          proNormal += _Normal;
          count++;
          data.IsInProspectiveCollision = true;
          data.ProspectiveCollisionSafe = _Impact + _Normal * 10000;
        }

        if (data.IsInProspectiveCollision || data.IsAvoidingCollision)
          data.ProspectiveCollisionLevel = 0;
        bool nextlevel = true;
        float dist = 0;
        while (nextlevel && data.ProspectiveCollisionLevel < 5)
        {
          data.ProspectiveCollisionLevel++;
          nextlevel = true;
          for (int i = -1; i <= 1; i++)
            for (int j = -1; j <= 1; j++)
            {
              if (i == 0 && j == 0)
                continue;

              proend0 = actor.GetRelativePositionXYZ(i * data.ProspectiveCollisionScanDistance * 0.1f * data.ProspectiveCollisionLevel
                                             , j * data.ProspectiveCollisionScanDistance * 0.1f * data.ProspectiveCollisionLevel
                                             , actor.TypeInfo.max_dimensions.z + 10 + data.ProspectiveCollisionScanDistance);
              TestCollision(engine, actor, prostart, proend0, true, out _Impact, out _Normal, out dummy);

              if (dummy >= 0)
              {
                proImpact += _Impact;
                proNormal += _Normal;
                count++;
                float newdist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.CoordData.Position, _Impact);
                if (dist < newdist)
                {
                  dist = newdist;
                  data.ProspectiveCollisionSafe = actor.CoordData.Position + (proend0 - actor.CoordData.Position) * 1000;
                  if (data.IsAvoidingCollision)
                    nextlevel = false;
                }
              }
              else
              {
                dist = float.MaxValue;
                if (!data.IsAvoidingCollision)
                  nextlevel = false;
                data.ProspectiveCollisionSafe = actor.CoordData.Position + (proend0 - actor.CoordData.Position) * 1000;
              }
            }
        }

        if (count > 0)
        {
          data.ProspectiveCollision.Impact = proImpact / count;
          data.ProspectiveCollision.Normal = proNormal / count;
        }
        data.IsTestingProspectiveCollision = false;
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
        if (engine.TrueVision.TVScene.AdvancedCollision(start, end, ref tvcres))
        {
          // removed all landscape collision; unused and unnecessary complication

          if (tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_MESH)
            return;

          vImpact = new TV_3DVECTOR(tvcres.vCollisionImpact.x, tvcres.vCollisionImpact.y, tvcres.vCollisionImpact.z);
          vNormal = new TV_3DVECTOR(tvcres.vCollisionNormal.x, tvcres.vCollisionNormal.y, tvcres.vCollisionNormal.z);

          if (!isProspective)
          {
            TVMesh tvm = engine.TrueVision.TVGlobals.GetMeshFromID(tvcres.iMeshID);
            if (tvm != null) // && tvm.IsVisible())
            {
              int checkID = -1;
              if (int.TryParse(tvm.GetTag(), out checkID))
              {
                ActorInfo checkActor = engine.ActorFactory.Get(checkID);
                if (checkActor != null
                     //&& checkID != actor.ID
                     && actor.TopParent != checkActor.TopParent
                     && engine.MaskDataSet[checkActor].Has(ComponentMask.CAN_BECOLLIDED)
                     && !checkActor.IsAggregateMode
                     )
                {
                  CollisionActorID = checkID;
                }
              }
            }
          }
          else
          {
            TVMesh tvm = engine.TrueVision.TVGlobals.GetMeshFromID(tvcres.iMeshID);
            if (tvm != null) // && tvm.IsVisible())
            {
              int checkID = -1;
              if (int.TryParse(tvm.GetTag(), out checkID))
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
