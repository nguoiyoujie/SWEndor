using Primrose.Primitives;
using System.Collections.Generic;
using SWEndor.Game.Core;
using System;

namespace SWEndor
{
  internal class GameEventQueue
  {
    private readonly Engine _engine;
    private readonly SortedSet<GameEventObject> list = new SortedSet<GameEventObject>(new GameEventObject.Comparer());
    private readonly Queue<GameEventObject> queue = new Queue<GameEventObject>();
    private readonly ScopeCounters.ScopeCounter _scope = new ScopeCounters.ScopeCounter();
    private readonly Predicate<GameEventObject> _expire;

    public GameEventQueue(Engine engine)
    {
      _engine = engine;
      _expire = Expire;
    }

    internal struct GameEventObject
    {
      public readonly float Time;
      public readonly Engine Engine;
      private readonly GameEvent Method;

      internal GameEventObject(Engine engine, float time, GameEvent gameEvent)
      {
        Time = time;
        Engine = engine;
        Method = gameEvent;
      }

      internal void Run()
      {
        Method?.Invoke();
      }

      internal class Comparer : IComparer<GameEventObject>
      {
        public int Compare(GameEventObject x, GameEventObject y)
        {
          return x.Time.CompareTo(y.Time);
        }
      }
    }

    public void Add<T>(float time, GameEvent<T> method, T arg)
    {
      Add(time, () => method(arg));
    }

    public void Add<T1, T2>(float time, GameEvent<T1, T2> method, T1 a1, T2 a2)
    {
      Add(time, () => method(a1, a2));
    }

    public void Add<T1, T2, T3>(float time, GameEvent<T1, T2, T3> method, T1 a1, T2 a2, T3 a3)
    {
      Add(time, () => method(a1, a2, a3));
    }

    public void Add(float time, GameEvent method)
    {
      GameEventObject geo = new GameEventObject(_engine, time, method);
      while (list.Contains(geo))
      {
        time += 0.0001f;
        geo = new GameEventObject(_engine, time, method);
      }

      using (ScopeCounters.Acquire(_scope))
        list.Add(geo);
    }

    public void Clear()
    {
      using (ScopeCounters.AcquireWhenZero(_scope))
      {
        list.Clear();
        queue.Clear();
      }
    }

    private bool Expire(GameEventObject a) 
    { 
      return a.Time < _engine.Game.GameTime; 
    }

    public void Process()
    {
      using (ScopeCounters.AcquireWhenZero(_scope))
      {
        foreach (var l in list)
        {
          if (Expire(l))
            queue.Enqueue(l);
          else
            break;
        }
        list.RemoveWhere(_expire);
      }

      foreach (var l in queue)
        l.Run();

      using (ScopeCounters.AcquireWhenZero(_scope))
      {
        queue.Clear();
      }
    }
  }
}
