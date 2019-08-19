using System.Collections.Generic;

namespace SWEndor.Primitives.Factories
{
  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Registry<T> : IRegistry<T>
  {
    protected Dictionary<string, T> list = new Dictionary<string, T>();
    public static T Default = default(T);

    public T Get(string id) { return (list.ContainsKey(id)) ? list[id] : Default; }
    public IEnumerable<T> GetAll() { return list.Values; }
    public virtual void Add(string id, T item) { list.Add(id, item); }
    public virtual void Put(string id, T item) { list.Put(id, item); }
    public virtual void Remove(string id) { list.Remove(id); }
    public virtual void Clear() { list.Clear(); }
  }

  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="K"></typeparam>
  /// <typeparam name="T"></typeparam>
  public class Registry<K, T> : IRegistry<K, T>
  {
    protected Dictionary<K, T> list = new Dictionary<K, T>();
    public static T Default = default(T);

    public bool Contains(K key) { return list.ContainsKey(key); }
    public T Get(K key) { return (list.ContainsKey(key)) ? list[key] : Default; }
    public IEnumerable<T> GetAll() { return list.Values; }
    public virtual void Add(K key, T item) { list.Add(key, item); }
    public virtual void Put(K key, T item) { list.Put(key, item); }
    public virtual void Remove(K key) { list.Remove(key); }
    public virtual void Clear() { list.Clear(); }
  }
}
