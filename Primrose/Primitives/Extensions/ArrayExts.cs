using System;

namespace SWEndor.Primitives.Extensions
{
  public static class ArrayExts
  {
    public static T Random<T>(this T[] list, Random rand)
    {
      return list.Length == 0 ? default(T) : list[rand.Next(0, list.Length)];
    }
  }
}
