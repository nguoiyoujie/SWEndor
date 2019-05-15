using MTV3D65;

namespace SWEndor.ActorTypes.Components
{
  public struct ActorCameraInfo
  {
    public TV_3DVECTOR LookFrom;
    public TV_3DVECTOR LookAt;

    public ActorCameraInfo(TV_3DVECTOR from, TV_3DVECTOR to)
    {
      LookFrom = from;
      LookAt = to;
    }
  }
}
