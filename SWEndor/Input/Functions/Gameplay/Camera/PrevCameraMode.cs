using SWEndor.Player;

namespace SWEndor.Input.Functions.Gameplay.Camera
{
  public class PrevCameraMode : InputFunction
  {
    private int _key = -1;
    public static string InternalName = "g_cammode-";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(InputManager manager)
    {
      manager.Engine.PlayerCameraInfo.CameraMode = manager.Engine.GameScenarioManager.Scenario.NextCameraMode();
    }
  }
}
