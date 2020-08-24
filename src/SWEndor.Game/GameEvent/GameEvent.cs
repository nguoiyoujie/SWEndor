using MTV3D65;
using SWEndor.Game.Actors;
using Primrose.Primitives;
using SWEndor.Game.Scenarios;
using System.Collections.Generic;
using System;
using SWEndor.Game.Core;
using SWEndor.Game.Models;

namespace SWEndor
{
  public class ShipSpawnEventArg
  {
    public readonly GSFunctions.ShipSpawnInfo Info;
    public readonly TV_3DVECTOR Position;
    public readonly TV_3DVECTOR TargetPosition;
    public readonly TV_3DVECTOR FacingPosition;

    public ShipSpawnEventArg(GSFunctions.ShipSpawnInfo info, TV_3DVECTOR position, TV_3DVECTOR targetposition, TV_3DVECTOR facingposition)
    {
      Info = info;
      Position = position;
      TargetPosition = targetposition;
      FacingPosition = facingposition;
    }
  }

  public delegate void GameEvent();
  public delegate void GameEvent<T>(T arg);
  public delegate void GameEvent<T1, T2>(T1 a1, T2 a2);
  public delegate void GameEvent<T1, T2, T3>(T1 a1, T2 a2, T3 a3);
  public delegate void ActorEvent(ActorInfo actor);
  public delegate void HitEvent(ActorInfo actor, ActorInfo attacker);
  public delegate void ActorStateChangeEvent(ActorInfo actor, ActorState state);
  public delegate void ShipSpawnEvent(ShipSpawnEventArg eventArg);


  internal class GameEventQueue
  {
    private readonly Engine _engine;
    private readonly SortedSet<GameEventObject> list = new SortedSet<GameEventObject>(new GameEventObject.Comparer());
    private readonly Queue<GameEventObject> queue = new Queue<GameEventObject>();
    private readonly ScopeCounters.ScopeCounter _scope = new ScopeCounters.ScopeCounter();

    public GameEventQueue(Engine engine)
    {
      _engine = engine;
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
        list.Clear();
    }

    private static readonly Predicate<GameEventObject> _expire = (a) => { return a.Time < a.Engine.Game.GameTime; };
    public void Process()
    {
      using (ScopeCounters.AcquireWhenZero(_scope))
      {
        foreach (var l in list)
        {
          if (_expire(l))
            queue.Enqueue(l);
          else
            break;
        }
        list.RemoveWhere(_expire);
      }

      foreach (var l in queue)
        l.Run();

      queue.Clear();
    }
  }
}
