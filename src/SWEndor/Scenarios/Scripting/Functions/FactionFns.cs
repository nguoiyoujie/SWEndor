﻿using MTV3D65;
using SWEndor.Scenarios.Scripting.Expressions;
using Primrose.Primitives.ValueTypes;
using System;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class FactionFns
  {
    /// <summary>
    /// Creates a faction. This function does nothing if a faction of the same name already exists.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    ///     1 FLOAT3 faction_color
    /// </param>
    /// <returns>NULL</returns>
    public static Val AddFaction(Context context, Val[] ps)
    {
      string name = (string)ps[0];
      COLOR color = new COLOR((float3)ps[1]);
      FactionInfo.Factory.Add(name, color);
      return Val.NULL;
    }

    /*
    public static Val SetAsMainAllyFaction(SWContext context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ValueS);
      if (f != null && context.Engine.GameScenarioManager.Scenario != null)
      {
        context.Engine.GameScenarioManager.Scenario.MainAllyFaction = f;
        return Val.TRUE;
      }
      return Val.FALSE;
    }

    public static Val SetAsMainEnemyFaction(SWContext context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get(ps[0].ValueS);
      if (f != null && context.Engine.GameScenarioManager.Scenario != null)
      {
        context.Engine.GameScenarioManager.Scenario.MainEnemyFaction = f;
        return Val.TRUE;
      }
      return Val.FALSE;
    }
    */


    public static Val MakeAlly(Context context, Val[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get((string)ps[0]);
      FactionInfo f2 = FactionInfo.Factory.Get((string)ps[1]);
      if (f1 != null && f2 != null)
      {
        if (!f1.Allies.Contains(f2))
          f1.Allies.Add(f2);

        if (!f2.Allies.Contains(f1))
          f2.Allies.Add(f1);
        return Val.TRUE;
      }
      return Val.FALSE;
    }

    public static Val MakeEnemy(Context context, Val[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get((string)ps[0]);
      FactionInfo f2 = FactionInfo.Factory.Get((string)ps[1]);
      if (f1 != null && f2 != null)
      {
        if (f1.Allies.Contains(f2))
          f1.Allies.Remove(f2);

        if (f2.Allies.Contains(f1))
          f2.Allies.Remove(f1);
        return Val.TRUE;
      }
      return Val.FALSE;
    }

    public static Val GetWingCount(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.WingCount : 0);
    }

    public static Val GetShipCount(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.ShipCount : 0);
    }

    public static Val GetStructureCount(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.StructureCount : 0);
    }

    public static Val GetWingLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.WingLimit : 0);
    }

    public static Val GetShipLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.ShipLimit : 0);
    }

    public static Val GetStructureLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.StructureLimit : 0);
    }

    public static Val SetWingLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f != null)
        f.WingLimit = Convert.ToInt32((string)ps[1]);
      return Val.TRUE;
    }

    public static Val SetShipLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f != null)
        f.ShipLimit = Convert.ToInt32((string)ps[1]);
      return Val.TRUE;
    }

    public static Val SetStructureLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f != null)
        f.StructureLimit = Convert.ToInt32((string)ps[1]);
      return Val.TRUE;
    }

    public static Val GetWingSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.WingSpawnLimit : 0);
    }

    public static Val GetShipSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.ShipSpawnLimit : 0);
    }

    public static Val GetStructureSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      return new Val((f != null) ? f.StructureSpawnLimit : 0);
    }

    public static Val SetWingSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f != null)
        f.WingSpawnLimit = Convert.ToInt32((string)ps[1]);
      return Val.TRUE;
    }

    public static Val SetShipSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f != null)
        f.ShipSpawnLimit = Convert.ToInt32((string)ps[1]);
      return Val.TRUE;
    }

    public static Val SetStructureSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f != null)
        f.StructureSpawnLimit = Convert.ToInt32((string)ps[1]);
      return Val.TRUE;
    }
  }
}
