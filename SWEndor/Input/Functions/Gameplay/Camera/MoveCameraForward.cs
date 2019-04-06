using MTV3D65;
using SWEndor.Player;

namespace SWEndor.Input.Functions.Gameplay
{
  public class MoveCameraForward : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_UP;
    public static string InternalName = "";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.WHILEPRESSED; } }

    public override void Process()
    {
      if (PlayerCameraInfo.Instance().CameraMode == CameraMode.FREEMODE)
      {
        float rate = InputManager.Instance().SHIFT ? 2500 : 500;
        TVCamera tvc = PlayerCameraInfo.Instance().Camera;
        rate *= Game.Instance().TimeControl.RenderInterval;
        tvc.MoveRelative(rate, 0, 0);
      }
    }
  }
}
