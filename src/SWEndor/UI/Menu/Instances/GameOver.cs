using MTV3D65;
using SWEndor.ActorTypes;
using SWEndor.Scenarios;

namespace SWEndor.UI.Menu.Pages
{
  public class GameOver : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();
    SelectionElement ButtonRestart = new SelectionElement();

    public GameOver(Screen2D owner) : base(owner)
    {
      MainText.Text = "GAME OVER";
      MainText.TextFont = FontFactory.Get(Font.T36).ID;
      MainText.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(-150, -80);
      MainText.HighlightBoxPosition = MainText.TextPosition - new TV_2DVECTOR(5, 5);
      MainText.HighlightBoxWidth = 330;
      MainText.HighlightBoxHeight = 50;

      ButtonRestart.Text = "Restart";
      ButtonRestart.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, 20);
      ButtonRestart.HighlightBoxPosition = ButtonRestart.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonRestart.HighlightBoxWidth = 110;
      ButtonRestart.HighlightBoxHeight = 30;
      ButtonRestart.Selectable = true;
      ButtonRestart.OnKeyPress += SelectRestart;

      ButtonReturn.Text = "Abort";
      ButtonReturn.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, 60);
      ButtonReturn.HighlightBoxPosition = ButtonReturn.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonReturn.HighlightBoxWidth = 110;
      ButtonReturn.HighlightBoxHeight = 30;
      ButtonReturn.Selectable = true;
      ButtonReturn.OnKeyPress += SelectReturn;

      Elements.Add(MainText);
      Elements.Add(ButtonRestart);
      Elements.Add(ButtonReturn);
      SelectedElementID = Elements.IndexOf(ButtonReturn);
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

    private bool SelectReturn(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        GameScenarioManager.Reset();
        GameScenarioManager.LoadMainMenu();
        return true;
      }
      return false;
    }
  }
}
