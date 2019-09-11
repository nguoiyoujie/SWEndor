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
}
