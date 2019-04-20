using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using System;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class FactionManagement
  {
    public static object AddFaction(Context context, object[] ps)
    {
      TV_COLOR color = new TV_COLOR(Convert.ToSingle(ps[1].ToString()), Convert.ToSingle(ps[2].ToString()), Convert.ToSingle(ps[3].ToString()), 1);
      FactionInfo.Factory.Add(ps[0].ToString(), color);
      return true;
    }

    public static object SetAsMainAllyFaction(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null && GameScenarioManager.Instance().Scenario != null)
      {
        GameScenarioManager.Instance().Scenario.MainAllyFaction = f;
        return true;
      }
      return false;
    }

    public static object SetAsMainEnemyFaction(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null && GameScenarioManager.Instance().Scenario != null)
      {
        GameScenarioManager.Instance().Scenario.MainEnemyFaction = f;
        return true;
      }
      return false;
    }

    public static object MakeAlly(Context context, object[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get(ps[0].ToString());
      FactionInfo f2 = FactionInfo.Factory.Get(ps[1].ToString());
      if (f1 != null && f2 != null)
      {
        if (!f1.Allies.Contains(f2))
          f1.Allies.Add(f2);

        if (!f2.Allies.Contains(f1))
          f2.Allies.Add(f1);
        return true;
      }
      return false;
    }

    public static object MakeEnemy(Context context, object[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get(ps[0].ToString());
      FactionInfo f2 = FactionInfo.Factory.Get(ps[1].ToString());
      if (f1 != null && f2 != null)
      {
        if (f1.Allies.Contains(f2))
          f1.Allies.Remove(f2);

        if (f2.Allies.Contains(f1))
          f2.Allies.Remove(f1);
        return true;
      }
      return false;
    }

    public static object GetWingCount(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.GetWings().Count : 0;
    }

    public static object GetShipCount(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.GetShips().Count : 0;
    }

    public static object GetStructureCount(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.GetStructures().Count : 0;
    }

    public static object GetWingLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.WingLimit : 0;
    }

    public static object GetShipLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.ShipLimit : 0;
    }

    public static object GetStructureLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.StructureLimit : 0;
    }

    public static object SetWingLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null)
        f.WingLimit = Convert.ToInt32(ps[1].ToString());
      return true;
    }

    public static object SetShipLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null)
        f.ShipLimit = Convert.ToInt32(ps[1].ToString());
      return true;
    }

    public static object SetStructureLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null)
        f.StructureLimit = Convert.ToInt32(ps[1].ToString());
      return true;
    }

    public static object GetWingSpawnLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.WingSpawnLimit : 0;
    }

    public static object GetShipSpawnLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.ShipSpawnLimit : 0;
    }

    public static object GetStructureSpawnLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      return (f != null) ? f.StructureSpawnLimit : 0;
    }

    public static object SetWingSpawnLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null)
        f.WingSpawnLimit = Convert.ToInt32(ps[1].ToString());
      return true;
    }

    public static object SetShipSpawnLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null)
        f.ShipSpawnLimit = Convert.ToInt32(ps[1].ToString());
      return true;
    }

    public static object SetStructureSpawnLimit(Context context, object[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ToString());
      if (f != null)
        f.StructureSpawnLimit = Convert.ToInt32(ps[1].ToString());
      return true;
    }
  }
}
