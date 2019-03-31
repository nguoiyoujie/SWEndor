using System;

namespace SWEndor.Actors
{
  public class StateElement
  {
    public readonly string Key;
    public readonly Type Type;
    public object Value;

    public StateElement(string key, object value)
    {
      Key = key;
      Value = value;
      Type = value.GetType();
    }
  }

  public class States
  {
    private ThreadSafeDictionary<string, StateElement> _list = new ThreadSafeDictionary<string, StateElement>();

    public Type getType(string key)
    {
      StateElement e = _list.Get(key);
      if (e != null)
        return e.Type;
      else
        return typeof(string);
    }

    public object getValue(string key)
    {
      StateElement e = _list.Get(key);
      if (e != null)
        return e.Value;
      else
        return null;
    }

    public void Put(string key, object value)
    {
      _list.Put(key, new StateElement(key, value));
    }
  }
}
