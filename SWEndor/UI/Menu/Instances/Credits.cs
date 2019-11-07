using MTV3D65;
using Primrose.Primitives.Extensions;
using System.IO;
using System.Text;

namespace SWEndor.UI.Menu.Pages
{
  public class Credits : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement CreditText = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();


    public Credits(Screen2D owner) : base(owner)
    {
      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_UNHIGHLIGHT_BACKGROUND);

      MainText.Text = "Credits";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      string credit = "";
      string path = Path.Combine(Globals.DataPath, "credits.txt");
      if (File.Exists(path))
        credit = File.ReadAllText(path).Multiline(60);

      CreditText.Text = credit;
      CreditText.TextFont = FontFactory.Get(Font.T12).ID;
      CreditText.TextPosition = new TV_2DVECTOR(75, 120);
      CreditText.HighlightBoxPosition = CreditText.TextPosition - new TV_2DVECTOR(5, 5);
      CreditText.HighlightBoxWidth = 660;
      CreditText.HighlightBoxHeight = 350;

      ButtonExit.Text = "Back";
      ButtonExit.TextPosition = owner.ScreenSize + new TV_2DVECTOR(-200, -80);
      ButtonExit.HighlightBoxPosition = ButtonExit.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonExit.HighlightBoxWidth = 200;
      ButtonExit.HighlightBoxHeight = 30;
      ButtonExit.Selectable = true;
      ButtonExit.OnKeyPress += SelectExit;

      Elements.Add(Cover);
      Elements.Add(MainText);
      Elements.Add(CreditText);
      Elements.Add(ButtonExit);
      SelectedElementID = Elements.IndexOf(ButtonExit);
    }
    
    private bool SelectExit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Back();
        return true;
      }
      return false;
    }
  }
}
