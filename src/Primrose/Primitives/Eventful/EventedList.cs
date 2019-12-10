using System.Collections;
using System.Collections.Generic;

namespace Primrose.Primitives.Eventful
{
  /// <summary>
  /// A wrapper for binding modification events to a list
  /// </summary>
  /// <typeparam name="TList">The encapsulated list type</typeparam>
  /// <typeparam name="T">The encapsulated element type</typeparam>
  public class EventedList<TList, T> : IList<T>
    where TList : IList<T>
  {
    private TList _list;

    private ChangeEvent<IList<T>> _listChanged = new ChangeEvent<IList<T>>();
    private ChangeEvent<T> _itemAdded = new ChangeEvent<T>();
    private ChangeEvent<T> _itemRemoved = new ChangeEvent<T>();
    private ChangeEvent<T> _itemChanged = new ChangeEvent<T>();

    /// <summary>Represents the set of functions to be called when the registry is replaced</summary>
    public event ChangeEventDelegate<IList<T>> ListChanged { add { _listChanged.Ev += value; } remove { _listChanged.Ev -= value; } }

    /// <summary>Represents the set of functions to be called when a key is added</summary>
    public event ChangeEventDelegate<T> ItemAdded { add { _itemAdded.Ev += value; } remove { _itemAdded.Ev -= value; } }

    /// <summary>Represents the set of functions to be called when a key is removed</summary>
    public event ChangeEventDelegate<T> ItemRemoved { add { _itemRemoved.Ev += value; } remove { _itemRemoved.Ev -= value; } }

    /// <summary>Represents the set of functions to be called when a value is changed</summary>
    public event ChangeEventDelegate<T> ItemChanged { add { _itemChanged.Ev += value; } remove { _itemChanged.Ev -= value; } }


    /// <summary>Gets or sets an element in the list, accessed by an index</summary>
    public T this[int index]
    {
      get { return _list[index]; }
      set
      {
        T old = _list[index];
        _list[index] = value;
        _itemChanged.Invoke(value, old);
      }
    }

    /// <summary>Gets the number of elements contained in the list</summary>
    public int Count { get { return _list.Count; } }

    /// <summary>Gets a value indicating whether the list is read-only</summary>
    public bool IsReadOnly { get { return _list.IsReadOnly; } }

    /// <summary>
    /// The encapsulated list
    /// </summary>
    public TList List
    {
      get { return _list; }
      set
      {
        TList old = _list;
        _list = value;
        if ((_list == null && old != null) || !(_list != null && _list.Equals(old)))
          _listChanged.Invoke(value, old);
      }
    }
    
    /// <summary>Adds an item to the list</summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
      // event
      _list.Add(item);
      _itemAdded.Invoke(item, default(T));
    }

    public void Clear() { _list.Clear(); }

    public bool Contains(T item) { return _list.Contains(item); }

    public void CopyTo(T[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }

    /// <summary>Determines the index of a specific item in the list</summary>
    /// <param name="item">The object to locate in the list</param>
    /// <returns></returns>
    public int IndexOf(T item) {return _list.IndexOf(item);}

    /// <summary>Inserts an item to the list at the specified index.</summary>
    /// <param name="index">The zero-based index at which item should be inserted</param>
    /// <param name="item">The object to insert into the list</param>
    public void Insert(int index, T item)
    {
      _list.Insert(index, item);
      _itemAdded.Invoke(item, default(T));
    }

    /// <summary>Removes the first occurrence of a specific object from the list</summary>
    /// <param name="item">The object to remove from the item</param>
    /// <returns></returns>
    public bool Remove(T item)
    {
      bool ret = _list.Remove(item);
      if (ret)
        _itemRemoved.Invoke(default(T), item);

      return ret;
    }

    /// <summary>Removes the item at the specified index.</summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
    {
      T item = _list[index];
      _list.RemoveAt(index);
      _itemRemoved.Invoke(default(T), item);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _list.GetEnumerator();
    }

  }
}
