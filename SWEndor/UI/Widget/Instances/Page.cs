using SWEndor.UI.Menu;

namespace SWEndor.UI.Widgets
{
  public class WidgetPage : Widget
  {
    public WidgetPage(Screen2D owner) : base(owner, "page") { }

    public override bool Visible
    {
      get
      {
        return Owner.ShowPage;
      }
    }

    public override void Draw()
    {
      Page uip = Owner.CurrentPage;
      if (uip != null)
        uip.Show();
    }
  }
}
