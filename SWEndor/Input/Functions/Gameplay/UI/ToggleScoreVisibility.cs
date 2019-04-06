using System;
using MTV3D65;
using SWEndor.Sound;

namespace SWEndor.Input.Functions.Gameplay.UI
{
  public class ToggleScoreVisibility : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_T;
    public static string InternalName = "g_ui_score_toggle";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      Screen2D.Instance().ShowScore = !Screen2D.Instance().ShowScore;
      SoundManager.Instance().SetSound("button_1");
    }
  }
}
