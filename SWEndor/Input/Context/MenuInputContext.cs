using MTV3D65;
using SWEndor.Sound;
using SWEndor.Terminal;

namespace SWEndor.Input.Context
{
  public class MenuInputContext : IInputContext
  {
    public virtual void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      if (keydata.Pressed > 0)
      {
        if (Screen2D.Instance().CurrentPage.OnKeyPress((CONST_TV_KEY)keydata.Key))
          SoundManager.Instance().SetSound("button_1");

        // Terminal
        if (InputManager.Instance().CTRL && keydata.Key.Equals((int)CONST_TV_KEY.TV_KEY_T))
        {
          TConsole.Visible = true;
        }
      }
    }

    public virtual void HandleKeyState(byte[] keyPressedStates)
    {
      // null
    }

    public virtual void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll)
    {
      
    }
  }
}
