using MTV3D65;
using System.Collections.Generic;

namespace SWEndor.UI.Menu.Pages
{
  public class OptionsMenu : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonMusicVol = new SelectionElement();
    SelectionElement ButtonSFXVol = new SelectionElement();
    SelectionElement ButtonSteeringSensitivity = new SelectionElement();
    SelectionElement ButtonKeyboardBindings = new SelectionElement();
    SelectionElement ButtonConfigSettings = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();


    public OptionsMenu()
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Globals.Engine.ScreenWidth;
      Cover.HighlightBoxHeight = Globals.Engine.ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      MainText.Text = "Options Menu";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      ButtonMusicVol.Text = "Music Volume";
      ButtonMusicVol.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonMusicVol.HighlightBoxPosition = ButtonMusicVol.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonMusicVol.HighlightBoxWidth = 500;
      ButtonMusicVol.HighlightBoxHeight = 30;
      ButtonMusicVol.Selectable = true;
      ButtonMusicVol.ToggleButtonsPosition = ButtonMusicVol.TextPosition + new TV_2DVECTOR(250, 0);
      ButtonMusicVol.ToggleButtonsNumber = 11;
      ButtonMusicVol.ToggleButtonsValues = new List<string> { "0", "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1" };
      ButtonMusicVol.ToggleButtonsCurrentNumber = 0;
      while (ButtonMusicVol.ToggleButtonsCurrentNumber < ButtonMusicVol.ToggleButtonsValues.Count 
        && float.Parse(ButtonMusicVol.ToggleButtonsValues[ButtonMusicVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) < Globals.Engine.SoundManager.MasterMusicVolume)
      {
        ButtonMusicVol.ToggleButtonsCurrentNumber++;
      }
      ButtonMusicVol.AfterKeyPress += SelectMusicVol;

      ButtonSFXVol.Text = "SFX Volume";
      ButtonSFXVol.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonSFXVol.HighlightBoxPosition = ButtonSFXVol.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSFXVol.HighlightBoxWidth = 500;
      ButtonSFXVol.HighlightBoxHeight = 30;
      ButtonSFXVol.Selectable = true;
      ButtonSFXVol.ToggleButtonsPosition = ButtonSFXVol.TextPosition + new TV_2DVECTOR(250, 0);
      ButtonSFXVol.ToggleButtonsNumber = 11;
      ButtonSFXVol.ToggleButtonsValues = new List<string> { "0", "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1" };
      ButtonSFXVol.ToggleButtonsCurrentNumber = 0;
      while (ButtonSFXVol.ToggleButtonsCurrentNumber < ButtonSFXVol.ToggleButtonsValues.Count
        && float.Parse(ButtonSFXVol.ToggleButtonsValues[ButtonSFXVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) <= Globals.Engine.SoundManager.MasterSFXVolume)
      {
        ButtonSFXVol.ToggleButtonsCurrentNumber++;
      }
      ButtonSFXVol.AfterKeyPress += SelectSFXVol;

      ButtonSteeringSensitivity.Text = "Steering Sensitivity";
      ButtonSteeringSensitivity.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonSteeringSensitivity.HighlightBoxPosition = ButtonSteeringSensitivity.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSteeringSensitivity.HighlightBoxWidth = 500;
      ButtonSteeringSensitivity.HighlightBoxHeight = 30;
      ButtonSteeringSensitivity.Selectable = true;
      ButtonSteeringSensitivity.ToggleButtonsPosition = ButtonSteeringSensitivity.TextPosition + new TV_2DVECTOR(250, 0);
      ButtonSteeringSensitivity.ToggleButtonsNumber = 11;
      ButtonSteeringSensitivity.ToggleButtonsValues = new List<string> { "0.25", "0.33", "0.5", "0.75", "1", "1.25", "1.5", "2", "2.5", "3", "4" };
      ButtonSteeringSensitivity.ToggleButtonsCurrentNumber = 0;
      while (ButtonSteeringSensitivity.ToggleButtonsCurrentNumber < ButtonSteeringSensitivity.ToggleButtonsValues.Count
        && float.Parse(ButtonSteeringSensitivity.ToggleButtonsValues[ButtonSteeringSensitivity.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) < Settings.SteeringSensitivity)
      {
        ButtonSteeringSensitivity.ToggleButtonsCurrentNumber++;
      }
      ButtonSteeringSensitivity.AfterKeyPress += SelectSteeringSensitivity;

      ButtonKeyboardBindings.Text = "Keyboard Bindings";
      ButtonKeyboardBindings.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonKeyboardBindings.HighlightBoxPosition = ButtonKeyboardBindings.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonKeyboardBindings.HighlightBoxWidth = 500;
      ButtonKeyboardBindings.HighlightBoxHeight = 30;
      ButtonKeyboardBindings.Selectable = true;
      ButtonKeyboardBindings.OnKeyPress += SelectKeyboardBindings;

      ButtonConfigSettings.Text = "Enter Configuration Menu";
      ButtonConfigSettings.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonConfigSettings.HighlightBoxPosition = ButtonConfigSettings.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonConfigSettings.HighlightBoxWidth = 500;
      ButtonConfigSettings.HighlightBoxHeight = 30;
      ButtonConfigSettings.Selectable = true;
      ButtonConfigSettings.OnKeyPress += SelectConfigSettings;

      ButtonExit.Text = "Back";
      ButtonExit.TextPosition = new TV_2DVECTOR(x, y);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(Cover);
      Elements.Add(MainText);
      Elements.Add(ButtonMusicVol);
      Elements.Add(ButtonSFXVol);
      Elements.Add(ButtonSteeringSensitivity);
      Elements.Add(ButtonKeyboardBindings);
      Elements.Add(ButtonConfigSettings);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonMusicVol);
    }

    private bool SelectMusicVol(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        //ButtonMusicVol.SecondaryText = ButtonMusicVol.ToggleButtonsCurrentNumber.ToString();
        Globals.Engine.SoundManager.MasterMusicVolume = float.Parse(ButtonMusicVol.ToggleButtonsValues[ButtonMusicVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        Settings.SaveSettings();
      }
      return false;
    }

    private bool SelectSFXVol(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Globals.Engine.SoundManager.MasterSFXVolume = float.Parse(ButtonSFXVol.ToggleButtonsValues[ButtonSFXVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        Globals.Engine.SoundManager.SetSound("shieldlow");
        Settings.SaveSettings();
      }
      return false;
    }

    private bool SelectSteeringSensitivity(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Settings.SteeringSensitivity = float.Parse(ButtonSteeringSensitivity.ToggleButtonsValues[ButtonSteeringSensitivity.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        Settings.SaveSettings();
      }
      return false;
    }

    private bool SelectKeyboardBindings(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new KeyboardControls());
        return true;
      }
      return false;
    }

    private bool SelectConfigSettings(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ConfigSettingsMenu());
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
  }
}
