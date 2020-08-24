using MTV3D65;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Sound;
using System;

namespace SWEndor.Game.Input.Functions.Gameplay.UI
{
  public class ToggleSquadVisibility : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_W;
    public static string InternalName = "g_squad_toggle";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.Screen2D.ShowSquad = (Screen2D.ShowSquadMode)((((int)engine.Screen2D.ShowSquad) + 1) % Enum.GetValues(typeof(Screen2D.ShowSquadMode)).Length);
      engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.SQUAD_INDICATOR).F(engine.Screen2D.ShowSquad)
                                                 , 2.5f
                                                 , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL));

      engine.SoundManager.SetSound(SoundGlobals.Button1);
    }
  }
}
