using MTV3D65;
using SWEndor.Game.Core;
using SWEndor.Game.Scenarios;
using System;
using System.Collections.Generic;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class ScenarioMessageLog : Page
  {
    readonly ScenarioBase SelectedScenario;
    readonly SelectionElement Cover = new SelectionElement();
    readonly List<SelectionElement> LogText = new List<SelectionElement>();
    readonly SelectionElement BackText = new SelectionElement();
    int log = 0;

    public ScenarioMessageLog(Screen2D owner, ScenarioBase selectedScenario) : base(owner)
    {
      SelectedScenario = selectedScenario;

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.GAME_MAP_BACKGROUND);

      float y = 50;
      while (y < owner.ScreenSize.y - 70)
      {
        SelectionElement line = new SelectionElement();
        LogText.Add(line);
        line.Text = "";
        line.TextPosition = new TV_2DVECTOR(20, y);
        line.TextFont = FontFactory.Get(Font.T12).ID;
        line.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);
        line.HighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);
        line.SecondaryText = "";
        line.SecondaryTextFont = FontFactory.Get(Font.T12).ID;
        line.SecondaryTextPosition = new TV_2DVECTOR(120, y);
        line.Selectable = false;
        y += 30;
      }
      log = (SelectedScenario.State.MessageLogs.Count - LogText.Count).Max(0);

      BackText.Text = "Press Directional arrows to scroll. \nBACKSPACE to reset position \nPress ESC to return to menu.";
      BackText.TextFont = FontFactory.Get(Font.T12).ID;
      BackText.TextPosition = new TV_2DVECTOR(20, Engine.ScreenHeight - 60);
      BackText.Selectable = true;
      BackText.OnKeyPress += SelectExit;
      BackText.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);
      BackText.HighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.TRANSPARENT);

      Elements.Add(Cover);
      Elements.AddRange(LogText);
      Elements.Add(BackText);
      SelectedElementID = Elements.IndexOf(BackText);
    }

    private bool SelectExit(CONST_TV_KEY key)
    {
      switch (key)
      {
        case CONST_TV_KEY.TV_KEY_ESCAPE:
          if (!Back())
          {
            Engine.Screen2D.CurrentPage = null;
            Engine.Screen2D.ShowPage = false;
          }
          return true;

        case CONST_TV_KEY.TV_KEY_UP:
          log = (log + 1).Min(SelectedScenario.State.MessageLogs.Count - 1); ;
          return false;

        case CONST_TV_KEY.TV_KEY_DOWN:
          log = (log - 1).Max(0);
          return false;

        case CONST_TV_KEY.TV_KEY_BACKSPACE:
          log = (SelectedScenario.State.MessageLogs.Count - LogText.Count).Max(0);
          return false;
      }

      return false;
    }

    public override void RenderTick()
    {
      base.RenderTick();
      for (int i = 0; i < LogText.Count; i++)
      {
        List<MessageLog> loglines = SelectedScenario.State.MessageLogs;
        if (i + log < loglines.Count)
        {
          LogText[i].Text = LookUpString.GetTimeDisplay(loglines[i + log].Time);
          LogText[i].SecondaryText = loglines[i + log].Text;
          LogText[i].TextColor = loglines[i + log].Color;
          LogText[i].SecondaryTextColor = loglines[i + log].Color;
        }
        else
        {
          LogText[i].Text = "";
          LogText[i].SecondaryText = "";
        }
      }
    }
  }
}
