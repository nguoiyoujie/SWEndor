using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay.Special
{
  public class AllEnemiesDying : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_5;
    public static string InternalName = "d_enemy_dying";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      ActorInfo p = engine.PlayerInfo.Actor;
      if (p != null)
      {
        foreach (ActorInfo actor in engine.ActorFactory.GetAll())
        {
          if (actor != null
            && !actor.IsDyingOrDead
            && actor.Faction != FactionInfo.Neutral
            && !actor.IsAlliedWith(p))
            actor.SetState_Dying();
        }
      }
    }
  }
}
