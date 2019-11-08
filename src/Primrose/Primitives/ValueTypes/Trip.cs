namespace Primrose.Primitives.ValueTypes
{
  public struct Trip<T, U, V>
  {
    public T t;
    public U u;
    public V v;
    public Trip(T t, U u, V v) { this.t = t; this.u = u; this.v = v; }
  }
}
