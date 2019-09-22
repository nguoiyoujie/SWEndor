using MTV3D65;
using SWEndor.Sound;

namespace SWEndor.Input.Functions.Gameplay.Weapon
{
  public class NextPrimary : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_Z;
    public static string InternalName = "g_weap1mode+";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerInfo.NextPrimaryWeapon();
      engine.SoundManager.SetSound(SoundGlobals.Button1);
    }
  }
}
