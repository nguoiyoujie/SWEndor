using MTV3D65;
using SWEndor.Input.Functions;

namespace SWEndor.Input.Context
{
  public abstract class AInputContext
  {
    protected AInputContext() { }

    public virtual void Set()
    {
      foreach (InputFunction fn in InputFunction.Registry.GetList())
        if (fn != null)
          fn.Enabled = false;
    }
    
    public virtual void HandleKeyBuffer(InputManager manager, TV_KEYDATA keydata) { }
    public virtual void HandleKeyState(InputManager manager, byte[] keyPressedStates) { }
    public virtual void HandleMouse(InputManager manager, int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll) { }
  }
}
