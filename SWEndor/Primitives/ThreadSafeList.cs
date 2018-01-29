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

    public void Update()
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
    private void SetDirty()
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
    /// Removes an item from the collection
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
  }
}
