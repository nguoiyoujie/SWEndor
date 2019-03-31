namespace SWEndor.UI
{
  public class UIWidget_Page : UIWidget
  {
    public UIWidget_Page() : base("page") { }

    public override bool Visible
    {
      get
      {
        return Screen2D.Instance().ShowPage;
      }
    }

    public override void Draw()
    {
      UIPage uip = Screen2D.Instance().CurrentPage;
      if (uip != null)
        uip.Show();
    }
  }
}
