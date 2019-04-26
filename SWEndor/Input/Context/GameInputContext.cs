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
    public readonly static GameInputContext Instance = new GameInputContext();
    protected GameInputContext() { }

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

    public override void HandleKeyBuffer(InputManager manager, TV_KEYDATA keydata)
    {
      base.HandleKeyBuffer(manager, keydata);
      if (keydata.Pressed > 0)
        InputFunction.Registry.ProcessOnPress(manager, keydata.Key);
    }

    public override void HandleKeyState(InputManager manager, byte[] keyPressedStates)
    {
      base.HandleKeyState(manager, keyPressedStates);
      InputFunction.Registry.ProcessWhilePressed(manager, keyPressedStates);
    }

    public override void HandleMouse(InputManager manager, int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      base.HandleMouse(manager, mouseX, mouseY, button1, button2, button3, button4, mouseScroll);

      if (button1)
        manager.Engine.PlayerInfo.FirePrimaryWeapon();

      if (button2)
        manager.Engine.PlayerInfo.FireSecondaryWeapon();

      manager.Engine.PlayerCameraInfo.RotateCam(mouseX / manager.Engine.ScreenWidth * 2 - 1, mouseY / manager.Engine.ScreenHeight * 2 - 1);
    }
  }
}
