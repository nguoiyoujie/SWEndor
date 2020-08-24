using SWEndor.Game.Actors;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Scenarios;
using System.Collections.Generic;

namespace SWEndor.Game
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

      public static FactionInfo Add(string name, COLOR color)
      {
        if (!list.ContainsKey(name))
          list.Add(name, new FactionInfo(name, color));
        return list[name];
      }

      public static FactionInfo Add(string name, COLOR color, COLOR lasercolor)
      {
        if (!list.ContainsKey(name))
          list.Add(name, new FactionInfo(name, color, lasercolor));
        return list[name];
      }
    }

    public static FactionInfo Neutral;

    private FactionInfo(string name, COLOR color)
    {
      Name = name;
      Color = color;
    }

    private FactionInfo(string name, COLOR color, COLOR lasercolor)
    {
      Name = name;
      Color = color;
      LaserColor = lasercolor;
    }

    public bool IsAlliedWith(FactionInfo faction)
    {
      return (this == Neutral || faction == Neutral || this == faction || Allies.Contains(faction));
    }

    public readonly string Name;
    public COLOR Color = ColorLocalization.Get(ColorLocalKeys.WHITE);
    public COLOR LaserColor = default;

    static FactionInfo()
    {
      Neutral = new FactionInfo("Neutral", ColorLocalization.Get(ColorLocalKeys.WHITE));
    }

    public override string ToString()
    {
      return "(F:{0})".F(Name);
    }

    public int WingLimit = -1;
    public int WingSpawnLimit = -1;
    public bool WingLimitIncludesAllies = true;
    private readonly List<int> _wings = new List<int>();

    public int ShipLimit = -1;
    public int ShipSpawnLimit = -1;
    public bool ShipLimitIncludesAllies = true;
    private readonly List<int> _ships = new List<int>();

    public int StructureLimit = -1;
    public int StructureSpawnLimit = -1;
    public bool StructureLimitIncludesAllies = true;
    private readonly List<int> _structures = new List<int>();

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

    public void RegisterActor(ActorInfo ainfo)
    {
      if (this == Neutral)
        return;

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER))
      {
        _wings.Add(ainfo.ID);
        int wc = WingCount;
        if (WingLimit != -1 && WingLimit < wc)
          WingLimit = wc;
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.SHIP))
      {
        _ships.Add(ainfo.ID);
        int wc = ShipCount;
        if (ShipLimit != -1 && ShipLimit < wc)
          ShipLimit = wc;
      }

      if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.STRUCTURE))
      {
        _structures.Add(ainfo.ID);
        int wc = StructureCount;
        if (StructureLimit != -1 && StructureLimit < wc)
          StructureLimit = wc;
      }
    }

    public void UnregisterActor(ActorInfo ainfo)
    {
      if (this == Neutral)
        return;

      if (_wings.Remove(ainfo.ID))
      {
        if (WingLimit > 0)
          WingLimit--;
      }

      if (_ships.Remove(ainfo.ID))
      {
        if (ShipLimit > 0)
          ShipLimit--;
      }

      if (_structures.Remove(ainfo.ID))
      {
        if (StructureLimit > 0)
          StructureLimit--;
      }

      if (ainfo.DisposingOrDisposed)
      {
        if (ainfo.IsDead)
        {
          FactionInfo p = ainfo.Engine.PlayerInfo.Actor?.Faction ?? Neutral;
          ScenarioBase b = ainfo.Engine.GameScenarioManager.Scenario;
          if (p != Neutral)
          {
            if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.FIGHTER)
              && b.State.InformLostWing
              && (p == this || p.WingLimitIncludesAllies && p.IsAlliedWith(this))
              )
              b.LostWing();

            if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.SHIP)
              && b.State.InformLostShip
              && (p == this || p.ShipLimitIncludesAllies && p.IsAlliedWith(this))
              )
              b.LostShip();

            if (ainfo.TypeInfo.AIData.TargetType.Has(TargetType.STRUCTURE)
              && b.State.InformLostStructure
              && (p == this || p.StructureLimitIncludesAllies && p.IsAlliedWith(this))
              )
              b.LostStructure();
          }
        }
      }
    }

    // check allocation
    public List<int> GetWings() { return _wings; }
    public List<int> GetShips() { return _ships; }
    public List<int> GetStructures() { return _structures; }

    /*
    public IEnumerable<int> GetWings(bool includeAllies = false)
    {
      List<int> f = _wings;
      for (int i = 0; i < f.Count; i++)
        yield return f[i];

      if (includeAllies)
        foreach (FactionInfo fi in Allies)
          for (int i = 0; i < fi._wings.Count; i++)
            yield return fi._wings[i];
    }

    public IEnumerable<int> GetShips(bool includeAllies = false)
    {
      List<int> f = _ships;
      for (int i = 0; i < f.Count; i++)
        yield return f[i];

      if (includeAllies)
        foreach (FactionInfo fi in Allies)
          for (int i = 0; i < fi._ships.Count; i++)
            yield return fi._ships[i];
    }

    public IEnumerable<int> GetStructures(bool includeAllies = false)
    {
      List<int> f = _structures;
      for (int i = 0; i < f.Count; i++)
        yield return f[i];

      if (includeAllies)
        foreach (FactionInfo fi in Allies)
          for (int i = 0; i < fi._structures.Count; i++)
            yield return fi._structures[i];
    }
    */

    public int GetRandomWing(Engine engine) { return _wings.Count > 0 ? _wings.Random(engine.Random) : -1; }
    public int GetRandomShip(Engine engine) { return _ships.Count > 0 ? _ships.Random(engine.Random) : -1; }
    public int GetRandomStructure(Engine engine) { return _structures.Count > 0 ? _structures.Random(engine.Random) : -1; }

    public int GetFirst(TargetType type)
    {
      switch (type)
      {
        case TargetType.FIGHTER:
          return _wings.Count > 0 ? _wings[0] : -1;
        case TargetType.SHIP:
          return _ships.Count > 0 ? _ships[0] : -1;
        case TargetType.STRUCTURE:
          return _structures.Count > 0 ? _structures[0] : -1;
        default:
          return -1;
      }
    }

    public int GetLast(TargetType type)
    {
      switch (type)
      {
        case TargetType.FIGHTER:
          return _wings.Count > 0 ? _wings[_wings.Count - 1] : -1;
        case TargetType.SHIP:
          return _ships.Count > 0 ? _ships[_ships.Count - 1] : -1;
        case TargetType.STRUCTURE:
          return _structures.Count > 0 ? _structures[_structures.Count - 1] : -1;
        default:
          return -1;
      }
    }
  }
}
