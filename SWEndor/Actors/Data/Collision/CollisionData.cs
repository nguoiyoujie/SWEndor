using MTV3D65;

namespace SWEndor.Actors.Data
{
  public struct CollisionResultData
  {
    public int ActorID;
    public TV_3DVECTOR Impact;
    public TV_3DVECTOR Normal;

    public void Reset()
    {
      Impact = new TV_3DVECTOR();
      Normal = new TV_3DVECTOR();
      ActorID = -1;
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
  }
}
