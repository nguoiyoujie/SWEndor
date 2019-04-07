using MTV3D65;
using SWEndor.Player;
using SWEndor.Scenarios;
using System.Collections.Generic;

namespace SWEndor.UI
{
  public class UIPage_GameWon : UIPage
  {
    UISelectionElement MainText = new UISelectionElement();
    UISelectionElement Score = new UISelectionElement();
    UISelectionElement ButtonReturn = new UISelectionElement();

    public UIPage_GameWon()
    {
      MainText.Text = "MISSION ACCOMPLISHED!";
      MainText.TextFont = Font.Factory.Get("Title_36").ID;
      MainText.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 - 242, 60);

      Score.Text += string.Format("{0,42} {1,8:0}", "Score".PadRight(42), PlayerInfo.Instance().Score.Score);
      Score.Text += string.Format("\n\n{0,42} {1,8:0}", "Total Hits".PadRight(42), PlayerInfo.Instance().Score.Hits);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Total Hits on Fighters".PadRight(42), PlayerInfo.Instance().Score.HitsOnFighters);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Damage Taken".PadRight(42), PlayerInfo.Instance().Score.DamageTaken);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Deaths".PadRight(42), PlayerInfo.Instance().Score.Deaths);
      Score.Text += string.Format("\n\n{0,42} {1,8:0}", "", "Kills");

      foreach (KeyValuePair<string, int> kvp in PlayerInfo.Instance().Score.KillsByType)
        Score.Text += string.Format("\n{0,42} {1,8:0}", (kvp.Key.Length > 42) ? kvp.Key.Remove(42) : kvp.Key.PadRight(42), kvp.Value);

      Score.TextFont = Font.Factory.Get("Text_12").ID;
      Score.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 - 242, 120);
      Score.HighlightBoxPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 - 255, 55);
      Score.HighlightBoxWidth = 255 * 2;
      Score.HighlightBoxHeight = Engine.Instance().ScreenHeight / 2 + 242 - 55;

      ButtonReturn.Text = "Return to Menu";
      ButtonReturn.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 + 60, Engine.Instance().ScreenHeight / 2 + 260);
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
        GameScenarioManager.Instance().Reset();
        GameScenarioManager.Instance().LoadMainMenu();
        return true;
      }
      return false;
    }
  }
}
