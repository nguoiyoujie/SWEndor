namespace Primrose.Primitives.Extensions
{
  /// <summary>
  /// Provides extension methods for float values
  /// </summary>
  public static class FloatExts
  {
    /// <summary>
    /// Returns a value clamped between a minimum and a maximum
    /// </summary>
    /// <param name="value">The input value</param>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <returns>min if the value is less than min, max is the value is more than max, value otherwise</returns>
    public static float Clamp(this float value, float min, float max)
    {
      return Max(Min(value, max), min);
    }

    /// <summary>
    /// Returns the maximum of two values
    /// </summary>
    /// <param name="value1">The first value</param>
    /// <param name="value2">The second value</param>
    /// <returns>The greater of the two values</returns>
    public static float Max(this float value1, float value2)
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
    public static float Min(this float value1, float value2)
    {
      return (value1 > value2) 
        ? value2
        : value1;
    }

    /// <summary>
    /// Returns a value at most max_delta value closer to a target
    /// </summary>
    /// <param name="value">The starting value</param>
    /// <param name="target">The target value</param>
    /// <param name="max_delta">The max_delta</param>
    /// <returns></returns>
    public static float Creep(this float value, float target, float max_delta)
    {
      return (value < target)
        ? Min(value + max_delta, target)
        : Max(value - max_delta, target);
    }

    /// <summary>
    /// Returns the result of (value % (max - min)), scaled so that lies between min and max
    /// </summary>
    /// <param name="value">The input value</param>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <returns>(value % (max - min)), scaled so that lies between min and max</returns>
    public static float Modulus(this float value, float min, float max)
    {
      if (max == min)
      {
        value = min;
        return value;
      }
      else if (max < min)
      {
        float temp = max;
        max = min;
        min = temp;
      }

      value %= max - min;

      if (value > max)
        value -= max - min;
      else if (value < min)
        value += max - min;

      return value;
    }
  }
}
