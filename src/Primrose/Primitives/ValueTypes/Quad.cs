namespace Primrose.Primitives.ValueTypes
{
  public struct Quad<T, U, V, W>
  {
    public T t;
    public U u;
    public V v;
    public W w;
    public Quad(T t, U u, V v, W w) { this.t = t; this.u = u; this.v = v; this.w = w; }
  }
}
