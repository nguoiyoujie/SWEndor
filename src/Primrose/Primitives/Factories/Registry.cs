using Primrose.Primitives.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Primrose.Primitives.Factories
{
  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="T">The type of the registered object</typeparam>
  public class Registry<T> : IRegistry<T>
  { 
    private object locker = new object();

    /// <summary>Creates an object registry</summary>
    public Registry() { list = new Dictionary<string, T>(); }

    /// <summary>Creates an object registry with an initial capacity</summary>
    /// <param name="capacity">The initial capacity of the registry</param>
    public Registry(int capacity) { list = new Dictionary<string, T>(capacity); }

    /// <summary>The container data source</summary>
    protected Dictionary<string, T> list;

    /// <summary>The default value returned</summary>
    public T Default = default(T);

    /// <summary>Determines whether the registry contains a key</summary>
    /// <param name="id">The identifier key to check</param>
    /// <returns>True if the registry contains this key, False if otherwise</returns>
    public bool Contains(string id) { lock (locker) return list.ContainsKey(id); }

    /// <summary>Retrieves the value associated with a key index</summary>
    /// <param name="id">The identifier key to check</param>
    public T this[string id] { get { return Get(id); } set { Put(id, value); } }

    /// <summary>Retrieves the value associated with a key</summary>
    /// <param name="id">The identifier key to check</param>
    /// <returns>The value associated with the key. If the registry does not contain this key, returns Default</returns>
    public T Get(string id) { lock (locker) { T t; if (!list.TryGetValue(id, out t)) return Default; return t; } }

    /// <summary>Retrieves the value associated with a key</summary>
    /// <param name="id">The identifier key to check</param>
    /// <param name="defaultValue">The default value to return if the key is not found in the registry</param>
    /// <returns>The value associated with the key. If the registry does not contain this key, returns Default</returns>
    public T Get(string id, T defaultValue) { lock (locker) { T t; if (!list.TryGetValue(id, out t)) return defaultValue; return t; } }

    /// <summary>Strictly retrieves the value associated with a key</summary>
    /// <param name="id">The identifier key to check</param>
    /// <returns>The value associated with the key</returns>
    /// <exception cref="KeyNotFoundException">The registry does not contain this key.</exception>
    public T GetX(string id) { lock (locker) return list[id]; }

    /// <summary>Retrives an array of all the keys in the registry</summary>
    /// <returns></returns>
    public string[] GetKeys() { lock (locker) return list.Keys.ToArray(); }

    /// <summary>Retrives an array of all the values in the registry</summary>
    /// <returns></returns>
    public T[] GetAll() { lock (locker) return list.Values.ToArray(); }

    /// <summary>Adds an object into the registry</summary>
    /// <param name="id">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    public virtual void Add(string id, T item) { lock (locker) list.Add(id, item); }

    /// <summary>Updates or adds an object into the registry</summary>
    /// <param name="id">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    public virtual void Put(string id, T item) { lock (locker) list.Put(id, item); }

    /// <summary>Removes an object from the registry</summary>
    /// <param name="id">The identifier key to remove</param>
    public virtual void Remove(string id) { lock (locker) list.Remove(id); }

    /// <summary>Purges all data from the registry</summary>
    public virtual void Clear() { lock (locker) list.Clear(); }

    /// <summary>The number of elements in this registry</summary>
    public int Count { get { lock (locker) return list.Count; } }
  }

  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="K">The type of the key</typeparam>
  /// <typeparam name="T">The type of the registered object</typeparam>
  public class Registry<K, T> : IRegistry<K, T>
  {
    private object locker = new object();

    /// <summary>Creates an object registry</summary>
    public Registry() { list = new Dictionary<K, T>(); }

    /// <summary>Creates an object registry with an initial capacity</summary>
    /// <param name="capacity">The initial capacity of the registry</param>
    public Registry(int capacity) { list = new Dictionary<K, T>(capacity); }

    /// <summary>The container data source</summary>
    protected Dictionary<K, T> list;

    /// <summary>The default value returned</summary>
    public T Default = default(T);

    /// <summary>Determines whether the registry contains a key</summary>
    /// <param name="key">The identifier key to check</param>
    /// <returns>True if the registry contains this key, False if otherwise</returns>
    public bool Contains(K key) { lock (locker) return list.ContainsKey(key); }

    /// <summary>Retrieves the value associated with a key index</summary>
    /// <param name="id">The identifier key to check</param>
    public T this[K id] { get { return Get(id); } set { Put(id, value); } }

    /// <summary>Retrieves the value associated with a key</summary>
    /// <param name="key">The identifier key to check</param>
    /// <returns>The value associated with the key. If the registry does not contain this key, returns Default</returns>
    public T Get(K key) { lock (locker) { T t; if (!list.TryGetValue(key, out t)) return Default; return t; } }

    /// <summary>Strictly retrieves the value associated with a key</summary>
    /// <param name="key">The identifier key to check</param>
    /// <returns>The value associated with the key</returns>
    /// <exception cref="KeyNotFoundException">The registry does not contain this key.</exception>
    public T GetX(K key) { lock (locker) return list[key]; }

    /// <summary>Retrives an array of all the keys in the registry</summary>
    /// <returns></returns>
    public K[] GetKeys() { lock (locker) return list.Keys.ToArray(); }

    /// <summary>Retrives an array of all the values in the registry</summary>
    /// <returns></returns>
    public T[] GetAll() { lock (locker) return list.Values.ToArray(); }

    /// <summary>Adds an object into the registry</summary>
    /// <param name="key">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    public virtual void Add(K key, T item) { lock (locker) list.Add(key, item); }

    /// <summary>Updates or adds an object into the registry</summary>
    /// <param name="key">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    public virtual void Put(K key, T item) { lock (locker) list.Put(key, item); }

    /// <summary>Removes an object from the registry</summary>
    /// <param name="key">The identifier key to remove</param>
    public virtual void Remove(K key) { lock (locker) list.Remove(key); }

    /// <summary>Purges all data from the registry</summary>
    public virtual void Clear() { lock (locker) list.Clear(); }

    /// <summary>The number of elements in this registry</summary>
    public int Count { get { lock (locker) return list.Count; } }
  }
}
