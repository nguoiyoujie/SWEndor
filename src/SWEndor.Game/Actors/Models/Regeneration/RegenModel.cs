using SWEndor.Game.ActorTypes.Components;
using Primrose.Primitives;

namespace SWEndor.Game.Actors.Models
{
  internal struct RegenModel
  {
    /// <summary>
    /// This actor does not regenerate shields at any point
    /// </summary>
    public bool NoRegen;

    /// <summary>
    /// The rate (per second) of regeneration of its own shields
    /// </summary>
    public float SelfRegenRate;

    /// <summary>
    /// The rate (per second) of regeneration of its own shields
    /// </summary>
    public float ParentRegenRate;

    /// <summary>
    /// The rate (per second) of regeneration of its own shields
    /// </summary>
    public float ChildRegenRate;

    /// <summary>
    /// The rate (per second) of regeneration of its own shields
    /// </summary>
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
      if (!a.IsSystemOperational(SystemPart.SHIELD_GENERATOR))
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
      if (a != null && !a.NoRegen && !a.IsDyingOrDead)
        a.InflictDamage(-amount, DamageType.ALWAYS_100PERCENT);
    }
  }
}
