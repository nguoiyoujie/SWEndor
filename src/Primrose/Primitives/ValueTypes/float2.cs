using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Primitives.ValueTypes
{
  /// <summary>
  /// A float2 pair value
  /// </summary>
  public struct float2
  {
    /// <summary>The x or [0] value</summary>
    public float x;

    /// <summary>The y or [1] value</summary>
    public float y;

    /// <summary>
    /// Creates a float2 value
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public float2(float x, float y) { this.x = x; this.y = y; }

    /// <summary>The value indexer</summary>
    /// <exception cref="IndexOutOfRangeException">The array is accessed with an invalid index</exception>
    public float this[int i]
    {
      get
      {
        if (i < 0 || i > 1)
          throw new IndexOutOfRangeException("Attempted to access invalid index '{0}' of float2".F(i));
        return (i == 0) ? x : y;
      }
    }

    /// <summary>Returns the string representation of this value</summary>
    public override string ToString() { return "{{{0},{1}}}".F(x, y); }

    /// <summary>Creates a float[] array from this value</summary>
    /// <returns>An array of length 3 with identical indexed values</returns>
    public float[] ToArray() { return new float[] { x, y }; }

    /// <summary>Creates a float2 from an array</summary>
    /// <param name="array">The array</param>
    /// <returns>A float2 value</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> cannot be null</exception>
    /// <exception cref="InvalidOperationException">Only an array of length 2 can be converted to a float2</exception>
    public static float2 FromArray(float[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (array.Length != 2)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float2".F(array.Length));
      else
        return new float2(array[0], array[1]);
    }

    /// <summary>Creates a float2 from an array</summary>
    /// <param name="array">The array</param>
    /// <returns>A float2 value</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> cannot be null</exception>
    /// <exception cref="InvalidOperationException">Only an array of length 2 can be converted to a float2</exception>
    public static float2 FromArray(int[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (array.Length != 2)
        throw new InvalidOperationException("Attempted assignment of an array of length {0} to a float2".F(array.Length));
      else
        return new float2(array[0], array[1]);
    }

    /// <summary>Performs a memberwise negation of a float2 value</summary>
    /// <param name="a"></param><returns></returns>
    public static float2 operator -(float2 a)
    {
      return new float2(-a.x, -a.y);
    }

    /// <summary>Performs an addition operation between two float2 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float2 operator +(float2 a, float2 b)
    {
      return new float2(a.x + b.x, a.y + b.y);
    }

    /// <summary>Performs a subtraction operation between two float2 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float2 operator -(float2 a, float2 b)
    {
      return new float2(a.x - b.x, a.y - b.y);
    }

    /// <summary>Performs a memberwise multiplication operation between two float2 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float2 operator *(float2 a, float2 b)
    {
      return new float2(a.x * b.x, a.y * b.y);
    }

    /// <summary>Performs a memberwise division between two float2 values</summary>
    /// <param name="a"></param><param name="b"></param><returns></returns>
    public static float2 operator /(float2 a, float2 b)
    {
      return new float2(a.x / b.x, a.y / b.y);
    }

    /// <summary>Performs a multiplication operation between a float2 value and a float multiplier</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float2 operator *(float2 a, float m)
    {
      return new float2(a.x * m, a.y * m);
    }

    /// <summary>Performs a division operation between a float2 value and a float divisor</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float2 operator /(float2 a, float m)
    {
      return new float2(a.x / m, a.y / m);
    }

    /// <summary>Performs a modulus operation between a float2 value and a float divisor</summary>
    /// <param name="a"></param><param name="m"></param><returns></returns>
    public static float2 operator %(float2 a, float m)
    {
      return new float2(a.x % m, a.y % m);
    }
  }
}
