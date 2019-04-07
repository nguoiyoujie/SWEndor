using MTV3D65;
using SWEndor.Input.Functions;
using SWEndor.Input.Functions.Gameplay.Camera;
using SWEndor.Input.Functions.Gameplay.Speed;
using SWEndor.Input.Functions.Gameplay.UI;
using SWEndor.Input.Functions.Gameplay.Weapon;

namespace SWEndor.UI.Menu.Pages
{
  public class KeyboardControls : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonToggleUI = new SelectionElement();
    SelectionElement ButtonToggleRadar = new SelectionElement();
    SelectionElement ButtonToggleScenarioInfo = new SelectionElement();
    SelectionElement ButtonToggleScoreboard = new SelectionElement();
    SelectionElement ButtonToggleCameraMode = new SelectionElement();
    SelectionElement ButtonToggleNextPrimaryWeaponMode = new SelectionElement();
    SelectionElement ButtonTogglePrevPrimaryWeaponMode = new SelectionElement();
    SelectionElement ButtonToggleNextSecondaryWeaponMode = new SelectionElement();
    SelectionElement ButtonTogglePrevSecondaryWeaponMode = new SelectionElement();
    SelectionElement ButtonToggleIncreaseSpeed = new SelectionElement();
    SelectionElement ButtonToggleDecreaseSpeed = new SelectionElement();

    SelectionElement ButtonSaveAndExit = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();

    CONST_TV_KEY KeyToggleUI;
    CONST_TV_KEY KeyToggleRadar;
    CONST_TV_KEY KeyToggleScenarioInfo;
    CONST_TV_KEY KeyToggleScoreboard;
    CONST_TV_KEY KeyToggleCameraMode;
    CONST_TV_KEY KeyToggleNextPrimaryWeaponMode;
    CONST_TV_KEY KeyTogglePrevPrimaryWeaponMode;
    CONST_TV_KEY KeyToggleNextSecondaryWeaponMode;
    CONST_TV_KEY KeyTogglePrevSecondaryWeaponMode;
    CONST_TV_KEY KeyToggleIncreaseSpeed;
    CONST_TV_KEY KeyToggleDecreaseSpeed;
    
    public KeyboardControls()
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;


      MainText.Text = "Configure Keyboard Bindings";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      ButtonToggleUI.Text = "Toggle UI";
      ButtonToggleUI.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleUI.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleUI.HighlightBoxPosition = ButtonToggleUI.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleUI.HighlightBoxWidth = 450;
      ButtonToggleUI.HighlightBoxHeight = 30;
      ButtonToggleUI.Selectable = true;
      ButtonToggleUI.OnKeyPress += SelectToggleUI;

      ButtonToggleRadar.Text = "Toggle Radar";
      ButtonToggleRadar.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleRadar.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleRadar.HighlightBoxPosition = ButtonToggleRadar.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleRadar.HighlightBoxWidth = 450;
      ButtonToggleRadar.HighlightBoxHeight = 30;
      ButtonToggleRadar.Selectable = true;
      ButtonToggleRadar.OnKeyPress += SelectToggleRadar;

      ButtonToggleScenarioInfo.Text = "Toggle Scenario Info";
      ButtonToggleScenarioInfo.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleScenarioInfo.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleScenarioInfo.HighlightBoxPosition = ButtonToggleScenarioInfo.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleScenarioInfo.HighlightBoxWidth = 450;
      ButtonToggleScenarioInfo.HighlightBoxHeight = 30;
      ButtonToggleScenarioInfo.Selectable = true;
      ButtonToggleScenarioInfo.OnKeyPress += SelectToggleScenarioInfo;

      ButtonToggleScoreboard.Text = "Toggle Scoreboard";
      ButtonToggleScoreboard.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleScoreboard.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleScoreboard.HighlightBoxPosition = ButtonToggleScoreboard.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleScoreboard.HighlightBoxWidth = 450;
      ButtonToggleScoreboard.HighlightBoxHeight = 30;
      ButtonToggleScoreboard.Selectable = true;
      ButtonToggleScoreboard.OnKeyPress += SelectToggleScoreboard;

