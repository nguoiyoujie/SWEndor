﻿using MTV3D65;
using SWEndor.Game.Sound;
using System.Collections.Generic;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class OptionsMenu : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonMusicVol = new SelectionElement();
    SelectionElement ButtonSFXVol = new SelectionElement();
    SelectionElement ButtonSpeechVol = new SelectionElement();
    SelectionElement ButtonSteeringSensitivity = new SelectionElement();
    SelectionElement ButtonKeyboardBindings = new SelectionElement();
    SelectionElement ButtonConfigSettings = new SelectionElement();
    SelectionElement ButtonModelViewerMenu = new SelectionElement();
    SelectionElement ButtonGoToProfilerMenu = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();


    public OptionsMenu(Screen2D owner) : base(owner)
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND);

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
        && float.Parse(ButtonMusicVol.ToggleButtonsValues[ButtonMusicVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) < Engine.SoundManager.MasterMusicVolume)
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
        && float.Parse(ButtonSFXVol.ToggleButtonsValues[ButtonSFXVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) <= Engine.SoundManager.MasterSFXVolume)
      {
        ButtonSFXVol.ToggleButtonsCurrentNumber++;
      }
      ButtonSFXVol.AfterKeyPress += SelectSFXVol;

      ButtonSpeechVol.Text = "Speech Volume";
      ButtonSpeechVol.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonSpeechVol.HighlightBoxPosition = ButtonSpeechVol.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSpeechVol.HighlightBoxWidth = 500;
      ButtonSpeechVol.HighlightBoxHeight = 30;
      ButtonSpeechVol.Selectable = true;
      ButtonSpeechVol.ToggleButtonsPosition = ButtonSpeechVol.TextPosition + new TV_2DVECTOR(250, 0);
      ButtonSpeechVol.ToggleButtonsNumber = 11;
      ButtonSpeechVol.ToggleButtonsValues = new List<string> { "0", "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1" };
      ButtonSpeechVol.ToggleButtonsCurrentNumber = 0;
      while (ButtonSpeechVol.ToggleButtonsCurrentNumber < ButtonSpeechVol.ToggleButtonsValues.Count
        && float.Parse(ButtonSpeechVol.ToggleButtonsValues[ButtonSpeechVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) <= Engine.SoundManager.MasterSFXVolume)
      {
        ButtonSpeechVol.ToggleButtonsCurrentNumber++;
      }
      ButtonSpeechVol.AfterKeyPress += SelectSpeechVol;

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
        && float.Parse(ButtonSteeringSensitivity.ToggleButtonsValues[ButtonSteeringSensitivity.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) < Engine.Settings.SteeringSensitivity)
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

      if (Engine.GameScenarioManager.IsMainMenu)
      {
        ButtonModelViewerMenu.Text = "Enter Model Viewer";
        ButtonModelViewerMenu.TextPosition = new TV_2DVECTOR(x, y);
        y += height_gap;
        ButtonModelViewerMenu.HighlightBoxPosition = ButtonModelViewerMenu.TextPosition - new TV_2DVECTOR(5, 5);
        ButtonModelViewerMenu.HighlightBoxWidth = 600;
        ButtonModelViewerMenu.HighlightBoxHeight = 30;
        ButtonModelViewerMenu.Selectable = true;
        ButtonModelViewerMenu.OnKeyPress += PlayModelViewer;
      }

      ButtonGoToProfilerMenu.Text = "Go To Profilers";
      ButtonGoToProfilerMenu.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonGoToProfilerMenu.HighlightBoxPosition = ButtonGoToProfilerMenu.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonGoToProfilerMenu.HighlightBoxWidth = 600;
      ButtonGoToProfilerMenu.HighlightBoxHeight = 30;
      ButtonGoToProfilerMenu.Selectable = true;
      ButtonGoToProfilerMenu.OnKeyPress += GoToProfilers;

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
      Elements.Add(ButtonSpeechVol);
      Elements.Add(ButtonSteeringSensitivity);
      Elements.Add(ButtonKeyboardBindings);
      Elements.Add(ButtonConfigSettings);
      if (Engine.GameScenarioManager.IsMainMenu)
        Elements.Add(ButtonModelViewerMenu);
      Elements.Add(ButtonGoToProfilerMenu);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonMusicVol);
    }

    private bool SelectMusicVol(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        //ButtonMusicVol.SecondaryText = ButtonMusicVol.ToggleButtonsCurrentNumber.ToString();
        Engine.SoundManager.MasterMusicVolume = float.Parse(ButtonMusicVol.ToggleButtonsValues[ButtonMusicVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        Engine.Settings.SaveSettings(Engine);
      }
      return false;
    }

    private bool SelectSFXVol(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Engine.SoundManager.MasterSFXVolume = float.Parse(ButtonSFXVol.ToggleButtonsValues[ButtonSFXVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        Engine.SoundManager.SetSound(SoundGlobals.LostShip);
        Engine.Settings.SaveSettings(Engine);
      }
      return false;
    }

    private bool SelectSpeechVol(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Engine.SoundManager.MasterSpeechVolume = float.Parse(ButtonSFXVol.ToggleButtonsValues[ButtonSFXVol.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        var speech = SoundGlobals.TestSpeech;
        Engine.SoundManager.QueueSpeech(speech[Engine.Random.Next(0, speech.Length)]);
        Engine.Settings.SaveSettings(Engine);
      }
      return false;
    }
    private bool SelectSteeringSensitivity(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Engine.Settings.SteeringSensitivity = float.Parse(ButtonSteeringSensitivity.ToggleButtonsValues[ButtonSteeringSensitivity.ToggleButtonsCurrentNumber], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        Engine.Settings.SaveSettings(Engine);
      }
      return false;
    }

    private bool SelectKeyboardBindings(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new KeyboardControls(Owner));
        return true;
      }
      return false;
    }

    private bool SelectConfigSettings(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ConfigSettingsMenu(Owner));
        return true;
      }
      return false;
    }

    private bool PlayModelViewer(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        GameScenarioManager.Reset();
        GameScenarioManager.LoadModelViewer();
        return true;
      }
      return false;
    }

    private bool GoToProfilers(CONST_TV_KEY key)
    {

      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ProfileSettingsMenu(Owner));
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