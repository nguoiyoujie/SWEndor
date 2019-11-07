using System;

namespace Primrose.Primitives.Factories
{
  public interface IFactory<T> : IRegistry<T>
  {
    T Create(string id);
  }

  public interface IRegistry<T>
  {
    T Get(string key);
    void Add(string id, T item);
    void Remove(string id);
    void Clear();
  }

  public interface IRegistry<K, T>
  {
    T Get(K key);
    void Add(K key, T item);
    void Remove(K key);
    void Clear();
  }

  public abstract class AFactoryObject
  {
    private string id;
    public string ID
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
    public AFactoryObject() { }
  }
}
