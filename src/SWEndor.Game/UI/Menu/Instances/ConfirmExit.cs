using MTV3D65;
using SWEndor.Game.Sound;

namespace SWEndor.Game.UI.Menu.Pages
{
  public class ConfirmExit : Page
  {
    SelectionElement ConfirmText = new SelectionElement();
    SelectionElement ConfirmNo = new SelectionElement();
    SelectionElement ConfirmYes = new SelectionElement();

    public ConfirmExit(Screen2D owner) : base(owner)
    {
      ConfirmText.Text = "Quit Game?";
      ConfirmText.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(-150, -80);
      ConfirmText.HighlightBoxPosition = ConfirmText.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmText.HighlightBoxWidth = 240;
      ConfirmText.HighlightBoxHeight = 30;

      ConfirmNo.Text = "No";
      ConfirmNo.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, 20);
      ConfirmNo.HighlightBoxPosition = ConfirmNo.TextPosition - new TV_2DVECTOR(5, 5);
      ConfirmNo.HighlightBoxWidth = 60;
      ConfirmNo.HighlightBoxHeight = 30;
      ConfirmNo.Selectable = true;
      ConfirmNo.OnKeyPress += SelectNo;

      ConfirmYes.Text = "Yes";
      ConfirmYes.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(60, 60);
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
        Engine.SoundManager.SetSound(SoundGlobals.Exit);
        Engine.SoundManager.StopMusic();
        Engine.BeginExit();
        return false;
      }
      return false;
    }
  }
}
