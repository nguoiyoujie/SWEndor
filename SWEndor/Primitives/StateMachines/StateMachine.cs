﻿using System;
using System.Collections.Generic;

namespace SWEndor.Primitives.StateMachines
{
  public class StateMachine<T, U> where T : struct, IConvertible where U : struct, IConvertible
  {
    public delegate void TransitionAction(T prevState, T currentState);
    public delegate bool TransitionCondition(T prevState, T currentState);

    public T PreviousState { get; private set; }
    public T CurrentState { get; private set; }
    private Dictionary<T, InStateMachine> _transitions = new Dictionary<T, InStateMachine>();

    protected void Initialize(T state)
    {
      PreviousState = state;
      CurrentState = state;

      InStateMachine _in;
      if (_transitions.TryGetValue(CurrentState, out _in))
        _in.Initialize();
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
      private StateMachine<T, U> SM;
      private T State;
      private TransitionAction Entry;
      private TransitionAction Exit;
      private Dictionary<U, OutStateMachine> _transitions = new Dictionary<U, OutStateMachine>();

      internal InStateMachine(StateMachine<T, U> statemachine, T state) { State = state; SM = statemachine; }

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

      public InStateMachine ExecuteOnEntry(TransitionAction action) { Entry = action; return this; }

      public InStateMachine ExecuteOnExit(TransitionAction action) { Exit = action; return this; }

      internal void Initialize()
      {
        Entry?.Invoke(SM.PreviousState, SM.CurrentState);
      }

      public void Fire(U command)
      {
        OutStateMachine _on;
        if (_transitions.TryGetValue(command, out _on))
        {
          Exit?.Invoke(SM.PreviousState, SM.CurrentState);
          _on.Fire(); // state is changed here
          SM.Initialize(SM.CurrentState);
        }
        else
          throw new InvalidOperationException(string.Format("Command '{0}' is not valid on state '{1}'", command, SM.CurrentState));
      }
    }

    protected class OutStateMachine
    {
      private StateMachine<T, U> SM;
      private InStateMachine IN;
      private U Command;
      private T NewState;
      private TransitionAction Transit;

      internal OutStateMachine(StateMachine<T, U> statemachine, InStateMachine instate, U command) { Command = command; IN = instate; SM = statemachine; }

      public OutStateMachine On(U command)
      {
        return IN.On(command);
      }

      public OutStateMachine Goto(T targetstate) { NewState = targetstate; return this; }

      public OutStateMachine Execute(TransitionAction action) { Transit = action; return this; }

      internal void Fire()
      {
        Transit?.Invoke(SM.PreviousState, SM.CurrentState);
        SM.CurrentState = NewState;
      }
    }

    public void Fire(U command)
    {
      InStateMachine _in;
      if (_transitions.TryGetValue(CurrentState, out _in))
        _in.Fire(command);
      else
        throw new InvalidOperationException(string.Format("Command '{0}' is not valid on state '{1}'", command, CurrentState));
    }
  }
}