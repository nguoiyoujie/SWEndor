﻿using MTV3D65;
using SWEndor.Sound;

namespace SWEndor.UI.Menu.Pages
{
  public class PauseMenu : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonResume = new SelectionElement();
    //UISelectionElement ButtonSave = new UISelectionElement();
    //UISelectionElement ButtonLoad = new UISelectionElement();
    SelectionElement ButtonOptions = new SelectionElement();
    SelectionElement ButtonQuit = new SelectionElement();


    public PauseMenu()
    {
      SoundManager.Instance().SetMusicPause();

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Engine.Instance().ScreenWidth;
      Cover.HighlightBoxHeight = Engine.Instance().ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      MainText.Text = "Game Paused";
      MainText.TextPosition = new TV_2DVECTOR(100, 80);

      ButtonResume.Text = "Resume";
      ButtonResume.TextPosition = new TV_2DVECTOR(200, 200);
      ButtonResume.HighlightBoxPosition = ButtonResume.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonResume.HighlightBoxWidth = 200;
      ButtonResume.HighlightBoxHeight = 30;
      ButtonResume.Selectable = true;
      ButtonResume.OnKeyPress += SelectResume;

      /*
      ButtonSave.Text = "Save Scenario";
      ButtonSave.TextPosition = new TV_2DVECTOR(200, 240);
      ButtonSave.HighlightBoxPosition = ButtonSave.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSave.HighlightBoxWidth = 200;
      ButtonSave.HighlightBoxHeight = 30;
      ButtonSave.Selectable = true;
      ButtonSave.OnKeyPress += SelectSave;

      ButtonLoad.Text = "Load Scenario";
      ButtonLoad.TextPosition = new TV_2DVECTOR(200, 280);
      ButtonLoad.HighlightBoxPosition = ButtonLoad.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonLoad.HighlightBoxWidth = 200;
      ButtonLoad.HighlightBoxHeight = 30;
      ButtonLoad.Selectable = true;
      ButtonLoad.OnKeyPress += SelectLoad;
      */

      ButtonOptions.Text = "Options";
      ButtonOptions.TextPosition = new TV_2DVECTOR(200, 320);
      ButtonOptions.HighlightBoxPosition = ButtonOptions.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonOptions.HighlightBoxWidth = 200;
      ButtonOptions.HighlightBoxHeight = 30;
      ButtonOptions.Selectable = true;
      ButtonOptions.OnKeyPress += SelectOptions;

      ButtonQuit.Text = "Quit";
      ButtonQuit.TextPosition = new TV_2DVECTOR(200, 360);
      ButtonQuit.HighlightBoxPosition = ButtonQuit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonQuit.HighlightBoxWidth = 200;
      ButtonQuit.HighlightBoxHeight = 30;
      ButtonQuit.Selectable = true;
      ButtonQuit.OnKeyPress += SelectQuit;

      Elements.Add(Cover);
      Elements.Add(MainText);
      Elements.Add(ButtonResume);
      //Elements.Add(ButtonSave);
      //Elements.Add(ButtonLoad);
      Elements.Add(ButtonOptions);
      Elements.Add(ButtonQuit);
      SelectedElementID = Elements.IndexOf(ButtonResume);
    }

    private bool SelectResume(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        ResumeGame();
        return true;
      }
      return false;
    }

    private void ResumeGame()
    {
      SoundManager.Instance().SetMusicResume();
      Game.Instance().IsPaused = false;
      Screen2D.Instance().CurrentPage = null;
      Screen2D.Instance().ShowPage = false;
    }

    private bool SelectSave(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        GameSaver.Save(@"save.txt");
        return true;
      }
      return false;
    }

    private bool SelectLoad(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        //EnterPage(null);
        return true;
      }
      return false;
    }

    private bool SelectOptions(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new OptionsMenu());
        return true;
      }
      return false;
    }

    private bool SelectQuit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ConfirmExitScenario());
        return true;
      }
      return false;
    }

    public override bool OnKeyPress(CONST_TV_KEY key)
    {
      if (base.OnKeyPress(key))
        return true;

      switch (key)
      {
        case CONST_TV_KEY.TV_KEY_ESCAPE:
          ResumeGame();
          return true;
      }
      return false;
    }
  }
}