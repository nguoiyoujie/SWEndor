using System;
using System.Collections.Generic;

namespace SWEndor.Primitives
{
  /// <summary>
  /// Provides a basic thread-safe list interface for multithreaded updates  
  /// </summary>
  /// <typeparam name="T">The item type to be stored in this list</typeparam>
  public class ThreadSafeList<T>
  {
    private object locker = new object();

    //private Mutex mu_pending_list = new Mutex();
    private List<T> _list = new List<T>();
    private List<T> _pending_list = new List<T>();
    private bool _dirty = true;

    /// <summary>
    /// Defines whether updates should be triggered explicitly. If true, call SetDirty() to update.
    /// </summary>
    public bool ExplicitUpdateOnly = false;

    public ThreadSafeList(IEnumerable<T> items = null) { if (items != null) _pending_list = new List<T>(items); }


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
      get { return Get(id); }
      set { Set(id, value); }
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
        //lock (locker)
        _list = new List<T>(_pending_list);
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
    /// Gets an item to the collection
    /// </summary>
    public T Get(int index)
    {
      Update();
      return _list[index];
    }

    /// <summary>
    /// Inserts an item at a specified index in the collection
    /// </summary>
    public void Insert(int index, T item)
    {
      try
      {
        lock (locker)
          _pending_list.Insert(index, item);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException("Attempted to insert in index '" + index + "' to a ThreadSafeList with " + _pending_list.Count + " items", ex);
      }

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Adds an item to the collection
    /// </summary>
    public void Add(T item)
    {
      lock (locker)
        _pending_list.Add(item);

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Adds an item to the collection only if this item is not already in the collection
    /// </summary>
    public bool AddUnique(T item)
    {
      if (!Contains(item))
      {
        Add(item);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Sets an item to the collection
    /// </summary>
    public void Set(int index, T item)
    {
      try
      {
        lock (locker)
          _pending_list[index] = item;
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentOutOfRangeException("Attempted to access index '" + index + "' to a ThreadSafeList with " + _pending_list.Count + " items", ex);
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
        _pending_list = new List<T>();

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Removes the first instance of an item from the collection
    /// </summary>
    public bool Remove(T item)
    {
      bool ret;
      lock (locker)
        ret = _pending_list.Remove(item);

      if (!ExplicitUpdateOnly)
        _dirty = true;

      return ret;
    }

    /// <summary>
    /// Removes the first instance of an item from the collection
    /// </summary>
    public void RemoveAt(int index)
    {
      lock (locker)
        _pending_list.RemoveAt(index);

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Removes all instances of an item from the collection
    /// </summary>
    public bool RemoveAllItem(T item)
    {
      bool ret = false;
      while (Remove(item))
        ret = true;
      return ret;
    }

    /// <summary>
    /// Sorts the list using a comparer
    /// </summary>
    public void Sort(IComparer<T> comparer)
    {
      lock (locker)
        _pending_list.Sort(comparer);

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }
  }
}
