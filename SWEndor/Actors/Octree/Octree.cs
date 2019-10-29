using MTV3D65;
using SWEndor.Primitives.Factories;
using SWEndor.Primitives.Geometry;
using SWEndor.Scenarios;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    //public ulong OctID = 0;
  }

  public class Octree 
  {
    // Unused in favor of the simpler Octree_String, in future 64-bit Octree may be more performant

    /*
     * OctID is encoded as 63 bits representing octree-space followed by 1 bit buffer
     * octree-space:
     *    xyzxyzxyzxyz... each bit representing whether the object belongs to a higher or lower subdivision
     * the octree-space is always terminated with a 1 bit. Thus the last 1 bit represents the end of the subdivision
     * If the entire octree-space is filled, the 64th bit is filled with the 1 bit terminus.
     * up to 21 full xyz divisions are supported.
    */

      /*
    public TV_3DVECTOR Center;
    public float Radius;
    public float TrueMinimum;
    private List<ActorInfo> list = new List<ActorInfo>(Globals.ActorLimit);
    private List<ulong> slist = new List<ulong>(Globals.ActorLimit);
    private object locker = new object();

    public Octree(TV_3DVECTOR center, float radius, float minimum)
    {
      Center = center;
      Radius = radius;
      TrueMinimum = minimum;
    }

    public Octree(GameScenarioManager mgr)
    {
      Center = (mgr.MaxBounds + mgr.MinBounds) * 0.5f;
      TV_3DVECTOR v = (mgr.MaxBounds - mgr.MinBounds) * 0.5f;
      Radius = Math.Max(Math.Max(v.x, v.y), v.z);
    }

    public ulong GetId(TV_3DVECTOR pos, float minSize)
    {
      float r = Radius;
      TV_3DVECTOR p = pos - Center;
      ulong n = 0;
      bool term = true;
      for (int i = 0; i < 21; i++)
      {
        n <<= 3;
        if (r > minSize
           && r > TrueMinimum
           && p.x - minSize >= -r
           && p.x + minSize < r
           && p.y - minSize >= -r
           && p.y + minSize < r
           && p.z - minSize >= -r
           && p.z + minSize < r
           )
        {
          n |= ((p.x > 0) ? 4u : 0) + ((p.y > 0) ? 2u : 0) + ((p.z > 0) ? 1u : 0);
          r *= 0.5f;
          p -= new TV_3DVECTOR((p.x > 0) ? r : -r, (p.y > 0) ? r : -r, (p.z > 0) ? r : -r);
        }
        else if (term)
        {
          n |= 4;
          term = false;
        }
      }
      n <<= 1;
      if (term)
        n |= 1;
      return n;
    }

    public ulong GetId(Box box)
    {
      return GetId(new TV_3DVECTOR(box.X.Min, box.Y.Min, box.Z.Min), new TV_3DVECTOR(box.X.Max, box.Y.Max, box.Z.Max));
    }

    public ulong GetId(TV_3DVECTOR p0, TV_3DVECTOR p1)
    {
      float r = Radius;
      TV_3DVECTOR p = p0 - Center;
      TV_3DVECTOR pd = p1 - Center;
      ulong n = 0;
      bool term = true;
      for (int i = 0; i < 21; i++)
      {
        n <<= 3;

        if (r > TrueMinimum && term)
        {
          uint c0 = ((p.x > 0) ? 4u : 0) + ((p.y > 0) ? 2u : 0) + ((p.z > 0) ? 1u : 0);
          uint c1 = ((pd.x > 0) ? 4u : 0) + ((pd.y > 0) ? 2u : 0) + ((pd.z > 0) ? 1u : 0);
          if (c0 == c1)
          {
            n |= c0;
            r *= 0.5f;
            TV_3DVECTOR c = new TV_3DVECTOR((p.x > 0) ? r : -r, (p.y > 0) ? r : -r, (p.z > 0) ? r : -r);
            p -= c;
            pd -= c;
          }
          else if (term)
          {
            n |= 4;
            term = false;
          }
        }
        else if (term)
        {
          n |= 4;
          term = false;
        }
      }
      n <<= 1;
      if (term)
        n |= 1;

      return n;
    }

    public Box GetBox(ulong id)
    {
      float r = Radius;
      TV_3DVECTOR p = Center;

      int term = 0;
      for (; term < 64; term++)
        if ((id & (1uL << term)) != 0)
          break;

      for (int i = 63; i > term; i -= 3)
      {
        r *= 0.5f;
        p.x += (((id >> i) & 1) > 0) ? r : -r;
        p.y += ((id >> (i - 1) & 1) > 0) ? r : -r;
        p.z += ((id >> (i - 2) & 1) > 0) ? r : -r;
      }

      return new Box(p.x - r, p.x + r, p.y - r, p.y + r, p.z - r, p.z + r);
    }

    public bool Contains(ulong first, ulong sub)
    {
      int term = 0;
      for (; term < 64; term++)
        if ((sub & (1u << term)) > 0)
          break;

      first &= ~((1u << term) - 1);
      sub &= ~((1u << term) - 1);

      return first == sub;
    }

    public ActorEnumerable Search(ulong sub)
    {
      if (sub == 0)
        return new ActorEnumerable(list, 0, list.Count, locker);

      lock (locker)
      {
        ulong k = sub & 0xE000000000000000;
        int i = k == 0 ? 0 : slist.BinarySearch(k - 1);
        if (i < 0)
          i = ~i;
        int j = slist.BinarySearch(sub + 1);
        if (j < 0)
          j = ~j;
        return new ActorEnumerable(list, i, j, locker);
      }
    }

    public struct ActorEnumerable
    {
      readonly List<ActorInfo> L;
      readonly int Start;
      readonly int End;
      readonly object Locker;
      public ActorEnumerable(List<ActorInfo> l, int start, int end, object locker)
      {
        L = l;
        Start = start;
        End = end;
        Locker = locker;
      }
      public ActorEnumerator GetEnumerator() { return new ActorEnumerator(L, Start, End, Locker); }
    }

    public struct ActorEnumerator : IDisposable
    {
      readonly List<ActorInfo> L;
      readonly int Start;
      readonly int End;
      readonly object Locker;
      int curr;
      public ActorEnumerator(List<ActorInfo> l, int start, int end, object locker)
      {
        L = l;
        Start = start;
        End = end;
        curr = start - 1;
        Locker = locker;
        Monitor.Enter(Locker);
      }
      public bool MoveNext()
      {
        return ++curr < End;
      }
      public ActorInfo Current { get { return L[curr]; } }
      public void Dispose()
      {
        Monitor.Exit(Locker);
      }
    }

    public class BaseComparer : IComparer<ActorInfo>
    {
      public int Compare(ActorInfo x, ActorInfo y)
      {
        return x.ID.CompareTo(y.ID);
      }
    }
    OctComparer bcmp = new OctComparer();

    public class OctComparer : IComparer<ActorInfo>
    {
      public int Compare(ActorInfo x, ActorInfo y)
      {
        int res = x.OctID.CompareTo(y.OctID);
        if (res == 0)
          res = x.ID.CompareTo(y.ID);

        return res;
      }
    }
    OctComparer cmp = new OctComparer();

    public void Update(ActorInfo a)
    {
      var prev = a.OctID;
      a.OctID = GetId(a.GetBoundingBox(false));//a.GetGlobalPosition(), a.GetBoundingSphere(true).R);
      if (prev != a.OctID)
      {
        lock (locker)
        {
          int i = list.IndexOf(a);
          if (i >= 0)
          {
            list.RemoveAt(i);
            slist.RemoveAt(i);
          }

          i = ~list.BinarySearch(a, cmp);
          list.Insert(i, a);
          slist.Insert(i, a.OctID);
        }
      }
    }
    */
  }
}
