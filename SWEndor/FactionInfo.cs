using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Scenarios;
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

      public static FactionInfo Add(string name, TV_COLOR color)
      {
        if (!list.ContainsKey(name))
          list.Add(name, new FactionInfo(name, color));
        return list[name];
      }
    }

    public static FactionInfo Neutral = new FactionInfo("Neutral", new TV_COLOR(1, 1, 1, 1));

    private FactionInfo(string name, TV_COLOR color)
    {
      Name = name;
      Color = color;
    }

    public bool IsAlliedWith(FactionInfo faction)
    {
      return (this == faction || Allies.Contains(faction));
    }

    public readonly string Name;
    public TV_COLOR Color = new TV_COLOR(1,1,1,1);
    public bool AutoAI = false;

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

    public int WingCount { get { return _wings.Count; } }
    public int ShipCount { get { return _ships.Count; } }
    public int StructureCount { get { return _structures.Count; } }

    public int AlliedWingCount
    {
      get
      {
        int ret = _wings.Count;
        foreach (FactionInfo fi in Allies)
          ret += fi.WingCount;
        return ret;
      }
    }

    public int AlliedShipCount
    {
      get
      {
        int ret = _ships.Count;
        foreach (FactionInfo fi in Allies)
          ret += fi.ShipCount;
        return ret;
      }
    }

    public int AlliedStructureCount
    {
      get
      {
        int ret = _structures.Count;
        foreach (FactionInfo fi in Allies)
          ret += fi.StructureCount;
        return ret;
      }
    }

    public void UnregisterActor(ActorInfo ainfo)
    {
      if (_wings.Remove(ainfo.ID))
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

      if (_ships.Remove(ainfo.ID))
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

      if (_structures.Remove(ainfo.ID))
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

    public void RegisterActor(ActorInfo ainfo)
    {
      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER) && !_wings.Contains(ainfo.ID))
      {
        _wings.Add(ainfo.ID);
        if (WingLimit != -1 && WingLimit < _wings.Count)
          WingLimit = _wings.Count;
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.SHIP) && !_ships.Contains(ainfo.ID))
      {
        _ships.Add(ainfo.ID);
        if (ShipLimit != -1 && ShipLimit < _ships.Count)
          ShipLimit = _ships.Count;
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.STRUCTURE) && !_structures.Contains(ainfo.ID))
      {
        _structures.Add(ainfo.ID);
        if (StructureLimit != -1 && StructureLimit < _structures.Count)
          StructureLimit = _structures.Count;
      }
    }

    public List<int> GetAll()
    {
      List<int> ret = new List<int>(WingCount + ShipCount + StructureCount);
      ret.AddRange(GetWings());
      ret.AddRange(GetShips());
      ret.AddRange(GetStructures());
      return ret;
    }

    public int GetWing(int index) { return _wings[index]; }
    public int GetShip(int index) { return _ships[index]; }
    public int GetStructure(int index) { return _structures[index]; }

    public IEnumerable<int> GetWings()
    {
      List<int> f = _wings;
      for (int i = 0; i < f.Count; i++)
        yield return f[i];

      if (ShipLimitIncludesAllies)
        foreach (FactionInfo fi in Allies)
          for (int i = 0; i < fi._wings.Count; i++)
            yield return fi._wings[i];
    }

    public IEnumerable<int> GetShips()
    {
      List<int> f = _ships;
      for (int i = 0; i < f.Count; i++)
        yield return f[i];

      if (ShipLimitIncludesAllies)
        foreach (FactionInfo fi in Allies)
          for (int i = 0; i < fi._ships.Count; i++)
            yield return fi._ships[i];
    }

    public IEnumerable<int> GetStructures()
    {
      List<int> f = _structures;
      for (int i = 0; i < f.Count; i++)
        yield return f[i];

      if (ShipLimitIncludesAllies)
        foreach (FactionInfo fi in Allies)
          for (int i = 0; i < fi._structures.Count; i++)
            yield return fi._structures[i];
    }

    /*
    public List<int> GetStructures()
    {
      List<int> ret = new List<int>(_structures.ToArray());
      if (StructureLimitIncludesAllies)
        foreach (FactionInfo fi in Allies)
          ret.AddRange(fi._structures);
      return ret;
    }
    */
  }
}
