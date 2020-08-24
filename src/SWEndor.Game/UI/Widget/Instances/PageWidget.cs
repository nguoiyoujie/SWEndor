using SWEndor.Game.UI.Menu;

namespace SWEndor.Game.UI.Widgets
{
  public class PageWidget : Widget
  {
    public PageWidget(Screen2D owner) : base(owner, "page") { }

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
