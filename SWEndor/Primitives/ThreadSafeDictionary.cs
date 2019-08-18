using System;
using System.Collections.Generic;

namespace SWEndor.Primitives
{
  /// <summary>
  /// Provides a basic thread-safe paired list interface for multithreaded updates  
  /// </summary>
  /// <typeparam name="T">The item type to be stored as keys in this list</typeparam>
  /// <typeparam name="U">The item type to be stored as values in this list</typeparam>
  public class ThreadSafeDictionary<T, U>
  {
    private object locker = new object();

    //private Mutex mu_pending_list = new Mutex();
    private Dictionary<T, U> _list = new Dictionary<T, U>();
    private Dictionary<T, U> _pending_list = new Dictionary<T, U>();
    private bool _dirty = true;

    /// <summary>
    /// Defines whether updates should be triggered explicitly. If true, call SetDirty() to update.
    /// </summary>
    public bool ExplicitUpdateOnly = false;

    public ThreadSafeDictionary(IDictionary<T, U> dict = null) { if (dict != null) _pending_list = new Dictionary<T, U>(dict); }

    public int Count
    {
      get
      {
        Update();
        return _list.Count;
      }
    }

    public bool ContainsKey(T key)
    {
      Update();
      return _list.ContainsKey(key);
    }

    public bool ContainsValue(U value)
    {
      Update();
      return _list.ContainsValue(value);
    }

    public U this[T key]
    {
      get { return Get(key); }
      set { Set(key, value); }
    }

    /// <summary>
    /// Obtains last updated collection
    /// </summary>
    /// <returns></returns>
    public Dictionary<T, U> GetList()
    {
      Update();
      Dictionary<T, U> ret = new Dictionary<T, U>(_list);
      return ret;
    }

    /// <summary>
    /// Obtains last updated keys
    /// </summary>
    /// <returns></returns>
    public T[] GetKeys()
    {
      Update();
      return new List<T>(_list.Keys).ToArray();
    }

    /// <summary>
    /// Obtains last updated values
    /// </summary>
    /// <returns></returns>
    public U[] GetValues()
    {
      Update();
      return new List<U>(_list.Values).ToArray();
    }

    /// <summary>
    /// Retrieves the value associated with this key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public U Get(T key)
    {
      Update();
      U ret = default(U);
      GetList().TryGetValue(key, out ret);
      return ret;
    }

    private void Update()
    {
      if (_dirty)
      {
        Refresh();
      }
      _dirty = false;
    }

    /// <summary>
    /// Explicity triggers the list for update
    /// </summary>
    public void SetDirty()
    {
      _dirty = true;
    }

    /// <summary>
    /// Forces a refresh
    /// </summary>
    public void Refresh()
    {
      lock (locker)
      {
        _list = new Dictionary<T, U>(_pending_list);
      }
    }

    /// <summary>
    /// Adds an item to the collection
    /// </summary>
    public void Add(T key, U value)
    {
      try
      {
        lock (locker)
          _pending_list.Add(key, value);
      }
      catch (ArgumentNullException ex)
      {
        throw new ArgumentNullException("Attempted to add a null key to a ThreadSafeDictionary", ex);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException("Attempted to add an existing key to a ThreadSafeDictionary", ex);
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Sets an item to the collection
    /// </summary>
    public void Set(T key, U value)
    {
      try
      {
        lock (locker)
          _pending_list[key] = value;
      }
      catch (ArgumentNullException ex)
      {
        throw new ArgumentNullException("Attempted to set value of a null key in a ThreadSafeDictionary", ex);
      }
      catch (KeyNotFoundException ex)
      {
        throw new ArgumentException("Attempted to set value to an non-existend key in a ThreadSafeDictionary", ex);
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Adds or Sets an item to the collection
    /// </summary>
    public void Put(T key, U value)
    {
      try
      {
        lock (locker)
          if (_pending_list.ContainsKey(key))
            _pending_list[key] = value;
          else
            _pending_list.Add(key, value);
      }
      catch (ArgumentNullException ex)
      {
        throw new ArgumentNullException("Attempted to put value of a null key in a ThreadSafeDictionary", ex);
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Clears the collection
    /// </summary>
    public void Clear()
    {
      lock (locker)
        _pending_list = new Dictionary<T, U>();

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Removes an item from the collection
    /// </summary>
    public bool Remove(T key)
    {
      bool ret = false;
      lock (locker)
        if (_pending_list.ContainsKey(key))
          ret = _pending_list.Remove(key);

      if (!ExplicitUpdateOnly)
        _dirty = true;

      return ret;
    }
  }
}
