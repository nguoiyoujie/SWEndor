namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A value pair
  /// </summary>
  /// <typeparam name="T">The type of the first value</typeparam>
  /// <typeparam name="U">The type of the second value</typeparam>
  public struct Pair<T, U>
  {
    /// <summary>The first value</summary>
    public T t;

    /// <summary>The second value</summary>
    public U u;

    /// <summary>
    /// Creates a value pair with given values
    /// </summary>
    /// <param name="t">The first value</param>
    /// <param name="u">The second value</param>
    public Pair(T t, U u) { this.t = t; this.u = u; }
  }
}
