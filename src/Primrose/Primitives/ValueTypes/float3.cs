using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A float3 triple value
  /// </summary>
  public struct float3
  {
    /// <summary>The x or [0] value</summary>
    public float x;

    /// <summary>The y or [1] value</summary>
    public float y;

    /// <summary>The z or [2] value</summary>
    public float z;

    /// <summary>
    /// Creates a float3 value
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public float3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

    /// <summary>The value indexer</summary>
    /// <exception cref="IndexOutOfRangeException">The array is accessed with an invalid index</exception>
    public float this[int i]
    {
      get
      {
        if (i < 0 || i > 2)
          throw new IndexOutOfRangeException("Attempted to access invalid index '{0}' of float3".F(i));
        return (i == 0) ? x : (i == 1) ? y : z;
      }
    }

    /// <summary>Returns the string representation of this value</summary>
    public override string ToString() { return "{{{0},{1},{2}}}".F(x, y, z); }

    /// <summary>Creates a float[] array from this value</summary>
    /// <returns>An array of length 3 with identical indexed values</returns>
    public float[] ToArray() { return new float[] { x, y, z }; }

    /// <summary>Creates a float3 from an array</summary>
    /// <param name="array">The array</param>
    /// <returns>A float3 value</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> cannot be null</exception>
    /// <exception cref="InvalidOperationException">Only an array of length 3 can be converted to a float3</exception>
    public static float3 FromArray(float[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (array.Length != 3)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float3".F(array.Length));
      else
        return new float3(array[0], array[1], array[2]);
    }

    /// <summary>Creates a float3 from an array</summary>
    /// <param name="array">The array</param>
    /// <returns>A float3 value</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> cannot be null</exception>
    /// <exception cref="InvalidOperationException">Only an array of length 3 can be converted to a float3</exception>
    public static float3 FromArray(int[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (array.Length != 3)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float3".F(array.Length));
      else
        return new float3(array[0], array[1], array[2]);
    }

    /// <summary>Performs a memberwise negation of a float3 value</summary>
    /// <param name="a"></param><returns></returns>
    public static float3 operator -(float3 a)
    {
      return new float3(-a.x, -a.y, -a.z);
    }

    /// <summary>Performs an addition operation between two float3 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float3 operator +(float3 a, float3 b)
    {
      return new float3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    /// <summary>Performs a subtraction operation between two float3 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float3 operator -(float3 a, float3 b)
    {
      return new float3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    /// <summary>Performs a memberwise multiplication operation between two float3 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float3 operator *(float3 a, float3 b)
    {
      return new float3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    /// <summary>Performs a memberwise division between two float3 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float3 operator /(float3 a, float3 b)
    {
      return new float3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    /// <summary>Performs a multiplication operation between a float3 value and a float multiplier</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float3 operator *(float3 a, float m)
    {
      return new float3(a.x * m, a.y * m, a.z * m);
    }

    /// <summary>Performs a division operation between a float3 value and a float divisor</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float3 operator /(float3 a, float m)
    {
      return new float3(a.x / m, a.y / m, a.z / m);
    }

    /// <summary>Performs a modulus operation between a float3 value and a float divisor</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float3 operator %(float3 a, float m)
    {
      return new float3(a.x % m, a.y % m, a.z % m);
    }
  }
}
