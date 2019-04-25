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
    public GSEndor(GameScenarioManager manager) : base(manager)
    {
      Name = "Battle of Endor";
      Description = "The Rebel fleet, amassed on Sullust, prepares to move to Endor "
                  + "where the Emperor is overseeing the construction of the second "
                  + "Death Star.";
      AllowedWings = new List<ActorTypeInfo> { Manager.Engine.ActorTypeFactory.Get("X-Wing")
                                               , Manager.Engine.ActorTypeFactory.Get("Y-Wing")
                                               , Manager.Engine.ActorTypeFactory.Get("A-Wing")
                                               , Manager.Engine.ActorTypeFactory.Get("B-Wing")
                                               , Manager.Engine.ActorTypeFactory.Get("Millennium Falcon")
                                               , Manager.Engine.ActorTypeFactory.Get("Corellian Corvette")
                                               , Manager.Engine.ActorTypeFactory.Get("Victory-I Star Destroyer")
                                               , Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                               , Manager.Engine.ActorTypeFactory.Get("TIE Advanced X1")
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

    private float TIESpawnTime = 0;
    private int TIEWaves = 0;
    private int SDWaves = 0;


    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      Globals.Engine.PlayerInfo.Name = "Red Three";

      Globals.Engine.AtmosphereInfo.ShowSun = false;
      Globals.Engine.AtmosphereInfo.ShowFlare = false;
    }

    public override void Launch()
    {
      base.Launch();

      Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(0, 0, 0);
      Globals.Engine.GameScenarioManager.CameraTargetPoint = new TV_3DVECTOR(0, 0, -100);
      Globals.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
      Globals.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(-20000, -1500, -10000);
      Globals.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
      Globals.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -10000);

      Globals.Engine.PlayerInfo.Lives = 4;
      Globals.Engine.PlayerInfo.ScorePerLife = 1000000;
      Globals.Engine.PlayerInfo.ScoreForNextLife = 1000000;

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

      if (!Globals.Engine.GameScenarioManager.GetGameStateB("rebels_arrive"))
      {
        Globals.Engine.GameScenarioManager.SetGameStateB("rebels_arrive", true);

        Globals.Engine.SoundManager.SetMusic("battle_3_1");
        Globals.Engine.SoundManager.SetMusicLoop("battle_3_3");

        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Rebel_HyperspaceIn);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 8f, Rebel_SetPositions);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 8f, Rebel_MakePlayer);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 9f, Message_01_AllWingsReport);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 12f, Message_02_RedLeader);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 13.5f, Message_03_GoldLeader);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15f, Message_04_BlueLeader);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 16.5f, Message_05_GreenLeader);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 19f, Message_06_Force);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 24f, Message_07_Break);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 25.2f, Message_08_Break);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 26.5f, Message_09_Conf);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 29.5f, Message_10_Break);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 30f, Rebel_GiveControl);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 38f, Message_11_Evasive);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 42f, Message_12_Trap);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 50f, Message_13_Fighters);
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 45f, Empire_SpawnStatics);
      }

      Globals.Engine.GameScenarioManager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Globals.Engine.GameScenarioManager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Globals.Engine.GameScenarioManager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      MainAllyFaction.WingLimit = 75;

      Globals.Engine.GameScenarioManager.IsCutsceneMode = false;
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
        ActorCreationInfo aci_Endor = new ActorCreationInfo(Manager.Engine.ActorTypeFactory.Get("Endor"))
        {
          Position = new TV_3DVECTOR(0, -1200, 0),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          InitialScale = new TV_3DVECTOR(6, 6, 6)
        };
        m_AEndor = ActorInfo.Create(Manager.Engine.ActorFactory, aci_Endor);
      }

      // Create DeathStar
      if (m_ADS == null)
      {
        ActorCreationInfo aci_DS = new ActorCreationInfo(Manager.Engine.ActorTypeFactory.Get("DeathStar2"))
        {
          Position = new TV_3DVECTOR(0, 800, 18000),
          Rotation = new TV_3DVECTOR(0, 0, 5),
          Faction = MainEnemyFaction
        };
        m_ADS = ActorInfo.Create(Manager.Engine.ActorFactory, aci_DS);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      if (Globals.Engine.PlayerInfo.Actor != null && Globals.Engine.PlayerInfo.IsMovementControlsEnabled && Globals.Engine.GameScenarioManager.GetGameStateB("in_battle"))
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
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5.5f, Message_14_Interceptors);
        }
        else if (TIEWaves > 11 && StageNumber == 3)
        {
          StageNumber = 4;
        }
        else if (SDWaves >= 2 && MainEnemyFaction.GetShips().Count == 0 && m_pendingSDspawnlist.Count == 0 && StageNumber == 4)
        {
          StageNumber = 5;
          Globals.Engine.SoundManager.SetMusic("battle_3_4");
          Globals.Engine.SoundManager.SetMusicLoop("battle_3_4");
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5.5f, Message_15_Bombers);
        }
        else if (TIEWaves > 17 && StageNumber == 5)
        {
          StageNumber = 6;
        }

        // Wedge and Falcon
        if (!Globals.Engine.GameScenarioManager.GetGameStateB("deathstar_noshield")
          && StageNumber == 4
          && SDWaves >= 2
          && (m_pendingSDspawnlist.Count + MainEnemyFaction.GetShips().Count) <= m_SDLeftForShieldDown)
        {
          Globals.Engine.GameScenarioManager.SetGameStateB("deathstar_noshield", true);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 3f, Message_30_ShieldDown);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 7f, Message_31_ResumeAttack);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15f, Rebel_DeathStarGo);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 16f, Rebel_ShipsForward_2);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 12f, Message_32_Han);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 17f, Message_33_Han);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 20f, Message_34_Han);
        }

        // TIE spawn
        if (TIESpawnTime < Globals.Engine.Game.GameTime)
        {
          if (( MainEnemyFaction.GetWings().Count < 36 &&  MainEnemyFaction.GetShips().Count == 0 && StageNumber == 1)
            || ( MainEnemyFaction.GetWings().Count < 32 &&  MainEnemyFaction.GetShips().Count == 0 && StageNumber == 3)
            || ( MainEnemyFaction.GetWings().Count < 28 &&  MainEnemyFaction.GetShips().Count == 0)
            || ( MainEnemyFaction.GetWings().Count < 14 &&  MainEnemyFaction.GetShips().Count > 0))
          {
            TIESpawnTime = Globals.Engine.Game.GameTime + 10f;

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
          m_Enemy_pull -= Globals.Engine.Game.TimeSinceRender * m_Enemy_pullrate;
          foreach (int enemyshipID in  MainEnemyFaction.GetShips())
          {
            ActorInfo enemyship = Manager.Engine.ActorFactory.Get(enemyshipID);
            if (enemyship != null)
              enemyship.MoveAbsolute(0, 0, Globals.Engine.Game.TimeSinceRender * m_Enemy_pullrate);
          }
        }

        //Rebel_ForceAwayFromBounds(null);

        if (StageNumber == 2 && !Globals.Engine.GameScenarioManager.GetGameStateB("DS2"))
        {
          Globals.Engine.GameScenarioManager.SetGameStateB("DS2", true);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime, Empire_DeathStarAttack_01);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 8f, Message_20_DeathStar);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15f, Empire_StarDestroyer_01);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 20f, Message_21_Close);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 25f, Rebel_YWingsAttackScan);
          Globals.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Globals.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(-20000, -1500, -17500);
          Globals.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Globals.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -17500);
        }
        else if (StageNumber == 4 && !Globals.Engine.GameScenarioManager.GetGameStateB("DS4"))
        {
          Globals.Engine.GameScenarioManager.SetGameStateB("DS4", true);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime, Empire_DeathStarAttack_02);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 16f, Rebel_ShipsForward);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 13f, Message_22_PointBlank);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15f, Empire_StarDestroyer_02);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 18f, Message_23_Take);
          Globals.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Globals.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(-20000, -1500, -22500);
          Globals.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Globals.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -22500);
        }
        else if (StageNumber == 6 && !Globals.Engine.GameScenarioManager.GetGameStateB("DS6"))
        {
          Globals.Engine.GameScenarioManager.SetGameStateB("DS6", true);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime, Empire_DeathStarAttack_03);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 8f, Empire_Executor);
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 13f, Message_40_Focus);
          Globals.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Globals.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(-20000, -1500, -25000);
          Globals.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          Globals.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(-20000, -1500, -25000);
        }
      }

      if (Globals.Engine.GameScenarioManager.Scenario.TimeSinceLostWing < Globals.Engine.Game.GameTime || Globals.Engine.Game.GameTime % 0.2f > 0.1f)
      {
        Globals.Engine.GameScenarioManager.Line1Text = string.Format("WINGS: {0}", MainAllyFaction.WingLimit);
      }
      else
      {
        Globals.Engine.GameScenarioManager.Line1Text = string.Format("");
      }

      if (Globals.Engine.GameScenarioManager.Scenario.TimeSinceLostShip < Globals.Engine.Game.GameTime || Globals.Engine.Game.GameTime % 0.2f > 0.1f)
      {
        Globals.Engine.GameScenarioManager.Line2Text = string.Format("SHIPS: {0}", MainAllyFaction.ShipLimit);
      }
      else
      {
        Globals.Engine.GameScenarioManager.Line2Text = string.Format("");
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AEndor != null && m_AEndor.CreationState == CreationState.ACTIVE)
      {
        float x_en = Globals.Engine.PlayerInfo.Position.x / 1.2f;
        float y_en = (Globals.Engine.PlayerInfo.Position.y > 0) ? (Globals.Engine.PlayerInfo.Position.y / 6f) - 9000.0f : (Globals.Engine.PlayerInfo.Position.y / 2.5f) - 9000.0f;
        float z_en = Globals.Engine.PlayerInfo.Position.z / 1.2f;
        m_AEndor.SetLocalPosition(x_en, y_en, z_en);
      }
      if (m_ADS != null && m_ADS.CreationState == CreationState.ACTIVE)
      {
        float x_ds = Globals.Engine.PlayerInfo.Position.x / 5f;
        float y_ds = (Globals.Engine.PlayerInfo.Position.y / 1.5f) + 1200.0f;
        float z_ds = (Globals.Engine.PlayerInfo.Position.z > 0) ? Globals.Engine.PlayerInfo.Position.z / 1.5f + 30000f : Globals.Engine.PlayerInfo.Position.z / 50f + 30000f;
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
      float creationTime = Globals.Engine.Game.GameTime;
      float creationDelay = 0.025f;

      Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(10, -20, 1500);

      ActorTypeInfo type;
      string name;
      string sidebar_name;
      FactionInfo faction;

      // Millennium Falcon
      type = Manager.Engine.ActorTypeFactory.Get("Millennium Falcon (Lando)");
      name = "Millennium Falcon (Lando)";
      sidebar_name = "FALCON";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Falcon");
      position = new TV_3DVECTOR(0, -10, 350);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x + Globals.Engine.Random.Next(-5, 5), position.y + Globals.Engine.Random.Next(-5, 5), -position.z - 1500)
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
      type = Manager.Engine.ActorTypeFactory.Get("X-Wing (Wedge)");
      name = "X-Wing (Wedge)";
      sidebar_name = "WEDGE";
      creationTime += creationDelay;
      faction = FactionInfo.Factory.Get("Rebels_Wedge");
      position = new TV_3DVECTOR(70, 20, 250);
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x + Globals.Engine.Random.Next(-5, 5), position.y + Globals.Engine.Random.Next(-5, 5), -position.z - 1500)
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
      if (Globals.Engine.PlayerInfo.ActorType == Manager.Engine.ActorTypeFactory.Get("X-Wing"))
      {
        position = new TV_3DVECTOR(0, 0, -25);
      }
      else if (Globals.Engine.PlayerInfo.ActorType == Manager.Engine.ActorTypeFactory.Get("Y-Wing"))
      {
        position = new TV_3DVECTOR(-250, 60, -520);
      }
      else if (Globals.Engine.PlayerInfo.ActorType == Manager.Engine.ActorTypeFactory.Get("A-Wing"))
      {
        position = new TV_3DVECTOR(100, 70, -720);
      }
      else if (Globals.Engine.PlayerInfo.ActorType == Manager.Engine.ActorTypeFactory.Get("B-Wing"))
      {
        position = new TV_3DVECTOR(-80, 20, -50);
      }
      else if (Globals.Engine.PlayerInfo.ActorType == Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"))
      {
        position = new TV_3DVECTOR(-20, -420, -30);
      }

      type = Globals.Engine.PlayerInfo.ActorType;
      name = "(Player)";
      sidebar_name = "";
      creationTime += creationDelay;
      faction = MainAllyFaction;
      actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Lock()
                                 , new Move(new TV_3DVECTOR(position.x + Globals.Engine.Random.Next(-5, 5), position.y + Globals.Engine.Random.Next(-5, 5), -position.z - 1500)
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
      Globals.Engine.PlayerInfo.TempActorID = ainfo.ID;
      Globals.Engine.GameScenarioManager.CameraTargetActor = ainfo;

      // Mon Calamari (HomeOne)
      type = Manager.Engine.ActorTypeFactory.Get("Mon Calamari Capital Ship");
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
      spawns.Add(new object[] { new TV_3DVECTOR(60, 30, -390), Manager.Engine.ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, -50, -320), Manager.Engine.ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(110, 20, -340), Manager.Engine.ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 50, -360), Manager.Engine.ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-120, 20, -380), Manager.Engine.ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(70, -20, -400), Manager.Engine.ActorTypeFactory.Get("X-Wing") });

      // A-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(200, -10, -750), Manager.Engine.ActorTypeFactory.Get("A-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(80, 20, -800), Manager.Engine.ActorTypeFactory.Get("A-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(270, 40, -850), Manager.Engine.ActorTypeFactory.Get("A-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(65, 65, -900), Manager.Engine.ActorTypeFactory.Get("A-Wing") });

      // B-Wings x4
      spawns.Add(new object[] { new TV_3DVECTOR(-150, 50, -250), Manager.Engine.ActorTypeFactory.Get("B-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-290, 80, -280), Manager.Engine.ActorTypeFactory.Get("B-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-20, 100, -350), Manager.Engine.ActorTypeFactory.Get("B-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-200, 65, -400), Manager.Engine.ActorTypeFactory.Get("B-Wing") });

      // Y-Wings x6
      spawns.Add(new object[] { new TV_3DVECTOR(-10, 100, -350), Manager.Engine.ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(50, 80, -380), Manager.Engine.ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-40, 90, -420), Manager.Engine.ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-90, 100, -440), Manager.Engine.ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(75, 110, -450), Manager.Engine.ActorTypeFactory.Get("Y-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 85, -500), Manager.Engine.ActorTypeFactory.Get("Y-Wing") });

      // Corellian x9 (5 forward, 4 rear)
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, -120, 1300), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1400, -320, 400), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2400, 150, 1500), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 470, 850), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 300, 200), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(600, 70, -150), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-700, -150, -600), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-600, 250, -1200), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, 200, -1200), Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"), 5500 });

      // Mon Calamari x2 (not including HomeOne)
      spawns.Add(new object[] { new TV_3DVECTOR(4500, -120, -500), Manager.Engine.ActorTypeFactory.Get("Mon Calamari Capital Ship"), 7200 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 550, -1500), Manager.Engine.ActorTypeFactory.Get("Mon Calamari Capital Ship"), 7200 });

      // Nebulon B x3
      spawns.Add(new object[] { new TV_3DVECTOR(-2700, 320, -1850), Manager.Engine.ActorTypeFactory.Get("Nebulon-B Frigate"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1800, -280, -900), Manager.Engine.ActorTypeFactory.Get("Nebulon-B Frigate"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 850, -150), Manager.Engine.ActorTypeFactory.Get("Nebulon-B Frigate"), 8000 });

      // Transport x9
      spawns.Add(new object[] { new TV_3DVECTOR(1200, 550, -750), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1750, 350, 150), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, 280, -1200), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(4000, 280, -800), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-500, -175, -1600), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1650, -230, -800), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 450, 0), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, -280, -1200), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-900, 10, -400), Manager.Engine.ActorTypeFactory.Get("Transport"), 8000 });

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
                                 , new Move(new TV_3DVECTOR(position.x + Globals.Engine.Random.Next(-5, 5), position.y + Globals.Engine.Random.Next(-5, 5), -position.z - 1500)
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
        ActorInfo actor = Manager.Engine.ActorFactory.Get(id);
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
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          Manager.Engine.ActionManager.ForceClearQueue(actorID);
          Manager.Engine.ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                        , actor.MovementInfo.MaxSpeed
                                                        , actor.TypeInfo.Move_CloseEnough));
          Manager.Engine.ActionManager.QueueLast(actorID, new HyperspaceOut());
          Manager.Engine.ActionManager.QueueLast(actorID, new Delete());
        }
      }
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          Manager.Engine.ActionManager.ForceClearQueue(actorID);
          Manager.Engine.ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                , actor.MovementInfo.MaxSpeed
                                                , actor.TypeInfo.Move_CloseEnough));
          Manager.Engine.ActionManager.QueueLast(actorID, new HyperspaceOut());
          Manager.Engine.ActionManager.QueueLast(actorID, new Delete());
        }
      }
    }

    public void Rebel_MakePlayer(object[] param)
    {
      Globals.Engine.PlayerInfo.ActorID = Globals.Engine.PlayerInfo.TempActorID;
      
      if (Globals.Engine.PlayerInfo.Actor != null 
        && Globals.Engine.PlayerInfo.Actor.CreationState != CreationState.DISPOSED)
      { 
        // m_Player = Globals.Engine.Player.Actor;
        if (!Globals.Engine.GameScenarioManager.GetGameStateB("in_battle"))
        {
          foreach (int actorID in MainAllyFaction.GetShips())
          {
            ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
            if (actor != null)
            {
              actor.ActorState = ActorState.FREE;
              actor.MovementInfo.Speed = 275;
            }
          }

          foreach (int actorID in MainAllyFaction.GetWings())
          {
            ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
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
        if (Globals.Engine.PlayerInfo.Lives > 0)
        {
          Globals.Engine.PlayerInfo.Lives--;
          Globals.Engine.PlayerInfo.RequestSpawn = true;
        }
      }
    }

    public void Rebel_ShipsForward(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          Manager.Engine.ActionManager.ForceClearQueue(actorID);
          Manager.Engine.ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() - new TV_3DVECTOR(actor.GetPosition().x * 0.35f, 0, Math.Abs(actor.GetPosition().x) + 1500)
                                                    , actor.MovementInfo.MinSpeed));
          Manager.Engine.ActionManager.QueueLast(actorID, new Lock());
        }
      }
    }

    public void Rebel_ShipsForward_2(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          Manager.Engine.ActionManager.ForceClearQueue(actorID);
          Manager.Engine.ActionManager.QueueLast(actorID, new Move(actor.GetPosition() - new TV_3DVECTOR(actor.GetPosition().x * 0.35f, 0, Math.Abs(actor.GetPosition().x) + 1500)
                                                    , actor.MovementInfo.MaxSpeed));
          Manager.Engine.ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() - new TV_3DVECTOR(0, 0, 20000)
                                                    , actor.MovementInfo.MinSpeed));
          Manager.Engine.ActionManager.QueueLast(actorID, new Lock());
        }
      }
    }

    public void Rebel_YWingsAttackScan(object[] param)
    {
      if (MainEnemyFaction.GetShips().Count > 0)
      {
        foreach (int actorID in MainAllyFaction.GetWings())
        {
          ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
          if (actor != null)
          {
            if (actor.TypeInfo is YWingATI || actor.TypeInfo is BWingATI)
            {
              int rsID = MainEnemyFaction.GetShips()[Globals.Engine.Random.Next(0, MainEnemyFaction.GetShips().Count)];
              ActorInfo rs = Manager.Engine.ActorFactory.Get(actorID);
              {
                foreach (int i in rs.GetAllChildren(1))
                {
                  ActorInfo rc = Manager.Engine.ActorFactory.Get(i);
                  if (rc.RegenerationInfo.ParentRegenRate > 0)
                    if (Globals.Engine.Random.NextDouble() > 0.4f)
                      rsID = rc.ID;
                }
              }

              Manager.Engine.ActionManager.ClearQueue(actorID);
              Manager.Engine.ActionManager.QueueLast(actorID, new AttackActor(rsID, -1, -1, false));
            }
          }
        }
      }

      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5f, Rebel_YWingsAttackScan);
    }

    public void Rebel_DeathStarGo(object[] param)
    {
      ActorInfo falcon = Manager.Engine.ActorFactory.Get(m_FalconID);
      if (falcon != null)
      {
        falcon.CombatInfo.DamageModifier = 0;
        Manager.Engine.ActionManager.ForceClearQueue(m_FalconID);
        Manager.Engine.ActionManager.QueueLast(m_FalconID, new ForcedMove(new TV_3DVECTOR(0, 0, 20000), falcon.MovementInfo.MaxSpeed, -1));
        Manager.Engine.ActionManager.QueueLast(m_FalconID, new HyperspaceOut());
        Manager.Engine.ActionManager.QueueLast(m_FalconID, new Delete());
        m_FalconID = -1;
      }

      ActorInfo wedge = Manager.Engine.ActorFactory.Get(m_WedgeID);
      if (wedge != null)
      {
        wedge.CombatInfo.DamageModifier = 0;
        Manager.Engine.ActionManager.ForceClearQueue(m_WedgeID);
        Manager.Engine.ActionManager.QueueLast(m_WedgeID, new ForcedMove(new TV_3DVECTOR(0, 0, 20000), wedge.MovementInfo.MaxSpeed, -1));
        Manager.Engine.ActionManager.QueueLast(m_WedgeID, new HyperspaceOut());
        Manager.Engine.ActionManager.QueueLast(m_WedgeID, new Delete());
        m_WedgeID = -1;
      }
    }

    public void Rebel_GiveControl(object[] param)
    {
      ActorInfo falcon = Manager.Engine.ActorFactory.Get(m_FalconID);
      ActorInfo wedge = Manager.Engine.ActorFactory.Get(m_WedgeID);

      Manager.Engine.ActionManager.UnlockOne(m_FalconID);
      falcon.ActorState = ActorState.NORMAL;
      falcon.MovementInfo.Speed = falcon.MovementInfo.MaxSpeed;

      Manager.Engine.ActionManager.UnlockOne(m_WedgeID);
      Manager.Engine.ActionManager.QueueFirst(m_WedgeID, new Wait(2.5f));
      wedge.ActorState = ActorState.NORMAL;
      wedge.MovementInfo.Speed = wedge.MovementInfo.MaxSpeed;

      float time = 5f;
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);

        Manager.Engine.ActionManager.UnlockOne(actorID);
        Manager.Engine.ActionManager.QueueFirst(actorID, new Wait(time));
        actor.ActorState = ActorState.NORMAL;
        actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
        time += 2.5f;
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);

        Manager.Engine.ActionManager.UnlockOne(actorID);
        actor.ActorState = ActorState.NORMAL;
        actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
        actor.SetSpawnerEnable(true);
      }
      Globals.Engine.PlayerInfo.IsMovementControlsEnabled = true;

      Globals.Engine.GameScenarioManager.SetGameStateB("in_battle", true);
      Globals.Engine.GameScenarioManager.SetGameStateB("TIEs", true);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 10f, Empire_FirstTIEWave);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 45f, Empire_SecondTIEWave);
      Rebel_RemoveTorps(null);
    }

    private void Rebel_GoBack(float chance)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          double d = Globals.Engine.Random.NextDouble();
          if (d < chance)
          {
            ActorInfo homeone = Manager.Engine.ActorFactory.Get(m_HomeOneID);

            Manager.Engine.ActionManager.ClearQueue(actorID);
            Manager.Engine.ActionManager.QueueLast(actorID, new Move(homeone.GetPosition() + new TV_3DVECTOR(Globals.Engine.Random.Next(-2500, 2500), Globals.Engine.Random.Next(-50, 50), Globals.Engine.Random.Next(-2500, 2500))
                                                       , actor.MovementInfo.MaxSpeed));
          }
        }
      }
    }

    public void Rebel_RemoveTorps(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
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
      ActorInfo av = Manager.Engine.ActorFactory.Get(actorID);

      if (av != null 
        && av.CombatInfo.Strength < av.TypeInfo.MaxStrength * 0.8f 
        && MainAllyFaction.GetShips().Count > 0)
      {
        ActorInfo homeone = Manager.Engine.ActorFactory.Get(m_HomeOneID);

        Manager.Engine.ActionManager.ClearQueue(actorID);
        Manager.Engine.ActionManager.QueueLast(actorID
                              , new Move(homeone.GetPosition() + new TV_3DVECTOR(Globals.Engine.Random.Next(-2500, 2500)
                              , Globals.Engine.Random.Next(-50, 50)
                              , Globals.Engine.Random.Next(-2500, 2500))
                              , av.MovementInfo.MaxSpeed));
      }
    }

    public void Rebel_CriticalUnitDanger(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      int actorID = (int)param[0];
      ActorInfo av = Manager.Engine.ActorFactory.Get(actorID);

      if (av != null
        && av.StrengthFrac < 0.67f 
        && av.StrengthFrac >= 0.33f)
      {
        Globals.Engine.Screen2D.MessageText(string.Format("{0}: {1}, I need cover!", av.Name, Globals.Engine.PlayerInfo.Name)
                                            , 5
                                            , av.Faction.Color
                                            , 100);
      }
      else if (av.StrengthFrac < 0.33f)
      {
        Globals.Engine.Screen2D.MessageText(string.Format("{0}: {1}, Get those TIEs off me!", av.Name, Globals.Engine.PlayerInfo.Name)
                                            , 5
                                            , av.Faction.Color
                                            , 100);
      }
    }

    public void Rebel_CriticalUnitDying(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (Globals.Engine.GameScenarioManager.GetGameStateB("GameWon"))
        return;

      if (Globals.Engine.GameScenarioManager.GetGameStateB("GameOver"))
        return;

      int actorID = (int)param[0];
      ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);

      if (actor != null
        && (actor.ActorState == ActorState.DYING || actor.ActorState == ActorState.DEAD))
      {
        Globals.Engine.PlayerInfo.TempActorID = actorID;

        Globals.Engine.GameScenarioManager.SetGameStateB("GameOver", true);
        Globals.Engine.GameScenarioManager.IsCutsceneMode = true;

        if (Globals.Engine.GameScenarioManager.SceneCamera == null || !(Globals.Engine.GameScenarioManager.SceneCamera.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(Manager.Engine.ActorTypeFactory.Get("Death Camera"));
          camaci.CreationTime = Globals.Engine.Game.GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = actor.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(Manager.Engine.ActorFactory, camaci);
          Globals.Engine.PlayerInfo.ActorID = a.ID;
          a.CombatInfo.Strength = 0;
          Globals.Engine.PlayerInfo.TempActorID = actorID;

          a.CameraSystemInfo.CamDeathCirclePeriod = actor.CameraSystemInfo.CamDeathCirclePeriod;
          a.CameraSystemInfo.CamDeathCircleRadius = actor.CameraSystemInfo.CamDeathCircleRadius;
          a.CameraSystemInfo.CamDeathCircleHeight = actor.CameraSystemInfo.CamDeathCircleHeight;

          if (actor.ActorState == ActorState.DYING)
          {
            actor.TickEvents += Globals.Engine.GameScenarioManager.Scenario.ProcessPlayerDying;
            actor.DestroyedEvents += Globals.Engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
          }
          else
          {
            actor.DestroyedEvents += Globals.Engine.GameScenarioManager.Scenario.ProcessPlayerKilled;
          }

          if (actor.TypeInfo is WedgeXWingATI)
          {
            Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime, Message_90_LostWedge);
          }
          else if (actor.TypeInfo is LandoFalconATI)
          {
            Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime, Message_91_LostFalcon);
          }
          else if (actor.TypeInfo is MC90ATI)
          {
            Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15, Message_92_LostHomeOne);
            actor.CombatInfo.TimedLife = 2000f;
            Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 25, FadeOut);
          }
        }
        else
        {
          Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(actor.GetPosition().x, actor.GetPosition().y, actor.GetPosition().z);
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);
        float fz = Globals.Engine.Random.Next(-500, 500);

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
              Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
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
        float fx = Globals.Engine.Random.Next(-4500, 4500);
        float fy = Globals.Engine.Random.Next(-500, 500);
        float fz = Globals.Engine.Random.Next(-500, 500);

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
              Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

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
              Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MinBounds.z - 8000),
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
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { Manager.Engine.ActorTypeFactory.Get("TIE"), Manager.Engine.ActorTypeFactory.Get("TIE Interceptor") };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        int n = Globals.Engine.Random.Next(0, tietypes.Length);
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
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MinBounds.z - 8000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

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
              Type = Manager.Engine.ActorTypeFactory.Get("TIE Interceptor"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        ActionInfo[] actions = new ActionInfo[] { new Wait(8) };
        switch (Difficulty.ToLower())
        {
          case "mental":
            actions = new ActionInfo[] { new Wait(8), new Hunt(TargetType.FIGHTER) };
            break;
        }

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("TIE Defender"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Globals.Engine.GameScenarioManager.MinBounds.z - 6000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("TIE Interceptor"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("TIE Interceptor"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(Globals.Engine.PlayerInfo.Actor.ID, -1, -1, false) },
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MinBounds.z - 2500),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        ActorSpawnInfo asi = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(18)
                                     , new AttackActor(Globals.Engine.PlayerInfo.Actor.ID, -1, -1, false) },
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = Manager.Engine.ActorTypeFactory.Get("TIE Bomber"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MinBounds.z - 2500),
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
      float creationTime = Globals.Engine.Game.GameTime;

      ActorTypeInfo type;
      string name;

      List<object[]> spawns = new List<object[]>();
      spawns.Add(new object[] { new TV_3DVECTOR(0, 50, -33500), Manager.Engine.ActorTypeFactory.Get("Executor Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-1500, 180, -32500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(1500, 180, -32500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, 280, -27500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(5500, 280, -27500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-5500, -30, -29500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, -30, -29500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-7500, 130, -31000), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(7500, 130, -31000), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-11500, -80, -29000), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(11500, -80, -29000), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(-14500, 80, -27500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });
      spawns.Add(new object[] { new TV_3DVECTOR(14500, 80, -27500), Manager.Engine.ActorTypeFactory.Get("Imperial-I Static") });

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
        SpawnTime = Globals.Engine.Game.GameTime,
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
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-2500, -250, -19500), new TV_3DVECTOR(-2500, -250, -9500) });
          break;
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-750, -50, -19000), new TV_3DVECTOR(-750, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(750, -50, -19000), new TV_3DVECTOR(750, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-2250, -50, -19000), new TV_3DVECTOR(-1250, -50, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(2250, -50, -19000), new TV_3DVECTOR(1250, -50, -6500) });
          
          break;
        case "hard":
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 110, -17500), new TV_3DVECTOR(0, 110, -9000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(2500, -250, -19500), new TV_3DVECTOR(2000, 110, -9500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-2500, -250, -19500), new TV_3DVECTOR(-2000, 110, -9500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });
          break;
      }
      Globals.Engine.SoundManager.SetMusic("battle_3_2");
    }

    public void Empire_StarDestroyer_02(object[] param)
    {
      SDWaves++;

      m_Enemy_pull = 12000;
      switch (Difficulty.ToLower())
      {
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-1500, 150, -18500), new TV_3DVECTOR(-1000, 210, -7500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(1500, 150, -18500), new TV_3DVECTOR(1000, 210, -7500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-7500, 60, -24500), new TV_3DVECTOR(-2000, 60, -12500) });
          break;
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-10000, -300, -23500), new TV_3DVECTOR(-1250, -300, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Devastator Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 120, -24500), new TV_3DVECTOR(0, 120, -11500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-2500, 150, -24500), new TV_3DVECTOR(-2000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-3200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(10000, -300, -23500), new TV_3DVECTOR(1250, -300, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(2500, 150, -24500), new TV_3DVECTOR(2000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(3200, 50, -12000) });

          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-6500, 100, -25500), new TV_3DVECTOR(-2200, 50, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(6500, 100, -25500), new TV_3DVECTOR(2200, 50, -12000) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, -100, -24500), new TV_3DVECTOR(2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(2500, 150, -26500), new TV_3DVECTOR(500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(7500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(0, 60, -22500), new TV_3DVECTOR(0, 60, -12000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-1500, 150, -24500), new TV_3DVECTOR(-1000, 150, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(1500, 150, -24500), new TV_3DVECTOR(1000, 150, -10500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-2500, -100, -24500), new TV_3DVECTOR(-2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-500, 150, -26500), new TV_3DVECTOR(-500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(-4500, 150, -26500), new TV_3DVECTOR(-3000, 100, -10500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2500, -100, -24500), new TV_3DVECTOR(2000, -100, -12500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(500, 150, -26500), new TV_3DVECTOR(500, 100, -10500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Arquitens Light Cruiser"), new TV_3DVECTOR(4500, 150, -26500), new TV_3DVECTOR(3000, 100, -10500) });

          break;
      }
      
      Globals.Engine.SoundManager.SetMusic("battle_3_2");
    }

    public void Empire_Executor(object[] param)
    {
      //m_Enemy_pull = 4000;
      SDWaves++;
      Manager.Engine.ActorFactory.Get(m_ExecutorStaticID)?.Kill();

      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Globals.Engine.Game.GameTime;

      TV_3DVECTOR position = new TV_3DVECTOR(0, -950, -20000);
      ActorTypeInfo atinfo = Manager.Engine.ActorTypeFactory.Get("Executor Super Star Destroyer");

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = atinfo,
        Name = "",
        RegisterName = "",
        SidebarName = "EXECUTOR",
        SpawnTime = Globals.Engine.Game.GameTime,
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
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-550, 80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(550, -160, -7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-1500, 90, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(1500, 90, -6500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -80, -24000), new TV_3DVECTOR(-2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2000, -80, -24000), new TV_3DVECTOR(2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });

          break;
        case "hard":
          Empire_TIEWave_0D(new object[] { 1 });
          Empire_TIEWave_TIEsvsShips(new object[] { 5 });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-2500, 120, -6500) });

          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500) });

          break;
        case "normal":
        default:
          Empire_TIEWave_TIEsvsShips(new object[] { 10 });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Acclamator Assault Ship"), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500) });
          break;
      }
    }

    public void Empire_ExecutorDestroyed(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (Globals.Engine.GameScenarioManager.GetGameStateB("GameOver"))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];
      ActorState state = (ActorState)param[1];

      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        Globals.Engine.GameScenarioManager.SetGameStateB("GameWon", true);
        Globals.Engine.GameScenarioManager.IsCutsceneMode = true;

        if (Globals.Engine.GameScenarioManager.SceneCamera == null || !(Globals.Engine.GameScenarioManager.SceneCamera.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(Manager.Engine.ActorTypeFactory.Get("Death Camera"));
          camaci.CreationTime = Globals.Engine.Game.GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = ainfo.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(Manager.Engine.ActorFactory, camaci);
          Globals.Engine.PlayerInfo.ActorID = a.ID;
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

          Globals.Engine.SoundManager.SetMusic("executorend");
          ainfo.CombatInfo.TimedLife = 2000f;

          ActorInfo homeone = Manager.Engine.ActorFactory.Get(m_HomeOneID);
          if (homeone != null)
            homeone.CombatInfo.DamageModifier = 0;
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 55, FadeOut);
        }
        else
        {
          Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
        }
      }
    }

    public void Empire_DeathStarAttack(object[] param)
    {
      ActorInfo lsrSrc = Manager.Engine.ActorFactory.Get(m_ADSLaserSourceID);
      if (lsrSrc != null 
        && lsrSrc.CreationState == CreationState.ACTIVE)
      {
        List<int> victims = new List<int>(MainAllyFaction.GetShips());
        victims.Remove(m_HomeOneID);

        if (victims.Count > 0)
        {
          m_ADS_targetID = victims[Globals.Engine.Random.Next(0, victims.Count)];

          Manager.Engine.ActionManager.ForceClearQueue(m_ADSLaserSourceID);
          Manager.Engine.ActionManager.QueueNext(m_ADSLaserSourceID, new AttackActor(m_ADS_targetID));
          Manager.Engine.ActionManager.QueueNext(m_ADSLaserSourceID, new Lock());

          ActorInfo target = Manager.Engine.ActorFactory.Get(m_ADS_targetID);
          target.DestroyedEvents += DeathStarKill_Effect;
          Globals.Engine.GameScenarioManager.AddEvent(0.1f, Scene_DeathStarCam);
        }
      }
    }

    public void Empire_DeathStarAttack_01(object[] param)
    {
      Empire_DeathStarAttack(Manager.Engine.ActorTypeFactory.Get("Corellian Corvette"));
    }

    public void Empire_DeathStarAttack_02(object[] param)
    {
      Empire_DeathStarAttack(Manager.Engine.ActorTypeFactory.Get("Transport"));
    }

    public void Empire_DeathStarAttack_03(object[] param)
    {
      Empire_DeathStarAttack(Manager.Engine.ActorTypeFactory.Get("Mon Calamari Capital Ship"));
    }

    private void Empire_DeathStarAttack(ActorTypeInfo type)
    {
      ActorInfo lsrSrc = Manager.Engine.ActorFactory.Get(m_ADSLaserSourceID);
      if (lsrSrc != null
        && lsrSrc.CreationState == CreationState.ACTIVE)
      {
        List<int> tgt = new List<int>(MainAllyFaction.GetShips());
        tgt.Reverse();

        foreach (int tid in tgt)
        {
          ActorInfo t = Manager.Engine.ActorFactory.Get(tid);
          if (t != null 
            && t.TypeInfo == type
            && tid != m_HomeOneID)
          {
            m_ADS_targetID = tid;
            
            Manager.Engine.ActionManager.ForceClearQueue(m_ADSLaserSourceID);
            Manager.Engine.ActionManager.QueueNext(m_ADSLaserSourceID, new AttackActor(tid));
            Manager.Engine.ActionManager.QueueNext(m_ADSLaserSourceID, new Lock());

            t.DestroyedEvents += DeathStarKill_Effect;
            Globals.Engine.GameScenarioManager.AddEvent(0.1f, Scene_DeathStarCam);
            return;
          }
        }
      }
    }

    #endregion

    #region Text
    public void Message_01_AllWingsReport(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: All wings report in.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_02_RedLeader(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING (WEDGE): Red Leader standing by.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_03_GoldLeader(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("Y-WING: Gray Leader standing by.", 5, new TV_COLOR(0.6f, 0.6f, 0.6f, 1));
    }

    public void Message_04_BlueLeader(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("B-WING: Blue Leader standing by.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_05_GreenLeader(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("A-WING: Green Leader standing by.", 5, new TV_COLOR(0.4f, 0.8f, 0.4f, 1));
    }

    public void Message_06_Force(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): May the Force be with us.", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_07_Break(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: Break off the attack!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_08_Break(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: The shield's still up!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_09_Conf(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING (WEDGE): I get no reading, are you sure?", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_10_Break(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: All wings, pull up!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_11_Evasive(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): Take evasive action!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_12_Trap(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): It's a trap!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_13_Fighters(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING (WEDGE): Watch out for enemy fighters.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_14_Interceptors(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING (WEDGE): TIE Interceptors inbound.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_15_Bombers(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): We have bombers inbound. Keep them away from our cruisers!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_20_DeathStar(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: That blast came from the Death Star...", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_21_Close(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: Get us close to those Imperial Star Destroyers.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_22_PointBlank(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: Closer! Get us closer, and engage them at point blank range.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_23_Take(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: If they fire, we might even take a few of them with us.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_30_ShieldDown(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): The shield is down.", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_31_ResumeAttack(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): Commence your attack on the Death Star's main reactor", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_32_Han(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: I knew Han can do it!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_33_Han(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: Wedge, follow me.", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_34_Han(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING (WEDGE): Copy, Gold Leader.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_40_Focus(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): Focus all firepower on that Super Star Destroyer!", 5, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_90_LostWedge(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING (WEDGE): I can't hold them!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_91_LostFalcon(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MILLIENNIUM FALCON: I can't hold them!", 5, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_92_LostHomeOne(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("MON CALAMARI (HOME ONE): We have no chance...", 15, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    #endregion

    #region Scene
    public void Scene_EnterCutscene(object[] param)
    {
      ActorInfo falcon = Manager.Engine.ActorFactory.Get(m_FalconID);
      ActorInfo wedge = Manager.Engine.ActorFactory.Get(m_WedgeID);
      ActorInfo homeone = Manager.Engine.ActorFactory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.CombatInfo.DamageModifier = 0;

      if (wedge != null)
        wedge.CombatInfo.DamageModifier = 0;

      if (homeone != null)
        homeone.CombatInfo.DamageModifier = 0;

      m_PlayerID = Globals.Engine.PlayerInfo.ActorID;
      ActorInfo player = Manager.Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_pos = player.Position;
        Manager.Engine.ActionManager.QueueFirst(m_PlayerID, new Lock());
        player.Position = new TV_3DVECTOR(30, 0, -100000);
        m_Player_PrimaryWeapon = Globals.Engine.PlayerInfo.PrimaryWeapon;
        m_Player_SecondaryWeapon = Globals.Engine.PlayerInfo.SecondaryWeapon;
        player.CombatInfo.DamageModifier = 0;
      }

      Globals.Engine.PlayerInfo.ActorID = Globals.Engine.GameScenarioManager.SceneCamera.ID;
      Globals.Engine.GameScenarioManager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(object[] param)
    {
      ActorInfo falcon = Manager.Engine.ActorFactory.Get(m_FalconID);
      ActorInfo wedge = Manager.Engine.ActorFactory.Get(m_WedgeID);
      ActorInfo homeone = Manager.Engine.ActorFactory.Get(m_HomeOneID);

      if (falcon != null)
        falcon.CombatInfo.DamageModifier = 1;

      if (wedge != null)
        wedge.CombatInfo.DamageModifier = 1;

      if (homeone != null)
        homeone.CombatInfo.DamageModifier = 1;

      ActorInfo player = Manager.Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.ActorState = ActorState.NORMAL;
        player.Position = m_Player_pos;
        Manager.Engine.ActionManager.ForceClearQueue(m_PlayerID);
        Globals.Engine.PlayerInfo.ActorID = m_PlayerID;
        Globals.Engine.PlayerInfo.PrimaryWeapon = m_Player_PrimaryWeapon;
        Globals.Engine.PlayerInfo.SecondaryWeapon = m_Player_SecondaryWeapon;
        player.CombatInfo.DamageModifier = 1;
      }

      Globals.Engine.GameScenarioManager.IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam(object[] param)
    {
      ActorInfo target = Manager.Engine.ActorFactory.Get(m_ADS_targetID);

      if (target != null 
        && target.CreationState == CreationState.ACTIVE)
      {
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
        Globals.Engine.SoundManager.SetSound("ds_beam", false, 1, false);

        TV_3DVECTOR pos = target.GetPosition();
        TV_3DVECTOR rot = target.GetRotation();

        if (target.TypeInfo is CorellianATI)
         pos += new TV_3DVECTOR(150, 120, -2000); 
        else if (target.TypeInfo is TransportATI)
         pos += new TV_3DVECTOR(300, 200, 700); 
        else if (target.TypeInfo is MC90ATI)
         pos += new TV_3DVECTOR(-850, -400, 2500); 

        Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(pos.x, pos.y, pos.z);
        Globals.Engine.GameScenarioManager.SceneCamera.LookAtPoint(new TV_3DVECTOR());
        Globals.Engine.GameScenarioManager.SceneCamera.MovementInfo.Speed = 50;
        Globals.Engine.GameScenarioManager.SceneCamera.MovementInfo.MaxSpeed = 50;
        Globals.Engine.GameScenarioManager.CameraTargetActor = target;
        Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5f, Scene_ExitCutscene);
      }
    }

    public void DeathStarKill_Effect(object[] param)
    {
      PlayerCameraInfo.Instance().Shake = 150;
      Globals.Engine.SoundManager.SetSoundStop("ds_beam");
      Globals.Engine.SoundManager.SetSound("exp_nave", false, 1, false);
    }
    #endregion
  }
}
