namespace Primrose.Primitives.Factories
{
  /// <summary>
  /// Allows creation of objects and stores them automatically. Limited to objects with parameterless constructors; for others, use Registry
  /// </summary>
  /// <typeparam name="K">The key type to be stored</typeparam>
  /// <typeparam name="T">The object type to be stored</typeparam>
  public class Factory<K, T> : Registry<K, T>, IFactory<K, T> where T : AFactoryObject<K>, new()
  {
    /// <summary>
    /// Creates a new object and stores its reference in its internal registry
    /// </summary>
    /// <param name="id">The identifier for the object</param>
    /// <returns></returns>
    public T Create(K id)
    {
      T ret = new T();
      ret.ID = id;
      Add(id, ret);
      return ret;
    }
  }
}
