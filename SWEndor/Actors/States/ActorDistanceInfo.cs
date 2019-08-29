using MTV3D65;
using SWEndor.Primitives;
using System;

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
    private static Cache<long, float, float, ActorInfo, ActorInfo> cache = new Cache<long, float, float, ActorInfo, ActorInfo>(); 
    private static float Cleartime = 0;
    private static Func<float, bool> clearfunc = (f) => { return f < Globals.Engine.Game.GameTime; };
    private static Func<ActorInfo, ActorInfo, float> dofunc = (a, b) => { return CalculateDistance(a, b); };

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

      lock (locker)
      {
        if (Cleartime < Globals.Engine.Game.GameTime)
        {
          cache.Clear(clearfunc);
          Cleartime = Globals.Engine.Game.GameTime + 5;
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
        return cache.GetOrDefine(hash, Globals.Engine.Game.GameTime, dofunc, a1, a2);
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

    public static TV_3DVECTOR Lerp(TV_3DVECTOR first, TV_3DVECTOR second, float frac)
    {
      TV_3DVECTOR ret = new TV_3DVECTOR();
      Globals.Engine.TrueVision.TVMathLibrary.TVVec3Lerp(ref ret, first, second, frac);
      return ret;
    }
  }
}
