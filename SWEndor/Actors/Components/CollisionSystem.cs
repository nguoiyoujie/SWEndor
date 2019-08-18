using MTV3D65;
using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Primitives;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors.Components
{
  public static class CollisionSystem
  {
    internal static bool ActivateImminentCollisionCheck(Engine engine, ActorInfo actor)
    {
      return ActivateImminentCollisionCheck(engine
                                          , actor
                                          , ref actor.CollisionData
                                          , ref actor.MoveData);
    }

    private static bool ActivateImminentCollisionCheck(Engine engine, ActorInfo actor, ref CollisionData data, ref MoveData mdata)
    {
      if (data.ProspectiveCollisionTime < engine.Game.GameTime)
      {
        if (data.IsInProspectiveCollision)
        {
          data.ProspectiveCollisionTime = engine.Game.GameTime + 0.25f; // delay should be adjusted with FPS / CPU load, ideally every ~0.5s, but not more than 2.5s. Can be slightly longer since it is already performing evasion.
        }
        else
        {
          data.ProspectiveCollisionTime = engine.Game.GameTime + 0.1f; // delay should be adjusted with FPS / CPU load, ideally every run (~0.1s), but not more than 2s.
        }
        data.ProspectiveCollisionScanDistance = mdata.Speed * data.ProspectiveCollisionFactor;
        data.IsTestingProspectiveCollision = true;
      }
      return data.IsInProspectiveCollision;
    }

    internal static void CreateAvoidAction(Engine engine, ActorInfo actor)
    {
      actor.QueueFirst(new AvoidCollisionRotate(actor.CollisionData.ProspectiveCollision.Impact, actor.CollisionData.ProspectiveCollision.Normal));
    }

    internal static void CheckCollision(Engine engine, ActorInfo actor)
    {
      CheckCollision(engine, actor, ref actor.CollisionData);
    }

    private static void CheckCollision(Engine engine, ActorInfo actor, ref CollisionData data)
    {
      data.IsTestingCollision = false;

      // only check player and projectiles
        if (actor.IsPlayer
          || actor.TypeInfo is ActorTypes.Groups.Projectile
          || ((actor.StateModel.IsDying) && actor.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER)))
      {
        if (data.Collision.Actor != null)
        {
          ActorInfo d = data.Collision.Actor;
          if (actor.IsPlayer
                && engine.PlayerInfo.PlayerAIEnabled)
          {
            engine.Screen2D.MessageSecondaryText("DEV WARNING: PLAYER AI COLLIDED: {0}".F(d), 1.5f, new TV_COLOR(1, 0.2f, 0.2f, 1), 99999);
            data.Collision.Actor = null;
            data.IsTestingCollision = true;
            return;
          }

          d.TypeInfo.ProcessHit(d, actor, data.Collision.Impact, data.Collision.Normal);
          actor.TypeInfo.ProcessHit(actor, d, data.Collision.Impact, -1 * data.Collision.Normal);

          data.Collision.Actor = null;
        }
        data.IsTestingCollision = true;
      }
    }

    internal static void TestCollision(Engine engine, ActorInfo actor)
    {
      TestCollision(engine, actor, ref actor.CollisionData);
    }

    private static void TestCollision(Engine engine, ActorInfo actor, ref CollisionData data)
    {
      if (data.IsTestingCollision)
      {
        TV_3DVECTOR vmin = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.max_dimensions.z, false);
        TV_3DVECTOR vmax = actor.GetRelativePositionXYZ(0, 0, actor.TypeInfo.min_dimensions.z, false) - (actor.Transform.GetGlobalPosition(actor, engine.Game.GameTime) - actor.Transform.GetPrevGlobalPosition(actor, engine.Game.GameTime));//- actor.CoordData.LastTravelled;

        TestCollision(engine, actor, vmin, vmax, false, out data.Collision.Impact, out data.Collision.Normal, out data.Collision.Actor);
        data.IsTestingCollision = false;
      }
      if (data.IsTestingProspectiveCollision)
      {
        ActorInfo dummy;

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

        TestCollision(engine, actor, prostart, proend0, true, out _Impact, out _Normal, out data.ProspectiveCollision.Actor);

        if (data.ProspectiveCollision.Actor != null)
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

              if (dummy != null)
              {
                proImpact += _Impact;
                proNormal += _Normal;
                count++;
                float newdist = engine.TrueVision.TVMathLibrary.GetDistanceVec3D(actor.Transform.Position, _Impact);
                if (dist < newdist)
                {
                  dist = newdist;
                  data.ProspectiveCollisionSafe = actor.Transform.Position + (proend0 - actor.Transform.Position) * 1000;
                  if (data.IsAvoidingCollision)
                    nextlevel = false;
                }
              }
              else
              {
                dist = float.MaxValue;
                if (!data.IsAvoidingCollision)
                  nextlevel = false;
                data.ProspectiveCollisionSafe = actor.Transform.Position + (proend0 - actor.Transform.Position) * 1000;
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

    private static void TestCollision(Engine engine, ActorInfo actor, TV_3DVECTOR start, TV_3DVECTOR end, bool isProspective, out TV_3DVECTOR vImpact, out TV_3DVECTOR vNormal, out ActorInfo CollisionActor)
    {
      try
      {
        CollisionActor = null;
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
                using (var v = engine.ActorFactory.Get(checkID))
                  if (v != null)
                  {
                    ActorInfo checkActor = v.Value;
                    if (checkActor != null
                         && checkActor != actor
                         && actor.TopParent != checkActor.TopParent
                         && checkActor.StateModel.ComponentMask.Has(ComponentMask.CAN_BECOLLIDED)
                         && !checkActor.IsAggregateMode
                         )
                    {
                      CollisionActor = checkActor;
                    }
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
                using (var v = engine.ActorFactory.Get(checkID))
                  if (v != null)
                  {
                    ActorInfo checkActor = v.Value;
                    if (checkActor != null
                       && !(checkActor.TypeInfo is ActorTypes.Groups.Fighter)
                       )
                    {
                      CollisionActor = checkActor;
                    }
                  }
              }
            }
          }
        }
      }
      catch
      {
        CollisionActor = null;
        vImpact = new TV_3DVECTOR();
        vNormal = new TV_3DVECTOR();
      }
    }
  }
}
