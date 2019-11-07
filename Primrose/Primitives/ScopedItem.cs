using SWEndor.Primitives.Factories;
using System;
using System.Collections.Concurrent;

namespace SWEndor.Primitives
{
  public static class ScopedManager<T> where T : class
  {
    static ScopedManager()
    {
      pool = new ObjectPool<ScopedItem>(() => new ScopedItem(), (p) => { });// p.Dispose());
    }

    private static ObjectPool<ScopedItem> pool;
    public static int PoolCount { get { return pool.Count; } }

    public static ScopedItem Scope(T item)
    {
      if (item == null)
        return null;

      ScopedItem e = pool.GetNew();
      e.ScopeOne(item);
      return e;
    }

    public static int Check(T item)
    {
      return ScopedItem.Check(item);
    }

    public class ScopedItem : IDisposable
    {
      private static readonly ConcurrentDictionary<T, int> _counter = new ConcurrentDictionary<T, int>();
      public T Value { get; private set; }

      internal static int Check(T item)
      {
        int ret;
        _counter.TryGetValue(item, out ret);
        return ret;
      }

      internal void ScopeOne(T item)
      {
        Value = item;

        int i;
        _counter.TryGetValue(item, out i);
        i++;
        _counter[item] = i;
      }

      private void ReleaseOne()
      {
        if (Value != null)
        {
          int i;
          _counter.TryGetValue(Value, out i);
          i--;
          //if (i > 0)
            _counter[Value] = i;
          //else
          //{
            //if (!_counter.TryRemove(Value, out i))
             // _counter[Value] = 0;
          //}
          Value = null;
        }
      }

      void IDisposable.Dispose()
      {
        ReleaseOne();
        GC.SuppressFinalize(this);
      }

      //~ScopedItem()
      //{
      //  Dispose();
      //}
    }
  }
}
