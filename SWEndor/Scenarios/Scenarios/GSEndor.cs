using MTV3D65;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace SWEndor.Scenarios
{
  public class GSEndor : GameScenarioBase
  {
    public GSEndor()
    {
      Name = "Battle of Endor";
      AllowedWings = new List<ActorTypeInfo> { XWingATI.Instance()
                                               , YWingATI.Instance()
                                               , AWingATI.Instance()
                                               , BWingATI.Instance()
                                               /*
                                               , FalconATI.Instance()
                                               , CorellianATI.Instance()
                                               , MC90ATI.Instance()
                                               , NebulonBATI.Instance()
                                               , NebulonB2ATI.Instance()
                                               , TIE_D_ATI.Instance()
                                               , TIE_IN_ATI.Instance()
                                               , TIE_X1_ATI.Instance()
                                               , TIE_sa_ATI.Instance()
                                               , TIE_LN_ATI.Instance()
                                               , ImperialIATI.Instance()
                                               , DevastatorATI.Instance()
                                               , ExecutorATI.Instance()
                                               , ArquitensATI.Instance()
                                               , AcclamatorATI.Instance()
                                               */
                                              };

      AllowedDifficulties = new List<string> { "easy"
                                               , "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private ActorInfo m_AScene = null;
    private ActorInfo m_AEndor = null;
    private ActorInfo m_ADS = null;
    private ActorInfo m_ADSLaserSource = null;
    private ActorInfo m_ExecutorStatic = null;
    private BackgroundWorker bw_rebelcontrol = new BackgroundWorker();
    private List<object[]> m_pendingSDspawnlist = new List<object[]>();
    private int m_SDLeftForShieldDown = 0;

    private ActorInfo m_ADS_target = null;

    private ActorInfo m_Falcon = null;
    private TV_3DVECTOR m_Falcon_pos = new TV_3DVECTOR();

    private ActorInfo m_Wedge = null;
    private TV_3DVECTOR m_Wedge_pos = new TV_3DVECTOR();

    private ActorInfo m_Player = null;
    private TV_3DVECTOR m_Player_pos = new TV_3DVECTOR();
    private string m_Player_PrimaryWeapon = "";
    private string m_Player_SecondaryWeapon = "";

    private ActorInfo m_HomeOne = null;
    private float m_HomeOne_Strength = 5000;


    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Instance().Name = "Red Three";

      if (GameScenarioManager.Instance().GetGameStateB("in_game"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_game", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().CameraTargetPoint = new TV_3DVECTOR(0, 0, -100);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -10000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -10000);

      //if (wing is FighterGroup)
      //{
      PlayerInfo.Instance().Lives = 4;
        PlayerInfo.Instance().ScorePerLife = 500000;
        PlayerInfo.Instance().ScoreForNextLife = 500000;
      /*}
      else
      {
        PlayerInfo.Instance().Lives = 0;
        PlayerInfo.Instance().ScorePerLife = 99999999;
        PlayerInfo.Instance().ScoreForNextLife = 99999999;
      }*/


      PlayerInfo.Instance().Score = new ScoreInfo();

      switch (GameScenarioManager.Instance().Difficulty.ToLower())
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

      bw_rebelcontrol.DoWork += Rebel_DelayedGiveControl;

      MakePlayer = Rebel_MakePlayer;

      if (!GameScenarioManager.Instance().GetGameStateB("rebels_arrive"))
      {
        GameScenarioManager.Instance().SetGameStateB("rebels_arrive", true);

        SoundManager.Instance().SetMusic("battle_3_1");
        SoundManager.Instance().SetMusicLoop("battle_3_3");

        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_HyperspaceIn");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, "Rebel_MakePlayer");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 9f, "Message.01");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 12f, "Message.02");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13.5f, "Message.03");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, "Message.04");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 16.5f, "Message.05");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 19f, "Message.06");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 24f, "Message.07");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 25.2f, "Message.08");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 26.5f, "Message.09");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 29.5f, "Message.10");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 30f, "Rebel_GiveControl");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 38f, "Message.11");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 42f, "Message.12");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 50f, "Message.13");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 45f, "Empire_SpawnStatics");
      }

      GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "hard":
        case "mental":
          RebelFighterLimit = 150;
          break;
        case "easy":
          RebelFighterLimit = 200;
          break;
        case "normal":
        default:
          RebelFighterLimit = 175;
          break;
      }

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.AddFaction("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Rebels_Wedge", new TV_COLOR(0.8f, 0.4f, 0.4f, 1)).AutoAI = true;
      FactionInfo.AddFaction("Rebels_Falcon", new TV_COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire_Advanced", new TV_COLOR(0.4f, 0.8f, 0.4f, 1)).AutoAI = true;

      FactionInfo.Get("Rebels").Allies.Add(FactionInfo.Get("Rebels_Wedge"));
      FactionInfo.Get("Rebels").Allies.Add(FactionInfo.Get("Rebels_Falcon"));
      FactionInfo.Get("Rebels_Wedge").Allies.Add(FactionInfo.Get("Rebels"));
      FactionInfo.Get("Rebels_Wedge").Allies.Add(FactionInfo.Get("Rebels_Falcon"));
      FactionInfo.Get("Rebels_Falcon").Allies.Add(FactionInfo.Get("Rebels"));
      FactionInfo.Get("Rebels_Falcon").Allies.Add(FactionInfo.Get("Rebels_Wedge"));

      FactionInfo.Get("Empire").Allies.Add(FactionInfo.Get("Empire_Advanced"));
      FactionInfo.Get("Empire_Advanced").Allies.Add(FactionInfo.Get("Empire"));
    }

    public override void LoadScene()
    {
      base.LoadScene();

      ActorCreationInfo acinfo = null;

      // Create Room
      if (m_AScene == null)
      {
        acinfo = new ActorCreationInfo(SceneRoomATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        m_AScene = ActorInfo.Create(acinfo);
      }

      // Create Endor
      if (m_AEndor == null)
      {
        acinfo = new ActorCreationInfo(EndorATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, -1200, 0);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        acinfo.InitialScale = new TV_3DVECTOR(6, 6, 6);
        m_AEndor = ActorInfo.Create(acinfo);
      }

      // Create DeathStar
      if (m_ADS == null)
      {
        acinfo = new ActorCreationInfo(DeathStar2ATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, 800, 18000);
        acinfo.Rotation = new TV_3DVECTOR(0, 0, 5);
        m_ADS = ActorInfo.Create(acinfo);
        m_ADS.Faction = FactionInfo.Get("Empire");
      }
    }


    public override void RegisterEvents()
    {
      base.RegisterEvents();
      GameEvent.RegisterEvent("Rebel_HyperspaceIn", Rebel_HyperspaceIn);
      GameEvent.RegisterEvent("Rebel_HyperspaceOut", Rebel_HyperspaceOut);
      GameEvent.RegisterEvent("Rebel_MakePlayer", Rebel_MakePlayer);
      GameEvent.RegisterEvent("Rebel_GiveControl", Rebel_GiveControl);
      GameEvent.RegisterEvent("Rebel_DeathStarGo", Rebel_DeathStarGo);
      GameEvent.RegisterEvent("Rebel_ShipsForward", Rebel_ShipsForward);
      GameEvent.RegisterEvent("Rebel_ShipsForward_2", Rebel_ShipsForward_2);
      GameEvent.RegisterEvent("Rebel_YWingsAttackScan", Rebel_YWingsAttackScan);
      GameEvent.RegisterEvent("Rebel_MC80Spawner", Rebel_MC80Spawner);
      GameEvent.RegisterEvent("Rebel_CriticalUnitHit", Rebel_CriticalUnitHit);
      GameEvent.RegisterEvent("Rebel_CriticalUnitDanger", Rebel_CriticalUnitDanger);
      GameEvent.RegisterEvent("Rebel_CriticalUnitLost", Rebel_CriticalUnitLost);

      GameEvent.RegisterEvent("Empire_SpawnStatics", Empire_SpawnStatics);
      GameEvent.RegisterEvent("Empire_DeathStarAttack", Empire_DeathStarAttack);
      GameEvent.RegisterEvent("Empire_DeathStarAttack_01", Empire_DeathStarAttack_01);
      GameEvent.RegisterEvent("Empire_DeathStarAttack_02", Empire_DeathStarAttack_02);
      GameEvent.RegisterEvent("Empire_DeathStarAttack_03", Empire_DeathStarAttack_03);
      GameEvent.RegisterEvent("Empire_FirstTIEWave", Empire_FirstTIEWave);
      GameEvent.RegisterEvent("Empire_TIEWave_01", Empire_TIEWave_01);
      GameEvent.RegisterEvent("Empire_TIEWave_02", Empire_TIEWave_02);
      GameEvent.RegisterEvent("Empire_TIEWave_03", Empire_TIEWave_03);
      GameEvent.RegisterEvent("Empire_TIEWave_TIEsvsFalcon", Empire_TIEWave_TIEsvsFalcon);
      GameEvent.RegisterEvent("Empire_TIEWave_InterceptorsvsFalcon", Empire_TIEWave_InterceptorsvsFalcon);
      GameEvent.RegisterEvent("Empire_TIEWave_TIEsvsWedge", Empire_TIEWave_TIEsvsWedge);
      GameEvent.RegisterEvent("Empire_TIEWave_InterceptorsvsWedge", Empire_TIEWave_InterceptorsvsWedge);
      GameEvent.RegisterEvent("Empire_TIEWave_TIEsvsPlayer", Empire_TIEWave_TIEsvsPlayer);
      GameEvent.RegisterEvent("Empire_TIEWave_InterceptorsvsPlayer", Empire_TIEWave_InterceptorsvsPlayer);
      GameEvent.RegisterEvent("Empire_StarDestroyer_01", Empire_StarDestroyer_01);
      GameEvent.RegisterEvent("Empire_StarDestroyer_02", Empire_StarDestroyer_02);
      GameEvent.RegisterEvent("Empire_TIEBombers", Empire_TIEBombers);
      GameEvent.RegisterEvent("Empire_Executor", Empire_Executor);
      GameEvent.RegisterEvent("Empire_SDSpawner", Empire_SDSpawner);
      GameEvent.RegisterEvent("Empire_ExecutorDestroyed", Empire_ExecutorDestroyed);

      GameEvent.RegisterEvent("Scene_EnterCutscene", Scene_EnterCutscene);
      GameEvent.RegisterEvent("Scene_ExitCutscene", Scene_ExitCutscene);
      GameEvent.RegisterEvent("Scene_DeathStarCam", Scene_DeathStarCam);
      GameEvent.RegisterEvent("Scene_DeathStarKill_Effect", Scene_DeathStarKill_Effect);

      GameEvent.RegisterEvent("Message.01", Message_01_AllWingsReport);
      GameEvent.RegisterEvent("Message.02", Message_02_RedLeader);
      GameEvent.RegisterEvent("Message.03", Message_03_GoldLeader);
      GameEvent.RegisterEvent("Message.04", Message_04_BlueLeader);
      GameEvent.RegisterEvent("Message.05", Message_05_GreenLeader);
      GameEvent.RegisterEvent("Message.06", Message_06_Force);
      GameEvent.RegisterEvent("Message.07", Message_07_Break);
      GameEvent.RegisterEvent("Message.08", Message_08_Break);
      GameEvent.RegisterEvent("Message.09", Message_09_Conf);
      GameEvent.RegisterEvent("Message.10", Message_10_Break);
      GameEvent.RegisterEvent("Message.11", Message_11_Evasive);
      GameEvent.RegisterEvent("Message.12", Message_12_Trap);
      GameEvent.RegisterEvent("Message.13", Message_13_Fighters);
      GameEvent.RegisterEvent("Message.14", Message_14_Interceptors);
      GameEvent.RegisterEvent("Message.15", Message_15_Bombers);

      GameEvent.RegisterEvent("Message.20", Message_20_DeathStar);
      GameEvent.RegisterEvent("Message.21", Message_21_Close);
      GameEvent.RegisterEvent("Message.22", Message_22_PointBlank);
      GameEvent.RegisterEvent("Message.23", Message_23_Take);

      GameEvent.RegisterEvent("Message.30", Message_30_ShieldDown);
      GameEvent.RegisterEvent("Message.31", Message_31_ResumeAttack);
      GameEvent.RegisterEvent("Message.32", Message_32_Han);
      GameEvent.RegisterEvent("Message.33", Message_33_Han);
      GameEvent.RegisterEvent("Message.34", Message_34_Han);

      GameEvent.RegisterEvent("Message.40", Message_40_Focus);

      GameEvent.RegisterEvent("Message.90", Message_90_LostWedge);
      GameEvent.RegisterEvent("Message.91", Message_91_LostFalcon);
      GameEvent.RegisterEvent("Message.92", Message_92_LostHomeOne);
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      if (GameScenarioManager.Instance().GetGameStateB("in_battle"))
      {
        if (GameScenarioManager.Instance().StageNumber == 0)
        {
          GameScenarioManager.Instance().StageNumber = 1;
        }
        else if (GameScenarioManager.Instance().TIEWaves > 7 && GameScenarioManager.Instance().StageNumber == 1)
        //else if (GameScenarioManager.Instance().TIEWaves > 1 && GameScenarioManager.Instance().StageNumber == 1)
        {
          GameScenarioManager.Instance().StageNumber = 2;
        }
        else if (GameScenarioManager.Instance().SDWaves >= 1 && GameScenarioManager.Instance().EnemyShips.Count == 0 && m_pendingSDspawnlist.Count == 0 && GameScenarioManager.Instance().StageNumber == 2)
        {
          GameScenarioManager.Instance().StageNumber = 3;
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5.5f, "Message.14");
        }
        else if (GameScenarioManager.Instance().TIEWaves > 13 && GameScenarioManager.Instance().StageNumber == 3)
        //else if (GameScenarioManager.Instance().TIEWaves > 2 && GameScenarioManager.Instance().StageNumber == 3)
        {
          GameScenarioManager.Instance().StageNumber = 4;
        }
        else if (GameScenarioManager.Instance().SDWaves >= 2 && GameScenarioManager.Instance().EnemyShips.Count == 0 && m_pendingSDspawnlist.Count == 0 && GameScenarioManager.Instance().StageNumber == 4)
        {
          GameScenarioManager.Instance().StageNumber = 5;
          SoundManager.Instance().SetMusic("battle_3_4");
          SoundManager.Instance().SetMusicLoop("battle_3_4");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5.5f, "Message.15");
        }
        else if (GameScenarioManager.Instance().TIEWaves > 18 && GameScenarioManager.Instance().StageNumber == 5)
        //else if (GameScenarioManager.Instance().TIEWaves > 3 && GameScenarioManager.Instance().StageNumber == 5)
        {
          GameScenarioManager.Instance().StageNumber = 6;
        }

        // Wedge and Falcon
        if (!GameScenarioManager.Instance().GetGameStateB("deathstar_noshield")
          && GameScenarioManager.Instance().StageNumber == 4 
          && GameScenarioManager.Instance().SDWaves >= 2 
          && (m_pendingSDspawnlist.Count + GameScenarioManager.Instance().EnemyShips.Count) <= m_SDLeftForShieldDown)
        {
          GameScenarioManager.Instance().SetGameStateB("deathstar_noshield", true);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3f, "Message.30");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7f, "Message.31");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, "Rebel_DeathStarGo");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 16f, "Rebel_ShipsForward_2");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 12f, "Message.32");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 17f, "Message.33");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 20f, "Message.34");
        }

        // TIE spawn
        if (TIESpawnTime < Game.Instance().GameTime)
        {
          if ((GameScenarioManager.Instance().EnemyFighters.Count < 40 && GameScenarioManager.Instance().EnemyShips.Count == 0)
            || (GameScenarioManager.Instance().EnemyFighters.Count < 20 && GameScenarioManager.Instance().EnemyShips.Count > 0))
          {
            TIESpawnTime = Game.Instance().GameTime + 10f;

            if (GameScenarioManager.Instance().StageNumber == 1 || GameScenarioManager.Instance().StageNumber == 3 || GameScenarioManager.Instance().StageNumber == 5)
            {
              GameScenarioManager.Instance().TIEWaves++;
            }

            if (GameScenarioManager.Instance().StageNumber == 1 || GameScenarioManager.Instance().StageNumber == 2)
            {
              switch (GameScenarioManager.Instance().Difficulty.ToLower())
              {
                case "easy":
                  Empire_TIEWave_01(new object[] { 5 });
                  break;
                case "hard":
                  if (GameScenarioManager.Instance().TIEWaves % 3 == 1)
                  {
                    Empire_TIEWave_01(new object[] { 5 });
                    Empire_TIEWave_TIEsvsWedge(new object[] { 4 });
                    Empire_TIEWave_TIEsvsFalcon(new object[] { 4 });
                  }
                  else
                  {
                    Empire_TIEWave_01(new object[] { 7 });
                  }
                  break;
                case "mental":
                  if (GameScenarioManager.Instance().TIEWaves % 2 == 1)
                  {
                    Empire_TIEWave_01(new object[] { 6 });
                    Empire_TIEWave_TIEsvsWedge(new object[] { 6 });
                    Empire_TIEWave_TIEsvsFalcon(new object[] { 6 });
                    Empire_TIEWave_TIEsvsPlayer(new object[] { 4 });
                  }
                  else
                  {
                    Empire_TIEWave_01(new object[] { 9 });
                  }
                  break;
                case "normal":
                default:
                  Empire_TIEWave_01(new object[] { 6 });
                  break;
              }
            }
            else if (GameScenarioManager.Instance().StageNumber == 3 || GameScenarioManager.Instance().StageNumber == 4)
            {
              switch (GameScenarioManager.Instance().Difficulty.ToLower())
              {
                case "easy":
                  Empire_TIEWave_02(new object[] { 5 });
                  break;
                case "hard":
                  if (GameScenarioManager.Instance().TIEWaves % 3 == 1)
                  {
                    Empire_TIEWave_02(new object[] { 6 });
                    Empire_TIEWave_InterceptorsvsWedge(new object[] { 2 });
                    Empire_TIEWave_InterceptorsvsFalcon(new object[] { 2 });
                  }
                  else
                  {
                    Empire_TIEWave_02(new object[] { 7 });
                  }

                  break;
                case "mental":
                  if (GameScenarioManager.Instance().TIEWaves % 2 == 1)
                  {
                    Empire_TIEWave_02(new object[] { 7 });
                    Empire_TIEWave_InterceptorsvsWedge(new object[] { 4 });
                    Empire_TIEWave_InterceptorsvsFalcon(new object[] { 4 });
                    Empire_TIEWave_InterceptorsvsPlayer(new object[] { 4 });
                  }
                  else
                  {
                    Empire_TIEWave_02(new object[] { 8 });
                  }
                  break;
                case "normal":
                default:
                  Empire_TIEWave_02(new object[] { 6 });
                  break;
              }
            }
            else if (GameScenarioManager.Instance().StageNumber == 5 || GameScenarioManager.Instance().StageNumber == 6)
            {
              switch (GameScenarioManager.Instance().Difficulty.ToLower())
              {
                case "easy":
                  Empire_TIEWave_01(new object[] { 3 });
                  Empire_TIEWave_03(new object[] { 3 });
                  Empire_TIEBombers(new object[] { 1 });
                  break;
                case "hard":
                  if (GameScenarioManager.Instance().TIEWaves % 3 == 1)
                  {
                    Empire_TIEWave_02(new object[] { 3 });
                    Empire_TIEWave_03(new object[] { 4 });
                    Empire_TIEBombers(new object[] { 3 });
                    Empire_TIEWave_TIEsvsPlayer(new object[] { 2 });
                    Empire_TIEWave_InterceptorsvsPlayer(new object[] { 2 });
                  }
                  else
                  {
                    Empire_TIEWave_02(new object[] { 3 });
                    Empire_TIEWave_03(new object[] { 5 });
                    Empire_TIEBombers(new object[] { 3 });
                  }
                  break;
                case "mental":
                  if (GameScenarioManager.Instance().TIEWaves % 2 == 1)
                  {
                    Empire_TIEWave_03(new object[] { 8 });
                    Empire_TIEBombers(new object[] { 4 });
                    Empire_TIEWave_InterceptorsvsPlayer(new object[] { 4 });
                  }
                  else
                  {
                    Empire_TIEWave_03(new object[] { 9 });
                    Empire_TIEBombers(new object[] { 4 });
                  }
                  break;
                case "normal":
                default:
                  Empire_TIEWave_02(new object[] { 3 });
                  Empire_TIEWave_03(new object[] { 4 });
                  Empire_TIEBombers(new object[] { 2 });
                  break;
              }
            }
          }
        }

        if (m_pendingSDspawnlist.Count > 0 && GameScenarioManager.Instance().EnemyShips.Count < 8)
        {
          if (m_pendingSDspawnlist[0].Length > 0 
            && (!(m_pendingSDspawnlist[0][0] is ImperialIATI) || GameScenarioManager.Instance().EnemyShips.Count < ((GameScenarioManager.Instance().StageNumber == 6) ? 3 : 2)) 
            && (!(m_pendingSDspawnlist[0][0] is DevastatorATI) || GameScenarioManager.Instance().EnemyShips.Count < 2))
          {
            Empire_StarDestroyer_Spawn(m_pendingSDspawnlist[0]);
            m_pendingSDspawnlist.RemoveAt(0);
          }
        }

        //Rebel_ForceAwayFromBounds(null);

        if (GameScenarioManager.Instance().StageNumber == 2 && GameScenarioManager.Instance().prevStageNumber < 2)
        {
          //RebelFighterLimit += 5;
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, "Empire_DeathStarAttack_01");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, "Message.20");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, "Empire_StarDestroyer_01");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 20f, "Message.21");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 25f, "Rebel_YWingsAttackScan");
          GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -17500);
          GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -17500);
        }
        else if (GameScenarioManager.Instance().StageNumber == 4 && GameScenarioManager.Instance().prevStageNumber < 4)
        {
          //RebelFighterLimit += 5;
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, "Empire_DeathStarAttack_02");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 16f, "Rebel_ShipsForward");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13f, "Message.22");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, "Empire_StarDestroyer_02");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 18f, "Message.23");
          GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -22500);
          GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -22500);
        }
        else if (GameScenarioManager.Instance().StageNumber == 6 && GameScenarioManager.Instance().prevStageNumber < 6)
        {
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, "Empire_DeathStarAttack_03");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, "Empire_Executor");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13f, "Message.40");
          //RebelFighterLimit += 5;
          GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-20000, -1500, -25000);
          GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
          GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-20000, -1500, -25000);
        }
      }

      if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().GameTime || Game.Instance().GameTime % 0.4f > 0.2f)
      {
        GameScenarioManager.Instance().Line1Text = string.Format("WINGS: {0}", GameScenarioManager.Instance().Scenario.RebelFighterLimit);
      }
      else
      {
        GameScenarioManager.Instance().Line1Text = string.Format("");
      }

      if (GameScenarioManager.Instance().Scenario.TimeSinceLostShip < Game.Instance().GameTime || Game.Instance().GameTime % 0.4f > 0.2f)
      {
        GameScenarioManager.Instance().Line2Text = string.Format("SHIPS: {0}", GameScenarioManager.Instance().AllyShips.Count);
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
      if (m_AScene != null && m_AScene.CreationState == CreationState.ACTIVE)
      {
        m_AScene.SetLocalPosition(PlayerInfo.Instance().Position.x, PlayerInfo.Instance().Position.y, PlayerInfo.Instance().Position.z);
      }

    }


    #region Rebellion spawns

    private void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      ActionInfo[] actions;
      Dictionary<string, ActorInfo>[] registries;
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
      faction = FactionInfo.Get("Rebels_Falcon");
      position = new TV_3DVECTOR(0, -10, 350);
      actions = new ActionInfo[] { new Actions.HyperspaceIn(position)
                                 , new Actions.Lock()
                                 , new Actions.Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

      registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyFighters
                                                                                       , GameScenarioManager.Instance().CriticalAllies,
                                                                                       };

      ainfo = SpawnActor(type
                       , name
                       , name
                       , sidebar_name
                       , creationTime
                       , faction
                       , position + hyperspaceInOffset
                       , rotation
                       , actions
                       , registries);

      ainfo.HitEvents.Add("Rebel_CriticalUnitHit");
      ainfo.HitEvents.Add("Rebel_CriticalUnitDanger");
      ainfo.ActorStateChangeEvents.Add("Rebel_CriticalUnitLost");
      m_Falcon = ainfo;

      // Wedge X-Wing
      type = WedgeXWingATI.Instance();
      name = "X-Wing (Wedge)";
      sidebar_name = "WEDGE";
      creationTime += creationDelay;
      faction = FactionInfo.Get("Rebels_Wedge");
      position = new TV_3DVECTOR(70, 20, 250);
      actions = new ActionInfo[] { new Actions.HyperspaceIn(position)
                                 , new Actions.Lock()
                                 , new Actions.Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

      registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyFighters
                                                                                       , GameScenarioManager.Instance().CriticalAllies,
                                                                                       };

      ainfo = SpawnActor(type
                       , name
                       , name
                       , sidebar_name
                       , creationTime
                       , faction
                       , position + hyperspaceInOffset
                       , rotation
                       , actions
                       , registries);

      ainfo.HitEvents.Add("Rebel_CriticalUnitHit");
      ainfo.HitEvents.Add("Rebel_CriticalUnitDanger");
      ainfo.ActorStateChangeEvents.Add("Rebel_CriticalUnitLost");
      m_Wedge = ainfo;

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
        position = new TV_3DVECTOR(100, 20, -850);
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
      faction = FactionInfo.Get("Rebels");
      actions = new ActionInfo[] { new Actions.HyperspaceIn(position)
                                 , new Actions.Lock()
                                 , new Actions.Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

      if (PlayerInfo.Instance().ActorType is FighterGroup || PlayerInfo.Instance().ActorType is TIEGroup)
      {
        registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyFighters };
      }
      else
      {
        registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyShips };
      }

      ainfo = SpawnActor(type
                       , name
                       , name
                       , sidebar_name
                       , creationTime
                       , faction
                       , position + hyperspaceInOffset
                       , rotation
                       , actions
                       , registries);

      GameScenarioManager.Instance().CameraTargetActor = ainfo;

      // Mon Calamari (HomeOne)
      type = MC90ATI.Instance();
      name = "Mon Calamari (Home One)";
      sidebar_name = "HOME ONE";
      //creationTime += creationDelay;
      faction = FactionInfo.Get("Rebels");
      position = new TV_3DVECTOR(1000, -300, 1000);
      TV_3DVECTOR nv = new TV_3DVECTOR(position.x + ((position.x > 0) ? 5 : -5), position.y, -position.z - 3000);
      actions = new ActionInfo[] { new Actions.HyperspaceIn(position)
                                 , new Actions.Lock()
                                 , new Actions.Move(nv
                                                  , type.MaxSpeed
                                                  , type.Move_CloseEnough
                                                  , false)
                                 , new Actions.Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                    , type.MinSpeed
                                                    , type.Move_CloseEnough
                                                    , false)
                                 , new Actions.Lock()
                                 };

      registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyShips
                                                       , GameScenarioManager.Instance().CriticalAllies,
                                                       };

      ainfo = SpawnActor(type
                       , name
                       , name
                       , sidebar_name
                       , Game.Instance().GameTime + 1.15f
                       , faction
                       , position + hyperspaceInOffset
                       , rotation
                       , actions
                       , registries);

      ainfo.ActorStateChangeEvents.Add("Rebel_CriticalUnitLost");
      ainfo.TickEvents.Add("Rebel_MC80Spawner");
      m_HomeOne = ainfo;


      // Other units x6
      sidebar_name = "";
      faction = FactionInfo.Get("Rebels");
      float nvd = 6000;
      string registername = "";
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

      // Corellian x7 (5 forward, 2 rear)
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, -120, 1300), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1400, -320, 400), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2400, 150, 1500), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 470, 850), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(100, 300, 200), CorellianATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(600, 70, -150), CorellianATI.Instance(), 5500 });
      spawns.Add(new object[] { new TV_3DVECTOR(-700, -150, -600), CorellianATI.Instance(), 5500 });

      // Mon Calamari x2 (not including HomeOne)
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 350, -1500), MC90ATI.Instance(), 7200 });
      spawns.Add(new object[] { new TV_3DVECTOR(4500, -120, -500), MC90ATI.Instance(), 7200 });

      // Nebulon B x3
      spawns.Add(new object[] { new TV_3DVECTOR(-2700, 250, -1850), NebulonBATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1800, -280, -900), NebulonBATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(2500, 850, -150), NebulonBATI.Instance(), 8000 });

      // Transport x9
      spawns.Add(new object[] { new TV_3DVECTOR(1200, 550, -750), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(1750, 350, 150), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(3500, 280, -1200), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(4000, 280, -800), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-500, -175, -1600), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-900, 10, -400), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-1650, -230, -800), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-2000, 450, 0), TransportATI.Instance(), 8000 });
      spawns.Add(new object[] { new TV_3DVECTOR(-3500, -280, -1200), TransportATI.Instance(), 8000 });

      foreach (object[] spawn in spawns)
      {
        type = (ActorTypeInfo)spawn[1];
        name = type.Name;
        float regi = 0;
        while (GameScenarioManager.Instance().AllyFighters.ContainsKey(name + " " + regi) 
            || GameScenarioManager.Instance().AllyShips.ContainsKey(name + " " + regi))
        {
          regi++;
        }
        registername = name + " " + regi;
        creationTime += creationDelay;
        position = (TV_3DVECTOR)spawn[0];
        if (type is FighterGroup)
        {
          actions = new ActionInfo[] { new Actions.HyperspaceIn(position)
                                 , new Actions.Lock()
                                 , new Actions.Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

          registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyFighters };
        }
        else
        {
          if (spawn.Length < 3 || !(spawn[2] is float))
            nvd = 6000;
          else
            nvd = (float)spawn[2];

          nv = new TV_3DVECTOR(position.x + ((position.x > 0) ? 5 : -5), position.y, -position.z - nvd);
          actions = new ActionInfo[] { new Actions.HyperspaceIn(position)
                                 , new Actions.Lock()
                                 , new Actions.Move(nv
                                                  , type.MaxSpeed
                                                  , type.Move_CloseEnough
                                                  , false)
                                 , new Actions.Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                    , type.MinSpeed
                                                    , type.Move_CloseEnough
                                                    , false)
                                 , new Actions.Lock()
                                 };

          registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyShips };
        }

        ainfo = SpawnActor(type
                         , name
                         , registername
                         , sidebar_name
                         , creationTime
                         , faction
                         , position + hyperspaceInOffset
                         , rotation
                         , actions
                         , registries);

        if (ainfo.TypeInfo is MC90ATI)
        {
          ainfo.TickEvents.Add("Rebel_MC80Spawner");
        }
      }
    }

    private void Rebel_HyperspaceOut(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        ActionManager.ForceClearQueue(a);
        ActionManager.QueueLast(a, new Actions.Rotate(a.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                                      , a.MaxSpeed
                                                      , a.TypeInfo.Move_CloseEnough));
        ActionManager.QueueLast(a, new Actions.HyperspaceOut());
        ActionManager.QueueLast(a, new Actions.Delete());
      }
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
      {
        ActionManager.ForceClearQueue(a);
        ActionManager.QueueLast(a, new Actions.Rotate(a.GetPosition() + new TV_3DVECTOR(500, 0, -20000)
                                              , a.MaxSpeed
                                              , a.TypeInfo.Move_CloseEnough));
        ActionManager.QueueLast(a, new Actions.HyperspaceOut());
        ActionManager.QueueLast(a, new Actions.Delete());
      }
    }

    public void Rebel_MakePlayer(object[] param)
    {
      if (GameScenarioManager.Instance().AllyFighters.ContainsKey("(Player)"))
      {
        PlayerInfo.Instance().Actor = GameScenarioManager.Instance().AllyFighters["(Player)"];
      }
      else if (GameScenarioManager.Instance().AllyShips.ContainsKey("(Player)"))
      {
        PlayerInfo.Instance().Actor = GameScenarioManager.Instance().AllyShips["(Player)"];
      }
      else
      {
        PlayerInfo.Instance().Actor = null;
      }
      
      if (PlayerInfo.Instance().Actor != null)
      { 
        m_Player = PlayerInfo.Instance().Actor;

        if (!GameScenarioManager.Instance().GetGameStateB("in_battle"))
        {
          foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
          {
            a.ActorState = ActorState.FREE;
            a.Speed = 275;
          }

          foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
          {
            a.ActorState = ActorState.FREE;
            if (a.Speed < 425)
              a.Speed = 425;
          }
        }
      }
      else if (m_HomeOne != null)
      {
        if (PlayerInfo.Instance().Lives > 0)
        {
          PlayerInfo.Instance().Lives--;
          m_HomeOne.FireWeapon(null, "Playerspawn");
          if (PlayerInfo.Instance().Actor != null)
          {
            GameScenarioManager.Instance().AllyFighters.Add("(Player)", PlayerInfo.Instance().Actor);
            m_Player = PlayerInfo.Instance().Actor;
          }
        }
      }
    }

    public void Rebel_ShipsForward(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
      {
        ActionManager.ForceClearQueue(a);
        ActionManager.QueueLast(a, new Actions.Rotate(a.GetPosition() - new TV_3DVECTOR(a.GetPosition().x * 0.35f, 0, Math.Abs(a.GetPosition().x) + 1500)
                                                  , a.MinSpeed));
        ActionManager.QueueLast(a, new Actions.Lock());
      }
    }

    public void Rebel_ShipsForward_2(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
      {
        ActionManager.ForceClearQueue(a);
        ActionManager.QueueLast(a, new Actions.Move(a.GetPosition() - new TV_3DVECTOR(a.GetPosition().x * 0.35f, 0, Math.Abs(a.GetPosition().x) + 1500)
                                                  , a.MaxSpeed));
        ActionManager.QueueLast(a, new Actions.Rotate(a.GetPosition() - new TV_3DVECTOR(0, 0, 20000)
                                                  , a.MinSpeed));
        ActionManager.QueueLast(a, new Actions.Lock());
      }
    }

    public void Rebel_YWingsAttackScan(object[] param)
    {
      if (GameScenarioManager.Instance().EnemyShips.Count > 0)
      {
        foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
        {
          if (ainfo.TypeInfo is YWingATI || ainfo.TypeInfo is BWingATI)
          {
            //if ((ainfo.AI.CurrentOrder.AIType == AIType.ATTACK_ACTOR || ainfo.AI.CurrentOrder.AIType == AIType.IDLE))
            //{
            string[] rskeys = new string[GameScenarioManager.Instance().EnemyShips.Count];
            GameScenarioManager.Instance().EnemyShips.Keys.CopyTo(rskeys, 0);
            ActorInfo rs = GameScenarioManager.Instance().EnemyShips[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

            foreach (ActorInfo rc in rs.GetAllChildren(1))
            {
              if (rc.TypeInfo is SDShieldGeneratorATI)
                if (Engine.Instance().Random.NextDouble() > 0.4f)
                  rs = rc;
            }

            ActionManager.ClearQueue(ainfo);
            ActionManager.QueueLast(ainfo, new Actions.AttackActor(rs, -1, -1, false));
          }
        }
      }

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Rebel_YWingsAttackScan");
    }

    public void Rebel_DeathStarGo(object[] param)
    {
      if (m_Falcon != null)
      {
        ActionManager.ForceClearQueue(m_Falcon);
        ActionManager.QueueLast(m_Falcon, new Actions.ForcedMove(new TV_3DVECTOR(0, 0, 20000), m_Falcon.MaxSpeed, -1));
        ActionManager.QueueLast(m_Falcon, new Actions.HyperspaceOut());
        ActionManager.QueueLast(m_Falcon, new Actions.Delete());
        m_Falcon = null;
      }

      if (m_Wedge != null)
      {
        ActionManager.ForceClearQueue(m_Wedge);
        ActionManager.QueueLast(m_Wedge, new Actions.ForcedMove(new TV_3DVECTOR(0, 0, 20000), m_Wedge.MaxSpeed, -1));
        ActionManager.QueueLast(m_Wedge, new Actions.HyperspaceOut());
        ActionManager.QueueLast(m_Wedge, new Actions.Delete());
        m_Wedge = null;
      }
    }

    public void Rebel_GiveControl(object[] param)
    {
      bw_rebelcontrol.RunWorkerAsync();

      foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
      {
        ActionManager.Unlock(a);
        a.ActorState = ActorState.NORMAL;
        a.Speed = a.MaxSpeed;
      }
      //PlayerInfo.Instance().PlayerAIEnabled = true;
      PlayerInfo.Instance().IsMovementControlsEnabled = true;

      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
      GameScenarioManager.Instance().SetGameStateB("TIEs", true);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Empire_FirstTIEWave");
      Rebel_RemoveTorps(null);
    }

    private void Rebel_DelayedGiveControl(object sender, DoWorkEventArgs e)
    {
      List<ActorInfo> list = new List<ActorInfo>(GameScenarioManager.Instance().AllyFighters.Values);
      foreach (ActorInfo a in list)
      {
        ActionManager.Unlock(a);
        a.ActorState = ActorState.NORMAL;
        a.Speed = a.MaxSpeed;
        Thread.Sleep(250);
      }
    }

    public void Rebel_RemoveTorps(object[] param)
    {
      foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
      {
        if (!ainfo.IsPlayer())
        { 
          foreach (KeyValuePair<string, WeaponInfo> kvp in ainfo.Weapons)
          {
            if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
            {
              kvp.Value.Ammo = 3;
              kvp.Value.MaxAmmo = 3;
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
        foreach (KeyValuePair<string, WeaponInfo> kvp in ainfo.Weapons)
        {
          if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
          {
            kvp.Value.Ammo = 3;
            kvp.Value.MaxAmmo = 3;
          }
        }
      }
    }

    public void Rebel_ForceAwayFromBounds(object[] param)
    {
      List<ActorInfo> list = new List<ActorInfo>(GameScenarioManager.Instance().AllyFighters.Values);
      foreach (ActorInfo a in list)
      {
        if (a.IsNearlyOutOfBounds(2500, 100, 2500))
        {
          ActionManager.ClearQueue(a);
          //a.AI.Orders.Clear();
          //a.AI.CurrentOrder.Complete = true;

          float x = Engine.Instance().Random.Next((int)(GameScenarioManager.Instance().MinBounds.x * 0.5f), (int)(GameScenarioManager.Instance().MaxBounds.x * 0.5f));
          float y = Engine.Instance().Random.Next(-200, 200);
          float z = Engine.Instance().Random.Next((int)(GameScenarioManager.Instance().MinBounds.z * 0.5f), (int)(GameScenarioManager.Instance().MaxBounds.z * 0.5f));

          ActionManager.QueueLast(a, new Actions.Move(new TV_3DVECTOR(x, y, z), a.MaxSpeed));

          //a.AI.Orders.Enqueue(new AIElement { AIType = AIType.ROTATE, TargetPosition = new TV_3DVECTOR(x, y, z) });
          //a.AI.Orders.Enqueue(new AIElement { AIType = AIType.MOVE, TargetPosition = new TV_3DVECTOR(x, y, z) });
        }
      }
    }

    public void Rebel_MC80Spawner(object[] param)
    {
      if (param.GetLength(0) < 1 || param[0] == null)
        return;

      ActorInfo ainfo = (ActorInfo)param[0];

      // spawner deployment logic
      if (ainfo.ActorState != ActorState.DEAD
          && ainfo.ActorState != ActorState.DYING
          && ainfo.IsStateFDefined("WingspawnCooldown")
          && ainfo.GetStateF("WingspawnCooldown") < Game.Instance().GameTime
          && GameScenarioManager.Instance().AllyFighters.Count < 25
          && GameScenarioManager.Instance().AllyFighters.Count < GameScenarioManager.Instance().Scenario.RebelFighterLimit)
      {
        if (ainfo.TypeInfo.FireWeapon(ainfo, null, "Wingspawn"))
        {
          foreach (ActorInfo a in ainfo.GetAllChildren(1))
          {
            if (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup)
            {
              Rebel_RemoveTorpsSingle(new object[] { a });
              if (!GameScenarioManager.Instance().AllyFighters.ContainsKey(a.Key))
              {
                GameScenarioManager.Instance().AllyFighters.Add(a.Key, a);
                RegisterEvents(a);
              }
            }
          }
        }
      }
    }

    public void Rebel_CriticalUnitHit(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      ActorInfo av = (ActorInfo)param[0];

      if (!av.GetStateB("IgnoreHit") && av.Strength < av.TypeInfo.MaxStrength * 0.8f && GameScenarioManager.Instance().AllyShips.Count > 0)
      {
        int shp = Engine.Instance().Random.Next(0, GameScenarioManager.Instance().AllyShips.Count - 1);
        ActorInfo[] ais = new ActorInfo[GameScenarioManager.Instance().AllyShips.Count];
        GameScenarioManager.Instance().AllyShips.Values.CopyTo(ais, 0);

        ActionManager.ClearQueue(av);
        ActionManager.QueueLast(av, new Actions.Move(ais[shp].GetPosition() + new TV_3DVECTOR(Engine.Instance().Random.Next(-2500, 2500), Engine.Instance().Random.Next(-50, 50), Engine.Instance().Random.Next(-2500, 2500))
                                                   , av.MaxSpeed));
      }
    }

    public void Rebel_CriticalUnitDanger(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      ActorInfo av = (ActorInfo)param[0];

      if (av.StrengthFrac < 0.67f && av.StrengthFrac >= 0.33f)
      {
        Screen2D.Instance().UpdateText(string.Format("{0}: {1}, I need cover!", av.Name, PlayerInfo.Instance().Name)
                                            , Game.Instance().GameTime + 5f
                                            , av.Faction.Color);
      }
      else if (av.StrengthFrac < 0.33f)
      {
        Screen2D.Instance().UpdateText(string.Format("{0}: {1}, Get those TIEs off me!", av.Name, PlayerInfo.Instance().Name)
                                            , Game.Instance().GameTime + 5f
                                            , av.Faction.Color);
      }
    }

    public void Rebel_CriticalUnitLost(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (GameScenarioManager.Instance().GetGameStateB("GameWon"))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];
      ActorState state = (ActorState)param[1];

      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        GameScenarioManager.Instance().SetGameStateB("GameOver", true);
        GameScenarioManager.Instance().IsCutsceneMode = true;

        if (GameScenarioManager.Instance().SceneCamera == null || !(GameScenarioManager.Instance().SceneCamera.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(DeathCameraATI.Instance());
          camaci.CreationTime = Game.Instance().GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = ainfo.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(camaci);
          PlayerInfo.Instance().Actor = a;
          PlayerInfo.Instance().Actor.Strength = 0;

          a.CamDeathCirclePeriod = ainfo.CamDeathCirclePeriod;
          a.CamDeathCircleRadius = ainfo.CamDeathCircleRadius;
          a.CamDeathCircleHeight = ainfo.CamDeathCircleHeight;

          if (ainfo.ActorState == ActorState.DYING)
          {
            ainfo.TickEvents.Add("Common_ProcessPlayerDying");
            ainfo.DestroyedEvents.Add("Common_ProcessPlayerKilled");
          }
          else
          {
            GameScenarioManager.Instance().Scenario.ProcessPlayerKilled(new object[] { ainfo });
          }

          if (ainfo.TypeInfo is WedgeXWingATI)
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, "Message.90");
          }
          else if (ainfo.TypeInfo is LandoFalconATI)
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, "Message.91");
          }
          else if (ainfo.TypeInfo is MC90ATI)
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15, "Message.92");
            ainfo.TimedLife = 2000f;
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 25, "Common_FadeOut");
          }
        }
        else
        {
          GameScenarioManager.Instance().SceneCamera.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
        }
      }
    }

    #endregion


    #region Empire spawns
    public void Empire_FirstTIEWave(object[] param)
    {
      if (m_ADS.GetAllChildren(1).Count > 0)
        m_ADSLaserSource = m_ADS.GetAllChildren(1)[0];

      // TIEs
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < 15; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);
        float fz = Engine.Instance().Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            aci = new ActorCreationInfo(TIE_LN_ATI.Instance());
            aci.CreationTime = Game.Instance().GameTime + t;
            aci.Faction = FactionInfo.Get("Empire");
            aci.InitialState = ActorState.NORMAL;
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, fz - 8000);
            aci.Rotation = new TV_3DVECTOR();

            ActorInfo a = ActorInfo.Create(aci);

            switch (GameScenarioManager.Instance().Difficulty.ToLower())
            {
              case "mental":
              case "hard":
                if (GameScenarioManager.Instance().AllyFighters.Count > 0)
                {
                  string[] rskeys = new string[GameScenarioManager.Instance().AllyFighters.Count];
                  GameScenarioManager.Instance().AllyFighters.Keys.CopyTo(rskeys, 0);
                  ActorInfo rs = GameScenarioManager.Instance().AllyFighters[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

                  ActionManager.QueueLast(a, new Actions.AttackActor(rs, -1, -1, false));
                }
                break;
            }

            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
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
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            aci = new ActorCreationInfo(TIE_LN_ATI.Instance());
            aci.CreationTime = Game.Instance().GameTime + t;
            aci.Faction = FactionInfo.Get("Empire");
            aci.InitialState = ActorState.NORMAL;
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 5000);
            aci.Rotation = new TV_3DVECTOR();

            ActorInfo a = ActorInfo.Create(aci);

            ActionManager.QueueLast(a, new Actions.Wait(18));
            switch (GameScenarioManager.Instance().Difficulty.ToLower())
            {
              case "mental":
                if (GameScenarioManager.Instance().AllyFighters.Count > 0)
                {
                  string[] rskeys = new string[GameScenarioManager.Instance().AllyFighters.Count];
                  GameScenarioManager.Instance().AllyFighters.Keys.CopyTo(rskeys, 0);
                  ActorInfo rs = GameScenarioManager.Instance().AllyFighters[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

                  ActionManager.QueueLast(a, new Actions.AttackActor(rs, -1, -1, false));
                }
                break;
            }

            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_02(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIEs
      ActorCreationInfo aci;
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { TIE_LN_ATI.Instance(), TIE_IN_ATI.Instance() };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        int n = Engine.Instance().Random.Next(0, tietypes.Length);
        aci = new ActorCreationInfo(tietypes[n]);
        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            aci.CreationTime = Game.Instance().GameTime + t;
            aci.Faction = FactionInfo.Get("Empire");
            aci.InitialState = ActorState.NORMAL;
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 5000);
            aci.Rotation = new TV_3DVECTOR();

            ActorInfo a = ActorInfo.Create(aci);

            ActionManager.QueueLast(a, new Actions.Wait(18));
            switch (GameScenarioManager.Instance().Difficulty.ToLower())
            {
              case "mental":
                if (GameScenarioManager.Instance().AllyFighters.Count > 0)
                {
                  string[] rskeys = new string[GameScenarioManager.Instance().AllyFighters.Count];
                  GameScenarioManager.Instance().AllyFighters.Keys.CopyTo(rskeys, 0);
                  ActorInfo rs = GameScenarioManager.Instance().AllyFighters[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

                  ActionManager.QueueLast(a, new Actions.AttackActor(rs, -1, -1, false));
                }
                break;
            }

            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
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
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            aci = new ActorCreationInfo(TIE_IN_ATI.Instance());
            aci.CreationTime = Game.Instance().GameTime + t;
            aci.Faction = FactionInfo.Get("Empire");
            aci.InitialState = ActorState.NORMAL;
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 5000);
            aci.Rotation = new TV_3DVECTOR();

            ActorInfo a = ActorInfo.Create(aci);

            ActionManager.QueueLast(a, new Actions.Wait(18));
            switch (GameScenarioManager.Instance().Difficulty.ToLower())
            {
              case "mental":
                if (GameScenarioManager.Instance().AllyFighters.Count > 0)
                {
                  string[] rskeys = new string[GameScenarioManager.Instance().AllyFighters.Count];
                  GameScenarioManager.Instance().AllyFighters.Keys.CopyTo(rskeys, 0);
                  ActorInfo rs = GameScenarioManager.Instance().AllyFighters[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

                  ActionManager.QueueLast(a, new Actions.AttackActor(rs, -1, -1, false));
                }
                break;
            }

            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsWedge(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        aci = new ActorCreationInfo(TIE_LN_ATI.Instance());
        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Empire");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000);
        aci.Rotation = new TV_3DVECTOR();

        ActorInfo a = ActorInfo.Create(aci);

        ActionManager.QueueLast(a, new Actions.Wait(18));
        if (m_Wedge != null && m_Wedge.CreationState == CreationState.ACTIVE)
        { 
          ActionManager.QueueLast(a, new Actions.AttackActor(m_Wedge, -1, -1, false));
        }

        GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
        RegisterEvents(a);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_InterceptorsvsWedge(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        aci = new ActorCreationInfo(TIE_IN_ATI.Instance());
        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Empire");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000);
        aci.Rotation = new TV_3DVECTOR();

        ActorInfo a = ActorInfo.Create(aci);

        ActionManager.QueueLast(a, new Actions.Wait(18));
        if (m_Wedge != null && m_Wedge.CreationState == CreationState.ACTIVE)
        {
          ActionManager.QueueLast(a, new Actions.AttackActor(m_Wedge, -1, -1, false));
        }

        GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
        RegisterEvents(a);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsFalcon(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        aci = new ActorCreationInfo(TIE_LN_ATI.Instance());
        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Empire");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000);
        aci.Rotation = new TV_3DVECTOR();

        ActorInfo a = ActorInfo.Create(aci);

        ActionManager.QueueLast(a, new Actions.Wait(18));
        if (m_Falcon != null && m_Falcon.CreationState == CreationState.ACTIVE)
        {
          ActionManager.QueueLast(a, new Actions.AttackActor(m_Falcon, -1, -1, false));
        }

        GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
        RegisterEvents(a);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_InterceptorsvsFalcon(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        aci = new ActorCreationInfo(TIE_IN_ATI.Instance());
        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Empire");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000);
        aci.Rotation = new TV_3DVECTOR();

        ActorInfo a = ActorInfo.Create(aci);

        ActionManager.QueueLast(a, new Actions.Wait(18));
        if (m_Falcon != null && m_Falcon.CreationState == CreationState.ACTIVE)
        {
          ActionManager.QueueLast(a, new Actions.AttackActor(m_Falcon, -1, -1, false));
        }

        GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
        RegisterEvents(a);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsPlayer(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        aci = new ActorCreationInfo(TIE_LN_ATI.Instance());
        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Empire");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000);
        aci.Rotation = new TV_3DVECTOR();

        ActorInfo a = ActorInfo.Create(aci);

        ActionManager.QueueLast(a, new Actions.Wait(18));
        if (PlayerInfo.Instance().Actor != null && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD)
        {
          ActionManager.QueueLast(a, new Actions.AttackActor(PlayerInfo.Instance().Actor, -1, -1, false));
        }

        GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
        RegisterEvents(a);

        t += 1.5f;
      }
    }

    public void Empire_TIEWave_InterceptorsvsPlayer(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Interceptors only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        aci = new ActorCreationInfo(TIE_IN_ATI.Instance());
        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Empire");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MinBounds.z - 5000);
        aci.Rotation = new TV_3DVECTOR();

        ActorInfo a = ActorInfo.Create(aci);

        ActionManager.QueueLast(a, new Actions.Wait(18));
        if (PlayerInfo.Instance().Actor != null && PlayerInfo.Instance().Actor.ActorState != ActorState.DEAD)
        {
          ActionManager.QueueLast(a, new Actions.AttackActor(PlayerInfo.Instance().Actor, -1, -1, false));
        }

        GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
        RegisterEvents(a);

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
            ActorTypeInfo type = TIE_sa_ATI.Instance();
            string name = TIE_sa_ATI.Instance().Name;
            float spawntime = Game.Instance().GameTime + t;
            FactionInfo faction = FactionInfo.Get("Empire");
            TV_3DVECTOR position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 2500);
            TV_3DVECTOR rotation = new TV_3DVECTOR();
            ActionInfo[] actions = new ActionInfo[] { };
            Dictionary<string, ActorInfo>[] registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().EnemyFighters };

            if (GameScenarioManager.Instance().AllyShips.Count > 0)
            {
              string[] rskeys = new string[GameScenarioManager.Instance().AllyShips.Count];
              GameScenarioManager.Instance().AllyShips.Keys.CopyTo(rskeys, 0);
              ActorInfo rs = GameScenarioManager.Instance().AllyShips[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

              actions = new ActionInfo[] { new Actions.Wait(18)
                                         , new Actions.AttackActor(rs, -1, -1, false)
                                         };
            }

            ActorInfo ainfo = SpawnActor(type
                 , name
                 , ""
                 , ""
                 , spawntime
                 , faction
                 , position
                 , rotation
                 , actions
                 , registries);
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
      ActionInfo[] actions = new ActionInfo[] { };
      Dictionary<string, ActorInfo>[] registries = new Dictionary<string, ActorInfo>[] { };
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
        actions = new ActionInfo[] { };
        registries = new Dictionary<string, ActorInfo>[] { };

        ainfo = SpawnActor(type
                         , name
                         , ""
                         , ""
                         , creationTime
                         , null
                         , position
                         , rotation
                         , actions
                         , registries);

        ainfo.ActorState = ActorState.FIXED;

        if (ainfo.TypeInfo is ExecutorStaticATI)
        {
          m_ExecutorStatic = ainfo;
        }
      }
    }


    public void Empire_StarDestroyer_Spawn(object[] param)
    {
      if (param == null || param.GetLength(0) < 3 || !(param[0] is ActorTypeInfo) || !(param[1] is TV_3DVECTOR || !(param[2] is TV_3DVECTOR)))
        return;

      ActorCreationInfo acinfo;
      ActorInfo ainfo;
      ActorTypeInfo type = (ActorTypeInfo)param[0];
      TV_3DVECTOR position = (TV_3DVECTOR)param[1];
      TV_3DVECTOR targetposition = (TV_3DVECTOR)param[2];
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -25000);

      acinfo = new ActorCreationInfo(type);
      acinfo.Faction = FactionInfo.Get("Empire");
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = Game.Instance().GameTime;
      acinfo.Position = position + hyperspaceInOffset;
      acinfo.Rotation = new TV_3DVECTOR();
      ainfo = ActorInfo.Create(acinfo);

      ActionManager.QueueLast(ainfo, new Actions.HyperspaceIn(position));
      ActionManager.QueueLast(ainfo, new Actions.Move(targetposition, ainfo.MaxSpeed));
      ActionManager.QueueLast(ainfo, new Actions.Rotate(targetposition + new TV_3DVECTOR(0, 0, 25000), ainfo.MinSpeed));
      ActionManager.QueueLast(ainfo, new Actions.Lock());

      GameScenarioManager.Instance().EnemyShips.Add(ainfo.Key, ainfo);
      GameScenarioManager.Instance().CriticalEnemies.Add(ainfo.Name.ToUpper() + "    " + ainfo.ID, ainfo);
      RegisterEvents(ainfo);
      if (ainfo.GetStateF("TIEspawnRemaining") > 0)
        ainfo.TickEvents.Add("Empire_SDSpawner");
    }

    public void Empire_StarDestroyer_01(object[] param)
    {
      GameScenarioManager.Instance().SDWaves++;

      switch (GameScenarioManager.Instance().Difficulty.ToLower())
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

          //m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2500, -250, -19500), new TV_3DVECTOR(-2000, 110, -9500) });
          //m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-3500, 150, -18500), new TV_3DVECTOR(-3000, 210, -7000) });
          //m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-3250, -150, -19000), new TV_3DVECTOR(-2250, -150, -6500) });
          m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(-2250, -50, -19000), new TV_3DVECTOR(-1250, -50, -6500) });
          //m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2500, -250, -19500), new TV_3DVECTOR(2000, 110, -9500) });
          //m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(3500, 150, -18500), new TV_3DVECTOR(3000, 210, -7000) });
          //m_pendingSDspawnlist.Add(new object[] { ArquitensATI.Instance(), new TV_3DVECTOR(3250, -150, -19000), new TV_3DVECTOR(2250, -150, -6500) });
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
      GameScenarioManager.Instance().SDWaves++;

      switch (GameScenarioManager.Instance().Difficulty.ToLower())
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
          m_pendingSDspawnlist.Add(new object[] { DevastatorATI.Instance(), new TV_3DVECTOR(0, 120, -26500), new TV_3DVECTOR(0, 120, -11500) });
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
      GameScenarioManager.Instance().SDWaves++;
      if (m_ExecutorStatic != null)
      {
        m_ExecutorStatic.Destroy();
      }

      ActorCreationInfo acinfo;
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.Instance().GameTime;

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      positions.Add(new TV_3DVECTOR(0, -750, -20000));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(ExecutorATI.Instance());
        acinfo.Faction = FactionInfo.Get("Empire");
        acinfo.InitialState = ActorState.NORMAL;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + hyperspaceInOffset;
        acinfo.Rotation = new TV_3DVECTOR();
        ainfo = ActorInfo.Create(acinfo);

        ActionManager.QueueLast(ainfo, new Actions.HyperspaceIn(v));
        ActionManager.QueueLast(ainfo, new Actions.Move(new TV_3DVECTOR(v.x, v.y, -4000), ainfo.MaxSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Rotate(v + new TV_3DVECTOR(0, 0, 22500), ainfo.MinSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Lock());

        /*
        ainfo.AI.Orders.Enqueue(new AIElement
        {
          AIType = AIType.HYPERSPACE_IN,
          TargetPosition = v
        });
        ainfo.AI.Orders.Enqueue(new AIElement
        {
          AIType = AIType.MOVE,
          TargetPosition = new TV_3DVECTOR(v.x, v.y, -4000)
        });
        ainfo.AI.Orders.Enqueue(new AIElement
        {
          AIType = AIType.ROTATE,
          TargetPosition = v + new TV_3DVECTOR(0, 0, 22500)
        });
        ainfo.AI.Orders.Enqueue(new AIElement { AIType = AIType.LOCK });
        */

        GameScenarioManager.Instance().EnemyShips.Add(ainfo.Key, ainfo);
        GameScenarioManager.Instance().CriticalEnemies.Add("EXECUTOR      " + ainfo.ID, ainfo);
        RegisterEvents(ainfo);
        ainfo.ActorStateChangeEvents.Add("Empire_ExecutorDestroyed");
      }

      // SD


      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "easy":
          break;
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2500, -80, -21000), new TV_3DVECTOR(-2500, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-3500, 120, -20500), new TV_3DVECTOR(-2500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(3500, 120, -20500), new TV_3DVECTOR(2500, 120, -6500) });


          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -80, -24000), new TV_3DVECTOR(-2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-4500, 120, -20500), new TV_3DVECTOR(-3500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(-500, 120, -20500), new TV_3DVECTOR(-500, 120, -6500) });

          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2000, -80, -24000), new TV_3DVECTOR(2000, -80, -7000) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(500, 120, -20500), new TV_3DVECTOR(500, 120, -6500) });
          m_pendingSDspawnlist.Add(new object[] { AcclamatorATI.Instance(), new TV_3DVECTOR(4500, 120, -20500), new TV_3DVECTOR(3500, 120, -6500) });

          break;
        case "hard":
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
          PlayerInfo.Instance().Actor = a;
          PlayerInfo.Instance().Actor.Strength = 0;

          a.CamDeathCirclePeriod = ainfo.CamDeathCirclePeriod;
          a.CamDeathCircleRadius = ainfo.CamDeathCircleRadius;
          a.CamDeathCircleHeight = ainfo.CamDeathCircleHeight;

          if (ainfo.ActorState == ActorState.DYING)
          {
            ainfo.TickEvents.Add("Common_ProcessPlayerDying");
            ainfo.DestroyedEvents.Add("Common_ProcessPlayerKilled");
          }
          else
          {
            GameScenarioManager.Instance().Scenario.ProcessPlayerKilled(new object[] { ainfo });
          }

          SoundManager.Instance().SetMusic("executorend");
          ainfo.TimedLife = 2000f;

          if (GameScenarioManager.Instance().AllyShips.ContainsKey("Mon Calamari (Home One)"))
          {
            GameScenarioManager.Instance().AllyShips["Mon Calamari (Home One)"].Strength = 100000;
          }
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 54, "Common_FadeOut");
        }
        else
        {
          GameScenarioManager.Instance().SceneCamera.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
        }
      }
    }

    public void Empire_DeathStarAttack(object[] param)
    {
      if (m_ADSLaserSource != null && m_ADSLaserSource.CreationState == CreationState.ACTIVE)
      {
        List<ActorInfo> victims = new List<ActorInfo>();
        foreach (KeyValuePair<string, ActorInfo> kvp in GameScenarioManager.Instance().AllyShips)
        {
          if (kvp.Key != "Mon Calamari (Home One)")
          {
            victims.Add(kvp.Value);
          }
        }

        if (GameScenarioManager.Instance().AllyShips.Count > 0)
        {
          m_ADS_target = victims[Engine.Instance().Random.Next(0, victims.Count)];

          ActionManager.ForceClearQueue(m_ADSLaserSource);
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.AttackActor(m_ADS_target));
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.Lock());

          m_ADS_target.DestroyedEvents.Add("Scene_DeathStarKill_Effect");
          GameScenarioManager.Instance().AddEvent(0.1f, "Scene_DeathStarCam");
        }
      }
    }

    public void Empire_DeathStarAttack_01(object[] param)
    {
      if (m_ADSLaserSource != null && m_ADSLaserSource.CreationState == CreationState.ACTIVE)
      {
        int c = -1;
        while (!GameScenarioManager.Instance().AllyShips.ContainsKey(CorellianATI.Instance().Name + " " + c) && c < 10)
          c++;

        if (GameScenarioManager.Instance().AllyShips.ContainsKey(CorellianATI.Instance().Name + " " + c))
        {
          m_ADS_target = GameScenarioManager.Instance().AllyShips[CorellianATI.Instance().Name + " " + c];

          ActionManager.ForceClearQueue(m_ADSLaserSource);
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.AttackActor(m_ADS_target));
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.Lock());

          m_ADS_target.DestroyedEvents.Add("Scene_DeathStarKill_Effect");
          GameScenarioManager.Instance().AddEvent(0.1f, "Scene_DeathStarCam");
        }
      }
    }

    public void Empire_DeathStarAttack_02(object[] param)
    {
      if (m_ADSLaserSource != null && m_ADSLaserSource.CreationState == CreationState.ACTIVE)
      {
        int c = -1;
        while (!GameScenarioManager.Instance().AllyShips.ContainsKey(TransportATI.Instance().Name + " " + c) && c < 10)
          c++;

        if (GameScenarioManager.Instance().AllyShips.ContainsKey(TransportATI.Instance().Name + " " + c))
        {
          m_ADS_target = GameScenarioManager.Instance().AllyShips[TransportATI.Instance().Name + " " + c];

          ActionManager.ForceClearQueue(m_ADSLaserSource);
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.AttackActor(m_ADS_target));
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.Lock());

          m_ADS_target.DestroyedEvents.Add("Scene_DeathStarKill_Effect");
          GameScenarioManager.Instance().AddEvent(0.1f, "Scene_DeathStarCam");
        }
      }
    }

    public void Empire_DeathStarAttack_03(object[] param)
    {
      if (m_ADSLaserSource != null && m_ADSLaserSource.CreationState == CreationState.ACTIVE)
      {
        int c = -1;
        while (!GameScenarioManager.Instance().AllyShips.ContainsKey(MC90ATI.Instance().Name + " " + c) && c < 10)
          c++;

        if (GameScenarioManager.Instance().AllyShips.ContainsKey(MC90ATI.Instance().Name + " " + c))
        {
          m_ADS_target = GameScenarioManager.Instance().AllyShips[MC90ATI.Instance().Name + " " + c];

          ActionManager.ForceClearQueue(m_ADSLaserSource);
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.AttackActor(m_ADS_target));
          ActionManager.QueueNext(m_ADSLaserSource, new Actions.Lock());

          GameScenarioManager.Instance().AddEvent(0.1f, "Scene_DeathStarCam");
        }
      }
    }

    public void Empire_SDSpawner(object[] param)
    {
      if (param.GetLength(0) < 1 || param[0] == null)
        return;

      ActorInfo ainfo = (ActorInfo)param[0];

      // spawner deployment logic
      if (ainfo.ActorState != ActorState.DEAD
          && ainfo.ActorState != ActorState.DYING
          && ainfo.IsStateFDefined("TIEspawnCooldown")
          && ainfo.GetStateF("TIEspawnCooldown") < Game.Instance().GameTime
          && GameScenarioManager.Instance().EnemyFighters.Count < 45)
      {
        if (ainfo.TypeInfo.FireWeapon(ainfo, null, "TIEspawn"))
        {
          foreach (ActorInfo a in ainfo.GetAllChildren(1))
          {
            if (a.TypeInfo is FighterGroup || a.TypeInfo is TIEGroup)
            {
              if (!GameScenarioManager.Instance().EnemyFighters.ContainsKey(a.Key))
              {
                GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
                RegisterEvents(a);
              }
            }
          }
        }
      }
    }

    #endregion

    #region Text
    public void Message_01_AllWingsReport(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: All wings report in.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_02_RedLeader(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING (WEDGE): Red Leader standing by.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_03_GoldLeader(object[] param)
    {
      Screen2D.Instance().UpdateText("Y-WING: Gray Leader standing by.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.6f, 1));
    }

    public void Message_04_BlueLeader(object[] param)
    {
      Screen2D.Instance().UpdateText("B-WING: Blue Leader standing by.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_05_GreenLeader(object[] param)
    {
      Screen2D.Instance().UpdateText("A-WING: Green Leader standing by.", Game.Instance().GameTime + 5f, new TV_COLOR(0.4f, 0.8f, 0.4f, 1));
    }

    public void Message_06_Force(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): May the Force be with us.", Game.Instance().GameTime + 5f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_07_Break(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: Break off the attack!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_08_Break(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: The shield's still up!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_09_Conf(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING (WEDGE): I get no reading, are you sure?", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_10_Break(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: All wings, pull up!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_11_Evasive(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): Take evasive action!", Game.Instance().GameTime + 5f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_12_Trap(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): It's a trap!", Game.Instance().GameTime + 5f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_13_Fighters(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING (WEDGE): Watch out for enemy fighters.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_14_Interceptors(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING (WEDGE): TIE Interceptors inbound.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_15_Bombers(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): We have bombers inbound. Keep them away from our cruisers!", Game.Instance().GameTime + 5f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_20_DeathStar(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: That blast came from the Death Star...", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_21_Close(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: Get us close to those Imperial Star Destroyers.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_22_PointBlank(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: Closer! Get us closer, and engage them at point blank range.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_23_Take(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: If they fire, we might even take a few of them with us.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_30_ShieldDown(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): The shield is down.", Game.Instance().GameTime + 5f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_31_ResumeAttack(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): Commence your attack on the Death Star's main reactor", Game.Instance().GameTime + 5f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_32_Han(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: I knew Han can do it!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_33_Han(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: Wedge, follow me.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_34_Han(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING (WEDGE): Copy, Gold Leader.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_40_Focus(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): Focus all firepower on that Super Star Destroyer!", Game.Instance().GameTime + 5f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    public void Message_90_LostWedge(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING (WEDGE): I can't hold them!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_91_LostFalcon(object[] param)
    {
      Screen2D.Instance().UpdateText("MILLIENNIUM FALCON: I can't hold them!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.8f, 1));
    }

    public void Message_92_LostHomeOne(object[] param)
    {
      Screen2D.Instance().UpdateText("MON CALAMARI (HOME ONE): We have no chance...", Game.Instance().GameTime + 15f, new TV_COLOR(0.2f, 0.4f, 1, 1));
    }

    #endregion

    #region Scene
    public void Scene_EnterCutscene(object[] param)
    {
      if (m_Falcon != null)
      {
        m_Falcon_pos = m_Falcon.Position;
        m_Falcon.ActorState = ActorState.FIXED;
        m_Falcon.Position = new TV_3DVECTOR(10, 0, -100000);
      }

      if (m_Wedge != null)
      {
        m_Wedge_pos = m_Wedge.Position;
        m_Wedge.ActorState = ActorState.FIXED;
        m_Wedge.Position = new TV_3DVECTOR(20, 0, -100000);
      }

      if (m_Player != null)
      {
        m_Player_pos = m_Player.Position;
        m_Player.ActorState = ActorState.FIXED;
        m_Player.Position = new TV_3DVECTOR(30, 0, -100000);
        //PlayerInfo.Instance().Actor = null;
        m_Player_PrimaryWeapon = PlayerInfo.Instance().PrimaryWeapon;
        m_Player_SecondaryWeapon = PlayerInfo.Instance().SecondaryWeapon;
      }

      if (m_HomeOne != null)
      {
        m_HomeOne_Strength = m_HomeOne.Strength;
        m_HomeOne.Strength += 2000;
      }

      PlayerInfo.Instance().Actor = GameScenarioManager.Instance().SceneCamera;
      GameScenarioManager.Instance().IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(object[] param)
    {
      if (m_Falcon != null)
      {
        m_Falcon.ActorState = ActorState.NORMAL;
        m_Falcon.Position = m_Falcon_pos;
      }

      if (m_Wedge != null)
      {
        m_Wedge.ActorState = ActorState.NORMAL;
        m_Wedge.Position = m_Wedge_pos;
      }

      if (m_Player != null)
      {
        m_Player.ActorState = ActorState.NORMAL;
        m_Player.Position = m_Player_pos;
        PlayerInfo.Instance().Actor = m_Player;
        PlayerInfo.Instance().PrimaryWeapon = m_Player_PrimaryWeapon;
        PlayerInfo.Instance().SecondaryWeapon = m_Player_SecondaryWeapon;
      }

      if (m_HomeOne != null)
      {
        m_HomeOne.Strength = m_HomeOne_Strength;
      }

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam(object[] param)
    {
      if (m_ADS_target.CreationState == CreationState.ACTIVE)
      {
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_EnterCutscene");
        SoundManager.Instance().SetSound("ds_beam", false, 1, false);

        TV_3DVECTOR pos = m_ADS_target.GetPosition();
        TV_3DVECTOR rot = m_ADS_target.GetRotation();

        if (m_ADS_target.TypeInfo is CorellianATI)
        { pos += new TV_3DVECTOR(150, 120, -2000); }
        else if (m_ADS_target.TypeInfo is TransportATI)
        { pos += new TV_3DVECTOR(300, 200, 700); }
        else if (m_ADS_target.TypeInfo is MC90ATI)
        { pos += new TV_3DVECTOR(-850, -400, 2500); }

        GameScenarioManager.Instance().SceneCamera.SetLocalPosition(pos.x, pos.y, pos.z);
        GameScenarioManager.Instance().SceneCamera.LookAtPoint(new TV_3DVECTOR());
        GameScenarioManager.Instance().SceneCamera.MaxSpeed = 50;
        GameScenarioManager.Instance().SceneCamera.Speed = 50;
        GameScenarioManager.Instance().CameraTargetActor = m_ADS_target;
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Scene_ExitCutscene");
      }
    }

    public void Scene_DeathStarKill_Effect(object[] param)
    {
      PlayerInfo.Instance().shake_displacement = 150;
      SoundManager.Instance().SetSoundStop("ds_beam");
      SoundManager.Instance().SetSound("exp_nave", false, 1, false);
    }
    #endregion

    public override void GameWonSequence(object[] param)
    {
      Engine.Instance().TVGraphicEffect.FadeIn(2.5f);
      Screen2D.Instance().CurrentPage = new UIPage_GameWon();
      Screen2D.Instance().ShowPage = true;
      Game.Instance().IsPaused = true;
      SoundManager.Instance().SetMusic("finale_3_1");
      SoundManager.Instance().SetMusicLoop("credits_3_1");
    }
  }
}
