using MTV3D65;
using System.Text;

namespace SWEndor.UI.Menu.Pages
{
  public class Credits : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement CreditText = new SelectionElement();
    SelectionElement ButtonExit = new SelectionElement();


    public Credits()
    {
      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Globals.Engine.ScreenWidth;
      Cover.HighlightBoxHeight = Globals.Engine.ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.3f);

      MainText.Text = "Credits";
      MainText.TextPosition = new TV_2DVECTOR(40, 60);

      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Author: Nguoi You Jie (2018)");
      sb.AppendLine();
      sb.AppendLine("Inspired by an original game by Bruno R. Marcos");
      sb.AppendLine("             (Battle of Endor, Battle of Yavin)");
      sb.AppendLine("Models: Master_Syrus, Lurker, Hory, Trousers, ");
      sb.AppendLine("        Yo-da-man (2007)");
      sb.AppendLine();
      sb.AppendLine("All copyrights regarding StarWars belong to ");
      sb.AppendLine("whoever owns the franchise (LucasArts / Disney)");

      CreditText.Text = sb.ToString();
      CreditText.TextPosition = new TV_2DVECTOR(75, 120);
      CreditText.HighlightBoxPosition = CreditText.TextPosition - new TV_2DVECTOR(5, 5);
      CreditText.HighlightBoxWidth = 660;
      CreditText.HighlightBoxHeight = 350;

      ButtonExit.Text = "Back";
      ButtonExit.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth - 200, Globals.Engine.ScreenHeight - 80);
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
