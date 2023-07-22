using MTV3D65;

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

    private int cacheLength = 0;

    public override void RenderTick()
    {
      int entrylimit = 20;
      int i = Owner.LoadingText.Length;
      int entryfound = 0;
      bool changed = cacheLength != Owner.LoadingText.Length;
      if (changed)
      {
        while (--i > 0)
        {
          if (Owner.LoadingText[i] == '\n')
          {
            entryfound++;
            if (entryfound >= entrylimit)
            {
              Owner.LoadingText.Remove(0, i + 1);
              break;
            }
          }
        }
        cacheLength = Owner.LoadingText.Length;
        MainText.Text = Owner.LoadingText.ToString();
      }
    }
  }
}
