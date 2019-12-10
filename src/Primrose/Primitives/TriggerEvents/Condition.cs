using Primrose.Primitives.Eventful;
using System.Collections.Generic;

namespace Primrose.Primitives.TriggerEvents
{
  public interface ICondition
  {
    event ConditionUpdateEventHandler Update;
    bool Fulfilled { get; }
  }

  public class Condition<T> : ICondition
  {
    private event ConditionUpdateEventHandler _update;
    private List<EventedVal<T>> subscribes = new List<EventedVal<T>>();

    public bool Fulfilled { get { return met; } }
    private bool met;
    protected bool ConditionMet
    {
      get { return met; }
      set
      {
        met = value;
        OnUpdate(Fulfilled);
      }
    }

    public Condition() { }

    public event ConditionUpdateEventHandler Update { add { _update += value; } remove { _update -= value; } }

    public void OnUpdate(bool fulfilled)
    {
      //if (fulfilled)
      //{ }

      _update?.Invoke(new ConditionUpdateEventArgs(this, fulfilled));
    }

    public virtual void Check(T oldValue, T newValue) { } // change the signature

    public void Subscribe(EventedVal<T> target)
    {
      subscribes.Add(target);
      target.ValueChanged += Check;
    }

    public void Unsubscribe(EventedVal<T> target)
    {
      if (subscribes.Contains(target))
      {
        subscribes.Remove(target);
        target.ValueChanged -= Check;
      }
    }

    public void UnsubscribeAll()
    {
      foreach (var s in subscribes)
        s.ValueChanged -= Check;
      subscribes.Clear();
    }
  }
}
