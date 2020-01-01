using SWEndor.FileFormat.INI;

namespace SWEndor.ActorTypes.Components
{
  internal struct CameraSystemData
  {
    private static LookData[] NullCam = new LookData[0];
    internal LookData[] Cameras;
    internal DeathCameraData DeathCamera;

    public static CameraSystemData Default { get { return new CameraSystemData(NullCam, new DeathCameraData(350, 25, 15)); } }

    public CameraSystemData(LookData[] initsrc, DeathCameraData deathCam)
    {
      Cameras = initsrc;
      DeathCamera = deathCam;
    }

    public void LoadFromINI(INIFile f, string sectionname)
    {
      LookData.LoadFromINI(f, sectionname, "Cameras", out Cameras);
      DeathCamera.LoadFromINI(f, sectionname);
    }

    public void SaveToINI(INIFile f, string sectionname)
    {
      LookData.SaveToINI(f, sectionname, "Cameras", "CAM", Cameras);
      DeathCamera.SaveToINI(f, sectionname);
    }
  }
}
