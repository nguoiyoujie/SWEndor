using MTV3D65;
using SWEndor.Game.Input.Functions;
using SWEndor.Game.Input.Functions.Gameplay.Camera;
using SWEndor.Game.Input.Functions.Gameplay.Speed;
using SWEndor.Game.Input.Functions.Gameplay.UI;
using SWEndor.Game.Input.Functions.Gameplay.Weapon;
using Primrose.Primitives;
using System.Collections.Generic;

namespace SWEndor.Game.UI.Menu.Pages
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

    public KeyboardControls(Screen2D owner) : base(owner)
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
      ButtonSaveAndExit.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -120); 
      ButtonSaveAndExit.HighlightBoxPosition = ButtonSaveAndExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSaveAndExit.HighlightBoxWidth = 200;
      ButtonSaveAndExit.HighlightBoxHeight = 30;
      ButtonSaveAndExit.Selectable = true;
      ButtonSaveAndExit.OnKeyPress += SelectSave;

      ButtonExit.Text = "Cancel";
      ButtonExit.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -80);
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

      DefineBindings();
      LoadBindings();
    }


    private bool Select(SelectionElement sel, CONST_TV_KEY key)
    {
      if (IsValidKeyBinding(key))
      {
        _bindvals[sel] = key;
        sel.SecondaryText = key.GetEnumName()?.Replace("TV_KEY_", "") ?? "";
        return true;
      }
      return false;
    }

    private bool SelectToggleUI(CONST_TV_KEY key)
    {
      return Select(ButtonToggleUI, key);
    }

    private bool SelectToggleRadar(CONST_TV_KEY key)
    {
      return Select(ButtonToggleRadar, key);
    }

    private bool SelectToggleScenarioInfo(CONST_TV_KEY key)
    {
      return Select(ButtonToggleScenarioInfo, key);
    }

    private bool SelectToggleScoreboard(CONST_TV_KEY key)
    {
      return Select(ButtonToggleScoreboard, key);
    }

    private bool SelectToggleCameraMode(CONST_TV_KEY key)
    {
      return Select(ButtonToggleCameraMode, key);
    }

    private bool SelectToggleNextPrimaryWeaponMode(CONST_TV_KEY key)
    {
      return Select(ButtonToggleNextPrimaryWeaponMode, key);
    }

    private bool SelectTogglePrevPrimaryWeaponMode(CONST_TV_KEY key)
    {
      return Select(ButtonTogglePrevPrimaryWeaponMode, key);
    }

    private bool SelectToggleNextSecondaryWeaponMode(CONST_TV_KEY key)
    {
      return Select(ButtonToggleNextSecondaryWeaponMode, key);
    }

    private bool SelectTogglePrevSecondaryWeaponMode(CONST_TV_KEY key)
    {
      return Select(ButtonTogglePrevSecondaryWeaponMode, key);
    }

    private bool SelectToggleIncreaseSpeed(CONST_TV_KEY key)
    {
      return Select(ButtonToggleIncreaseSpeed, key);
    }

    private bool SelectToggleDecreaseSpeed(CONST_TV_KEY key)
    {
      return Select(ButtonToggleDecreaseSpeed, key);

    }

    private bool SelectSave(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        SaveBindings();
        Engine.Settings.SaveSettings(Engine);
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
      foreach (SelectionElement e in _bindings.Keys)
        InputFunction.Registry.Get(_bindings[e]).Key = (int)_bindvals[e];

      LoadBindings();
    }

    private Dictionary<SelectionElement, string> _bindings = new Dictionary<SelectionElement, string>();
    private Dictionary<SelectionElement, CONST_TV_KEY> _bindvals = new Dictionary<SelectionElement, CONST_TV_KEY>();

    private void DefineBindings()
    {
      _bindings.Clear();
      _bindings.Add(ButtonToggleUI, ToggleUIVisibility.InternalName);
      _bindings.Add(ButtonToggleRadar, ToggleRadarVisibility.InternalName);
      _bindings.Add(ButtonToggleScenarioInfo, ToggleStatusVisibility.InternalName);
      _bindings.Add(ButtonToggleScoreboard, ToggleScoreVisibility.InternalName);
      _bindings.Add(ButtonToggleCameraMode, NextCameraMode.InternalName);
      _bindings.Add(ButtonToggleNextPrimaryWeaponMode, NextPrimary.InternalName);
      _bindings.Add(ButtonTogglePrevPrimaryWeaponMode, PrevPrimary.InternalName);
      _bindings.Add(ButtonToggleNextSecondaryWeaponMode, NextSecondary.InternalName);
      _bindings.Add(ButtonTogglePrevSecondaryWeaponMode, PrevSecondary.InternalName);
      _bindings.Add(ButtonToggleIncreaseSpeed, Up.InternalName);
      _bindings.Add(ButtonToggleDecreaseSpeed, Down.InternalName);

      foreach (SelectionElement e in _bindings.Keys)
        _bindvals.Add(e, 0);
    }

    private void LoadBindings()
    {
      foreach (SelectionElement e in _bindings.Keys)
      {
        _bindvals[e] = (CONST_TV_KEY)InputFunction.Registry.Get(_bindings[e]).Key;
        e.SecondaryText = _bindvals[e].GetEnumName()?.Replace("TV_KEY_", "") ?? "";
      }
    }
  }
}
