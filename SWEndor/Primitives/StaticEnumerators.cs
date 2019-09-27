using System.Collections.Generic;

namespace SWEndor.Primitives
{
  public struct LinkedListEnumerable<T>
  {
    readonly LinkedList<T> L;
    public LinkedListEnumerable(LinkedList<T> l) { L = l; }
    public LinkedListEnumerator<T> GetEnumerator() { return new LinkedListEnumerator<T>(L); }
  }

  public struct LinkedListEnumerator<T>
  {
    readonly LinkedList<T> L;
    LinkedListNode<T> current;
    public LinkedListEnumerator(LinkedList<T> l) { L = l; current = null; }
    public bool MoveNext() { return (current = (current == null) ? L.First : current?.Next) != null; }
    public T Current { get { return (current == null) ? default(T) : current.Value; } }
  }

}
