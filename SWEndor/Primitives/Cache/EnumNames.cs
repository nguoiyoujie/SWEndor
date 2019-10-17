using SWEndor.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace SWEndor.Primitives
{
  public static class EnumNames<T> where T : struct
  {
    private static Dictionary<T, string> _dict;

    static EnumNames()
    {
      Array a = Enum.GetValues(typeof(T));
      _dict = new Dictionary<T, string>(a.Length);
      for (int i = 0; i < a.Length; i++)
      {
        object o = a.GetValue(i);
        _dict.Add((T)o, o.ToString());
      }
    }

    public static string Get(T key) { string s = null; _dict.TryGetValue(key, out s); return s; }
  }

  public static class EnumNamesExt
  {
    public static string GetEnumName<T>(this T key) where T : struct
    {
      if (!typeof(T).IsEnum)
        throw new InvalidOperationException("Attempted to use GetEnumName on an non Enum object! ({0})".F(typeof(T)));
      return EnumNames<T>.Get(key);
    }
  }
}
