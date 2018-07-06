using System.Collections.Generic;
using System.Threading;

namespace SWEndor
{
  /// <summary>
  /// Provides a basic thread-safe paired list interface for multithreaded updates  
  /// </summary>
  /// <typeparam name="T">The item type to be stored as keys in this list</typeparam>
  /// <typeparam name="U">The item type to be stored as values in this list</typeparam>
  public class ThreadSafeDictionary<T, U>
  {
    private Mutex mu_pending_list = new Mutex();
    private Dictionary<T, U> _list = new Dictionary<T, U>();
    private Dictionary<T, U> _pending_list = new Dictionary<T, U>();
    private bool _dirty = true;
    public bool ExplicitUpdateOnly = false;

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
      get
      {
        Update();
        if (_list.ContainsKey(key))
          return _list[key];
        return default(U);
      }
      set
      {
        mu_pending_list.WaitOne();
        _pending_list[key] = value;
        mu_pending_list.ReleaseMutex();
        if (!ExplicitUpdateOnly)
          _dirty = true;
      }
    }

    /// <summary>
    /// Obtains last updated collection
    /// </summary>
    /// <returns></returns>
    public Dictionary<T, U> GetList()
    {
      Update();
      Dictionary<T, U> ret = _list;
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
    public U GetItem(T key)
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
        mu_pending_list.WaitOne();
        _list = _pending_list;
        _pending_list = new Dictionary<T, U>(_pending_list);
        mu_pending_list.ReleaseMutex();
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
    /// Adds an item to the collection
    /// </summary>
    public void AddItem(T key, U value)
    {
      mu_pending_list.WaitOne();
      _pending_list.Add(key, value);
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Adds or Updates an item to the collection
    /// </summary>
    public void AddorUpdateItem(T key, U value)
    {
      mu_pending_list.WaitOne();
      if (_pending_list.ContainsKey(key))
        _pending_list[key] = value;
      else
        _pending_list.Add(key, value);
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Clears the collection
    /// </summary>
    public void ClearList()
    {
      mu_pending_list.WaitOne();
      _pending_list = new Dictionary<T, U>();
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Removes an item from the collection
    /// </summary>
    public bool RemoveItem(T key)
    {
      bool ret = false;
      mu_pending_list.WaitOne();
      if (_pending_list.ContainsKey(key))
        ret = _pending_list.Remove(key);
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;

      return ret;
    }
  }
}
