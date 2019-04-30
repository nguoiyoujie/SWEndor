using SWEndor.ActorTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public class Factory
    {
      public readonly Engine Engine;
      internal Factory(Engine engine)
      { Engine = engine; }

      private const int Capacity = 2500; // hard code limit to ActorInfo. We should not be exceeding 1000 normally.
      private ConcurrentQueue<ActorInfo> deadqueue = new ConcurrentQueue<ActorInfo>();
      private ActorInfo[] list = new ActorInfo[Capacity];
      private int count = 0;
      private int[] holdinglist = new int[0];
      private float listtime = 0;
      private int counter = 0;
      private int emptycounter = 0;
      private Mutex mu_counter = new Mutex();

      public ActorInfo Register(ActorCreationInfo amake, string key = "")
      {
        ActorInfo actor = null;
        if (amake.ActorTypeInfo == null)
          throw new Exception("Attempted to register actor with null ActorTypeInfo!");

        mu_counter.WaitOne();

        counter = emptycounter;
        while (counter < list.Length && (list[counter] != null && list[counter].CreationState != CreationState.DISPOSED))
          counter++;

        if (counter >= list.Length)
          throw new Exception("The list of ActorInfo has exceeded capacity!");

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
          actor.ID += Capacity;
          actor.Rebuild(amake);
        }
        count++;

        mu_counter.ReleaseMutex();
        return actor;
      }

      public void ActivatePlanned()
      {
        foreach (ActorInfo a in list)
          if (a != null)
            if (a.CreationState == CreationState.PLANNED && a.CreationTime < Engine.Game.GameTime)
              a.Generate();
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

      public int[] GetList()
      {
        if (listtime < Engine.Game.GameTime)
        {
          List<int> hl = new List<int>();
          foreach (ActorInfo a in list)
            if (a != null && a.CreationState != CreationState.DISPOSED)
              hl.Add(a.ID);
          holdinglist = hl.ToArray();
          listtime = Engine.Game.GameTime;
        }
        return holdinglist;
      }

      public int[] GetHoldingList()
      {
        return holdinglist;
      }

      public ActorInfo Get(int id)
      {
        if (id < 0)
          return null;
        ActorInfo a = list[id % Capacity];
        if (a != null && id == a.ID)
          return a;
        return null;
      }

      public bool Exists(int id)
      {
        return id >= 0 && list[id % Capacity] != null && list[id % Capacity].ID == id;
      }

      public bool IsPlayer(int id)
      {
        return id == Engine.PlayerInfo.Actor?.ID;
      }

      public void Remove(int id)
      {
        int x = id % Capacity;

        // don't remove, keep future reuse. But reduce counters
        if (list[x]?.CreationState != CreationState.DISPOSED)
        {
          mu_counter.WaitOne();
          count--;
          if (x < emptycounter)
            emptycounter = x;
          mu_counter.ReleaseMutex();
        }
      }

      public void Reset()
      {
        DestroyDead();
        for (int i = 0; i < list.Length; i++)
        {
          list[i]?.Destroy();
          list[i] = null;
        }
      }
    }
  }
}
