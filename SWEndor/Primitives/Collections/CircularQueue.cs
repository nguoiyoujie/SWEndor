using System;

namespace SWEndor.Primitives.Collections
{
  public class CircularQueue<T>
  {
    readonly T[] nodes;
    int current;
    int emptySpot;
    readonly bool errifexceed;

    public CircularQueue(int size, bool errifexceed)
    {
      this.nodes = new T[size];
      this.current = 0;
      this.emptySpot = 0;
      this.errifexceed = errifexceed;
    }

    public int Count
    {
      get
      {
        int ret = emptySpot - current;
        return ret < 0 ? (ret + nodes.Length) : ret;
      }
    }

    public void Enqueue(T value)
    {
      if (Count == nodes.Length - 1)
      {
        if (errifexceed)
          throw new InvalidOperationException("Attempted to enqueue an item into {0} that has reached capacity limit of {1}.".F(GetType().Name, nodes.Length));
        else
          Dequeue();
      }
      nodes[emptySpot] = value;
      emptySpot++;
      if (emptySpot >= nodes.Length)
        emptySpot = 0;
    }

    public T Dequeue()
    {
      int ret = current;
      current++;
      if (current >= nodes.Length)
        current = 0;
      return nodes[ret];
    }
  }
}
