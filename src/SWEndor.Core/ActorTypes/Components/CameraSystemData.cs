using Primitives.FileFormat.INI;
using Primrose.Primitives.ValueTypes;

namespace SWEndor.ActorTypes.Components
{
  internal struct CameraSystemData
  {
    private const string sCamera = "Camera";
    private static LookData[] NullCam = new LookData[0];

    [INISubSectionList(sCamera, "CAM", "Cameras")]
    internal LookData[] Cameras;

    [INIValue(sCamera, "DeathCam")]
    private float3 DeathCam;
    internal DeathCameraData DeathCamera { get { return new DeathCameraData(DeathCam); } }

    public static CameraSystemData Default { get { return new CameraSystemData(NullCam, new float3(350, 25, 15)); } }

    public CameraSystemData(LookData[] initsrc, float3 deathCam)
    {
      Cameras = initsrc;
      DeathCam = deathCam;
    }
  }
}
