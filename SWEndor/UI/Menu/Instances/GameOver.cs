﻿using MTV3D65;
using SWEndor.Scenarios;

namespace SWEndor.UI.Menu.Pages
{
  public class GameOver : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();

    public GameOver()
    {
      MainText.Text = "GAME OVER";
      MainText.TextFont = Font.Factory.Get("Title_36").ID;
      MainText.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 - 150, Engine.Instance().ScreenHeight / 2 - 80);

      ButtonReturn.Text = "Return to Menu";
      ButtonReturn.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 + 60, Engine.Instance().ScreenHeight / 2 + 60);
      ButtonReturn.HighlightBoxPosition = ButtonReturn.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonReturn.HighlightBoxWidth = 200;
      ButtonReturn.HighlightBoxHeight = 30;
      ButtonReturn.Selectable = true;
      ButtonReturn.OnKeyPress += SelectReturn;

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