using MTV3D65;
using SWEndor.Primitives;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public static class ActorDistanceInfo
  {
    private static ThreadSafeDictionary<long, CachedFloat> list = new ThreadSafeDictionary<long, CachedFloat>() { ExplicitUpdateOnly = true };
    private static float Cleartime = 0;

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

        if (Cleartime < Game.Instance().GameTime)
        {
          list.Clear();
          list.Refresh();
          Cleartime = Game.Instance().GameTime + 5;
        }

        long hash = (uint)(first.ID << sizeof(int)) | (uint)second.ID;
        float dist;
        CachedFloat cached = list.Get(hash);
        if (cached.Time < Game.Instance().GameTime)
        {
          dist = CalculateDistance(first, second);
          list.Put(hash, new CachedFloat() { Time = Game.Instance().GameTime, Value = dist });
        }
        else
        {
          dist = cached.Value;
        }

        return dist;
      }
    }

    public static float CalculateDistance(ActorInfo first, ActorInfo second)
    {
      return Engine.Instance().TVMathLibrary.GetDistanceVec3D(first.GetPosition(), second.GetPosition());
    }
  }
}
