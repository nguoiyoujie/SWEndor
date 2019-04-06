using MTV3D65;
using SWEndor.Sound;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class ToggleUIVisibility : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_U;
    public static string InternalName = "g_ui_toggle";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      Screen2D.Instance().ShowUI = !Screen2D.Instance().ShowUI;
      SoundManager.Instance().SetSound("button_1");
    }
  }
}
