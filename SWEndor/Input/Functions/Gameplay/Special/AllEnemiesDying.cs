using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Traits;

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
      if (engine.GameScenarioManager.Scenario != null)
      {
        foreach (ActorInfo actor in engine.GameScenarioManager.Scenario.MainEnemyFaction.GetAll())
        {
          if (actor != null)
            actor.StateModel.MakeDying(actor);
        }
      }
    }
  }
}
