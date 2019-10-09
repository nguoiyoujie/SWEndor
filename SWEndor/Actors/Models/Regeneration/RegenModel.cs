using SWEndor.ActorTypes.Components;
using SWEndor.Primitives;

namespace SWEndor.Actors.Models
{
  public struct RegenModel
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
      // Regen
      if (SelfRegenRate != 0)
        Regenerate(a, SelfRegenRate * time);

      if (ParentRegenRate != 0)
        Regenerate(a.Parent, ParentRegenRate * time);

      if (ChildRegenRate != 0)
        foreach (ActorInfo c in a.Children)
          using (ScopeCounterManager.Acquire(c.Scope))
            Regenerate(c, ChildRegenRate * time);

      if (SiblingRegenRate != 0)
        foreach (ActorInfo r in a.Siblings)
          using (ScopeCounterManager.Acquire(r.Scope))
            Regenerate(r, SiblingRegenRate * time);
    }

    private void Regenerate(ActorInfo a, float amount)
    {
      if (!a.NoRegen && !a.IsDyingOrDead)
        a.InflictDamage(a, -amount, DamageType.ALWAYS_100PERCENT);
    }
  }
}

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public void Regenerate(float time)
    {
      using (ScopeCounterManager.Acquire(Scope))
        Regen.Process(this, time);
    }

    public bool NoRegen { get { return Regen.NoRegen; } }
  }
}
