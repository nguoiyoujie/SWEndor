using MTV3D65;
using System.Collections.Generic;

namespace SWEndor.UI
{
  public delegate bool KeyEvent(CONST_TV_KEY key);

  public class SelectionElement
  {
    public bool Selectable;

    public string Text = "";
    public TV_2DVECTOR TextPosition = new TV_2DVECTOR();
    public TV_COLOR TextColor = new TV_COLOR(0.8f, 0.8f, 0, 1);
    public int TextFont = -1;

    public string SecondaryText = "";
    public TV_2DVECTOR SecondaryTextPosition = new TV_2DVECTOR();
    public TV_COLOR SecondaryTextColor = new TV_COLOR(0.8f, 0.8f, 0, 1);
    public int SecondaryTextFont = -1;

    public TV_2DVECTOR HighlightBoxPosition = new TV_2DVECTOR();
    public float HighlightBoxWidth = 0;
    public float HighlightBoxHeight = 0;
    public TV_COLOR UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.8f);
    public TV_COLOR HighlightBoxPositionColor = new TV_COLOR(0.05f, 0.2f, 0, 0.8f);

    public int ToggleButtonsNumber = 0;
    public int ToggleButtonsCurrentNumber = 0;
    public float ToggleButtonsWidth = 10;
    public float ToggleButtonsHeight = 20;
    public float ToggleButtonsGapWidth = 5;
    public TV_2DVECTOR ToggleButtonsPosition = new TV_2DVECTOR();
    public TV_COLOR ToggleButtonsColor = new TV_COLOR(0.8f, 0.8f, 0, 1);
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
                                       , (ishighlighted) ? HighlightBoxPositionColor.GetIntColor() : UnHighlightBoxPositionColor.GetIntColor());

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
                                                   , ToggleButtonsColor.GetIntColor());
        }
        else
        {
          engine.TrueVision.TVScreen2DImmediate.Draw_Box(x
                                         , y
                                         , x + ToggleButtonsWidth
                                         , y + ToggleButtonsHeight
                                         , ToggleButtonsColor.GetIntColor());
        }

        x += ToggleButtonsWidth + ToggleButtonsGapWidth;
        n++;
      }

      engine.TrueVision.TVScreen2DImmediate.Action_End2D();


      engine.TrueVision.TVScreen2DText.Action_BeginText();
      engine.TrueVision.TVScreen2DText.TextureFont_DrawText(Text
                                                            , TextPosition.x
                                                            , TextPosition.y
                                                            , TextColor.GetIntColor()
                                                            , font);

      engine.TrueVision.TVScreen2DText.TextureFont_DrawText(SecondaryText
                                                      , SecondaryTextPosition.x
                                                      , SecondaryTextPosition.y
                                                      , SecondaryTextColor.GetIntColor()
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
