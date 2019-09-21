using MTV3D65;

namespace SWEndor.UI.Widgets
{
  public class PerfText : Widget
  {
    public PerfText(Screen2D owner) : base(owner, "perf") { }

    public override bool Visible
    {
      get
      {
        return Engine.PerfManager.Enabled;
      }
    }

    public static char[] Lines = new char[] { '\n' };
    public override void Draw()
    {
      string perftext = Engine.PerfManager.Report;

      TV_2DVECTOR loc = new TV_2DVECTOR(10, 215);
      int lines = perftext.Split(Lines).Length;

      TVScreen2DImmediate.Action_Begin2D();
      TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      TVScreen2DImmediate.Action_End2D();

      TVScreen2DText.Action_BeginText();
      TVScreen2DText.TextureFont_DrawText(perftext
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), FontFactory.Get(Font.T08).ID);
      TVScreen2DText.Action_EndText();
    }
  }
}
