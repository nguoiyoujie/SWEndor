using MTV3D65;

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
  }
}
