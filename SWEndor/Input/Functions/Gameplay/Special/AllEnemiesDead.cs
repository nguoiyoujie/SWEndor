using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;

namespace SWEndor.Input.Functions.Gameplay.Special
{
  public class AllEnemiesDead : InputFunction
  {
    private int _key = (int)CONST_TV_KEY.TV_KEY_6;
    public static string InternalName = "d_enemy_dead";
    public override int Key { get { return _key; } set { _key = value; } }
    public override string Name { get { return InternalName; } }
    public override InputOptions Options { get { return InputOptions.ONPRESS; } }

    public override void Process(Engine engine)
    {
      // Broken. TO-DO: Fix
      foreach (int actorID in engine.GameScenarioManager.Scenario.MainEnemyFaction.GetActors(Models.TargetType.ANY, true))
      {
        ActorInfo actor = engine.ActorFactory.Get(actorID);
        if (actor != null && !actor.IsDead)
          actor.SetState_Dead();
      }
    }
  }
}
