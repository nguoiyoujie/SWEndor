using MTV3D65;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SWEndor.Actors
{
  public class ActorCreationInfo
  {
    public float CreationTime;

    public FactionInfo Faction = FactionInfo.Neutral;
    public ActorTypeInfo ActorTypeInfo;
    public string Name;
    public CreationState CreationState = CreationState.PLANNED;
    public ActorState InitialState = ActorState.NORMAL;
    public TV_3DVECTOR InitialScale = new TV_3DVECTOR(1, 1, 1);
    public TV_3DVECTOR Position = new TV_3DVECTOR();
    public TV_3DVECTOR Rotation = new TV_3DVECTOR();

    public float InitialStrength;
    public float InitialSpeed;

    private ActorCreationInfo()
    { }

    public ActorCreationInfo(ActorTypeInfo at)
    {
      // Load defaults from actortype
      ActorTypeInfo = at;
      Name = at.Name;
      InitialStrength = at.MaxStrength;
      InitialSpeed = at.MaxSpeed;
    }
  }

  public class ActorFactory
  {
    private static ActorFactory _instance;
    public static ActorFactory Instance()
    {
      if (_instance == null) { _instance = new ActorFactory(); }
      return _instance;
    }

    private ActorFactory()
    {
    }

    private ActorInfo[] list = new ActorInfo[10000];
    private int count = 0;
    //private ThreadSafeDictionary<int, ActorInfo> list = new ThreadSafeDictionary<int, ActorInfo>() { ExplicitUpdateOnly = true };
    private ActorInfo[] holdinglist = new ActorInfo[0];
    private float listtime = 0;
    private int counter = 0;
    private int emptycounter = 0;
    private Mutex mu_counter = new Mutex();


    public int Register(ActorCreationInfo amake, out ActorInfo actor, string key = "")
    {
      if (amake.ActorTypeInfo == null)
        throw new Exception("Attempted to register actor with null ActorTypeInfo!");

      mu_counter.WaitOne();
      //if (counter >= 10000)
        counter = emptycounter;
      while (counter < list.Length && list[counter] != null)
        counter++;

      if (counter >= list.Length)
        throw new Exception("The list of ActorInfo has exceeded capacity!");

      int i = counter;
      emptycounter++;
      actor = ActorInfo.FactoryCreate(i, amake);

      list[i] = actor;
      count++; 
      //list.Add(i, actor);
      //list.Refresh();

      mu_counter.ReleaseMutex();

      return i;
    }

    // Why query the list again?
    public void ActivatePlanned()
    {
      foreach (ActorInfo a in list)//.GetValues())
      {
        if (a != null)
          if (a.CreationState == CreationState.PLANNED && a.CreationTime < Game.Instance().GameTime)
          {
            a.Generate();
          }
      }
    }

    public int GetActorCount()
    {
      return count;
      //return list.Count;
    }

    public ActorInfo[] GetActorList()
    {
      using (new PerfElement("fn_getactorlist"))
      {
        if (listtime < Game.Instance().GameTime)
        {
          //list.Refresh();
          //holdinglist = list.GetValues();

          List<ActorInfo> hl = new List<ActorInfo>();
          foreach (ActorInfo a in list)//.GetValues())
          {
            if (a != null)
              hl.Add(a);
          }
          holdinglist = hl.ToArray();
          listtime = Game.Instance().GameTime;
        }
        return holdinglist;
      }
    }

    public ActorInfo[] GetHoldingList()
    {
      return holdinglist;
    }

    public ActorInfo GetActor(int id)
    {
      if (id < 0)
        return null;
      return list[id];
      //return list.Get(id);
    }

    public void RemoveActor(int id)
    {
      list[id] = null;
      count--;
      if (id < emptycounter)
        emptycounter = id;
      //list.Remove(id);
    }

  }
}
