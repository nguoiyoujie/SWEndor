using MTV3D65;
using SWEndor.Game.Core;
using SWEndor.Game.Sound;

namespace SWEndor.Game.Input.Functions.Gameplay.Weapon
{
  public class SquadCommand : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_S;
    public static string InternalName = "g_squadassist";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerInfo.SquadronAssist();
      engine.SoundManager.SetSound(SoundGlobals.Button1);
    }
  }
}
