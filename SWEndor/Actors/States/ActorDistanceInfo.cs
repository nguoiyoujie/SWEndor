using MTV3D65;
using SWEndor.Primitives;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public static class ActorDistanceInfo
  {
    private static Dictionary<int, TimeCache<float>> list = new Dictionary<int, TimeCache<float>>(); //{ ExplicitUpdateOnly = true };
    private static float Cleartime = 0;
    private static object locker = new object();

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
      if (a1 == null || a2 == null)
        return float.MaxValue;

      if (a1 == a2)
        return 0;

      TV_3DVECTOR a1v = a1.GetGlobalPosition();
      TV_3DVECTOR a2v = a2.GetGlobalPosition();
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
      if (a1 == null || a2 == null)
        return float.MaxValue;

      if (a1 == a2)
        return 0;

      if (Cleartime < Globals.Engine.Game.GameTime)
      {
        list.Clear();
        Cleartime = Globals.Engine.Game.GameTime + 5;
      }

      int hash = (a1.ID < a2.ID) ? a1.ID * Globals.ActorLimit + a2.ID : a2.ID * Globals.ActorLimit + a1.ID;

      Func<float> fn = () => CalculateDistance(a1, a2);

      lock (locker)
      {
        if (!list.ContainsKey(hash))
          //list[hash].Set(Globals.Engine.Game.GameTime, fn);
        //else
          list.Add(hash, new TimeCache<float>(Globals.Engine.Game.GameTime, fn));

        return list[hash].Get(Globals.Engine.Game.GameTime);
      }
    }

    public static float CalculateDistance(ActorInfo first, ActorInfo second)
    {
      return first.GetEngine().TrueVision.TVMathLibrary.GetDistanceVec3D(first.GetGlobalPosition(), second.GetGlobalPosition());
    }
  }
}
