using System;
using Primrose.Primitives.Extensions;

namespace Primrose.Primitives.Factories
{
  /// <summary>
  /// Provides a basic object pool for pooling objects for further use  
  /// </summary>
  public interface IPool
  {
    /// <summary>Retrieves the number of elements in the pool</summary>
    int Count { get; }

    /// <summary>Retrieves the number of generated elements in the pool that were not yet collected</summary>
    int UncollectedCount { get; }
  }

  public partial class ObjectPool<T> : IPool where T : class
  {
    private static ObjectPool<T> _staticpool;

    /// <summary>
    /// Creates a static version of the object pool for global use
    /// </summary>
    /// <param name="createFn">The function for creating new instances</param>
    /// <param name="resetFn">The function for reseting instances that are returned to the pool</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">The background object pool has already been created.</exception>
    public static ObjectPool<T> CreateStaticPool(Func<T> createFn, Action<T> resetFn = null)
    {
      if (_staticpool != null)
        throw new InvalidOperationException("There is already a background object pool for '{0}'".F(typeof(T).Name));

      _staticpool = new ObjectPool<T>(createFn, resetFn);
      return _staticpool;
    }

    /// <summary>
    /// Retrieves the static version of the pool if created
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">The background object pool has not been created.</exception>
    public static ObjectPool<T> GetStaticPool()
    {
      if (_staticpool == null)
        throw new InvalidOperationException("The background object pool for '{0}' has not been created.".F(typeof(T).Name));

      return _staticpool;
    }
  }
}
