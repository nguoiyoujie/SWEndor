using SWEndor.Player;
using SWEndor.Sound;

namespace SWEndor.Input.Functions.Gameplay.Weapon
{
  public class PrevPrimary : InputFunction
  {
    private int _key = -1;
    public static string InternalName = "g_weap1mode-";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      Globals.Engine.PlayerInfo.PrevPrimaryWeapon();
      Globals.Engine.SoundManager.SetSound("button_1");
    }
  }
}
