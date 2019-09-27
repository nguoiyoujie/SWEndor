using MTV3D65;
using SWEndor.Actors;
using SWEndor.Primitives;

namespace SWEndor.Input.Functions.Gameplay.Weapon
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
        Globals.Engine.Screen2D.MessageSecondaryText(engine.PlayerInfo.LockTarget ? "Target Locked to {0}".F(a.Name) : "Target Unlocked", 2.5f, new TV_COLOR(0.5f, 0.5f, 1, 1));
      }
    }
  }
}
