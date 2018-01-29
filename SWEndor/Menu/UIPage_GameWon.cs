﻿using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SWEndor
{
  public class UIPage_GameWon : UIPage
  {
    UISelectionElement MainText = new UISelectionElement();
    UISelectionElement ButtonReturn = new UISelectionElement();

    public UIPage_GameWon()
    {
      MainText.Text = "MISSION ACCOMPLISHED!";
      MainText.TextFont = Screen2D.Instance().FontID36;
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
