using MTV3D65;
using SWEndor.Sound;
using SWEndor.Terminal;

namespace SWEndor.Input.Context
{
  public class MenuInputContext : AInputContext
  {
    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      if (keydata.Pressed > 0)
      {
        if (Globals.Engine.Screen2D.CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          Globals.Engine.SoundManager.SetSound("button_1");

        // Terminal
        if (Globals.Engine.InputManager.CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
          TConsole.Visible = true;

        // Terminal
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_0))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-EMP-SM", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_1))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-EMP-LG", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }
        
      }
    }
  }
}
