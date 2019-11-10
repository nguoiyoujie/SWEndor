using SWEndor.Scenarios.Scripting.Expressions;
using Primrose.Primitives.ValueTypes;
using System;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
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

    /// <summary>
    /// Makes two factions to be allies. Does nothing if the factions are already allies.
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_1_name
    ///     1 STRING faction_2_name
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">At least one of the factions are invalid</exception>
    public static Val MakeAlly(Context context, Val[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get((string)ps[0]);
      FactionInfo f2 = FactionInfo.Factory.Get((string)ps[1]);
      if (f1 == null || f2 == null)
        throw new InvalidOperationException("At least one of the factions are not defined.");

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
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING faction_1_name
    ///     1 STRING faction_2_name
    /// </param>
    /// <returns>NULL</returns>
    /// <exception cref="InvalidOperationException">At least one of the factions are invalid</exception>
    public static Val MakeEnemy(Context context, Val[] ps)
    {
      FactionInfo f1 = FactionInfo.Factory.Get((string)ps[0]);
      FactionInfo f2 = FactionInfo.Factory.Get((string)ps[1]);
      if (f1 == null || f2 == null)
        throw new InvalidOperationException("At least one of the factions are not defined.");

      if (f1.Allies.Contains(f2))
        f1.Allies.Remove(f2);

      if (f2.Allies.Contains(f1))
        f2.Allies.Remove(f1);

      return Val.NULL;
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
    public static Val GetWingCount(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val GetShipCount(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val GetStructureCount(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val GetWingLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val GetShipLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val GetStructureLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val SetWingLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.WingLimit = (int)ps[1];
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
    public static Val SetShipLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.ShipLimit = (int)ps[1];
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
    public static Val SetStructureLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.StructureLimit = (int)ps[1];
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
    public static Val GetWingSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val GetShipSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val GetStructureSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
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
    public static Val SetWingSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.WingSpawnLimit = (int)ps[1];
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
    public static Val SetShipSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.ShipSpawnLimit = (int)ps[1];
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
    public static Val SetStructureSpawnLimit(Context context, Val[] ps)
    {
      FactionInfo f = FactionInfo.Factory.Get((string)ps[0]);
      if (f == null)
        throw new InvalidOperationException("The faction is not defined.");
      f.StructureSpawnLimit = (int)ps[1];
      return Val.NULL;
    }
  }
}
