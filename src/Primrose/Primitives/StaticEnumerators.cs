using System.Collections.Generic;

namespace Primrose.Primitives
{
  /// <summary>Represents an enumeration of items in a linked list</summary>
  /// <typeparam name="T"></typeparam>
  public struct LinkedListEnumerable<T>
  {
    readonly LinkedList<T> L;
    /// <summary>Creates the enumerable</summary>
    public LinkedListEnumerable(LinkedList<T> l) { L = l; }

    /// <summary>Gets the enumerator</summary>
    public LinkedListEnumerator<T> GetEnumerator() { return new LinkedListEnumerator<T>(L); }

    /// <summary>The enumerator for an empty list</summary>
    public static LinkedListEnumerable<T> Empty = new LinkedListEnumerable<T>(null);
  }

  /// <summary>Represents an enumerator of items in a linked list</summary>
  /// <typeparam name="T"></typeparam>
  public struct LinkedListEnumerator<T>
  {
    readonly LinkedList<T> L;
    LinkedListNode<T> current;
    /// <summary>Creates an enumerator</summary>
    public LinkedListEnumerator(LinkedList<T> l) { L = l; current = null; }

    /// <summary>Retrieves the next item</summary>
    public bool MoveNext() { return (current = (current == null) ? L?.First : current?.Next) != null; }

    /// <summary>Retrieves the current item</summary>
    public T Current { get { return (current == null) ? default(T) : current.Value; } }
  }

}
