using System.Collections;
using System.Collections.Generic;

namespace Primrose.Primitives
{
  /// <summary>
  /// Provides a basic thread-safe list interface for multithreaded updates  
  /// </summary>
  /// <typeparam name="T">The item type to be stored in this list</typeparam>
  public class ThreadSafeList<T> : IList<T>
  {
    private object locker = new object();
    //private ObjectPool<List<T>> pool = new ObjectPool<List<T>>(() => new List<T>(), (t) => { t.Clear(); });

    //private Mutex mu_pending_list = new Mutex();
    private List<T> _list = new List<T>();
    private List<T> _pending_list = new List<T>();
    private bool _dirty = true;

    /// <summary>
    /// Defines whether updates should be triggered explicitly. If true, call SetDirty() to update.
    /// </summary>
    public bool ExplicitUpdateOnly = false;

    /// <summary>
    /// Creates a thread-safe list
    /// </summary>
    /// <param name="items">The initial list source</param>
    public ThreadSafeList(IEnumerable<T> items = null) { if (items != null) _pending_list = new List<T>(items); }

    /// <summary>
    /// The number of elements in the list
    /// </summary>
    public int Count
    {
      get
      {
        Update();
        return _list.Count;
      }
    }

    /// <summary>
    /// Returns whether the list is read-only
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return ((IList<T>)_list).IsReadOnly;
      }
    }

    /// <summary>
    /// Returns whether the item exists in the dictionary
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>True if the item exists, false otherwise</returns>
    public bool Contains(T item)
    {
      Update();
      return _list.Contains(item);
    }

    /// <summary>
    /// Returns the index of an item in the dictionary
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns>The zero-based index of the item, or -1 if the item is not found</returns>
    public int IndexOf(T item)
    {
      Update();
      return _list.IndexOf(item);
    }

    /// <summary>
    /// Retrieves a value using an index
    /// </summary>
    /// <param name="id">The index to check</param>
    /// <returns>Returns the value associated with this index</returns>
    public T this[int id]
    {
      get { return Get(id); }
      set { Set(id, value); }
    }

    /// <summary>
    /// Obtains last updated collection
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> GetList()
    {
      Update();
      return _list;
      //T[] ret = _list.ToArray();
      //return ret;
    }

    private void Update()
    {
      if (_dirty)
      {
        _list = new List<T>(_pending_list);
        //List<T> temp = _list;
        //_list = pool.GetNew();
        //pool.Return(temp);
        //foreach (T t in _pending_list)
        //  _list.Add(t);
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
      lock (locker)
        _pending_list.Insert(index, item);

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
      lock (locker)
        _pending_list[index] = item;

      if (!ExplicitUpdateOnly)
        _dirty = true;
    }

    /// <summary>
    /// Clears the collection
    /// </summary>
    public void Clear()
    {
      lock (locker)
      {
        _pending_list = new List<T>();
        //List<T> temp = _pending_list;
        //_pending_list = pool.GetNew();
        //pool.Return(temp);
      }

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
    /// Removes a range of items from the collection
    /// </summary>
    public void RemoveRange(int index, int count)
    {
      lock (locker)
        _pending_list.RemoveRange(index, count);

      if (!ExplicitUpdateOnly)
        _dirty = true;
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

    /// <summary>
    /// Copies the elements of the list to an array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      ((IList<T>)_list).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
      return ((IList<T>)_list).GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the list.
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IList<T>)_list).GetEnumerator();
    }
  }
}
