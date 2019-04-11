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
        if (Screen2D.Instance().CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          SoundManager.Instance().SetSound("button_1");

        // Terminal
        if (InputManager.Instance().CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
          TConsole.Visible = true;
      }
    }
  }
}
