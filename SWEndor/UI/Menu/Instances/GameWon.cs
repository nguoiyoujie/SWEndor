using MTV3D65;
using System.Collections.Generic;
using System.Text;

namespace SWEndor.UI.Menu.Pages
{
  public class GameWon : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement Score = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();
    StringBuilder sb = new StringBuilder();

    public GameWon(Screen2D owner) : base(owner)
    {
      MainText.Text = "MISSION ACCOMPLISHED!";
      MainText.TextFont = FontFactory.Get(Font.T36).ID;
      MainText.TextPosition = new TV_2DVECTOR(Engine.ScreenWidth / 2 - 242, 60);

      sb.Clear();
      sb.Append("Score                                     ");
      sb.Append(Engine.PlayerInfo.Score.Score.ToString(" 00000000"));
      sb.AppendLine();
      sb.AppendLine();
      sb.Append("Total Hits                                ");
      sb.Append(Engine.PlayerInfo.Score.Hits.ToString(" #######0"));
      sb.AppendLine();
      //sb.Append("Total Hits on Fighters                     ");
      //sb.Append(PlayerInfo.Score.HitsOnFighters.ToString("00000000"));
      //sb.AppendLine();
      sb.Append("Damage Taken                              ");
      sb.Append(Engine.PlayerInfo.Score.DamageTaken.ToString(" #######0"));
      sb.AppendLine();
      sb.Append("Deaths                                    ");
      sb.Append(Engine.PlayerInfo.Score.Deaths.ToString(" #######0"));
      sb.AppendLine();
      sb.AppendLine();
      sb.Append("Kills                                     ");
      sb.Append(Engine.PlayerInfo.Score.Kills.ToString(" #######0"));
      sb.AppendLine();

      foreach (KeyValuePair<string, int> kvp in Engine.PlayerInfo.Score.KillsByName.GetList())
      {
        sb.Append((kvp.Key.Length > 42) ? kvp.Key.Remove(42) : kvp.Key.PadRight(42));
        sb.Append(kvp.Value.ToString(" #######0"));
        sb.AppendLine();
      }
      /*
      Score.Text += string.Format("{0,42} {1,8:0}", "Score".PadRight(42), PlayerInfo.Score.Score);
      Score.Text += string.Format("\n\n{0,42} {1,8:0}", "Total Hits".PadRight(42), PlayerInfo.Score.Hits);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Damage Taken".PadRight(42), PlayerInfo.Score.DamageTaken);
      Score.Text += string.Format("\n{0,42} {1,8:0}", "Deaths".PadRight(42), PlayerInfo.Score.Deaths);
      Score.Text += string.Format("\n\n{0,42} {1,8:0}", "", "Kills");
      foreach (KeyValuePair<string, int> kvp in PlayerInfo.Score.KillsByName.GetList())
        Score.Text += string.Format("\n{0,42} {1,8:0}", (kvp.Key.Length > 42) ? kvp.Key.Remove(42) : kvp.Key.PadRight(42), kvp.Value);
      */
      Score.Text = sb.ToString();

      Score.TextFont = FontFactory.Get(Font.T12).ID;
      Score.TextPosition = new TV_2DVECTOR(Engine.ScreenWidth / 2 - 242, 120);
      Score.HighlightBoxPosition = new TV_2DVECTOR(Engine.ScreenWidth / 2 - 255, 55);
      Score.HighlightBoxWidth = 255 * 2;
      Score.HighlightBoxHeight = Engine.ScreenHeight / 2 + 242 - 55;

      ButtonReturn.Text = "Return to Menu";
      ButtonReturn.TextPosition = new TV_2DVECTOR(Engine.ScreenWidth / 2 + 60, Engine.ScreenHeight / 2 + 260);
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
        GameScenarioManager.Reset();
        GameScenarioManager.LoadMainMenu();
        return true;
      }
      return false;
    }
  }
}
