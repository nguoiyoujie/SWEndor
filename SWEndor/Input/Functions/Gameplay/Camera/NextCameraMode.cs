using MTV3D65;
using SWEndor.Player;
using SWEndor.Scenarios;

namespace SWEndor.Input.Functions.Gameplay.Camera
{
  public class NextCameraMode : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_E;
    public static string InternalName = "g_cammode+";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      PlayerCameraInfo.Instance().CameraMode = GameScenarioManager.Instance().Scenario.NextCameraMode();
    }
  }
}
