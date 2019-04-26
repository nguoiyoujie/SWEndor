using MTV3D65;
using SWEndor.ActorTypes;

namespace SWEndor.Actors.Components
{
  public struct CollisionInfo
  {
    private readonly ActorInfo Actor;

    public bool IsTestingCollision;
    public bool IsInCollision;
    public TV_3DVECTOR CollisionImpact;
    public TV_3DVECTOR CollisionNormal;
    public int CollisionActorID;

    public bool IsTestingProspectiveCollision;
    public bool IsInProspectiveCollision;
    public int ProspectiveCollisionActorID;
    public TV_3DVECTOR ProspectiveCollisionImpact;
    public TV_3DVECTOR ProspectiveCollisionNormal;
    public TV_3DVECTOR ProspectiveCollisionSafe;
    public float ProspectiveCollisionScanDistance;
    public float ProspectiveCollisionLevel;

    public bool IsAvoidingCollision;


    public CollisionInfo(ActorInfo actor)
    {
      Actor = actor;

      IsTestingCollision = false;
      IsInCollision = false;
      CollisionImpact = new TV_3DVECTOR();
      CollisionNormal = new TV_3DVECTOR();
      CollisionActorID = -1;
      IsTestingProspectiveCollision = false;
      IsInProspectiveCollision = false;
      ProspectiveCollisionActorID = -1;
      ProspectiveCollisionImpact = new TV_3DVECTOR();
      ProspectiveCollisionNormal = new TV_3DVECTOR();
      ProspectiveCollisionSafe = new TV_3DVECTOR();
      ProspectiveCollisionScanDistance = 1000;
      ProspectiveCollisionLevel = 0;
      IsAvoidingCollision = false;
    }

    public void Reset()
    {
      IsTestingCollision = false;
      IsInCollision = false;
      CollisionImpact = new TV_3DVECTOR();
      CollisionNormal = new TV_3DVECTOR();
      CollisionActorID = -1;
      IsTestingProspectiveCollision = false;
      IsInProspectiveCollision = false;
      ProspectiveCollisionActorID = -1;
      ProspectiveCollisionImpact = new TV_3DVECTOR();
      ProspectiveCollisionNormal = new TV_3DVECTOR();
      ProspectiveCollisionSafe = new TV_3DVECTOR();
      ProspectiveCollisionScanDistance = 1000;
      ProspectiveCollisionLevel = 0;
      IsAvoidingCollision = false;
    }


    internal void CheckCollision()
    {
      IsTestingCollision = false;
      if (!Actor.PrevPosition.Equals(new TV_3DVECTOR()) && !Actor.GetPosition().Equals(new TV_3DVECTOR()))
      {
        // only check player and projectiles
        if (Actor.IsPlayer()
          || Actor.TypeInfo is ActorTypes.Group.Projectile
          || (Actor.ActorState == ActorState.DYING 
          && Actor.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER)))
        {
          if (IsInCollision)
          {
            ActorInfo a = Actor.GetEngine().ActorFactory.Get(CollisionActorID);
            if (Actor.IsPlayer() 
              && Actor.GetEngine().PlayerInfo.PlayerAIEnabled)
            {
              Actor.GetEngine().Screen2D.MessageSecondaryText(string.Format("DEV WARNING: PLAYER AI COLLIDED: {0}", a.ToString()), 1.5f, new TV_COLOR(1, 0.2f, 0.2f, 1), 99999);
              IsInCollision = false;
              IsTestingCollision = true;
              return;
            }
            if (a != null)
            {
              a.TypeInfo.ProcessHit(CollisionActorID, Actor.ID, CollisionImpact, CollisionNormal);
              Actor.TypeInfo.ProcessHit(Actor.ID, CollisionActorID, CollisionImpact, -1 * CollisionNormal);
            }
            IsInCollision = false;
          }
          IsTestingCollision = true;
        }
      }
    }

    private void TestLandscapeCollision()
    {
      TV_3DVECTOR vmin = Actor.GetRelativePositionXYZ(0, 0, Actor.TypeInfo.max_dimensions.z, false);
      TV_3DVECTOR vmax = Actor.GetRelativePositionXYZ(0, 0, Actor.TypeInfo.min_dimensions.z, false) + Actor.PrevPosition - Actor.Position;

      // check landscape only
      if (Actor.TypeInfo is ActorTypes.Group.Projectile || Actor.TypeInfo.TargetType != TargetType.NULL)
        if (Actor.GetEngine().LandInfo.Land.AdvancedCollide(vmin, vmax).IsCollision())
        {
          Actor.ActorState = ActorState.DEAD;
        }
    }

