using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using Primrose.Primitives;
using SWEndor.Game.Projectiles;
using System;
using System.Collections.Generic;
using Primrose.Primitives.ValueTypes;
using Primrose.Expressions;

namespace SWEndor.Game.Models
{
  /// <summary>
  /// Provides implementation for calculating distances and caching the result
  /// </summary>
  public static class DistanceModel
  {
    private struct EngineTime : IEquatable<EngineTime>
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
        public bool Equals(EngineTime a, EngineTime b) { return a.Time == b.Time; }
        public int GetHashCode(EngineTime o) { return o.GetHashCode(); }
      }

      public static bool operator ==(EngineTime a, EngineTime b) { return a.Time == b.Time; }
      public static bool operator !=(EngineTime a, EngineTime b) { return !(a.Time == b.Time); }
      public override bool Equals(object obj) { return obj is EngineTime time && Time == time.Time; }
      public bool Equals(EngineTime time) { return Time == time.Time; }
      public override int GetHashCode() { return Time.GetHashCode(); }

      public bool Passed { get { return Engine == null || Time < Engine.Game.GameTime; } }
    }

    private static readonly Cache<int, EngineTime, float, Trip<TVMathLibrary, ITransformable, ITransformable>> cache = new Cache<int, EngineTime, float, Trip<TVMathLibrary, ITransformable, ITransformable>>(8192); 
    private static float Cleartime = 0;
    private static readonly Func<EngineTime, bool> clearfunc = (f) => { return f.Passed; };
    private static readonly Func<Trip<TVMathLibrary, ITransformable, ITransformable>, float> dofunc = (o) => { return CalculateDistance(o.t, o.u, o.v); };

    private static readonly object locker = new object();

    /// <summary>Marks the cache to allow clearing</summary>
    public static void Reset() { Cleartime = 0; }

    /// <summary>Returns the number of cached results</summary>
    public static int CacheCount { get { return cache.Count; } }

    /// <summary>
    /// Gets a higher bound on the distance between Actors by taking the Manhattan distance.
    /// </summary>
    /// <returns></returns>
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
    /// Gets a higher bound on the distance between Actors by taking the Manhattan distance. This result is not cached.
    /// </summary>
    /// <param name="a1">The first object</param>
    /// <param name="a2">The second object</param>
    /// <returns>The Manhattan distance between the two objects</returns>
    public static float GetRoughDistance(ITransformable a1, ITransformable a2)
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

    /// <summary>
    /// Gets the distance between two objects, up to a limit. Returns the distance or limit
    /// </summary>
    /// <typeparam name="T1">The type of the first transformable</typeparam>
    /// <typeparam name="T2">The type of the second transformable</typeparam>
    /// <param name="e">The game engine</param>
    /// <param name="a1">The first object</param>
    /// <param name="a2">The second object</param>
    /// <param name="limit">The maximum returned value</param>
    /// <returns>The distance between two objects, or limit if the distance exceeds the limit</returns>
    public static float GetDistance<T1, T2>(Engine e, T1 a1, T2 a2, float limit)
      where T1 : IEngineObject, IIdentity<int>, ITransformable
      where T2 : IEngineObject, IIdentity<int>, ITransformable
    {
      float d = GetRoughDistance(a1, a2);
      if (d > limit)
        return limit;
      else
        return GetDistance(e, a1, a2);
    }

    /// <summary>
    /// Gets the distance between two objects, up to a limit. Returns the distance or limit
    /// </summary>
    /// <typeparam name="T1">The type of the first transformable</typeparam>
    /// <typeparam name="T2">The type of the second transformable</typeparam>
    /// <param name="e">The game engine</param>
    /// <param name="a1">The first object</param>
    /// <param name="a2">The second object</param>
    /// <returns>The distance between two objects</returns>
    public static float GetDistance<T1, T2>(Engine e, T1 a1, T2 a2)
      where T1 : IEngineObject, IIdentity<int>, ITransformable
      where T2 : IEngineObject, IIdentity<int>, ITransformable
    {
      if (a1 == null || a2 == null)
        return float.MaxValue;

      if (a1.Equals(a2))
        return 0;

      lock (locker)
      {
        if (Cleartime < e.Game.GameTime)
        {
          cache.Clear(clearfunc);
          Cleartime = e.Game.GameTime + 5;
        }

        int hash;
        if (a1.ID > a2.ID)
        {
          hash = (a1 is ActorInfo) ? 1 : (a1 is ProjectileInfo) ? 2 : 0;
          hash <<= 2;
          hash += a1.ID;
          hash <<= 8;
          hash += (a2 is ActorInfo) ? 1 : (a2 is ProjectileInfo) ? 2 : 0;
          hash <<= 2;
          hash += a2.ID;
        }
        else
        {
          hash = (a2 is ActorInfo) ? 1 : (a2 is ProjectileInfo) ? 2 : 0;
          hash <<= 2;
          hash += a2.ID;
          hash <<= 8;
          hash += (a1 is ActorInfo) ? 1 : (a1 is ProjectileInfo) ? 2 : 0;
          hash <<= 2;
          hash += a1.ID;
        }
        return cache.GetOrDefine(hash, new EngineTime(e), dofunc, new Trip<TVMathLibrary, ITransformable, ITransformable>(e.TrueVision.TVMathLibrary, a1, a2), EngineTime.EqualityComparer.Instance);
      }
    }

    /// <summary>
    /// Gets the distance between two points
    /// </summary>
    /// <returns></returns>
    public static float GetDistance(TVMathLibrary math, TV_3DVECTOR first, TV_3DVECTOR second)
    {
      return CalculateDistance(math, first, second);
    }

    private static float CalculateDistance(TVMathLibrary math, ITransformable first, ITransformable second)
    {
      return CalculateDistance(math, first.GetGlobalPosition(), second.GetGlobalPosition());
    }

    private static float CalculateDistance(TVMathLibrary math, TV_3DVECTOR first, TV_3DVECTOR second)
    {
      return math.GetDistanceVec3D(first, second);
    }

    /// <summary>
    /// Returns a linear interpolated position between two points
    /// </summary>
    /// <param name="e">The game engine</param>
    /// <param name="first">The first point</param>
    /// <param name="second">The first point</param>
    /// <param name="frac">The fractional position between the two points.</param>
    /// <returns></returns>
    public static TV_3DVECTOR Lerp(TVMathLibrary math, TV_3DVECTOR first, TV_3DVECTOR second, float frac)
    {
      TV_3DVECTOR ret = new TV_3DVECTOR();
      math.TVVec3Lerp(ref ret, first, second, frac);
      return ret;
    }
  }
}
