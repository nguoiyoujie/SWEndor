namespace SWEndor.Actors.Components
{
  public class CombatSystem
  {
    internal static void Deactivate(Engine engine, ActorInfo actor)
    {
      engine.ActorDataSet.CombatData[actor.dataID].IsCombatObject = false;
    }
  }
}
