using System;

namespace Primrose.Primitives.Factories
{
  /// <summary>
  /// Allows creation of objects and stores them automatically. Limited to objects with parameterless constructors; for others, use Registry
  /// </summary>
  /// <typeparam name="K">The key type to be stored</typeparam>
  /// <typeparam name="T">The object type to be stored</typeparam>
  public interface IFactory<K, T> : IRegistry<K, T>
  {
    /// <summary>
    /// Creates a new object and stores its reference in its internal registry
    /// </summary>
    /// <param name="id">The identifier for the object</param>
    /// <returns></returns>
    T Create(K id);
  }

  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="T">The type of the registered object</typeparam>
  public interface IRegistry<T>
  {
    /// <summary>Retrieves the value associated with a key</summary>
    /// <param name="key">The identifier key to check</param>
    /// <returns>The value associated with the key. If the registry does not contain this key, returns Default</returns>
    T Get(string key);

    /// <summary>Adds an object into the registry</summary>
    /// <param name="id">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    void Add(string id, T item);

    /// <summary>Updates or adds an object into the registry</summary>
    /// <param name="id">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    void Put(string id, T item);

    /// <summary>Removes an object from the registry</summary>
    /// <param name="id">The identifier key to remove</param>
    void Remove(string id);

    /// <summary>Purges all data from the registry</summary>
    void Clear();
  }

  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="K">The type of the key</typeparam>
  /// <typeparam name="T">The type of the registered object</typeparam>
  public interface IRegistry<K, T>
  {
    /// <summary>Retrieves the value associated with a key</summary>
    /// <param name="key">The identifier key to check</param>
    /// <returns>The value associated with the key. If the registry does not contain this key, returns Default</returns>
    T Get(K key);

    /// <summary>Adds an object into the registry</summary>
    /// <param name="key">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    void Add(K key, T item);

    /// <summary>Updates or adds an object into the registry</summary>
    /// <param name="key">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    void Put(K key, T item);

    /// <summary>Removes an object from the registry</summary>
    /// <param name="key">The identifier key to remove</param>
    void Remove(K key);

    /// <summary>Purges all data from the registry</summary>
    void Clear();
  }

  /// <summary>
  /// Defines a Factory object>
  /// </summary>
  /// <typeparam name="K">The type of the associated key</typeparam>
  public abstract class AFactoryObject<K>
  {
    private K id;
    /// <summary>The unique identifier of the object</summary>
    public K ID
    {
      get { return id; }
      set
      {
        if (id == null)
          id = value;
        else
          throw new InvalidOperationException("Setting ID to AFactoryObject with existing ID '" + id + "' is not allowed!");
      }
    }

    /// <summary>Creates an instance of the object</summary>
    public AFactoryObject() { }
  }
}
