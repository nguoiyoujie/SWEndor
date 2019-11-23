namespace Primrose.Primitives.Extensions
{
  /// <summary>
  /// Provides extension methods for int values
  /// </summary>
  public static class IntExts
  {
    /// <summary>
    /// Returns a value clamped between a minimum and a maximum
    /// </summary>
    /// <param name="value">The input value</param>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <returns>min if the value is less than min, max is the value is more than max, value otherwise</returns>
    public static int Clamp(this int value, int min, int max)
    {
      return Max(Min(value, max), min);
    }

    /// <summary>
    /// Returns the maximum of two values
    /// </summary>
    /// <param name="value1">The first value</param>
    /// <param name="value2">The second value</param>
    /// <returns>The greater of the two values</returns>
    public static int Max(this int value1, int value2)
    {
      return (value1 > value2)
        ? value1 
        : value2;
    }

    /// <summary>
    /// Returns the minimum of two values
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns>The smaller of the two values</returns>
    public static int Min(this int value1, int value2)
    {
      return (value1 > value2) 
        ? value2
        : value1;
    }
  }
}
