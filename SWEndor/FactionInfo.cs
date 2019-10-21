using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives.Extensions;
using SWEndor.Scenarios;
using System;
using System.Collections.Generic;

namespace SWEndor
{
  public class FactionInfo
  {
    public static class Factory
    {
      private static Dictionary<string, FactionInfo> list = new Dictionary<string, FactionInfo>();
      public static Dictionary<string, FactionInfo> GetList() { return list; }
      public static FactionInfo Get(string key) { return list.ContainsKey(key) ? list[key] : null; }

      public static void Clear()
      {
        list = new Dictionary<string, FactionInfo>();
      }

      public static FactionInfo Add(string name, int color)
      {
        if (!list.ContainsKey(name))
          list.Add(name, new FactionInfo(name, color));
        return list[name];
      }
    }

    public static FactionInfo Neutral;

    private FactionInfo(string name, int color)
    {
      Name = name;
      Color = color;

      //Array a = Enum.GetValues(typeof(TargetType));
      int len = 32; // a.Length;
      _first = new int[len];
      _cnt = new int[len];
      _limit = new int[len];
      _flush = new Queue<int>(124);
      //_enumerables = new Dictionary<ActorEnumerableParam, ActorEnumerable>(len);
      for (int i = 0; i < len; i++)
      {
        _first[i] = -1;
        _limit[i] = -1;
        //TargetType t = (TargetType)(1 << i); // a.GetValue(i);

        //_enumerables.Add(new ActorEnumerableParam(t, true), new ActorEnumerable(_map, this, t, true));
        //_enumerables.Add(new ActorEnumerableParam(t, false), new ActorEnumerable(_map, this, t, false));
      }
    }

    public bool IsAlliedWith(FactionInfo faction)
    {
      return (this == faction || Allies.Contains(faction));
    }

    public readonly string Name;
    public int Color = ColorLocalization.Get(ColorLocalKeys.WHITE);
    public bool AutoAI = false;

    private static Dictionary<int, int> _map;
    //private static Dictionary<TargetType, int[]> _types;

    private int[] _first;
    private int[] _cnt;
    private int _cnttotal;
    private Queue<int> _flush;

    //private Dictionary<ActorEnumerableParam, ActorEnumerable> _enumerables;

    static FactionInfo()
    {
      //Array a = Enum.GetValues(typeof(TargetType));
      //int len = 32; // a.Length;
      _map = new Dictionary<int, int>(Globals.ActorLimit);
      /*_types = new Dictionary<TargetType, int[]>(len);
      for (int i = 0; i < len; i++)
      {
        TargetType t = (TargetType)(1 << i);//a.GetValue(i);
        List<int> r = new List<int>(len);
        for (int j = 0; j < len; j++)
        {
          TargetType tj = (TargetType)a.GetValue(j);
          if ((t & tj) == tj && tj != 0)
            r.Add(j);
        }
        _types.Add(t, r.ToArray());
      }
      */
      Neutral = new FactionInfo("Neutral", ColorLocalization.Get(ColorLocalKeys.WHITE));
    }

    private int[] _limit;


    public int WingLimit = -1;
    public int WingSpawnLimit = -1;
    public bool WingLimitIncludesAllies = true;
    private List<int> _wings = new List<int>();

    public int ShipLimit = -1;
    public int ShipSpawnLimit = -1;
    public bool ShipLimitIncludesAllies = true;
    private List<int> _ships = new List<int>();

    public int StructureLimit = -1;
    public int StructureSpawnLimit = -1;
    public bool StructureLimitIncludesAllies = true;
    private List<int> _structures = new List<int>();

    public List<FactionInfo> Allies = new List<FactionInfo>();

    public int WingCount { get { return GetCount(TargetType.FIGHTER, false); } }
    public int ShipCount { get { return GetCount(TargetType.SHIP, false); } }
    public int StructureCount { get { return GetCount(TargetType.STRUCTURE, false); } }

    public override string ToString()
    {
      return "(F:{0})".F(Name);
    }

