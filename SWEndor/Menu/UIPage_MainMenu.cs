using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class UIPage_MainMenu : UIPage
  {
    UISelectionElement MainMenuText = new UISelectionElement();
    UISelectionElement ButtonPlay = new UISelectionElement();
    UISelectionElement ButtonLoad = new UISelectionElement();
    UISelectionElement ButtonOptions = new UISelectionElement();
    UISelectionElement ButtonCredits = new UISelectionElement();
    UISelectionElement ButtonExit = new UISelectionElement();


    public UIPage_MainMenu()
    {
      MainMenuText.Text = "Battle of Endor";
      MainMenuText.TextPosition = new TV_2DVECTOR(75, 60);
      MainMenuText.TextFont = Screen2D.Instance().FontID48;

      ButtonPlay.Text = "Play Scenario";
      ButtonPlay.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 300);
      ButtonPlay.HighlightBoxPosition = ButtonPlay.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonPlay.HighlightBoxWidth = 200;
      ButtonPlay.HighlightBoxHeight = 30;
      ButtonPlay.Selectable = true;
      ButtonPlay.OnKeyPress += SelectPlay;

      ButtonLoad.Text = "Load Scenario";
      ButtonLoad.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 260);
      ButtonLoad.HighlightBoxPosition = ButtonLoad.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonLoad.HighlightBoxWidth = 200;
      ButtonLoad.HighlightBoxHeight = 30;
      ButtonLoad.Selectable = true;
      ButtonLoad.OnKeyPress += SelectLoad;

      ButtonOptions.Text = "Options";
      ButtonOptions.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 220);
      ButtonOptions.HighlightBoxPosition = ButtonOptions.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonOptions.HighlightBoxWidth = 200;
      ButtonOptions.HighlightBoxHeight = 30;
      ButtonOptions.Selectable = true;
      ButtonOptions.OnKeyPress += SelectOptions;

      ButtonCredits.Text = "Credits";
      ButtonCredits.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 180);
      ButtonCredits.HighlightBoxPosition = ButtonCredits.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonCredits.HighlightBoxWidth = 200;
      ButtonCredits.HighlightBoxHeight = 30;
      ButtonCredits.Selectable = true;
      ButtonCredits.OnKeyPress += SelectCredits;

      ButtonExit.Text = "Exit";
      ButtonExit.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 140);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(MainMenuText);
      Elements.Add(ButtonPlay);
      Elements.Add(ButtonLoad);
      Elements.Add(ButtonOptions);
      Elements.Add(ButtonCredits);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonPlay);
    }

    private bool SelectPlay(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new UIPage_PlayScenario());
        return true;
      }
      return false;
    }

    private bool SelectLoad(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        //EnterPage(null);
        return true;
      }
      return false;
    }

    private bool SelectOptions(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new UIPage_OptionsMenu());
        return true;
      }
      return false;
    }

    private bool SelectCredits(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new UIPage_Credits());
        return true;
      }
      return false;
    }

    private bool SelectExit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new UIPage_ConfirmExit());
        return true;
      }
      return false;
    }
  }
}
