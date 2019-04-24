using MTV3D65;
using SWEndor.Primitives;

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
    public static float GetRoughDistance(int actorID1, int actorID2)
    {
      if (actorID1 == actorID2)
        return 0;

      ActorInfo a1 = ActorInfo.Factory.Get(actorID1);
      ActorInfo a2 = ActorInfo.Factory.Get(actorID2);
      if (a1 == null || a2 == null)
        return float.MaxValue;

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

    public static float GetDistance(int actorID1, int actorID2, float limit)
    {
      float d = GetRoughDistance(actorID1, actorID2);
      if (d > limit)
        return limit;
      else
        return GetDistance(actorID1, actorID2);
    }

    public static float GetDistance(int actorID1, int actorID2)
    {
      if (actorID1 == actorID2)
        return 0;

      ActorInfo a1 = ActorInfo.Factory.Get(actorID1);
      ActorInfo a2 = ActorInfo.Factory.Get(actorID2);

      if (a1 == null || a2 == null)
        return float.MaxValue;

      if (Cleartime < Globals.Engine.Game.GameTime)
      {
        list.Clear();
        list.Refresh();
        Cleartime = Globals.Engine.Game.GameTime + 5;
      }

      long hash = (actorID1 < actorID2) ? (uint)(actorID1 << sizeof(int)) | (uint)actorID2 : (uint)(actorID2 << sizeof(int)) | (uint)actorID1 ;
      float dist;
      CachedFloat cached = list.Get(hash);
      if (cached.Time < Globals.Engine.Game.GameTime)
      {
        dist = CalculateDistance(a1, a2);
        list.Put(hash, new CachedFloat() { Time = Globals.Engine.Game.GameTime, Value = dist });
      }
      else
      {
        dist = cached.Value;
      }

      return dist;
    }

    public static float CalculateDistance(ActorInfo first, ActorInfo second)
    {
      return Globals.Engine.TVMathLibrary.GetDistanceVec3D(first.GetPosition(), second.GetPosition());
    }
  }
}
