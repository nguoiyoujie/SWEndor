using SWEndor.ActorTypes;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public static class Factory
    {
      private static ActorInfo[] list = new ActorInfo[10000]; // hard code limit to ActorInfo. We should not be exceeding 1000 normally.
      private static int count = 0;
      private static ActorInfo[] holdinglist = new ActorInfo[0];
      private static float listtime = 0;
      private static int counter = 0;
      private static int emptycounter = 0;
      private static Mutex mu_counter = new Mutex();

      public static int Register(ActorCreationInfo amake, out ActorInfo actor, string key = "")
      {
        if (amake.ActorTypeInfo == null)
          throw new Exception("Attempted to register actor with null ActorTypeInfo!");

        mu_counter.WaitOne();

        counter = emptycounter;
        while (counter < list.Length && list[counter] != null)
          counter++;

        if (counter >= list.Length)
          throw new Exception("The list of ActorInfo has exceeded capacity!");

        int i = counter;
        emptycounter = i + 1;
        actor = new ActorInfo(i, amake);

        list[i] = actor;
        count++;

        mu_counter.ReleaseMutex();

        return i;
      }

      // Why query the list again?
      public static void ActivatePlanned()
      {
        foreach (ActorInfo a in list)
          if (a != null)
            if (a.CreationState == CreationState.PLANNED && a.CreationTime < Game.Instance().GameTime)
              a.Generate();
      }

      public static int GetActorCount()
      {
        return count;
      }

      public static ActorInfo[] GetActorList()
      {
        using (new PerfElement("fn_getactorlist"))
        {
          if (listtime < Game.Instance().GameTime)
          {
            List<ActorInfo> hl = new List<ActorInfo>();
            foreach (ActorInfo a in list)
              if (a != null)
                hl.Add(a);
            holdinglist = hl.ToArray();
            listtime = Game.Instance().GameTime;
          }
          return holdinglist;
        }
      }

      public static ActorInfo[] GetHoldingList()
      {
        return holdinglist;
      }

      public static ActorInfo GetActor(int id)
      {
        if (id < 0)
          return null;
        return list[id];
      }

      public static void RemoveActor(int id)
      {
        list[id] = null;
        count--;
        if (id < emptycounter)
          emptycounter = id;
      }
    }
  }
}
