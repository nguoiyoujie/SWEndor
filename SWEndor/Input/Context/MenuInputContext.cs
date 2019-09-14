using MTV3D65;
using SWEndor.Actors;
using SWEndor.Terminal;
using System.Collections.Generic;

namespace SWEndor.Input.Context
{
  public class MenuInputContext : AInputContext
  {
    public MenuInputContext(InputManager manager) : base(manager) { }

    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      if (keydata.Pressed > 0)
      {
        if (Engine.Screen2D.CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          Engine.SoundManager.SetSound("button_1");

        // Terminal
        if (Engine.InputManager.CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
          TConsole.Visible = true;
      }
    }
  }
}
