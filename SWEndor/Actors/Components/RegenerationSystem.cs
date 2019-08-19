using SWEndor.Actors.Data;

namespace SWEndor.Actors.Components
{
  public static class RegenerationSystem
  {
    public static void Process(Engine engine, ActorInfo actor, float time)
    {
      RegenData data = engine.ActorDataSet.RegenData[actor.dataID];

      // Regen
      if (data.SelfRegenRate != 0)
        Regenerate(engine, actor, data.SelfRegenRate * time);

      if (data.ParentRegenRate != 0)
          Regenerate(engine, actor.Relation.Parent, data.ParentRegenRate * time);

      if (data.ChildRegenRate != 0)
        foreach (ActorInfo c in actor.Relation.Children)
          Regenerate(engine, c, data.ChildRegenRate * time);

      if (data.SiblingRegenRate != 0)
        foreach (ActorInfo r in actor.Relation.Siblings)
          Regenerate(engine, r, data.SiblingRegenRate * time);
    }

    private static void Regenerate(Engine engine, ActorInfo a, float amount)
    {
      if (!engine.ActorDataSet.RegenData[a.dataID].NoRegen && !a.IsDyingOrDead)
        CombatSystem.onNotify(engine, a, CombatEventType.RECOVER, amount);
    }
  }
}
