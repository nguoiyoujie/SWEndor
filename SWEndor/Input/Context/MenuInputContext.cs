﻿using MTV3D65;
using SWEndor.Sound;
using SWEndor.Terminal;

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
          Engine.SoundManager.SetSound(SoundGlobals.Button1);

        // Terminal
        if (Engine.InputManager.CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
          TConsole.Visible = true;
      }
    }
  }
}
