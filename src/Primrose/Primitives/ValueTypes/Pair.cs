﻿namespace Primrose.Primitives.ValueTypes
{
  public struct Pair<T, U>
  {
    public T t;
    public U u;
    public Pair(T t, U u) { this.t = t; this.u = u; }
  }
}
