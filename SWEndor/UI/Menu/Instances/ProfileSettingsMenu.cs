﻿using MTV3D65;
using SWEndor.UI.Forms;
using System;

namespace SWEndor.UI.Menu.Pages
{
  public class ProfileSettingsMenu : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement ButtonObjCounter = new SelectionElement();
    SelectionElement ButtonShowPerformanceStat = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();

    StatForm statform;

    public ProfileSettingsMenu(Screen2D owner) : base(owner)
    {
      float height_gap = 40;
      float x = 75;
      float y = 120;


      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Globals.Engine.ScreenWidth;
      Cover.HighlightBoxHeight = Globals.Engine.ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      MainText.Text = "Configuration Menu";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      ButtonObjCounter.Text = "Show Object Counters";
      ButtonObjCounter.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonObjCounter.HighlightBoxPosition = ButtonObjCounter.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonObjCounter.HighlightBoxWidth = 600;
      ButtonObjCounter.HighlightBoxHeight = 30;
      ButtonObjCounter.Selectable = true;
      ButtonObjCounter.OnKeyPress += ShowObjCounter;

      ButtonShowPerformanceStat.Text = "Show/Hide Performance Metrics";
      ButtonShowPerformanceStat.TextPosition = new TV_2DVECTOR(x, y);
      y += height_gap;
      ButtonShowPerformanceStat.HighlightBoxPosition = ButtonShowPerformanceStat.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonShowPerformanceStat.HighlightBoxWidth = 600;
      ButtonShowPerformanceStat.HighlightBoxHeight = 30;
      ButtonShowPerformanceStat.SecondaryText = Engine.PerfManager.Enabled.ToString();
      ButtonShowPerformanceStat.SecondaryTextPosition = ButtonShowPerformanceStat.TextPosition + new TV_2DVECTOR(400, 0);
      ButtonShowPerformanceStat.Selectable = true;
      ButtonShowPerformanceStat.OnKeyPress += SelectPerformanceToggle;
      
      ButtonExit.Text = "Back";
      ButtonExit.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth - 200, Globals.Engine.ScreenHeight - 80);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(Cover);
      Elements.Add(MainText);

      Elements.Add(ButtonObjCounter);
      Elements.Add(ButtonShowPerformanceStat);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonObjCounter);
    }


    private bool ShowObjCounter(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Globals.Engine.Form.Invoke(
          new Action(() =>
          {
            if (statform == null || statform.IsDisposed)
              statform = new StatForm(Engine);
            statform.Show();
            }));
        return true;
      }
      return false;
    }

    private bool SelectPerformanceToggle(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_LEFT || key == CONST_TV_KEY.TV_KEY_RIGHT)
      {
        Engine.PerfManager.Enabled = !Engine.PerfManager.Enabled;
        ButtonShowPerformanceStat.SecondaryText = Engine.PerfManager.Enabled.ToString();
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