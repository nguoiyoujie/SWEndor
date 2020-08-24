using MTV3D65;
using SWEndor.Game.Input.Functions;
using SWEndor.Game.Input.Functions.Utility.Screen;
using SWEndor.Game.Sound;

namespace SWEndor.Game.Input.Context
{
  public class MenuInputContext : AInputContext
  {
    public MenuInputContext(InputManager manager) : base(manager) { }

    public static string[] _functions = new string[]
    {
      SaveScreenshot.InternalName
    };

    public override void Set()
    {
      base.Set();
      foreach (string s in _functions)
      {
        InputFunction fn = InputFunction.Registry.Get(s);
        if (fn != null)
          fn.Enabled = true;
      }
    }

    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      base.HandleKeyBuffer(keydata);
      if (keydata.Pressed > 0)
      {
        if (Engine.Screen2D.CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          Engine.SoundManager.SetSound(SoundGlobals.Button1);

        // Terminal
        //if (Engine.InputManager.CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
        //  TConsole.Visible = true;
      }
    }
  }
}
