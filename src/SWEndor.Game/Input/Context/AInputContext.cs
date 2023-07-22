using MTV3D65;
using SWEndor.Game.Core;
using SWEndor.Game.Input.Functions;
using SWEndor.Game.Sound;

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
      {
        // Handle pages
        if (Engine.Screen2D.CurrentPage?.OnKeyPress((CONST_TV_KEY)keydata.Key) ?? false)
          Engine.SoundManager.SetSound(SoundGlobals.Button1);

        InputFunction.Registry.ProcessOnPress(Engine, keydata.Key);
      }
    }

    public virtual void HandleKeyState(byte[] keyPressedStates)
    {
      for (int key = 0; key < keyPressedStates.Length; key++)
        if (keyPressedStates[key] != 0)
          Engine.Screen2D.CurrentPage?.WhileKeyPressed((CONST_TV_KEY)key);

      InputFunction.Registry.ProcessWhilePressed(Engine, keyPressedStates);
    }

    public virtual void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll) 
    {
      Engine.Screen2D.CurrentPage?.HandleMouse(mouseX, mouseY, button1, button2, button3, button4, mouseScroll);
    }
  }
}
