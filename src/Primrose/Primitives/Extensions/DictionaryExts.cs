using System;
using System.Collections.Generic;

namespace Primrose.Primitives.Extensions
{
  public static class DictionaryExts
  {
    public static void Put<K, V>(this Dictionary<K, V> d, K k, V v)
    {
      if (d.ContainsKey(k))
        d[k] = v;
      else
        d.Add(k, v);
    }

    public static V GetOrDefault<K, V>(this Dictionary<K, V> d, K k)
    {
      V ret;
      if (!d.TryGetValue(k, out ret))
        return default(V);
      return ret;
    }

    public static V GetOrAdd<K, V>(this Dictionary<K, V> d, K k)
      where V : new()
    {
      V ret;
      if (!d.TryGetValue(k, out ret))
        d.Add(k, ret = new V());
      return ret;
    }

    public static V GetOrAdd<K, V>(this Dictionary<K, V> d, K k, Func<K, V> createFn)
    {
      V ret;
      if (!d.TryGetValue(k, out ret))
        d.Add(k, ret = createFn(k));
      return ret;
    }
  }
}
