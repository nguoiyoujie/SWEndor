using MTV3D65;
using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay.Special
{
  public class ToggleMovementLock : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_N;
    public static string InternalName = "d_movelock";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      engine.PlayerInfo.IsMovementControlsEnabled = !engine.PlayerInfo.IsMovementControlsEnabled;
      engine.Screen2D.MessageSecondaryText(engine.PlayerInfo.IsMovementControlsEnabled ? "Movement Unlocked" : "Movement Locked"
                                                 , 2.5f
                                                 , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL));
    }
  }
}
