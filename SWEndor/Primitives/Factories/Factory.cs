namespace SWEndor.Primitives.Factories
{
  /// <summary>
  /// Allows creation of objects and stores them automatically. Limited to objects with parameterless constructors; for others, use Registry
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Factory<T> : Registry<T>, IFactory<T> where T : AFactoryObject, new()
  {
    public T Create(string id)
    {
      T ret = new T();
      ret.ID = id;
      Add(id, ret);
      return ret;
    }
  }
}
