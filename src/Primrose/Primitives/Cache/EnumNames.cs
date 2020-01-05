using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace Primrose.Primitives
{
  /// <summary>
  /// Provides a cache for string names for enumerables. Improves runtime performance of name-based lookups.
  /// </summary>
  /// <typeparam name="T">The enumerable type</typeparam>
  public static class EnumNames<T>
  {
    private static Dictionary<T, string> _dict;

    static EnumNames()
    {
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException("Attempted to use GetEnumName on an non Enum object! ({0})".F(typeof(T)));

      Array a = Enum.GetValues(typeof(T));
      _dict = new Dictionary<T, string>(a.Length);
      for (int i = 0; i < a.Length; i++)
      {
        object o = a.GetValue(i);
        if (!_dict.ContainsKey((T)o))
          _dict.Add((T)o, o.ToString());
      }
    }

    /// <summary>
    /// Retrieves the name of an string enum value from the cached list.
    /// </summary>
    /// <param name="key">The enum value</param>
    /// <returns>The cached ToString() result</returns>
    public static string Get(T key) { string s = null; _dict.TryGetValue(key, out s); return s; }
  }

  /// <summary>
  /// Provides extension methods for enumerable value types
  /// </summary>
  public static class EnumNamesExt
  {
    /// <summary>
    /// Retrieves the name of an string enum value from a cached list.
    /// </summary>
    /// <typeparam name="T">The enumerable type</typeparam>
    /// <param name="key">The enum value</param>
    /// <returns></returns>
    public static string GetEnumName<T>(this T key)
    {
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException("Attempted to use GetEnumName on an non Enum object! ({0})".F(typeof(T)));
      return EnumNames<T>.Get(key);
    }
  }
}
