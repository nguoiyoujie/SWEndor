using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class UIPage_ConfirmExitScenario : UIPage
  {
    UISelectionElement Cover = new UISelectionElement();
    UISelectionElement ConfirmText = new UISelectionElement();
    UISelectionElement ConfirmNo = new UISelectionElement();
    UISelectionElement ConfirmYes = new UISelectionElement();

    public UIPage_ConfirmExitScenario()
    {
      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Engine.Instance().ScreenWidth;
      Cover.HighlightBoxHeight = Engine.Instance().ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      ConfirmText.Text = "Confirm Quit Scenario?";
      ConfirmText.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 - 150, Engine.Instance().ScreenHeight / 2 - 80);

      ConfirmNo.Text = "NO";
      ConfirmNo.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 + 60, Engine.Instance().ScreenHeight / 2 + 20);
      ConfirmNo.HighlightBoxPosition = ConfirmNo.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmNo.HighlightBoxWidth = 60;
      ConfirmNo.HighlightBoxHeight = 30;
      ConfirmNo.Selectable = true;
      ConfirmNo.OnKeyPress += SelectNo;

      ConfirmYes.Text = "YES";
      ConfirmYes.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth / 2 + 60, Engine.Instance().ScreenHeight / 2 + 60);
      ConfirmYes.HighlightBoxPosition = ConfirmYes.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmYes.HighlightBoxWidth = 60;
      ConfirmYes.HighlightBoxHeight = 30;
      ConfirmYes.Selectable = true;
      ConfirmYes.OnKeyPress += SelectYes;

      Elements.Add(Cover);
      Elements.Add(ConfirmText);
      Elements.Add(ConfirmNo);
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
        GameScenarioManager.Instance().Reset();
        GameScenarioManager.Instance().LoadMainMenu();
        return true;
      }
      return false;
    }
  }
}
