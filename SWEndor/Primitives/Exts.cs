using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SWEndor.Primitives
{
  public static class ListExts
  {
    public static int BinarySearchMany<T>(this List<T> list, int searchFor) where T : IIdentity
    {
      int start = 0;
      int end = list.Count;
      while (start != end)
      {
        int mid = (start + end) / 2;
        // debug
        if (list[mid] == null)
        { }

        if (list[mid].ID < searchFor)
          start = mid + 1;
        else
          end = mid;
      }
      return start;
    }

    public static T Random<T>(this List<T> list, Engine engine)
    {
      return list[engine.Random.Next(0, list.Count)];
    }

    public static T Random<T>(this ConcurrentBag<T> list, Engine engine)
    {
      return list.ToArray()[engine.Random.Next(0, list.Count)];
    }

    public static int RemoveAll<T>(this LinkedList<T> list, Predicate<T> match)
    {
      if (list == null) throw new ArgumentNullException("list");
      if (match == null) throw new ArgumentNullException("match");
      
      int count = 0;
      LinkedListNode<T> node = list.First;
      LinkedListNode<T> next;
      while (node != null)
      {
        next = node.Next;
        if (match(node.Value))
        {
          list.Remove(node);
          count++;
        }
        node = next;
      }
      return count;
    }

    public static T Random<T>(this LinkedList<T> list, Engine engine)
    {
      if (list == null) throw new ArgumentNullException("list");

      int count = engine.Random.Next(0, list.Count);
      LinkedListNode<T> node = list.First;
      while (count > 0)
      {
        node = node.Next;
        count--;
      }
      return node.Value;
    }
  }

  public static class ArrayExts
  {
    public static T Random<T>(this T[] list, Engine engine)
    {
      return list.Length == 0 ? default(T) : list[engine.Random.Next(0, list.Length)];
    }
  }

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
      return d.GetOrAdd(k, _ => new V());
    }

    public static V GetOrAdd<K, V>(this Dictionary<K, V> d, K k, Func<K, V> createFn)
    {
      V ret;
      if (!d.TryGetValue(k, out ret))
        d.Add(k, ret = createFn(k));
      return ret;
    }
  }

  public static class TypeExts
  {
    public static IEnumerable<Type> BaseTypes(this Type t)
    {
      while (t != null)
      {
        yield return t;
        t = t.BaseType;
      }
    }
  }

  public static class StringExts
  {
    public static string F(this string fmt, params object[] args)
    {
      return string.Format(fmt, args);
    }
  }
}