      ButtonToggleCameraMode.Text = "Toggle Camera Mode";
      ButtonToggleCameraMode.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleCameraMode.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleCameraMode.HighlightBoxPosition = ButtonToggleCameraMode.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleCameraMode.HighlightBoxWidth = 450;
      ButtonToggleCameraMode.HighlightBoxHeight = 30;
      ButtonToggleCameraMode.Selectable = true;
      ButtonToggleCameraMode.OnKeyPress += SelectToggleCameraMode;

      ButtonToggleNextPrimaryWeaponMode.Text = "Next Primary Weapon";
      ButtonToggleNextPrimaryWeaponMode.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleNextPrimaryWeaponMode.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleNextPrimaryWeaponMode.HighlightBoxPosition = ButtonToggleNextPrimaryWeaponMode.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleNextPrimaryWeaponMode.HighlightBoxWidth = 450;
      ButtonToggleNextPrimaryWeaponMode.HighlightBoxHeight = 30;
      ButtonToggleNextPrimaryWeaponMode.Selectable = true;
      ButtonToggleNextPrimaryWeaponMode.OnKeyPress += SelectToggleNextPrimaryWeaponMode;

      ButtonTogglePrevPrimaryWeaponMode.Text = "Prev Primary Weapon";
      ButtonTogglePrevPrimaryWeaponMode.TextPosition = new TV_2DVECTOR(x, y);
      ButtonTogglePrevPrimaryWeaponMode.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonTogglePrevPrimaryWeaponMode.HighlightBoxPosition = ButtonTogglePrevPrimaryWeaponMode.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonTogglePrevPrimaryWeaponMode.HighlightBoxWidth = 450;
      ButtonTogglePrevPrimaryWeaponMode.HighlightBoxHeight = 30;
      ButtonTogglePrevPrimaryWeaponMode.Selectable = true;
      ButtonTogglePrevPrimaryWeaponMode.OnKeyPress += SelectTogglePrevPrimaryWeaponMode;

      ButtonToggleNextSecondaryWeaponMode.Text = "Next Secondary Weapon";
      ButtonToggleNextSecondaryWeaponMode.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleNextSecondaryWeaponMode.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleNextSecondaryWeaponMode.HighlightBoxPosition = ButtonToggleNextSecondaryWeaponMode.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleNextSecondaryWeaponMode.HighlightBoxWidth = 450;
      ButtonToggleNextSecondaryWeaponMode.HighlightBoxHeight = 30;
      ButtonToggleNextSecondaryWeaponMode.Selectable = true;
      ButtonToggleNextSecondaryWeaponMode.OnKeyPress += SelectToggleNextSecondaryWeaponMode;

      ButtonTogglePrevSecondaryWeaponMode.Text = "Prev Secondary Weapon";
      ButtonTogglePrevSecondaryWeaponMode.TextPosition = new TV_2DVECTOR(x, y);
      ButtonTogglePrevSecondaryWeaponMode.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonTogglePrevSecondaryWeaponMode.HighlightBoxPosition = ButtonTogglePrevSecondaryWeaponMode.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonTogglePrevSecondaryWeaponMode.HighlightBoxWidth = 450;
      ButtonTogglePrevSecondaryWeaponMode.HighlightBoxHeight = 30;
      ButtonTogglePrevSecondaryWeaponMode.Selectable = true;
      ButtonTogglePrevSecondaryWeaponMode.OnKeyPress += SelectTogglePrevSecondaryWeaponMode;

      ButtonToggleIncreaseSpeed.Text = "Increase Speed";
      ButtonToggleIncreaseSpeed.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleIncreaseSpeed.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleIncreaseSpeed.HighlightBoxPosition = ButtonToggleIncreaseSpeed.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleIncreaseSpeed.HighlightBoxWidth = 450;
      ButtonToggleIncreaseSpeed.HighlightBoxHeight = 30;
      ButtonToggleIncreaseSpeed.Selectable = true;
      ButtonToggleIncreaseSpeed.OnKeyPress += SelectToggleIncreaseSpeed;

