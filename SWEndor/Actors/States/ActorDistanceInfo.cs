using MTV3D65;
using SWEndor.Core;
using SWEndor.Primitives;
using System;
using System.Collections.Generic;

namespace SWEndor.Actors
{
  public struct int2
  {
    public int Value1;
    public int Value2;

    public int2(int v1, int v2)
    {
      Value1 = v1;
      Value2 = v2;
    }
  }

  public static class ActorDistanceInfo
  {
    private struct EngineTime
    {
      public Engine Engine;
      public float Time;

      public EngineTime(Engine e)
      {
        Engine = e;
        Time = e.Game.GameTime;
      }

      public class EqualityComparer : IEqualityComparer<EngineTime>
      {
        public static EqualityComparer Instance = new EqualityComparer();
        public bool Equals(EngineTime a, EngineTime b)
        {
          return a.Time == b.Time;
        }

        public int GetHashCode(EngineTime o)
        {
          return o.GetHashCode();
        }
      }

      public static bool operator ==(EngineTime a, EngineTime b)
      {
        return a.Time == b.Time;
      }
      public static bool operator !=(EngineTime a, EngineTime b)
      {
        return !(a.Time == b.Time);
      }
      public override bool Equals(object obj)
      {
        return obj is EngineTime && Time == ((EngineTime)obj).Time;
      }
      public override int GetHashCode()
      {
        return Time.GetHashCode();
      }

      public bool Passed { get { return Engine == null || Time < Engine.Game.GameTime; } }
    }

    private static Cache<long, EngineTime, float, ActorInfo, ActorInfo> cache = new Cache<long, EngineTime, float, ActorInfo, ActorInfo>(8192); 
    private static float Cleartime = 0;
    private static Func<EngineTime, bool> clearfunc = (f) => { return f.Passed; };
    private static Func<ActorInfo, ActorInfo, float> dofunc = (a, b) => { return CalculateDistance(a, b); };

    private static object locker = new object();

    public static void Reset()
    {
      Cleartime = 0;
    }

    public static int CacheCount
    {
      get
      {
        return cache.Count;
      }
    }

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

    public static float GetDistance(Engine e, ActorInfo a1, ActorInfo a2, float limit)
    {
      float d = GetRoughDistance(a1, a2);
      if (d > limit)
        return limit;
      else
        return GetDistance(e, a1, a2);
    }

    public static float GetDistance(Engine e, ActorInfo a1, ActorInfo a2)
    {
      if (a1 == null || a2 == null)
        return float.MaxValue;

      if (a1 == a2)
        return 0;

      lock (locker)
      {
        if (Cleartime < e.Game.GameTime)
        {
          cache.Clear(clearfunc);
          Cleartime = e.Game.GameTime + 5;
        }

        long hash;
        if (a1.ID > a2.ID)
        {
          hash = a1.ID;
          hash = hash << 32;
          hash += a2.ID;
        }
        else
        {
          hash = a2.ID;
          hash = hash << 32;
          hash += a1.ID;
        }
        return cache.GetOrDefine(hash, new EngineTime(e), dofunc, a1, a2, EngineTime.EqualityComparer.Instance);
      }
    }

    public static float GetDistance(TV_3DVECTOR first, TV_3DVECTOR second)
    {
      return CalculateDistance(first, second);
    }

    private static float CalculateDistance(ActorInfo first, ActorInfo second)
    {
      return CalculateDistance(first.GetGlobalPosition(), second.GetGlobalPosition());
    }

    private static float CalculateDistance(TV_3DVECTOR first, TV_3DVECTOR second)
    {
      return Globals.Engine.TrueVision.TVMathLibrary.GetDistanceVec3D(first, second);
    }

    public static TV_3DVECTOR Lerp(Engine e, TV_3DVECTOR first, TV_3DVECTOR second, float frac)
    {
      TV_3DVECTOR ret = new TV_3DVECTOR();
      e.TrueVision.TVMathLibrary.TVVec3Lerp(ref ret, first, second, frac);
      return ret;
    }
  }
}
