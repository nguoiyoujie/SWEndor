using MTV3D65;

namespace SWEndor.UI.Menu.Pages
{
  public class PauseMenu : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonResume = new SelectionElement();
    SelectionElement ButtonMap = new SelectionElement();
    SelectionElement ButtonMsgLog = new SelectionElement();
    SelectionElement ButtonOptions = new SelectionElement();
    SelectionElement ButtonQuit = new SelectionElement();


    public PauseMenu(Screen2D owner) : base(owner)
    {
      Engine.SoundManager.SetMusicPause();

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND);

      MainText.Text = "Game Paused";
      MainText.TextPosition = new TV_2DVECTOR(100, 80);

      float y = 200;
      ButtonResume.Text = "Resume";
      ButtonResume.TextPosition = new TV_2DVECTOR(200, y);
      ButtonResume.HighlightBoxPosition = ButtonResume.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonResume.HighlightBoxWidth = 200;
      ButtonResume.HighlightBoxHeight = 30;
      ButtonResume.Selectable = true;
      ButtonResume.OnKeyPress += SelectResume;

      y += 40;
      ButtonMap.Text = "Map";
      ButtonMap.TextPosition = new TV_2DVECTOR(200, y);
      ButtonMap.HighlightBoxPosition = ButtonMap.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonMap.HighlightBoxWidth = 200;
      ButtonMap.HighlightBoxHeight = 30;
      ButtonMap.Selectable = true;
      ButtonMap.OnKeyPress += SelectMap;

      y += 40;
      ButtonMsgLog.Text = "Message Log";
      ButtonMsgLog.TextPosition = new TV_2DVECTOR(200, y);
      ButtonMsgLog.HighlightBoxPosition = ButtonMsgLog.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonMsgLog.HighlightBoxWidth = 200;
      ButtonMsgLog.HighlightBoxHeight = 30;
      ButtonMsgLog.Selectable = true;
      ButtonMsgLog.OnKeyPress += SelectMsgLog;

      y += 40;
      ButtonOptions.Text = "Options";
      ButtonOptions.TextPosition = new TV_2DVECTOR(200, y);
      ButtonOptions.HighlightBoxPosition = ButtonOptions.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonOptions.HighlightBoxWidth = 200;
      ButtonOptions.HighlightBoxHeight = 30;
      ButtonOptions.Selectable = true;
      ButtonOptions.OnKeyPress += SelectOptions;

      y += 40;
      ButtonQuit.Text = "Quit";
      ButtonQuit.TextPosition = new TV_2DVECTOR(200, y);
      ButtonQuit.HighlightBoxPosition = ButtonQuit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonQuit.HighlightBoxWidth = 200;
      ButtonQuit.HighlightBoxHeight = 30;
      ButtonQuit.Selectable = true;
      ButtonQuit.OnKeyPress += SelectQuit;

      Elements.Add(Cover);
      Elements.Add(MainText);
      Elements.Add(ButtonResume);
      Elements.Add(ButtonMap);
      Elements.Add(ButtonMsgLog);
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

    private bool SelectMap(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ScenarioMap(Owner, Engine.GameScenarioManager.Scenario));
        return true;
      }
      return false;
    }

    private bool SelectMsgLog(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ScenarioMessageLog(Owner, Engine.GameScenarioManager.Scenario));
        return true;
      }
      return false;
    }

    private void ResumeGame()
    {
      Engine.SoundManager.SetMusicResume();
      Engine.Game.IsPaused = false;
      Engine.Screen2D.CurrentPage = null;
      Engine.Screen2D.ShowPage = false;
    }

    /*
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
    */

    private bool SelectOptions(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new OptionsMenu(Owner));
        return true;
      }
      return false;
    }

    private bool SelectQuit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ConfirmExitScenario(Owner));
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
