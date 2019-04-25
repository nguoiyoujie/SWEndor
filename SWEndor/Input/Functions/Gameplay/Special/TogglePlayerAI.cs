using MTV3D65;

namespace SWEndor.Input.Functions.Gameplay.Special
{
  public class TogglePlayerAI : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_P;
    public static string InternalName = "d_playerai";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process()
    {
      Globals.Engine.PlayerInfo.PlayerAIEnabled = !Globals.Engine.PlayerInfo.PlayerAIEnabled;
    }
  }
}
