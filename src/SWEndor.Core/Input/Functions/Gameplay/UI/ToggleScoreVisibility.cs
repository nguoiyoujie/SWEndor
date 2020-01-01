using MTV3D65;
using SWEndor.Core;
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

    public override void Process(Engine engine)
    {
      engine.Screen2D.ShowScore = !engine.Screen2D.ShowScore;
      engine.SoundManager.SetSound(SoundGlobals.Button1);
    }
  }
}
