using MTV3D65;

namespace SWEndor.Input.Functions.Gameplay.Weapon
{
  public class RemoveLockOn : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_SPACE;
    public static string InternalName = "g_unlock";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      if (engine.PlayerInfo.AimTargetID >= 0 || engine.PlayerInfo.AssistTargetID >= 0)
      {
        engine.PlayerInfo.AimTargetID = -1;
        engine.PlayerInfo.AssistTargetID = -1;
        Globals.Engine.Screen2D.MessageSecondaryText("Target cleared.", 5, new TV_COLOR(0.5f, 0.5f, 1, 1));
        engine.SoundManager.SetSound("button_1");
      }
    }
  }
}
