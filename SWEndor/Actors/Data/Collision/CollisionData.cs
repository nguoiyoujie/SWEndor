using MTV3D65;

namespace SWEndor.Actors.Data
{
  public struct CollisionResultData
  {
    public ActorInfo Actor;
    public TV_3DVECTOR Impact;
    public TV_3DVECTOR Normal;

    public void Reset()
    {
      Impact = new TV_3DVECTOR();
      Normal = new TV_3DVECTOR();
      Actor = null;
    }
  }

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
    public float ProspectiveCollisionTime;
    public float ProspectiveCollisionFactor;

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
      ProspectiveCollisionTime = 0;
      ProspectiveCollisionFactor = 2.5f;
      IsAvoidingCollision = false;
    }
  }
}
