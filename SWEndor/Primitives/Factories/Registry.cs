namespace SWEndor.Primitives.Factories
{
  /// <summary>
  /// Maintains a typed registry of objects.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Registry<T> : IRegistry<T>
  {
    protected static ThreadSafeDictionary<string, T> list = new ThreadSafeDictionary<string, T>();
    public string DefaultKey = null;

    public T Get(string id) { if (DefaultKey == null || list.ContainsKey(id)) { return list.Get(id); } else { return list.Get(DefaultKey); } }
    public T[] GetAll() { return list.GetValues(); }
    public virtual void Add(string id, T item) { list.Add(id, item); }
    public virtual void Put(string id, T item) { list.Put(id, item); }
    public virtual void Remove(string id) { list.Remove(id); }
    public virtual void Clear() { list.Clear(); }
  }
}
