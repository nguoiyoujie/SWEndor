using MTV3D65;
using System.Threading;

namespace SWEndor.UI.Menu.Pages
{
  public class ConfirmExit : Page
  {
    SelectionElement ConfirmText = new SelectionElement();
    SelectionElement ConfirmNo = new SelectionElement();
    SelectionElement ConfirmYes = new SelectionElement();

    public ConfirmExit()
    {
      ConfirmText.Text = "Confirm Exit?";
      ConfirmText.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 - 150, Globals.Engine.ScreenHeight / 2 - 80);
      ConfirmText.HighlightBoxPosition = ConfirmText.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmText.HighlightBoxWidth = 240;
      ConfirmText.HighlightBoxHeight = 30;

      ConfirmNo.Text = "NO";
      ConfirmNo.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 + 60, Globals.Engine.ScreenHeight / 2 + 20);
      ConfirmNo.HighlightBoxPosition = ConfirmNo.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmNo.HighlightBoxWidth = 60;
      ConfirmNo.HighlightBoxHeight = 30;
      ConfirmNo.Selectable = true;
      ConfirmNo.OnKeyPress += SelectNo;

      ConfirmYes.Text = "YES";
      ConfirmYes.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 + 60, Globals.Engine.ScreenHeight / 2 + 60);
      ConfirmYes.HighlightBoxPosition = ConfirmYes.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmYes.HighlightBoxWidth = 60;
      ConfirmYes.HighlightBoxHeight = 30;
      ConfirmYes.Selectable = true;
      ConfirmYes.OnKeyPress += SelectYes;

      Elements.Add(ConfirmText);
      Elements.Add(ConfirmNo);
      Elements.Add(ConfirmYes);
      SelectedElementID = Elements.IndexOf(ConfirmNo);
    }

    private bool SelectNo(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Back();
        return true;
      }
      return false;
    }

    private bool SelectYes(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Globals.Engine.SoundManager.SetSound("r23");
        Globals.Engine.SoundManager.SetMusicStop();
        Thread.Sleep(1500);
        Globals.Engine.Exit();
        return false;
      }
      return false;
    }
  }
}
