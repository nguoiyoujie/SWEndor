﻿using MTV3D65;

namespace SWEndor.UI.Menu.Pages
{
  public class GameOver : Page
  {
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();

    public GameOver(Screen2D owner) : base(owner)
    {
      MainText.Text = "GAME OVER";
      MainText.TextFont = FontFactory.Get(Font.T36).ID;
      MainText.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(-150, -80);

      ButtonReturn.Text = "Return to Menu";
      ButtonReturn.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, 60);
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
        GameScenarioManager.Reset();
        GameScenarioManager.LoadMainMenu();
        return true;
      }
      return false;
    }
  }
}