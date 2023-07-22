using MTV3D65;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.Scenarios;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class ConfirmExitScenario : Page
  {
    readonly SelectionElement Cover = new SelectionElement();
    readonly SelectionElement ConfirmText = new SelectionElement();
    readonly SelectionElement ConfirmNo = new SelectionElement();
    readonly SelectionElement ConfirmYes = new SelectionElement();
    readonly SelectionElement ConfirmRestart = new SelectionElement();

    public ConfirmExitScenario(Screen2D owner) : base(owner)
    {
      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND);

      ConfirmText.Text = "Confirm Quit Scenario?";
      ConfirmText.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(-150, -80);
      ConfirmText.HighlightBoxPosition = ConfirmText.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmText.HighlightBoxWidth = 320;
      ConfirmText.HighlightBoxHeight = 30;

      ConfirmNo.Text = "Cancel";
      ConfirmNo.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, -20);
      ConfirmNo.HighlightBoxPosition = ConfirmNo.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmNo.HighlightBoxWidth = 110;
      ConfirmNo.HighlightBoxHeight = 30;
      ConfirmNo.Selectable = true;
      ConfirmNo.OnKeyPress += SelectNo;

      ConfirmRestart.Text = "Restart";
      ConfirmRestart.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, 20);
      ConfirmRestart.HighlightBoxPosition = ConfirmRestart.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmRestart.HighlightBoxWidth = 110;
      ConfirmRestart.HighlightBoxHeight = 30;
      ConfirmRestart.Selectable = true;
      ConfirmRestart.OnKeyPress += SelectRestart;

      ConfirmYes.Text = "Abort";
      ConfirmYes.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, 60);
      ConfirmYes.HighlightBoxPosition = ConfirmYes.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmYes.HighlightBoxWidth = 110;
      ConfirmYes.HighlightBoxHeight = 30;
      ConfirmYes.Selectable = true;
      ConfirmYes.OnKeyPress += SelectYes;

      Elements.Add(Cover);
      Elements.Add(ConfirmText);
      Elements.Add(ConfirmNo);
      Elements.Add(ConfirmRestart);
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
        GameScenarioManager.Reset();
        GameScenarioManager.LoadMainMenu();
        Engine.SoundManager.ResumeMusic();
        return true;
      }
      return false;
    }

    private bool SelectRestart(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        ScenarioBase b = GameScenarioManager.Scenario;
        ActorTypeInfo a = Engine.PlayerInfo.ActorType;
        string d = GameScenarioManager.Scenario.State.Difficulty;
        GameScenarioManager.Reset();
        EnterPage(new LoadingScenario(Owner, b, a, d));
        return true;
      }
      return false;
    }
  }
}
