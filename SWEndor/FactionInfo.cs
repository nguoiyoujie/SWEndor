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

    public string Name { get; private set; }
    public TV_COLOR Color = new TV_COLOR(1,1,1,1);
    public bool AutoAI = false;

    public int WingLimit = -1;
    public int WingSpawnLimit = -1;
    public bool WingLimitIncludesAllies = true;
    public List<int> Wings = new List<int>();

    public int ShipLimit = -1;
    public int ShipSpawnLimit = -1;
    public bool ShipLimitIncludesAllies = true;
    public List<int> Ships = new List<int>();

    public int StructureLimit = -1;
    public int StructureSpawnLimit = -1;
    public bool StructureLimitIncludesAllies = true;
    public List<int> Structures = new List<int>();

    public List<FactionInfo> Allies = new List<FactionInfo>();

    public void UnregisterActor(ActorInfo ainfo)
    {
      if (ainfo.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER) && Wings.Contains(ainfo.ID))
      {
        Wings.Remove(ainfo.ID);
        if (ainfo.CreationState == CreationState.DISPOSED)
        {
          if (WingLimit > 0)
            WingLimit--;

          if (GameScenarioManager.Instance().Scenario != null
             && GameScenarioManager.Instance().Scenario.MainAllyFaction != null
             && (this == GameScenarioManager.Instance().Scenario.MainAllyFaction
              || (GameScenarioManager.Instance().Scenario.MainAllyFaction.WingLimitIncludesAllies && GameScenarioManager.Instance().Scenario.MainAllyFaction.IsAlliedWith(this))))
            GameScenarioManager.Instance().Scenario.LostWing();
        }
      }

      if (ainfo.TypeInfo.TargetType.HasFlag(TargetType.SHIP) && Ships.Contains(ainfo.ID))
      {
        Ships.Remove(ainfo.ID);
        if (ainfo.CreationState == CreationState.DISPOSED)
        {
          if (ShipLimit > 0)
            ShipLimit--;

          if (GameScenarioManager.Instance().Scenario != null
             && GameScenarioManager.Instance().Scenario.MainAllyFaction != null
             && (this == GameScenarioManager.Instance().Scenario.MainAllyFaction
              || (GameScenarioManager.Instance().Scenario.MainAllyFaction.ShipLimitIncludesAllies && GameScenarioManager.Instance().Scenario.MainAllyFaction.IsAlliedWith(this))))
            GameScenarioManager.Instance().Scenario.LostShip();
        }
      }


      if (ainfo.TypeInfo.TargetType.HasFlag(TargetType.STRUCTURE) && Structures.Contains(ainfo.ID))
      {
        Structures.Remove(ainfo.ID);
        if (ainfo.CreationState == CreationState.DISPOSED)
        {
          if (StructureLimit > 0)
            StructureLimit--;

          if (GameScenarioManager.Instance().Scenario != null
             && GameScenarioManager.Instance().Scenario.MainAllyFaction != null
             && (this == GameScenarioManager.Instance().Scenario.MainAllyFaction
              || (GameScenarioManager.Instance().Scenario.MainAllyFaction.StructureLimitIncludesAllies && GameScenarioManager.Instance().Scenario.MainAllyFaction.IsAlliedWith(this))))
            GameScenarioManager.Instance().Scenario.LostStructure();
        }
      }
    }

    public void RegisterActor(ActorInfo ainfo)
    {
      if (ainfo.TypeInfo.TargetType.HasFlag(TargetType.FIGHTER) && !Wings.Contains(ainfo.ID))
      {
        Wings.Add(ainfo.ID);
        if (WingLimit != -1 && WingLimit < Wings.Count)
          WingLimit = Wings.Count;
      }

      if (ainfo.TypeInfo.TargetType.HasFlag(TargetType.SHIP) && !Ships.Contains(ainfo.ID))
      {
        Ships.Add(ainfo.ID);
        if (ShipLimit != -1 && ShipLimit < Ships.Count)
          ShipLimit = Ships.Count;
      }

      if (ainfo.TypeInfo.TargetType.HasFlag(TargetType.STRUCTURE) && !Structures.Contains(ainfo.ID))
      {
        Structures.Add(ainfo.ID);
        if (StructureLimit != -1 && StructureLimit < Structures.Count)
          StructureLimit = Structures.Count;
      }
    }

    public List<int> GetAll()
    {
      List<int> ret = GetWings();
      ret.AddRange(GetShips());
      ret.AddRange(GetStructures());
      return ret;
    }

    public List<int> GetWings()
    {
      List<int> ret = new List<int>(Wings);
      if (WingLimitIncludesAllies)
        foreach (FactionInfo fi in Allies)
          ret.AddRange(fi.Wings);
      return ret;
    }

    public List<int> GetShips()
    {
      List<int> ret = new List<int>(Ships);
      if (ShipLimitIncludesAllies)
        foreach (FactionInfo fi in Allies)
          ret.AddRange(fi.Ships);
      return ret;
    }

    public List<int> GetStructures()
    {
      List<int> ret = new List<int>(Structures);
      if (StructureLimitIncludesAllies)
        foreach (FactionInfo fi in Allies)
          ret.AddRange(fi.Structures);
      return ret;
    }
  }
}
