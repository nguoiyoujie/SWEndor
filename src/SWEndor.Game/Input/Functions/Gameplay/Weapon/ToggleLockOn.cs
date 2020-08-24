using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.Primitives.Extensions;

namespace SWEndor.Game.Input.Functions.Gameplay.Weapon
{
  public class ToggleLockOn : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_SPACE;
    public static string InternalName = "g_lock";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      ActorInfo a = engine.PlayerInfo.TargetActor;
      if (a != null)
      {
        engine.PlayerInfo.LockTarget = !engine.PlayerInfo.LockTarget;
        engine.Screen2D.MessageSecondaryText(engine.PlayerInfo.LockTarget ? TextLocalization.Get(TextLocalKeys.TARGET_LOCKED).F(a.Name) : TextLocalization.Get(TextLocalKeys.TARGET_UNLOCKED)
                                                   , 2.5f
                                                   , ColorLocalization.Get(ColorLocalKeys.GAME_MESSAGE_NORMAL));
      }
    }
  }
}
