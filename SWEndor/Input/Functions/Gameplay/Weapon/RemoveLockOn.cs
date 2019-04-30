using MTV3D65;

namespace SWEndor.Input.Functions.Gameplay.Weapon
{
  public class RemoveLockOn : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_SPACE;
    public static string InternalName = "g_weapunlock";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerInfo.AimTargetID = -1;
      engine.SoundManager.SetSound("button_1");
    }
  }
}
