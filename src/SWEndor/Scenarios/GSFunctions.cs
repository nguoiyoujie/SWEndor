using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Core;
using SWEndor.Models;
using Primrose.Primitives.Geometry;
using System.Collections.Generic;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Scenarios
{
  public enum SquadFormation
  {
    LINE,
    VSHAPE,
    HORIZONTAL_SQUARE,
    VERTICAL_SQUARE
  }

  public struct SquadSpawnInfo
  {
    public string SquadName;
    public ActorTypeInfo TypeInfo;
    public FactionInfo Faction;
    public int MemberCount;
    public float WaitDelay;
    public TargetType HuntTargetType;
    public bool HyperspaceIn;
    public SquadFormation Formation;
    public TV_3DVECTOR Rotation;
    public float FormationDistance;
    public string[] Registries;

    public SquadSpawnInfo(string squadName,
                          ActorTypeInfo typeInfo,
                          FactionInfo faction,
                          int memberCount,
                          float waitDelay,
                          TargetType huntTargetType,
                          bool hyperspaceIn,
                          SquadFormation formation,
                          TV_3DVECTOR rotation,
                          float formationDistance,
                          string[] registries
                          )
    {
      SquadName = squadName;
      TypeInfo = typeInfo;
      Faction = faction;
      MemberCount = memberCount;
      WaitDelay = waitDelay;
      HuntTargetType = huntTargetType;
      HyperspaceIn = hyperspaceIn;
      Formation = formation;
      Rotation = rotation;
      FormationDistance = formationDistance;
      Registries = registries;
    }
  }

  public static class GSFunctions
  {
    public static ActorInfo[] MultipleSquadron_Spawn(Engine engine, ScenarioBase scenario, int sets, Box spawn_vol, float spawndelay, SquadSpawnInfo spawninfo)
    {
      ActorInfo[] ret = new ActorInfo[sets * spawninfo.MemberCount];
      float time = engine.Game.GameTime;
      for (int k = 0; k < sets; k++)
      {
        float fx = (spawn_vol.X.Min == spawn_vol.X.Max) ? spawn_vol.X.Min : spawn_vol.X.Min + (float)engine.Random.NextDouble() * (spawn_vol.X.Max - spawn_vol.X.Min);
        float fy = (spawn_vol.Y.Min == spawn_vol.Y.Max) ? spawn_vol.Y.Min : spawn_vol.Y.Min + (float)engine.Random.NextDouble() * (spawn_vol.Y.Max - spawn_vol.Y.Min);
        float fz = (spawn_vol.Z.Min == spawn_vol.Z.Max) ? spawn_vol.Z.Min : spawn_vol.Z.Min + (float)engine.Random.NextDouble() * (spawn_vol.Z.Max - spawn_vol.Z.Min);

        ActorInfo[] r = Squadron_Spawn(engine, scenario, new TV_3DVECTOR(fx, fy, fz), time, spawninfo);
        for (int i = 0; i < spawninfo.MemberCount; i++)
          ret[spawninfo.MemberCount * k + i] = r[i];
        time += spawndelay;
      }
      return ret;
    }

    public static ActorInfo[] Squadron_Spawn(Engine engine, ScenarioBase scenario, TV_3DVECTOR position, float spawntime, SquadSpawnInfo spawninfo)
    {
      ActorInfo[] ret = new ActorInfo[spawninfo.MemberCount];
      TV_3DVECTOR[] spawnpos;
      TV_3DVECTOR[] poss = GetMemberPositions(position, spawninfo, out spawnpos);
      AI.Squads.Squadron squad = engine.SquadronFactory.Create(spawninfo.SquadName);
      for (int i = 0; i < poss.Length; i++)
      {
        ActionInfo[] actions;
        if (spawninfo.HyperspaceIn)
          actions = new ActionInfo[] { HyperspaceIn.GetOrCreate(poss[i]), Wait.GetOrCreate(spawninfo.WaitDelay), Hunt.GetOrCreate(spawninfo.HuntTargetType) };
        else if (spawninfo.HuntTargetType == TargetType.ANY)
          actions = new ActionInfo[] { Wait.GetOrCreate(spawninfo.WaitDelay) };
        else
          actions = new ActionInfo[] { Wait.GetOrCreate(spawninfo.WaitDelay), Hunt.GetOrCreate(spawninfo.HuntTargetType) };

        string name = (spawninfo.SquadName == null || spawninfo.SquadName == "") ? "" : string.Concat(spawninfo.SquadName, " ", i.ToString());

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = spawninfo.TypeInfo,
          Name = name,
          Obsolete_var = name,
          SidebarName = name,
          SpawnTime = spawntime,
          Faction = spawninfo.Faction,
          Position = spawnpos[i],
          Rotation = spawninfo.Rotation,
          Actions = actions,
          Registries = spawninfo.Registries
        };

        ret[i] = asi.Spawn(scenario);
        ret[i].Squad = squad;
      }
      return ret;
    }

    public static TV_3DVECTOR[] GetMemberPositions(TV_3DVECTOR position, SquadSpawnInfo spawninfo, out TV_3DVECTOR[] spawnpos)
    {
      TV_3DVECTOR[] ret = new TV_3DVECTOR[spawninfo.MemberCount];
      spawnpos = new TV_3DVECTOR[spawninfo.MemberCount];
      TV_3DVECTOR dirs = new TV_3DVECTOR(spawninfo.Rotation.x, spawninfo.Rotation.y + 90, spawninfo.Rotation.z).ConvertRotToDir();
      TV_3DVECTOR dirf = new TV_3DVECTOR(spawninfo.Rotation.x, spawninfo.Rotation.y, spawninfo.Rotation.z).ConvertRotToDir();

      for (int m = 0; m < spawninfo.MemberCount; m++)
      {
        switch (spawninfo.Formation)
        {
          case SquadFormation.LINE:
            {
              int relative = (m % 2 == 0) ? (m + 1) / 2 : -(m + 1) / 2;
              ret[m] = position + dirs * relative * spawninfo.FormationDistance;
            }
            break;

          case SquadFormation.VSHAPE:
            {
              int abrelative = (m + 1) / 2;
              int relative = (m % 2 == 0) ? abrelative : -abrelative;
              TV_3DVECTOR dirv = dirs * 0.7f - dirf * 0.3f;
              ret[m] = position + (dirs * 0.7f * relative - dirf * 0.7f * abrelative) * spawninfo.FormationDistance;
            }
            break;

          case SquadFormation.HORIZONTAL_SQUARE:
            {
              int m2 = m / 2;
              float df = (m % 2 == 0) ? 1 / 2 : -1 / 2;
              int relative = (m2 % 2 == 0) ? (m2 + 1) / 2 : -(m2 + 1) / 2;

              ret[m] = position + dirs * relative * spawninfo.FormationDistance + df * dirf;
            }
            break;

          case SquadFormation.VERTICAL_SQUARE:
            {
              int m2 = m / 2;
              float dy = (m % 2 == 0) ? spawninfo.FormationDistance / 2 : -spawninfo.FormationDistance / 2;
              int relative = (m2 % 2 == 0) ? (m2 + 1) / 2 : -(m2 + 1) / 2;

              ret[m] = position + dirs * relative * spawninfo.FormationDistance + new TV_3DVECTOR(0, dy, 0);
            }
            break;
        }

        if (spawninfo.HyperspaceIn)
          spawnpos[m] = ret[m] - dirf * 50000;
        else
          spawnpos[m] = ret[m];
      }

      return ret;
    }


    public struct ShipSpawnInfo
    {
      public string ShipName;
      public ActorTypeInfo TypeInfo;
      public FactionInfo Faction;
      public TV_3DVECTOR Rotation;
      public int CarrierSpawns;
      public bool HyperspaceIn;
      public bool EnableSpawn;
      public int IntermissionMood;
      public string[] Registries;

      public ShipSpawnInfo(string shipName,
                      ActorTypeInfo typeInfo,
                      FactionInfo faction,
                      bool hyperspaceIn,
                      TV_3DVECTOR rotation,
                      int carrierSpawns,
                      bool enableSpawn,
                      int intermissionMood = 0,
                      string[] registries = null
                      )
      {
        ShipName = shipName;
        TypeInfo = typeInfo;
        Faction = faction;
        HyperspaceIn = hyperspaceIn;
        Rotation = rotation;
        CarrierSpawns = carrierSpawns;
        EnableSpawn = enableSpawn;
        IntermissionMood = intermissionMood;
        Registries = registries;
      }
    }

    public static ActorInfo Ship_Spawn(Engine engine
                                    , ScenarioBase scenario
                                    , TV_3DVECTOR position
                                    , TV_3DVECTOR targetposition
                                    , TV_3DVECTOR facingposition
                                    , float spawndelay
                                    , ShipSpawnInfo spawninfo)
    {
      string name = spawninfo.ShipName ?? "";

      List<ActionInfo> actionlist = new List<ActionInfo>();
      if (spawninfo.IntermissionMood != 0) actionlist.Add(SetMood.GetOrCreate((MoodStates)spawninfo.IntermissionMood, true));
      if (spawninfo.HyperspaceIn) actionlist.Add(HyperspaceIn.GetOrCreate(position));
      if (spawninfo.EnableSpawn) actionlist.Add(EnableSpawn.GetOrCreate(true));
      actionlist.Add(Move.GetOrCreate(targetposition, spawninfo.TypeInfo.MoveLimitData.MaxSpeed));
      actionlist.Add(Rotate.GetOrCreate(facingposition, spawninfo.TypeInfo.MoveLimitData.MinSpeed));
      actionlist.Add(Lock.GetOrCreate());

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = spawninfo.TypeInfo,
        Name = name,
        Obsolete_var = name,
        SidebarName = name,
        SpawnTime = engine.Game.GameTime + spawndelay,
        Faction = spawninfo.Faction,
        Position = GetSpawnPosition(position, spawninfo),
        Rotation = new TV_3DVECTOR(),
        Actions = actionlist.ToArray(),
        Registries = spawninfo.Registries
      };

      ActorInfo a = asi.Spawn(scenario);
      a.SpawnerInfo.SpawnsRemaining = spawninfo.CarrierSpawns;
      return a;
    }

    public static TV_3DVECTOR GetSpawnPosition(TV_3DVECTOR position, ShipSpawnInfo spawninfo)
    {
      if (spawninfo.HyperspaceIn)
      {
        TV_3DVECTOR dirf = new TV_3DVECTOR(spawninfo.Rotation.x, spawninfo.Rotation.y, spawninfo.Rotation.z).ConvertRotToDir();
        return position - dirf * 50000;
      }
      else
        return position;
    }
  }
}
