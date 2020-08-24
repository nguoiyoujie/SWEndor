using MTV3D65;
using SWEndor.Game.Core;
using SWEndor.Game.Input.Functions;

namespace SWEndor.Game.Input.Context
{
  public abstract class AInputContext
  {
    protected AInputContext(InputManager manager) { Manager = manager; }

    protected InputManager Manager;
    public Engine Engine { get { return Manager.Engine; } }

    public virtual void Set()
    {
      foreach (InputFunction fn in InputFunction.Registry.Functions)
        if (fn != null)
          fn.Enabled = false;
    }

    public virtual void HandleKeyBuffer(TV_KEYDATA keydata)
    {
      if (keydata.Pressed > 0)
        InputFunction.Registry.ProcessOnPress(Engine, keydata.Key);
    }

    public virtual void HandleKeyState(byte[] keyPressedStates)
    {
      InputFunction.Registry.ProcessWhilePressed(Engine, keyPressedStates);
    }

    public virtual void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll) { }
  }
}
