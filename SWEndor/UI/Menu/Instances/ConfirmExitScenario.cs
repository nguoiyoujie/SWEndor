using MTV3D65;

namespace SWEndor.UI.Menu.Pages
{
  public class ConfirmExitScenario : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement ConfirmText = new SelectionElement();
    SelectionElement ConfirmNo = new SelectionElement();
    SelectionElement ConfirmYes = new SelectionElement();

    public ConfirmExitScenario(Screen2D owner) : base(owner)
    {
      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Globals.Engine.ScreenWidth;
      Cover.HighlightBoxHeight = Globals.Engine.ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      ConfirmText.Text = "Confirm Quit Scenario?";
      ConfirmText.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 - 150, Globals.Engine.ScreenHeight / 2 - 80);
      ConfirmText.HighlightBoxPosition = ConfirmText.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmText.HighlightBoxWidth = 320;
      ConfirmText.HighlightBoxHeight = 30;

      ConfirmNo.Text = "NO";
      ConfirmNo.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 + 60, Globals.Engine.ScreenHeight / 2 + 20);
      ConfirmNo.HighlightBoxPosition = ConfirmNo.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmNo.HighlightBoxWidth = 60;
      ConfirmNo.HighlightBoxHeight = 30;
      ConfirmNo.Selectable = true;
      ConfirmNo.OnKeyPress += SelectNo;

      ConfirmYes.Text = "YES";
      ConfirmYes.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 + 60, Globals.Engine.ScreenHeight / 2 + 60);
      ConfirmYes.HighlightBoxPosition = ConfirmYes.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmYes.HighlightBoxWidth = 60;
      ConfirmYes.HighlightBoxHeight = 30;
      ConfirmYes.Selectable = true;
      ConfirmYes.OnKeyPress += SelectYes;

      Elements.Add(Cover);
      Elements.Add(ConfirmText);
      Elements.Add(ConfirmNo);
      Elements.Add(ConfirmYes);
      SelectedElementID = Elements.IndexOf(ConfirmNo);
    }

    private bool SelectNo(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Back();
        return true;
      }
      return false;
    }

    private bool SelectYes(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        this.GetEngine().GameScenarioManager.Reset();
        this.GetEngine().GameScenarioManager.LoadMainMenu();
        this.GetEngine().SoundManager.SetMusicResume();

        return true;
      }
      return false;
    }
  }
}
