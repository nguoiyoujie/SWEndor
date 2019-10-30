using MTV3D65;

namespace SWEndor.Models
{
  public struct CollisionResultData
  {
    public int ActorID;
    public TV_3DVECTOR Impact;
    public TV_3DVECTOR Normal;
    public TV_3DVECTOR Start;
    public TV_3DVECTOR End;

    public void Reset()
    {
      Impact = new TV_3DVECTOR();
      Normal = new TV_3DVECTOR();
      Start = new TV_3DVECTOR();
      End = new TV_3DVECTOR();
      ActorID = -1;
    }
  }
}
