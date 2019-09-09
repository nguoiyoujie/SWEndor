using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public static class GSFunctions
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

    public struct BoxInfo
    {
      public readonly TV_3DVECTOR Min;
      public readonly TV_3DVECTOR Max;

      public BoxInfo(TV_3DVECTOR min, TV_3DVECTOR max)
      {
        Min = min;
        Max = max;
      } 
    }

    public static ActorInfo[] MultipleSquadron_Spawn(Engine engine, GameScenarioBase scenario, int sets, BoxInfo spawn_vol, float spawndelay, SquadSpawnInfo spawninfo)
    {
      ActorInfo[] ret = new ActorInfo[sets * spawninfo.MemberCount];
      float time = engine.Game.GameTime;
      for (int k = 0; k < sets; k++)
      {
        float fx = (spawn_vol.Min.x == spawn_vol.Max.x) ? spawn_vol.Min.x : spawn_vol.Min.x + (float)engine.Random.NextDouble() * (spawn_vol.Max.x - spawn_vol.Min.x);
        float fy = (spawn_vol.Min.y == spawn_vol.Max.y) ? spawn_vol.Min.y : spawn_vol.Min.y + (float)engine.Random.NextDouble() * (spawn_vol.Max.y - spawn_vol.Min.y);
        float fz = (spawn_vol.Min.z == spawn_vol.Max.z) ? spawn_vol.Min.z : spawn_vol.Min.z + (float)engine.Random.NextDouble() * (spawn_vol.Max.z - spawn_vol.Min.z);

        ActorInfo[] r = Squadron_Spawn(engine, scenario, new TV_3DVECTOR(fx, fy, fz), time, spawninfo);
        for (int i = 0; i < spawninfo.MemberCount; i++)
          ret[spawninfo.MemberCount * k + i] = r[i];
        time += spawndelay;
      }
      return ret;
    }

    public static ActorInfo[] Squadron_Spawn(Engine engine, GameScenarioBase scenario, TV_3DVECTOR position, float spawntime, SquadSpawnInfo spawninfo)
    {
      ActorInfo[] ret = new ActorInfo[spawninfo.MemberCount];
      TV_3DVECTOR[] spawnpos;
      TV_3DVECTOR[] poss = GetMemberPositions(position, spawninfo, out spawnpos);
      AI.Squads.Squadron squad = engine.SquadronFactory.Create();
      for (int i = 0; i < poss.Length; i++)
      {
        ActionInfo[] actions;
        if (spawninfo.HyperspaceIn)
          actions = new ActionInfo[] { new HyperspaceIn(poss[i]), new Wait(spawninfo.WaitDelay), new Hunt(spawninfo.HuntTargetType) };
        else if (spawninfo.HuntTargetType == TargetType.ANY)
          actions = new ActionInfo[] { new Wait(spawninfo.WaitDelay) };
        else
          actions = new ActionInfo[] { new Wait(spawninfo.WaitDelay), new Hunt(spawninfo.HuntTargetType) };

        string name = (spawninfo.SquadName == null || spawninfo.SquadName == "") ? "" : string.Concat(spawninfo.SquadName, " ", i);

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
      TV_3DVECTOR dirs = Utilities.GetDirection(new TV_3DVECTOR(spawninfo.Rotation.x, spawninfo.Rotation.y + 90, spawninfo.Rotation.z));
      TV_3DVECTOR dirf = Utilities.GetDirection(new TV_3DVECTOR(spawninfo.Rotation.x, spawninfo.Rotation.y, spawninfo.Rotation.z));

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
                                    , GameScenarioBase scenario
                                    , TV_3DVECTOR position
                                    , TV_3DVECTOR targetposition
                                    , TV_3DVECTOR facingposition
                                    , float spawndelay
                                    , ShipSpawnInfo spawninfo)
    {
      string name = spawninfo.ShipName ?? "";

      //ActionInfo[] actions;

      List<ActionInfo> actionlist = new List<ActionInfo>();
      if (spawninfo.IntermissionMood != 0) actionlist.Add(new SetMood(spawninfo.IntermissionMood, true));
      if (spawninfo.HyperspaceIn) actionlist.Add(new HyperspaceIn(position));
      if (spawninfo.EnableSpawn) actionlist.Add(new EnableSpawn(true));
      actionlist.Add(new Move(targetposition, spawninfo.TypeInfo.MoveLimitData.MaxSpeed));
      actionlist.Add(new Rotate(facingposition, spawninfo.TypeInfo.MoveLimitData.MinSpeed));
      actionlist.Add(new Lock());

      /*
      if (spawninfo.IntermissionMood != 0)
      {
        if (spawninfo.HyperspaceIn && spawninfo.EnableSpawn)
        {
          actions = new ActionInfo[] {
                                     new SetMood(spawninfo.IntermissionMood, true)
                                   , new HyperspaceIn(position)
                                   , new EnableSpawn(true)
                                   , new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
        else if (spawninfo.HyperspaceIn)
        {
          actions = new ActionInfo[] {
                                     new SetMood(spawninfo.IntermissionMood, true)
                                   , new HyperspaceIn(position)
                                   , new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
        else if (spawninfo.EnableSpawn)
        {
          actions = new ActionInfo[] {
                                     new SetMood(spawninfo.IntermissionMood, true)
                                   , new EnableSpawn(true)
                                   , new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
        else
        {
          actions = new ActionInfo[] {
                                     new SetMood(spawninfo.IntermissionMood, true)
                                   , new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
      }
      else
      {
        if (spawninfo.HyperspaceIn && spawninfo.EnableSpawn)
        {
          actions = new ActionInfo[] {
                                     new HyperspaceIn(position)
                                   , new EnableSpawn(true)
                                   , new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
        else if (spawninfo.HyperspaceIn)
        {
          actions = new ActionInfo[] {
                                     new HyperspaceIn(position)
                                   , new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
        else if (spawninfo.EnableSpawn)
        {
          actions = new ActionInfo[] {
                                     new EnableSpawn(true)
                                   , new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
        else
        {
          actions = new ActionInfo[] {
                                    new Move(targetposition, spawninfo.TypeInfo.MaxSpeed)
                                   , new Rotate(facingposition, spawninfo.TypeInfo.MinSpeed)
                                   , new Lock()
                                   };
        }
      }
      */

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
      if (a.SpawnerInfo != null)
        a.SpawnerInfo.SpawnsRemaining = spawninfo.CarrierSpawns;
      return a;
    }

    public static TV_3DVECTOR GetSpawnPosition(TV_3DVECTOR position, ShipSpawnInfo spawninfo)
    {
      if (spawninfo.HyperspaceIn)
      {
        TV_3DVECTOR dirf = Utilities.GetDirection(new TV_3DVECTOR(spawninfo.Rotation.x, spawninfo.Rotation.y, spawninfo.Rotation.z));
        return position - dirf * 50000;
      }
      else
        return position;
    }
  }
}
