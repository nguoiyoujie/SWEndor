using MTV3D65;

namespace SWEndor.UI.Menu.Pages
{
  public class MainMenu : Page
  {
    SelectionElement MainMenuText = new SelectionElement();
    SelectionElement VersionText = new SelectionElement();
    SelectionElement ButtonPlay = new SelectionElement();
    //UISelectionElement ButtonLoad = new UISelectionElement();
    SelectionElement ButtonOptions = new SelectionElement();
    SelectionElement ButtonCredits = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();


    public MainMenu()
    {
      MainMenuText.Text = "<Title>";
      MainMenuText.TextPosition = new TV_2DVECTOR(75, 60);
      MainMenuText.TextFont = Font.Factory.Get("Title_48").ID;

      VersionText.Text = Globals.Version;
      VersionText.TextPosition = new TV_2DVECTOR(85, 150);
      VersionText.TextFont = Font.Factory.Get("Text_12").ID;

      ButtonPlay.Text = "Play Scenario";
      ButtonPlay.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth - 200, Globals.Engine.ScreenHeight - 300);
      ButtonPlay.HighlightBoxPosition = ButtonPlay.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonPlay.HighlightBoxWidth = 200;
      ButtonPlay.HighlightBoxHeight = 30;
      ButtonPlay.Selectable = true;
      ButtonPlay.OnKeyPress += SelectPlay;

      /*
      ButtonLoad.Text = "Load Scenario";
      ButtonLoad.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth - 200, Globals.Engine.ScreenHeight - 260);
      ButtonLoad.HighlightBoxPosition = ButtonLoad.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonLoad.HighlightBoxWidth = 200;
      ButtonLoad.HighlightBoxHeight = 30;
      ButtonLoad.Selectable = true;
      ButtonLoad.OnKeyPress += SelectLoad;
      */

      ButtonOptions.Text = "Options";
      ButtonOptions.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth - 200, Globals.Engine.ScreenHeight - 220);
      ButtonOptions.HighlightBoxPosition = ButtonOptions.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonOptions.HighlightBoxWidth = 200;
      ButtonOptions.HighlightBoxHeight = 30;
      ButtonOptions.Selectable = true;
      ButtonOptions.OnKeyPress += SelectOptions;

      ButtonCredits.Text = "Credits";
      ButtonCredits.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth - 200, Globals.Engine.ScreenHeight - 180);
      ButtonCredits.HighlightBoxPosition = ButtonCredits.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonCredits.HighlightBoxWidth = 200;
      ButtonCredits.HighlightBoxHeight = 30;
      ButtonCredits.Selectable = true;
      ButtonCredits.OnKeyPress += SelectCredits;

      ButtonExit.Text = "Exit";
      ButtonExit.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth - 200, Globals.Engine.ScreenHeight - 140);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(MainMenuText);
      Elements.Add(VersionText);
      Elements.Add(ButtonPlay);
      //Elements.Add(ButtonLoad);
      Elements.Add(ButtonOptions);
      Elements.Add(ButtonCredits);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonPlay);
    }

    private bool SelectPlay(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new PlayScenario());
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

    private bool SelectCredits(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new Credits());
        return true;
      }
      return false;
    }

    private bool SelectExit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ConfirmExit());
        return true;
      }
      return false;
    }
  }
}
