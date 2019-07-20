using SWEndor.Actors.Data;
using SWEndor.Actors.Traits;

namespace SWEndor.Actors.Components
{
  public static class RegenerationSystem
  {
    public static void Process(Engine engine, ActorInfo a, float time)
    {
      RegenData data = a.RegenData;

      // Regen
      if (data.SelfRegenRate != 0)
        Regenerate(engine, a, data.SelfRegenRate * time);

      if (data.ParentRegenRate != 0)
          Regenerate(engine, a.Relation.Parent, data.ParentRegenRate * time);

      if (data.ChildRegenRate != 0)
        foreach (ActorInfo c in a.Children)
          Regenerate(engine, c, data.ChildRegenRate * time);

      if (data.SiblingRegenRate != 0)
        foreach (ActorInfo r in a.Siblings)
          Regenerate(engine, r, data.SiblingRegenRate * time);
    }

    private static void Regenerate(Engine engine, ActorInfo a, float amount)
    {
      if (!a.RegenData.NoRegen && !a.StateModel.IsDyingOrDead)
        a.Health.InflictDamage(a, new DamageInfo<ActorInfo>(a, -amount));
    }
  }
}
