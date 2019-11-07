using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A float4 quad
  /// </summary>
  public struct float4
  {
    public float x;
    public float y;
    public float z;
    public float w;

    public float4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }

    // indexing
    public float this[int i]
    {
      get
      {
        if (i < 0 || i > 3)
          throw new IndexOutOfRangeException("Attempted to access invalid index '{0}' of float4".F(i));
        return (i == 0) ? x : (i == 1) ? y : (i == 2) ? z : w;
      }
    }

    public float[] ToArray() { return new float[] { x, y, z, w }; }
    public static float4 FromArray(float[] array)
    {
      if (array == null)
        throw new NullReferenceException("array");
      else if (array.Length != 4)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float4".F(array.Length));
      else
        return new float4(array[0], array[1], array[2], array[3]);
    }

    public static float4 FromArray(int[] array)
    {
      if (array == null)
        throw new NullReferenceException("array");
      else if (array.Length != 4)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float4".F(array.Length));
      else
        return new float4(array[0], array[1], array[2], array[3]);
    }

    // operations
    public static float4 operator +(float4 a, float4 b)
    {
      return new float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public static float4 operator -(float4 a, float4 b)
    {
      return new float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    public static float4 operator *(float4 a, float4 b)
    {
      return new float4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
    }

    public static float4 operator /(float4 a, float4 b)
    {
      return new float4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
    }

    public static float4 operator *(float4 a, float m)
    {
      return new float4(a.x * m, a.y * m, a.z * m, a.w * m);
    }

    public static float4 operator /(float4 a, float m)
    {
      return new float4(a.x / m, a.y / m, a.z / m, a.w / m);
    }
  }
}
