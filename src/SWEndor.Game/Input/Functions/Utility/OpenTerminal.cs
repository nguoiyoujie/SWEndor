using MTV3D65;
using SWEndor.Game.Core;

namespace SWEndor.Game.Input.Functions.Utility
{
  public class OpenTerminal : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_T;
    public static string InternalName = "u_terminal";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS | InputOptions.CTRL; } }

    public override void Process(Engine engine)
    {
      Terminal.TConsole.Visible = true;
    }
  }
}
