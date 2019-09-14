using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public class Factory : Primitives.Factories.Registry<int, ActorInfo>
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      {
        Engine = engine;
        Actors = new ActorEnumerable(this);
      }

      private int counter = 0;
      private ConcurrentQueue<ActorInfo> planned = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> prepool = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> pool = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> dead = new ConcurrentQueue<ActorInfo>();

      // temp holding pools
      private ConcurrentQueue<ActorInfo> redo = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> nextplan = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> nextdead = new ConcurrentQueue<ActorInfo>();

      int lastdataid = 0;

      private ActorInfo First;
      private ActorInfo Last;
      private object creationLock = new object();

      private int GetNewDataID()
      {
        // since Actors are reused, there is no need to count backwards
        if (lastdataid >= Globals.ActorLimit)
          throw new Exception("Number of current actors exceeded limit of {0}!".F(Globals.ActorLimit));

        return lastdataid++;
      }

      public ActorInfo Create(ActorCreationInfo acinfo)
      {
        ActorInfo actor = null;
        if (acinfo.ActorTypeInfo == null)
          throw new Exception("Attempted to register actor with null ActorTypeInfo!");

        lock (creationLock)
        {
          int id = counter++;
          if (pool.TryDequeue(out actor))
          {
            actor.ID = id;
            actor.Rebuild(acinfo);
          }
          else
          {
            actor = new ActorInfo(this, id, GetNewDataID(), acinfo);
          }

          using (ScopeCounterManager.Acquire(actor.Scope))
          {
            planned.Enqueue(actor);
            Add(id, actor);
            if (actor.Logged)
              Log.Write(Log.DEBUG, LogType.ACTOR_CREATED, actor);

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

      public void ActivatePlanned()
      {
        ActorInfo actor = null;
        while (planned.TryDequeue(out actor))
        {
          using (ScopeCounterManager.Acquire(actor.Scope))
          {
            if (!actor.DisposingOrDisposed)
              if (actor.CreationTime < Engine.Game.GameTime)
                actor.Initialize();
              else
                nextplan.Enqueue(actor);
          }
        }

        while (nextplan.TryDequeue(out actor))
          planned.Enqueue(actor);
      }

      public void MakeDead(ActorInfo a)
      {
        dead.Enqueue(a);
      }

      public void DestroyDead()
      {
        ActorInfo a;
        while (dead.TryDequeue(out a))
          if (a.Scope.Count == 0)
            a.Destroy();
          else
            nextdead.Enqueue(a);

        while (nextdead.TryDequeue(out a))
          dead.Enqueue(a);
      }

      public void ReturnToPool()
      {
        ActorInfo a;

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

      public void DoUntil(Func<Engine, ActorInfo, bool> action)
      {
        foreach (ActorInfo a in Actors)
          if (!action.Invoke(a.Engine, a))
            break;
      }

      public void DoEach(Action<Engine, ActorInfo> action)
      {
        foreach (ActorInfo a in Actors)
          action.Invoke(a.Engine, a);
      }

      public ActorEnumerable Actors;

      public struct ActorEnumerable
      {
        readonly Factory F;
        public ActorEnumerable(Factory f) { F = f; }
        public ActorEnumerator GetEnumerator() { return new ActorEnumerator(F); }
      }

      public struct ActorEnumerator //: IEnumerator<ActorInfo>
      {
        readonly Factory F;
        ActorInfo current;
        public ActorEnumerator(Factory f) { F = f; current = null; }

        public void Reset() { current = null; }
        public bool MoveNext() { return (current = (current == null) ? F.First : current?.Next) != null; }
        public ActorInfo Current { get { return current; } }
        //object System.Collections.IEnumerator.Current { get { return Current; } }
        public void Dispose() { }
      }

      public new void Remove(int id)
      {
        ActorInfo actor = Get(id);
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

      Action<Engine, ActorInfo> destroy = (e, a) => { a?.Delete(); };
      public void Reset()
      {
        DoEach(destroy);
        DestroyDead();

        list.Clear();
        planned = new ConcurrentQueue<ActorInfo>();
        dead = new ConcurrentQueue<ActorInfo>();
        prepool = new ConcurrentQueue<ActorInfo>();
        pool = new ConcurrentQueue<ActorInfo>();
        First = null;
        Last = null;

        lastdataid = 0;
        counter = 0;
      }
    }
  }
}
