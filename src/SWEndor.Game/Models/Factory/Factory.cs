using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives;
using System;
using System.Collections.Concurrent;

namespace SWEndor
{
  public class Factory<T, TCreate, TType> : Primrose.Primitives.Factories.Registry<int, T>
    where T :
    class,
    IEngineObject,
    IIdentity<int>,
    ILinked<T>,
    IScoped,
    IActorDisposable,
    IActorCreateable<TCreate>
    where TCreate :
    ICreationInfo<T, TType>
    where TType :
    ITypeInfo<T>
  {
    // delegates
    public delegate void EngineFunctionDelegate<V>(Engine e, T obj, ref V prevValue);
    public delegate void EngineFunctionDelegate<P, V>(Engine e, T obj, P p, ref V prevValue);
    public delegate void EngineFunctionDelegate<P1, P2, V>(Engine e, T obj, P1 p1, P2 p2, ref V prevValue);
    public delegate void EngineFunctionDelegate<P1, P2, P3, V>(Engine e, T obj, P1 p1, P2 p2, P3 p3, ref V prevValue);

    public delegate bool EnginePredicateDelegate(Engine e, T obj);
    public delegate bool EnginePredicateDelegate<P>(Engine e, T obj, P p);
    public delegate bool EnginePredicateDelegate<P1, P2>(Engine e, T obj, P1 p1, P2 p2);
    public delegate bool EnginePredicateDelegate<P1, P2, P3>(Engine e, T obj, P1 p1, P2 p2, P3 p3);
    public delegate bool EnginePredicateDelegate<P1, P2, P3, P4>(Engine e, T obj, P1 p1, P2 p2, P3 p3, P4 p4);

    public delegate void EngineActionDelegate(Engine e, T obj);
    public delegate void EngineActionDelegate<P>(Engine e, T obj, P p);
    public delegate void EngineActionDelegate<P1, P2>(Engine e, T obj, P1 p1, P2 p2);
    public delegate void EngineActionDelegate<P1, P2, P3>(Engine e, T obj, P1 p1, P2 p2, P3 p3);

    /// <summary>The game engine</summary>
    public readonly Engine Engine;
    internal Factory(Engine engine, Func<Engine, Factory<T, TCreate, TType>, short, TCreate, T> createfunc, int limit) : base(limit)
    {
      Engine = engine;
      Actors = new ActorEnumerable(this);
      create = createfunc;
    }

    private short counter = 0;
    private readonly ConcurrentQueue<T> planned = new ConcurrentQueue<T>();
    private readonly ConcurrentQueue<T> prepool = new ConcurrentQueue<T>();
    private readonly ConcurrentQueue<T> pool = new ConcurrentQueue<T>();
    private readonly ConcurrentQueue<T> dead = new ConcurrentQueue<T>();

    // temp holding pools
    private readonly ConcurrentQueue<T> redo = new ConcurrentQueue<T>();
    private readonly ConcurrentQueue<T> nextplan = new ConcurrentQueue<T>();
    private readonly ConcurrentQueue<T> nextdead = new ConcurrentQueue<T>();

    private T First;
    private T Last;
    private readonly object creationLock = new object();

    private readonly Func<Engine, Factory<T, TCreate, TType>, short, TCreate, T> create;

    internal int PoolPlanCount { get { return planned.Count; } }
    internal int PoolPrepCount { get { return prepool.Count; } }
    internal int PoolMainCount { get { return pool.Count; } }
    internal int PoolDeadCount { get { return dead.Count; } }
    internal int TempPoolPlanCount { get { return nextplan.Count; } }
    internal int TempPoolRedoCount { get { return redo.Count; } }
    internal int TempPoolDeadCount { get { return nextdead.Count; } }

    public T Create(TCreate acinfo)
    {
      T actor = null;
      if (acinfo.TypeInfo == null)
        throw new Exception(TextLocalization.Get(TextLocalKeys.ACTOR_INVALID_ERROR));

      lock (creationLock)
      {
        short id = counter++;
        if (pool.TryDequeue(out actor))
        {
          actor.Rebuild(Engine, id, acinfo);
        }
        else
        {
          actor = create(Engine, this, id, acinfo);
        }

        using (ScopeCounters.Acquire(actor.Scope))
        {
          planned.Enqueue(actor);
          Add(id, actor);
          //if (actor.Logged)
          //  Log.Write(Log.DEBUG, LogType.ACTOR_CREATED, actor);

          if (First == null)
          {
            First = actor;
            actor.Prev = null;
            actor.Next = null;
          }
          else
          {
            Last.Next = actor;
            actor.Prev = Last;
            actor.Next = null;
          }
          Last = actor;
        }
      }

      return actor;
    }

    internal void ActivatePlanned()
    {
      while (planned.TryDequeue(out T actor))
      {
        using (ScopeCounters.Acquire(actor.Scope))
        {
          if (!actor.DisposingOrDisposed)
            if (actor.CreationTime < Engine.Game.GameTime)
              actor.Initialize(Engine, true);
            else
              nextplan.Enqueue(actor);
        }
      }

      while (nextplan.TryDequeue(out T actor))
        planned.Enqueue(actor);
    }

    internal void DisposePlanned()
    {
      while (planned.TryDequeue(out T actor))
        dead.Enqueue(actor);
    }

    internal void MakeDead(T a)
    {
      dead.Enqueue(a);
    }

    internal void DestroyDead()
    {
      T a;
      while (dead.TryDequeue(out a))
          a.Destroy();

      while (nextdead.TryDequeue(out a))
        dead.Enqueue(a);
    }

    internal void ReturnToPool()
    {
      T a;

      while (prepool.TryDequeue(out a))
        if (a.Scope.Count == 0)
        {
          RemoveInPrepool(a.ID);
          pool.Enqueue(a);
        }
        else
          redo.Enqueue(a);

      while (redo.TryDequeue(out a))
        prepool.Enqueue(a);
    }

    public int GetActorCount()
    {
      return list.Count;
    }

    public bool DoUntil(EnginePredicateDelegate action)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a))
          return false;
      return true;
    }

    public bool DoUntil<P>(EnginePredicateDelegate<P> action, P cmp)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a, cmp))
          return false;
      return true;
    }

    public bool DoUntil<P1, P2>(EnginePredicateDelegate<P1, P2> action, P1 p1, P2 p2)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a, p1, p2))
          return false;
      return true;
    }

    public bool DoUntil<P1, P2, P3>(EnginePredicateDelegate<P1, P2, P3> action, P1 p1, P2 p2, P3 p3)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a, p1, p2, p3))
          return false;
      return true;
    }

    public bool DoUntil<P1, P2, P3, P4>(EnginePredicateDelegate<P1, P2, P3, P4> action, P1 p1, P2 p2, P3 p3, P4 p4)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a, p1, p2, p3, p4))
          return false;
      return true;
    }

    public void DoEach(EngineActionDelegate action)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a);
    }

    public void DoEach<P>(EngineActionDelegate<P> action, P param)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, param);
    }

    public void DoEach<P1, P2>(EngineActionDelegate<P1, P2> action, P1 p1, P2 p2)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, p1, p2);
    }

    public void DoEach<P1, P2, P3>(EngineActionDelegate<P1, P2, P3> action, P1 p1, P2 p2, P3 p3)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, p1, p2, p3);
    }

    public void DoEach<V>(EngineFunctionDelegate<V> action, ref V value)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, ref value);
    }

    public void DoEach<V, P>(EngineFunctionDelegate<P, V> action, P param, ref V value)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, param, ref value);
    }

    public void DoEach<V, P1, P2>(EngineFunctionDelegate<P1, P2, V> action, P1 p1, P2 p2, ref V value)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, p1, p2, ref value);
    }

    public void DoEach<V, P1, P2, P3>(EngineFunctionDelegate<P1, P2, P3, V> action, P1 p1, P2 p2, P3 p3, ref V value)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, p1, p2, p3, ref value);
    }

    public void StaggeredDoEach(int stagger, ref int source, EngineActionDelegate action)
    {
      foreach (T a in Actors)
        if (a.ID % stagger == source % stagger)
        action.Invoke(a.Engine, a);
    }

    public void StaggeredDoEach<P1>(int stagger, ref int source, EngineActionDelegate<P1> action, P1 p)
    {
      foreach (T a in Actors)
        if (a.ID % stagger == source % stagger)
          action.Invoke(a.Engine, a, p);
    }

    public void StaggeredDoEach<P1, P2>(int stagger, ref int source, EngineActionDelegate<P1, P2> action, P1 p1, P2 p2)
    {
      foreach (T a in Actors)
        if (a.ID % stagger == source % stagger)
          action.Invoke(a.Engine, a, p1, p2);
    }

    public void StaggeredDoEach<P1, P2, P3>(int stagger, ref int source, EngineActionDelegate<P1, P2, P3> action, P1 p1, P2 p2, P3 p3)
    {
      foreach (T a in Actors)
        if (a.ID % stagger == source % stagger)
          action.Invoke(a.Engine, a, p1, p2, p3);
    }


    public ActorEnumerable Actors;

    public struct ActorEnumerable
    {
      readonly Factory<T, TCreate, TType> F;
      public ActorEnumerable(Factory<T, TCreate, TType> f) { F = f; }
      public ActorEnumerator GetEnumerator() { return new ActorEnumerator(F); }
    }

    public struct ActorEnumerator
    {
      readonly Factory<T, TCreate, TType> F;
      T current;
      public ActorEnumerator(Factory<T, TCreate, TType> f) { F = f; current = null; }
      public bool MoveNext() { return (current = (current == null) ? F.First : current?.Next) != null; }
      public T Current { get { return current; } }
    }

    public new void Remove(int id)
    {
      T actor = Get(id);
      if (actor != null)
      {
        lock (creationLock)
        {
          if (First == actor && Last == actor)
          {
            First = null;
            Last = null;
          }
          else if (First == actor)
          {
            First = actor.Next;
          }
          else if (Last == actor)
          {
            Last = actor.Prev;
          }
          else
          {
            actor.Prev.Next = actor.Next;
            actor.Next.Prev = actor.Prev;
          }

          actor.Next = null;
          actor.Prev = null;

          // Allow Factory.Get(int) to still access destroyed actor until it is cleaned from prepool (when scope is 0)
          //base.Remove(id);
        }
        prepool.Enqueue(actor);
      }
    }

    private void RemoveInPrepool(int id)
    {
       base.Remove(id);
    }

    public void Reset()
    {
      DisposePlanned();
      DoEach((e, a) => { a?.Delete(); });
      DestroyDead();
    }

    public override T Get(int key)
    {
      return base.Get(key);
    }
  }
}
