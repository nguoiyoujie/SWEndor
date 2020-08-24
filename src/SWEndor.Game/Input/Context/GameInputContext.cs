using SWEndor.Game.Input.Functions;
using SWEndor.Game.Input.Functions.Gameplay.UI;
using SWEndor.Game.Input.Functions.Gameplay;
using SWEndor.Game.Input.Functions.Gameplay.Camera;
using SWEndor.Game.Input.Functions.Gameplay.Speed;
using SWEndor.Game.Input.Functions.Gameplay.Weapon;
using SWEndor.Game.Input.Functions.Utility.Screen;

namespace SWEndor.Game.Input.Context
{
  public class GameInputContext : AInputContext
  {
    public GameInputContext(InputManager manager) : base(manager) { }

    public static string[] _functions = new string[]
    {
      PauseToMenu.InternalName,
      ShowMap.InternalName,

      ToggleUIVisibility.InternalName,
      ToggleSquadVisibility.InternalName,
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

      ToggleLockOn.InternalName,
      SquadCommand.InternalName,
      SquadCancelCommand.InternalName,

      Up.InternalName,
      Down.InternalName,

      SaveScreenshot.InternalName
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

    public override void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      base.HandleMouse(mouseX, mouseY, button1, button2, button3, button4, mouseScroll);

      if (button1)
        Engine.PlayerInfo.FirePrimaryWeapon();

      if (button2)
        Engine.PlayerInfo.FireSecondaryWeapon();

      Engine.PlayerCameraInfo.RotateCam(mouseX / (float)Engine.ScreenWidth * 2 - 1, mouseY / (float)Engine.ScreenHeight * 2 - 1, mouseScroll);
    }
  }
}
