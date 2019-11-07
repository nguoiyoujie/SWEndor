using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A float3 triple
  /// </summary>
  public struct float3
  {
    public float x;
    public float y;
    public float z;

    public float3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

    // indexing
    public float this[int i]
    {
      get
      {
        if (i < 0 || i > 2)
          throw new IndexOutOfRangeException("Attempted to access invalid index '{0}' of float3".F(i));
        return (i == 0) ? x : (i == 1) ? y : z;
      }
    }

    public float[] ToArray() { return new float[] { x, y, z }; }
    public static float3 FromArray(float[] array)
    {
      if (array == null)
        throw new NullReferenceException("array");
      else if (array.Length != 3)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float3".F(array.Length));
      else
        return new float3(array[0], array[1], array[2]);
    }

    public static float3 FromArray(int[] array)
    {
      if (array == null)
        throw new NullReferenceException("array");
      else if (array.Length != 3)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float3".F(array.Length));
      else
        return new float3(array[0], array[1], array[2]);
    }

    // operations
    public static float3 operator +(float3 a, float3 b)
    {
      return new float3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static float3 operator -(float3 a, float3 b)
    {
      return new float3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static float3 operator *(float3 a, float3 b)
    {
      return new float3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static float3 operator /(float3 a, float3 b)
    {
      return new float3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static float3 operator *(float3 a, float m)
    {
      return new float3(a.x * m, a.y * m, a.z * m);
    }

    public static float3 operator /(float3 a, float m)
    {
      return new float3(a.x / m, a.y / m, a.z / m);
    }
  }
}
