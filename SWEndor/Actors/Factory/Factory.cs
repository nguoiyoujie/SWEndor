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

    /*
    public class Factory
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      private const int capacity = Globals.ActorLimit; // hard code limit to ActorInfo. We should not be exceeding 1000 normally.
      private ConcurrentQueue<ActorInfo> deadqueue = new ConcurrentQueue<ActorInfo>();
      private ActorInfo[] list = new ActorInfo[capacity];
      private int count = 0;
      private int counter = 0;
      private int emptycounter = 0;
      private Mutex mu_counter = new Mutex();
      private int First = -1;
      private int Last = -1;

      public ActorInfo Create(ActorCreationInfo acinfo)
      {
        return Register(acinfo);
      }

      public ActorInfo Register(ActorCreationInfo amake, string key = "")
      {
        ActorInfo actor = null;
        if (amake.ActorTypeInfo == null)
          throw new Exception("Attempted to register actor with null ActorTypeInfo!");

        try
        {
          mu_counter.WaitOne();
          counter = emptycounter;
          while (counter < list.Length && (list[counter] != null && !list[counter].Disposed))
            counter++;

          if (counter >= list.Length)
            throw new Exception("The list of Actors has exceeded capacity!");

          int i = counter;
          emptycounter = i + 1;

          if (list[i] == null)
          {
            actor = new ActorInfo(this, i, amake);
            list[i] = actor;
          }
          else
          {
            actor = list[i];
            actor.ID += capacity;
            actor.Rebuild(amake);
          }

          if (First < 0)
            First = actor.ID;
          else
          {
            Get(Last).NextID = actor.ID;
            actor.PrevID = Last;
          }
          Last = actor.ID;
          count++;
        }
        catch (Exception ex)
        { throw ex; }
        finally
        {
          mu_counter.ReleaseMutex();
        }
        return actor;
      }

      public void ActivatePlanned()
      {
        foreach (ActorInfo a in list)
          if (a != null)
            if (a.Planned && a.CreationTime < Engine.Game.GameTime)
              a.Initialize();
      }

      public void MakeDead(ActorInfo a)
      {
        deadqueue.Enqueue(a);
      }

      public void DestroyDead()
      {
        ActorInfo a;
        while (deadqueue.TryDequeue(out a))
          a.Destroy();
      }

      public int GetActorCount()
      {
        return count;
      }

      public void DoEach(Action<Engine, int> action)
      {
        ActorInfo actor = Get(First);
        int freezecount = count;
        for (int i = 0; i < freezecount && actor != null; i++)
        {
          action.Invoke(Engine, actor.ID);

          if (Get(actor.NextID) == null && i < count - 1)
          { }

          actor = Get(actor.NextID);
        }
      }

      public int GetIndex(int id)
      {
        return id % capacity;
      }

      public ActorInfo Get(int id)
      {
        if (id < 0)
          return null;
        ActorInfo a = list[id % capacity];
        if (a != null && id == a.ID)
          return a;
        return null;
      }

      public bool Exists(int id)
      {
        return id >= 0 && list[id % capacity] != null && list[id % capacity].ID == id;
      }

      public bool IsPlayer(int id)
      {
        return id == Engine.PlayerInfo.Actor?.ID;
      }

      public void Remove(int id)
      {
        int x = id % capacity;

        //if (list[x]?.CreationState != CreationState.DISPOSED)
        //{
          try
          {
            mu_counter.WaitOne();

            count--;

            ActorInfo actor = Get(id);
            if (First == id && Last == id)
            {
              First = -1;
              Last = -1;
            }
            else if (First == id)
            {
              First = actor.NextID;
            }
            else if (Last == id)
            {
              Last = actor.PrevID;
            }
            else
            {
              Get(actor.PrevID).NextID = actor.NextID;
              Get(actor.NextID).PrevID = actor.PrevID;
            }

            if (x < emptycounter)
              emptycounter = x;
          }
          catch (Exception ex)
          { throw ex; }
          finally
          {
            mu_counter.ReleaseMutex();
          }
        //}
      }

      public void Reset()
      {
        DestroyDead();
        for (int i = 0; i < list.Length; i++)
        {
          list[i]?.Destroy();
          list[i] = null;
        }
        First = -1;
        Last = -1;
      }
    }
    */
  }
}
