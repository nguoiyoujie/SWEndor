using MTV3D65;
using SWEndor.Terminal;

namespace SWEndor.Input.Context
{
  public class TerminalGameInputContext : GameInputContext
  {
    public TerminalGameInputContext(InputManager manager) : base(manager) { }

    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      // base.HandleKeyBuffer(keydata); // disable
      if (keydata.Key == (int)CONST_TV_KEY.TV_KEY_ESCAPE)
      {
        TConsole.Visible = false;
      }

      if (keydata.Pressed > 0)
      {
        if (((CONST_TV_KEY)keydata.Key).Equals(CONST_TV_KEY.TV_KEY_RETURN))
          TConsole.Execute();
        else if (((CONST_TV_KEY)keydata.Key).Equals(CONST_TV_KEY.TV_KEY_DELETE) 
              || ((CONST_TV_KEY)keydata.Key).Equals(CONST_TV_KEY.TV_KEY_BACKSPACE))
          TConsole.InputLine = TConsole.InputLine.Substring(0, TConsole.InputLine.Length - 1);
        else
        {
          char chr = Utilities.TVKeyToChar(Globals.Engine.InputManager.SHIFT, (CONST_TV_KEY)keydata.Key);
          if (!char.IsControl(chr))
            TConsole.InputLine += chr;
        }
      }
    }

    public override void HandleKeyState(byte[] keyPressedStates)
    {
      //base.HandleKeyState(keyPressedStates); // disable
    }
  }
}
