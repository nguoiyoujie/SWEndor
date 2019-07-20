using System;

namespace SWEndor.Primitives
{
  public struct Pair<U, V> : IEquatable<Pair<U, V>>
  {
    public readonly U u;
    public readonly V v;

    public Pair(U u, V v) { this.u = u; this.v = v; }

    public static bool operator ==(Pair<U, V> me, Pair<U, V> other) { return Equals(me.u, other.u) && Equals(me.v, other.v); }
    public static bool operator !=(Pair<U, V> me, Pair<U, V> other) { return !(me == other); }

    public override int GetHashCode() { return u.GetHashCode() ^ v.GetHashCode(); }

    public bool Equals(Pair<U, V> other) { return this == other; }
    public override bool Equals(object obj) { return obj is Pair<U, V> && Equals((Pair<U, V>)obj); }
  }
}
