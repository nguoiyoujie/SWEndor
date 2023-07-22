using MTV3D65;
using SWEndor.Game.Core;
using System.Collections.Generic;

namespace SWEndor.Game.UI
{
  public delegate bool KeyEvent(CONST_TV_KEY key);

  public class SelectionElement
  {
    public bool Selectable;

    public string Text = "";
    public TV_2DVECTOR TextPosition;
    public COLOR TextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT);
    public int TextFont = -1;

    public string SecondaryText = "";
    public TV_2DVECTOR SecondaryTextPosition;
    public COLOR SecondaryTextColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT);
    public int SecondaryTextFont = -1;

    public TV_2DVECTOR HighlightBoxPosition;
    public float HighlightBoxWidth = 0;
    public float HighlightBoxHeight = 0;
    public COLOR UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND);
    public COLOR HighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_HIGHLIGHT_BACKGROUND);

    public int ToggleButtonsNumber = 0;
    public int ToggleButtonsCurrentNumber = 0;
    public float ToggleButtonsWidth = 10;
    public float ToggleButtonsHeight = 20;
    public float ToggleButtonsGapWidth = 5;
    public TV_2DVECTOR ToggleButtonsPosition;
    public COLOR ToggleButtonsColor = ColorLocalization.Get(ColorLocalKeys.UI_TEXT);
    public List<string> ToggleButtonsValues = new List<string>();

    public KeyEvent OnKeyPress;
    public KeyEvent AfterKeyPress;

    public virtual void Show(Engine engine, bool ishighlighted)
    {
      int font = (TextFont > -1) ? TextFont : engine.FontFactory.Get(Font.T16).ID;

      engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
      engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(HighlightBoxPosition.x
                                       , HighlightBoxPosition.y
                                       , HighlightBoxPosition.x + HighlightBoxWidth
                                       , HighlightBoxPosition.y + HighlightBoxHeight
                                       , ((ishighlighted) ? HighlightBoxColor : UnHighlightBoxColor).Value);

      int n = 0;
      //bool lit = true;
      float x = ToggleButtonsPosition.x;
      float y = ToggleButtonsPosition.y;
      while (n < ToggleButtonsNumber - 1)
      {
        if (n < ToggleButtonsCurrentNumber)
        {
          engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(x
                                                   , y
                                                   , x + ToggleButtonsWidth
                                                   , y + ToggleButtonsHeight
                                                   , ToggleButtonsColor.Value);
        }
        else
        {
          engine.TrueVision.TVScreen2DImmediate.Draw_Box(x
                                         , y
                                         , x + ToggleButtonsWidth
                                         , y + ToggleButtonsHeight
                                         , ToggleButtonsColor.Value);
        }

        x += ToggleButtonsWidth + ToggleButtonsGapWidth;
        n++;
      }

      engine.TrueVision.TVScreen2DImmediate.Action_End2D();


      engine.TrueVision.TVScreen2DText.Action_BeginText();
      if (!string.IsNullOrEmpty(Text))
        engine.TrueVision.TVScreen2DText.TextureFont_DrawText(Text
                                                            , TextPosition.x
                                                            , TextPosition.y
                                                            , TextColor.Value
                                                            , font);

      if (!string.IsNullOrEmpty(SecondaryText))
        engine.TrueVision.TVScreen2DText.TextureFont_DrawText(SecondaryText
                                                      , SecondaryTextPosition.x
                                                      , SecondaryTextPosition.y
                                                      , SecondaryTextColor.Value
                                                      , font);
      engine.TrueVision.TVScreen2DText.Action_EndText();
    }

    public virtual bool IncrementToggleButtonNumber()
    {
      if (ToggleButtonsNumber - 1 > ToggleButtonsCurrentNumber)
      {
        ToggleButtonsCurrentNumber++;
        return true;
      }
      return false;
    }

    public virtual bool DecrementToggleButtonNumber()
    {
      if (ToggleButtonsCurrentNumber > 0)
      {
        ToggleButtonsCurrentNumber--;
        return true;
      }
      return false;
    }
  }
}
