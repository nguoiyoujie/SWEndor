using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay.Camera
{
  public class PrevCameraMode : InputFunction
  {
    private int _key = -1;
    public static string InternalName = "g_cammode-";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerCameraInfo.CameraMode = engine.PlayerCameraInfo.PrevCameraMode();
      engine.Screen2D.MessageSecondaryText(string.Format("CAMERA: {0}", engine.PlayerCameraInfo.CameraMode)
                                                 , 2.5f
                                                 , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL)
                                                 , 1);
    }
  }
}
