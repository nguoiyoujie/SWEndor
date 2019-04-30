using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSEndor : GameScenarioBase
  {
    public GSEndor(GameScenarioManager manager) : base(manager)
    {
      Name = "Battle of Endor";
      Description = "The Rebel fleet, amassed on Sullust, prepares to move to Endor "
                  + "where the Emperor is overseeing the construction of the second "
                  + "Death Star.";
      AllowedWings = new List<ActorTypeInfo> { ActorTypeFactory.Get("X-Wing")
                                               , ActorTypeFactory.Get("Y-Wing")
                                               , ActorTypeFactory.Get("A-Wing")
                                               , ActorTypeFactory.Get("B-Wing")
                                               , ActorTypeFactory.Get("Millennium Falcon")
                                               , ActorTypeFactory.Get("Corellian Corvette")
                                               , ActorTypeFactory.Get("Victory-I Star Destroyer")
                                               , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                               , ActorTypeFactory.Get("TIE Advanced X1")
                                              };

      AllowedDifficulties = new List<string> { "easy"
                                               , "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private ActorInfo m_ADS = null;
    private int m_ADSLaserSourceID = -1;
    private int m_ExecutorStaticID = -1;
    private List<object[]> m_pendingSDspawnlist;
    private Dictionary<int, TV_3DVECTOR> m_rebelPosition;
    private int m_SDLeftForShieldDown = 0;

    private float m_Enemy_pull = 0;
    private float m_Enemy_pullrate = 5;

    private int m_ADS_targetID = -1;
    private int m_FalconID = -1;
    private int m_WedgeID = -1;
    private int m_PlayerID = -1;
    private TV_3DVECTOR m_Player_pos;
    private string m_Player_PrimaryWeapon = "";
    private string m_Player_SecondaryWeapon = "";

    private int m_HomeOneID = -1;

    private float TIESpawnTime = 0;
    private int TIEWaves = 0;
    private int SDWaves = 0;


    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Name = "Red Three";

      AtmosphereInfo.ShowSun = false;
      AtmosphereInfo.ShowFlare = false;
      m_rebelPosition = new Dictionary<int, TV_3DVECTOR>();
      m_pendingSDspawnlist = new List<object[]>();
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

      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      cam.SetLocalPosition(0, 0, 0);
      Manager.CameraTargetPoint = new TV_3DVECTOR(0, 0, -100);
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

      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Manager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Manager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      MainAllyFaction.WingLimit = 75;

      Manager.IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Wedge", new TV_COLOR(0.8f, 0.4f, 0.4f, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Falcon", new TV_COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire_Advanced", new TV_COLOR(0.4f, 0.8f, 0.4f, 1)).AutoAI = true;

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
      ActorCreationInfo aci_Endor = new ActorCreationInfo(ActorTypeFactory.Get("Endor"))
      {
        Position = new TV_3DVECTOR(0, -40000, 0),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        InitialScale = new TV_3DVECTOR(60, 60, 60)
      };
      ActorInfo.Create(ActorFactory, aci_Endor);

      // Create DeathStar
      ActorCreationInfo aci_DS = new ActorCreationInfo(ActorTypeFactory.Get("DeathStar2"))
      {
        Position = new TV_3DVECTOR(0, 5600, 50000),
        Rotation = new TV_3DVECTOR(0, 0, 5),
        InitialScale = new TV_3DVECTOR(1, 1, 1),
        Faction = MainEnemyFaction
      };
      m_ADS = ActorInfo.Create(ActorFactory, aci_DS);
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
        else if (SDWaves >= 1 && MainEnemyFaction.GetShips().Count == 0 && m_pendingSDspawnlist.Count == 0 && StageNumber == 2)
        {
          StageNumber = 3;
          Manager.AddEvent(Game.GameTime + 5.5f, Message_14_Interceptors);
        }
        else if (TIEWaves > 11 && StageNumber == 3)
        {
          StageNumber = 4;
        }
        else if (SDWaves >= 2 && MainEnemyFaction.GetShips().Count == 0 && m_pendingSDspawnlist.Count == 0 && StageNumber == 4)
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
          && (m_pendingSDspawnlist.Count + MainEnemyFaction.GetShips().Count) <= m_SDLeftForShieldDown)
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
          if ((MainEnemyFaction.GetWings().Count < 36 && MainEnemyFaction.GetShips().Count == 0 && StageNumber == 1)
            || (MainEnemyFaction.GetWings().Count < 32 && MainEnemyFaction.GetShips().Count == 0 && StageNumber == 3)
            || (MainEnemyFaction.GetWings().Count < 28 && MainEnemyFaction.GetShips().Count == 0)
            || (MainEnemyFaction.GetWings().Count < 14 && MainEnemyFaction.GetShips().Count > 0))
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
                    Manager.AddEvent(0, Empire_TIEWave_01, 4);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsWedge, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsFalcon, 2);
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
                    Manager.AddEvent(0, Empire_TIEWave_01, 5);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsWedge, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsFalcon, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsPlayer, 1);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_01, 6);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
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
                    Manager.AddEvent(0, Empire_TIEWave_02, 4);
                    Manager.AddEvent(0, Empire_TIEWave_InterceptorsvsWedge, 2);
                    Manager.AddEvent(0, Empire_TIEWave_InterceptorsvsFalcon, 2);
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
                    Manager.AddEvent(0, Empire_TIEWave_02, 5);
                    Manager.AddEvent(0, Empire_TIEWave_InterceptorsvsWedge, 2);
                    Manager.AddEvent(0, Empire_TIEWave_InterceptorsvsFalcon, 2);
                    Manager.AddEvent(0, Empire_TIEWave_InterceptorsvsPlayer, 1);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
                  }
                  else
                  {
                    Manager.AddEvent(0, Empire_TIEWave_02, 6);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
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
                    Manager.AddEvent(0, Empire_TIEWave_02, 2);
                    Manager.AddEvent(0, Empire_TIEWave_03, 2);
                    Manager.AddEvent(0, Empire_TIEBombers, 2);
                    Manager.AddEvent(0, Empire_TIEWave_TIEsvsPlayer, 2);
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
                    Manager.AddEvent(0, Empire_TIEWave_03, 5);
                    Manager.AddEvent(0, Empire_TIEBombers, 2);
                    Manager.AddEvent(0, Empire_TIEWave_InterceptorsvsPlayer, 2);
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

        if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.GetShips().Count < 8)
        {
          if (m_pendingSDspawnlist[0].Length > 0
            && (!(m_pendingSDspawnlist[0][0] is ImperialIATI) || MainEnemyFaction.GetShips().Count < ((StageNumber == 6) ? 4 : 2))
            && (!(m_pendingSDspawnlist[0][0] is DevastatorATI) || MainEnemyFaction.GetShips().Count < 2))
          {
            Manager.AddEvent(0, Empire_StarDestroyer_Spawn, m_pendingSDspawnlist[0]);
            m_pendingSDspawnlist.RemoveAt(0);
          }
        }

        if (m_Enemy_pull > 0)
        {
          m_Enemy_pull -= Game.TimeSinceRender * m_Enemy_pullrate;
          foreach (int enemyshipID in MainEnemyFaction.GetShips())
          {
            ActorInfo enemyship = ActorFactory.Get(enemyshipID);
            if (enemyship != null)
              enemyship.MoveAbsolute(0, 0, Game.TimeSinceRender * m_Enemy_pullrate);
          }
        }

        //Rebel_ForceAwayFromBounds(null);

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
        Manager.Line1Text = string.Format("WINGS: {0}", MainAllyFaction.WingLimit);
      }
      else
      {
        Manager.Line1Text = string.Format("");
      }

      if (Manager.Scenario.TimeSinceLostShip < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
      {
        Manager.Line2Text = string.Format("SHIPS: {0}", MainAllyFaction.ShipLimit);
      }
      else
      {
        Manager.Line2Text = string.Format("");
      }
    }

    #region Rebellion spawns

    private void Rebel_HyperspaceIn(params object[] param)
    {
      ActorInfo ainfo;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      ActionInfo[] actions;
      string[] registries;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.GameTime;
      float creationDelay = 0.025f;

      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      cam.SetLocalPosition(10, -20, 1500);

      ActorTypeInfo type;
      string name;
      string sidebar_name;
      FactionInfo faction;

      // Millennium Falcon
      type = ActorTypeFactory.Get("Millennium Falcon (Lando)");
      name = "Millennium Falcon (Lando)";
      sidebar_name = "FALCON";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Falcon");
      position = new TV_3DVECTOR(0, -10, 350);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x +Engine.Random.Next(-5, 5), position.y +Engine.Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

      registries = new string[] { "CriticalAllies" };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        RegisterName = "",
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
      type = ActorTypeFactory.Get("X-Wing (Wedge)");
      name = "X-Wing (Wedge)";
      sidebar_name = "WEDGE";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Wedge");
      position = new TV_3DVECTOR(70, 20, 250);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x +Engine.Random.Next(-5, 5), position.y +Engine.Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

      registries = new string[] { "CriticalAllies" };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        RegisterName = "",
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
      if (PlayerInfo.ActorType == ActorTypeFactory.Get("X-Wing"))
      {
        position = new TV_3DVECTOR(0, 0, -25);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("Y-Wing"))
      {
        position = new TV_3DVECTOR(-250, 60, -520);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("A-Wing"))
      {
        position = new TV_3DVECTOR(100, 70, -720);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("B-Wing"))
      {
        position = new TV_3DVECTOR(-80, 20, -50);
      }
      else if (PlayerInfo.ActorType == ActorTypeFactory.Get("Corellian Corvette"))
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
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        RegisterName = "",
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
      Manager.CameraTargetActorID = ainfo.ID;

      // Mon Calamari (HomeOne)
      type = ActorTypeFactory.Get("Mon Calamari Capital Ship");
      name = "Mon Calamari (Home One)";
      sidebar_name = "HOME ONE";
      faction = MainAllyFaction;
      position = new TV_3DVECTOR(1000, -300, 1000);
      TV_3DVECTOR nv = new TV_3DVECTOR(position.x + ((position.x > 0) ? 5 : -5), position.y, -position.z - 3000);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(nv
                                                  , type.MaxSpeed
                                                  , type.Move_CloseEnough
                                                  , false)
                                 , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                    , type.MinSpeed
                                                    , type.Move_CloseEnough
                                                    , false)
                                 , new Lock()
                                 };

      registries = new string[] { "CriticalAllies" };

      ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = name,
        RegisterName = "",
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
      spawns.Add(new object[] { new TV_3DVECTOR(60, 30, -390), ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, -50, -320), ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(110, 20, -340), ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 50, -360), ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-120, 20, -380), ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(70, -20, -400), ActorTypeFactory.Get("X-Wing") });

      // A-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(200, -10, -750), ActorTypeFactory.Get("A-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(80, 20, -800), ActorTypeFactory.Get("A-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(270, 40, -850), ActorTypeFactory.Get("A-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(65, 65, -900), ActorTypeFactory.Get("A-Wing") });

      // B-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(-150, 50, -250), ActorTypeFactory.Get("B-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-290, 80, -280), ActorTypeFactory.Get("B-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, 100, -350), ActorTypeFactory.Get("B-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-200, 65, -400), ActorTypeFactory.Get("B-Wing") });

      // Y-Wings x6
      spawns.Add(new object[] { new TV_3DVECTOR(-10, 100, -350), ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(50, 80, -380), ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 90, -420), ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-90, 100, -440), ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(75, 110, -450), ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 85, -500), ActorTypeFactory.Get("Y-Wing") });

      // Corellian x9 (5 forward, 4 rear)
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, -120, 1300), ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1400, -320, 400), ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2400, 150, 1500), ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 470, 850), ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 300, 200), ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(600, 70, -150), ActorTypeFactory.Get("Corellian Corvette"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-700, -150, -600), ActorTypeFactory.Get("Corellian Corvette"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-600, 250, -1200), ActorTypeFactory.Get("Corellian Corvette"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, 200, -1200), ActorTypeFactory.Get("Corellian Corvette"), 5500 });

      // Mon Calamari x2 (not including HomeOne)
      spawns.Add(new object[] { new TV_3DVECTOR(4500, -120, -500), ActorTypeFactory.Get("Mon Calamari Capital Ship"), 7200 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 550, -1500), ActorTypeFactory.Get("Mon Calamari Capital Ship"), 7200 });

      // Nebulon B x3
      spawns.Add(new object[] { new TV_3DVECTOR(-2700, 320, -1850), ActorTypeFactory.Get("Nebulon-B Frigate"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1800, -280, -900), ActorTypeFactory.Get("Nebulon-B Frigate"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 850, -150), ActorTypeFactory.Get("Nebulon-B Frigate"), 8000 });

      // Transport x9
      spawns.Add(new object[] { new TV_3DVECTOR(1200, 550, -750), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1750, 350, 150), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, 280, -1200), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(4000, 280, -800), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-500, -175, -1600), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1650, -230, -800), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 450, 0), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, -280, -1200), ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-900, 10, -400), ActorTypeFactory.Get("Transport"), 8000 });

      foreach (object[] spawn in spawns)
      {
        type = (ActorTypeInfo)spawn[1];
        int huntw = 5;
        creationTime += creationDelay;
        position = (TV_3DVECTOR)spawn[0];
        if (type is ActorTypes.Groups.Fighter)
        {
          actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x +Engine.Random.Next(-5, 5), position.y +Engine.Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
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
                                                  , type.MaxSpeed
                                                  , type.Move_CloseEnough
                                                  , false)
                                 , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                    , type.MinSpeed
                                                    , type.Move_CloseEnough
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
          RegisterName = "",
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

      MainAllyFaction.ShipLimit = MainAllyFaction.GetShips().Count;
    }

    private void Rebel_SetPositions(params object[] param)
    {
      foreach (int id in m_rebelPosition.Keys)
      {
        ActorInfo actor = ActorFactory.Get(id);
        if (actor != null)
        {
          actor.Position = m_rebelPosition[id] + new TV_3DVECTOR(0, 0, actor.TypeInfo.MaxSpeed * 8f);
          actor.MoveComponent.Speed = actor.TypeInfo.MaxSpeed;
        }
      }
    }

    private void Rebel_HyperspaceOut(params object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                        , actor.MoveComponent.MaxSpeed
                                                        , actor.TypeInfo.Move_CloseEnough));
          ActionManager.QueueLast(actorID, new HyperspaceOut());
          ActionManager.QueueLast(actorID, new Delete());
        }
      }
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                , actor.MoveComponent.MaxSpeed
                                                , actor.TypeInfo.Move_CloseEnough));
          ActionManager.QueueLast(actorID, new HyperspaceOut());
          ActionManager.QueueLast(actorID, new Delete());
        }
      }
    }

    public void Rebel_MakePlayer(params object[] param)
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;

      if (PlayerInfo.Actor != null
        && PlayerInfo.Actor.CreationState != CreationState.DISPOSED)
      {
        // m_Player = Player.Actor;
        if (!Manager.GetGameStateB("in_battle"))
        {
          foreach (int actorID in MainAllyFaction.GetShips())
          {
            ActorInfo actor = ActorFactory.Get(actorID);
            if (actor != null)
            {
              actor.ActorState = ActorState.FREE;
              actor.MoveComponent.Speed = 275;
            }
          }

          foreach (int actorID in MainAllyFaction.GetWings())
          {
            ActorInfo actor = ActorFactory.Get(actorID);
            if (actor != null)
            {
              actor.ActorState = ActorState.FREE;
              if (actor.MoveComponent.Speed < 425)
                actor.MoveComponent.Speed = 425;
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

    public void Rebel_ShipsForward(params object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() - new TV_3DVECTOR(actor.GetPosition().x * 0.35f, 0, Math.Abs(actor.GetPosition().x) + 1500)
                                                    , actor.MoveComponent.MinSpeed));
          ActionManager.QueueLast(actorID, new Lock());
        }
      }
    }

    public void Rebel_ShipsForward_2(params object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Move(actor.GetPosition() - new TV_3DVECTOR(actor.GetPosition().x * 0.35f, 0, Math.Abs(actor.GetPosition().x) + 1500)
                                                    , actor.MoveComponent.MaxSpeed));
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() - new TV_3DVECTOR(0, 0, 20000)
                                                    , actor.MoveComponent.MinSpeed));
          ActionManager.QueueLast(actorID, new Lock());
        }
      }
    }

    public void Rebel_YWingsAttackScan(params object[] param)
    {
      if (MainEnemyFaction.GetShips().Count > 0)
      {
        foreach (int actorID in MainAllyFaction.GetWings())
        {
          ActorInfo actor = ActorFactory.Get(actorID);
          if (actor != null)
          {
            if (actor.TypeInfo is YWingATI || actor.TypeInfo is BWingATI)
            {
              int rsID = MainEnemyFaction.GetShips()[Engine.Random.Next(0, MainEnemyFaction.GetShips().Count)];
              ActorInfo rs = ActorFactory.Get(actorID);
              {
                foreach (int i in rs.GetAllChildren(1))
                {
                  ActorInfo rc = ActorFactory.Get(i);
                  if (rc.RegenerationInfo.ParentRegenRate > 0)
                    if (Engine.Random.NextDouble() > 0.4f)
                      rsID = rc.ID;
                }
              }

              ActionManager.ClearQueue(actorID);
              ActionManager.QueueLast(actorID, new AttackActor(rsID, -1, -1, false));
            }
          }
        }
      }

      Manager.AddEvent(Game.GameTime + 5f, Rebel_YWingsAttackScan);
    }

    public void Rebel_DeathStarGo(params object[] param)
    {
      ActorInfo falcon = ActorFactory.Get(m_FalconID);
      if (falcon != null)
      {
        falcon.CombatInfo.DamageModifier = 0;
        ActionManager.ForceClearQueue(m_FalconID);
        ActionManager.QueueLast(m_FalconID, new ForcedMove(new TV_3DVECTOR(0, 0, 20000), falcon.MoveComponent.MaxSpeed, -1));
        ActionManager.QueueLast(m_FalconID, new HyperspaceOut());
        ActionManager.QueueLast(m_FalconID, new Delete());
        m_FalconID = -1;
      }

      ActorInfo wedge = ActorFactory.Get(m_WedgeID);
      if (wedge != null)
      {
        wedge.CombatInfo.DamageModifier = 0;
        ActionManager.ForceClearQueue(m_WedgeID);
        ActionManager.QueueLast(m_WedgeID, new ForcedMove(new TV_3DVECTOR(0, 0, 20000), wedge.MoveComponent.MaxSpeed, -1));
        ActionManager.QueueLast(m_WedgeID, new HyperspaceOut());
        ActionManager.QueueLast(m_WedgeID, new Delete());
        m_WedgeID = -1;
      }
    }

    public void Rebel_GiveControl(params object[] param)
    {
      ActorInfo falcon = ActorFactory.Get(m_FalconID);
      ActorInfo wedge = ActorFactory.Get(m_WedgeID);

      ActionManager.UnlockOne(m_FalconID);
      falcon.ActorState = ActorState.NORMAL;
      falcon.MoveComponent.Speed = falcon.MoveComponent.MaxSpeed;

      ActionManager.UnlockOne(m_WedgeID);
      ActionManager.QueueFirst(m_WedgeID, new Wait(2.5f));
      wedge.ActorState = ActorState.NORMAL;
      wedge.MoveComponent.Speed = wedge.MoveComponent.MaxSpeed;

      float time = 5f;
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);

        ActionManager.UnlockOne(actorID);
        ActionManager.QueueFirst(actorID, new Wait(time));
        actor.ActorState = ActorState.NORMAL;
        actor.MoveComponent.Speed = actor.MoveComponent.MaxSpeed;
        time += 2.5f;
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);

        ActionManager.UnlockOne(actorID);
        actor.ActorState = ActorState.NORMAL;
        actor.MoveComponent.Speed = actor.MoveComponent.MaxSpeed;
        actor.SetSpawnerEnable(true);
      }
      PlayerInfo.IsMovementControlsEnabled = true;

      Manager.SetGameStateB("in_battle", true);
      Manager.SetGameStateB("TIEs", true);
      Manager.AddEvent(Game.GameTime + 10f, Empire_FirstTIEWave);
      Manager.AddEvent(Game.GameTime + 45f, Empire_SecondTIEWave);
      Rebel_RemoveTorps(null);
    }

    private void Rebel_GoBack(float chance)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          double d = Engine.Random.NextDouble();
          if (d < chance)
          {
            ActorInfo homeone = ActorFactory.Get(m_HomeOneID);

            ActionManager.ClearQueue(actorID);
            ActionManager.QueueLast(actorID, new Move(homeone.GetPosition() + new TV_3DVECTOR(Engine.Random.Next(-2500, 2500), Engine.Random.Next(-50, 50), Engine.Random.Next(-2500, 2500))
                                                       , actor.MoveComponent.MaxSpeed));
          }
        }
      }
    }

    public void Rebel_RemoveTorps(params object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (!ActorInfo.IsPlayer(Engine, actorID))
          {
            foreach (KeyValuePair<string, WeaponInfo> kvp in actor.WeaponSystemInfo.Weapons)
            {
              if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
              {
                kvp.Value.Ammo = 2;
                kvp.Value.MaxAmmo = 2;
              }
            }
          }
        }
      }
    }

    public void Rebel_RemoveTorpsSingle(params object[] param)
    {
      if (param.Length == 0 || !(param[0] is ActorInfo))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];

      if (!ActorInfo.IsPlayer(Engine, ainfo.ID))
      {
        foreach (KeyValuePair<string, WeaponInfo> kvp in ainfo.WeaponSystemInfo.Weapons)
        {
          if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
          {
            kvp.Value.Ammo = 2;
            kvp.Value.MaxAmmo = 2;
          }
        }
      }
    }

    public void Rebel_CriticalUnitHit(params object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      int actorID = (int)param[0];
      ActorInfo av = ActorFactory.Get(actorID);

      if (av != null
        && av.CombatInfo.Strength < av.TypeInfo.MaxStrength * 0.8f
        && MainAllyFaction.GetShips().Count > 0)
      {
        ActorInfo homeone = ActorFactory.Get(m_HomeOneID);

        ActionManager.ClearQueue(actorID);
        ActionManager.QueueLast(actorID
                              , new Move(homeone.GetPosition() + new TV_3DVECTOR(Engine.Random.Next(-2500, 2500)
                              , Engine.Random.Next(-50, 50)
                              , Engine.Random.Next(-2500, 2500))
                              , av.MoveComponent.MaxSpeed));
      }
    }

    public void Rebel_CriticalUnitDanger(params object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      int actorID = (int)param[0];
      ActorInfo av = ActorFactory.Get(actorID);

      if (av != null
        && av.StrengthFrac < 0.67f
        && av.StrengthFrac >= 0.33f)
      {
        Screen2D.MessageText(string.Format("{0}: {1}, I need cover!", av.Name, PlayerInfo.Name)
                                            , 5
                                            , av.Faction.Color
                                            , 100);
      }
      else if (av.StrengthFrac < 0.33f)
      {
        Screen2D.MessageText(string.Format("{0}: {1}, Get those TIEs off me!", av.Name, PlayerInfo.Name)
                                            , 5
                                            , av.Faction.Color
                                            , 100);
      }
    }

    public void Rebel_CriticalUnitDying(params object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (Manager.GetGameStateB("GameWon"))
        return;

      if (Manager.GetGameStateB("GameOver"))
        return;

      int actorID = (int)param[0];
      ActorInfo actor = ActorFactory.Get(actorID);

      if (actor != null
        && actor.ActorState.IsDyingOrDead())
      {
        PlayerInfo.TempActorID = actorID;

        Manager.SetGameStateB("GameOver", true);
        Manager.IsCutsceneMode = true;

        ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
        if (cam == null || !(cam.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(ActorTypeFactory.Get("Death Camera"));
          camaci.CreationTime = Game.GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = actor.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(ActorFactory, camaci);
          PlayerInfo.ActorID = a.ID;
          PlayerInfo.TempActorID = actorID;

          a.CameraSystemInfo.CamDeathCirclePeriod = actor.CameraSystemInfo.CamDeathCirclePeriod;
          a.CameraSystemInfo.CamDeathCircleRadius = actor.CameraSystemInfo.CamDeathCircleRadius;
          a.CameraSystemInfo.CamDeathCircleHeight = actor.CameraSystemInfo.CamDeathCircleHeight;

          if (actor.ActorState.IsDying())
          {
            actor.TickEvents += Manager.Scenario.ProcessPlayerDying;
            actor.DestroyedEvents += Manager.Scenario.ProcessPlayerKilled;
          }
          else
          {
            actor.DestroyedEvents += Manager.Scenario.ProcessPlayerKilled;
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
            actor.CombatInfo.TimedLife = 2000f;
            Manager.AddEvent(Game.GameTime + 25, FadeOut);
          }
        }
        else
        {
          cam.SetLocalPosition(actor.GetPosition().x, actor.GetPosition().y, actor.GetPosition().z);
        }
      }
    }

    #endregion


    #region Empire spawns
    public void Empire_FirstTIEWave(params object[] param)
    {
      List<int> list = m_ADS.GetAllChildren(1);
      if (list.Count > 0)
        m_ADSLaserSourceID = list[0];

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
                actions = new ActionInfo[] { new Hunt(TargetType.FIGHTER) };
                break;
            }

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
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

    public void Empire_SecondTIEWave(params object[] param)
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
                actions = new ActionInfo[] { new Hunt(TargetType.FIGHTER) };
                break;
            }

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
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

    public void Empire_TIEWave_01(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIEs
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = new ActionInfo[] { new Wait(18) };
            switch (Difficulty.ToLower())
            {
              case "mental":
                actions = new ActionInfo[] { new Wait(18), new Hunt(TargetType.FIGHTER) };
                break;
            }

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 8000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 0.25f; // 1.5f
      }
    }

    public void Empire_TIEWave_02(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIEs
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { ActorTypeFactory.Get("TIE"), ActorTypeFactory.Get("TIE Interceptor") };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        int n = Engine.Random.Next(0, tietypes.Length);
        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = new ActionInfo[] { new Wait(18) };
            switch (Difficulty.ToLower())
            {
              case "mental":
                actions = new ActionInfo[] { new Wait(18), new Hunt(TargetType.FIGHTER) };
                break;
            }

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = tietypes[n],
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 8000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_03(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = new ActionInfo[] { new Wait(18) };
            switch (Difficulty.ToLower())
            {
              case "mental":
                actions = new ActionInfo[] { new Wait(18), new Hunt(TargetType.FIGHTER) };
                break;
            }

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE Interceptor"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 5000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_0D(params object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE Defenders!
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        ActionInfo[] actions = new ActionInfo[] { new Wait(8) };
        switch (Difficulty.ToLower())
        {
          case "mental":
            actions = new ActionInfo[] { new Wait(8), new Hunt(TargetType.FIGHTER) };
            break;
        }

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE Defender"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MinBounds.z - 6000),
          Rotation = new TV_3DVECTOR(),
          Actions = actions,
        };

        asi.Spawn(this);
      }
    }

    public void Empire_TIEWave_TIEsvsWedge(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_WedgeID, -1, -1, false) },
        };

        asi.Spawn(this);
      }
    }

    public void Empire_TIEWave_InterceptorsvsWedge(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE Interceptor"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_WedgeID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsFalcon(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_FalconID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_InterceptorsvsFalcon(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE Interceptor"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_FalconID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsPlayer(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(PlayerInfo.Actor.ID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsShips(params object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 2500),
              Rotation = new TV_3DVECTOR(),
              Actions = new ActionInfo[] { new Wait(18)
                                     ,  new Hunt(TargetType.SHIP) },
            };

            asi.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_InterceptorsvsPlayer(params object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(PlayerInfo.Actor.ID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEBombers(params object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE Bombers only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE Bomber"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 2500),
              Rotation = new TV_3DVECTOR(),
              Actions = new ActionInfo[] { new Wait(18)
                                     ,  new Hunt(TargetType.SHIP) },
            };

            asi.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_SpawnStatics(params object[] param)
    {
      ActorInfo ainfo;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      float creationTime = Game.GameTime;

      ActorTypeInfo type;
      string name;

      List<object[]> spawns = new List<object[]>();
      spawns.Add(new object[] { new TV_3DVECTOR(0, 50, -33500), ActorTypeFactory.Get("Executor Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-1500, 180, -32500), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(1500, 180, -32500), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, 280, -27500), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(5500, 280, -27500), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-5500, -30, -29500), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, -30, -29500), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-7500, 130, -31000), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(7500, 130, -31000), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-11500, -80, -29000), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(11500, -80, -29000), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-14500, 80, -27500), ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(14500, 80, -27500), ActorTypeFactory.Get("Imperial-I Static") });

      foreach (object[] spawn in spawns)
      {
        type = (ActorTypeInfo)spawn[1];
        name = type.Name;
        position = (TV_3DVECTOR)spawn[0];

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = type,
          Name = name,
          RegisterName = "",
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

    public void Empire_StarDestroyer_Spawn(params object[] param)
    {
      if (param == null || param.GetLength(0) < 3 || !(param[0] is ActorTypeInfo) || !(param[1] is TV_3DVECTOR || !(param[2] is TV_3DVECTOR)))
        return;

      ActorTypeInfo type = (ActorTypeInfo)param[0];
      TV_3DVECTOR position = (TV_3DVECTOR)param[1];
      TV_3DVECTOR targetposition = (TV_3DVECTOR)param[2];
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -25000);

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = type,
        Name = "",
        RegisterName = "",
        SidebarName = type.Name.Substring(0, type.Name.IndexOf(' ')).ToUpper(),
        SpawnTime = Game.GameTime,
        Faction = MainEnemyFaction,
        Position = position + hyperspaceInOffset,
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[]
                                 {
                                   new HyperspaceIn(position)
                                   , new Move(targetposition, type.MaxSpeed)
                                   , new Rotate(targetposition + new TV_3DVECTOR(0, 0, 25000), type.MinSpeed)
                                   , new Lock()
                                 },
        Registries = new string[] { "CriticalEnemies" }
      };

      ActorInfo ainfo = ainfo = asi.Spawn(this);

      ainfo.SetSpawnerEnable(true);
      ainfo.HuntWeight = 3;

      if (type is DevastatorATI)
        Manager.AddEvent(0, Empire_TIEWave_0D, 2);
    }

    public void Empire_StarDestroyer_01(params object[] param)
    {
      SDWaves++;

      m_Enemy_pull = 8000;
      switch (Difficulty.ToLower())
      {
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-2500, -250, -19500), new TV_3DVECTOR(-2500, -250, -9500) });
          break;
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-750, -50, -19000), new TV_3DVECTOR(-750, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(750, -50, -19000), new TV_3DVECTOR(750, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-2250, -50, -19000), new TV_3DVECTOR(-1250, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(2250, -50, -19000), new TV_3DVECTOR(1250, -50, -6500) });

          break;
        case "hard":
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(2500, -250, -19500), new TV_3DVECTOR(2000, 110, -9500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-2500, -250, -19500), new TV_3DVECTOR(-2000, 110, -9500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });
          break;
      }
      SoundManager.SetMusic("battle_3_2");
    }

    public void Empire_StarDestroyer_02(params object[] param)
    {
      SDWaves++;

      m_Enemy_pull = 12000;
      switch (Difficulty.ToLower())
      {
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-7500, 60, -24500), new TV_3DVECTOR(-2000, 60, -12500) });
          break;
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-10000, -300, -23500), new TV_3DVECTOR(-1250, -300, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Devastator Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 120, -24500), new TV_3DVECTOR(0, 120, -11500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-2500, 150, -24500), new TV_3DVECTOR(-2000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-3200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(10000, -300, -23500), new TV_3DVECTOR(1250, -300, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(2500, 150, -24500), new TV_3DVECTOR(2000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(3200, 50, -12000) });

          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, -100, -24500), new TV_3DVECTOR(2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(2500, 150, -26500), new TV_3DVECTOR(500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(7500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2500, -100, -24500), new TV_3DVECTOR(2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(500, 150, -26500), new TV_3DVECTOR(500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          break;
      }

      SoundManager.SetMusic("battle_3_2");
    }

    public void Empire_Executor(params object[] param)
    {
      //m_Enemy_pull = 4000;
      SDWaves++;
      ActorInfo.Kill(Engine, m_ExecutorStaticID);

      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.GameTime;

      TV_3DVECTOR position = new TV_3DVECTOR(0, -950, -20000);
      ActorTypeInfo atinfo = ActorTypeFactory.Get("Executor Super Star Destroyer");

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = atinfo,
        Name = "",
        RegisterName = "",
        SidebarName = "EXECUTOR",
        SpawnTime = Game.GameTime,
        Faction = MainEnemyFaction,
        Position = position + hyperspaceInOffset,
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[]
                           {
                             new HyperspaceIn(position)
                             , new Move(new TV_3DVECTOR(position.x, position.y, -4000), atinfo.MaxSpeed)
                             , new Rotate(position + new TV_3DVECTOR(0, 0, 22500), atinfo.MinSpeed)
                             , new Lock()
                           },
        Registries = new string[] { "CriticalEnemies" }
      };

      ActorInfo ainfo = ainfo = asi.Spawn(this);
      ainfo.HuntWeight = 5;
      ainfo.ActorStateChangeEvents += Empire_ExecutorDestroyed;

      // SD

      switch (Difficulty.ToLower())
      {
        case "easy":
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 10);
          break;
        case "mental":
          Manager.AddEvent(0, Empire_TIEWave_0D, 2);

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-550, 80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(550, -160, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-1500, 90, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(1500, 90, -6500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -80, -24000), new TV_3DVECTOR(-2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2000, -80, -24000), new TV_3DVECTOR(2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });

          break;
        case "hard":
          Manager.AddEvent(0, Empire_TIEWave_0D, 1);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 5);

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-2500, 120, -6500) });

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500) });

          break;
        case "normal":
        default:
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 10);

          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500) });
          break;
      }
    }

    public void Empire_ExecutorDestroyed(params object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (Manager.GetGameStateB("GameOver"))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];
      ActorState state = (ActorState)param[1];

      if (ainfo.ActorState.IsDyingOrDead())
      {
        Manager.SetGameStateB("GameWon", true);
        Manager.IsCutsceneMode = true;

        ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
        if (cam == null || !(cam.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(ActorTypeFactory.Get("Death Camera"));
          camaci.CreationTime = Game.GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = ainfo.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(ActorFactory, camaci);
          PlayerInfo.ActorID = a.ID;

          a.CameraSystemInfo.CamDeathCirclePeriod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
          a.CameraSystemInfo.CamDeathCircleRadius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
          a.CameraSystemInfo.CamDeathCircleHeight = ainfo.CameraSystemInfo.CamDeathCircleHeight;

          if (ainfo.ActorState.IsDying())
          {
            ainfo.TickEvents += ProcessPlayerDying;
            ainfo.DestroyedEvents += ProcessPlayerKilled;
          }
          else
          {
            ainfo.DestroyedEvents += ProcessPlayerKilled;
          }

          SoundManager.SetMusic("executorend");
          ainfo.CombatInfo.TimedLife = 2000f;

          ActorInfo homeone = ActorFactory.Get(m_HomeOneID);
          if (homeone != null)
            homeone.CombatInfo.DamageModifier = 0;
          Manager.AddEvent(Game.GameTime + 55, FadeOut);
        }
        else
        {
          cam.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
        }
      }
    }

    public void Empire_DeathStarAttack(params object[] param)
    {
      ActorInfo lsrSrc = ActorFactory.Get(m_ADSLaserSourceID);
      if (lsrSrc != null
        && lsrSrc.CreationState == CreationState.ACTIVE)
      {
        List<int> victims = new List<int>(MainAllyFaction.GetShips());
        victims.Remove(m_HomeOneID);

        if (victims.Count > 0)
        {
          m_ADS_targetID = victims[Engine.Random.Next(0, victims.Count)];

          ActionManager.ForceClearQueue(m_ADSLaserSourceID);
          ActionManager.QueueNext(m_ADSLaserSourceID, new AttackActor(m_ADS_targetID));
          ActionManager.QueueNext(m_ADSLaserSourceID, new Lock());

          ActorInfo target = ActorFactory.Get(m_ADS_targetID);
          target.DestroyedEvents += DeathStarKill_Effect;
          Manager.AddEvent(0.1f, Scene_DeathStarCam);
        }
      }
    }

    public void Empire_DeathStarAttack_01(params object[] param)
    {
      Empire_DeathStarAttack(ActorTypeFactory.Get("Corellian Corvette"));
    }

    public void Empire_DeathStarAttack_02(params object[] param)
    {
      Empire_DeathStarAttack(ActorTypeFactory.Get("Transport"));
    }

    public void Empire_DeathStarAttack_03(params object[] param)
    {
      Empire_DeathStarAttack(ActorTypeFactory.Get("Mon Calamari Capital Ship"));
    }

    private void Empire_DeathStarAttack(ActorTypeInfo type)
    {
      ActorInfo lsrSrc = ActorFactory.Get(m_ADSLaserSourceID);
      if (lsrSrc != null
        && lsrSrc.CreationState == CreationState.ACTIVE)
      {
        List<int> tgt = new List<int>(MainAllyFaction.GetShips());
        tgt.Reverse();

        foreach (int tid in tgt)
        {
          ActorInfo t = ActorFactory.Get(tid);
          if (t != null
            && t.TypeInfo == type
            && tid != m_HomeOneID)
          {
            m_ADS_targetID = tid;

            ActionManager.ForceClearQueue(m_ADSLaserSourceID);
            ActionManager.QueueNext(m_ADSLaserSourceID, new AttackActor(tid));
            ActionManager.QueueNext(m_ADSLaserSourceID, new Lock());

            t.DestroyedEvents += DeathStarKill_Effect;
            Manager.AddEvent(0.1f, Scene_DeathStarCam);
            return;
          }
        }
      }
    }

    #endregion

    #region Text
    public void Message_01_AllWingsReport(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: All wings report in.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_02_RedLeader(params object[] param)
    {
      Screen2D.MessageText("X-WING (WEDGE): Red Leader standing by.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_03_GoldLeader(params object[] param)
    {
      Screen2D.MessageText("Y-WING: Gray Leader standing by.", 5, new TV_COLOR(0.6f, 0.6f, 0.6f, 1));
    }

    public void Message_04_BlueLeader(params object[] param)
    {
      Screen2D.MessageText("B-WING: Blue Leader standing by.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_05_GreenLeader(params object[] param)
    {
      Screen2D.MessageText("A-WING: Green Leader standing by.", 5, new TV_COLOR(0.4f, 0.8f, 0.4f, 1));
    }

    public void Message_06_Force(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): May the Force be with us.", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_07_Break(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Break off the attack!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_08_Break(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: The shield's still up!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_09_Conf(params object[] param)
    {
      Screen2D.MessageText("X-WING (WEDGE): I get no reading, are you sure?", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_10_Break(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: All wings, pull up!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_11_Evasive(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): Take evasive action!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_12_Trap(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): It's a trap!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_13_Fighters(params object[] param)
    {
      Screen2D.MessageText("X-WING (WEDGE): Watch out for enemy fighters.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_14_Interceptors(params object[] param)
    {
      Screen2D.MessageText("X-WING (WEDGE): TIE Interceptors inbound.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_15_Bombers(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): We have bombers inbound. Keep them away from our cruisers!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_20_DeathStar(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: That blast came from the Death Star...", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_21_Close(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Get us close to those Imperial Star Destroyers.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_22_PointBlank(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Closer! Get us closer, and engage them at point blank range.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_23_Take(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: If they fire, we might even take a few of them with us.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_30_ShieldDown(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): The shield is down.", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_31_ResumeAttack(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): Commence your attack on the Death Star's main reactor", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_32_Han(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: I knew Han can do it!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_33_Han(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: Wedge, follow me.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_34_Han(params object[] param)
    {
      Screen2D.MessageText("X-WING (WEDGE): Copy, Gold Leader.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_40_Focus(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): Focus all firepower on that Super Star Destroyer!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_90_LostWedge(params object[] param)
    {
      Screen2D.MessageText("X-WING (WEDGE): I can't hold them!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_91_LostFalcon(params object[] param)
    {
      Screen2D.MessageText("MILLIENNIUM FALCON: I can't hold them!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_92_LostHomeOne(params object[] param)
    {
      Screen2D.MessageText("MON CALAMARI (HOME ONE): We have no chance...", 15, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    #endregion

    #region Scene
    public void Scene_EnterCutscene(params object[] param)
    {
      ActorInfo falcon = ActorFactory.Get(m_FalconID);
      ActorInfo wedge = ActorFactory.Get(m_WedgeID);
      ActorInfo homeone = ActorFactory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.CombatInfo.DamageModifier = 0;

      if (wedge != null)
        wedge.CombatInfo.DamageModifier = 0;

      if (homeone != null)
        homeone.CombatInfo.DamageModifier = 0;

      m_PlayerID = PlayerInfo.ActorID;
      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_pos = player.Position;
        ActionManager.QueueFirst(m_PlayerID, new Lock());
        player.Position = new TV_3DVECTOR(30, 0, -100000);
        m_Player_PrimaryWeapon = PlayerInfo.PrimaryWeapon;
        m_Player_SecondaryWeapon = PlayerInfo.SecondaryWeapon;
        player.CombatInfo.DamageModifier = 0;
      }

      PlayerInfo.ActorID = Manager.SceneCameraID;
      Manager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(params object[] param)
    {
      ActorInfo falcon = ActorFactory.Get(m_FalconID);
      ActorInfo wedge = ActorFactory.Get(m_WedgeID);
      ActorInfo homeone = ActorFactory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.CombatInfo.DamageModifier = 1;

      if (wedge != null)
        wedge.CombatInfo.DamageModifier = 1;

      if (homeone != null)
        homeone.CombatInfo.DamageModifier = 1;

      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.ActorState = ActorState.NORMAL;
        player.Position = m_Player_pos;
        ActionManager.ForceClearQueue(m_PlayerID);
        PlayerInfo.ActorID = m_PlayerID;
        PlayerInfo.PrimaryWeapon = m_Player_PrimaryWeapon;
        PlayerInfo.SecondaryWeapon = m_Player_SecondaryWeapon;
        player.CombatInfo.DamageModifier = 1;
      }

      Manager.IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam(params object[] param)
    {
      ActorInfo target = ActorFactory.Get(m_ADS_targetID);

      if (target != null
        && target.CreationState == CreationState.ACTIVE)
      {
        Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
        SoundManager.SetSound("ds_beam", false, 1, false);

        TV_3DVECTOR pos = target.GetPosition();
        TV_3DVECTOR rot = target.GetRotation();

        if (target.TypeInfo is CorellianATI)
          pos += new TV_3DVECTOR(150, 120, -2000);
        else if (target.TypeInfo is TransportATI)
          pos += new TV_3DVECTOR(300, 200, 700);
        else if (target.TypeInfo is MC90ATI)
          pos += new TV_3DVECTOR(-850, -400, 2500);

        ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
        cam.SetLocalPosition(pos.x, pos.y, pos.z);
        cam.LookAtPoint(new TV_3DVECTOR());
        cam.MoveComponent.MaxSpeed = 50;
        cam.MoveComponent.Speed = 50;
        Manager.CameraTargetActorID = target.ID;
        Manager.AddEvent(Game.GameTime + 5f, Scene_ExitCutscene);
      }
    }

    public void DeathStarKill_Effect(params object[] param)
    {
      PlayerCameraInfo.Shake = 150;
      SoundManager.SetSoundStop("ds_beam");
      SoundManager.SetSound("exp_nave", false, 1, false);
    }
    #endregion
  }
}
