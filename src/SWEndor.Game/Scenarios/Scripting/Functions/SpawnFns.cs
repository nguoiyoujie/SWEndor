using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.Models;
using Primrose.Primitives.ValueTypes;
using System;
using SWEndor.Game.Primitives.Extensions;
using Primrose.Expressions;
using SWEndor.Game.Core;

namespace SWEndor.Game.Scenarios.Scripting.Functions
{
  public static class SpawnFns
  {
    public static Val Squadron_Spawn(IContext context
                               , string actorType
                               , string squadName
                               , string faction
                               , int squadCount
                               , float spawntime
                               , bool hyperspaceIn
                               , float3 position
                               , float3 rotation
                               , string squadFormation
                               , float squadDistance
                               , float waitDelay
                               , string targetType
                               )
    {
      return Squadron_Spawn(context
                               , actorType
                               , squadName
                               , faction
                               , squadCount
                               , spawntime
                               , hyperspaceIn
                               , position
                               , rotation
                               , squadFormation
                               , squadDistance
                               , waitDelay
                               , targetType
                               , null 
                               );
    }

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
    public static Val Squadron_Spawn(IContext context
                                   , string actorType
                                   , string squadName
                                   , string faction
                                   , int squadCount
                                   , float spawntime
                                   , bool hyperspaceIn
                                   , float3 position
                                   , float3 rotation
                                   , string squadFormation
                                   , float squadDistance
                                   , float waitDelay
                                   , string targetType
                                   , string[] registries
                                   )
    {
      Engine e = ((Context)context).Engine;
      ScenarioBase gscenario = e.GameScenarioManager.Scenario;

      TV_3DVECTOR pos = position.ToVec3();
      TV_3DVECTOR rot = rotation.ToVec3();

      SquadFormation sqdFmn = (SquadFormation)Enum.Parse(typeof(SquadFormation), squadFormation, true);
      TargetType tgtType = (TargetType)Enum.Parse(typeof(TargetType), targetType, true);

      // registries // TO-DO: Remove, replace registries
      //string[] registries = (ps.Length > 12) ? (string[])ps[12] : null;

      SquadSpawnInfo sinfo = new SquadSpawnInfo(
        squadName,
        e.ActorTypeFactory.Get(actorType),
        FactionInfo.Factory.Get(faction),
        squadCount,
        waitDelay,
        tgtType,
        hyperspaceIn,
        sqdFmn,
        rot,
        squadDistance,
        registries
        );

      ActorInfo[] squad = GSFunctions.Squadron_Spawn(e, gscenario, pos, spawntime, sinfo);
      int[] ret = new int[squad.Length];
      for (int i = 0; i < squad.Length; i++)
        ret[i] = squad[i].ID;

      return new Val(ret);
    }

    public static Val Spawn(IContext context
                      , string actorType
                      , string actorName
                      , string faction
                      , string sidebarName
                      , float spawntime
                      , float3 position
                      , float3 rotation
                      )
    {
      return Spawn(context
                      , actorType
                      , actorName
                      , faction
                      , sidebarName
                      , spawntime
                      , position
                      , rotation
                      , null
                      );
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
    public static Val Spawn(IContext context
                          , string actorType
                          , string actorName
                          , string faction
                          , string sidebarName
                          , float spawntime
                          , float3 position
                          , float3 rotation
                          , string[] registries
                          )
    {
      Engine e = ((Context)context).Engine;
      ScenarioBase gscenario = e.GameScenarioManager.Scenario;
      if (gscenario == null)
        return new Val("-1");

      TV_3DVECTOR pos = position.ToVec3();
      TV_3DVECTOR rot = rotation.ToVec3();

      // TO-DO: Remove, replace registries

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = e.ActorTypeFactory.Get(actorType),
        Name = actorName,
        SidebarName = sidebarName,
        SpawnTime = spawntime,
        Faction = FactionInfo.Factory.Get(faction),
        Position = pos,
        Rotation = rot,
        Actions = null,
        Registries = registries
      };

      ActorInfo res = asi.Spawn(gscenario);
      if (res == null)
        return new Val("-1");
      return new Val(res.ID);
    }

    public static Val QueueAtSpawner(IContext context, int actorID, int spawnerID)
    {
      Engine e = ((Context)context).Engine;
      ActorInfo actor = e.ActorFactory.Get(actorID);
      ActorInfo spawner = e.ActorFactory.Get(spawnerID);
      if (e.GameScenarioManager.Scenario == null || actor == null || spawner == null)
        return new Val();

      spawner.SpawnerInfo.QueueFighter(actor, spawner);
      return Val.NULL;
    }
  }
}
