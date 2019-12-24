using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A float4 quad value
  /// </summary>
  public struct float4
  {
    /// <summary>The x or [0] value</summary>
    public float x;

    /// <summary>The y or [1] value</summary>
    public float y;

    /// <summary>The z or [2] value</summary>
    public float z;

    /// <summary>The w or [3] value</summary>
    public float w;

    /// <summary>
    /// Creates a float4 value
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="w"></param>
    public float4(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }

    /// <summary>The value indexer</summary>
    /// <exception cref="IndexOutOfRangeException">The array is accessed with an invalid index</exception>
    public float this[int i]
    {
      get
      {
        if (i < 0 || i > 3)
          throw new IndexOutOfRangeException("Attempted to access invalid index '{0}' of float4".F(i));
        return (i == 0) ? x : (i == 1) ? y : (i == 2) ? z : w;
      }
    }

    /// <summary>Returns the string representation of this value</summary>
    public override string ToString() { return "{{{0},{1},{2},{3}}}".F(x, y, z, w); }

    /// <summary>Creates a float[] array from this value</summary>
    /// <returns>An array of length 4 with identical indexed values</returns>
    public float[] ToArray() { return new float[] { x, y, z, w }; }

    /// <summary>Creates a float4 from an array</summary>
    /// <param name="array">The array</param>
    /// <returns>A float4 value</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> cannot be null</exception>
    /// <exception cref="InvalidOperationException">Only an array of length 4 can be converted to a float4</exception>
    public static float4 FromArray(float[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (array.Length != 4)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float4".F(array.Length));
      else
        return new float4(array[0], array[1], array[2], array[3]);
    }

    /// <summary>Creates a float4 from an array</summary>
    /// <param name="array">The array</param>
    /// <returns>A float4 value</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> cannot be null</exception>
    /// <exception cref="InvalidOperationException">Only an array of length 4 can be converted to a float4</exception>
    public static float4 FromArray(int[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (array.Length != 4)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float4".F(array.Length));
      else
        return new float4(array[0], array[1], array[2], array[3]);
    }

    /// <summary>Performs a memberwise negation of a float4 value</summary>
    /// <param name="a"></param><returns></returns>
    public static float4 operator -(float4 a)
    {
      return new float4(-a.x, -a.y, -a.z, -a.w);
    }

    /// <summary>Performs an addition operation between two float4 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float4 operator +(float4 a, float4 b)
    {
      return new float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    /// <summary>Performs a subtraction operation between two float4 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float4 operator -(float4 a, float4 b)
    {
      return new float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
    }

    /// <summary>Performs a memberwise multiplication operation between two float4 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float4 operator *(float4 a, float4 b)
    {
      return new float4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
    }

    /// <summary>Performs a memberwise division between two float4 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float4 operator /(float4 a, float4 b)
    {
      return new float4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
    }

    /// <summary>Performs a multiplication operation between a float4 value and a float multiplier</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float4 operator *(float4 a, float m)
    {
      return new float4(a.x * m, a.y * m, a.z * m, a.w * m);
    }

    /// <summary>Performs a division operation between a float4 value and a float divisor</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float4 operator /(float4 a, float m)
    {
      return new float4(a.x / m, a.y / m, a.z / m, a.w / m);
    }

    /// <summary>Performs a modulus operation between a float4 value and a float divisor</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float4 operator %(float4 a, float m)
    {
      return new float4(a.x % m, a.y % m, a.z % m, a.w % m);
    }
  }
}
