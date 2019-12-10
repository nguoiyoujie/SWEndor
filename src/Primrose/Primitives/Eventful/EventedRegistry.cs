using Primrose.Primitives.Factories;

namespace Primrose.Primitives.Eventful
{
  /// <summary>
  /// A delegate representing a change in value
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="newValue">Represents the new value</param>
  /// <param name="oldValue">Represents the old value</param>
  public delegate void ChangeEventDelegate<T>(T newValue, T oldValue);

  internal struct ChangeEvent<T>
  {
    private event ChangeEventDelegate<T> _ev;

    internal event ChangeEventDelegate<T> Ev
    {
      add { _ev += value; }
      remove { _ev -= value; }
    }

    internal void Invoke(T newV, T oldV)
    {
      if ((newV == null && oldV != null) || !(newV != null && newV.Equals(oldV)))
        _ev?.Invoke(newV, oldV);
    }
  }

  /// <summary>
  /// A wrapper for binding modification events to a registry
  /// </summary>
  /// <typeparam name="K">The encapsulated key type</typeparam>
  /// <typeparam name="V">The encapsulated value type</typeparam>
  public class EventedRegistry<K, V> : IRegistry<K, V>
  {
    private Registry<K, V> _reg;

    private ChangeEvent<Registry<K, V>> _regChanged = new ChangeEvent<Registry<K, V>>();
    private ChangeEvent<K> _keyAdded = new ChangeEvent<K>();
    private ChangeEvent<K> _keyRemoved = new ChangeEvent<K>();
    private ChangeEvent<V> _valueChanged = new ChangeEvent<V>();

    /// <summary>Represents the set of functions to be called when the registry is replaced</summary>
    public event ChangeEventDelegate<Registry<K, V>> RegistryChanged { add { _regChanged.Ev += value; } remove { _regChanged.Ev -= value; } }

    /// <summary>Represents the set of functions to be called when a key is added</summary>
    public event ChangeEventDelegate<K> KeyAdded { add { _keyAdded.Ev += value; } remove { _keyAdded.Ev -= value; } }

    /// <summary>Represents the set of functions to be called when a key is removed</summary>
    public event ChangeEventDelegate<K> KeyRemoved { add { _keyRemoved.Ev += value; } remove { _keyRemoved.Ev -= value; } }

    /// <summary>Represents the set of functions to be called when a value is changed</summary>
    public event ChangeEventDelegate<V> ValueChanged { add { _valueChanged.Ev += value; } remove { _valueChanged.Ev -= value; } }

    /// <summary>
    /// Defines an event-bounded registry
    /// </summary>
    /// <param name="reg">The initial registry</param>
    public EventedRegistry(Registry<K, V> reg)
    {
      _reg = reg;
      _regChanged = default(ChangeEvent<Registry<K, V>>);
      _keyAdded = default(ChangeEvent<K>);
      _keyRemoved = default(ChangeEvent<K>);
      _valueChanged = default(ChangeEvent<V>);
    }

    /// <summary>
    /// The encapsulated registry
    /// </summary>
    public Registry<K, V> Registry
    {
      get { return _reg; }
      set
      {
        Registry<K, V> old = _reg;
        _reg = value;
        _regChanged.Invoke(value, old);
      }
    }

    /// <summary>Retrieves the value associated with a key</summary>
    /// <param name="key">The identifier key to check</param>
    /// <returns>The value associated with the key. If the registry does not contain this key, returns Default</returns>
    public V Get(K key)
    {
      return _reg.Get(key);
    }

    /// <summary>Adds an object into the registry</summary>
    /// <param name="key">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    public void Add(K key, V item)
    {
      _reg.Add(key, item);
      _keyAdded.Invoke(key, default(K));
    }


    /// <summary>Updates or adds an object into the registry</summary>
    /// <param name="key">The identifier key to add</param>
    /// <param name="item">The object to be associated with this key</param>
    public void Put(K key, V item)
    {
      if (_reg.Contains(key))
      {
        V old = _reg[key];
        _reg[key] = item;
        _valueChanged.Invoke(item, old);
      }
      else
      {
        _reg.Add(key, item);
        _keyAdded.Invoke(key, default(K));
      }
    }

    /// <summary>Removes an object from the registry</summary>
    /// <param name="key">The identifier key to remove</param>
    public void Remove(K key)
    {
      _reg.Remove(key);
      _keyRemoved.Invoke(default(K), key);
    }

    /// <summary>Purges all data from the registry</summary>
    public void Clear()
    {
      _reg.Clear();
    }
  }
}
