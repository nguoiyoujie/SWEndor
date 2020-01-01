using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay.Camera
{
  public class NextCameraMode : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_E;
    public static string InternalName = "g_cammode+";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerCameraInfo.CameraMode = engine.PlayerCameraInfo.NextCameraMode();
      engine.Screen2D.MessageSecondaryText(string.Format("CAMERA: {0}", engine.PlayerCameraInfo.CameraMode)
                                                 , 2.5f
                                                 , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL)
                                                 , 1);
    }
  }
}
