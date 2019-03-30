using MTV3D65;

namespace SWEndor.UI
{
  public class UIWidget_PerfText : UIWidget
  {
    public UIWidget_PerfText() : base("perf") { }

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

      Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
      Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(loc.x - 5, loc.y - 5, loc.x + 405, loc.y + 40 / 3 * lines + 5, new TV_COLOR(0, 0, 0, 0.5f).GetIntColor());
      Engine.Instance().TVScreen2DImmediate.Action_End2D();

      Engine.Instance().TVScreen2DText.Action_BeginText();
      Engine.Instance().TVScreen2DText.TextureFont_DrawText(perftext
        , loc.x, loc.y, new TV_COLOR(0.6f, 0.8f, 0.6f, 1).GetIntColor(), Font.GetFont("Text_08").ID);
      Engine.Instance().TVScreen2DText.Action_EndText();
    }
  }
}
