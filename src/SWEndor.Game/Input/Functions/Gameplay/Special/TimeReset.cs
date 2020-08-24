using MTV3D65;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Input.Functions.Gameplay.Special
{
  public class TimeReset : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_BACKSPACE;
    public static string InternalName = "d_timespeed#";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.WHILEPRESSED; } }

    public override void Process(Engine engine)
    {
      engine.Game.TimeControl.SpeedModifier = 1;
      engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.TIME_MULT).F(engine.Game.TimeControl.SpeedModifier)
                                                      , 1.5f
                                                      , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL)
                                                      , 99);
    }
  }
}
