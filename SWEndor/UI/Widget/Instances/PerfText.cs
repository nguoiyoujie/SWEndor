using MTV3D65;

namespace SWEndor.UI.Widgets
{
  public class PerfText : Widget
  {
    public PerfText() : base("perf") { }

    public override bool Visible
    {
      get
      {
        return Settings.ShowPerformance;
      }
    }

    public override void Draw()
    {
      string perftext = PerfManager.Report;

      TV_2DVECTOR loc = new TV_2DVECTOR(10, 215);
      int lines = perftext.Split('\n').Length;

      Globals.Engine.TVScreen2DImmediate.Action_Begin2D();
      Globals.Engine.TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Globals.Engine.TVScreen2DImmediate.Action_End2D();

      Globals.Engine.TVScreen2DText.Action_BeginText();
      Globals.Engine.TVScreen2DText.TextureFont_DrawText(perftext
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), Font.Factory.Get("Text_08").ID);
      Globals.Engine.TVScreen2DText.Action_EndText();
    }
  }
}
