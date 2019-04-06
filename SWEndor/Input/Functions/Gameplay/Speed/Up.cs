using MTV3D65;
using SWEndor.Player;

namespace SWEndor.Input.Functions.Gameplay.Speed
{
  public class Up : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_Q;
    public static string InternalName = "g_speed+";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.WHILEPRESSED; } }

    public override void Process()
    {
      PlayerInfo.Instance().ChangeSpeed(1);
    }
  }
}