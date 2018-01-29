using MTV3D65;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SWEndor
{
  public enum CreationState { PLANNED, PREACTIVE, ACTIVE, DISPOSED }
  public enum ActorState { FIXED, FREE, HYPERSPACE, NORMAL, DYING, DEAD }
  public enum OrientationMode { ROTATION, DIRECTION }

  public class Orientation
  {
    public OrientationMode Mode = OrientationMode.ROTATION;
    public TV_3DVECTOR Vector = new TV_3DVECTOR();
  }

  public class ActorCreationInfo
  {
    public float CreationTime;

    public FactionInfo Faction = FactionInfo.Neutral;
    public ActorTypeInfo ActorTypeInfo;
    public string Name;
    public CreationState CreationState;
    public ActorState InitialState;
    public TV_3DVECTOR InitialScale;
    public TV_3DVECTOR Position;
    public TV_3DVECTOR Rotation;

    //public Orientation Orientation;

    public float InitialStrength;
    public float InitialSpeed;

    private ActorCreationInfo()
    { }

    public ActorCreationInfo(ActorTypeInfo at)
    {
      // Load defaults from actortype
      ActorTypeInfo = at;
      Name = at.Name;
      CreationState = CreationState.PLANNED;
      InitialState = ActorState.NORMAL;
      Position = new TV_3DVECTOR();
      Rotation = new TV_3DVECTOR();
      InitialScale = new TV_3DVECTOR(1, 1, 1);

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

    ThreadSafeDictionary<int, ActorInfo> list = new ThreadSafeDictionary<int, ActorInfo>() { ExplicitUpdateOnly = true };
    float listtime = 0;
    private int counter = 0;
    private Mutex mu_counter = new Mutex();


    public int Register(ActorCreationInfo amake, out ActorInfo actor)
    {
      if (amake.ActorTypeInfo == null)
        throw new Exception("Attempted to register actor with null ActorTypeInfo!");

      mu_counter.WaitOne();
      int i = counter++;
      actor = ActorInfo.FactoryCreate(i, amake);
      list.AddItem(i, actor);
      mu_counter.ReleaseMutex();

      return i;
    }

    // Why query the list again?
    public void ActivatePlanned()
    {
      foreach (ActorInfo a in list.GetValues())
      {
        if (a.CreationState == CreationState.PLANNED && a.CreationTime < Game.Instance().GameTime)
        {
          a.Generate();
        }
      }
    }

    public ActorInfo[] GetActorList()
    {
      using (new PerfElement("fn_getactorlist"))
      {
        if (listtime < Game.Instance().GameTime)
        {
          list.SetDirty();
          listtime = Game.Instance().GameTime;
        }

        return list.GetValues();
      }
    }

    public ActorInfo GetActor(int id)
    {
      return list.GetItem(id);
    }

    public void RemoveActor(int id)
    {
      list.RemoveItem(id);
    }

  }
}
