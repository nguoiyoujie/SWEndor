using Primrose.Primitives.Extensions;
using System;

namespace Primrose.Primitives.Collections
{
  /// <summary>
  /// Provides a queue based on circular linkage
  /// </summary>
  /// <typeparam name="T">The item type stored in the queue</typeparam>
  public class CircularQueue<T>
  {
    private readonly T[] nodes;
    private int current;
    private int emptySpot;
    private readonly bool errifexceed;

    /// <summary>
    /// Creates a queue.
    /// </summary>
    /// <param name="size">The size of the queue</param>
    /// <param name="errifexceed">Whether an exception is thrown if the queue is full</param>
    public CircularQueue(int size, bool errifexceed)
    {
      this.nodes = new T[size];
      this.current = 0;
      this.emptySpot = 0;
      this.errifexceed = errifexceed;
    }

    /// <summary>Retrieves the number of elements in the queue</summary>
    public int Count
    {
      get
      {
        int ret = emptySpot - current;
        return ret < 0 ? (ret + nodes.Length) : ret;
      }
    }

    /// <summary>Enqueues an item from the queue</summary>
    /// <param name="value">The item to be enqueued</param>
    /// <exception cref="InvalidOperationException">Attempted to enqueue an item into the queue that has reached capacity limit.</exception>
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

    /// <summary>Dequeues an item from the queue</summary>
    /// <returns></returns>
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
