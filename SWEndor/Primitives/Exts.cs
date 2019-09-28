using SWEndor.Core;
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
    public static string[] Split(this string s, char c)
    {
      return new List<string>(s.InnerSplit(c)).ToArray();
    }

    public static IEnumerable<string> InnerSplit(this string s, char c)
    {
      int index = 0;
      for (int i = 0; i < s.Length; i++)
      {
        if (s[i] == c)
        {
          yield return s.Substring(index, i - index);
          index = i + 1;
        }
      }

      yield return s.Substring(index, s.Length - index);
    }

    public static string F<T1>(this string fmt, T1 o1) { return string.Format(fmt, o1); }
    public static string F<T1, T2>(this string fmt, T1 o1, T2 o2) { return string.Format(fmt, o1, o2); }
    public static string F<T1, T2, T3>(this string fmt, T1 o1, T2 o2, T3 o3) { return string.Format(fmt, o1, o2, o3); }

    // avoiding params
    public static string C(this string s1, string s2) { return string.Concat(s1, s2); }
    public static string C(this string s1, string s2, string s3) { return string.Concat(s1, s2, s3); }
    public static string C(this string s1, string s2, string s3, string s4) { return string.Concat(s1, s2, s3, s4); }
    public static string C(this string s1, string s2, string s3, string s4, string s5) { return s1.C(s2, s3, s4).C(s5); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6) { return s1.C(s2, s3, s4).C(s5, s6); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7) { return s1.C(s2, s3, s4).C(s5, s6, s7); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8, s9); }
    public static string C(this string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8, string s9, string s10) { return s1.C(s2, s3, s4).C(s5, s6, s7).C(s8, s9, s10); }

    public static string F(this string fmt, params object[] args) { return string.Format(fmt, args); }
  }
}
