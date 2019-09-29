using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System;
using System.Collections.Concurrent;

namespace SWEndor
{
  public class Factory<T, TCreate, TType> : Primitives.Factories.Registry<int, T>
    where T :
    class,
    IEngineObject,
    ILinked<T>,
    IScoped,
    IActorDisposable,
    IActorCreateable<TCreate>
    where TCreate :
    ICreationInfo<T, TType>
    where TType :
    ITypeInfo<T>
  {
    public readonly Engine Engine;
    internal Factory(Engine engine, Func<Engine, Factory<T, TCreate, TType>, int, int, TCreate, T> createfunc)
    {
      Engine = engine;
      Actors = new ActorEnumerable(this);
      create = createfunc;
    }

    private int counter = 0;
    private ConcurrentQueue<T> planned = new ConcurrentQueue<T>();
    private ConcurrentQueue<T> prepool = new ConcurrentQueue<T>();
    private ConcurrentQueue<T> pool = new ConcurrentQueue<T>();
    private ConcurrentQueue<T> dead = new ConcurrentQueue<T>();

    // temp holding pools
    private ConcurrentQueue<T> redo = new ConcurrentQueue<T>();
    private ConcurrentQueue<T> nextplan = new ConcurrentQueue<T>();
    private ConcurrentQueue<T> nextdead = new ConcurrentQueue<T>();

    int lastdataid = 0;

    private T First;
    private T Last;
    private object creationLock = new object();

    private readonly Func<Engine, Factory<T, TCreate, TType>, int, int, TCreate, T> create;

    private int GetNewDataID()
    {
      // since Actors are reused, there is no need to count backwards
      if (lastdataid >= Globals.ActorLimit)
        throw new Exception(TextLocalization.Get(TextLocalKeys.ACTOR_OVERFLOW_ERROR).F(Globals.ActorLimit));

      return lastdataid++;
    }

    public T Create(TCreate acinfo)
    {
      T actor = null;
      if (acinfo.TypeInfo == null)
        throw new Exception(TextLocalization.Get(TextLocalKeys.ACTOR_INVALID_ERROR));

      lock (creationLock)
      {
        int id = counter++;
        if (pool.TryDequeue(out actor))
        {
          actor.Rebuild(Engine, id, acinfo);
        }
        else
        {
          actor = create(Engine, this, id, GetNewDataID(), acinfo);
        }

        using (ScopeCounterManager.Acquire(actor.Scope))
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
      T actor = null;
      while (planned.TryDequeue(out actor))
      {
        using (ScopeCounterManager.Acquire(actor.Scope))
        {
          if (!actor.DisposingOrDisposed)
            if (actor.CreationTime < Engine.Game.GameTime)
              actor.Initialize(Engine);
            else
              nextplan.Enqueue(actor);
        }
      }

      while (nextplan.TryDequeue(out actor))
        planned.Enqueue(actor);
    }

    internal void MakeDead(T a)
    {
      dead.Enqueue(a);
    }

    internal void DestroyDead()
    {
      T a;
      while (dead.TryDequeue(out a))
        if (a.Scope.Count == 0)
          a.Destroy();
        else
          nextdead.Enqueue(a);

      while (nextdead.TryDequeue(out a))
        dead.Enqueue(a);
    }

    internal void ReturnToPool()
    {
      T a;

      while (prepool.TryDequeue(out a))
        if (a.Scope.Count == 0)
          pool.Enqueue(a);
        else
          redo.Enqueue(a);

      while (redo.TryDequeue(out a))
        prepool.Enqueue(a);
    }

    public int GetActorCount()
    {
      return list.Count;
    }

    public void DoUntil(Func<Engine, T, bool> action)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a))
          break;
    }

    public void DoUntil<T1>(Func<Engine, T, T1, bool> action, T1 cmp)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a, cmp))
          break;
    }

    public void DoUntil<T1, T2>(Func<Engine, T, T1, T2, bool> action, T1 c1, T2 c2)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a, c1, c2))
          break;
    }

    public void DoUntil<T1, T2, T3>(Func<Engine, T, T1, T2, T3, bool> action, T1 c1, T2 c2, T3 c3)
    {
      foreach (T a in Actors)
        if (!action.Invoke(a.Engine, a, c1, c2, c3))
          break;
    }

    public void DoEach(Action<Engine, T> action)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a);
    }

    public void DoEach<T1>(Action<Engine, T, T1> action, T1 cmp)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, cmp);
    }

    public void DoEach<T1, T2>(Action<Engine, T, T1, T2> action, T1 c1, T2 c2)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, c1, c2);
    }

    public void DoEach<T1, T2, T3>(Action<Engine, T, T1, T2, T3> action, T1 c1, T2 c2, T3 c3)
    {
      foreach (T a in Actors)
        action.Invoke(a.Engine, a, c1, c2, c3);
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

          base.Remove(id);
        }
        prepool.Enqueue(actor);
      }
    }

    Action<Engine, T> destroy = (e, a) => { a?.Delete(); };
    public void Reset()
    {
      DoEach(destroy);
      DestroyDead();

      list.Clear();
      planned = new ConcurrentQueue<T>();
      dead = new ConcurrentQueue<T>();
      prepool = new ConcurrentQueue<T>();
      pool = new ConcurrentQueue<T>();
      First = null;
      Last = null;

      lastdataid = 0;
      counter = 0;
    }
  }
}
