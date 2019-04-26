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

    public override void Process(InputManager manager)
    {
      if (manager.Engine.PlayerCameraInfo.CameraMode == CameraMode.FREEMODE)
      {
        float rate = Globals.Engine.InputManager.SHIFT ? 2500 : 500;
        TVCamera tvc = manager.Engine.PlayerCameraInfo.Camera;
        rate *= Globals.Engine.Game.TimeControl.RenderInterval;
        tvc.MoveRelative(rate, 0, 0);
      }
    }
  }
}
