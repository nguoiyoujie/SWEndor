using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Sound;
using SWEndor.Weapons;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSEndor : GameScenarioBase
  {
    public GSEndor()
    {
      Name = "Battle of Endor";
      Description = "The Rebel fleet, amassed on Sullust, prepares to move to Endor "
                  + "where the Emperor is overseeing the construction of the second "
                  + "Death Star.";
      AllowedWings = new List<ActorTypeInfo> { XWingATI.Instance()
                                               , YWingATI.Instance()
                                               , AWingATI.Instance()
                                               , BWingATI.Instance()
                                               , WedgeXWingATI.Instance()
                                               , LandoFalconATI.Instance()
                                               , CorellianATI.Instance()
                                               , VictoryIATI.Instance()
                                               , ImperialIATI.Instance()
                                               , TIE_X1_ATI.Instance()
                                              };

      AllowedDifficulties = new List<string> { "easy"
                                               , "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private ActorInfo m_AEndor = null;
    private ActorInfo m_ADS = null;
    private int m_ADSLaserSourceID = -1;
    private int m_ExecutorStaticID = -1;
    private List<object[]> m_pendingSDspawnlist = new List<object[]>();
    private Dictionary<int, TV_3DVECTOR> m_rebelPosition = new Dictionary<int, TV_3DVECTOR>();
    private int m_SDLeftForShieldDown = 0;

    private float m_Enemy_pull = 0;
    private float m_Enemy_pullrate = 5;

    private int m_ADS_targetID = -1;
    private int m_FalconID = -1;
    private int m_WedgeID = -1;
    private int m_PlayerID = -1;
    private TV_3DVECTOR m_Player_pos = new TV_3DVECTOR();
    private string m_Player_PrimaryWeapon = "";
    private string m_Player_SecondaryWeapon = "";

    private int m_HomeOneID = -1;

    public int TIEWaves = 0;
    public int SDWaves = 0;


    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Instance().Name = "Red Three";

      AtmosphereInfo.Instance().ShowSun = false;
      AtmosphereInfo.Instance().ShowFlare = false;
    }

    public override void Launch()
    {
      base.Launch();

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().CameraTargetPoint = new TV_3DVECTOR(0, 0, -100);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -10000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -10000);

      PlayerInfo.Instance().Lives = 4;
      PlayerInfo.Instance().ScorePerLife = 1000000;
      PlayerInfo.Instance().ScoreForNextLife = 1000000;

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

      if (!GameScenarioManager.Instance().GetGameStateB("rebels_arrive"))
      {
        GameScenarioManager.Instance().SetGameStateB("rebels_arrive", true);

        SoundManager.Instance().SetMusic("battle_3_1");
        SoundManager.Instance().SetMusicLoop("battle_3_3");

        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, Rebel_HyperspaceIn);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, Rebel_SetPositions);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, Rebel_MakePlayer);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 9f, Message_01_AllWingsReport);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 12f, Message_02_RedLeader);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13.5f, Message_03_GoldLeader);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, Message_04_BlueLeader);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 16.5f, Message_05_GreenLeader);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 19f, Message_06_Force);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 24f, Message_07_Break);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 25.2f, Message_08_Break);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 26.5f, Message_09_Conf);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 29.5f, Message_10_Break);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 30f, Rebel_GiveControl);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 38f, Message_11_Evasive);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 42f, Message_12_Trap);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 50f, Message_13_Fighters);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 45f, Empire_SpawnStatics);
      }

      GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      MainAllyFaction.WingLimit = 75;

      GameScenarioManager.Instance().IsCutsceneMode = false;
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
      if (m_AEndor == null)
      {
        ActorCreationInfo aci_Endor = new ActorCreationInfo(EndorATI.Instance())
        {
          Position = new TV_3DVECTOR(0, -1200, 0),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          InitialScale = new TV_3DVECTOR(6, 6, 6)
        };
        m_AEndor = ActorInfo.Create(aci_Endor);
      }

      // Create DeathStar
      if (m_ADS == null)
      {
        ActorCreationInfo aci_DS = new ActorCreationInfo(DeathStar2ATI.Instance())
        {
          Position = new TV_3DVECTOR(0, 800, 18000),
          Rotation = new TV_3DVECTOR(0, 0, 5),
          Faction = MainEnemyFaction
        };
        m_ADS = ActorInfo.Create(aci_DS);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      if (PlayerInfo.Instance().Actor != null && PlayerInfo.Instance().IsMovementControlsEnabled && GameScenarioManager.Instance().GetGameStateB("in_battle"))
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
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5.5f, Message_14_Interceptors);
        }
        else if (TIEWaves > 11 && StageNumber == 3)
        {
          StageNumber = 4;
        }
        else if (SDWaves >= 2 && MainEnemyFaction.GetShips().Count == 0 && m_pendingSDspawnlist.Count == 0 && StageNumber == 4)
        {
          StageNumber = 5;
          SoundManager.Instance().SetMusic("battle_3_4");
          SoundManager.Instance().SetMusicLoop("battle_3_4");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5.5f, Message_15_Bombers);
        }
        else if (TIEWaves > 17 && StageNumber == 5)
        {
          StageNumber = 6;
        }

        // Wedge and Falcon
        if (!GameScenarioManager.Instance().GetGameStateB("deathstar_noshield")
          && StageNumber == 4
          && SDWaves >= 2
          && (m_pendingSDspawnlist.Count + MainEnemyFaction.GetShips().Count) <= m_SDLeftForShieldDown)
        {
          GameScenarioManager.Instance().SetGameStateB("deathstar_noshield", true);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3f, Message_30_ShieldDown);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7f, Message_31_ResumeAttack);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, Rebel_DeathStarGo);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 16f, Rebel_ShipsForward_2);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 12f, Message_32_Han);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 17f, Message_33_Han);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 20f, Message_34_Han);
        }

        // TIE spawn
        if (TIESpawnTime < Game.Instance().GameTime)
        {
          if (( MainEnemyFaction.GetWings().Count < 36 &&  MainEnemyFaction.GetShips().Count == 0 && StageNumber == 1)
            || ( MainEnemyFaction.GetWings().Count < 32 &&  MainEnemyFaction.GetShips().Count == 0 && StageNumber == 3)
            || ( MainEnemyFaction.GetWings().Count < 28 &&  MainEnemyFaction.GetShips().Count == 0)
            || ( MainEnemyFaction.GetWings().Count < 14 &&  MainEnemyFaction.GetShips().Count > 0))
          {
            TIESpawnTime = Game.Instance().GameTime + 10f;

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
                  Empire_TIEWave_01(new object[] { 4 });
                  break;
                case "hard":
                  if (TIEWaves % 3 == 1)
                  {
                    Empire_TIEWave_01(new object[] { 4 });
                    Empire_TIEWave_TIEsvsWedge(new object[] { 2 });
                    Empire_TIEWave_TIEsvsFalcon(new object[] { 2 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 1 });
                  }
                  else
                  {
                    Empire_TIEWave_01(new object[] { 4 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 2 });
                  }
                  break;
                case "mental":
                  if (TIEWaves % 2 == 1)
                  {
                    Empire_TIEWave_01(new object[] { 5 });
                    Empire_TIEWave_TIEsvsWedge(new object[] { 2 });
                    Empire_TIEWave_TIEsvsFalcon(new object[] { 2 });
                    Empire_TIEWave_TIEsvsPlayer(new object[] { 1 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 2 });
                  }
                  else
                  {
                    Empire_TIEWave_01(new object[] { 6 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 2 });
                  }
                  break;
                case "normal":
                default:
                  Empire_TIEWave_01(new object[] { 5 });
                  break;
              }
            }
            else if (StageNumber == 3 || StageNumber == 4)
            {
              Rebel_GoBack(0.3f);
              switch (Difficulty.ToLower())
              {
                case "easy":
                  Empire_TIEWave_02(new object[] { 4 });
                  break;
                case "hard":
                  if (TIEWaves % 3 == 1)
                  {
                    Empire_TIEWave_02(new object[] { 4 });
                    Empire_TIEWave_InterceptorsvsWedge(new object[] { 2 });
                    Empire_TIEWave_InterceptorsvsFalcon(new object[] { 2 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 1 });
                  }
                  else
                  {
                    Empire_TIEWave_02(new object[] { 4 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 2 });
                  }

                  break;
                case "mental":
                  if (TIEWaves % 2 == 1)
                  {
                    Empire_TIEWave_02(new object[] { 5 });
                    Empire_TIEWave_InterceptorsvsWedge(new object[] { 2 });
                    Empire_TIEWave_InterceptorsvsFalcon(new object[] { 2 });
                    Empire_TIEWave_InterceptorsvsPlayer(new object[] { 2 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 2 });
                  }
                  else
                  {
                    Empire_TIEWave_02(new object[] { 6 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 2 });
                  }
                  break;
                case "normal":
                default:
                  Empire_TIEWave_02(new object[] { 5 });
                  break;
              }
            }
            else if (StageNumber == 5 || StageNumber == 6)
            {
              switch (Difficulty.ToLower())
              {
                case "easy":
                  Empire_TIEWave_01(new object[] { 2 });
                  Empire_TIEWave_03(new object[] { 2 });
                  Empire_TIEBombers(new object[] { 1 });
                  break;
                case "hard":
                  if (TIEWaves % 3 == 1)
                  {
                    Empire_TIEWave_02(new object[] { 2 });
                    Empire_TIEWave_03(new object[] { 2 });
                    Empire_TIEBombers(new object[] { 2 });
                    Empire_TIEWave_TIEsvsPlayer(new object[] { 2 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 1 });
                  }
                  else
                  {
                    Empire_TIEWave_02(new object[] { 2 });
                    Empire_TIEWave_03(new object[] { 3 });
                    Empire_TIEBombers(new object[] { 2 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 2 });
                  }
                  break;
                case "mental":
                  if (TIEWaves % 2 == 1)
                  {
                    Empire_TIEWave_03(new object[] { 5 });
                    Empire_TIEBombers(new object[] { 2 });
                    Empire_TIEWave_InterceptorsvsPlayer(new object[] { 2 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 3 });
                  }
                  else
                  {
                    Empire_TIEWave_03(new object[] { 5 });
                    Empire_TIEBombers(new object[] { 2 });
                    Empire_TIEWave_TIEsvsShips(new object[] { 3 });
                  }
                  break;
                case "normal":
                default:
                  Empire_TIEWave_02(new object[] { 2 });
                  Empire_TIEWave_03(new object[] { 3 });
                  Empire_TIEBombers(new object[] { 2 });
                  break;
              }
            }
          }
        }

        if (m_pendingSDspawnlist.Count > 0 &&  MainEnemyFaction.GetShips().Count < 8)
        {
          if (m_pendingSDspawnlist[0].Length > 0
            && (!(m_pendingSDspawnlist[0][0] is ImperialIATI) ||  MainEnemyFaction.GetShips().Count < ((StageNumber == 6) ? 4 : 2))
            && (!(m_pendingSDspawnlist[0][0] is DevastatorATI) ||  MainEnemyFaction.GetShips().Count < 2))
          {
            Empire_StarDestroyer_Spawn(m_pendingSDspawnlist[0]);
            m_pendingSDspawnlist.RemoveAt(0);
          }
        }

        if (m_Enemy_pull > 0)
        {
          m_Enemy_pull -= Game.Instance().TimeSinceRender * m_Enemy_pullrate;
          foreach (int enemyshipID in  MainEnemyFaction.GetShips())
          {
            ActorInfo enemyship = ActorInfo.Factory.Get(enemyshipID);
            if (enemyship != null)
              enemyship.MoveAbsolute(0, 0, Game.Instance().TimeSinceRender * m_Enemy_pullrate);
          }
        }

        //Rebel_ForceAwayFromBounds(null);

        if (StageNumber == 2 && !GameScenarioManager.Instance().GetGameStateB("DS2"))
        {
          GameScenarioManager.Instance().SetGameStateB("DS2", true);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, Empire_DeathStarAttack_01);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, Message_20_DeathStar);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, Empire_StarDestroyer_01);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 20f, Message_21_Close);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 25f, Rebel_YWingsAttackScan);
          GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -17500);
          GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -17500);
        }
        else if (StageNumber == 4 && !GameScenarioManager.Instance().GetGameStateB("DS4"))
        {
          GameScenarioManager.Instance().SetGameStateB("DS4", true);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, Empire_DeathStarAttack_02);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 16f, Rebel_ShipsForward);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13f, Message_22_PointBlank);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, Empire_StarDestroyer_02);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 18f, Message_23_Take);
          GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -22500);
          GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -22500);
        }
        else if (StageNumber == 6 && !GameScenarioManager.Instance().GetGameStateB("DS6"))
        {
          GameScenarioManager.Instance().SetGameStateB("DS6", true);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, Empire_DeathStarAttack_03);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, Empire_Executor);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13f, Message_40_Focus);
          GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -25000);
          GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -25000);
        }
      }

      if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().GameTime || Game.Instance().GameTime % 0.2f > 0.1f)
      {
        GameScenarioManager.Instance().Line1Text = string.Format("WINGS: {0}", MainAllyFaction.WingLimit);
      }
      else
      {
        GameScenarioManager.Instance().Line1Text = string.Format("");
      }

      if (GameScenarioManager.Instance().Scenario.TimeSinceLostShip < Game.Instance().GameTime || Game.Instance().GameTime % 0.2f > 0.1f)
      {
        GameScenarioManager.Instance().Line2Text = string.Format("SHIPS: {0}", MainAllyFaction.ShipLimit);
      }
      else
      {
        GameScenarioManager.Instance().Line2Text = string.Format("");
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AEndor != null && m_AEndor.CreationState == CreationState.ACTIVE)
      {
        float x_en = PlayerInfo.Instance().Position.x / 1.2f;
        float y_en = (PlayerInfo.Instance().Position.y > 0) ? (PlayerInfo.Instance().Position.y / 6f) - 9000.0f : (PlayerInfo.Instance().Position.y / 2.5f) - 9000.0f;
        float z_en = PlayerInfo.Instance().Position.z / 1.2f;
        m_AEndor.SetLocalPosition(x_en, y_en, z_en);
      }
      if (m_ADS != null && m_ADS.CreationState == CreationState.ACTIVE)
      {
        float x_ds = PlayerInfo.Instance().Position.x / 5f;
        float y_ds = (PlayerInfo.Instance().Position.y / 1.5f) + 1200.0f;
        float z_ds = (PlayerInfo.Instance().Position.z > 0) ? PlayerInfo.Instance().Position.z / 1.5f + 30000f : PlayerInfo.Instance().Position.z / 50f + 30000f;
        m_ADS.SetLocalPosition(x_ds, y_ds, z_ds);
      }
    }


    #region Rebellion spawns

    private void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      ActionInfo[] actions;
      string[] registries;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.Instance().GameTime;
      float creationDelay = 0.025f;

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(10, -20, 1500);

      ActorTypeInfo type;
      string name;
      string sidebar_name;
      FactionInfo faction;

      // Millennium Falcon
      type = LandoFalconATI.Instance();
      name = "Millennium Falcon (Lando)";
      sidebar_name = "FALCON";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Falcon");
      position = new TV_3DVECTOR(0, -10, 350);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
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
      type = WedgeXWingATI.Instance();
      name = "X-Wing (Wedge)";
      sidebar_name = "WEDGE";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Wedge");
      position = new TV_3DVECTOR(70, 20, 250);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
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
      if (PlayerInfo.Instance().ActorType == XWingATI.Instance())
      {
        position = new TV_3DVECTOR(0, 0, -25);
      }
      else if (PlayerInfo.Instance().ActorType == YWingATI.Instance())
      {
        position = new TV_3DVECTOR(-250, 60, -520);
      }
      else if (PlayerInfo.Instance().ActorType == AWingATI.Instance())
      {
        position = new TV_3DVECTOR(100, 70, -720);
      }
      else if (PlayerInfo.Instance().ActorType == BWingATI.Instance())
      {
        position = new TV_3DVECTOR(-80, 20, -50);
      }
      else if (PlayerInfo.Instance().ActorType == CorellianATI.Instance())
      {
        position = new TV_3DVECTOR(-20, -420, -30);
      }

      type = PlayerInfo.Instance().ActorType;
      name = "(Player)";
      sidebar_name = "";
      creationTime += creationDelay;
      faction = MainAllyFaction;
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
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
      PlayerInfo.Instance().TempActorID = ainfo.ID;
      GameScenarioManager.Instance().CameraTargetActor = ainfo;

      // Mon Calamari (HomeOne)
      type = MC90ATI.Instance();
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
      spawns.Add(new object[] { new TV_3DVECTOR(60, 30, -390), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, -50, -320), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(110, 20, -340), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 50, -360), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-120, 20, -380), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(70, -20, -400), XWingATI.Instance() });

      // A-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(200, -10, -750), AWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(80, 20, -800), AWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(270, 40, -850), AWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(65, 65, -900), AWingATI.Instance() });

      // B-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(-150, 50, -250), BWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-290, 80, -280), BWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, 100, -350), BWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-200, 65, -400), BWingATI.Instance() });

      // Y-Wings x6
      spawns.Add(new object[] { new TV_3DVECTOR(-10, 100, -350), YWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(50, 80, -380), YWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 90, -420), YWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-90, 100, -440), YWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(75, 110, -450), YWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 85, -500), YWingATI.Instance() });

      // Corellian x9 (5 forward, 4 rear)
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, -120, 1300), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1400, -320, 400), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2400, 150, 1500), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 470, 850), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 300, 200), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(600, 70, -150), CorellianATI.Instance(), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-700, -150, -600), CorellianATI.Instance(), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-600, 250, -1200), CorellianATI.Instance(), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, 200, -1200), CorellianATI.Instance(), 5500 });

      // Mon Calamari x2 (not including HomeOne)
      spawns.Add(new object[] { new TV_3DVECTOR(4500, -120, -500), MC90ATI.Instance(), 7200 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 550, -1500), MC90ATI.Instance(), 7200 });

      // Nebulon B x3
      spawns.Add(new object[] { new TV_3DVECTOR(-2700, 320, -1850), NebulonBATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1800, -280, -900), NebulonBATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 850, -150), NebulonBATI.Instance(), 8000 });

      // Transport x9
      spawns.Add(new object[] { new TV_3DVECTOR(1200, 550, -750), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1750, 350, 150), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, 280, -1200), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(4000, 280, -800), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-500, -175, -1600), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1650, -230, -800), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 450, 0), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, -280, -1200), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-900, 10, -400), TransportATI.Instance(), 8000 });

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
                                 , new Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
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

    private void Rebel_SetPositions(object[] param)
    {
      foreach (int id in m_rebelPosition.Keys)
      {
        ActorInfo actor = ActorInfo.Factory.Get(id);
        if (actor != null)
        {
          actor.Position = m_rebelPosition[id] + new TV_3DVECTOR(0, 0, actor.TypeInfo.MaxSpeed * 8f);
          actor.MovementInfo.Speed = actor.TypeInfo.MaxSpeed;
        }
      }
    }

    private void Rebel_HyperspaceOut(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                        , actor.MovementInfo.MaxSpeed
                                                        , actor.TypeInfo.Move_CloseEnough));
          ActionManager.QueueLast(actorID, new HyperspaceOut());
          ActionManager.QueueLast(actorID, new Delete());
        }
      }
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                , actor.MovementInfo.MaxSpeed
                                                , actor.TypeInfo.Move_CloseEnough));
          ActionManager.QueueLast(actorID, new HyperspaceOut());
          ActionManager.QueueLast(actorID, new Delete());
        }
      }
    }

    public void Rebel_MakePlayer(object[] param)
    {
      PlayerInfo.Instance().ActorID = PlayerInfo.Instance().TempActorID;
      
      if (PlayerInfo.Instance().Actor != null 
        && PlayerInfo.Instance().Actor.CreationState != CreationState.DISPOSED)
      { 
        // m_Player = PlayerInfo.Instance().Actor;
        if (!GameScenarioManager.Instance().GetGameStateB("in_battle"))
        {
          foreach (int actorID in MainAllyFaction.GetShips())
          {
            ActorInfo actor = ActorInfo.Factory.Get(actorID);
            if (actor != null)
            {
              actor.ActorState = ActorState.FREE;
              actor.MovementInfo.Speed = 275;
            }
          }

          foreach (int actorID in MainAllyFaction.GetWings())
          {
            ActorInfo actor = ActorInfo.Factory.Get(actorID);
            if (actor != null)
            {
              actor.ActorState = ActorState.FREE;
              if (actor.MovementInfo.Speed < 425)
                actor.MovementInfo.Speed = 425;
            }
          }
        }
      }
      else
      {
        if (PlayerInfo.Instance().Lives > 0)
        {
          PlayerInfo.Instance().Lives--;
          PlayerInfo.Instance().RequestSpawn = true;
        }
      }
    }

    public void Rebel_ShipsForward(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() - new TV_3DVECTOR(actor.GetPosition().x * 0.35f, 0, Math.Abs(actor.GetPosition().x) + 1500)
                                                    , actor.MovementInfo.MinSpeed));
          ActionManager.QueueLast(actorID, new Lock());
        }
      }
    }

    public void Rebel_ShipsForward_2(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Move(actor.GetPosition() - new TV_3DVECTOR(actor.GetPosition().x * 0.35f, 0, Math.Abs(actor.GetPosition().x) + 1500)
                                                    , actor.MovementInfo.MaxSpeed));
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() - new TV_3DVECTOR(0, 0, 20000)
                                                    , actor.MovementInfo.MinSpeed));
          ActionManager.QueueLast(actorID, new Lock());
        }
      }
    }

    public void Rebel_YWingsAttackScan(object[] param)
    {
      if (MainEnemyFaction.GetShips().Count > 0)
      {
        foreach (int actorID in MainAllyFaction.GetWings())
        {
          ActorInfo actor = ActorInfo.Factory.Get(actorID);
          if (actor != null)
          {
            if (actor.TypeInfo is YWingATI || actor.TypeInfo is BWingATI)
            {
              int rsID = MainEnemyFaction.GetShips()[Engine.Instance().Random.Next(0, MainEnemyFaction.GetShips().Count)];
              ActorInfo rs = ActorInfo.Factory.Get(actorID);
              {
                foreach (int i in rs.GetAllChildren(1))
                {
                  ActorInfo rc = ActorInfo.Factory.Get(i);
                  if (rc.RegenerationInfo.ParentRegenRate > 0)
                    if (Engine.Instance().Random.NextDouble() > 0.4f)
                      rsID = rc.ID;
                }
              }

              ActionManager.ClearQueue(actorID);
              ActionManager.QueueLast(actorID, new AttackActor(rsID, -1, -1, false));
            }
          }
        }
      }

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, Rebel_YWingsAttackScan);
    }

    public void Rebel_DeathStarGo(object[] param)
    {
      ActorInfo falcon = ActorInfo.Factory.Get(m_FalconID);
      if (falcon != null)
      {
        falcon.CombatInfo.DamageModifier = 0;
        ActionManager.ForceClearQueue(m_FalconID);
        ActionManager.QueueLast(m_FalconID, new ForcedMove(new TV_3DVECTOR(0, 0, 20000), falcon.MovementInfo.MaxSpeed, -1));
        ActionManager.QueueLast(m_FalconID, new HyperspaceOut());
        ActionManager.QueueLast(m_FalconID, new Delete());
        m_FalconID = -1;
      }

      ActorInfo wedge = ActorInfo.Factory.Get(m_WedgeID);
      if (wedge != null)
      {
        wedge.CombatInfo.DamageModifier = 0;
        ActionManager.ForceClearQueue(m_WedgeID);
        ActionManager.QueueLast(m_WedgeID, new ForcedMove(new TV_3DVECTOR(0, 0, 20000), wedge.MovementInfo.MaxSpeed, -1));
        ActionManager.QueueLast(m_WedgeID, new HyperspaceOut());
        ActionManager.QueueLast(m_WedgeID, new Delete());
        m_WedgeID = -1;
      }
    }

    public void Rebel_GiveControl(object[] param)
    {
      ActorInfo falcon = ActorInfo.Factory.Get(m_FalconID);
      ActorInfo wedge = ActorInfo.Factory.Get(m_WedgeID);

      ActionManager.UnlockOne(m_FalconID);
      falcon.ActorState = ActorState.NORMAL;
      falcon.MovementInfo.Speed = falcon.MovementInfo.MaxSpeed;

      ActionManager.UnlockOne(m_WedgeID);
      ActionManager.QueueFirst(m_WedgeID, new Wait(2.5f));
      wedge.ActorState = ActorState.NORMAL;
      wedge.MovementInfo.Speed = wedge.MovementInfo.MaxSpeed;

      float time = 5f;
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);

        ActionManager.UnlockOne(actorID);
        ActionManager.QueueFirst(actorID, new Wait(time));
        actor.ActorState = ActorState.NORMAL;
        actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
        time += 2.5f;
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);

        ActionManager.UnlockOne(actorID);
        actor.ActorState = ActorState.NORMAL;
        actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
        actor.SetSpawnerEnable(true);
      }
      PlayerInfo.Instance().IsMovementControlsEnabled = true;

      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
      GameScenarioManager.Instance().SetGameStateB("TIEs", true);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, Empire_FirstTIEWave);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 45f, Empire_SecondTIEWave);
      Rebel_RemoveTorps(null);
    }

    private void Rebel_GoBack(float chance)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          double d = Engine.Instance().Random.NextDouble();
          if (d < chance)
          {
            ActorInfo homeone = ActorInfo.Factory.Get(m_HomeOneID);

            ActionManager.ClearQueue(actorID);
            ActionManager.QueueLast(actorID, new Move(homeone.GetPosition() + new TV_3DVECTOR(Engine.Instance().Random.Next(-2500, 2500), Engine.Instance().Random.Next(-50, 50), Engine.Instance().Random.Next(-2500, 2500))
                                                       , actor.MovementInfo.MaxSpeed));
          }
        }
      }
    }

    public void Rebel_RemoveTorps(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          if (!actor.IsPlayer())
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

    public void Rebel_RemoveTorpsSingle(object[] param)
    {
      if (param.Length == 0 || !(param[0] is ActorInfo))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];

      if (!ainfo.IsPlayer())
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

    public void Rebel_CriticalUnitHit(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      int actorID = (int)param[0];
      ActorInfo av = ActorInfo.Factory.Get(actorID);

      if (av != null 
        && av.CombatInfo.Strength < av.TypeInfo.MaxStrength * 0.8f 
        && MainAllyFaction.GetShips().Count > 0)
      {
        ActorInfo homeone = ActorInfo.Factory.Get(m_HomeOneID);

        ActionManager.ClearQueue(actorID);
        ActionManager.QueueLast(actorID
                              , new Move(homeone.GetPosition() + new TV_3DVECTOR(Engine.Instance().Random.Next(-2500, 2500)
                              , Engine.Instance().Random.Next(-50, 50)
                              , Engine.Instance().Random.Next(-2500, 2500))
                              , av.MovementInfo.MaxSpeed));
      }
    }

    public void Rebel_CriticalUnitDanger(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      int actorID = (int)param[0];
      ActorInfo av = ActorInfo.Factory.Get(actorID);

      if (av != null
        && av.StrengthFrac < 0.67f 
        && av.StrengthFrac >= 0.33f)
      {
        Screen2D.Instance().MessageText(string.Format("{0}: {1}, I need cover!", av.Name, PlayerInfo.Instance().Name)
                                            , 5
                                            , av.Faction.Color
                                            , 100);
      }
      else if (av.StrengthFrac < 0.33f)
      {
        Screen2D.Instance().MessageText(string.Format("{0}: {1}, Get those TIEs off me!", av.Name, PlayerInfo.Instance().Name)
                                            , 5
                                            , av.Faction.Color
                                            , 100);
      }
    }

    public void Rebel_CriticalUnitDying(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (GameScenarioManager.Instance().GetGameStateB("GameWon"))
        return;

      if (GameScenarioManager.Instance().GetGameStateB("GameOver"))
        return;

      int actorID = (int)param[0];
      ActorInfo actor = ActorInfo.Factory.Get(actorID);

      if (actor != null
        && (actor.ActorState == ActorState.DYING || actor.ActorState == ActorState.DEAD))
      {
        PlayerInfo.Instance().TempActorID = actorID;

        GameScenarioManager.Instance().SetGameStateB("GameOver", true);
        GameScenarioManager.Instance().IsCutsceneMode = true;

        if (GameScenarioManager.Instance().SceneCamera == null || !(GameScenarioManager.Instance().SceneCamera.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(DeathCameraATI.Instance());
          camaci.CreationTime = Game.Instance().GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = actor.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(camaci);
          PlayerInfo.Instance().ActorID = a.ID;
          a.CombatInfo.Strength = 0;
          PlayerInfo.Instance().TempActorID = actorID;

          a.CameraSystemInfo.CamDeathCirclePeriod = actor.CameraSystemInfo.CamDeathCirclePeriod;
          a.CameraSystemInfo.CamDeathCircleRadius = actor.CameraSystemInfo.CamDeathCircleRadius;
          a.CameraSystemInfo.CamDeathCircleHeight = actor.CameraSystemInfo.CamDeathCircleHeight;

          if (actor.ActorState == ActorState.DYING)
          {
            actor.TickEvents += GameScenarioManager.Instance().Scenario.ProcessPlayerDying;
            actor.DestroyedEvents += GameScenarioManager.Instance().Scenario.ProcessPlayerKilled;
          }
          else
          {
            actor.DestroyedEvents += GameScenarioManager.Instance().Scenario.ProcessPlayerKilled;
          }

          if (actor.TypeInfo is WedgeXWingATI)
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, Message_90_LostWedge);
          }
          else if (actor.TypeInfo is LandoFalconATI)
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, Message_91_LostFalcon);
          }
          else if (actor.TypeInfo is MC90ATI)
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15, Message_92_LostHomeOne);
            actor.CombatInfo.TimedLife = 2000f;
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 25, FadeOut);
          }
        }
        else
        {
          GameScenarioManager.Instance().SceneCamera.SetLocalPosition(actor.GetPosition().x, actor.GetPosition().y, actor.GetPosition().z);
        }
      }
    }

    #endregion


    #region Empire spawns
    public void Empire_FirstTIEWave(object[] param)
    {
      List<int> list = m_ADS.GetAllChildren(1);
      if (list.Count > 0)
        m_ADSLaserSourceID = list[0];

      // TIEs
      float t = 0;
      for (int k = 1; k < 9; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);
        float fz = Engine.Instance().Random.Next(-500, 500);

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
              Type = TIE_LN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
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

    public void Empire_SecondTIEWave(object[] param)
    {
      // TIEs
      float t = 0;
      for (int k = 1; k < 3; k++)
      {
        float fx = Engine.Instance().Random.Next(-4500, 4500);
        float fy = Engine.Instance().Random.Next(-500, 500);
        float fz = Engine.Instance().Random.Next(-500, 500);

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
              Type = TIE_LN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
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

    public void Empire_TIEWave_01(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIEs
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

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
              Type = TIE_LN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 8000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 0.25f; // 1.5f
      }
    }

    public void Empire_TIEWave_02(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIEs
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { TIE_LN_ATI.Instance(), TIE_IN_ATI.Instance() };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        int n = Engine.Instance().Random.Next(0, tietypes.Length);
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
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 8000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_03(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

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
              Type = TIE_IN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 5000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
            };

            asi.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_0D(object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE Defenders!
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActionInfo[] actions = new ActionInfo[] { new Wait(8) };
        switch (Difficulty.ToLower())
        {
          case "mental":
            actions = new ActionInfo[] { new Wait(8), new Hunt(TargetType.FIGHTER) };
            break;
        }

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = TIE_D_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 6000),
          Rotation = new TV_3DVECTOR(),
          Actions = actions,
        };

        asi.Spawn(this);
      }
    }

    public void Empire_TIEWave_TIEsvsWedge(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = TIE_LN_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_WedgeID, -1, -1, false) },
        };

        asi.Spawn(this);
      }
    }

    public void Empire_TIEWave_InterceptorsvsWedge(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = TIE_IN_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_WedgeID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsFalcon(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = TIE_LN_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_FalconID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_InterceptorsvsFalcon(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = TIE_IN_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(m_FalconID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsPlayer(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = TIE_LN_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(PlayerInfo.Instance().Actor.ID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsShips(object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = TIE_LN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 2500),
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

    public void Empire_TIEWave_InterceptorsvsPlayer(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = TIE_LN_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(PlayerInfo.Instance().Actor.ID, -1, -1, false) },
        };

        asi.Spawn(this);

        t += 1.5f;
      }
    }

    public void Empire_TIEBombers(object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE Bombers only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = TIE_sa_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 2500),
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

    public void Empire_SpawnStatics(object[] param)
    {
      ActorInfo ainfo;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      float creationTime = Game.Instance().GameTime;

      ActorTypeInfo type;
      string name;

      List<object[]> spawns = new List<object[]>();
      spawns.Add(new object[] { new TV_3DVECTOR(0, 50, -33500), ExecutorStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-1500, 180, -32500), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(1500, 180, -32500), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, 280, -27500), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(5500, 280, -27500), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-5500, -30, -29500), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, -30, -29500), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-7500, 130, -31000), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(7500, 130, -31000), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-11500, -80, -29000), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(11500, -80, -29000), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-14500, 80, -27500), ImperialIStaticATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(14500, 80, -27500), ImperialIStaticATI.Instance() });

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

    public void Empire_StarDestroyer_Spawn(object[] param)
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
        SpawnTime = Game.Instance().GameTime,
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
        Empire_TIEWave_0D(new object[] { 2 });
    }

    public void Empire_StarDestroyer_01(object[] param)
    {
      SDWaves++;

      m_Enemy_pull = 8000;
      switch (Difficulty.ToLower())
      {
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-2500, -250, -19500), new TV_3DVECTOR(-2500, -250, -9500) });
          break;
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-750, -50, -19000), new TV_3DVECTOR(-750, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(750, -50, -19000), new TV_3DVECTOR(750, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });

          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-2250, -50, -19000), new TV_3DVECTOR(-1250, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(2250, -50, -19000), new TV_3DVECTOR(1250, -50, -6500) });
          
          break;
        case "hard":
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(2500, -250, -19500), new TV_3DVECTOR(2000, 110, -9500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-2500, -250, -19500), new TV_3DVECTOR(-2000, 110, -9500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });
          break;
      }
      SoundManager.Instance().SetMusic("battle_3_2");
    }

    public void Empire_StarDestroyer_02(object[] param)
    {
      SDWaves++;

      m_Enemy_pull = 12000;
      switch (Difficulty.ToLower())
      {
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-7500, 60, -24500), new TV_3DVECTOR(-2000, 60, -12500) });
          break;
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-10000, -300, -23500), new TV_3DVECTOR(-1250, -300, -10500) });
          m_pendingSDspawnlist.Add(new object[] { DevastatorATI.Instance(), new TV_3DVECTOR(0, 120, -24500), new TV_3DVECTOR(0, 120, -11500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-2500, 150, -24500), new TV_3DVECTOR(-2000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-3200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(10000, -300, -23500), new TV_3DVECTOR(1250, -300, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(2500, 150, -24500), new TV_3DVECTOR(2000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(3200, 50, -12000) });

          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(4500, -100, -24500), new TV_3DVECTOR(2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(2500, 150, -26500), new TV_3DVECTOR(500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(7500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });

          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2500, -100, -24500), new TV_3DVECTOR(2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(500, 150, -26500), new TV_3DVECTOR(500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          break;
      }
      
      SoundManager.Instance().SetMusic("battle_3_2");
    }

    public void Empire_Executor(object[] param)
    {
      //m_Enemy_pull = 4000;
      SDWaves++;
      ActorInfo.Factory.Get(m_ExecutorStaticID)?.Kill();

      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.Instance().GameTime;

      TV_3DVECTOR position = new TV_3DVECTOR(0, -950, -20000);
      ActorTypeInfo atinfo = ExecutorATI.Instance();

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = atinfo,
        Name = "",
        RegisterName = "",
        SidebarName = "EXECUTOR",
        SpawnTime = Game.Instance().GameTime,
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
          Empire_TIEWave_TIEsvsShips(new object[] { 10 });
          break;
        case "mental":
          Empire_TIEWave_0D (new object[] { 2 });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-550, 80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(550, -160, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-1500, 90, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(1500, 90, -6500) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -80, -24000), new TV_3DVECTOR(-2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2000, -80, -24000), new TV_3DVECTOR(2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });

          break;
        case "hard":
          Empire_TIEWave_0D(new object[] { 1 });
          Empire_TIEWave_TIEsvsShips(new object[] { 5 });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-2500, 120, -6500) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500) });

          break;
        case "normal":
        default:
          Empire_TIEWave_TIEsvsShips(new object[] { 10 });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500) });
          break;
      }
    }

    public void Empire_ExecutorDestroyed(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (GameScenarioManager.Instance().GetGameStateB("GameOver"))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];
      ActorState state = (ActorState)param[1];

      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        GameScenarioManager.Instance().SetGameStateB("GameWon", true);
        GameScenarioManager.Instance().IsCutsceneMode = true;

        if (GameScenarioManager.Instance().SceneCamera == null || !(GameScenarioManager.Instance().SceneCamera.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(DeathCameraATI.Instance());
          camaci.CreationTime = Game.Instance().GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = ainfo.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(camaci);
          PlayerInfo.Instance().ActorID = a.ID;
          a.CombatInfo.Strength = 0;

          a.CameraSystemInfo.CamDeathCirclePeriod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
          a.CameraSystemInfo.CamDeathCircleRadius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
          a.CameraSystemInfo.CamDeathCircleHeight = ainfo.CameraSystemInfo.CamDeathCircleHeight;

          if (ainfo.ActorState == ActorState.DYING)
          {
            ainfo.TickEvents += ProcessPlayerDying;
            ainfo.DestroyedEvents += ProcessPlayerKilled;
          }
          else
          {
            ainfo.DestroyedEvents += ProcessPlayerKilled;
          }

          SoundManager.Instance().SetMusic("executorend");
          ainfo.CombatInfo.TimedLife = 2000f;

          ActorInfo homeone = ActorInfo.Factory.Get(m_HomeOneID);
          if (homeone != null)
            homeone.CombatInfo.DamageModifier = 0;
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 55, FadeOut);
        }
        else
        {
          GameScenarioManager.Instance().SceneCamera.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
        }
      }
    }

    public void Empire_DeathStarAttack(object[] param)
    {
      ActorInfo lsrSrc = ActorInfo.Factory.Get(m_ADSLaserSourceID);
      if (lsrSrc != null 
        && lsrSrc.CreationState == CreationState.ACTIVE)
      {
        List<int> victims = new List<int>(MainAllyFaction.GetShips());
        victims.Remove(m_HomeOneID);

        if (victims.Count > 0)
        {
          m_ADS_targetID = victims[Engine.Instance().Random.Next(0, victims.Count)];

          ActionManager.ForceClearQueue(m_ADSLaserSourceID);
          ActionManager.QueueNext(m_ADSLaserSourceID, new AttackActor(m_ADS_targetID));
          ActionManager.QueueNext(m_ADSLaserSourceID, new Lock());

          ActorInfo target = ActorInfo.Factory.Get(m_ADS_targetID);
          target.DestroyedEvents += DeathStarKill_Effect;
          GameScenarioManager.Instance().AddEvent(0.1f, Scene_DeathStarCam);
        }
      }
    }

    public void Empire_DeathStarAttack_01(object[] param)
    {
      Empire_DeathStarAttack(CorellianATI.Instance());
    }

    public void Empire_DeathStarAttack_02(object[] param)
    {
      Empire_DeathStarAttack(TransportATI.Instance());
    }

    public void Empire_DeathStarAttack_03(object[] param)
    {
      Empire_DeathStarAttack(MC90ATI.Instance());
    }

    private void Empire_DeathStarAttack(ActorTypeInfo type)
    {
      ActorInfo lsrSrc = ActorInfo.Factory.Get(m_ADSLaserSourceID);
      if (lsrSrc != null
        && lsrSrc.CreationState == CreationState.ACTIVE)
      {
        List<int> tgt = new List<int>(MainAllyFaction.GetShips());
        tgt.Reverse();

        foreach (int tid in tgt)
        {
          ActorInfo t = ActorInfo.Factory.Get(tid);
          if (t != null 
            && t.TypeInfo == type
            && tid != m_HomeOneID)
          {
            m_ADS_targetID = tid;
            
            ActionManager.ForceClearQueue(m_ADSLaserSourceID);
            ActionManager.QueueNext(m_ADSLaserSourceID, new AttackActor(tid));
            ActionManager.QueueNext(m_ADSLaserSourceID, new Lock());

            t.DestroyedEvents += DeathStarKill_Effect;
            GameScenarioManager.Instance().AddEvent(0.1f, Scene_DeathStarCam);
            return;
          }
        }
      }
    }

    #endregion

    #region Text
    public void Message_01_AllWingsReport(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: All wings report in.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_02_RedLeader(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING (WEDGE): Red Leader standing by.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_03_GoldLeader(object[] param)
    {
      Screen2D.Instance().MessageText("Y-WING: Gray Leader standing by.", 5, new TV_COLOR(0.6f, 0.6f, 0.6f, 1));
    }

    public void Message_04_BlueLeader(object[] param)
    {
      Screen2D.Instance().MessageText("B-WING: Blue Leader standing by.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_05_GreenLeader(object[] param)
    {
      Screen2D.Instance().MessageText("A-WING: Green Leader standing by.", 5, new TV_COLOR(0.4f, 0.8f, 0.4f, 1));
    }

    public void Message_06_Force(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): May the Force be with us.", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_07_Break(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: Break off the attack!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_08_Break(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: The shield's still up!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_09_Conf(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING (WEDGE): I get no reading, are you sure?", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_10_Break(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: All wings, pull up!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_11_Evasive(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): Take evasive action!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_12_Trap(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): It's a trap!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_13_Fighters(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING (WEDGE): Watch out for enemy fighters.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_14_Interceptors(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING (WEDGE): TIE Interceptors inbound.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_15_Bombers(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): We have bombers inbound. Keep them away from our cruisers!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_20_DeathStar(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: That blast came from the Death Star...", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_21_Close(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: Get us close to those Imperial Star Destroyers.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_22_PointBlank(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: Closer! Get us closer, and engage them at point blank range.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_23_Take(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: If they fire, we might even take a few of them with us.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_30_ShieldDown(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): The shield is down.", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_31_ResumeAttack(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): Commence your attack on the Death Star's main reactor", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_32_Han(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: I knew Han can do it!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_33_Han(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: Wedge, follow me.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_34_Han(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING (WEDGE): Copy, Gold Leader.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_40_Focus(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): Focus all firepower on that Super Star Destroyer!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_90_LostWedge(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING (WEDGE): I can't hold them!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_91_LostFalcon(object[] param)
    {
      Screen2D.Instance().MessageText("MILLIENNIUM FALCON: I can't hold them!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_92_LostHomeOne(object[] param)
    {
      Screen2D.Instance().MessageText("MON CALAMARI (HOME ONE): We have no chance...", 15, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    #endregion

    #region Scene
    public void Scene_EnterCutscene(object[] param)
    {
      ActorInfo falcon = ActorInfo.Factory.Get(m_FalconID);
      ActorInfo wedge = ActorInfo.Factory.Get(m_WedgeID);
      ActorInfo homeone = ActorInfo.Factory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.CombatInfo.DamageModifier = 0;

      if (wedge != null)
        wedge.CombatInfo.DamageModifier = 0;

      if (homeone != null)
        homeone.CombatInfo.DamageModifier = 0;

      m_PlayerID = PlayerInfo.Instance().ActorID;
      ActorInfo player = ActorInfo.Factory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_pos = player.Position;
        ActionManager.QueueFirst(m_PlayerID, new Lock());
        player.Position = new TV_3DVECTOR(30, 0, -100000);
        m_Player_PrimaryWeapon = PlayerInfo.Instance().PrimaryWeapon;
        m_Player_SecondaryWeapon = PlayerInfo.Instance().SecondaryWeapon;
        player.CombatInfo.DamageModifier = 0;
      }

      PlayerInfo.Instance().ActorID = GameScenarioManager.Instance().SceneCamera.ID;
      GameScenarioManager.Instance().IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(object[] param)
    {
      ActorInfo falcon = ActorInfo.Factory.Get(m_FalconID);
      ActorInfo wedge = ActorInfo.Factory.Get(m_WedgeID);
      ActorInfo homeone = ActorInfo.Factory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.CombatInfo.DamageModifier = 1;

      if (wedge != null)
        wedge.CombatInfo.DamageModifier = 1;

      if (homeone != null)
        homeone.CombatInfo.DamageModifier = 1;

      ActorInfo player = ActorInfo.Factory.Get(m_PlayerID);
      if (player != null)
      {
        player.ActorState = ActorState.NORMAL;
        player.Position = m_Player_pos;
        ActionManager.ForceClearQueue(m_PlayerID);
        PlayerInfo.Instance().ActorID = m_PlayerID;
        PlayerInfo.Instance().PrimaryWeapon = m_Player_PrimaryWeapon;
        PlayerInfo.Instance().SecondaryWeapon = m_Player_SecondaryWeapon;
        player.CombatInfo.DamageModifier = 1;
      }

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam(object[] param)
    {
      ActorInfo target = ActorInfo.Factory.Get(m_ADS_targetID);

      if (target != null 
        && target.CreationState == CreationState.ACTIVE)
      {
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, Scene_EnterCutscene);
        SoundManager.Instance().SetSound("ds_beam", false, 1, false);

        TV_3DVECTOR pos = target.GetPosition();
        TV_3DVECTOR rot = target.GetRotation();

        if (target.TypeInfo is CorellianATI)
         pos += new TV_3DVECTOR(150, 120, -2000); 
        else if (target.TypeInfo is TransportATI)
         pos += new TV_3DVECTOR(300, 200, 700); 
        else if (target.TypeInfo is MC90ATI)
         pos += new TV_3DVECTOR(-850, -400, 2500); 

        GameScenarioManager.Instance().SceneCamera.SetLocalPosition(pos.x, pos.y, pos.z);
        GameScenarioManager.Instance().SceneCamera.LookAtPoint(new TV_3DVECTOR());
        GameScenarioManager.Instance().SceneCamera.MovementInfo.Speed = 50;
        GameScenarioManager.Instance().SceneCamera.MovementInfo.MaxSpeed = 50;
        GameScenarioManager.Instance().CameraTargetActor = target;
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, Scene_ExitCutscene);
      }
    }

    public void DeathStarKill_Effect(object[] param)
    {
      PlayerCameraInfo.Instance().Shake = 150;
      SoundManager.Instance().SetSoundStop("ds_beam");
      SoundManager.Instance().SetSound("exp_nave", false, 1, false);
    }
    #endregion
  }
}
