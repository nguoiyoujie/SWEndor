using SWEndor.ActorTypes;

namespace SWEndor.Actors
{
  public partial class ActorInfo
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

      public void Process(ActorInfo actor, float time)
      {
        // Regen
        if (SelfRegenRate != 0)
          Regenerate(actor, SelfRegenRate * time);

        if (ParentRegenRate != 0)
          Regenerate(actor.Parent, ParentRegenRate * time);

        if (ChildRegenRate != 0)
          foreach (ActorInfo c in actor.Children)
            Regenerate(c, ChildRegenRate * time);

        if (SiblingRegenRate != 0)
          foreach (ActorInfo r in actor.Siblings)
            Regenerate(r, SiblingRegenRate * time);
      }

      private void Regenerate(ActorInfo a, float amount)
      {
        if (!NoRegen && !a.IsDyingOrDead)
          a.InflictDamage(a, -amount, DamageType.ALWAYS_100PERCENT);
      }
    }

    public void Regenerate(float time) { Regen.Process(this, time); }
  }
}
