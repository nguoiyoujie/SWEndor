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

      if (value > max)
        value = max;
      else if (value < min)
        value = min;

      return value;
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
