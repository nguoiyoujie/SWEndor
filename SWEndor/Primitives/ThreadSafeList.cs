using System.Collections.Generic;
using System.Threading;

namespace SWEndor
{
  /// <summary>
  /// Provides a basic thread-safe list interface for multithreaded updates  
  /// </summary>
  /// <typeparam name="T">The item type to be stored in this list</typeparam>
  public class ThreadSafeList<T>
  {
    private Mutex mu_pending_list = new Mutex();
    private List<T> _list = new List<T>();
    private List<T> _pending_list = new List<T>();
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

    public bool Contains(T item)
    {
      Update();
      return _list.Contains(item);
    }

    public int IndexOf(T item)
    {
      Update();
      return _list.IndexOf(item);
    }

    public T this[int id]
    {
      get
      {
        Update();
        if (id >= 0 && id < _list.Count)
          return _list[id];
        return default(T);
      }
      set
      {
        mu_pending_list.WaitOne();
        _pending_list[id] = value;
        mu_pending_list.ReleaseMutex();
        if (!ExplicitUpdateOnly)
          _dirty = true;
      }
    }

    /// <summary>
    /// Obtains last updated collection
    /// </summary>
    /// <returns></returns>
    public T[] GetList()
    {
      Update();
      T[] ret = _list.ToArray();
      return ret;
    }

    private void Update()
    {
      if (_dirty)
      {
        mu_pending_list.WaitOne();
        _list = _pending_list;
        _pending_list = new List<T>(_pending_list);
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
    public void AddItem(T item)
    {
      mu_pending_list.WaitOne();
      _pending_list.Add(item);
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Adds an item to the collection only if this item is not already in the collection
    /// </summary>
    public bool AddUniqueItem(T item)
    {
      if (!Contains(item))
      {
        AddItem(item);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Clears the collection
    /// </summary>
    public void ClearList()
    {
      mu_pending_list.WaitOne();
      _pending_list = new List<T>();
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Removes the first instance of an item from the collection
    /// </summary>
    public bool RemoveItem(T item)
    {
      mu_pending_list.WaitOne();
      bool ret = _pending_list.Remove(item);
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;

      return ret;
    }

    /// <summary>
    /// Removes all instances of an item from the collection
    /// </summary>
    public bool RemoveAllItem(T item)
    {
      bool ret = false;
      while (RemoveItem(item))
      {
        ret = true;
      }
      return ret;
    }

    /// <summary>
    /// Sorts the list using a comparer
    /// </summary>
    public void Sort(IComparer<T> comparer)
    {
      mu_pending_list.WaitOne();
      _pending_list.Sort(comparer);
      mu_pending_list.ReleaseMutex();
      if (!ExplicitUpdateOnly)
        _dirty = true;
    }
  }
}
