using MTV3D65;
using System.Collections.Generic;

namespace SWEndor
{
  public static class ActorDistanceInfo
  {
    private static ThreadSafeDictionary<long, KeyValuePair<float, float>> list = new ThreadSafeDictionary<long, KeyValuePair<float, float>>() { ExplicitUpdateOnly = true };
    private static float clearlisttime = 0;

    public static float GetRoughDistance(TV_3DVECTOR v1, TV_3DVECTOR v2)
    {
      float dx = v1.x - v2.x;
      float dy = v1.y - v2.y;
      float dz = v1.z - v2.z;
      if (dx < 0)
        dx = -dx;
      if (dy < 0)
        dy = -dy;
      if (dz < 0)
        dz = -dz;

      return dx + dy + dz;
    }

    /// <summary>
    /// Gets a higher bound on the distance between Actors by taking the sum of the coordinate deltas.
    /// </summary>
    /// <returns></returns>
    public static float GetRoughDistance(ActorInfo a1, ActorInfo a2)
    {
      TV_3DVECTOR a1v = a1.GetPosition();
      TV_3DVECTOR a2v = a2.GetPosition();
      float dx = a1v.x - a2v.x;
      float dy = a1v.y - a2v.y;
      float dz = a1v.z - a2v.z;
      if (dx < 0)
        dx = -dx;
      if (dy < 0)
        dy = -dy;
      if (dz < 0)
        dz = -dz;

      return dx + dy + dz;
    }

    public static float GetDistance(ActorInfo a1, ActorInfo a2, float limit)
    {

      float d = GetRoughDistance(a1, a2);
      if (d > limit)
        return limit;
      else
        return GetDistance(a1, a2);
    }

    public static float GetDistance(ActorInfo a1, ActorInfo a2)
    {
      using (new PerfElement("fn_getactordistance"))
      {
        if (a1 == a2)
          return 0;

        ActorInfo first;
        ActorInfo second;

        if (a1.ID < a2.ID)
        {
          first = a1;
          second = a2;
        }
        else
        {
          first = a2;
          second = a1;
        }

        if (clearlisttime < Game.Instance().GameTime)
        {
          list.SetDirty();
          list.ClearList();
          clearlisttime = Game.Instance().GameTime + 5;
        }

        long hash = (uint)(first.ID << sizeof(int)) | (uint)second.ID;
        float dist;
        KeyValuePair<float, float> kvp = list.GetItem(hash);
        if (kvp.Key < Game.Instance().GameTime)
        {
          dist = CalculateDistance(first, second);
          list.AddorUpdateItem(hash, new KeyValuePair<float, float>(Game.Instance().GameTime, dist));
          //list.SetDirty();
        }
        else
        {
          dist = kvp.Value;
        }

        return dist;
      }
    }

    public static float CalculateDistance(ActorInfo first, ActorInfo second)
    {
      return Engine.Instance().TVMathLibrary.GetDistanceVec3D(first.GetPosition(), second.GetPosition());
    }
  }

  /*
  public static class ActorDistanceInfo
  {
    private static Dictionary<int, Dictionary<int, KeyValuePair<float,float>>> m_distancelist = new Dictionary<int, Dictionary<int, KeyValuePair<float, float>>>();
    private static float clearlisttime = 0;
    private static Mutex mu_distancelist = new Mutex();

    public static float GetDistance(ActorInfo a1, ActorInfo a2)
    {
      using (new PerfElement("fn_getactordistance"))
      {
        if (a1 == a2)
          return 0;

        ActorInfo first;
        ActorInfo second;

        if (a1.ID < a2.ID)
        {
          first = a1;
          second = a2;
        }
        else
        {
          first = a2;
          second = a1;
        }

        mu_distancelist.WaitOne();
        if (clearlisttime < Game.Instance().Time)
        {
          m_distancelist.Clear();
          clearlisttime = Game.Instance().Time + 15f;
        }


        bool generate = (!m_distancelist.ContainsKey(first.ID) || !m_distancelist[first.ID].ContainsKey(second.ID) || m_distancelist[first.ID][second.ID].Key < Game.Instance().Time);

        if (generate)
        {
          if (!m_distancelist.ContainsKey(first.ID))
            m_distancelist.Add(first.ID, new Dictionary<int, KeyValuePair<float, float>>());

          if (!m_distancelist[first.ID].ContainsKey(second.ID))
            m_distancelist[first.ID].Add(second.ID, new KeyValuePair<float, float>());

          m_distancelist[first.ID][second.ID] = new KeyValuePair<float, float>(Game.Instance().Time, Engine.Instance().TVMathLibrary.GetDistanceVec3D(first.GetPosition(), second.GetPosition()));
        }
        float ret = m_distancelist[first.ID][second.ID].Value;
        mu_distancelist.ReleaseMutex();

        return ret;
      }
    }
  }
*/
}
