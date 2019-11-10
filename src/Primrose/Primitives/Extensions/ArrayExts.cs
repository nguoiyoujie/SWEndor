using System;

namespace Primrose.Primitives.Extensions
{
  /// <summary>
  /// Provides extension methods for arrays
  /// </summary>
  public static class ArrayExts
  {
    /// <summary>Retrives a random object from an array of objects</summary>
    /// <typeparam name="T">The member type</typeparam>
    /// <param name="array">The array</param>
    /// <param name="rand">The random object</param>
    /// <returns>A random object from the array. If the array has no members, return the default value of the member type</returns>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> and <paramref name="rand"/> cannot be null</exception>
    public static T Random<T>(this T[] array, Random rand)
    {
      if (array == null) throw new ArgumentNullException("array");
      if (rand == null) throw new ArgumentNullException("rand");
      return array.Length == 0 ? default(T) : array[rand.Next(0, array.Length)];
    }
  }
}
