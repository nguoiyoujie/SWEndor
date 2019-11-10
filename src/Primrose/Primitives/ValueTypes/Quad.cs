namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A value quad
  /// </summary>
  /// <typeparam name="T">The type of the first value</typeparam>
  /// <typeparam name="U">The type of the second value</typeparam>
  /// <typeparam name="V">The type of the third value</typeparam>
  /// <typeparam name="W">The type of the fourth value</typeparam>
  public struct Quad<T, U, V, W>
  {
    /// <summary>The first value</summary>
    public T t;

    /// <summary>The second value</summary>
    public U u;

    /// <summary>The third value</summary>
    public V v;

    /// <summary>The fourth value</summary>
    public W w;

    /// <summary>
    /// Creates a value triple with given values
    /// </summary>
    /// <param name="t">The first value</param>
    /// <param name="u">The second value</param>
    /// <param name="v">The third value</param>
    /// <param name="w">The fourth value</param>
    public Quad(T t, U u, V v, W w) { this.t = t; this.u = u; this.v = v; this.w = w; }
  }
}
