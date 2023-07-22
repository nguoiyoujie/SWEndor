using Primrose.Primitives.ValueTypes;
using System;
using Primrose.Expressions;
using SWEndor.Game.Primitives.Extensions;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  /// <summary>
  /// Script API for Faction functions
  /// </summary>
  public static class FactionFns
  {
    /// <summary>
    /// Creates a faction. This function does nothing if a faction of the same name already exists.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>NULL</returns>
    public static Val AddFaction(IContext context, string name, float3 vec_color)
    {
      COLOR color = new COLOR(vec_color);
      FactionInfo.Factory.Add(name, color);
      return Val.NULL;
    }

    public static Val AddFaction(IContext context, string name, float3 vec_color, float3 laser_color)
    {
      COLOR color = new COLOR(vec_color);
      COLOR lasercolor = new COLOR(laser_color);
      FactionInfo.Factory.Add(name, color, lasercolor);
      return Val.NULL;
    }

    public static Val GetColor(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.Color.ToFloat3());
    }

    public static Val SetColor(IContext context, string faction, float3 vec_color)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      COLOR color = new COLOR(vec_color);
      f.Color = color;
      return Val.NULL;
    }

    public static Val GetLaserColor(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.LaserColor.ToFloat3());
    }

    public static Val SetLaserColor(IContext context, string faction, float3 vec_color)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      COLOR color = new COLOR(vec_color);
      f.LaserColor = color;
      return Val.NULL;
    }

    /// <summary>
    /// Returns if two factions are allies.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>TRUE if the factions are allies, FALSE if not</returns>
    /// <exception cref="InvalidOperationException">At least one of the factions are invalid</exception>
    public static Val IsAlly(IContext context, string faction1, string faction2)
    {
      FactionInfo f1 = FactionInfo.Factory.Get(faction1);
      FactionInfo f2 = FactionInfo.Factory.Get(faction2);
      if (f1 == null || f2 == null)
        return Val.FALSE;

      return new Val(f1.Allies.Contains(f2) || f2.Allies.Contains(f1));
    }

    /// <summary>
    /// Makes two factions to be allies. Does nothing if the factions are already allies.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">At least one of the factions are invalid</exception>
    public static Val MakeAlly(IContext context, string faction1, string faction2)
    {
      FactionInfo f1 = FactionInfo.Factory.Get(faction1);
      FactionInfo f2 = FactionInfo.Factory.Get(faction2);
      if (f1 == null || f2 == null)
        throw new InvalidOperationException("At least one of the factions is not defined.");

      if (!f1.Allies.Contains(f2))
        f1.Allies.Add(f2);

      if (!f2.Allies.Contains(f1))
        f2.Allies.Add(f1);
      return Val.NULL;
    }

    /// <summary>
    /// Makes two factions to be enemies. Does nothing if the factions are already enemies.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">At least one of the factions are invalid</exception>
    public static Val MakeEnemy(IContext context, string faction1, string faction2)
    {
      FactionInfo f1 = FactionInfo.Factory.Get(faction1);
      FactionInfo f2 = FactionInfo.Factory.Get(faction2);
      if (f1 == null || f2 == null)
        throw new InvalidOperationException("At least one of the factions is not defined.");

      if (f1.Allies.Contains(f2))
        f1.Allies.Remove(f2);

      if (f2.Allies.Contains(f1))
        f2.Allies.Remove(f1);

      return Val.NULL;
    }

    /// <summary>
    /// Gets the faction's Wings 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetWings(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.GetWings().ToArray());
    }

    /// <summary>
    /// Gets the faction's Ships 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetShips(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.GetShips().ToArray());
    }

    /// <summary>
    /// Gets the faction's Structures 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetStructures(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.GetStructures().ToArray());
    }

    /// <summary>
    /// Gets the faction's WingCount 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetWingCount(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.WingCount);
    }

    /// <summary>
    /// Gets the faction's ShipCount 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetShipCount(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.ShipCount);
    }

    /// <summary>
    /// Gets the faction's StructureCount 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetStructureCount(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.StructureCount);
    }

    /// <summary>
    /// Gets the faction's WingLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT limit</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetWingLimit(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.WingLimit);
    }

    /// <summary>
    /// Gets the faction's ShipLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT limit</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetShipLimit(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.ShipLimit);
    }

    /// <summary>
    /// Gets the faction's StructureLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT limit</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetStructureLimit(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");

      return new Val(f.StructureLimit);
    }

    /// <summary>
    /// Sets the faction's WingLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    ///     1 INT value
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val SetWingLimit(IContext context, string faction, int limit)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.WingLimit = limit;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the faction's ShipLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    ///     1 INT value
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val SetShipLimit(IContext context, string faction, int limit)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.ShipLimit = limit;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the faction's StructureLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    ///     1 INT value
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val SetStructureLimit(IContext context, string faction, int limit)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.StructureLimit = limit;
      return Val.NULL;
    }

    /// <summary>
    /// Gets the faction's WingSpawnLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetWingSpawnLimit(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      return new Val(f.WingSpawnLimit);
    }

    /// <summary>
    /// Gets the faction's ShipSpawnLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetShipSpawnLimit(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      return new Val(f.ShipSpawnLimit);
    }

    /// <summary>
    /// Gets the faction's StructureSpawnLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    /// </param>
    /// <returns>INT count</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val GetStructureSpawnLimit(IContext context, string faction)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      return new Val(f.ShipSpawnLimit);
    }

    /// <summary>
    /// Sets the faction's WingSpawnLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    ///     1 INT value
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val SetWingSpawnLimit(IContext context, string faction, int limit)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.WingSpawnLimit = limit;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the faction's ShipSpawnLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    ///     1 INT value
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val SetShipSpawnLimit(IContext context, string faction, int limit)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.ShipSpawnLimit = limit;
      return Val.NULL;
    }

    /// <summary>
    /// Sets the faction's StructureSpawnLimit 
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_name
    ///     1 INT value
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">The faction is not defined</exception>
    public static Val SetStructureSpawnLimit(IContext context, string faction, int limit)
    {
      FactionInfo f = FactionInfo.Factory.Get(faction);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.StructureSpawnLimit = limit;
      return Val.NULL;
    }
  }
}
