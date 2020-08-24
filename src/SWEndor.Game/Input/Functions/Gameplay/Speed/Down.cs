using MTV3D65;
using SWEndor.Game.Core;

namespace SWEndor.Game.Input.Functions.Gameplay.Speed
{
  public class Down : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_A;
    public static string InternalName = "g_speed-";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.WHILEPRESSED; } }

    public override void Process(Engine engine)
    {
      engine.PlayerInfo.ChangeSpeed(-1);
    }
  }
}
