using MTV3D65;
using SWEndor.Game.Core;

namespace SWEndor.Game.Input.Functions.Utility.Game
{
  public class SaveSnapshot : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_F12;
    public static string InternalName = "u_savegame";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      GameSaver.Save(@"save.txt");
    }
  }
}