    public void RegisterActor(ActorInfo ainfo)
    {
      RegActor(ainfo);

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
      {
        int wc = WingCount;
        if (WingLimit != -1 && WingLimit < wc)
          WingLimit = wc;
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.SHIP))
      {
        int wc = ShipCount;
        if (ShipLimit != -1 && ShipLimit < wc)
          ShipLimit = wc;
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.STRUCTURE))
      {
        int wc = StructureCount;
        if (StructureLimit != -1 && StructureLimit < StructureCount)
          StructureLimit = StructureCount;
      }
    }

    public void UnregisterActor(ActorInfo ainfo)
    {
      RemActor(ainfo);

      // flush
      if (_flush.Count > 100)
        for (int i = 80; i >= 0; i--)
          _map.Remove(_flush.Dequeue());

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
      {
        if (ainfo.DisposingOrDisposed)
        {
          if (WingLimit > 0)
            WingLimit--;
          GameScenarioBase b = ainfo.Engine.GameScenarioManager.Scenario;
          if (b != null
             && (this == b.MainAllyFaction
              || (b.MainAllyFaction.WingLimitIncludesAllies && b.MainAllyFaction.IsAlliedWith(this))))
            b.LostWing();
        }
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.SHIP))
      {
        if (ainfo.DisposingOrDisposed)
        {
          if (ShipLimit > 0)
            ShipLimit--;
          GameScenarioBase b = ainfo.Engine.GameScenarioManager.Scenario;
          if (b != null
             && (this == b.MainAllyFaction
              || (b.MainAllyFaction.WingLimitIncludesAllies && b.MainAllyFaction.IsAlliedWith(this))))
            b.LostShip();
        }
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.STRUCTURE))
      {
        if (ainfo.DisposingOrDisposed)
        {
          if (StructureLimit > 0)
            StructureLimit--;
          GameScenarioBase b = ainfo.Engine.GameScenarioManager.Scenario;
          if (b != null
             && (this == b.MainAllyFaction
              || (b.MainAllyFaction.WingLimitIncludesAllies && b.MainAllyFaction.IsAlliedWith(this))))
            b.LostStructure();
        }
      }
    }

    private void RegActor(ActorInfo actor)
    {
      int id = actor.ID;
      TargetType type = actor.TypeInfo.AIData.TargetType;
      foreach (int i in ((uint)type).GetUniqueBits())//_types[type])
      {
        _map[id] = _first[i];
        _first[i] = id;
        _cnt[i]++;
        if (_limit[i] != -1 && _limit[i] < _cnt[i])
          _limit[i] = _cnt[i];
      }
      _cnttotal++;
    }

    private void RemActor(ActorInfo actor)
    {
      int id = actor.ID;
      TargetType type = actor.TypeInfo.AIData.TargetType;
      foreach (int i in ((uint)type).GetUniqueBits())
      {
        int curr = _first[i];
        int prev = -1;
        while (curr != -1)
        {
          if (curr == id)
          {
            int next = _map[curr];
            if (prev >= 0)
              _map[prev] = next;

            if (_first[i] == curr)
              _first[i] = next;

            _flush.Enqueue(curr);
            _cnt[i]--;
            _limit[i]--;
            break;
          }
          prev = curr;
          if (!_map.TryGetValue(curr, out curr))
            break;
        }
      }
      _cnttotal--;
    }

    public ActorEnumerable GetActors(TargetType type, bool includeAllies)
    {
      return new ActorEnumerable(_map, this, type, includeAllies);
      //return _enumerables[new ActorEnumerableParam(type, includeAllies)];
    }

    public int GetRandom(Engine engine, TargetType type)
    {
      int curr = -1;
      int[] l = new int[32];
      int i = 0;
      int cnt = 0;
      foreach (int u in ((uint)type).GetUniqueBits())
      {
        l[i++] = u;
        cnt += _cnt[u];
      }
      l[i++] = -1;

      if (cnt > 0)
      {
        int r = engine.Random.Next(0, cnt);

        for (int li = 0; r > 0 && li < l.Length; li++)
        {
          int x = l[li];
          curr = _first[x];
          //int prev = -1;
          for (; r > 0 && _map[curr] > -1; r--)
            curr = _map[curr];
          // in case chain is broken, but something would be wrong
          //if (!_map.TryGetValue(curr, out curr))
          //  return prev;
          //else
          //  prev = curr;
        }
      }
      return curr;
    }

    public int GetFirst(TargetType type)
    {
      int curr = _first[((uint)type).GetMostSignificantBit()];
      while (_map[curr] != -1)
        curr = _map[curr];
      return curr;
    }

    public int GetLast(TargetType type)
    {
      return _first[((uint)type).GetMostSignificantBit()];
    }

    public int GetCount(TargetType type, bool includeAllies)
    {
      if (type == TargetType.ANY)
        return GetTotalCount(includeAllies);

      int count = _cnt[((uint)type).GetMostSignificantBit()];

      if (includeAllies)
        foreach (FactionInfo fi in Allies)
          count += fi.GetCount(type, false);
      return count;
    }

    public int GetLimit(TargetType type, bool includeAllies)
    {
      int limit = _limit[((uint)type).GetMostSignificantBit()];

      if (includeAllies)
        foreach (FactionInfo fi in Allies)
          limit += fi.GetLimit(type, false);
      return limit;
    }

    public void SetLimit(TargetType type, int value)
    {
      _limit[((uint)type).GetMostSignificantBit()] = value;
    }

    public int GetTotalCount(bool includeAllies)
    {
      int count = _cnttotal;

      if (includeAllies)
        foreach (FactionInfo fi in Allies)
          count += fi.GetTotalCount(false);
      return count;
    }
    
    private struct ActorEnumerableParam
    {
      TargetType T;
      bool inclAlly;
      public ActorEnumerableParam(TargetType t, bool ally)
      {
        T = t;
        inclAlly = ally;
      }
    }

    public struct ActorEnumerable
    {
      readonly Dictionary<int, int> _map;
      readonly FactionInfo F;
      readonly uint T;
      bool inclAlly;
      public ActorEnumerable(Dictionary<int, int> map, FactionInfo f, TargetType t, bool ally)
      {
        _map = map;
        F = f;
        T = ((uint)t).GetMostSignificantBit();
        inclAlly = ally;
      }
      public ActorEnumerator GetEnumerator() { return new ActorEnumerator(_map, F, T, inclAlly); }
    }

    public struct ActorEnumerator
    {
      readonly Dictionary<int, int> _map;
      readonly FactionInfo F;
      readonly uint T;
      int curr;
      FactionInfo currF;
      int intF;
      bool inclAlly;
      public ActorEnumerator(Dictionary<int, int> map, FactionInfo f, uint t, bool ally)
      {
        _map = map;
        F = f;
        currF = f;
        T = t;
        curr = -2;
        intF = 0;
        inclAlly = ally;
      }
      public bool MoveNext()
      {
        if (curr == -2)
          curr = F._first[T];
        else
        {
          if (_map[curr] == -1 && inclAlly && F.Allies.Count > 0 && F.Allies.Count <= intF)
            curr = (currF = F.Allies[intF++])._first[T];
          else
            curr = _map[curr];
        }

        return curr != -1;
      }
      public int Current { get { return curr; } }
    }
  }
}
