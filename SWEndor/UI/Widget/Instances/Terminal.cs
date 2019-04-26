using MTV3D65;
using SWEndor.Terminal;

namespace SWEndor.UI.Widgets
{
  public class WidgetTerminal : Widget
  {
    public WidgetTerminal(Screen2D owner) : base(owner, "terminal") { }

    public override bool Visible
    {
      get
      {
        return TConsole.Visible;
      }
    }

    public override void Draw()
    {
      string[] text = TConsole.GetLines();

      TV_2DVECTOR loc = new TV_2DVECTOR(10, 10);
      //int lines = perftext.Split('\n').Length;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, this.GetEngine().ScreenWidth - 5, this.GetEngine().ScreenHeight - 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      float y = loc.y;
      foreach (string t in text)
      {
        TVScreen2DText.TextureFont_DrawText(t
          , loc.x, y, new TV_COLOR(1, 1, 1, 1).GetIntColor(), Font.Factory.Get("Text_08").ID);
        y += 40f / 3f;
      }
      TVScreen2DText.TextureFont_DrawText(TConsole.InputLine
        , loc.x, this.GetEngine().ScreenHeight - loc.y - 40f / 3f, new TV_COLOR(1, 1, 1, 1).GetIntColor(), Font.Factory.Get("Text_08").ID);
      TVScreen2DText.Action_EndText();
    }
  }
}
