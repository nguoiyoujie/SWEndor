using SWEndor.Core;

namespace SWEndor.Primitives.Extensions
{
  public static class FloatExts
  {

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
