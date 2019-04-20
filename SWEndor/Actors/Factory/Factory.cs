using SWEndor.ActorTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public static class Factory
    {
      private const int Capacity = 2500; // hard code limit to ActorInfo. We should not be exceeding 1000 normally.
      private static ConcurrentQueue<ActorInfo> deadqueue = new ConcurrentQueue<ActorInfo>();
      private static ActorInfo[] list = new ActorInfo[Capacity];
      private static int count = 0;
      private static int[] holdinglist = new int[0];
      private static float listtime = 0;
      private static int counter = 0;
      private static int emptycounter = 0;
      private static Mutex mu_counter = new Mutex();

      public static ActorInfo Register(ActorCreationInfo amake, string key = "")
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
          actor = new ActorInfo(i, amake);
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

      public static void ActivatePlanned()
      {
        foreach (ActorInfo a in list)
          if (a != null)
            if (a.CreationState == CreationState.PLANNED && a.CreationTime < Game.Instance().GameTime)
              a.Generate();
      }

      public static void MakeDead(ActorInfo a)
      {
        deadqueue.Enqueue(a);
      }

      public static void DestroyDead()
      {
        ActorInfo a;
        while (deadqueue.TryDequeue(out a))
          a.Destroy();
      }

      public static int GetActorCount()
      {
        return count;
      }

      public static int[] GetList()
      {
        if (listtime < Game.Instance().GameTime)
        {
          List<int> hl = new List<int>();
          foreach (ActorInfo a in list)
            if (a != null && a.CreationState != CreationState.DISPOSED)
              hl.Add(a.ID);
          holdinglist = hl.ToArray();
          listtime = Game.Instance().GameTime;
        }
        return holdinglist;
      }

      public static int[] GetHoldingList()
      {
        return holdinglist;
      }

      public static ActorInfo Get(int id)
      {
        if (id < 0)
          return null;
        ActorInfo a = list[id % Capacity];
        if (a != null && id == a.ID)
          return a;
        return null;
      }

      public static bool Exists(int id)
      {
        return id >= 0 && list[id % Capacity] != null && list[id % Capacity].ID == id;
      }

      public static bool IsPlayer(int id)
      {
        return id == Player.PlayerInfo.Instance().Actor?.ID;
      }

      public static void Remove(int id)
      {
        int x = id % Capacity;

        /*
        list[x] = null;
        count--;
        if (x < emptycounter)
          emptycounter = x;
        */

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

      public static void Reset()
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
