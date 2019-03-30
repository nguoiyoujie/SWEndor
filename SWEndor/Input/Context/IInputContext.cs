using MTV3D65;

namespace SWEndor.Input.Context
{
  public interface IInputContext
  {
    void HandleKeyBuffer(TV_KEYDATA keydata);
    void HandleKeyState(byte[] keyPressedStates);
    void HandleMouse(int mouseX, int mouseY, bool button1, bool button2, bool button3, bool button4, int mouseScroll);
  }
}
