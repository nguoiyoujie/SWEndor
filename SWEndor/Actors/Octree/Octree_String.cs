using MTV3D65;
using SWEndor.Primitives.Factories;
using SWEndor.Primitives.Geometry;
using SWEndor.Scenarios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SWEndor.Actors
{
  public partial class ActorInfo
  {
    public string OctID = "x"; // init to invalid value to ensure first update
  }

  public class Octree_String
  {
    /*
     * OctID is encoded as a string
     * octree-space:
     *    a string of characters '0' to '7', of any length. Each character represents the successive octant the object is in.
     * If the object cannot be fit to any octant, a 0-length string is represented.
     * All length of divisions is supported, however it is recommended to still limit the length to reasonable levels.
    */

    public TV_3DVECTOR Center;
    public float Radius;
    public float TrueMinimum;
    private List<ActorInfo> list = new List<ActorInfo>(Globals.ActorLimit);
    private List<string> slist = new List<string>(Globals.ActorLimit);
    private object locker = new object();
    private ObjectPool<StringBuilder> sbpool;

    public Octree_String(TV_3DVECTOR center, float radius, float minimum)
    {
      Center = center;
      Radius = radius;
      TrueMinimum = minimum;
      sbpool = new ObjectPool<StringBuilder>(() => new StringBuilder(16), (p) => p.Clear());
    }

    public Octree_String(GameScenarioManager mgr)
    {
      Center = (mgr.MaxBounds + mgr.MinBounds) * 0.5f;
      TV_3DVECTOR v = (mgr.MaxBounds - mgr.MinBounds) * 0.5f;
      Radius = Math.Max(Math.Max(v.x, v.y), v.z);
    }

    public void GetId(TV_3DVECTOR pos, float minSize, ref string oid)
    {
      float r = Radius;
      TV_3DVECTOR p = pos - Center;
      StringBuilder sb = sbpool.GetNew();
      while (r > minSize
         && r > TrueMinimum
         && p.x - minSize >= -r
         && p.x + minSize < r
         && p.y - minSize >= -r
         && p.y + minSize < r
         && p.z - minSize >= -r
         && p.z + minSize < r
         )
      {
        sb.Append((char)('0' + ((p.x > 0) ? 1 : 0) + ((p.y > 0) ? 2 : 0) + ((p.z > 0) ? 4 : 0)));
        r *= 0.5f;
        p -= new TV_3DVECTOR((p.x > 0) ? r : -r, (p.y > 0) ? r : -r, (p.z > 0) ? r : -r);
      }
      if (!StringEquals(sb, oid))
        oid = sb.ToString();
      sbpool.Return(sb);
    }

    public void GetId(Box box, ref string oid)
    {
      GetId(new TV_3DVECTOR(box.X.Min, box.Y.Min, box.Z.Min), new TV_3DVECTOR(box.X.Max, box.Y.Max, box.Z.Max), ref oid);
    }

    public void GetId(TV_3DVECTOR p0, TV_3DVECTOR p1, ref string oid)
    {
      float r = Radius;
      TV_3DVECTOR p = p0 - Center;
      TV_3DVECTOR pd = p1 - Center;
      StringBuilder sb = sbpool.GetNew();
      while (r > TrueMinimum)
      {
        uint c0 = ((p.x > 0) ? 4u : 0) + ((p.y > 0) ? 2u : 0) + ((p.z > 0) ? 1u : 0);
        uint c1 = ((pd.x > 0) ? 4u : 0) + ((pd.y > 0) ? 2u : 0) + ((pd.z > 0) ? 1u : 0);
        if (c0 == c1)
        {
          sb.Append((char)('0' + ((p.x > 0) ? 1 : 0) + ((p.y > 0) ? 2 : 0) + ((p.z > 0) ? 4 : 0)));
          r *= 0.5f;
          TV_3DVECTOR c = new TV_3DVECTOR((p.x > 0) ? r : -r, (p.y > 0) ? r : -r, (p.z > 0) ? r : -r);
          p -= c;
          pd -= c;
        }
        else
          break;
      }
      if (!StringEquals(sb, oid))
      oid = sb.ToString();
      sbpool.Return(sb);
    }

    private bool StringEquals(StringBuilder sb, string s)
    {
      if (sb.Length != s.Length)
        return false;

      for (int i = 0; i < s.Length; i++)
        if (s[i] != sb[i])
          return false;
      return true;
    }


    public Box GetBox(string id)
    {
      float r = Radius;
      TV_3DVECTOR p = Center;

      for (int i = 0; i < id.Length; i++)
      {
        char c = id[i];
        r *= 0.5f;
        switch (c)
        {
          case '0':
            p += new TV_3DVECTOR(-r, -r, -r);
            break;
          case '1':
            p += new TV_3DVECTOR(r, -r, -r);
            break;
          case '2':
            p += new TV_3DVECTOR(-r, r, -r);
            break;
          case '3':
            p += new TV_3DVECTOR(r, r, -r);
            break;
          case '4':
            p += new TV_3DVECTOR(-r, -r, r);
            break;
          case '5':
            p += new TV_3DVECTOR(r, -r, r);
            break;
          case '6':
            p += new TV_3DVECTOR(-r, r, r);
            break;
          case '7':
            p += new TV_3DVECTOR(r, r, r);
            break;
        }
      }
      return new Box(p.x - r, p.x + r, p.y - r, p.y + r, p.z - r, p.z + r);
    }

    public bool Contains(string first, string sub)
    {
      return first.StartsWith(sub);
    }

    private string[] head = new string[] { " x", "0x", "1x", "2x", "3x", "4x", "5x", "6x" };

    public ActorEnumerable Search(string sub)
    {
      if (sub == null || sub.Length == 0)
        return new ActorEnumerable(list, slist, "", locker);

      //lock (locker)
      //{
        //int k = sub[0] - '0';
        //int i = k == 0 ? 0 : ~slist.BinarySearch(head[k]);
        //int j = slist.BinarySearch(sub);
        //if (j < 0) j = ~j;
        return new ActorEnumerable(list, slist, sub, locker);
      //}
    }

    public struct ActorEnumerable
    {
      readonly List<ActorInfo> L;
      readonly List<string> S;
      readonly string Str;
      //readonly int Start;

      readonly object Locker;
      public ActorEnumerable(List<ActorInfo> l, List<string> s, string str, object locker)
      {
        L = l;
        S = s;
        Str = str;
        //Start = start;
        Locker = locker;
      }
      public ActorEnumerator GetEnumerator() { return new ActorEnumerator(L, S, Str, Locker); }
    }

    public struct ActorEnumerator : IDisposable
    {
      readonly List<ActorInfo> L;
      readonly List<string> S;
      readonly string Str;
      readonly object Locker;
      int curr;
      public ActorEnumerator(List<ActorInfo> l, List<string> s, string str, object locker)
      {
        L = l;
        S = s;
        Str = str;
        curr = -1;
        Locker = locker;
        Monitor.Enter(Locker);
      }
      public bool MoveNext()
      {
        curr++;
        while (curr < S.Count && !Str.StartsWith(S[curr]) && S[curr].CompareTo(Str) < 0)
          curr++;

        return curr < S.Count && Str.StartsWith(S[curr]);
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
      string prev = a.OctID;
      GetId(a.GetBoundingBox(false), ref a.OctID);
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
  }
}
