using MTV3D65;
using SWEndor.Terminal;

namespace SWEndor.UI
{
  public class UIWidget_Terminal : UIWidget
  {
    public UIWidget_Terminal() : base("terminal") { }

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

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, Engine.Instance().ScreenWidth - 5, Engine.Instance().ScreenHeight - 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      Engine.Instance().TVScreen2DText.Action_BeginText();
      float y = loc.y;
      foreach (string t in text)
      {
        Engine.Instance().TVScreen2DText.TextureFont_DrawText(t
          , loc.x, y, new TV_COLOR(1, 1, 1, 1).GetIntColor(), Font.GetFont("Text_08").ID);
        y += 40f / 3f;
      }
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(TConsole.InputLine
        , loc.x, Engine.Instance().ScreenHeight - loc.y - 40f / 3f, new TV_COLOR(1, 1, 1, 1).GetIntColor(), Font.GetFont("Text_08").ID);
      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
