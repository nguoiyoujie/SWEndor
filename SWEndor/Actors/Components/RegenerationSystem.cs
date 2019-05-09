using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public static class RegenerationSystem
  {
    public static void Process(Engine engine, int id, float time)
    {
      ActorInfo actor = engine.ActorFactory.Get(id);
      RegenData data = engine.ActorDataSet.RegenData[engine.ActorFactory.GetIndex(id)];

      // Regen
      if (data.SelfRegenRate != 0)
        Regenerate(engine, id, data.SelfRegenRate * time);

      if (data.ParentRegenRate != 0)
          Regenerate(engine, actor.ParentID, data.ParentRegenRate * time);

      if (data.ChildRegenRate != 0)
        foreach (int c in actor.Children)
          Regenerate(engine, c, data.ChildRegenRate * time);

      if (data.SiblingRegenRate != 0)
        foreach (int r in actor.Siblings)
          Regenerate(engine, r, data.SiblingRegenRate * time);
    }

    private static void Regenerate(Engine engine, int i, float amount)
    {
      ActorInfo a = engine.ActorFactory.Get(i);
      if (!engine.ActorDataSet.RegenData[engine.ActorFactory.GetIndex(i)].NoRegen && !a.ActorState.IsDyingOrDead())
        CombatSystem.onNotify(engine, i, CombatEventType.RECOVER, amount);
    }
  }
}
