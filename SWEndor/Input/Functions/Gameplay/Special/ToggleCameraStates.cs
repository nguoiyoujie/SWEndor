using MTV3D65;
using SWEndor.Player;

namespace SWEndor.Input.Functions.Gameplay.Special
{
  public class ToggleCameraStates : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_H;
    public static string InternalName = "d_cam+";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(InputManager manager)
    {
      if (manager.Engine.PlayerCameraInfo.CameraMode == CameraMode.FREEMODE) // last camera mode
        manager.Engine.PlayerCameraInfo.CameraMode = 0;
      else
        manager.Engine.PlayerCameraInfo.CameraMode++;
    }
  }
}
