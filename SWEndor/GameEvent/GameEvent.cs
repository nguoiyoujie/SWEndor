using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Primitives;
using SWEndor.Scenarios;
using System.Collections.Generic;

namespace SWEndor
{
  public class GameEventArg {}
  public class IntegerEventArg : GameEventArg
  {
    public static IntegerEventArg N0 = new IntegerEventArg(0);
    public static IntegerEventArg N1 = new IntegerEventArg(1);
    public static IntegerEventArg N2 = new IntegerEventArg(2);
    public static IntegerEventArg N3 = new IntegerEventArg(3);
    public static IntegerEventArg N4 = new IntegerEventArg(4);
    public static IntegerEventArg N5 = new IntegerEventArg(5);
    public static IntegerEventArg N6 = new IntegerEventArg(6);
    public static IntegerEventArg N7 = new IntegerEventArg(7);
    public static IntegerEventArg N8 = new IntegerEventArg(8);
    public static IntegerEventArg N9 = new IntegerEventArg(9);
    public static IntegerEventArg N10 = new IntegerEventArg(10);
    public readonly int Num;

    public IntegerEventArg(int num)
    {
      Num = num;
    }
  }

  public class ActorEventArg : GameEventArg
  {
    public readonly int ActorID;

    public ActorEventArg(int actorID)
    {
      ActorID = actorID;
    }
  }

  public class HitEventArg : ActorEventArg
  {
    public readonly int VictimID;

    public HitEventArg(int actorID, int victimID) : base(actorID)
    {
      VictimID = victimID;
    }
  }

  public class ActorStateChangeEventArg : ActorEventArg
  {
    public readonly ActorState State;

    public ActorStateChangeEventArg(int actorID, ActorState state) : base(actorID)
    {
      State = state;
    }
  }

  public class ShipSpawnEventArg : GameEventArg
  {
    public readonly GSFunctions.ShipSpawnInfo Info;
    public readonly TV_3DVECTOR Position;
    public readonly TV_3DVECTOR TargetPosition;
    public readonly TV_3DVECTOR FacingPosition;

    public ShipSpawnEventArg(GSFunctions.ShipSpawnInfo info, TV_3DVECTOR position, TV_3DVECTOR targetposition, TV_3DVECTOR facingposition)
    {
      Info = info;
      Position = position;
      TargetPosition = targetposition;
      FacingPosition = facingposition;
    }
  }



  public delegate void GameEvent(GameEventArg eventArg);
  public delegate void ActorEvent(ActorEventArg eventArg);
  public delegate void HitEvent(HitEventArg eventArg);
  public delegate void IntegerEvent(IntegerEventArg eventArg);
  public delegate void ActorStateChangeEvent(ActorStateChangeEventArg eventArg);
  public delegate void ShipSpawnEvent(ShipSpawnEventArg eventArg);


  public class GameEventQueue
  {
    private static ThreadSafeDictionary<float, GameEventObject> list = new ThreadSafeDictionary<float, GameEventObject>();

    private struct GameEventObject
    {
      internal readonly GameEvent Method;
      internal readonly GameEventArg Arg;

      internal GameEventObject(GameEvent gameEvent, GameEventArg arg)
      {
        Method = gameEvent;
        Arg = arg;
      }
    }

    public static void Add(float time, GameEvent method, GameEventArg arg)
    {
      // should replace this with more efficient checking?
      while (list.ContainsKey(time))
        time += 0.01f;

      if (method != null)
        list.Add(time, new GameEventObject(method, arg));
    }

    public static void Clear()
    {
      list.Clear();
    }

    public static void Process(Engine engine)
    {
      List<float> remove = new List<float>();

      //float[] gekeys = new float[list.Count];
      //list.GetKeys().CopyTo(gekeys, 0);
      foreach (KeyValuePair<float, GameEventObject> t in list.GetList()) //(int i = 0; i < list.GetKeys().Length; i++)
      {
        float time = t.Key;  //gekeys[i];
        GameEventObject ev = t.Value; //list[time];
        if (ev.Method == null || time < engine.Game.GameTime)
        {
          remove.Add(time);
          ev.Method?.Invoke(ev.Arg);
        }
      }

      foreach (float f in remove)
        list.Remove(f);
    }
  }
}
