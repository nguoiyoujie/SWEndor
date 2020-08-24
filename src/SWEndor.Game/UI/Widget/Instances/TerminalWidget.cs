using MTV3D65;
using SWEndor.Game.Terminal;

namespace SWEndor.Game.UI.Widgets
{
  public class TerminalWidget : Widget
  {
    public TerminalWidget(Screen2D owner) : base(owner, "terminal") { }

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
      TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, Engine.ScreenWidth - 5, Engine.ScreenHeight - 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      float y = loc.y;
      foreach (string t in text)
      {
        TVScreen2DText.TextureFont_DrawText(t
          , loc.x, y, new TV_COLOR(1, 1, 1, 1).GetIntColor(), FontFactory.Get(Font.T08).ID);
        y += 40f / 3f;
      }
      TVScreen2DText.TextureFont_DrawText(TConsole.InputLine
        , loc.x, Engine.ScreenHeight - loc.y - 40f / 3f, new TV_COLOR(1, 1, 1, 1).GetIntColor(), FontFactory.Get(Font.T08).ID);
      TVScreen2DText.Action_EndText();
    }
  }
}