    internal void TestCollision()
    {
      if (IsTestingCollision)
      {
        TV_3DVECTOR vmin = Actor.GetRelativePositionXYZ(0, 0, Actor.TypeInfo.max_dimensions.z, false);
        TV_3DVECTOR vmax = Actor.GetRelativePositionXYZ(0, 0, Actor.TypeInfo.min_dimensions.z, false) + Actor.PrevPosition - Actor.Position;

        IsInCollision = TestCollision(vmin, vmax, true, out CollisionImpact, out CollisionNormal, out CollisionActorID);
        IsTestingCollision = false;
      }
      if (IsTestingProspectiveCollision)
      {
        int dummy;

        TV_3DVECTOR prostart = Actor.GetRelativePositionXYZ(0, 0, Actor.TypeInfo.max_dimensions.z + 10);
        TV_3DVECTOR proend0 = Actor.GetRelativePositionXYZ(0, 0, Actor.TypeInfo.max_dimensions.z + 10 + ProspectiveCollisionScanDistance);

        TV_3DVECTOR proImpact = new TV_3DVECTOR();
        TV_3DVECTOR proNormal = new TV_3DVECTOR();

        TV_3DVECTOR _Impact = new TV_3DVECTOR();
        TV_3DVECTOR _Normal = new TV_3DVECTOR();
        int count = 0;

        IsInProspectiveCollision = false;

        if (TestCollision(prostart, proend0, false, out _Impact, out _Normal, out ProspectiveCollisionActorID))
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

              proend0 = Actor.GetRelativePositionXYZ(i * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                             , j * ProspectiveCollisionScanDistance * 0.1f * ProspectiveCollisionLevel
                                             , Actor.TypeInfo.max_dimensions.z + 10 + ProspectiveCollisionScanDistance);
              if (TestCollision(prostart, proend0, false, out _Impact, out _Normal, out dummy))
              {
                proImpact += _Impact;
                proNormal += _Normal;
                count++;
                float newdist = Actor.GetEngine().TrueVision.TVMathLibrary.GetDistanceVec3D(Actor.Position, _Impact);
                if (dist < newdist)
                {
                  dist = newdist;
                  ProspectiveCollisionSafe = Actor.Position + (proend0 - Actor.Position) * 1000;
                  if (IsAvoidingCollision)
                    nextlevel = false;
                }
              }
              else
              {
                dist = float.MaxValue;
                if (!IsAvoidingCollision)
                  nextlevel = false;
                ProspectiveCollisionSafe = Actor.Position + (proend0 - Actor.Position) * 1000;
              }
            }
        }

        if (count > 0)
        {
          ProspectiveCollisionImpact = proImpact / count;
          ProspectiveCollisionNormal = proNormal / count;
        }
        IsTestingProspectiveCollision = false;
      }
    }

    private bool TestCollision(TV_3DVECTOR start, TV_3DVECTOR end, bool getActorInfo, out TV_3DVECTOR vImpact, out TV_3DVECTOR vNormal, out int actorID)
    {
      using (new PerfElement("fn_testcollision"))
      {
        try
        {
          actorID = -1;
          vImpact = new TV_3DVECTOR();
          vNormal = new TV_3DVECTOR();

          TV_COLLISIONRESULT tvcres = new TV_COLLISIONRESULT();

          if (Actor.GetEngine().TrueVision.TVScene.AdvancedCollision(start, end, ref tvcres))
          {
            if (Actor.IsPlayer())
            {
              Actor.GetEngine().TrueVision.TVScreen2DImmediate.Action_Begin2D();
              Actor.GetEngine().TrueVision.TVScreen2DImmediate.Draw_Line3D(start.x
                                                , start.y
                                                , start.z
                                                , end.x
                                                , end.y
                                                , end.z
                                                , new TV_COLOR(0.5f, 1, 0.2f, 1).GetIntColor()
                                                );
              Actor.GetEngine().TrueVision.TVScreen2DImmediate.Action_End2D();
            }

            if (tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_MESH && tvcres.eCollidedObjectType != CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              return false;

            vImpact = new TV_3DVECTOR(tvcres.vCollisionImpact.x, tvcres.vCollisionImpact.y, tvcres.vCollisionImpact.z);
            vNormal = new TV_3DVECTOR(tvcres.vCollisionNormal.x, tvcres.vCollisionNormal.y, tvcres.vCollisionNormal.z);

            if (getActorInfo)
            {
              if (tvcres.eCollidedObjectType == CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              {
                Actor.ActorState = ActorState.DEAD;
                return true;
              }

              TVMesh tvm = Actor.GetEngine().TrueVision.TVGlobals.GetMeshFromID(tvcres.iMeshID);
              if (tvm != null) // && tvm.IsVisible())
              {
                if (int.TryParse(tvm.GetTag(), out actorID))
                {
                  ActorInfo actor = Actor.GetEngine().ActorFactory.Get(actorID);
                  return (actor != null
                       && actorID != Actor.ID
                       && !Actor.HasRelative(actorID)
                       && actor.TypeInfo.CollisionEnabled
                       && !actor.IsAggregateMode()
                       && actor.CreationState == CreationState.ACTIVE);
                }
              }
              return false;
            }
            else
            {
              if (tvcres.eCollidedObjectType == CONST_TV_OBJECT_TYPE.TV_OBJECT_LANDSCAPE)
              {
                return true;
              }

              TVMesh tvm = Actor.GetEngine().TrueVision.TVGlobals.GetMeshFromID(tvcres.iMeshID);
              if (tvm != null) // && tvm.IsVisible())
              {
                if (int.TryParse(tvm.GetTag(), out actorID))
                {
                  return true;
                }
              }
              return false;
            }
          }
          else
          {
            if (Actor.IsPlayer())
            {
              Actor.GetEngine().TrueVision.TVScreen2DImmediate.Action_Begin2D();
              Actor.GetEngine().TrueVision.TVScreen2DImmediate.Draw_Line3D(start.x
                                                , start.y
                                                , start.z
                                                , end.x
                                                , end.y
                                                , end.z
                                                , new TV_COLOR(1, 0.5f, 0.2f, 1).GetIntColor()
                                                );
              Actor.GetEngine().TrueVision.TVScreen2DImmediate.Action_End2D();
            }
          }
          return false;
        }
        catch
        {
          actorID = -1;
          vImpact = new TV_3DVECTOR();
          vNormal = new TV_3DVECTOR();
          return false;
        }
      }
    }
  }
}
