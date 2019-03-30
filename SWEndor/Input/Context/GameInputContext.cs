using MTV3D65;
using SWEndor.Scenarios;
using SWEndor.Sound;
using SWEndor.Terminal;
using SWEndor.UI;

namespace SWEndor.Input.Context
{
  public class GameInputContext : IInputContext
  {
    public virtual void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      if (keydata.Pressed > 0)
      {
        // Enter Menu
        if (keydata.Key == (int)CONST_TV_KEY.TV_KEY_ESCAPE)
        {
          if (!Screen2D.Instance().ShowPage)
          {
            Screen2D.Instance().CurrentPage = new UIPage_PauseMenu(); // configurable?
            Screen2D.Instance().ShowPage = true;
            Game.Instance().IsPaused = true;
          }
          return;
        }

        bool makeSound = false;

        // Camera control
        if (keydata.Key == InputKeyMap.GetFnKey("g_cammode+"))
          PlayerCameraInfo.Instance().CameraMode = GameScenarioManager.Instance().Scenario.NextCameraMode();
        else if (keydata.Key == InputKeyMap.GetFnKey("g_cammode-"))
          PlayerCameraInfo.Instance().CameraMode = GameScenarioManager.Instance().Scenario.PrevCameraMode();

        // Primary Weapon
        if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_weap1mode+")))
        {
          PlayerInfo.Instance().NextPrimaryWeapon();
          makeSound = true;
        }
        else if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_weap1mode-")))
        {
          PlayerInfo.Instance().PrevPrimaryWeapon();
          makeSound = true;
        }

        // Secondary Weapon
        if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_weap2mode+")))
        {
          PlayerInfo.Instance().NextSecondaryWeapon();
          makeSound = true;
        }
        else if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_weap2mode-")))
        {
          PlayerInfo.Instance().PrevSecondaryWeapon();
          makeSound = true;
        }

        // UI Toggle
        if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_ui_toggle")))
        {
          Screen2D.Instance().ShowUI = !Screen2D.Instance().ShowUI;
          makeSound = true;
        }

        if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_ui_status_toggle")))
        {
          Screen2D.Instance().ShowStatus = !Screen2D.Instance().ShowStatus;
          makeSound = true;
        }

        if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_ui_score_toggle")))
        {
          Screen2D.Instance().ShowScore = !Screen2D.Instance().ShowScore;
          makeSound = true;
        }

        if (keydata.Key.Equals(InputKeyMap.GetFnKey("g_ui_radar_toggle")))
        {
          Screen2D.Instance().ShowRadar = !Screen2D.Instance().ShowRadar;
          makeSound = true;
        }

        // Terminal
        if (InputManager.Instance().CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
        {
          TConsole.Visible = true;
        }

        if (makeSound)
          SoundManager.Instance().SetSound("button_1");
      }
    }

    public virtual void HandleKeyState(byte[] keyPressedStates)
    {
      // speed
      if (keyPressedStates[InputKeyMap.GetFnKey("g_speed+")] != 0)
        PlayerInfo.Instance().ChangeSpeed(1);
      else if (keyPressedStates[InputKeyMap.GetFnKey("g_speed-")] != 0)
        PlayerInfo.Instance().ChangeSpeed(-1);

      // freemode camera
      if (PlayerCameraInfo.Instance().CameraMode == CameraMode.FREEMODE)
      {
        float rate = InputManager.Instance().SHIFT ? 2500 : 500;
        TVCamera tvc = PlayerCameraInfo.Instance().Camera;
        rate *= Game.Instance().TimeControl.RenderInterval;

        if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_UP] != 0)
          tvc.MoveRelative(rate, 0, 0);
        else if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_DOWN] != 0)
          //tvc.MoveRelative(-Game.Instance().TimeSinceRender / Game.Instance().TimeControl.SpeedModifier * rate, 0, 0);
          tvc.MoveRelative(-rate, 0, 0);

        if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_PAGEUP] != 0)
          tvc.MoveRelative(0, rate, 0);
        else if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_PAGEDOWN] != 0)
          tvc.MoveRelative(0, -rate, 0);

        if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_RIGHT] != 0)
          tvc.MoveRelative(0, 0, rate);
        else if (keyPressedStates[(int)CONST_TV_KEY.TV_KEY_LEFT] != 0)
          tvc.MoveRelative(0, 0, -rate);
      }
    }

    public virtual void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      if (button1)
        PlayerInfo.Instance().FirePrimaryWeapon();

      if (button2)
        PlayerInfo.Instance().FireSecondaryWeapon();

      PlayerCameraInfo.Instance().RotateCam(mouseX / Engine.Instance().ScreenWidth * 2 - 1, mouseY / Engine.Instance().ScreenHeight * 2 - 1);
    }
  }
}
