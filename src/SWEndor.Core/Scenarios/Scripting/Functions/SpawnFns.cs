using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Models;
using Primrose.Primitives.ValueTypes;
using System;
using SWEndor.Primitives.Extensions;
using Primrose.Expressions;

namespace SWEndor.Scenarios.Scripting.Functions
{
  public static class SpawnFns
  {
    /// <summary>
    /// Spawns a group of actors and assigns them to a single squadron
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING actor_type, 
    ///     1 STRING squad_name, 
    ///     2 STRING faction,
    ///     3 INT squad_count,
    ///     4 FLOAT spawn_time,
    ///     5 BOOL hyperspaceIn,
    ///     6 FLOAT3 position,
    ///     7 FLOAT3 rotation,
    ///     8 STRING squad_formation,
    ///     9 FLOAT formation_distance,
    ///     10 FLOAT wait_delay,  // ai
    ///     11 STRING targetType, // ai
    ///     12 STRING[] registries // TO-DO: Remove, replace registries
    /// </param>
    /// <returns>INT[] containing the IDs of the spawned actors</returns>
    public static Val Squadron_Spawn(Context context, params Val[] ps)
    {
      ScenarioBase gscenario = context.Engine.GameScenarioManager.Scenario;

      string actorType = (string)ps[0];
      string squadName = (string)ps[1];
      string faction = (string)ps[2];
      int squadCount = (int)ps[3];
      float spawntime = (float)ps[4];

      // positioning
      bool hyperspaceIn = (bool)ps[5];
      TV_3DVECTOR position = ((float3)ps[6]).ToVec3();
      TV_3DVECTOR rotation = ((float3)ps[7]).ToVec3();
      SquadFormation squadFormation = (SquadFormation)Enum.Parse(typeof(SquadFormation), (string)ps[8], true);
      float squadDistance = (float)ps[9]; // formation distance

      // ai
      float waitDelay = (float)ps[10];
      TargetType targetType = (TargetType)Enum.Parse(typeof(TargetType), (string)ps[11], true);

      // registries // TO-DO: Remove, replace registries
      string[] registries = (ps.Length > 12) ? (string[])ps[12] : null;

      SquadSpawnInfo sinfo = new SquadSpawnInfo(
        squadName,
        context.Engine.ActorTypeFactory.Get(actorType),
        FactionInfo.Factory.Get(faction),
        squadCount,
        waitDelay,
        targetType,
        hyperspaceIn,
        squadFormation,
        rotation,
        squadDistance,
        registries
        );

      ActorInfo[] squad = GSFunctions.Squadron_Spawn(context.Engine, gscenario, position, spawntime, sinfo);
      int[] ret = new int[squad.Length];
      for (int i = 0; i < squad.Length; i++)
        ret[i] = squad[i].ID;

      return new Val(ret);
    }

    /// <summary>
    /// Spawns an actor
    /// </summary>
    /// <param name="context">The game context</param>
    /// <param name="ps">
    ///   Parameters: 
    ///     0 STRING actor_type, 
    ///     1 STRING actor_name,
    ///     2 STRING faction,
    ///     3 STRING sidebar_name,
    ///     4 FLOAT spawntime,
    ///     5 FLOAT3 position, 
    ///     6 FLOAT3 rotation, 
    ///     7 STRING[] registries // TO-DO: Remove, replace registries
    /// </param>
    /// <returns>INT: the ID of the spawned actor. If the spawn failed, returns -1</returns>
    public static Val Spawn(Context context, params Val[] ps)
    {
      ScenarioBase gscenario = context.Engine.GameScenarioManager.Scenario;
      if (gscenario == null)
        return new Val("-1");

      ActorTypeInfo atype = context.Engine.ActorTypeFactory.Get((string)ps[0]);
      string unitname = (string)ps[1];
      FactionInfo faction = FactionInfo.Factory.Get((string)ps[2]);
      string sidebarname = (string)ps[3];
      float spawntime = (float)ps[4];

      // positioning
      TV_3DVECTOR position = ((float3)ps[5]).ToVec3();
      TV_3DVECTOR rotation = ((float3)ps[6]).ToVec3();

      // TO-DO: Remove, replace registries
      string[] registries = (ps.Length > 7) ? (string[])ps[7] : null;

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = atype,
        Name = unitname,
        SidebarName = sidebarname,
        SpawnTime = spawntime,
        Faction = faction,
        Position = position,
        Rotation = rotation,
        Actions = null,
        Registries = registries
      };

      ActorInfo res = asi.Spawn(gscenario);
      if (res == null)
        return new Val("-1");
      return new Val(res.ID);
    }

    public static Val QueueAtSpawner(Context context, int actorID, int spawnerID)
    {
      ActorInfo actor = context.Engine.ActorFactory.Get(actorID);
      ActorInfo spawner = context.Engine.ActorFactory.Get(spawnerID);
      if (context.Engine.GameScenarioManager.Scenario == null || actor == null || spawner == null)
        return new Val();

      spawner.SpawnerInfo.QueueFighter(actor, spawner);
      return Val.NULL;
    }
  }
}
