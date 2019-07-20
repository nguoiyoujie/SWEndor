using System;

namespace SWEndor.Primitives.Traits
{
  public struct TraitPair<A,T> : IEquatable<TraitPair<A,T>>
    where A : ITraitOwner
    where T : ITrait
  {
    public readonly A Owner;
    public readonly T Trait;

    public TraitPair(A owner, T trait) { Owner = owner; Trait = trait; }

    public static bool operator ==(TraitPair<A, T> me, TraitPair<A,T> other) { return Equals(me.Owner, other.Owner) && Equals(me.Trait, other.Trait); }
    public static bool operator !=(TraitPair<A, T> me, TraitPair<A, T> other) { return !(me == other); }

    public override int GetHashCode() { return Owner.GetHashCode() ^ Trait.GetHashCode(); }

    public bool Equals(TraitPair<A,T> other) { return this == other; }
    public override bool Equals(object obj) { return obj is TraitPair<A,T> && Equals((TraitPair<A,T>)obj); }

    public override string ToString() { return Owner.Name + "->" + Trait.GetType().Name; }
  }
}
