using System.Collections.Generic;

namespace SWEndor
{
  public delegate void GameEvent(params object[] parameters);

  public class GameEventQueue
  {
    private static ThreadSafeDictionary<float, GameEvent> list = new ThreadSafeDictionary<float, GameEvent>();

    public static void Add(float time, GameEvent method)
    {
      // should replace this with more efficient checking?
      while (list.ContainsKey(time))
        time += 0.01f;

      if (method != null)
        list.Add(time, method);
    }

    public static void Clear()
    {
      list.Clear();
    }

    public static void Process()
    {
      List<float> remove = new List<float>();

      float[] gekeys = new float[list.Count];
      list.GetKeys().CopyTo(gekeys, 0);
      for (int i = 0; i < gekeys.Length; i++)
      {
        float time = gekeys[i];
        GameEvent ev = list[time];
        if (ev == null || time < Game.Instance().GameTime)
        {
          remove.Add(time);
          ev?.Invoke();
        }
      }

      foreach (float f in remove)
        list.Remove(f);
    }
  }
}
