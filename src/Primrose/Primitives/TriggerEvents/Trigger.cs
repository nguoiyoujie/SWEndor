using Primrose.Primitives.ValueTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Primrose.Primitives.TriggerEvents
{
  public enum EventType
  {
    AND = 0,
    OR = 1
  }

  public class Trigger
  {
    private Task m_Task;

    public ConditionCollection Conditions;
    public ActionCollection Actions;
    public EventType ConditionType = EventType.AND;
    private List<Pair<bool, ICondition>> _cond = new List<Pair<bool, ICondition>>();
    private List<ICondition> _subs = new List<ICondition>();
    private List<Action> _acts = new List<Action>();

    public Trigger()
    {
      Conditions.Init(this);
      Actions.Init(this);
    }

    public struct ConditionCollection
    {
      private Trigger _p;
      internal void Init(Trigger t) { _p = t; }

      public void Add(ICondition tc, bool negativeCheck)
      {
        Pair<bool, ICondition> p = new Pair<bool, ICondition>(negativeCheck, tc);
        if (!_p._cond.Contains(p))
        {
          _p._cond.Add(p);
          Subscribe(tc);
        }
      }

      public void Remove(ICondition tc)
      {
        if (_p._cond.Remove(new Pair<bool, ICondition>(true, tc)) || _p._cond.Remove(new Pair<bool, ICondition>(false, tc)))
          Unsubscribe(tc);
      }

      public void RemoveAll()
      {
        UnsubscribeAll();
        _p._cond.Clear();
      }

      private void Subscribe(ICondition c)
      {
        _p._subs.Add(c);
        c.Update += _p.EventCheck;
      }

      private void Unsubscribe(ICondition c)
      {
        if (_p._subs.Remove(c))
          c.Update -= _p.EventCheck;
      }

      private void UnsubscribeAll()
      {
        foreach (ICondition c in _p._subs)
          c.Update -= _p.EventCheck;
        _p._subs.Clear();
      }
    }

    public struct ActionCollection
    {
      private Trigger _p;
      internal void Init(Trigger t) { _p = t; }

      public void Add(Action a) { if (!_p._acts.Contains(a)) _p._acts.Add(a); }

      public void Remove(Action a) { _p._acts.Remove(a); }

      public void RemoveAll() { _p._acts.Clear(); }
    }

    private void EventCheck(ConditionUpdateEventArgs args)
    {
      if (ConditionType == EventType.OR)
      {
        if (args.Fulfilled)
          Execute();
        return;
      }
      else
      {
        foreach (Pair<bool, ICondition> bc in _cond)
        {
          if (bc.u.Fulfilled == bc.t)
            return;
        }
        Execute();
      }
    }

    private void Execute()
    {
      foreach (Action a in _acts)
        a.Invoke();
    }
  }
}
