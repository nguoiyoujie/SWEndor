using MTV3D65;
using SWEndor.Game.Input.Functions;
using SWEndor.Game.Input.Functions.Utility;
using SWEndor.Game.Input.Functions.Utility.Game;
using SWEndor.Game.Input.Functions.Utility.Screen;

namespace SWEndor.Game.Input.Context
{
  public class MenuInputContext : AInputContext
  {
    public MenuInputContext(InputManager manager) : base(manager) { }

    public static string[] _functions = new string[]
    {
      SaveScreenshot.InternalName,
      SaveSnapshot.InternalName,
      OpenTerminal.InternalName
    };

    public override void Set()
    {
      base.Set();
      foreach (string s in _functions)
      {
        int index = -1;
        while ((index = InputFunction.Registry.GetNext(s, ++index, out InputFunction fn)) != -1)
        {
          if (fn != null)
            fn.Enabled = true;
        }
      }
    }

    public override void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      base.HandleKeyBuffer(keydata);
      // Move to AInputContext
      //if (keydata.Pressed > 0)
      //{
      //  if (Engine.Screen2D.CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
      //    Engine.SoundManager.SetSound(SoundGlobals.Button1);
      //}
    }
  }
}
