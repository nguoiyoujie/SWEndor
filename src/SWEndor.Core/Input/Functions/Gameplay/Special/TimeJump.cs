using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay.Special
{
  public class TimeJump : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_M;
    public static string InternalName = "d_time+";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.Game.AddTime = 5;
    }
  }
}
