using SWEndor.Primitives.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SWEndor.Primitives.Factories
{
  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Registry<T> : IRegistry<T>
  {
    public Registry() { list = new Dictionary<string, T>(); }
    public Registry(int capacity) { list = new Dictionary<string, T>(capacity); }

    protected Dictionary<string, T> list;
    public static T Default = default(T);
    private object locker = new object();

    public bool Contains(string id) { lock (locker) return list.ContainsKey(id); }
    public T Get(string id) { lock (locker) return (list.ContainsKey(id)) ? list[id] : Default; }
    public T GetX(string id) { lock (locker) return list[id]; }
    public T[] GetAll() { lock (locker) return list.Values.ToArray(); }
    public virtual void Add(string id, T item) { lock (locker) list.Add(id, item); }
    public virtual void Put(string id, T item) { lock (locker) list.Put(id, item); }
    public virtual void Remove(string id) { lock (locker) list.Remove(id); }
    public virtual void Clear() { lock (locker) list.Clear(); }

    public int Count { get { lock (locker) return list.Count; } }
  }

  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="K"></typeparam>
  /// <typeparam name="T"></typeparam>
  public class Registry<K, T> : IRegistry<K, T>
  {
    public Registry() { list = new Dictionary<K, T>(); }
    public Registry(int capacity) { list = new Dictionary<K, T>(capacity); }

    protected Dictionary<K, T> list;
    public static T Default = default(T);
    private object locker = new object();

    public bool Contains(K key) { lock (locker) return list.ContainsKey(key); }
    public T Get(K key) { lock (locker) return (list.ContainsKey(key)) ? list[key] : Default; }
    public T GetX(K key) { lock (locker) return list[key]; }
    public T[] GetAll() { lock (locker) return list.Values.ToArray(); }
    public virtual void Add(K key, T item) { lock (locker) list.Add(key, item); }
    public virtual void Put(K key, T item) { lock (locker) list.Put(key, item); }
    public virtual void Remove(K key) { lock (locker) list.Remove(key); }
    public virtual void Clear() { lock (locker) list.Clear(); }

    public int Count { get { lock (locker) return list.Count; } }
  }
}
