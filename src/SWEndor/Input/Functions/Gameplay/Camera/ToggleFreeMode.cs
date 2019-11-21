using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay
{
  public class ToggleFreeMode : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_O;
    public static string InternalName = "c_free";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerCameraInfo.IsFreeLook = !engine.PlayerCameraInfo.IsFreeLook;

      if (engine.PlayerCameraInfo.IsFreeLook)
      {
        engine.PlayerCameraInfo.FreeLook.Position = engine.PlayerCameraInfo.Position;
        engine.PlayerCameraInfo.FreeLook.Rotation = engine.PlayerCameraInfo.Rotation;
        engine.Screen2D.MessageSecondaryText("CAMERA: FREEMODE"
                                           , 2.5f
                                           , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL)
                                           , 1);
      }
      else
      {
        engine.Screen2D.MessageSecondaryText(string.Format("CAMERA: {0}", engine.PlayerCameraInfo.CameraMode)
                                           , 2.5f
                                           , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL)
                                           , 1);
      }
    }
  }
}
