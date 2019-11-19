using System;
using System.Collections.Concurrent;

namespace Primrose.Primitives.Factories
{
  /// <summary>
  /// Provides a basic object pool for pooling objects for further use  
  /// </summary>
  /// <typeparam name="T">The item type to be pooled</typeparam>
  public partial class ObjectPool<T> : IPool where T : class
  {
    /// <summary>
    /// Creates an object pool
    /// </summary>
    /// <param name="createFn">The function for creating new instances</param>
    /// <param name="resetFn">The function for reseting instances that are returned to the pool</param>
    /// <exception cref="ArgumentNullException">createFn cannot be null</exception>
    public ObjectPool(Func<T> createFn, Action<T> resetFn = null)
    {
      if (createFn == null)
        throw new ArgumentNullException("createFn");

      creator = createFn;
      resetor = resetFn;
    }

    private ConcurrentQueue<T> list = new ConcurrentQueue<T>();
    private Func<T> creator;
    private Action<T> resetor;

    /// <summary>Retrieves the number of elements in the pool</summary>
    public int Count { get { return list.Count; } }

    /// <summary>Retrieves the number of generated elements in the pool that were not yet collected</summary>
    public int UncollectedCount { get; private set; }

    /// <summary>Returns an instance of <typeparamref name="T"/> from the pool, or creates a new instance if the pool is empty</summary>
    /// <returns>An instance of <typeparamref name="T"/> from the pool, or created from the creator function if the pool is empty</returns>
    public T GetNew()
    {
      T ret;
      UncollectedCount++;
      if (list.TryDequeue(out ret))
        return ret;
      return creator();
    }

    /// <summary>Returns an instance of <typeparamref name="T"/> to the pool.</summary>
    /// <param name="item">The object to be returnedto the pool</param>
    public void Return(T item)
    {
      resetor?.Invoke(item);
      list.Enqueue(item);
      UncollectedCount--;
    }

    /// <summary>Removes all instances from the pool</summary>
    public void Clear()
    {
      list = new ConcurrentQueue<T>();
    }
  }
}
