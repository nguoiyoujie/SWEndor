using MTV3D65;
using SWEndor.Primitives;
using System.Collections.Generic;

namespace SWEndor.UI.Menu.Pages
{
  public class GameWon : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement Score = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();

    public GameWon(Screen2D owner) : base(owner)
    {
      MainText.Text = "MISSION ACCOMPLISHED!";
      MainText.TextFont = FontFactory.Get(Font.T36).ID;
      MainText.TextPosition = new TV_2DVECTOR(Engine.ScreenWidth / 2 - 242, 60);

      Score.Text += "{0,42} {1,8:0}".F("Score".PadRight(42), PlayerInfo.Score.Score);
      Score.Text += "\n\n{0,42} {1,8:0}".F("Total Hits".PadRight(42), PlayerInfo.Score.Hits);
      Score.Text += "\n{0,42} {1,8:0}".F("Damage Taken".PadRight(42), PlayerInfo.Score.DamageTaken);
      Score.Text += "\n{0,42} {1,8:0}".F("Deaths".PadRight(42), PlayerInfo.Score.Deaths);
      Score.Text += "\n\n{0,42} {1,8:0}".F("", "Kills");

      foreach (string key in PlayerInfo.Score.KillsByName.Keys)
        Score.Text += "\n{0,42} {1,8:0}".F((key.Length > 42) ? key.Remove(42) : key.PadRight(42), PlayerInfo.Score.KillsByName[key]);

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