      ButtonToggleDecreaseSpeed.Text = "Decrease Speed";
      ButtonToggleDecreaseSpeed.TextPosition = new TV_2DVECTOR(x, y);
      ButtonToggleDecreaseSpeed.SecondaryTextPosition = new TV_2DVECTOR(x + 350, y);
      y += height_gap;
      ButtonToggleDecreaseSpeed.HighlightBoxPosition = ButtonToggleDecreaseSpeed.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonToggleDecreaseSpeed.HighlightBoxWidth = 450;
      ButtonToggleDecreaseSpeed.HighlightBoxHeight = 30;
      ButtonToggleDecreaseSpeed.Selectable = true;
      ButtonToggleDecreaseSpeed.OnKeyPress += SelectToggleDecreaseSpeed;


      ButtonSaveAndExit.Text = "Save";
      ButtonSaveAndExit.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 120);
      ButtonSaveAndExit.HighlightBoxPosition = ButtonSaveAndExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSaveAndExit.HighlightBoxWidth = 200;
      ButtonSaveAndExit.HighlightBoxHeight = 30;
      ButtonSaveAndExit.Selectable = true;
      ButtonSaveAndExit.OnKeyPress += SelectSave;

      ButtonExit.Text = "Cancel";
      ButtonExit.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 80);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(MainText);
      Elements.Add(ButtonToggleUI);
      Elements.Add(ButtonToggleRadar);
      Elements.Add(ButtonToggleScenarioInfo);
      Elements.Add(ButtonToggleScoreboard);
      Elements.Add(ButtonToggleCameraMode);
      Elements.Add(ButtonToggleNextPrimaryWeaponMode);
      Elements.Add(ButtonTogglePrevPrimaryWeaponMode);
      Elements.Add(ButtonToggleNextSecondaryWeaponMode);
      Elements.Add(ButtonTogglePrevSecondaryWeaponMode);
      Elements.Add(ButtonToggleIncreaseSpeed);
      Elements.Add(ButtonToggleDecreaseSpeed);

      Elements.Add(ButtonSaveAndExit);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonToggleUI);

      LoadBindings();
    }

    private bool SelectToggleUI(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleUI = key;
        ButtonToggleUI.SecondaryText = KeyToggleUI.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleRadar(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleRadar = key;
        ButtonToggleRadar.SecondaryText = KeyToggleRadar.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleScenarioInfo(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleScenarioInfo = key;
        ButtonToggleScenarioInfo.SecondaryText = KeyToggleScenarioInfo.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleScoreboard(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleScoreboard = key;
        ButtonToggleScoreboard.SecondaryText = KeyToggleScoreboard.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleCameraMode(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleCameraMode = key;
        ButtonToggleCameraMode.SecondaryText = KeyToggleCameraMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleNextPrimaryWeaponMode(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleNextPrimaryWeaponMode = key;
        ButtonToggleNextPrimaryWeaponMode.SecondaryText = KeyToggleNextPrimaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectTogglePrevPrimaryWeaponMode(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyTogglePrevPrimaryWeaponMode = key;
        ButtonTogglePrevPrimaryWeaponMode.SecondaryText = KeyTogglePrevPrimaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleNextSecondaryWeaponMode(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleNextSecondaryWeaponMode = key;
        ButtonToggleNextSecondaryWeaponMode.SecondaryText = KeyToggleNextSecondaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectTogglePrevSecondaryWeaponMode(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyTogglePrevSecondaryWeaponMode = key;
        ButtonTogglePrevSecondaryWeaponMode.SecondaryText = KeyTogglePrevSecondaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleIncreaseSpeed(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleIncreaseSpeed = key;
        ButtonToggleIncreaseSpeed.SecondaryText = KeyToggleIncreaseSpeed.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectToggleDecreaseSpeed(CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        KeyToggleDecreaseSpeed = key;
        ButtonToggleDecreaseSpeed.SecondaryText = KeyToggleDecreaseSpeed.ToString().Replace("TV_KEY_", "").Replace("-1", "");
        return true;
      }
      return false;
    }

    private bool SelectSave(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        SaveBindings();
        Settings.SaveSettings();
        Back();
        return true;
      }
      return false;
    }

    private bool SelectExit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Back();
        return true;
      }
      return false;
    }

    private bool IsValidKeyBinding(CONST_TV_KEY key)
    {
      switch (key)
        {
        case CONST_TV_KEY.TV_KEY_0:
        case CONST_TV_KEY.TV_KEY_1:
        case CONST_TV_KEY.TV_KEY_2:
        case CONST_TV_KEY.TV_KEY_3:
        case CONST_TV_KEY.TV_KEY_4:
        case CONST_TV_KEY.TV_KEY_5:
        case CONST_TV_KEY.TV_KEY_6:
        case CONST_TV_KEY.TV_KEY_7:
        case CONST_TV_KEY.TV_KEY_8:
        case CONST_TV_KEY.TV_KEY_9:
        case CONST_TV_KEY.TV_KEY_A:
        case CONST_TV_KEY.TV_KEY_B:
        case CONST_TV_KEY.TV_KEY_C:
        case CONST_TV_KEY.TV_KEY_D:
        case CONST_TV_KEY.TV_KEY_E:
        case CONST_TV_KEY.TV_KEY_F:
        case CONST_TV_KEY.TV_KEY_G:
        case CONST_TV_KEY.TV_KEY_H:
        case CONST_TV_KEY.TV_KEY_I:
        case CONST_TV_KEY.TV_KEY_J:
        case CONST_TV_KEY.TV_KEY_K:
        case CONST_TV_KEY.TV_KEY_L:
        case CONST_TV_KEY.TV_KEY_M:
        case CONST_TV_KEY.TV_KEY_N:
        case CONST_TV_KEY.TV_KEY_O:
        case CONST_TV_KEY.TV_KEY_P:
        case CONST_TV_KEY.TV_KEY_Q:
        case CONST_TV_KEY.TV_KEY_R:
        case CONST_TV_KEY.TV_KEY_S:
        case CONST_TV_KEY.TV_KEY_T:
        case CONST_TV_KEY.TV_KEY_U:
        case CONST_TV_KEY.TV_KEY_V:
        case CONST_TV_KEY.TV_KEY_W:
        case CONST_TV_KEY.TV_KEY_X:
        case CONST_TV_KEY.TV_KEY_Y:
        case CONST_TV_KEY.TV_KEY_Z:
        //case CONST_TV_KEY.TV_KEY_UP:
        //case CONST_TV_KEY.TV_KEY_DOWN:
        //case CONST_TV_KEY.TV_KEY_LEFT:
        //case CONST_TV_KEY.TV_KEY_RIGHT:
        case CONST_TV_KEY.TV_KEY_HOME:
        case CONST_TV_KEY.TV_KEY_INSERT:
        case CONST_TV_KEY.TV_KEY_PAGEDOWN:
        case CONST_TV_KEY.TV_KEY_PAGEUP:
        case CONST_TV_KEY.TV_KEY_NUMPAD0:
        case CONST_TV_KEY.TV_KEY_NUMPAD1:
        case CONST_TV_KEY.TV_KEY_NUMPAD2:
        case CONST_TV_KEY.TV_KEY_NUMPAD3:
        case CONST_TV_KEY.TV_KEY_NUMPAD4:
        case CONST_TV_KEY.TV_KEY_NUMPAD5:
        case CONST_TV_KEY.TV_KEY_NUMPAD6:
        case CONST_TV_KEY.TV_KEY_NUMPAD7:
        case CONST_TV_KEY.TV_KEY_NUMPAD8:
        case CONST_TV_KEY.TV_KEY_NUMPAD9:
        case CONST_TV_KEY.TV_KEY_NUMPADCOMMA:
        case CONST_TV_KEY.TV_KEY_NUMPADENTER:
        case CONST_TV_KEY.TV_KEY_NUMPADEQUALS:
        case CONST_TV_KEY.TV_KEY_NUMPADMINUS:
        case CONST_TV_KEY.TV_KEY_NUMPADPERIOD:
        case CONST_TV_KEY.TV_KEY_NUMPADPLUS:
        case CONST_TV_KEY.TV_KEY_NUMPADSLASH:
        case CONST_TV_KEY.TV_KEY_NUMPADSTAR:
        case CONST_TV_KEY.TV_KEY_COMMA:
        case CONST_TV_KEY.TV_KEY_COLON:
        case CONST_TV_KEY.TV_KEY_SEMICOLON:
        case CONST_TV_KEY.TV_KEY_SLASH:
        case CONST_TV_KEY.TV_KEY_SPACE:
        case CONST_TV_KEY.TV_KEY_DELETE:
          return true;
      }

      return false;
    }

    private void SaveBindings()
    {
      InputFunction.Registry.Get(ToggleUIVisibility.InternalName).Key = (int)KeyToggleUI;
      InputFunction.Registry.Get(ToggleRadarVisibility.InternalName).Key = (int)KeyToggleRadar;
      InputFunction.Registry.Get(ToggleStatusVisibility.InternalName).Key = (int)KeyToggleScenarioInfo;
      InputFunction.Registry.Get(ToggleScoreVisibility.InternalName).Key = (int)KeyToggleScoreboard;

      InputFunction.Registry.Get(NextCameraMode.InternalName).Key = (int)KeyToggleCameraMode;
      InputFunction.Registry.Get(NextPrimary.InternalName).Key = (int)KeyToggleNextPrimaryWeaponMode;
      InputFunction.Registry.Get(PrevPrimary.InternalName).Key = (int)KeyTogglePrevPrimaryWeaponMode;
      InputFunction.Registry.Get(NextSecondary.InternalName).Key = (int)KeyToggleNextSecondaryWeaponMode;
      InputFunction.Registry.Get(PrevSecondary.InternalName).Key = (int)KeyTogglePrevSecondaryWeaponMode;

      InputFunction.Registry.Get(Up.InternalName).Key = (int)KeyToggleIncreaseSpeed;
      InputFunction.Registry.Get(Down.InternalName).Key = (int)KeyToggleDecreaseSpeed;

      LoadBindings();
    }

    private void LoadBindings()
    {
      KeyToggleUI = (CONST_TV_KEY)InputFunction.Registry.Get(ToggleUIVisibility.InternalName).Key;
      KeyToggleRadar = (CONST_TV_KEY)InputFunction.Registry.Get(ToggleRadarVisibility.InternalName).Key;
      KeyToggleScenarioInfo = (CONST_TV_KEY)InputFunction.Registry.Get(ToggleStatusVisibility.InternalName).Key;
      KeyToggleScoreboard = (CONST_TV_KEY)InputFunction.Registry.Get(ToggleScoreVisibility.InternalName).Key;

      KeyToggleCameraMode = (CONST_TV_KEY)InputFunction.Registry.Get(NextCameraMode.InternalName).Key;
      KeyToggleNextPrimaryWeaponMode = (CONST_TV_KEY)InputFunction.Registry.Get(NextPrimary.InternalName).Key;
      KeyTogglePrevPrimaryWeaponMode = (CONST_TV_KEY)InputFunction.Registry.Get(PrevPrimary.InternalName).Key;
      KeyToggleNextSecondaryWeaponMode = (CONST_TV_KEY)InputFunction.Registry.Get(NextSecondary.InternalName).Key;
      KeyTogglePrevSecondaryWeaponMode = (CONST_TV_KEY)InputFunction.Registry.Get(PrevSecondary.InternalName).Key;

      KeyToggleIncreaseSpeed = (CONST_TV_KEY)InputFunction.Registry.Get(Up.InternalName).Key;
      KeyToggleDecreaseSpeed = (CONST_TV_KEY)InputFunction.Registry.Get(Down.InternalName).Key;

      ButtonToggleUI.SecondaryText = KeyToggleUI.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleRadar.SecondaryText = KeyToggleRadar.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleScenarioInfo.SecondaryText = KeyToggleScenarioInfo.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleScoreboard.SecondaryText = KeyToggleScoreboard.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleCameraMode.SecondaryText = KeyToggleCameraMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleNextPrimaryWeaponMode.SecondaryText = KeyToggleNextPrimaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonTogglePrevPrimaryWeaponMode.SecondaryText = KeyTogglePrevPrimaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleNextSecondaryWeaponMode.SecondaryText = KeyToggleNextSecondaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonTogglePrevSecondaryWeaponMode.SecondaryText = KeyTogglePrevSecondaryWeaponMode.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleIncreaseSpeed.SecondaryText = KeyToggleIncreaseSpeed.ToString().Replace("TV_KEY_", "").Replace("-1", "");
      ButtonToggleDecreaseSpeed.SecondaryText = KeyToggleDecreaseSpeed.ToString().Replace("TV_KEY_", "").Replace("-1", "");
    }
  }
}
