using SWEndor.UI.Menu;

namespace SWEndor.UI.Widgets
{
  public class WidgetPage : Widget
  {
    public WidgetPage() : base("page") { }

    public override bool Visible
    {
      get
      {
        return Screen2D.Instance().ShowPage;
      }
    }

    public override void Draw()
    {
      Page uip = Screen2D.Instance().CurrentPage;
      if (uip != null)
        uip.Show();
    }
  }
}
