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
        return Globals.Engine.Screen2D.ShowPage;
      }
    }

    public override void Draw()
    {
      Page uip = Globals.Engine.Screen2D.CurrentPage;
      if (uip != null)
        uip.Show();
    }
  }
}
