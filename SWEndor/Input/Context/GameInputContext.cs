using MTV3D65;
using SWEndor.Input.Functions;
using SWEndor.Player;
using SWEndor.Input.Functions.Gameplay.UI;
using SWEndor.Input.Functions.Gameplay;
using SWEndor.Input.Functions.Gameplay.Camera;
using SWEndor.Input.Functions.Gameplay.Speed;
using SWEndor.Input.Functions.Gameplay.Weapon;

namespace SWEndor.Input.Context
{
  public class GameInputContext : AInputContext
  {
    public static string[] _functions = new string[]
    {
      PauseToMenu.InternalName,

      ToggleUIVisibility.InternalName,
      ToggleRadarVisibility.InternalName,
      ToggleStatusVisibility.InternalName,
      ToggleScoreVisibility.InternalName,

      MoveCameraForward.InternalName,
      MoveCameraBackward.InternalName,
      MoveCameraUpward.InternalName,
      MoveCameraDownward.InternalName,
      MoveCameraRightward.InternalName,
      MoveCameraLeftward.InternalName,

      NextCameraMode.InternalName,
      PrevCameraMode.InternalName,
      NextPrimary.InternalName,
      PrevPrimary.InternalName,
      NextSecondary.InternalName,
      PrevSecondary.InternalName,

      Up.InternalName,
      Down.InternalName,
    };

    public override void Set()
    {
      base.Set();
      foreach (string s in _functions)
      {
        InputFunction fn = InputFunction.Registry.Get(s);
        if (fn != null)
          fn.Enabled = true;
      }
    }

    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      base.HandleKeyBuffer(keydata);
      if (keydata.Pressed > 0)
        InputFunction.Registry.ProcessOnPress(keydata.Key);
    }

    public override void HandleKeyState(byte[] keyPressedStates)
    {
      base.HandleKeyState(keyPressedStates);
      InputFunction.Registry.ProcessWhilePressed(keyPressedStates);
    }

    public override void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      base.HandleMouse(mouseX, mouseY, button1, button2, button3, button4, mouseScroll);

      if (button1)
        PlayerInfo.Instance().FirePrimaryWeapon();

      if (button2)
        PlayerInfo.Instance().FireSecondaryWeapon();

      PlayerCameraInfo.Instance().RotateCam(mouseX / Engine.Instance().ScreenWidth * 2 - 1, mouseY / Engine.Instance().ScreenHeight * 2 - 1);
    }
  }
}
