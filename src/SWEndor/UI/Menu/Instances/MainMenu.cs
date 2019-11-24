using MTV3D65;

namespace SWEndor.UI.Menu.Pages
{
  public class MainMenu : Page
  {
    SelectionElement MainMenuText = new SelectionElement();
    SelectionElement VersionText = new SelectionElement();
    SelectionElement ButtonPlay = new SelectionElement();
    SelectionElement ButtonOptions = new SelectionElement();
    SelectionElement ButtonCredits = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();


    public MainMenu(Screen2D owner) : base(owner)
    {
      MainMenuText.Text = "SW Endor";
      MainMenuText.TextPosition = new TV_2DVECTOR(75, 60);
      MainMenuText.TextFont = FontFactory.Get(Font.T48).ID;

      VersionText.Text = Globals.Version;
      VersionText.TextPosition = new TV_2DVECTOR(85, 150);
      VersionText.TextFont = FontFactory.Get(Font.T12).ID;

      ButtonPlay.Text = "Play Scenario";
      ButtonPlay.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -300);
      ButtonPlay.HighlightBoxPosition = ButtonPlay.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonPlay.HighlightBoxWidth = 200;
      ButtonPlay.HighlightBoxHeight = 30;
      ButtonPlay.Selectable = true;
      ButtonPlay.OnKeyPress += SelectPlay;

      ButtonOptions.Text = "Options";
      ButtonOptions.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -220);
      ButtonOptions.HighlightBoxPosition = ButtonOptions.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonOptions.HighlightBoxWidth = 200;
      ButtonOptions.HighlightBoxHeight = 30;
      ButtonOptions.Selectable = true;
      ButtonOptions.OnKeyPress += SelectOptions;

      ButtonCredits.Text = "Credits";
      ButtonCredits.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -180);
      ButtonCredits.HighlightBoxPosition = ButtonCredits.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonCredits.HighlightBoxWidth = 200;
      ButtonCredits.HighlightBoxHeight = 30;
      ButtonCredits.Selectable = true;
      ButtonCredits.OnKeyPress += SelectCredits;

      ButtonExit.Text = "Exit";
      ButtonExit.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -140);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(MainMenuText);
      Elements.Add(VersionText);
      Elements.Add(ButtonPlay);
      Elements.Add(ButtonOptions);
      Elements.Add(ButtonCredits);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonPlay);
    }

    private bool SelectPlay(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new PlayScenario(Owner));
        return true;
      }
      return false;
    }

    private bool SelectOptions(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new OptionsMenu(Owner));
        return true;
      }
      return false;
    }

    private bool SelectCredits(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new Credits(Owner));
        return true;
      }
      return false;
    }

    private bool SelectExit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ConfirmExit(Owner));
        return true;
      }
      return false;
    }
  }
}
