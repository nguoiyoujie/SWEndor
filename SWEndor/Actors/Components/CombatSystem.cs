namespace SWEndor.Actors.Components
{
  public class CombatSystem
  {
    internal static void Deactivate(Engine engine, ActorInfo actor)
    {
      actor.CombatData.IsCombatObject = false;
    }
  }
}
