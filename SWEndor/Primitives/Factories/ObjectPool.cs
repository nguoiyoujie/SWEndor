using System;
using System.Collections.Concurrent;

namespace SWEndor.Primitives.Factories
{
  public class ObjectPool<T>
  {
    public ObjectPool(Func<T> createFn, Action<T> resetFn)
    {
      creator = createFn;
      resetor = resetFn;
    }

    private ConcurrentQueue<T> list = new ConcurrentQueue<T>();
    internal Func<T> creator;
    internal Action<T> resetor;

    public int Count { get { return list.Count; } }

    public T GetNew()
    {
      T ret;
      if (list.TryDequeue(out ret))
        return ret;

      return creator();
    }

    public void Return(T item)
    {
      resetor(item);
      list.Enqueue(item);
    }

    public void Clear()
    {
      list = new ConcurrentQueue<T>();
    }
  }
}
