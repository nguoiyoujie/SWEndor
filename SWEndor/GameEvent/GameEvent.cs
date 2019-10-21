using MTV3D65;
using SWEndor.Actors;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using System.Collections.Generic;
using System;
using SWEndor.Core;
using SWEndor.Models;

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
  public delegate void ActorEvent(ActorInfo actor);
  public delegate void HitEvent(ActorInfo actor, ActorInfo victim);
  public delegate void ActorStateChangeEvent(ActorInfo actor, ActorState state);
  public delegate void ShipSpawnEvent(ShipSpawnEventArg eventArg);


  internal static class GameEventQueue
  {
    private static SortedSet<GameEventObject> list = new SortedSet<GameEventObject>(new GameEventObject.Comparer());
    private static Queue<GameEventObject> queue = new Queue<GameEventObject>();
    private static ScopeCounterManager.ScopeCounter _scope = new ScopeCounterManager.ScopeCounter();

    internal struct GameEventObject
    {
      public readonly float Time;
      private readonly GameEvent Method;

      internal GameEventObject(float time, GameEvent gameEvent)
      {
        Time = time;
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

    public static void Add<T>(float time, GameEvent<T> method, T arg)
    {
      Add(time, () => method(arg));
    }

    public static void Add(float time, GameEvent method)
    {
      GameEventObject geo = new GameEventObject(time, method);
      while (list.Contains(geo))
      {
        time += 0.0001f;
        geo = new GameEventObject(time, method);
      }

      using (ScopeCounterManager.Acquire(_scope))
        list.Add(geo);
    }

    public static void Clear()
    {
      using (ScopeCounterManager.AcquireWhenZero(_scope))
        list.Clear();
    }

    private static Predicate<GameEventObject> _expire = (a) => { return a.Time < Globals.Engine.Game.GameTime; };
    public static void Process(Engine engine)
    {
      using (ScopeCounterManager.AcquireWhenZero(_scope))
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
