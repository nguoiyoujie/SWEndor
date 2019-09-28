using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay.Special
{
  public class TimeSlow : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_MINUS;
    public static string InternalName = "d_timespeed-";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.WHILEPRESSED; } }

    public override void Process(Engine engine)
    {
      engine.Game.TimeControl.SpeedModifier *= 0.9f;
      engine.Game.TimeControl.SpeedModifier = engine.Game.TimeControl.SpeedModifier.Clamp(0.01f, 10);
      engine.Screen2D.MessageSecondaryText(string.Format("DEV: TIMEMULT = {0:0.00}", engine.Game.TimeControl.SpeedModifier)
                                                      , 1.5f
                                                      , new TV_COLOR(0.5f, 0.5f, 1, 1)
                                                      , 99);
    }
  }
}
