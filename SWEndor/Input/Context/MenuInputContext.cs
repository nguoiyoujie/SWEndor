using MTV3D65;
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
        if (Globals.Engine.Screen2D.CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          Globals.Engine.SoundManager.SetSound("button_1");

        // Terminal
        if (Globals.Engine.InputManager.CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
          TConsole.Visible = true;

        // Terminal
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_1))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-EMP-SM", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_2))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-EMP-LG", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }
        
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_3))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-REB-SM", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_4))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-REB-LG", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_5))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-NEU-SM", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_6))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-NEU-LG", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }

        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_7))
        {
          string[] win = new string[] { "dynamic\\S-WIN-1", "dynamic\\S-WIN-2", "dynamic\\S-WIN-3", "dynamic\\S-WIN-4" };
          Globals.Engine.SoundManager.QueueMusic(win[Globals.Engine.Random.Next(0, win.Length)], 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }
        
        if (keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_8))
        {
          Globals.Engine.SoundManager.QueueMusic("dynamic\\S-WIN-LG", 1657);
          Globals.Engine.SoundManager.SetSound("button_1");
        }
      }
    }
  }
}
