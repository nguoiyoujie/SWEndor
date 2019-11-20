using MTV3D65;
using Primrose.Primitives.Extensions;
using SWEndor.Sound;
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
    bool exitGame = false;

    public FatalError(Screen2D owner, Exception exception) : base(owner)
    {
      exitGame = GameScenarioManager.IsMainMenu;

      Log.WriteErr(Log.ERROR, exception);

      Cover.HighlightBoxPosition = new TV_2DVECTOR();
      Cover.HighlightBoxWidth = owner.ScreenSize.x;
      Cover.HighlightBoxHeight = owner.ScreenSize.y;
      Cover.UnHighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.UI_BACKGROUND_DARK);

      MainText.Text = TextLocalization.Get(TextLocalKeys.SYSTEM_DISP_FATAL_ERROR);
      MainText.TextFont = FontFactory.Get(Font.T24).ID;
      MainText.TextColor = ColorLocalization.Get(ColorLocalKeys.SYSTEM_FATAL);
      MainText.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(-200, -180);

      Instructions.Text = TextLocalization.Get(TextLocalKeys.SYSTEM_TEXT_FATAL_ERROR).F(errorfilename)
        .C("Error: ".C(exception.Message).Multiline(72));
      Instructions.TextFont = FontFactory.Get(Font.T12).ID;
      Instructions.TextColor = ColorLocalization.Get(ColorLocalKeys.SYSTEM_FATAL);
      Instructions.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(-250, -130);

      ButtonReturn.Text = exitGame ? "Exit" : "Return";
      ButtonReturn.TextPosition = owner.ScreenCenter + new TV_2DVECTOR(100, 60);
      ButtonReturn.HighlightBoxPosition = ButtonReturn.TextPosition - new TV_2DVECTOR(5, 5);
      ButtonReturn.HighlightBoxWidth = 200;
      ButtonReturn.HighlightBoxHeight = 30;
      ButtonReturn.TextColor = ColorLocalization.Get(ColorLocalKeys.SYSTEM_FATAL);
      ButtonReturn.HighlightBoxColor = ColorLocalization.Get(ColorLocalKeys.SYSTEM_FATAL_BACKGROUND);
      ButtonReturn.Selectable = true;
      if (exitGame)
        ButtonReturn.OnKeyPress += ProceedQuit;
      else
        ButtonReturn.OnKeyPress += ProceedMainMenu;

      Elements.Add(Cover);
      Elements.Add(MainText);
      Elements.Add(Instructions);
      Elements.Add(ButtonReturn);
      SelectedElementID = Elements.IndexOf(ButtonReturn);
    }

    private bool ProceedQuit(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        Engine.SoundManager.SetSound(SoundGlobals.Exit);
        Engine.SoundManager.SetMusicStop();
        Engine.BeginExit();
        return true;
      }
      return false;
    }

    private bool ProceedMainMenu(CONST_TV_KEY key)
    {
      if (key == CONST_TV_KEY.TV_KEY_RETURN)
      {
        GameScenarioManager.Reset();
        GameScenarioManager.LoadMainMenu();
        Engine.SoundManager.SetMusicResume();
        return true;
      }
      return false;
    }
  }
}
