using SWEndor.Primitives.Extensions;
using System;
using System.Collections.Generic;

namespace SWEndor.Primitives.StateMachines
{
  public class StaticStateMachine<O, T, U> where T : struct, IConvertible where U : struct, IConvertible
  {
    private Dictionary<T, InStateMachine> _transitions = new Dictionary<T, InStateMachine>();

    protected void Initialize(O owner, T state)
    {
      InStateMachine _in;
      if (_transitions.TryGetValue(state, out _in))
        _in.Initialize(owner, state, state);
    }

    protected InStateMachine In(T state)
    {
      InStateMachine ret;
      if (!_transitions.TryGetValue(state, out ret))
      {
        ret = new InStateMachine(this, state);
        _transitions.Add(state, ret);
      }
      return ret;
    }

    protected class InStateMachine
    {
      private StaticStateMachine<O, T, U> SM;
      private T State;
      private Action<O, T> Entry;
      private Action<O, T> Exit;
      private Dictionary<U, OutStateMachine> _transitions = new Dictionary<U, OutStateMachine>();

      internal InStateMachine(StaticStateMachine<O, T, U> statemachine, T state) { State = state; SM = statemachine; }

      public OutStateMachine On(U command)
      {
        OutStateMachine ret;
        if (!_transitions.TryGetValue(command, out ret))
        {
          ret = new OutStateMachine(SM, this, command);
          _transitions.Add(command, ret);
        }
        return ret;
      }

      public InStateMachine ExecuteOnEntry(Action<O, T> action) { Entry = action; return this; }

      public InStateMachine ExecuteOnExit(Action<O, T> action) { Exit = action; return this; }

      internal void Initialize(O owner, T prevstate, T state)
      {
        Entry?.Invoke(owner, state);
      }

      public void Fire(O owner, U command, ref T state)
      {
        OutStateMachine _on;
        if (_transitions.TryGetValue(command, out _on))
        {
          Exit?.Invoke(owner, state);
          _on.Fire(owner, ref state); // state is changed here
          SM.Initialize(owner, state);
        }
        else
          throw new InvalidOperationException("Command '{0}' is not valid on state '{1}'".F(command, state));
      }
    }

    protected class OutStateMachine
    {
      private StaticStateMachine<O, T, U> SM;
      private InStateMachine IN;
      private U Command;
      private T NewState;
      private Action<O, T> Transit;

      internal OutStateMachine(StaticStateMachine<O, T, U> statemachine, InStateMachine instate, U command) { Command = command; IN = instate; SM = statemachine; }

      public OutStateMachine On(U command)
      {
        return IN.On(command);
      }

      public OutStateMachine Goto(T targetstate) { NewState = targetstate; return this; }

      public OutStateMachine Execute(Action<O, T> action) { Transit = action; return this; }

      internal void Fire(O owner, ref T state)
      {
        Transit?.Invoke(owner, state);
        state = NewState;
      }
    }

    public void Fire(O owner, U command, ref T state)
    {
      InStateMachine _in;
      if (_transitions.TryGetValue(state, out _in))
        _in.Fire(owner, command, ref state);
      else
        throw new InvalidOperationException("Command '{0}' is not valid on state '{1}'".F(command, state));
    }
  }
}
