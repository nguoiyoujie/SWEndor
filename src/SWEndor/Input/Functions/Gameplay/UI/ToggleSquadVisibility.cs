using MTV3D65;
using SWEndor.Core;
using Primrose.Primitives.Extensions;
using SWEndor.Sound;
using System;

namespace SWEndor.Input.Functions.Gameplay.UI
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
      Globals.Engine.Screen2D.ShowSquad = (Screen2D.ShowSquadMode)((((int)Globals.Engine.Screen2D.ShowSquad) + 1) % Enum.GetValues(typeof(Screen2D.ShowSquadMode)).Length);
      Globals.Engine.Screen2D.MessageSecondaryText(TextLocalization.Get(TextLocalKeys.SQUAD_INDICATOR).F(Globals.Engine.Screen2D.ShowSquad)
                                                 , 2.5f
                                                 , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL));

      Globals.Engine.SoundManager.SetSound(SoundGlobals.Button1);
    }
  }
}
