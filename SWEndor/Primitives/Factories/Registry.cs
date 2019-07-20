using System.Collections.Generic;

namespace SWEndor.Primitives.Factories
{
  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Registry<T> : IRegistry<T>
  {
    protected static ThreadSafeDictionary<string, T> list = new ThreadSafeDictionary<string, T>();
    public string DefaultKey = null;

    public T Get(string id) { if (DefaultKey == null || list.ContainsKey(id)) { return list.Get(id); } else { return list.Get(DefaultKey); } }
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
    protected ThreadSafeDictionary<K, T> list = new ThreadSafeDictionary<K, T>();
    public K DefaultKey = default(K);

    public bool Contains(K key) { return list.ContainsKey(key); }
    public T Get(K key) { if (DefaultKey == null || list.ContainsKey(key)) { return list.Get(key); } else { return list.Get(DefaultKey); } }
    public IEnumerable<T> GetAll() { return list.Values; }
    public virtual void Add(K key, T item) { list.Add(key, item); }
    public virtual void Put(K key, T item) { list.Put(key, item); }
    public virtual void Remove(K key) { list.Remove(key); }
    public virtual void Clear() { list.Clear(); }
  }

  /*
  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class IntRegistry<T> //: IRegistry<int, T>
  {
    protected ThreadSafeList<T> list = new ThreadSafeList<T>();
    protected ThreadSafeList<int> empty = new ThreadSafeList<int>();

    public bool Contains(int id) { return list.Count < id && !empty.Contains(id); }
    public int NonEmptyCount { get { return list.Count - empty.Count; } }
    public T Get(int id) { return list.Get(id); }
    public T[] GetAll() { return list.GetList(); }
    public T[] GetAllNonEmpty()
    {
      T[] ret = new T[NonEmptyCount];
      int n = -1;
      for (int i = 0; i < list.Count; i++)
        if (!empty.Contains(i))
          ret[++n] = Get(i);

      return ret;
    }

    public virtual int Add(T item)
    {
      if (empty.Count == 0)
      {
        list.Add(item);
        return list.Count - 1;
      }

      int i = empty[0];
      list[i] = item;
      empty.Remove(i);
      return i;
    }

    public virtual void Set(int id, T item) { list.Set(id, item); empty.Remove(id); }
    public virtual void Remove(int id) { if (empty.AddUnique(id)) list[id] = default(T); }
    public virtual void Clear() { list.Clear(); empty.Clear(); }
  }
  */
}
