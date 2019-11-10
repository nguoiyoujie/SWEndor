using Primrose.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace Primrose.Primitives
{
  /// <summary>
  /// Provides a basic thread-safe paired dictionary for multithreaded updates  
  /// </summary>
  /// <typeparam name="T">The item type to be stored as keys in this dictionary</typeparam>
  /// <typeparam name="U">The item type to be stored as values in this dictionary</typeparam>
  public class ThreadSafeDictionary<T, U>
  {
    private object write_locker = new object();
    private Dictionary<T, U> _list = new Dictionary<T, U>();
    private Dictionary<T, U> _pending_list = new Dictionary<T, U>();
    private bool _dirty = true;

    /// <summary>
    /// Defines whether updates should be triggered explicitly. If true, call SetDirty() to update.
    /// </summary>
    public bool ExplicitUpdateOnly = false;

    /// <summary>
    /// Creates a thread-safe dictionary
    /// </summary>
    /// <param name="dict">The initial dictionary source</param>
    public ThreadSafeDictionary(IDictionary<T, U> dict = null) { if (dict != null) _pending_list = new Dictionary<T, U>(dict); }

    /// <summary>Retrieves the number of elements in the dictionary</summary>
    public int Count
    {
      get
      {
        Update();
        return _list.Count;
      }
    }

    /// <summary>Returns whether the key exists in the dictionary</summary>
    /// <param name="key">The key to check</param>
    /// <returns>True if the key exists, false otherwise</returns>
    public bool ContainsKey(T key)
    {
      Update();
      return _list.ContainsKey(key);
    }

    /// <summary>Returns whether the value exists in the dictionary</summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value exists, false otherwise</returns>
    public bool ContainsValue(U value)
    {
      Update();
      return _list.ContainsValue(value);
    }

    /// <summary>Retrieves a value using a key</summary>
    /// <param name="key">The key to check</param>
    /// <returns>Returns the value associated with this key</returns>
    public U this[T key]
    {
      get { return Get(key); }
      set { Set(key, value); }
    }

    /// <summary>Obtains last updated collection</summary>
    /// <returns></returns>
    public IDictionary<T, U> GetList()
    {
      Update();
      return new Dictionary<T, U>(_list);
    }

    /// <summary>Obtains last updated keys</summary>
    /// <returns></returns>
    public IEnumerable<T> Keys
    {
      get
      {
        Update();
        return _list.Keys;
      }
    }

    /// <summary>Obtains last updated values</summary>
    /// <returns></returns>
    public IEnumerable<U> Values
    {
      get
      {
        Update();
        return _list.Values;
      }
    }

    /// <summary>Retrieves the value associated with this key</summary>
    /// <param name="key">The key to check</param>
    /// <returns></returns>
    public U Get(T key)
    {
      Update();
      U ret = default(U);
      _list.TryGetValue(key, out ret);
      return ret;
    }

    private void Update()
    {
      if (_dirty)
      {
        Refresh();
        _dirty = false;
      }
    }

    /// <summary>Explicity triggers the list for update</summary>
    public void SetDirty()
    {
      _dirty = true;
    }

    /// <summary>Forces a refresh</summary>
    public void Refresh()
    {
      lock (write_locker)
      {
        _list = new Dictionary<T, U>(_pending_list);
      }
    }

    /// <summary>Adds an item to the collection</summary>
    public void Add(T key, U value)
    {
      try
      {
        lock (write_locker)
          _pending_list.Add(key, value);
      }
      catch (ArgumentNullException ex)
      {
        throw new ArgumentNullException("Attempted to add a null key to a {0}".F(GetType().Name), ex);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException("Attempted to add an existing key '{0}' to a {1}".F(key, GetType().Name), ex);
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>Sets an item to the collection</summary>
    public void Set(T key, U value)
    {
      try
      {
        lock (write_locker)
          _pending_list[key] = value;
      }
      catch (ArgumentNullException ex)
      {
        throw new ArgumentNullException("Attempted to set value of a null key to a {0}".F(GetType().Name), ex);
      }
      catch (KeyNotFoundException ex)
      {
        throw new ArgumentException("Attempted to set value to an non-existent key '{0}' in a {1}".F(key, GetType().Name), ex);
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>Adds or Sets an item to the collection</summary>
    public void Put(T key, U value)
    {
      try
      {
        lock (write_locker)
          if (_pending_list.ContainsKey(key))
            _pending_list[key] = value;
          else
            _pending_list.Add(key, value);
      }
      catch (ArgumentNullException ex)
      {
        throw new ArgumentNullException("Attempted to put value of a null key in a {0}".F(GetType().Name), ex);
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>Clears the collection</summary>
    public void Clear()
    {
      lock (write_locker)
      {
        _pending_list.Clear();
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>Removes an item from the collection</summary>
    public bool Remove(T key)
    {
      bool ret = false;
      lock (write_locker)
        if (_pending_list.ContainsKey(key))
          ret = _pending_list.Remove(key);

      if (!ExplicitUpdateOnly)
        _dirty = true;

      return ret;
    }
  }
}
