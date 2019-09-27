using SWEndor.ActorTypes;
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

    public void Init(ActorTypeInfo atype)
    {
      NoRegen = atype.RegenData.NoRegen;
      SelfRegenRate = atype.RegenData.SelfRegenRate;
      ParentRegenRate = atype.RegenData.ParentRegenRate;
      ChildRegenRate = atype.RegenData.ChildRegenRate;
      SiblingRegenRate = atype.RegenData.SiblingRegenRate;
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
