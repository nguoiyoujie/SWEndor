using SWEndor.Game.ActorTypes.Components;
using Primrose.Primitives;

namespace SWEndor.Game.Actors.Models
{
  internal struct RegenModel
  {
    public bool NoRegen;
    public float SelfRegenRate;
    public float ParentRegenRate;
    public float ChildRegenRate;
    public float SiblingRegenRate;

    public void Reset()
    {
      NoRegen = false;
      SelfRegenRate = 0;
      ParentRegenRate = 0;
      ChildRegenRate = 0;
      SiblingRegenRate = 0;
    }

    public void Init(ref RegenData data)
    {
      NoRegen = data.NoRegen;
      SelfRegenRate = data.SelfRegenRate;
      ParentRegenRate = data.ParentRegenRate;
      ChildRegenRate = data.ChildRegenRate;
      SiblingRegenRate = data.SiblingRegenRate;
    }

    public void Process(ActorInfo a, float time)
    {
      if (a.TypeInfo.SystemData.AllowSystemDamage && a.GetStatus(SystemPart.SHIELD_GENERATOR) != SystemState.ACTIVE)
        return;

      if (SelfRegenRate != 0)
        Regenerate(a, SelfRegenRate * time);

      if (ParentRegenRate != 0)
        Regenerate(a.Parent, ParentRegenRate * time);

      if (ChildRegenRate != 0)
        foreach (ActorInfo c in a.Children)
          using (ScopeCounters.Acquire(c.Scope))
            Regenerate(c, ChildRegenRate * time);

      if (SiblingRegenRate != 0)
        foreach (ActorInfo r in a.Siblings)
          using (ScopeCounters.Acquire(r.Scope))
            Regenerate(r, SiblingRegenRate * time);
    }

    private void Regenerate(ActorInfo a, float amount)
    {
      if (!a.NoRegen && !a.IsDyingOrDead)
        a.InflictDamage(-amount, DamageType.ALWAYS_100PERCENT);
    }
  }
}
