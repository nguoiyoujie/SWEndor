using MTV3D65;
using System;

namespace SWEndor.UI
{
  public class UIPage_ConfigSettingsMenu : UIPage
  {
    UISelectionElement Cover = new UISelectionElement();
    UISelectionElement MainText = new UISelectionElement();
    UISelectionElement ButtonScreenResolution = new UISelectionElement();
    UISelectionElement ButtonFullScreen = new UISelectionElement();
    UISelectionElement ButtonShowPerformanceStat = new UISelectionElement();
    UISelectionElement ButtonChangesComment = new UISelectionElement();
    UISelectionElement ButtonSaveAndExit = new UISelectionElement();
    UISelectionElement ButtonExit = new UISelectionElement();


    public UIPage_ConfigSettingsMenu()
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;


      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Engine.Instance().ScreenWidth;
      Cover.HighlightBoxHeight = Engine.Instance().ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      MainText.Text = "Configuration Menu";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      ButtonScreenResolution.Text = "Screen Resolution";
      ButtonScreenResolution.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonScreenResolution.HighlightBoxPosition = ButtonScreenResolution.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonScreenResolution.HighlightBoxWidth = 600;
      ButtonScreenResolution.HighlightBoxHeight = 30;
      ButtonScreenResolution.SecondaryText = Settings.ResolutionMode.ToString().TrimStart('R', '_');
      ButtonScreenResolution.SecondaryTextPosition = ButtonScreenResolution.TextPosition + new TV_2DVECTOR(400, 0);
      ButtonScreenResolution.Selectable = true;
      ButtonScreenResolution.OnKeyPress += SelectScreenResolution;

      ButtonFullScreen.Text = "Use Full Screen";
      ButtonFullScreen.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonFullScreen.HighlightBoxPosition = ButtonFullScreen.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonFullScreen.HighlightBoxWidth = 600;
      ButtonFullScreen.HighlightBoxHeight = 30;
      ButtonFullScreen.SecondaryText = Settings.FullScreenMode.ToString();
      ButtonFullScreen.SecondaryTextPosition = ButtonFullScreen.TextPosition + new TV_2DVECTOR(400, 0);
      ButtonFullScreen.Selectable = true;
      ButtonFullScreen.OnKeyPress += SelectFullScreen;

      ButtonShowPerformanceStat.Text = "Show/Hide Performance Metrics";
      ButtonShowPerformanceStat.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonShowPerformanceStat.HighlightBoxPosition = ButtonShowPerformanceStat.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonShowPerformanceStat.HighlightBoxWidth = 600;
      ButtonShowPerformanceStat.HighlightBoxHeight = 30;
      ButtonShowPerformanceStat.SecondaryText = Settings.ShowPerformance.ToString();
      ButtonShowPerformanceStat.SecondaryTextPosition = ButtonShowPerformanceStat.TextPosition + new TV_2DVECTOR(400, 0);
      ButtonShowPerformanceStat.Selectable = true;
      ButtonShowPerformanceStat.OnKeyPress += SelectPerformanceToggle;

      ButtonChangesComment.Text = "";
      ButtonChangesComment.TextColor = new TV_COLOR(1, 0.5f, 0.2f, 1);
      ButtonChangesComment.TextFont = Font.GetFont("Text_12").ID;
      ButtonChangesComment.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      //ButtonChangesComment.HighlightBoxPosition = ButtonChangesComment.TextPosition - new TV_2DVECTOR(5, 5);
      //ButtonChangesComment.HighlightBoxWidth = 500;
      //ButtonChangesComment.HighlightBoxHeight = 30;
      ButtonChangesComment.Selectable = false;

      ButtonSaveAndExit.Text = "Save";
      ButtonSaveAndExit.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 120);
      ButtonSaveAndExit.HighlightBoxPosition = ButtonSaveAndExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSaveAndExit.HighlightBoxWidth = 200;
      ButtonSaveAndExit.HighlightBoxHeight = 30;
      ButtonSaveAndExit.Selectable = true;
      ButtonSaveAndExit.OnKeyPress += SelectSave;

      ButtonExit.Text = "Cancel";
      ButtonExit.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 80);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(Cover);
      Elements.Add(MainText);

      Elements.Add(ButtonScreenResolution);
      Elements.Add(ButtonFullScreen);
      Elements.Add(ButtonShowPerformanceStat);
      Elements.Add(ButtonChangesComment);
      Elements.Add(ButtonSaveAndExit);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonScreenResolution);
    }


    private bool SelectScreenResolution(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        int max = Enum.GetValues(typeof(ResolutionSettings)).Length;
        int mode = (int)Settings.ResolutionMode;
        if (key == CONST_TV_KEY.TV_KEY_LEFT && mode > 0)
        {
          mode--;
        }
        else if (key == CONST_TV_KEY.TV_KEY_RIGHT && mode < max - 1)
        {
          mode++;
        }
        Settings.ResolutionMode = (ResolutionSettings)mode;
        ButtonScreenResolution.SecondaryText = Settings.ResolutionMode.ToString().TrimStart('R', '_');
        ButtonChangesComment.Text = "Changes to screen mode will be applied after restarting the application";
        return true;
      }
      return false;
    }

    private bool SelectFullScreen(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Settings.FullScreenMode = !Settings.FullScreenMode;
        ButtonFullScreen.SecondaryText = Settings.FullScreenMode.ToString();
        ButtonChangesComment.Text = "Changes to screen mode will be applied after restarting the application";
        return true;
      }
      return false;
    }

    private bool SelectPerformanceToggle(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Settings.ShowPerformance = !Settings.ShowPerformance;
        ButtonShowPerformanceStat.SecondaryText = Settings.ShowPerformance.ToString();
        return true;
      }
      return false;
    }

    private bool SelectSave(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Settings.SaveSettings();
        Back();
        return true;
      }
      return false;
    }

    private bool SelectExit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Back();
        return true;
      }
      return false;
    }
  }
}
