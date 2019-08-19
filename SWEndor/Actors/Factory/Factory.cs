using SWEndor.ActorTypes;
using SWEndor.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

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
      }

      private int counter = 0;
      private ConcurrentQueue<ActorInfo> planned = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> prepool = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> pool = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> dead = new ConcurrentQueue<ActorInfo>();

      // temp pools
      private ConcurrentQueue<ActorInfo> redo = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> nextplan = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> nextdead = new ConcurrentQueue<ActorInfo>();

      //private HashSet<int> dataid = new HashSet<int>();
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

          planned.Enqueue(actor);
          Add(id, actor);
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

        return actor;
      }


      public void ActivatePlanned()
      {
        ActorInfo actor = null;
        while (planned.TryDequeue(out actor))
        {
          if (actor.CreationTime < Engine.Game.GameTime)
          {
            actor.Initialize();
            //Add(actor.ID, actor);
          }
          else
            nextplan.Enqueue(actor);
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
          if (ScopedManager<ActorInfo>.Check(a) == 0)
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
          if (ScopedManager<ActorInfo>.Check(a) == 0)
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

      public void DoEach(Action<Engine, ActorInfo> action)
      {
        ActorInfo actor = First;
        while (actor != null)
        {
          action.Invoke(Engine, actor);
          actor = actor.Next;
        }
      }

      //public new ScopedManager<ActorInfo>.ScopedItem Get(int id)
      //{
      //  return ScopedManager<ActorInfo>.Scope(base.Get(id));
      //}

      public new void Remove(int id)
      {
        //using (var v = Get(id))
        //if (v != null)
        //{
        ActorInfo actor = Get(id);//v.Value;
        if (actor != null)
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
          prepool.Enqueue(actor);
        }
        //}
      }

      Action<Engine, ActorInfo> destroy = (e, a) => { a?.Destroy(); };
      public void Reset()
      {
        DestroyDead();
        DoEach(destroy);

        //foreach (ActorInfo a in GetAll())
        //  a?.Destroy();

        list.Clear();
        planned = new ConcurrentQueue<ActorInfo>();
        dead = new ConcurrentQueue<ActorInfo>();
        prepool = new ConcurrentQueue<ActorInfo>();
        pool = new ConcurrentQueue<ActorInfo>();
        First = null;
        Last = null;
      }
    }
  }
}
