using SWEndor.Primitives;
using System.Collections.Generic;

namespace SWEndor
{
  public delegate void GameEvent(params object[] parameters);
    
  public class GameEventQueue
  {
    private static ThreadSafeDictionary<float, GameEventObject> list = new ThreadSafeDictionary<float, GameEventObject>();

    private struct GameEventObject
    {
      internal readonly GameEvent Method;
      internal readonly object[] Parameters;

      internal GameEventObject(GameEvent gameEvent,  params object[] param)
      {
        Method = gameEvent;
        Parameters = param;
      }
    }

    public static void Add(float time, GameEvent method, params object[] param)
    {
      // should replace this with more efficient checking?
      while (list.ContainsKey(time))
        time += 0.01f;

      if (method != null)
        list.Add(time, new GameEventObject(method, param));
    }

    public static void Clear()
    {
      list.Clear();
    }

    public static void Process(Engine engine)
    {
      List<float> remove = new List<float>();

      float[] gekeys = new float[list.Count];
      list.GetKeys().CopyTo(gekeys, 0);
      for (int i = 0; i < gekeys.Length; i++)
      {
        float time = gekeys[i];
        GameEventObject ev = list[time];
        if (ev.Method == null || time < engine.Game.GameTime)
        {
          remove.Add(time);
          ev.Method?.Invoke(ev.Parameters);
        }
      }

      foreach (float f in remove)
        list.Remove(f);
    }
  }
}
