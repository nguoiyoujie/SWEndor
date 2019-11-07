using MTV3D65;
using Primrose.Primitives;
using System;

namespace SWEndor.UI.Menu.Pages
{
  public class ConfigSettingsMenu : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonScreenResolution = new SelectionElement();
    SelectionElement ButtonFullScreen = new SelectionElement();
    SelectionElement ButtonGoToProfilerMenu = new SelectionElement();
    SelectionElement ButtonChangesComment = new SelectionElement();
    SelectionElement ButtonSaveAndExit = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();

    char[] ResDelimiter = new char[] { 'R', '_' };

    public ConfigSettingsMenu(Screen2D owner) : base(owner)
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;


      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND);

      MainText.Text = "Configuration Menu";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      ButtonScreenResolution.Text = "Screen Resolution";
      ButtonScreenResolution.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonScreenResolution.HighlightBoxPosition = ButtonScreenResolution.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonScreenResolution.HighlightBoxWidth = 600;
      ButtonScreenResolution.HighlightBoxHeight = 30;
      ButtonScreenResolution.SecondaryText = Settings.ResolutionMode.GetEnumName().TrimStart(ResDelimiter);
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

      ButtonGoToProfilerMenu.Text = "Go To Profilers";
      ButtonGoToProfilerMenu.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonGoToProfilerMenu.HighlightBoxPosition = ButtonGoToProfilerMenu.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonGoToProfilerMenu.HighlightBoxWidth = 600;
      ButtonGoToProfilerMenu.HighlightBoxHeight = 30;
      ButtonGoToProfilerMenu.Selectable = true;
      ButtonGoToProfilerMenu.OnKeyPress += GoToProfilers;

      ButtonChangesComment.Text = "";
      ButtonChangesComment.TextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT_ORANGE);
      ButtonChangesComment.TextFont = FontFactory.Get(Font.T12).ID;
      ButtonChangesComment.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      //ButtonChangesComment.HighlightBoxPosition = ButtonChangesComment.TextPosition - new TV_2DVECTOR(5, 5);
      //ButtonChangesComment.HighlightBoxWidth = 500;
      //ButtonChangesComment.HighlightBoxHeight = 30;
      ButtonChangesComment.Selectable = false;

      ButtonSaveAndExit.Text = "Save";
      ButtonSaveAndExit.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -120);
      ButtonSaveAndExit.HighlightBoxPosition = ButtonSaveAndExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonSaveAndExit.HighlightBoxWidth = 200;
      ButtonSaveAndExit.HighlightBoxHeight = 30;
      ButtonSaveAndExit.Selectable = true;
      ButtonSaveAndExit.OnKeyPress += SelectSave;

      ButtonExit.Text = "Cancel";
      ButtonExit.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -80);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(Cover);
      Elements.Add(MainText);

      Elements.Add(ButtonScreenResolution);
      Elements.Add(ButtonFullScreen);
      Elements.Add(ButtonGoToProfilerMenu);
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
        ButtonScreenResolution.SecondaryText = Settings.ResolutionMode.GetEnumName().TrimStart(ResDelimiter);
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

    private bool GoToProfilers(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        EnterPage(new ProfileSettingsMenu(Owner));
        return true;
      }
      return false;
    }

    private bool SelectSave(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Settings.SaveSettings(Engine);
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
