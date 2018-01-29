using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWEndor
{
  public class UIPage_Credits : UIPage
  {
    UISelectionElement Cover = new UISelectionElement();
    UISelectionElement MainText = new UISelectionElement();
    UISelectionElement CreditText = new UISelectionElement();
    UISelectionElement ButtonExit = new UISelectionElement();


    public UIPage_Credits()
    {
      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Engine.Instance().ScreenWidth;
      Cover.HighlightBoxHeight = Engine.Instance().ScreenHeight;
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
      ButtonExit.TextPosition = new TV_2DVECTOR(Engine.Instance().ScreenWidth - 200, Engine.Instance().ScreenHeight - 80);
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
