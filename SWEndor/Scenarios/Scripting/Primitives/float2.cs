using MTV3D65;
using SWEndor.Primitives.Extensions;
using System;

namespace SWEndor.Scenarios.Scripting.Expressions.Primitives
{
  /// <summary>
  /// A float2 pair
  /// </summary>
  public struct float2
  {
    public float x;
    public float y;

    public float2(float x, float y) { this.x = x; this.y = y; }

    // indexing
    public float this[int i]
    {
      get
      {
        if (i < 0 || i > 1)
          throw new IndexOutOfRangeException("Attempted to access invalid index '{0}' of float2".F(i));
        return (i == 0) ? x : y;
      }
    }

    public float[] ToArray() { return new float[] { x, y }; }
    public TV_2DVECTOR ToVec2() { return new TV_2DVECTOR(x, y); }
    public static float2 FromArray(float[] array)
    {
      if (array == null)
        throw new NullReferenceException("array");
      else if (array.Length != 2)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float2".F(array.Length));
      else
        return new float2(array[0], array[1]);
    }

    public static float2 FromArray(int[] array)
    {
      if (array == null)
        throw new NullReferenceException("array");
      else if (array.Length != 2)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float2".F(array.Length));
      else
        return new float2(array[0], array[1]);
    }


    // operations
    public static float2 operator +(float2 a, float2 b)
    {
      return new float2(a.x + b.x, a.y + b.y);
    }

    public static float2 operator -(float2 a, float2 b)
    {
      return new float2(a.x - b.x, a.y - b.y);
    }

    public static float2 operator *(float2 a, float2 b)
    {
      return new float2(a.x * b.x, a.y * b.y);
    }

    public static float2 operator /(float2 a, float2 b)
    {
      return new float2(a.x / b.x, a.y / b.y);
    }

    public static float2 operator *(float2 a, float m)
    {
      return new float2(a.x * m, a.y * m);
    }

    public static float2 operator /(float2 a, float m)
    {
      return new float2(a.x / m, a.y / m);
    }
  }
}
