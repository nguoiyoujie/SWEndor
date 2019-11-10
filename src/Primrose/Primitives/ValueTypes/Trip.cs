namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A value triple
  /// </summary>
  /// <typeparam name="T">The type of the first value</typeparam>
  /// <typeparam name="U">The type of the second value</typeparam>
  /// <typeparam name="V">The type of the third value</typeparam>
  public struct Trip<T, U, V>
  {
    /// <summary>The first value</summary>
    public T t;

    /// <summary>The second value</summary>
    public U u;

    /// <summary>The third value</summary>
    public V v;

    /// <summary>
    /// Creates a value triple with given values
    /// </summary>
    /// <param name="t">The first value</param>
    /// <param name="u">The second value</param>
    /// <param name="v">The third value</param>
    public Trip(T t, U u, V v) { this.t = t; this.u = u; this.v = v; }
  }
}
