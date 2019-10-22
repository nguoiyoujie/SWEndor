using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Primitives.Extensions;
using SWEndor.Sound;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSEndor : GameScenarioBase
  {
    public GSEndor(GameScenarioManager manager) : base(manager)
    {
      Name = "Battle of Endor [Maintenance]";
      Description = "The Rebel fleet, amassed on Sullust, prepares to move to Endor "
                  + "where the Emperor is overseeing the construction of the second "
                  + "Death Star.";
      AllowedWings = new List<ActorTypeInfo> { ActorTypeFactory.Get("XWING")
                                               , ActorTypeFactory.Get("YWING")
                                               , ActorTypeFactory.Get("AWING")
                                               , ActorTypeFactory.Get("BWING")
                                               , ActorTypeFactory.Get("FALC")
                                               , ActorTypeFactory.Get("CORV")
                                               , ActorTypeFactory.Get("VICT")
                                               , ActorTypeFactory.Get("IMPL")
                                               , ActorTypeFactory.Get("TIEX")
                                              };

      AllowedDifficulties = new List<string> { "easy"
                                               , "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private ActorInfo m_ADS = null;
    private ActorInfo m_ADSLS = null;
    private int m_ExecutorStaticID = -1;
    private List<ShipSpawnEventArg> m_pendingSDspawnlist;
    private Dictionary<int, TV_3DVECTOR> m_rebelPosition;
    private int m_SDLeftForShieldDown = 0;

    private float m_Enemy_pull = 0;
    private float m_Enemy_pullrate = 5;

    private int m_ADS_targetID = -1;
    private int m_FalconID = -1;
    private int m_WedgeID = -1;
    private int m_PlayerID = -1;
    private TV_3DVECTOR m_Player_pos;
    private int m_Player_PrimaryWeaponN;
    private int m_Player_SecondaryWeaponN;

    private int m_HomeOneID = -1;

    private float TIESpawnTime = 0;
    private int TIEWaves = 0;
    private int SDWaves = 0;


    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Name = "Red Three";

      Engine.AtmosphereInfo.ShowSun = false;
      Engine.AtmosphereInfo.ShowFlare = false;
      m_rebelPosition = new Dictionary<int, TV_3DVECTOR>();
      m_pendingSDspawnlist = new List<ShipSpawnEventArg>();
    }

    public override void Unload()
    {
      base.Unload();

      m_ADS = null;
      m_rebelPosition = null;
      m_pendingSDspawnlist = null;
    }

    public override void Launch()
    {
      base.Launch();

      Manager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
      Manager.MinBounds = new TV_3DVECTOR(-20000, -1500, -10000);
      Manager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
      Manager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -10000);

      PlayerInfo.Lives = 4;
      PlayerInfo.ScorePerLife = 1000000;
      PlayerInfo.ScoreForNextLife = 1000000;

      switch (Difficulty.ToLower())
      {
        case "mental":
          m_SDLeftForShieldDown = 3;
          break;
        case "hard":
          m_SDLeftForShieldDown = 2;
          break;
        case "easy":
        case "normal":
        default:
          m_SDLeftForShieldDown = 1;
          break;
      }

      MakePlayer = Rebel_MakePlayer;

      if (!Manager.GetGameStateB("rebels_arrive"))
      {
        Manager.SetGameStateB("rebels_arrive", true);

        SoundManager.SetMusic("battle_3_1");
        SoundManager.SetMusicLoop("battle_3_3");

        Manager.AddEvent(Game.GameTime + 0.1f, Rebel_HyperspaceIn);
        Manager.AddEvent(Game.GameTime + 8f, Rebel_SetPositions);
        Manager.AddEvent(Game.GameTime + 8f, Rebel_MakePlayer);
        Manager.AddEvent(Game.GameTime + 9f, Message_01_AllWingsReport);
        Manager.AddEvent(Game.GameTime + 12f, Message_02_RedLeader);
        Manager.AddEvent(Game.GameTime + 13.5f, Message_03_GoldLeader);
        Manager.AddEvent(Game.GameTime + 15f, Message_04_BlueLeader);
        Manager.AddEvent(Game.GameTime + 16.5f, Message_05_GreenLeader);
        Manager.AddEvent(Game.GameTime + 19f, Message_06_Force);
        Manager.AddEvent(Game.GameTime + 24f, Message_07_Break);
        Manager.AddEvent(Game.GameTime + 25.2f, Message_08_Break);
        Manager.AddEvent(Game.GameTime + 26.5f, Message_09_Conf);
        Manager.AddEvent(Game.GameTime + 29.5f, Message_10_Break);
        Manager.AddEvent(Game.GameTime + 30f, Rebel_GiveControl);
        Manager.AddEvent(Game.GameTime + 38f, Message_11_Evasive);
        Manager.AddEvent(Game.GameTime + 42f, Message_12_Trap);
        Manager.AddEvent(Game.GameTime + 50f, Message_13_Fighters);
        Manager.AddEvent(Game.GameTime + 45f, Empire_SpawnStatics);
      }

      Manager.Line1Color = new COLOR(1f, 1f, 0.3f, 1);
      Manager.Line2Color = new COLOR(1f, 1f, 0.3f, 1);
      Manager.Line3Color = new COLOR(0.7f, 1f, 0.3f, 1);

      MainAllyFaction.WingLimit = 75;

      Manager.IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Wedge", new COLOR(0.8f, 0.4f, 0.4f, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Falcon", new COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new COLOR(0, 0.8f, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire_Advanced", new COLOR(0.4f, 0.8f, 0.4f, 1)).AutoAI = true;

      FactionInfo.Factory.Get("Rebels").Allies.Add(FactionInfo.Factory.Get("Rebels_Wedge"));
      FactionInfo.Factory.Get("Rebels").Allies.Add(FactionInfo.Factory.Get("Rebels_Falcon"));
      FactionInfo.Factory.Get("Rebels_Wedge").Allies.Add(FactionInfo.Factory.Get("Rebels"));
      FactionInfo.Factory.Get("Rebels_Wedge").Allies.Add(FactionInfo.Factory.Get("Rebels_Falcon"));
      FactionInfo.Factory.Get("Rebels_Falcon").Allies.Add(FactionInfo.Factory.Get("Rebels"));
      FactionInfo.Factory.Get("Rebels_Falcon").Allies.Add(FactionInfo.Factory.Get("Rebels_Wedge"));

      FactionInfo.Factory.Get("Empire").Allies.Add(FactionInfo.Factory.Get("Empire_Advanced"));
      FactionInfo.Factory.Get("Empire_Advanced").Allies.Add(FactionInfo.Factory.Get("Empire"));

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");

      MainAllyFaction.WingLimitIncludesAllies = true;
      MainAllyFaction.WingSpawnLimit = 22;
      MainEnemyFaction.WingSpawnLimit = 32;
    }

    public override void LoadScene()
    {
      base.LoadScene();

      // Create Endor
      ActorCreationInfo aci_Endor = new ActorCreationInfo(ActorTypeFactory.Get("ENDOR"))
      {
        Position = new TV_3DVECTOR(0, -40000, 0),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        InitialScale = 60
      };
      Engine.ActorFactory.Create(aci_Endor);

      // Create DeathStar
      ActorCreationInfo aci_DS = new ActorCreationInfo(ActorTypeFactory.Get("DSTAR2"))
      {
        Position = new TV_3DVECTOR(0, 5600, 50000),
        Rotation = new TV_3DVECTOR(0, 0, 5),
        InitialScale = 1,
        Faction = MainEnemyFaction
      };
      m_ADS = Engine.ActorFactory.Create(aci_DS);


      // Create DeathStarLaser
      ActorCreationInfo aci_DSLS = new ActorCreationInfo(ActorTypeFactory.Get("DSLSRSRC"))
      {
        Position = new TV_3DVECTOR(650, 1000, 10000),
        Rotation = new TV_3DVECTOR(0, 0, 5),
        InitialScale = 1,
        Faction = MainEnemyFaction
      };
      m_ADSLS = Engine.ActorFactory.Create(aci_DSLS);
    }

    public override void GameTick()
    {
      base.GameTick();
      if (PlayerInfo.Actor != null && PlayerInfo.IsMovementControlsEnabled && Manager.GetGameStateB("in_battle"))
      {
        if (StageNumber == 0)
        {
          StageNumber = 1;
        }
        else if (TIEWaves > 5 && StageNumber == 1)
        {
          StageNumber = 2;
        }
        else if (SDWaves >= 1 && MainEnemyFaction.ShipCount == 0 && m_pendingSDspawnlist.Count == 0 && StageNumber == 2)
        {
          StageNumber = 3;
          Manager.AddEvent(Game.GameTime + 5.5f, Message_14_Interceptors);
        }
        else if (TIEWaves > 11 && StageNumber == 3)
        {
          StageNumber = 4;
        }
        else if (SDWaves >= 2 && MainEnemyFaction.ShipCount == 0 && m_pendingSDspawnlist.Count == 0 && StageNumber == 4)
        {
          StageNumber = 5;
          SoundManager.SetMusic("battle_3_4");
          SoundManager.SetMusicLoop("battle_3_4");
          Manager.AddEvent(Game.GameTime + 5.5f, Message_15_Bombers);
        }
        else if (TIEWaves > 17 && StageNumber == 5)
        {
          StageNumber = 6;
        }

        // Wedge and Falcon
        if (!Manager.GetGameStateB("deathstar_noshield")
          && StageNumber == 4
          && SDWaves >= 2
          && (m_pendingSDspawnlist.Count + MainEnemyFaction.ShipCount) <= m_SDLeftForShieldDown)
        {
          Manager.SetGameStateB("deathstar_noshield", true);
          Manager.AddEvent(Game.GameTime + 3f, Message_30_ShieldDown);
          Manager.AddEvent(Game.GameTime + 7f, Message_31_ResumeAttack);
          Manager.AddEvent(Game.GameTime + 15f, Rebel_DeathStarGo);
          Manager.AddEvent(Game.GameTime + 16f, Rebel_ShipsForward_2);
          Manager.AddEvent(Game.GameTime + 12f, Message_32_Han);
          Manager.AddEvent(Game.GameTime + 17f, Message_33_Han);
          Manager.AddEvent(Game.GameTime + 20f, Message_34_Han);
        }

        // TIE spawn
        if (TIESpawnTime < Game.GameTime)
        {
          int tie = MainEnemyFaction.GetCount(TargetType.FIGHTER, false);
          int sdest = MainEnemyFaction.GetCount(TargetType.SHIP, false);
          if ((tie < 36 && sdest == 0 && StageNumber == 1)
            || (tie < 32 && sdest == 0 && StageNumber == 3)
            || (tie < 28 && sdest == 0)
            || (tie < 14 && sdest > 0))
          {
            TIESpawnTime = Game.GameTime + 10f;

            if (StageNumber == 1 || StageNumber == 3 || StageNumber == 5)
            {
              TIEWaves++;
            }

            if (StageNumber == 1 || StageNumber == 2)
            {
              Rebel_GoBack(0.7f);
              switch (Difficulty.ToLower())
              {
                case "easy":
                  Manager.AddEvent(0, Empire_TIEWave_01, 4);
                  break;
                case "hard":
                  if (TIEWaves % 3 == 1)
                  {
                    Manager.AddEvent(0, Empire_TIEWave_01, 5);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 1);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_01, 4);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
                  }
                  break;
                case "mental":
                  if (TIEWaves % 2 == 1)
                  {
                    Manager.AddEvent(0, Empire_TIEWave_01, 7);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_01, 5);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 3);
                  }
                  break;
                case "normal":
                default:
                  Manager.AddEvent(0, Empire_TIEWave_01, 5);
                  break;
              }
            }
            else if (StageNumber == 3 || StageNumber == 4)
            {
              Rebel_GoBack(0.3f);
              switch (Difficulty.ToLower())
              {
                case "easy":
                  Manager.AddEvent(0, Empire_TIEWave_02, 4);
                  break;
                case "hard":
                  if (TIEWaves % 3 == 1)
                  {
                    Manager.AddEvent(0, Empire_TIEWave_02, 5);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 1);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_02, 4);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
                  }

                  break;
                case "mental":
                  if (TIEWaves % 2 == 1)
                  {
                    Manager.AddEvent(0, Empire_TIEWave_02, 6);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_02, 5);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 3);
                  }
                  break;
                case "normal":
                default:
                  Manager.AddEvent(0, Empire_TIEWave_02, 5);
                  break;
              }
            }
            else if (StageNumber == 5 || StageNumber == 6)
            {
              switch (Difficulty.ToLower())
              {
                case "easy":
                  Manager.AddEvent(0, Empire_TIEWave_01, 2);
                  Manager.AddEvent(0, Empire_TIEWave_03, 2);
                  Manager.AddEvent(0, Empire_TIEBombers, 1);
                  break;
                case "hard":
                  if (TIEWaves % 3 == 1)
                  {
                    Manager.AddEvent(0, Empire_TIEWave_02, 3);
                    Manager.AddEvent(0, Empire_TIEWave_03, 2);
                    Manager.AddEvent(0, Empire_TIEBombers, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 1);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_02, 2);
                    Manager.AddEvent(0, Empire_TIEWave_03, 3);
                    Manager.AddEvent(0, Empire_TIEBombers, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
                  }
                  break;
                case "mental":
                  if (TIEWaves % 2 == 1)
                  {
                    Manager.AddEvent(0, Empire_TIEWave_03, 6);
                    Manager.AddEvent(0, Empire_TIEBombers, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 3);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_03, 5);
                    Manager.AddEvent(0, Empire_TIEBombers, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 3);
                  }
                  break;
                case "normal":
                default:
                  Manager.AddEvent(0, Empire_TIEWave_02, 2);
                  Manager.AddEvent(0, Empire_TIEWave_03, 3);
                  Manager.AddEvent(0, Empire_TIEBombers, 2);
                  break;
              }
            }
          }
        }

        if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.ShipCount < 8)
        {
          if ((!(m_pendingSDspawnlist[0].Info.TypeInfo is ImperialIATI) || MainEnemyFaction.ShipCount < ((StageNumber == 6) ? 4 : 2))
            && (!(m_pendingSDspawnlist[0].Info.TypeInfo is DevastatorATI) || MainEnemyFaction.ShipCount < 2))
          {
            Manager.AddEvent(0, Empire_StarDestroyer_Spawn, m_pendingSDspawnlist[0]);
            m_pendingSDspawnlist.RemoveAt(0);
          }
        }

        if (m_Enemy_pull > 0)
        {
          m_Enemy_pull -= Game.TimeSinceRender * m_Enemy_pullrate;
          foreach (int enemyshipID in MainEnemyFaction.GetActors(TargetType.SHIP, true))
          {
            ActorInfo enemyship = Engine.ActorFactory.Get(enemyshipID);
            if (enemyship != null)
              enemyship.MoveAbsolute(0, 0, Game.TimeSinceRender * m_Enemy_pullrate);
          }
        }

        //Rebel_ForceAwayFromBounds();

        if (StageNumber == 2 && !Manager.GetGameStateB("DS2"))
        {
          Manager.SetGameStateB("DS2", true);
          Manager.AddEvent(Game.GameTime, Empire_DeathStarAttack_01);
          Manager.AddEvent(Game.GameTime + 8f, Message_20_DeathStar);
          Manager.AddEvent(Game.GameTime + 15f, Empire_StarDestroyer_01);
          Manager.AddEvent(Game.GameTime + 20f, Message_21_Close);
          Manager.AddEvent(Game.GameTime + 25f, Rebel_YWingsAttackScan);
          Manager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Manager.MinBounds = new TV_3DVECTOR(-20000, -1500, -17500);
          Manager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Manager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -17500);
        }
        else if (StageNumber == 4 && !Manager.GetGameStateB("DS4"))
        {
          Manager.SetGameStateB("DS4", true);
          Manager.AddEvent(Game.GameTime, Empire_DeathStarAttack_02);
          Manager.AddEvent(Game.GameTime + 16f, Rebel_ShipsForward);
          Manager.AddEvent(Game.GameTime + 13f, Message_22_PointBlank);
          Manager.AddEvent(Game.GameTime + 15f, Empire_StarDestroyer_02);
          Manager.AddEvent(Game.GameTime + 18f, Message_23_Take);
          Manager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Manager.MinBounds = new TV_3DVECTOR(-20000, -1500, -22500);
          Manager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Manager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -22500);
        }
        else if (StageNumber == 6 && !Manager.GetGameStateB("DS6"))
        {
          Manager.SetGameStateB("DS6", true);
          Manager.AddEvent(Game.GameTime, Empire_DeathStarAttack_03);
          Manager.AddEvent(Game.GameTime + 8f, Empire_Executor);
          Manager.AddEvent(Game.GameTime + 13f, Message_40_Focus);
          Manager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Manager.MinBounds = new TV_3DVECTOR(-20000, -1500, -25000);
          Manager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Manager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -25000);
        }
      }

      if (Manager.Scenario.TimeSinceLostWing < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
      {
        Manager.Line1Text = "WINGS: {0}".F(MainAllyFaction.WingLimit);
      }
      else
      {
        Manager.Line1Text = "";
      }

      if (Manager.Scenario.TimeSinceLostShip < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
      {
        Manager.Line2Text = "SHIPS: {0}".F(MainAllyFaction.ShipLimit);
      }
      else
      {
        Manager.Line2Text = "";
      }
    }

    #region Rebellion spawns

    private void Rebel_HyperspaceIn()
    {
      ActorInfo ainfo;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      ActionInfo[] actions;
      string[] registries;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.GameTime;
      float creationDelay = 0.025f;

      ActorTypeInfo type;
      string name;
      string sidebar_name;
      FactionInfo faction;

      // Millennium Falcon
      type = ActorTypeFactory.Get("LANDO");
      name = "LANDO";
      sidebar_name = "FALCON";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Falcon");
      position = new TV_3DVECTOR(0, -10, 350);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x +Engine.Random.Next(-5, 5), position.y +Engine.Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MoveLimitData.MaxSpeed
                                                                  , type.AIData.Move_CloseEnough)
                                 };

      registries = new string[] { "CriticalAllies" };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        SidebarName = sidebar_name,
        SpawnTime = creationTime,
        Faction = faction,
        Position = position + hyperspaceInOffset,
        Rotation = rotation,
        Actions = actions,
        Registries = registries
      }.Spawn(this);

      m_rebelPosition.Add(ainfo.ID, position);
      ainfo.HitEvents += Rebel_CriticalUnitHit;
      ainfo.HitEvents += Rebel_CriticalUnitDanger;
      ainfo.ActorStateChangeEvents += Rebel_CriticalUnitDying;
      m_FalconID = ainfo.ID;

      // Wedge X-Wing
      type = ActorTypeFactory.Get("WEDGE");
      name = "WEDGE";
      sidebar_name = "WEDGE";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Wedge");
      position = new TV_3DVECTOR(70, 20, 250);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x +Engine.Random.Next(-5, 5), position.y +Engine.Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MoveLimitData.MaxSpeed
                                                                  , type.AIData.Move_CloseEnough)
                                 };

      registries = new string[] { "CriticalAllies" };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        SidebarName = sidebar_name,
        SpawnTime = creationTime,
        Faction = faction,
        Position = position + hyperspaceInOffset,
        Rotation = rotation,
        Actions = actions,
        Registries = registries
      }.Spawn(this);

      m_rebelPosition.Add(ainfo.ID, position);
      ainfo.HitEvents += Rebel_CriticalUnitHit;
      ainfo.HitEvents += Rebel_CriticalUnitDanger;
      ainfo.ActorStateChangeEvents += Rebel_CriticalUnitDying;
      m_WedgeID = ainfo.ID;

      // Player X-Wing
      position = new TV_3DVECTOR(0, 0, -25);
      if (PlayerInfo.ActorType == ActorTypeFactory.Get("XWING"))
      {
        position = new TV_3DVECTOR(0, 0, -25);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("YWING"))
      {
        position = new TV_3DVECTOR(-250, 60, -520);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("AWING"))
      {
        position = new TV_3DVECTOR(100, 70, -720);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("BWING"))
      {
        position = new TV_3DVECTOR(-80, 20, -50);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("CORV"))
      {
        position = new TV_3DVECTOR(-20, -420, -30);
      }

      type = PlayerInfo.ActorType;
      name = "(Player)";
      sidebar_name = "";
      creationTime += creationDelay;
      faction = MainAllyFaction;
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x +Engine.Random.Next(-5, 5), position.y +Engine.Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MoveLimitData.MaxSpeed
                                                                  , type.AIData.Move_CloseEnough)
                                 };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        SidebarName = sidebar_name,
        SpawnTime = creationTime,
        Faction = faction,
        Position = position + hyperspaceInOffset,
        Rotation = rotation,
        Actions = actions,
        Registries = null
      }.Spawn(this);

      m_rebelPosition.Add(ainfo.ID, position);
      PlayerInfo.TempActorID = ainfo.ID;
      PlayerCameraInfo.SceneLook.SetPosition_Point(new TV_3DVECTOR(10, -20, 1500));
      PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(ainfo.ID);
      PlayerCameraInfo.SetSceneLook();

      // Mon Calamari (HomeOne)
      type = ActorTypeFactory.Get("MC90");
      name = "Mon Calamari (Home One)";
      sidebar_name = "HOME ONE";
      faction = MainAllyFaction;
      position = new TV_3DVECTOR(1000, -300, 1000);
      TV_3DVECTOR nv = new TV_3DVECTOR(position.x + ((position.x > 0) ? 5 : -5), position.y, -position.z - 3000);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(nv
                                                  , type.MoveLimitData.MaxSpeed
                                                  , type.AIData.Move_CloseEnough
                                                  , false)
                                 , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                    , type.MoveLimitData.MinSpeed
                                                    , type.AIData.Move_CloseEnough
                                                    , false)
                                 , new Lock()
                                 };

      registries = new string[] { "CriticalAllies" };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        SidebarName = sidebar_name,
        SpawnTime = creationTime,
        Faction = faction,
        Position = position + hyperspaceInOffset,
        Rotation = rotation,
        Actions = actions,
        Registries = registries
      }.Spawn(this);

      ainfo.HuntWeight = 15;
      m_rebelPosition.Add(ainfo.ID, position);
      ainfo.ActorStateChangeEvents += Rebel_CriticalUnitDying;
      m_HomeOneID = ainfo.ID;


      // Other units x6
      faction = MainAllyFaction;
      float nvd = 6000;
      List<object[]> spawns = new List<object[]>();
      // X-Wings x6
      spawns.Add(new object[] { new TV_3DVECTOR(60, 30, -390), ActorTypeFactory.Get("XWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, -50, -320), ActorTypeFactory.Get("XWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(110, 20, -340), ActorTypeFactory.Get("XWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 50, -360), ActorTypeFactory.Get("XWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-120, 20, -380), ActorTypeFactory.Get("XWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(70, -20, -400), ActorTypeFactory.Get("XWING") });

      // A-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(200, -10, -750), ActorTypeFactory.Get("AWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(80, 20, -800), ActorTypeFactory.Get("AWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(270, 40, -850), ActorTypeFactory.Get("AWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(65, 65, -900), ActorTypeFactory.Get("AWING") });

      // B-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(-150, 50, -250), ActorTypeFactory.Get("BWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-290, 80, -280), ActorTypeFactory.Get("BWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, 100, -350), ActorTypeFactory.Get("BWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-200, 65, -400), ActorTypeFactory.Get("BWING") });

      // Y-Wings x6
      spawns.Add(new object[] { new TV_3DVECTOR(-10, 100, -350), ActorTypeFactory.Get("YWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(50, 80, -380), ActorTypeFactory.Get("YWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 90, -420), ActorTypeFactory.Get("YWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(-90, 100, -440), ActorTypeFactory.Get("YWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(75, 110, -450), ActorTypeFactory.Get("YWING") });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 85, -500), ActorTypeFactory.Get("YWING") });

      // Corellian x9 (5 forward, 4 rear)
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, -120, 1300), ActorTypeFactory.Get("CORV"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1400, -320, 400), ActorTypeFactory.Get("CORV"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2400, 150, 1500), ActorTypeFactory.Get("CORV"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 470, 850), ActorTypeFactory.Get("CORV"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 300, 200), ActorTypeFactory.Get("CORV"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(600, 70, -150), ActorTypeFactory.Get("CORV"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-700, -150, -600), ActorTypeFactory.Get("CORV"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-600, 250, -1200), ActorTypeFactory.Get("CORV"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, 200, -1200), ActorTypeFactory.Get("CORV"), 5500 });

      // Mon Calamari x2 (not including HomeOne)
      spawns.Add(new object[] { new TV_3DVECTOR(4500, -120, -500), ActorTypeFactory.Get("MC90"), 7200 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 550, -1500), ActorTypeFactory.Get("MC90"), 7200 });

      // Nebulon B x3
      spawns.Add(new object[] { new TV_3DVECTOR(-2700, 320, -1850), ActorTypeFactory.Get("NEBL"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1800, -280, -900), ActorTypeFactory.Get("NEBL"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 850, -150), ActorTypeFactory.Get("NEBL"), 8000 });

      // Transport x9
      spawns.Add(new object[] { new TV_3DVECTOR(1200, 550, -750), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1750, 350, 150), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, 280, -1200), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(4000, 280, -800), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-500, -175, -1600), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1650, -230, -800), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 450, 0), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, -280, -1200), ActorTypeFactory.Get("TRAN"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-900, 10, -400), ActorTypeFactory.Get("TRAN"), 8000 });

      foreach (object[] spawn in spawns)
      {
        type = (ActorTypeInfo)spawn[1];
        int huntw = 5;
        creationTime += creationDelay;
        position = (TV_3DVECTOR)spawn[0];
        if (type.AIData.TargetType.Has(TargetType.FIGHTER))
        {
          actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x +Engine.Random.Next(-5, 5), position.y +Engine.Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MoveLimitData.MaxSpeed
                                                                  , type.AIData.Move_CloseEnough)
                                 };
          if (type is LandoFalconATI || type is WedgeXWingATI)
            huntw = 5;
          else
            huntw = 10;
        }
        else
        {
          if (spawn.Length < 3 || !(spawn[2] is float))
            nvd = 6000;
          else
            nvd = (float)spawn[2];

          nv = new TV_3DVECTOR(position.x + ((position.x > 0) ? 5 : -5), position.y, -position.z - nvd);
          actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(nv
                                                  , type.MoveLimitData.MaxSpeed
                                                  , type.AIData.Move_CloseEnough
                                                  , false)
                                 , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                    , type.MoveLimitData.MinSpeed
                                                    , type.AIData.Move_CloseEnough
                                                    , false)
                                 , new Lock()
                                 };

          if (type is TransportATI)
            huntw = 5;
          else if (type is CorellianATI)
            huntw = 15;
          else
            huntw = 25;
        }

        ainfo = new ActorSpawnInfo
        {
          Type = type,
          Name = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = faction,
          Position = position + hyperspaceInOffset,
          Rotation = rotation,
          Actions = actions,
        }.Spawn(this);

        ainfo.HuntWeight = huntw;
        m_rebelPosition.Add(ainfo.ID, position);
      }

      MainAllyFaction.ShipLimit = MainAllyFaction.ShipCount;
    }

    private void Rebel_SetPositions()
    {
      foreach (int id in m_rebelPosition.Keys)
      {
        ActorInfo actor = Engine.ActorFactory.Get(id);
        if (actor != null)
        {
          actor.Position = m_rebelPosition[id] + new TV_3DVECTOR(0, 0, actor.TypeInfo.MoveLimitData.MaxSpeed * 8f);
          actor.MoveData.Speed = actor.TypeInfo.MoveLimitData.MaxSpeed;
        }
      }
    }

    private void Rebel_HyperspaceOut()
    {
      foreach (int actorID in MainAllyFaction.GetActors(TargetType.FIGHTER, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.ForceClearQueue();
          actor.QueueLast(new Rotate(actor.GetGlobalPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                        , actor.MoveData.MaxSpeed
                                                        , actor.TypeInfo.AIData.Move_CloseEnough));
          actor.QueueLast(new HyperspaceOut());
          actor.QueueLast(new Delete());
        }
      }
      foreach (int actorID in MainAllyFaction.GetActors(TargetType.SHIP, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.ForceClearQueue();
          actor.QueueLast(new Rotate(actor.GetGlobalPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                , actor.MoveData.MaxSpeed
                                                , actor.TypeInfo.AIData.Move_CloseEnough));
          actor.QueueLast(new HyperspaceOut());
          actor.QueueLast(new Delete());
        }
      }
    }

    public void Rebel_MakePlayer()
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;
      PlayerCameraInfo.SetPlayerLook();

      if (PlayerInfo.Actor != null && !PlayerInfo.Actor.Disposed)
      {
        // m_Player = Player.Actor;
        if (!Manager.GetGameStateB("in_battle"))
        {
          foreach (int actorID in MainAllyFaction.GetActors(TargetType.SHIP, true))
          {
            ActorInfo actor = Engine.ActorFactory.Get(actorID);
            if (actor != null)
            {
              actor.MoveData.FreeSpeed = true;
              actor.MoveData.Speed = 275;
            }
          }

          foreach (int actorID in MainAllyFaction.GetActors(TargetType.FIGHTER, true))
          {
            ActorInfo actor = Engine.ActorFactory.Get(actorID);
            if (actor != null)
            {
              actor.MoveData.FreeSpeed = true;
              if (actor.MoveData.Speed < 425)
                actor.MoveData.Speed = 425;
            }
          }
        }
      }
      else
      {
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;
          PlayerInfo.RequestSpawn = true;
        }
      }
    }

    public void Rebel_ShipsForward()
    {
      foreach (int actorID in MainAllyFaction.GetActors(TargetType.SHIP, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.ForceClearQueue();
          actor.QueueLast(new Rotate(actor.GetGlobalPosition() - new TV_3DVECTOR(actor.GetGlobalPosition().x * 0.35f, 0, Math.Abs(actor.GetGlobalPosition().x) + 1500)
                                                    , actor.MoveData.MinSpeed));
          actor.QueueLast(new Lock());
        }
      }
    }

    public void Rebel_ShipsForward_2()
    {
      foreach (int actorID in MainAllyFaction.GetActors(TargetType.SHIP, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.ForceClearQueue();
          actor.QueueLast(new Move(actor.GetGlobalPosition() - new TV_3DVECTOR(actor.GetGlobalPosition().x * 0.35f, 0, Math.Abs(actor.GetGlobalPosition().x) + 1500)
                                                    , actor.MoveData.MaxSpeed));
          actor.QueueLast(new Rotate(actor.GetGlobalPosition() - new TV_3DVECTOR(0, 0, 20000)
                                                    , actor.MoveData.MinSpeed));
          actor.QueueLast(new Lock());
        }
      }
    }

    public void Rebel_YWingsAttackScan()
    {
      if (MainEnemyFaction.ShipCount > 0)
      {
        foreach (int actorID in MainAllyFaction.GetActors(TargetType.FIGHTER, true))
        {
          ActorInfo actor = Engine.ActorFactory.Get(actorID);
          if (actor != null)
          {
            if (actor.TypeInfo is YWingATI || actor.TypeInfo is BWingATI)
            {
              int rsID = MainEnemyFaction.GetRandom(Engine, TargetType.SHIP);
              ActorInfo rs = Engine.ActorFactory.Get(actorID);
              {
                foreach (ActorInfo c in rs.Children)
                {
                  if (c.TypeInfo.AIData.TargetType.Contains(TargetType.SHIELDGENERATOR))
                    if (Engine.Random.NextDouble() > 0.4f)
                      rsID = c.ID;
                }
              }

              actor.ClearQueue();
              actor.QueueLast(AttackActor.GetOrCreate(rsID, -1, -1, false));
            }
          }
        }
      }

      Manager.AddEvent(Game.GameTime + 5f, Rebel_YWingsAttackScan);
    }

    public void Rebel_DeathStarGo()
    {
      ActorInfo falcon = Engine.ActorFactory.Get(m_FalconID);
      if (falcon != null)
      {
        falcon.SetArmor(DamageType.ALL, 0);
        falcon.ForceClearQueue();
        falcon.QueueLast(new ForcedMove(new TV_3DVECTOR(0, 0, 20000), falcon.MoveData.MaxSpeed, -1));
        falcon.QueueLast(new HyperspaceOut());
        falcon.QueueLast(new Delete());
        m_FalconID = -1;
      }

      ActorInfo wedge = Engine.ActorFactory.Get(m_WedgeID);
      if (wedge != null)
      {
        wedge.SetArmor(DamageType.NORMAL, 0);
        wedge.ForceClearQueue();
        wedge.QueueLast(new ForcedMove(new TV_3DVECTOR(0, 0, 20000), wedge.MoveData.MaxSpeed, -1));
        wedge.QueueLast(new HyperspaceOut());
        wedge.QueueLast(new Delete());
        m_WedgeID = -1;
      }
    }

    public void Rebel_GiveControl()
    {
      ActorInfo falcon = Engine.ActorFactory.Get(m_FalconID);
      falcon.UnlockOne();
      falcon.MoveData.FreeSpeed = false;
      falcon.MoveData.Speed = falcon.MoveData.MaxSpeed;

      ActorInfo wedge = Engine.ActorFactory.Get(m_WedgeID);
      wedge.UnlockOne();
      wedge.QueueFirst(new Wait(0.5f));
      wedge.MoveData.FreeSpeed = false;
      wedge.MoveData.Speed = wedge.MoveData.MaxSpeed;

      float time = 3f;
      foreach (int actorID in MainAllyFaction.GetActors(TargetType.FIGHTER, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);

        actor.UnlockOne();
        actor.QueueFirst(new Wait(time));
        actor.MoveData.FreeSpeed = false;
        actor.MoveData.Speed = actor.MoveData.MaxSpeed;
        time = (float)Engine.Random.NextDouble() * 3f;
      }

      foreach (int actorID in MainAllyFaction.GetActors(TargetType.SHIP, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);

        actor.UnlockOne();
        actor.MoveData.FreeSpeed = false;
        actor.MoveData.Speed = actor.MoveData.MaxSpeed;
        actor.SetSpawnerEnable(true);
      }
      PlayerInfo.IsMovementControlsEnabled = true;

      Manager.SetGameStateB("in_battle", true);
      Manager.SetGameStateB("TIEs", true);
      Manager.AddEvent(Game.GameTime + 10f, Empire_FirstTIEWave);
      Manager.AddEvent(Game.GameTime + 45f, Empire_SecondTIEWave);
      Rebel_RemoveTorps();
    }

    private void Rebel_GoBack(float chance)
    {
      foreach (int actorID in MainAllyFaction.GetActors(TargetType.FIGHTER, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          double d = Engine.Random.NextDouble();
          if (d < chance)
          {
            ActorInfo homeone = Engine.ActorFactory.Get(m_HomeOneID);

            actor.ClearQueue();
            actor.QueueLast( new Move(homeone.GetGlobalPosition() + new TV_3DVECTOR(Engine.Random.Next(-2500, 2500), Engine.Random.Next(-50, 50), Engine.Random.Next(-2500, 2500))
                           , actor.MoveData.MaxSpeed));
          }
        }
      }
    }

    public void Rebel_RemoveTorps()
    {
      foreach (int actorID in MainAllyFaction.GetActors(TargetType.FIGHTER, true))
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (!actor.IsPlayer)
          {
            foreach (WeaponInfo w in actor.WeaponDefinitions.Weapons)
            {
              if (w.Type == WeaponType.TORPEDO)
              {
                w.Ammo = 2;
                w.MaxAmmo = 2;
              }
            }
          }
        }
      }
    }

    public void Rebel_CriticalUnitHit(ActorInfo actor)
    {
      if (actor != null
        && actor.HP_Frac < 0.8f
        && MainAllyFaction.ShipCount > 0)
      {
        ActorInfo homeone = Engine.ActorFactory.Get(m_HomeOneID);

        actor.ClearQueue();
        actor.QueueLast(new Move(homeone.GetGlobalPosition() + new TV_3DVECTOR(Engine.Random.Next(-2500, 2500)
                    , Engine.Random.Next(-50, 50)
                    , Engine.Random.Next(-2500, 2500))
                    , actor.MoveData.MaxSpeed));
      }
    }

    public void Rebel_CriticalUnitDanger(ActorInfo actor)
    {
      float f = actor.HP_Frac;
      if (f < 0.67f && f >= 0.33f)
      {
        Screen2D.MessageText(string.Format("{0}: {1}, I need cover!", actor.Name, PlayerInfo.Name)
                                            , 5
                                            , actor.Faction.Color
                                            , 100);
      }
      else if (f < 0.33f)
      {
        Screen2D.MessageText(string.Format("{0}: {1}, Get those TIEs off me!", actor.Name, PlayerInfo.Name)
                                            , 5
                                            , actor.Faction.Color
                                            , 100);
      }
    }

    public void Rebel_CriticalUnitDying(ActorInfo actor, ActorState state)
    {
      if (Manager.GetGameStateB("GameWon"))
        return;

      if (Manager.GetGameStateB("GameOver"))
        return;

      if (actor != null
        && actor.IsDyingOrDead)
      {
        PlayerInfo.TempActorID = actor.ID;

        Manager.SetGameStateB("GameOver", true);
        Manager.IsCutsceneMode = true;

        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.DeathLook.SetPosition_Actor(actor.ID, actor.TypeInfo.DeathCamera);
        PlayerCameraInfo.SetDeathLook();

        if (actor.IsDying)
        {
          actor.TickEvents += ProcessPlayerDying;
          actor.DestroyedEvents += ProcessPlayerKilled;
        }
        else
        {
          actor.DestroyedEvents += ProcessPlayerKilled;
        }

        if (actor.TypeInfo is WedgeXWingATI)
        {
          Manager.AddEvent(Game.GameTime, Message_90_LostWedge);
        }
        else if (actor.TypeInfo is LandoFalconATI)
        {
          Manager.AddEvent(Game.GameTime, Message_91_LostFalcon);
        }
        else if (actor.TypeInfo is MC90ATI)
        {
          Manager.AddEvent(Game.GameTime + 15, Message_92_LostHomeOne);
          actor.DyingTimerSet(2000, true);
          Manager.AddEvent(Game.GameTime + 25, FadeOut);
        }
      }
    }

    #endregion


    #region Empire spawns
    public void Empire_FirstTIEWave()
    {

      // TIEs
      float t = 0;
      for (int k = 1; k < 9; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);
        float fz = Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = null;
            switch (Difficulty.ToLower())
            {
              case "mental":
              case "hard":
                actions = new ActionInfo[] { Hunt.GetOrCreate(TargetType.FIGHTER) };
                break;
            }

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              SidebarName = "",
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, fz - 8000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 0.5f;
      }
    }

    public void Empire_SecondTIEWave()
    {
      // TIEs
      float t = 0;
      for (int k = 1; k < 3; k++)
      {
        float fx = Engine.Random.Next(-4500, 4500);
        float fy = Engine.Random.Next(-500, 500);
        float fz = Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = null;
            switch (Difficulty.ToLower())
            {
              case "mental":
              case "hard":
                actions = new ActionInfo[] { Hunt.GetOrCreate(TargetType.FIGHTER) };
                break;
            }

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              SidebarName = "",
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, fz - 8000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 0.5f;
      }
    }

    public void Empire_TIEWave_01(int sets)
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, Manager.MinBounds.z - 2500), new TV_3DVECTOR(2500, 500, Manager.MinBounds.z - 3500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 18
                                                                          , TargetType.FIGHTER
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 0.25f, spawninfo);
    }

    public void Empire_TIEWave_02(int sets)
    {
      int setI = sets / 2;
      sets -= setI;
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, Manager.MinBounds.z - 2500), new TV_3DVECTOR(2500, 500, Manager.MinBounds.z - 3500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 18
                                                                          , TargetType.FIGHTER
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("TIEI");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, setI, box, 1.5f, spawninfo);
    }

    public void Empire_TIEWave_03(int sets)
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, Manager.MinBounds.z - 2500), new TV_3DVECTOR(2500, 500, Manager.MinBounds.z - 3500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIEI")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 18
                                                                          , TargetType.FIGHTER
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 0.25f, spawninfo);
    }

    public void Empire_TIEWave_0D(int sets)
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, Manager.MinBounds.z - 2500), new TV_3DVECTOR(2500, 500, Manager.MinBounds.z - 3500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIED")
                                                                          , MainEnemyFaction
                                                                          , 2
                                                                          , 8
                                                                          , TargetType.FIGHTER
                                                                          , false
                                                                          , GSFunctions.SquadFormation.LINE
                                                                          , new TV_3DVECTOR()
                                                                          , 1000
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 0.25f, spawninfo);
    }

    public void Empire_TIEWave_TIEsvsShips(int sets)
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, Manager.MinBounds.z - 2500), new TV_3DVECTOR(2500, 500, Manager.MinBounds.z - 3500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 18
                                                                          , TargetType.SHIP
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
    }

    public void Empire_TIEBombers(int sets)
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, Manager.MinBounds.z - 2500), new TV_3DVECTOR(2500, 500, Manager.MinBounds.z - 3500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIESA")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 18
                                                                          , TargetType.SHIP
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
    }

    public void Empire_SpawnStatics()
    {
      ActorInfo ainfo;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      float creationTime = Game.GameTime;

      ActorTypeInfo type;
      string name;

      List<object[]> spawns = new List<object[]>();
      spawns.Add(new object[] { new TV_3DVECTOR(0, 50, -33500), ActorTypeFactory.Get("EXECSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(-1500, 180, -32500), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(1500, 180, -32500), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, 280, -27500), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(5500, 280, -27500), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(-5500, -30, -29500), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, -30, -29500), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(-7500, 130, -31000), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(7500, 130, -31000), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(-11500, -80, -29000), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(11500, -80, -29000), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(-14500, 80, -27500), ActorTypeFactory.Get("IMPLSTS") });
      spawns.Add(new object[] { new TV_3DVECTOR(14500, 80, -27500), ActorTypeFactory.Get("IMPLSTS") });

      foreach (object[] spawn in spawns)
      {
        type = (ActorTypeInfo)spawn[1];
        name = type.ID;
        position = (TV_3DVECTOR)spawn[0];

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = type,
          Name = name,
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = null,
          Position = position,
          Rotation = rotation,
        };

        ainfo = asi.Spawn(this);

        if (ainfo.TypeInfo is ExecutorStaticATI)
          m_ExecutorStaticID = ainfo.ID;
      }
    }

    public void Empire_StarDestroyer_Spawn(ShipSpawnEventArg s)
    {
      ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);

      if (s.Info.TypeInfo is DevastatorATI)
        Manager.AddEvent(0, Empire_TIEWave_0D, 2);
    }

    public void Empire_StarDestroyer_01()
    {
      SDWaves++;

      m_Enemy_pull = 8000;
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                                    , ActorTypeFactory.Get("IMPL")
                                                                    , MainEnemyFaction
                                                                    , true
                                                                    , new TV_3DVECTOR()
                                                                    , 90
                                                                    , true
                                                                    , 0
                                                                    , new string[] { "CriticalEnemies" }
                                                                    );

      switch (Difficulty.ToLower())
      {
        case "easy":
          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 110, -17500)
                                                        , new TV_3DVECTOR(0, 110, -9000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2500, -250, -19500)
                                                        , new TV_3DVECTOR(-2500, -250, -9500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "mental":
          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 110, -17500)
                                                        , new TV_3DVECTOR(0, 110, -9000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-750, -50, -19000)
                                                        , new TV_3DVECTOR(-750, -50, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(750, -50, -19000)
                                                        , new TV_3DVECTOR(750, -50, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-1500, 150, -18500)
                                                        , new TV_3DVECTOR(-1000, 210, -7500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 150, -18500)
                                                        , new TV_3DVECTOR(1000, 210, -7500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2250, -50, -19000)
                                                        , new TV_3DVECTOR(-1250, -50, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2250, -50, -19000)
                                                        , new TV_3DVECTOR(-1250, -50, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "hard":
        case "normal":
        default:
          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 110, -17500)
                                                        , new TV_3DVECTOR(0, 110, -9000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(2500, -250, -19500)
                                                        , new TV_3DVECTOR(2000, 110, -9500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2500, -250, -19500)
                                                        , new TV_3DVECTOR(-2000, 110, -9500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-1500, 150, -18500)
                                                        , new TV_3DVECTOR(-1000, 210, -7500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 150, -18500)
                                                        , new TV_3DVECTOR(1000, 210, -7500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
      }
      SoundManager.SetMusic("battle_3_2");
    }

    public void Empire_StarDestroyer_02()
    {
      SDWaves++;

      m_Enemy_pull = 12000;
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("IMPL")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 90
                                                              , true
                                                              , 0
                                                              , new string[] { "CriticalEnemies" }
                                                              );

      switch (Difficulty.ToLower())
      {
        case "easy":
          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 60, -22500)
                                                        , new TV_3DVECTOR(0, 60, -12000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-1500, 150, -18500)
                                                        , new TV_3DVECTOR(-1000, 210, -7500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 150, -18500)
                                                        , new TV_3DVECTOR(1000, 210, -7500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-7500, 60, -24500)
                                                        , new TV_3DVECTOR(-2000, 60, -12500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "mental":
          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 60, -22500)
                                                        , new TV_3DVECTOR(0, 60, -12000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-10000, -300, -23500), new TV_3DVECTOR(-1250, -300, -10500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("DEVA");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 120, -24500), new TV_3DVECTOR(0, 120, -11500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-2500, 150, -24500), new TV_3DVECTOR(-2000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-3200, 50, -12000)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(2500, 150, -24500), new TV_3DVECTOR(2000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(3200, 50, -12000)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(10000, -300, -23500), new TV_3DVECTOR(1250, -300, -10500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "hard":
          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 60, -22500)
                                                        , new TV_3DVECTOR(0, 60, -12000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(2500, 150, -26500), new TV_3DVECTOR(500, 100, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(4500, -100, -24500), new TV_3DVECTOR(2000, -100, -12500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(7500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "normal":
        default:
          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(0, 60, -22500)
                                                        , new TV_3DVECTOR(0, 60, -12000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(500, 150, -26500), new TV_3DVECTOR(500, 100, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                      , new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500)
                                                      , new TV_3DVECTOR(0, 0, 99999)
                                                      ));

          break;
      }

      SoundManager.SetMusic("battle_3_2");
    }

    public void Empire_Executor()
    {
      //m_Enemy_pull = 4000;
      SDWaves++;
      ActorInfo executor = Engine.ActorFactory.Get(m_ExecutorStaticID);
      executor?.Delete();

      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.GameTime;

      TV_3DVECTOR position = new TV_3DVECTOR(0, -950, -20000);
      ActorTypeInfo atinfo = ActorTypeFactory.Get("EXEC");

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = atinfo,
        Name = "",
        SidebarName = "EXECUTOR",
        SpawnTime = Game.GameTime,
        Faction = MainEnemyFaction,
        Position = position + hyperspaceInOffset,
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[]
                           {
                             new HyperspaceIn(position)
                             , new Move(new TV_3DVECTOR(position.x, position.y, -4000), atinfo.MoveLimitData.MaxSpeed)
                             , new Rotate(position + new TV_3DVECTOR(0, 0, 22500), atinfo.MoveLimitData.MinSpeed)
                             , new Lock()
                           },
        Registries = new string[] { "CriticalEnemies" }
      };

      ActorInfo ainfo = ainfo = asi.Spawn(this);
      ainfo.HuntWeight = 5;
      ainfo.ActorStateChangeEvents += Empire_ExecutorDestroyed;

      // SD
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("IMPL")
                                                        , MainEnemyFaction
                                                        , true
                                                        , new TV_3DVECTOR()
                                                        , 90
                                                        , true
                                                        , 0
                                                        , new string[] { "CriticalEnemies" }
                                                        );

      switch (Difficulty.ToLower())
      {
        case "easy":
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 10);
          break;
        case "mental":
          Manager.AddEvent(0, Empire_TIEWave_0D, 2);

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-550, 80, -7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(550, -160, -7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-1500, 90, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(1500, 90, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -80, -24000), new TV_3DVECTOR(-2000, -80, -7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(2000, -80, -24000), new TV_3DVECTOR(2000, -80, -7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));
          break;
        case "hard":
          Manager.AddEvent(0, Empire_TIEWave_0D, 1);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 5);

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-2500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(2500, -80, -7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "normal":
        default:
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 10);

          sspawn.TypeInfo = ActorTypeFactory.Get("IMPL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));
          break;
      }
    }

    public void Empire_ExecutorDestroyed(ActorInfo actor, ActorState state)
    {
      if (Manager.GetGameStateB("GameOver"))
        return;

      if (actor.IsDyingOrDead)
      {
        Manager.SetGameStateB("GameWon", true);
        Manager.IsCutsceneMode = true;

        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.DeathLook.SetPosition_Actor(actor.ID, actor.TypeInfo.DeathCamera);
        PlayerCameraInfo.SetDeathLook();

        if (actor.IsDying)
        {
          actor.TickEvents += ProcessPlayerDying;
          actor.DestroyedEvents += ProcessPlayerKilled;
        }
        else
        {
          actor.DestroyedEvents += ProcessPlayerKilled;
        }

        SoundManager.SetMusic("executorend");
        actor.DyingTimerSet(2000, true);

        ActorInfo homeone = Engine.ActorFactory.Get(m_HomeOneID);
        if (homeone != null)
          homeone.SetArmor(DamageType.ALL, 0);
        Manager.AddEvent(Game.GameTime + 55, FadeOut);
      }
    }

    public void Empire_DeathStarAttack_01()
    {
      Empire_DeathStarAttack(ActorTypeFactory.Get("CORV"));
    }

    public void Empire_DeathStarAttack_02()
    {
      Empire_DeathStarAttack(ActorTypeFactory.Get("TRAN"));
    }

    public void Empire_DeathStarAttack_03()
    {
      Empire_DeathStarAttack(ActorTypeFactory.Get("MC90"));
    }

    private void Empire_DeathStarAttack(ActorTypeInfo type)
    {
      if (m_ADSLS != null
        && m_ADSLS.Active)
      {
        foreach (int tid in MainAllyFaction.GetActors(TargetType.SHIP, true))
        {
          ActorInfo t = Engine.ActorFactory.Get(tid);
          if (t != null
            && t.TypeInfo == type
            && tid != m_HomeOneID)
          {
            m_ADS_targetID = tid;

            m_ADSLS.ForceClearQueue();
            m_ADSLS.QueueNext(AttackActor.GetOrCreate(tid));
            m_ADSLS.QueueNext(new Lock());

            t.DestroyedEvents += DeathStarKill_Effect;
            Manager.AddEvent(0.1f, Scene_DeathStarCam);
            return;
          }
        }
      }
    }

    #endregion

    #region Text
    COLOR color_lando = new COLOR(0.8f, 0.8f, 0.8f, 1);
    COLOR color_wedge = new COLOR(0.8f, 0.4f, 0.4f, 1);
    COLOR color_ywing = new COLOR(0.6f, 0.6f, 0.6f, 1);
    COLOR color_bwing = new COLOR(0.6f, 0.6f, 0.9f, 1);
    COLOR color_awing = new COLOR(0.4f, 0.8f, 0.4f, 1);
    COLOR color_ackbar = new COLOR(0.2f, 0.4f, 1, 1);

    public void Message_01_AllWingsReport()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: All wings report in.", 5, color_lando);
    }

    public void Message_02_RedLeader()
    {
      Screen2D.MessageText("X-WING (WEDGE): Red Leader standing by.", 5, color_wedge);
    }

    public void Message_03_GoldLeader()
    {
      Screen2D.MessageText("Y-WING: Gray Leader standing by.", 5, color_ywing);
    }

    public void Message_04_BlueLeader()
    {
      Screen2D.MessageText("B-WING: Blue Leader standing by.", 5, color_bwing);
    }

    public void Message_05_GreenLeader()
    {
      Screen2D.MessageText("A-WING: Green Leader standing by.", 5, color_awing);
    }

    public void Message_06_Force()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): May the Force be with us.", 5, color_ackbar);
    }

    public void Message_07_Break()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Break off the attack!", 5, color_lando);
    }

    public void Message_08_Break()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: The shield's still up!", 5, color_lando);
    }

    public void Message_09_Conf()
    {
      Screen2D.MessageText("X-WING (WEDGE): I get no reading, are you sure?", 5, color_wedge);
    }

    public void Message_10_Break()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: All wings, pull up!", 5, color_lando);
    }

    public void Message_11_Evasive()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): Take evasive action!", 5, color_ackbar);
    }

    public void Message_12_Trap()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): It's a trap!", 5, color_ackbar);
    }

    public void Message_13_Fighters()
    {
      Screen2D.MessageText("X-WING (WEDGE): Watch out for enemy fighters.", 5, color_wedge);
    }

    public void Message_14_Interceptors()
    {
      Screen2D.MessageText("X-WING (WEDGE): TIE Interceptors inbound.", 5, color_wedge);
    }

    public void Message_15_Bombers()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): We have bombers inbound. Keep them away from our cruisers!", 5, color_ackbar);
    }

    public void Message_20_DeathStar()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: That blast came from the Death Star...", 5, color_lando);
    }

    public void Message_21_Close()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Get us close to those Imperial Star Destroyers.", 5, color_lando);
    }

    public void Message_22_PointBlank()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Closer! Get us closer, and engage them at point blank range.", 5, color_lando);
    }

    public void Message_23_Take()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: If they fire, we might even take a few of them with us.", 5, color_lando);
    }

    public void Message_30_ShieldDown()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): The shield is down.", 5, color_ackbar);
    }

    public void Message_31_ResumeAttack()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): Commence your attack on the Death Star's main reactor", 5, color_ackbar);
    }

    public void Message_32_Han()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: I knew Han can do it!", 5, color_lando);
    }

    public void Message_33_Han()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Wedge, follow me.", 5, color_lando);
    }

    public void Message_34_Han()
    {
      Screen2D.MessageText("X-WING (WEDGE): Copy, Gold Leader.", 5, color_wedge);
    }

    public void Message_40_Focus()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): Focus all firepower on that Super Star Destroyer!", 5, color_ackbar);
    }

    public void Message_90_LostWedge()
    {
      Screen2D.MessageText("X-WING (WEDGE): I can't hold them!", 5, color_wedge);
    }

    public void Message_91_LostFalcon()
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: I can't hold them!", 5, color_lando);
    }

    public void Message_92_LostHomeOne()
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): We have no chance...", 15, color_ackbar);
    }

    #endregion

    #region Scene
    public void Scene_EnterCutscene()
    {
      ActorInfo falcon = Engine.ActorFactory.Get(m_FalconID);
      ActorInfo wedge = Engine.ActorFactory.Get(m_WedgeID);
      ActorInfo homeone = Engine.ActorFactory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.SetArmor(DamageType.ALL, 0);

      if (wedge != null)
        wedge.SetArmor(DamageType.ALL, 0);

      if (homeone != null)
        homeone.SetArmor(DamageType.ALL, 0);

      m_PlayerID = PlayerInfo.ActorID;
      ActorInfo player = PlayerInfo.Actor;
      if (player != null)
      {
        m_Player_pos = player.Position;
        player.QueueFirst(new Lock());
        player.Position = new TV_3DVECTOR(30, 0, -100000);
        m_Player_PrimaryWeaponN = PlayerInfo.PrimaryWeaponN;
        m_Player_SecondaryWeaponN = PlayerInfo.SecondaryWeaponN;
        player.SetArmor(DamageType.ALL, 0);
      }

      PlayerInfo.ActorID = -1;
      PlayerInfo.TempActorID = -1;
      Manager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene()
    {
      ActorInfo falcon = Engine.ActorFactory.Get(m_FalconID);
      ActorInfo wedge = Engine.ActorFactory.Get(m_WedgeID);
      ActorInfo homeone = Engine.ActorFactory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.SetArmor(DamageType.ALL, 1);

      if (wedge != null)
        wedge.SetArmor(DamageType.ALL, 1);

      if (homeone != null)
        homeone.SetArmor(DamageType.ALL, 1);

      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.SetState_Normal();
        player.Position = m_Player_pos;
        player.ForceClearQueue();
        PlayerInfo.ActorID = m_PlayerID;
        PlayerInfo.PrimaryWeaponN = m_Player_PrimaryWeaponN;
        PlayerInfo.SecondaryWeaponN = m_Player_SecondaryWeaponN;
        player.SetArmor(DamageType.ALL, 1);
      }

      Manager.IsCutsceneMode = false;
      PlayerCameraInfo.SetPlayerLook();
    }

    public void Scene_DeathStarCam()
    {
      ActorInfo target = Engine.ActorFactory.Get(m_ADS_targetID);

      if (target != null
        && target.Active)
      {
        Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
        SoundManager.SetSound("ds_beam", false, 1, false);

        TV_3DVECTOR pos = target.GetGlobalPosition();
        TV_3DVECTOR rot = target.GetGlobalRotation();

        if (target.TypeInfo is CorellianATI)
          pos += new TV_3DVECTOR(150, 120, -2000);
        else if (target.TypeInfo is TransportATI)
          pos += new TV_3DVECTOR(300, 200, 700);
        else if (target.TypeInfo is MC90ATI)
          pos += new TV_3DVECTOR(-850, -400, 2500);

        PlayerCameraInfo.SceneLook.SetPosition_Point(pos, 50);
        PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(target.ID);
        PlayerCameraInfo.SetSceneLook();
        Manager.AddEvent(Game.GameTime + 5f, Scene_ExitCutscene);
      }
    }

    public void DeathStarKill_Effect(ActorInfo actor)
    {
      PlayerCameraInfo.Shake(150);
      SoundManager.SetSoundStop("ds_beam");
      SoundManager.SetSound(SoundGlobals.ExpLg, false, 1, false);
    }
    #endregion
  }
}
