using SWEndor.ActorTypes;
using System;
using System.Collections.Concurrent;
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
        DefaultKey = -1;
      }

      private int counter = 0;
      private ConcurrentQueue<ActorInfo> planned = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> pool = new ConcurrentQueue<ActorInfo>();
      private ConcurrentQueue<ActorInfo> dead = new ConcurrentQueue<ActorInfo>();
      private ActorInfo First;
      private ActorInfo Last;
      private object creationLock = new object();

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
            actor = new ActorInfo(this, id, acinfo);
          }

          planned.Enqueue(actor);
          Add(id, actor);

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
        ConcurrentQueue<ActorInfo> nextplan = new ConcurrentQueue<ActorInfo>();
        ActorInfo actor = null;
        while (planned.TryDequeue(out actor))
        {
          if (actor.StateModel.CreationTime < Engine.Game.GameTime)
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
          a.Destroy();
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

      public new void Remove(int id)
      {
        int x = id;

        ActorInfo actor = Get(id);
        if (actor == null)
          return;

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
        pool.Enqueue(actor);
      }

      public void Reset()
      {
        DestroyDead();
        foreach (ActorInfo a in GetAll())
          a?.Destroy();

        list.Clear();
        planned = new ConcurrentQueue<ActorInfo>();
        dead = new ConcurrentQueue<ActorInfo>();
        pool = new ConcurrentQueue<ActorInfo>();
        First = null;
        Last = null;
      }
    }
  }
}
