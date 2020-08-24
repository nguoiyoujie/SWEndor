using MTV3D65;
using System.Text;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class LoadingGame : Page
  {
    readonly SelectionElement MainText = new SelectionElement();

    public LoadingGame(Screen2D owner) : base(owner)
    {
      MainText.Text = "";
      MainText.TextPosition = new TV_2DVECTOR(40, 40);
      MainText.TextColor = ColorLocalization.Get(ColorLocalKeys.WHITE);
      MainText.TextFont = FontFactory.Get(Font.T12).ID;

      Elements.Add(MainText);
    }

    private readonly StringBuilder loadingText = new StringBuilder(1000);

    public override void RenderTick()
    {
      int i = 0;
      while (Owner.LoadingTextLines.Count > 20)
      {
        Owner.LoadingTextLines.RemoveAt(0);
      }

      loadingText.Clear();
      while (i < Owner.LoadingTextLines.Count)
      {
        loadingText.AppendLine(Owner.LoadingTextLines[i]);
        i++;
      }

      MainText.Text = loadingText.ToString();
    }
  }
}
