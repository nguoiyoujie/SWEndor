using SWEndor.Core;

namespace SWEndor.Primitives.Extensions
{
  public static class ArrayExts
  {
    public static T Random<T>(this T[] list, Engine engine)
    {
      return list.Length == 0 ? default(T) : list[engine.Random.Next(0, list.Length)];
    }
  }
}
