using Primrose.Primitives.Extensions;
using System;

namespace SWEndor.Game
{
  public static class LookUp
  {
    private static float[] _cos = new float[36000];
    private static float[] _sin = new float[36000];

    public enum Measure { DEGREES, RADIANS }

    static LookUp()
    {
      for (int i = 0; i < 36000; i++)
      {
        float a = i / 100.0f;
        _cos[i] = (float)Math.Cos(a * Globals.Deg2Rad);
        _sin[i] = (float)Math.Sin(a * Globals.Deg2Rad);
      }
    }

    public static float Cos(float angle, Measure measure = Measure.RADIANS)
    {
      if (measure == Measure.RADIANS)
      {
        angle *= Globals.Rad2Deg;
      }
      int n = ((int)(angle * 100)).Modulus(0, 36000);
      return _cos[n];
    }

    public static float Sin(float angle, Measure measure = Measure.RADIANS)
    {
      if (measure == Measure.RADIANS)
      {
        angle *= Globals.Rad2Deg;
      }
      int n = ((int)(angle * 100)).Modulus(0, 36000);
      return _sin[n];
    }
  }
}