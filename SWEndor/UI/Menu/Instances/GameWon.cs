using MTV3D65;
using SWEndor.Player;
using SWEndor.Scenarios;
using System.Collections.Generic;

namespace SWEndor.UI.Menu.Pages
{
  public class GameWon : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement Score = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();

    public GameWon()
    {
      MainText.Text = "MISSION ACCOMPLISHED!";
      MainText.TextFont = Font.Factory.Get("Title_36").ID;
      MainText.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 - 242, 60);

      Score.Text += string.Format("{0,42} {1,8:0}", "Score".PadRight(42), Globals.Engine.PlayerInfo.Score.Score);
      Score.Text += string.Format("\n\n{0,42} {1,8:0}", "Total Hits".PadRight(42), Globals.Engine.PlayerInfo.Score.Hits);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Total Hits on Fighters".PadRight(42), Globals.Engine.PlayerInfo.Score.HitsOnFighters);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Damage Taken".PadRight(42), Globals.Engine.PlayerInfo.Score.DamageTaken);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Deaths".PadRight(42), Globals.Engine.PlayerInfo.Score.Deaths);
      Score.Text += string.Format("\n\n{0,42} {1,8:0}", "", "Kills");

      foreach (KeyValuePair<string, int> kvp in Globals.Engine.PlayerInfo.Score.KillsByType)
        Score.Text += string.Format("\n{0,42} {1,8:0}", (kvp.Key.Length > 42) ? kvp.Key.Remove(42) : kvp.Key.PadRight(42), kvp.Value);

      Score.TextFont = Font.Factory.Get("Text_12").ID;
      Score.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 - 242, 120);
      Score.HighlightBoxPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 - 255, 55);
      Score.HighlightBoxWidth = 255 * 2;
      Score.HighlightBoxHeight = Globals.Engine.ScreenHeight / 2 + 242 - 55;

      ButtonReturn.Text = "Return to Menu";
      ButtonReturn.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 + 60, Globals.Engine.ScreenHeight / 2 + 260);
      ButtonReturn.HighlightBoxPosition = ButtonReturn.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonReturn.HighlightBoxWidth = 200;
      ButtonReturn.HighlightBoxHeight = 30;
      ButtonReturn.Selectable = true;
      ButtonReturn.OnKeyPress += SelectReturn;

      Elements.Add(Score);
      Elements.Add(MainText);
      Elements.Add(ButtonReturn);
      SelectedElementID = Elements.IndexOf(ButtonReturn);
    }

    private bool SelectReturn(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Globals.Engine.GameScenarioManager.Reset();
        Globals.Engine.GameScenarioManager.LoadMainMenu();
        return true;
      }
      return false;
    }
  }
}
