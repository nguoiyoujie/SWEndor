using MTV3D65;
using SWEndor.Primitives;
using System;

namespace SWEndor.UI.Menu.Pages
{
  public class FatalError : Page
  {
    SelectionElement Cover = new SelectionElement();
    SelectionElement MainText = new SelectionElement();
    SelectionElement Instructions = new SelectionElement();
    SelectionElement ButtonReturn = new SelectionElement();
    string errorfilename = @"error.txt";

    public FatalError(Screen2D owner, Exception exception) : base(owner)
    {
      Log.WriteErr(Log.ERROR, exception);

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = Globals.Engine.ScreenWidth;
      Cover.HighlightBoxHeight = Globals.Engine.ScreenHeight;
      Cover.UnHighlightBoxPositionColor = new TV_COLOR(0, 0, 0, 0.6f);

      MainText.Text = "FATAL ERROR ENCOUNTERED";
      MainText.TextFont = FontFactory.Get(Font.T24).ID;
      MainText.TextColor = new TV_COLOR(0.8f, 0.2f, 0.2f, 1);
      MainText.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 - 200, Globals.Engine.ScreenHeight / 2 - 180);

      Instructions.Text = "A fatal error has been encountered and the program needs to close.\nPlease see "
        .C(errorfilename)
        .C(" in the /Log folder for the error message.\n\n")
        .C(Utilities.Multiline("Error: ".C(exception.Message), 72));
      Instructions.TextFont = FontFactory.Get(Font.T12).ID;
      Instructions.TextColor = new TV_COLOR(0.8f, 0.2f, 0.2f, 1);
      Instructions.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 - 250, Globals.Engine.ScreenHeight / 2 - 130);

      ButtonReturn.Text = "Exit";
      ButtonReturn.TextPosition = new TV_2DVECTOR(Globals.Engine.ScreenWidth / 2 + 100, Globals.Engine.ScreenHeight / 2 + 60);
      ButtonReturn.HighlightBoxPosition = ButtonReturn.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonReturn.HighlightBoxWidth = 200;
      ButtonReturn.HighlightBoxHeight = 30;
      ButtonReturn.TextColor = new TV_COLOR(0.8f, 0.2f, 0.2f, 1);
      ButtonReturn.HighlightBoxPositionColor = new TV_COLOR(0.8f, 0.2f, 0.2f, 0.3f);
      ButtonReturn.Selectable = true;
      ButtonReturn.OnKeyPress += SelectReturn;

      Elements.Add(Cover);
      Elements.Add(MainText);
      Elements.Add(Instructions);
      Elements.Add(ButtonReturn);
      SelectedElementID = Elements.IndexOf(ButtonReturn);
    }

    private bool SelectReturn(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Engine.SoundManager.SetSound("r23");
        Engine.SoundManager.SetMusicStop();
        Engine.BeginExit();
        return true;
      }
      return false;
    }
  }
}
