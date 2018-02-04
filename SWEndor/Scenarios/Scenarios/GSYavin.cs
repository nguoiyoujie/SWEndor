using MTV3D65;
using System.Collections.Generic;
using System;

namespace SWEndor.Scenarios
{
  public class GSYavin : GameScenarioBase
  {
    public GSYavin()
    {
      Name = "Battle of Yavin (WIP)";
      AllowedWings = new List<ActorTypeInfo> { XWingATI.Instance() };

      AllowedDifficulties = new List<string> { "easy"
                                               , "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private ActorInfo m_AScene = null;
    private ActorInfo m_AYavin = null;
    private ActorInfo m_AYavin4 = null;
    private ActorInfo m_ADS = null;
    private ActorInfo m_ADS_Surface = null;
    private List<ActorInfo> m_ADS_SurfaceParts = new List<ActorInfo>();
    private ThreadSafeDictionary<int, ActorInfo> m_ADS_TrenchParts = new ThreadSafeDictionary<int, ActorInfo>();
    private ActorInfo m_AStar = null;
    private List<object[]> m_pendingSDspawnlist = new List<object[]>();
    private Dictionary<string, ActorInfo> m_GroundObjects = new Dictionary<string, ActorInfo>();
    private float expiretime = 1800;
    private float target_distX = 118000;
    private float vader_distX = 91000;
    private float vaderend_distX = 107000;
    private float last_target_distX = 0;
    private float last_sound_distX = 0;
    List<string> names = new List<string>();

    private ActorInfo m_Player = null;
    private ActorInfo m_Falcon = null;
    private ActorInfo m_Vader = null;
    private ActorInfo m_VaderEscort1 = null;
    private ActorInfo m_VaderEscort2 = null;
    private float m_Player_DamageModifier = 1;
    private string m_Player_PrimaryWeapon = "";
    private string m_Player_SecondaryWeapon = "";

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Instance().Name = "Red Five";

      if (GameScenarioManager.Instance().GetGameStateB("in_game"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_game", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(15000, 1500, 10000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-15000, -1500, -15000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(15000, 1500, 10000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-15000, -1500, -15000);

      PlayerInfo.Instance().CameraMode = CameraMode.FIRSTPERSON;

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_HyperspaceIn");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3.5f, "Rebel_MakePlayer");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3f, "Rebel_RemoveTorps");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7.5f, "Rebel_GiveControl");

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 6f, "Message.01");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 1f, "Empire_FirstWave");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, "Empire_FirstTIEWave");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 9f, "Rebel_YWingsAttackScan");

      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "mental":
          expiretime = Game.Instance().GameTime + 1230;
          break;
        case "hard":
          expiretime = Game.Instance().GameTime + 1530;
          break;
        case "normal":
          expiretime = Game.Instance().GameTime + 1530;
          break;
        case "easy":
        default:
          expiretime = Game.Instance().GameTime + 1830;
          break;
      }

      GameScenarioManager.Instance().AddEvent(expiretime, "Message.100");

      if (expiretime - Game.Instance().GameTime > 60)
        GameScenarioManager.Instance().AddEvent(expiretime - 60, "Message.101");

      if (expiretime - Game.Instance().GameTime > 120)
        GameScenarioManager.Instance().AddEvent(expiretime - 120, "Message.102");

      if (expiretime - Game.Instance().GameTime > 180)
        GameScenarioManager.Instance().AddEvent(expiretime - 180, "Message.103");

      if (expiretime - Game.Instance().GameTime > 300)
        GameScenarioManager.Instance().AddEvent(expiretime - 300, "Message.105");

      if (expiretime - Game.Instance().GameTime > 600)
        GameScenarioManager.Instance().AddEvent(expiretime - 600, "Message.110");

      if (expiretime - Game.Instance().GameTime > 900)
        GameScenarioManager.Instance().AddEvent(expiretime - 900, "Message.115");

      if (expiretime - Game.Instance().GameTime > 1200)
        GameScenarioManager.Instance().AddEvent(expiretime - 1200, "Message.120");

      if (expiretime - Game.Instance().GameTime > 1500)
        GameScenarioManager.Instance().AddEvent(expiretime - 1500, "Message.125");

      if (expiretime - Game.Instance().GameTime > 1800)
        GameScenarioManager.Instance().AddEvent(expiretime - 1800, "Message.130");

      PlayerInfo.Instance().Lives = 4;
      PlayerInfo.Instance().ScorePerLife = 1000000;
      PlayerInfo.Instance().ScoreForNextLife = 1000000;
      PlayerInfo.Instance().Score = new ScoreInfo();

      MakePlayer = Rebel_MakePlayer;

      // Team mate names

      names.Add("Red Leader (Garven Dreis)");
      names.Add("Red Two (Wedge Antilles)");
      names.Add("Red Three (Biggs Darklighter)");
      names.Add("Red Four (John Branon)");
      //names.Add("Red Five (Luke Skywalker)");
      names.Add("Red Six (Jek Porkins)");
      names.Add("Red Seven (Harb Binli)");
      names.Add("Red Eight (Zal Dinnes)");
      names.Add("Red Nine (Nozzo Naytaan)");
      names.Add("Red Ten (Theron Nett)");
      names.Add("Red Eleven (Ralo Surrel)");
      names.Add("Red Twelve (Puck Naeco)");
      names.Add("Green Leader");
      names.Add("Green Two");
      names.Add("Green Three");
      names.Add("Green Four");
      names.Add("Green Five");
      names.Add("Green Six");
      names.Add("Green Seven");
      names.Add("Green Eight");
      names.Add("Green Nine");
      names.Add("Green Ten");
      names.Add("Gold Leader (Jon Vander)");
      names.Add("Gold Two (Dex Tiree)");
      names.Add("Gold Three (Evaan Verlaine)");
      names.Add("Gold Four");
      names.Add("Gold Five (Davish Krail)");
      names.Add("Gold Six");
      names.Add("Gold Seven (Gazdo Woolcob)");
      names.Add("Gold Eight");

      GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      //GameScenarioManager.Instance().Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      SoundManager.Instance().SetMusic("battle_1_1");
      SoundManager.Instance().SetMusicLoop("battle_1_4");

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.AddFaction("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Rebels_Gold", new TV_COLOR(0.8f, 0.3f, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire_DeathStarDefenses", new TV_COLOR(0.1f, 0.8f, 0, 1)).AutoAI = true;

      FactionInfo.Get("Rebels").Allies.Add(FactionInfo.Get("Rebels_Gold"));
      FactionInfo.Get("Rebels_Gold").Allies.Add(FactionInfo.Get("Rebels"));

      FactionInfo.Get("Empire").Allies.Add(FactionInfo.Get("Empire_DeathStarDefenses"));
      FactionInfo.Get("Empire_DeathStarDefenses").Allies.Add(FactionInfo.Get("Empire"));
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

      // Create Yavin
      if (m_AYavin == null)
      {
        acinfo = new ActorCreationInfo(YavinATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, 0, 18000);
        acinfo.Rotation = new TV_3DVECTOR(90, 90, 0);
        acinfo.InitialScale = new TV_3DVECTOR(4, 4, 4);
        m_AYavin = ActorInfo.Create(acinfo);
      }

      // Create Yavin 4
      if (m_AYavin4 == null)
      {
        acinfo = new ActorCreationInfo(Yavin4ATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, 800, -18000);
        acinfo.Rotation = new TV_3DVECTOR(0, 0, 0);
        m_AYavin4 = ActorInfo.Create(acinfo);
      }

      // Create DeathStar
      if (m_ADS == null)
      {
        acinfo = new ActorCreationInfo(DeathStarATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, 800, 28000);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        m_ADS = ActorInfo.Create(acinfo);
        m_ADS.Faction = FactionInfo.Get("Empire");
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      if (GameScenarioManager.Instance().GetGameStateB("in_battle"))
      {
        float enemystrength = 0;
        if (GameScenarioManager.Instance().StageNumber == 0)
        {
          GameScenarioManager.Instance().StageNumber = 1;
        }
        else if (GameScenarioManager.Instance().StageNumber == 1)
        {
          enemystrength = GameScenarioManager.Instance().EnemyFighters.Count;
          foreach (ActorInfo a in GameScenarioManager.Instance().EnemyShips.Values)
          {
            enemystrength += a.StrengthFrac * 100 + a.GetStateF("TIEspawnRemaining");
          }

          if (!GameScenarioManager.Instance().GetGameStateB("Stage1B"))
          {
            if (enemystrength < 50)
            {
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Message.02");
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13f, "Empire_SecondWave");
              GameScenarioManager.Instance().SetGameStateB("Stage1B", true);
            }
          }
          else if (GameScenarioManager.Instance().GetGameStateB("Stage1BBegin"))
          {
            if (enemystrength == 0 && !GameScenarioManager.Instance().GetGameStateB("Stage1BEnd"))
            {
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 1.5f, "Message.03");
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 2f, "Rebel_Forward");
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Scene_Stage02_ApproachDeathStar");
              GameScenarioManager.Instance().SetGameStateB("Stage1BEnd", true);
            }
          }
        }
        else if (GameScenarioManager.Instance().StageNumber == 2)
        {
          enemystrength = GameScenarioManager.Instance().EnemyFighters.Count;
          enemystrength += GameScenarioManager.Instance().EnemyShips.Count;

          if (enemystrength == 0 && !GameScenarioManager.Instance().GetGameStateB("Stage2End"))
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 1.5f, "Message.05");
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 2f, "Rebel_Forward");
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Scene_Stage03_Spawn");
            GameScenarioManager.Instance().SetGameStateB("Stage2End", true);
          }
        }
        else if (GameScenarioManager.Instance().StageNumber == 3)
        {
          enemystrength = GameScenarioManager.Instance().EnemyFighters.Count;
          enemystrength += GameScenarioManager.Instance().EnemyShips.Count;

          if (enemystrength == 0 && !GameScenarioManager.Instance().GetGameStateB("Stage3End"))
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 1.5f, "Message.07");
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 2f, "Rebel_Forward");
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Scene_Stage04_Spawn");
            GameScenarioManager.Instance().SetGameStateB("Stage3End", true);
          }
        }
        else if (GameScenarioManager.Instance().StageNumber == 4)
        {
          enemystrength = GameScenarioManager.Instance().EnemyFighters.Count;
          enemystrength += GameScenarioManager.Instance().EnemyShips.Count;

          if (enemystrength == 0 && !GameScenarioManager.Instance().GetGameStateB("Stage4End"))
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 1.5f, "Message.09");
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 2f, "Rebel_Forward");
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Scene_Stage05_Spawn");
            GameScenarioManager.Instance().SetGameStateB("Stage4End", true);
          }
        }
        else if (GameScenarioManager.Instance().StageNumber == 5)
        {
          if (PlayerInfo.Instance().Actor != null
           && PlayerInfo.Instance().Actor.GetPosition().x > GameScenarioManager.Instance().MaxBounds.x - 500
           && PlayerInfo.Instance().Actor.GetPosition().y < -180
           && PlayerInfo.Instance().Actor.GetPosition().z < 120
           && PlayerInfo.Instance().Actor.GetPosition().z > -120
           && !GameScenarioManager.Instance().GetGameStateB("Stage5StartRun"))
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_Stage05b_Spawn");
            GameScenarioManager.Instance().SetGameStateB("Stage5StartRun", true);
            Screen2D.Instance().OverrideTargetingRadar = true;
          }
          else if (m_Player != null && GameScenarioManager.Instance().GetGameStateB("Stage5StartRun"))
          {
            if (last_target_distX < m_Player.GetPosition().x)
            {
              PlayerInfo.Instance().Score.Score += (m_Player.GetPosition().x - last_target_distX) * 10;
              last_target_distX = m_Player.GetPosition().x;
            }
            if (last_sound_distX < m_Player.GetPosition().x && !GameScenarioManager.Instance().IsCutsceneMode)
            {
              SoundManager.Instance().SetSound("Button_3");
              last_sound_distX = m_Player.GetPosition().x + 250;
            }
            Screen2D.Instance().TargetingRadar_text = string.Format("{0:00000000}", (target_distX - m_Player.GetPosition().x) * 30);
            Scene_Stage05b_ContinuouslySpawnRoute(null);

            if (m_Player.GetPosition().x > vader_distX && m_Player.ActorState == ActorState.NORMAL && !GameScenarioManager.Instance().GetGameStateB("Stage5End"))
            {
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_Stage06_Vader");
              GameScenarioManager.Instance().SetGameStateB("Stage5End", true);
            }
          }
        }
        else if (GameScenarioManager.Instance().StageNumber == 6)
        {
          if (!GameScenarioManager.Instance().GetGameStateB("Stage6VaderEnd"))
          {
            if (PlayerInfo.Instance().ShowRadar)
            {
              m_Player.DamageModifier = 0.8f;
            }
            else
            {
              m_Player.DamageModifier = 0.4f;
            }
          }
          if (m_Player != null)
          {
            if (last_target_distX < m_Player.GetPosition().x)
            {
              PlayerInfo.Instance().Score.Score += (m_Player.GetPosition().x - last_target_distX) * 10;
              last_target_distX = m_Player.GetPosition().x;
            }
            if (last_sound_distX < m_Player.GetPosition().x && !GameScenarioManager.Instance().IsCutsceneMode)
            {
              SoundManager.Instance().SetSound("Button_3");
              last_sound_distX = m_Player.GetPosition().x + 250;
            }
            Screen2D.Instance().TargetingRadar_text = string.Format("{0:00000000}", (target_distX - m_Player.GetPosition().x) * 30);
            Scene_Stage05b_ContinuouslySpawnRoute(null);

            if (m_Player.GetPosition().x > vaderend_distX && m_Player.ActorState == ActorState.NORMAL && !GameScenarioManager.Instance().GetGameStateB("Stage6VaderEnd"))
            {
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_Stage06_VaderEnd");
              GameScenarioManager.Instance().SetGameStateB("Stage6VaderEnd", true);
              Rebel_RemoveTorps(null);
            }
            if (m_Vader != null && GameScenarioManager.Instance().GetGameStateB("Stage6VaderAttacking"))
            {
              m_Vader.SetLocalPosition(m_Player.GetPosition().x - 1700, -200, 0);
              m_VaderEscort1.SetLocalPosition(m_Player.GetPosition().x - 1700, -220, 75);
              m_VaderEscort2.SetLocalPosition(m_Player.GetPosition().x - 1700, -220, -75);
            }
          }
        }
      }


      if (m_pendingSDspawnlist.Count > 0 && GameScenarioManager.Instance().EnemyShips.Count < 5)
      {
        if (m_pendingSDspawnlist[0].Length > 0
        && (!(m_pendingSDspawnlist[0][0] is ImperialIATI) || GameScenarioManager.Instance().EnemyShips.Count < ((GameScenarioManager.Instance().StageNumber == 6) ? 3 : 2))
        && (!(m_pendingSDspawnlist[0][0] is DevastatorATI) || GameScenarioManager.Instance().EnemyShips.Count < 2))
        {
          Empire_StarDestroyer_Spawn(m_pendingSDspawnlist[0]);
          m_pendingSDspawnlist.RemoveAt(0);
        }
      }

      if (!GameScenarioManager.Instance().GetGameStateB("Stage5StartRun"))
      {
        if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().GameTime || Game.Instance().GameTime % 0.4f > 0.2f)
        {
          GameScenarioManager.Instance().Line1Text = string.Format("WINGS: {0}", GameScenarioManager.Instance().Scenario.RebelFighterLimit);
        }
        else
        {
          GameScenarioManager.Instance().Line1Text = string.Format("");
        }
      }
      else
      {
        GameScenarioManager.Instance().Line1Text = Screen2D.Instance().TargetingRadar_text;
      }

      GameScenarioManager.Instance().Line2Text = string.Format("TIME: {0:00}:{1:00}", (int)(expiretime - Game.Instance().GameTime) / 60, (int)(expiretime - Game.Instance().GameTime) % 60);
      if ((int)(expiretime - Game.Instance().GameTime) / 60 < 4)
      {
        GameScenarioManager.Instance().Line2Color = new TV_COLOR(1, 0.3f, 0.3f, 1);
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AYavin != null && m_AYavin.CreationState == CreationState.ACTIVE)
      {
        if (GameScenarioManager.Instance().StageNumber < 2)
        {
          float x_yv = (PlayerInfo.Instance().Position.y > 0) ? (PlayerInfo.Instance().Position.y / 6f) - 18000.0f : (PlayerInfo.Instance().Position.y / 2.5f) - 18000.0f;
          float y_yv = PlayerInfo.Instance().Position.x / 1.2f;
          float z_yv = PlayerInfo.Instance().Position.z / 1.2f;
          m_AYavin.SetLocalPosition(x_yv, y_yv, z_yv);
        }
        else
        {
          float x_yv = PlayerInfo.Instance().Position.x / 1.2f;
          float y_yv = 20000.0f;
          float z_yv = PlayerInfo.Instance().Position.z / 1.2f;
          m_AYavin.SetLocalPosition(x_yv, y_yv, z_yv);
        }
      }
      if (m_AYavin4 != null && m_AYavin4.CreationState == CreationState.ACTIVE)
      {
        float x_y4 = PlayerInfo.Instance().Position.x / 10f;
        float y_y4 = PlayerInfo.Instance().Position.y / 2f;
        float z_y4 = (PlayerInfo.Instance().Position.z > 0) ? PlayerInfo.Instance().Position.z / 1.5f + 30000f : PlayerInfo.Instance().Position.z / 100f + 30000f;
        m_AYavin4.SetLocalPosition(x_y4, y_y4, z_y4);
      }
      if (m_ADS != null && m_ADS.CreationState == CreationState.ACTIVE)
      {
        float x_ds = PlayerInfo.Instance().Position.x / 5f;
        float y_ds = (PlayerInfo.Instance().Position.y / 1.5f) + 3200.0f;
        float z_ds = (PlayerInfo.Instance().Position.z > 0) ? PlayerInfo.Instance().Position.z / 1.5f - 30000f : PlayerInfo.Instance().Position.z / 100f - 30000f;
        m_ADS.SetLocalPosition(x_ds, y_ds, z_ds);
      }
      if (m_AScene != null && m_AScene.CreationState == CreationState.ACTIVE)
      {
        m_AScene.SetLocalPosition(PlayerInfo.Instance().Position.x, PlayerInfo.Instance().Position.y, PlayerInfo.Instance().Position.z);
      }
    }

    public override void RegisterEvents()
    {
      base.RegisterEvents();
      GameEvent.RegisterEvent("Rebel_HyperspaceIn", Rebel_HyperspaceIn);
      GameEvent.RegisterEvent("Rebel_MakePlayer", Rebel_MakePlayer);
      GameEvent.RegisterEvent("Rebel_RemoveTorps", Rebel_RemoveTorps);
      GameEvent.RegisterEvent("Rebel_StrongerWings", Rebel_StrongerWings);
      GameEvent.RegisterEvent("Rebel_YWingsAttackScan", Rebel_YWingsAttackScan);
      GameEvent.RegisterEvent("Rebel_GiveControl", Rebel_GiveControl);
      GameEvent.RegisterEvent("Rebel_Forward", Rebel_Forward);
      GameEvent.RegisterEvent("Rebel_Reposition", Rebel_Reposition);

      GameEvent.RegisterEvent("Empire_FirstWave", Empire_FirstWave);
      GameEvent.RegisterEvent("Empire_SecondWave", Empire_SecondWave);
      GameEvent.RegisterEvent("Empire_StarDestroyer_Spawn", Empire_StarDestroyer_Spawn);
      GameEvent.RegisterEvent("Empire_FirstTIEWave", Empire_FirstTIEWave);
      GameEvent.RegisterEvent("Empire_TIEWave", Empire_TIEWave);
      GameEvent.RegisterEvent("Empire_SDSpawner", Empire_SDSpawner);
      GameEvent.RegisterEvent("Empire_Towers01", Empire_Towers01);
      GameEvent.RegisterEvent("Empire_Towers02", Empire_Towers02);
      GameEvent.RegisterEvent("Empire_Towers03", Empire_Towers03);
      GameEvent.RegisterEvent("Empire_Towers04", Empire_Towers04);

      GameEvent.RegisterEvent("Scene_EnterCutscene", Scene_EnterCutscene);
      GameEvent.RegisterEvent("Scene_ExitCutscene", Scene_ExitCutscene);
      GameEvent.RegisterEvent("Scene_DeathStarCam", Scene_DeathStarCam);
      GameEvent.RegisterEvent("Scene_Stage02_ApproachDeathStar", Scene_Stage02_ApproachDeathStar);
      GameEvent.RegisterEvent("Scene_Stage02_Spawn", Scene_Stage02_Spawn);
      GameEvent.RegisterEvent("Scene_Stage03_Spawn", Scene_Stage03_Spawn);
      GameEvent.RegisterEvent("Scene_Stage04_Spawn", Scene_Stage04_Spawn);
      GameEvent.RegisterEvent("Scene_Stage05_Spawn", Scene_Stage05_Spawn);
      GameEvent.RegisterEvent("Scene_Stage05b_Spawn", Scene_Stage05b_Spawn);
      GameEvent.RegisterEvent("Scene_Stage05b_SpawnRoute", Scene_Stage05b_SpawnRoute);
      GameEvent.RegisterEvent("Scene_Stage06_Vader", Scene_Stage06_Vader);
      GameEvent.RegisterEvent("Scene_Stage06_VaderAttack", Scene_Stage06_VaderAttack);
      GameEvent.RegisterEvent("Scene_Stage06_SetPlayer", Scene_Stage06_SetPlayer);
      GameEvent.RegisterEvent("Scene_Stage06_VaderEnd", Scene_Stage06_VaderEnd);
      GameEvent.RegisterEvent("Scene_Stage06_VaderFlee", Scene_Stage06_VaderFlee);

      
      GameEvent.RegisterEvent("Message.01", Message_01_EnemyShipsInbound);
      GameEvent.RegisterEvent("Message.02", Message_02_MoreEnemyShipsInbound);
      GameEvent.RegisterEvent("Message.03", Message_03_Clear);
      GameEvent.RegisterEvent("Message.04", Message_04_Target);
      GameEvent.RegisterEvent("Message.05", Message_05_Clear);
      GameEvent.RegisterEvent("Message.06", Message_06_TIE);
      GameEvent.RegisterEvent("Message.07", Message_07_Clear);
      GameEvent.RegisterEvent("Message.08", Message_08_Target);
      GameEvent.RegisterEvent("Message.09", Message_09_Clear);
      GameEvent.RegisterEvent("Message.10", Message_10_BeginRun);

      GameEvent.RegisterEvent("Message.100", Message_100_RebelBaseInRange);
      GameEvent.RegisterEvent("Message.101", Message_101_RebelBase);
      GameEvent.RegisterEvent("Message.102", Message_102_RebelBase);
      GameEvent.RegisterEvent("Message.103", Message_103_RebelBase);
      GameEvent.RegisterEvent("Message.105", Message_105_RebelBase);
      GameEvent.RegisterEvent("Message.110", Message_110_RebelBase);
      GameEvent.RegisterEvent("Message.115", Message_115_RebelBase);
      GameEvent.RegisterEvent("Message.120", Message_120_RebelBase);
      GameEvent.RegisterEvent("Message.125", Message_125_RebelBase);
      GameEvent.RegisterEvent("Message.130", Message_130_RebelBase);
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorCreationInfo acinfo;
      ActorInfo ainfo;

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(150, 100, GameScenarioManager.Instance().MaxBounds.z - 1000);

      // Player X-Wing
      acinfo = new ActorCreationInfo(PlayerInfo.Instance().ActorType);
      acinfo.Name = "(Player)";
      acinfo.Faction = FactionInfo.Get("Rebels");
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = Game.Instance().GameTime;
      acinfo.Position = new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z - 150);
      acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
      ainfo = ActorInfo.Create(acinfo);
      GameScenarioManager.Instance().AllyFighters.Add(ainfo.Name, ainfo);
      RegisterEvents(ainfo);

      ActionManager.QueueLast(ainfo, new Actions.Lock());
      ActionManager.QueueLast(ainfo, new Actions.Move(new TV_3DVECTOR(acinfo.Position.x + Engine.Instance().Random.Next(-5, 5), acinfo.Position.y + Engine.Instance().Random.Next(-5, 5), acinfo.Position.z - 4500)
                                                    , ainfo.MaxSpeed));

      GameScenarioManager.Instance().CameraTargetActor = ainfo;

      // X-Wings x21, Y-Wing x8
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 29; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Engine.Instance().Random.Next(-2800, -80), Engine.Instance().Random.Next(-200, 200), GameScenarioManager.Instance().MaxBounds.z + Engine.Instance().Random.Next(-2200, -150)));
        else
          positions.Add(new TV_3DVECTOR(Engine.Instance().Random.Next(80, 2800), Engine.Instance().Random.Next(-200, 200), GameScenarioManager.Instance().MaxBounds.z + Engine.Instance().Random.Next(-2200, -150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        if (i <= 21)
        {
          acinfo = new ActorCreationInfo(XWingATI.Instance());
          acinfo.Faction = FactionInfo.Get("Rebels");
        }
        else
        {
          acinfo = new ActorCreationInfo(YWingATI.Instance());
          acinfo.Faction = FactionInfo.Get("Rebels_Gold");
        }
        acinfo.Name = names[i];
        acinfo.InitialState = ActorState.NORMAL;
        acinfo.CreationTime = Game.Instance().GameTime;
        acinfo.Position = v;
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        ainfo = ActorInfo.Create(acinfo);

        ActionManager.QueueLast(ainfo, new Actions.Lock());
        ActionManager.QueueLast(ainfo, new Actions.Move(new TV_3DVECTOR(acinfo.Position.x + Engine.Instance().Random.Next(-5, 5), acinfo.Position.y + Engine.Instance().Random.Next(-5, 5), acinfo.Position.z - 4500)
                                                      , ainfo.MaxSpeed));

        GameScenarioManager.Instance().AllyFighters.Add(names[i], ainfo);
        GameScenarioManager.Instance().CriticalAllies.Add(names[i], ainfo);
        RegisterEvents(ainfo);
      }
      RebelFighterLimit = GameScenarioManager.Instance().AllyFighters.Count;
    }

    public void Rebel_RemoveTorps(object[] param)
    {
      foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
      {
        if (ainfo.TypeInfo is YWingATI)
        {
          foreach (KeyValuePair<string, WeaponInfo> kvp in ainfo.Weapons)
          {
            if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
            {
              kvp.Value.Ammo = 2;
              kvp.Value.MaxAmmo = 2;
            }
          }
        }
        else
        {
          foreach (KeyValuePair<string, WeaponInfo> kvp in ainfo.Weapons)
          {
            if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
            {
              kvp.Value.Ammo = 1;
              kvp.Value.MaxAmmo = 1;
            }
          }
          ainfo.SecondaryWeapons = new List<string> { "none" };
          ainfo.AIWeapons = new List<string> { "1:laser" };
        }
      }
      if (m_Player != null)
      {
        if (GameScenarioManager.Instance().GetGameStateB("Stage6VaderEnd"))
        {
          m_Player.MinSpeed = 400;
          m_Player.MaxSpeed = 400;
          m_Player.DamageModifier = 0.5f;
          m_Player.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new XWingTorpWeapon() }
                                                        , {"laser", new XWingLaserWeapon() }
                                                        };
          m_Player.PrimaryWeapons = new List<string> { "1:laser", "2:laser", "4:laser" };
          m_Player.SecondaryWeapons = new List<string> { "4:laser", "1:torp" };
          m_Player.AIWeapons = new List<string> { "1:torp", "1:laser" };
        }
        else if (GameScenarioManager.Instance().GetGameStateB("Stage5StartRun"))
        {
          m_Player.MinSpeed = 400;
          m_Player.MaxSpeed = 400;
          //m_Player.DamageModifier = 0.5f;
        }
        else if (GameScenarioManager.Instance().StageNumber > 1)
        {
          m_Player.MinSpeed = m_Player.MaxSpeed * 0.75f;
        }
      }
      PlayerInfo.Instance().ResetPrimaryWeapon();
      PlayerInfo.Instance().ResetSecondaryWeapon();
    }

    public void Rebel_StrongerWings(object[] param)
    {
      foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
      {
        if (!ainfo.GetStateB("Stronger") && !ainfo.IsPlayer())
        {
          ainfo.SetStateB("Stronger", true);
          ainfo.SelfRegenRate *= 2.5f;
          ainfo.MaxStrength *= 5;
          ainfo.Strength *= 5;
        }
      }
    }

    public void Rebel_YWingsAttackScan(object[] param)
    {
      if (GameScenarioManager.Instance().EnemyShips.Count > 0)
      {
        foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
        {
          if (ainfo.TypeInfo is YWingATI)
          {
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

    public void Rebel_MakePlayer(object[] param)
    {
      if (GameScenarioManager.Instance().AllyFighters.ContainsKey("(Player)"))
      {
        PlayerInfo.Instance().Actor = GameScenarioManager.Instance().AllyFighters["(Player)"];
      }
      else
      {
        if (PlayerInfo.Instance().Lives > 0)
        {
          PlayerInfo.Instance().Lives--;

          ActorCreationInfo placi = new ActorCreationInfo(PlayerInfo.Instance().ActorType);
          placi.Name = "(Player)";
          placi.CreationTime = Game.Instance().GameTime;
          placi.Faction = FactionInfo.Get("Rebels");
          placi.InitialState = ActorState.NORMAL;

          if (GameScenarioManager.Instance().GetGameStateB("Stage5StartRun"))
          {
            placi.Rotation = new TV_3DVECTOR(0, 90, 0);

            if (GameScenarioManager.Instance().StageNumber == 6)
            {
              placi.Position = new TV_3DVECTOR(vader_distX - 5000, -200, 0);
              GameScenarioManager.Instance().SetGameStateB("Stage6VaderEnd", false);
              GameScenarioManager.Instance().SetGameStateB("Stage6VaderAttacking", false);
              GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_Stage06_Vader");
            }
            else
            {
              placi.Position = new TV_3DVECTOR(last_target_distX - 5000, -200, 0);
            }
          }
          else
          {
            placi.Position = new TV_3DVECTOR(0, 200, GameScenarioManager.Instance().MaxBounds.z - 1000);
            placi.Rotation = new TV_3DVECTOR(0, 180, 0);
          }

          ActorInfo ainfo = ActorInfo.Create(placi);
          PlayerInfo.Instance().Actor = ainfo;
          GameScenarioManager.Instance().AllyFighters.Add("(Player)", PlayerInfo.Instance().Actor);
          RegisterEvents(ainfo);
        }
      }
      m_Player = PlayerInfo.Instance().Actor;
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_RemoveTorps");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_StrongerWings");
    }

    public void Rebel_GiveControl(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        ActionManager.Unlock(a);
        a.ActorState = ActorState.NORMAL;
        a.Speed = a.MaxSpeed;
      }
      PlayerInfo.Instance().IsMovementControlsEnabled = true;
      //PlayerInfo.Instance().PlayerAIEnabled = true;

      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
    }

    public void Rebel_Forward(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        ActionManager.ForceClearQueue(a);
        ActionManager.QueueNext(a, new Actions.Rotate(a.GetPosition() + new TV_3DVECTOR(0, 0, -20000), a.MaxSpeed));
        ActionManager.QueueNext(a, new Actions.Lock());
      }
    }

    public void Rebel_Reposition(object[] param)
    {
      int sw = -1;
      float x = 0;
      float y = 0;
      float z = GameScenarioManager.Instance().MaxBounds.z - 150 ;
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        ActionManager.ForceClearQueue(a);
        if (a.Name == "(Player)")
        {
          a.SetLocalPosition(0, 100, GameScenarioManager.Instance().MaxBounds.z - 150);
          a.SetLocalRotation(0, 180, 0);
          a.XTurnAngle = 0;
          a.YTurnAngle = 0;
          a.Speed = a.MaxSpeed;
          ActionManager.QueueNext(a, new Actions.Wait(5));
        }
        else
        {
          x = Engine.Instance().Random.Next(80, 1800) * sw;
          y = Engine.Instance().Random.Next(-40, 100);
          z = GameScenarioManager.Instance().MaxBounds.z + Engine.Instance().Random.Next(-1800, -150);
          sw = -sw;
          a.SetLocalPosition(x, y, z);
          a.SetLocalRotation(0, 180, 0);
          a.XTurnAngle = 0;
          a.YTurnAngle = 0;
          a.Speed = a.MaxSpeed;
          ActionManager.QueueNext(a, new Actions.Wait(5));
        }
      }
      Rebel_RemoveTorps(null);
    }



    #endregion


    #region Empire spawns

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
      GameScenarioManager.Instance().CriticalEnemies.Add("IMPERIAL I STAR DESTROYER" + ainfo.ID, ainfo);
      RegisterEvents(ainfo);
      if (ainfo.GetStateF("TIEspawnRemaining") > 0)
      {
        ainfo.TickEvents.Add("Empire_SDSpawner");
        if (param.GetLength(0) >= 4 && param[3] is int)
        {
          ainfo.SetStateF("TIEspawnRemaining", (int)param[3]);
        }
      }
    }
    
    public void Empire_FirstWave(object[] param)
    {
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-5000, -150, 6000), 12 });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(1500, 100, -4000), new TV_3DVECTOR(3000, 150, 5500), 12 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-6000, -150, 7000), 10 });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(1500, 100, -4000), new TV_3DVECTOR(4000, 100, 5000), 10 });
          break;
        case "normal":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-2500, -150, 7000), 10 });
          break;
        case "easy":
        default:
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-2500, -150, 7000), 15 });
          break;
      }
    }

    public void Empire_SecondWave(object[] param)
    {
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "mental":
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000), 15 });
          Empire_TIEWave(new object[] { 6 });
          break;
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000), 10 });
          Empire_TIEWave(new object[] { 4 });
          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000), 12 });
          Empire_TIEWave(new object[] { 4 });
          break;
      }
      GameScenarioManager.Instance().SetGameStateB("Stage1BBegin", true);
    }

    public void Empire_FirstTIEWave(object[] param)
    {
      int count = 0;
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "mental":
          count = 8;
          break;
        case "hard":
          count = 6;
          break;
        case "normal":
          count = 5;
          break;
        case "easy":
        default:
          count = 4;
          break;
      }

      // TIEs
      ActorCreationInfo aci;
      for (int k = 1; k < count; k++)
      {
        float fx = Engine.Instance().Random.Next(-500, 500);
        float fy = Engine.Instance().Random.Next(-500, 0);
        float fz = Engine.Instance().Random.Next(-2500, 2500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            aci = new ActorCreationInfo(TIE_LN_ATI.Instance());
            aci.CreationTime = Game.Instance().GameTime;
            aci.Faction = FactionInfo.Get("Empire");
            aci.InitialState = ActorState.NORMAL;
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, fz - 4000 - k * 100);
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
                  //a.AI.Orders.Enqueue(new AIElement { AIType = AIType.ATTACK_LOCK_ACTOR, TargetActor = rs });
                }
                break;
            }

            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
          }
        }
      }
    }

    public void Empire_TIEWave(object[] param)
    {
      int sets = 10;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 10;

      // TIE Fighters only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-200, 800);

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

            ActionManager.QueueLast(a, new Actions.Wait(15));
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
          && GameScenarioManager.Instance().EnemyFighters.Count < 52)
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

    public void Empire_Towers01(object[] param)
    {
      float dist = 2500;
      float height = GameScenarioManager.Instance().MinBounds.y;
      Dictionary<string, ActorInfo>[] register = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().EnemyShips, m_GroundObjects };
      Dictionary<string, ActorInfo>[] registerminor = new Dictionary<string, ActorInfo>[] { m_GroundObjects };

      for (int x = -2; x <= 2; x++)
      {
        for (int z = -3; z <= -1; z++)
        {
          SpawnActor(Tower03ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 90 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], register);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist + 300, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist - 300, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 30 + height, z * dist + 300), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 30 + height, z * dist - 300), new TV_3DVECTOR(), new ActionInfo[0], registerminor);
        }
      }
    }

    public void Empire_Towers02(object[] param)
    {
      float dist = 2500;
      float height = GameScenarioManager.Instance().MinBounds.y;
      Dictionary<string, ActorInfo>[] register = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().EnemyShips, m_GroundObjects };
      Dictionary<string, ActorInfo>[] registerminor = new Dictionary<string, ActorInfo>[] { m_GroundObjects };

      for (int x = -3; x <= 3; x += 2)
      {
        for (int z = -2; z <= -1; z++)
        {
          SpawnActor(Tower03ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist / 2, 90 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], register);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist / 2 + 300, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist / 2 - 300, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist / 2, 30 + height, z * dist + 300), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist / 2, 30 + height, z * dist - 300), new TV_3DVECTOR(), new ActionInfo[0], registerminor);
        }
      }
    }

    public void Empire_Towers03(object[] param)
    {
      float dist = 2500;
      float height = GameScenarioManager.Instance().MinBounds.y;
      Dictionary<string, ActorInfo>[] register = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().EnemyShips, m_GroundObjects };
      Dictionary<string, ActorInfo>[] registerminor = new Dictionary<string, ActorInfo>[] { m_GroundObjects };

      for (int z = -3; z <= 0; z++)
      {
        int z0 = (z > 0) ? z : -z;
        for (int x = -z0; x <= z0; x++)
        {
          if (z == -2)
          {
            SpawnActor(Tower01ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 90 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], register);

            SpawnActor(Tower02ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist + 500, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

            SpawnActor(Tower02ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist - 500, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);
          }
          else
          { 
            SpawnActor(Tower03ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 90 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], register);

            SpawnActor(Tower02ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist + 300, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

            SpawnActor(Tower02ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist - 300, 30 + height, z * dist), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

            SpawnActor(Tower02ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 30 + height, z * dist + 300), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

            SpawnActor(Tower02ATI.Instance(), "", "", ""
                     , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 30 + height, z * dist - 300), new TV_3DVECTOR(), new ActionInfo[0], registerminor);
          }
        }
      }
    }

    public void Empire_Towers04(object[] param)
    {
      float dist = 1000;
      float height = -175;
      Dictionary<string, ActorInfo>[] register = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().EnemyShips, m_GroundObjects };
      Dictionary<string, ActorInfo>[] registerminor = new Dictionary<string, ActorInfo>[] { m_GroundObjects };

      for (int x = -6; x <= 6; x++)
      {
        SpawnActor(Tower02ATI.Instance(), "", "", ""
                 , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist + 500, 30 + height, -150), new TV_3DVECTOR(), new ActionInfo[0], registerminor);

        SpawnActor(Tower02ATI.Instance(), "", "", ""
                 , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist - 500, 30 + height, 150), new TV_3DVECTOR(), new ActionInfo[0], registerminor);
      }
    }

    #endregion

    #region Scene

    public void Scene_EnterCutscene(object[] param)
    {
      if (m_Player != null)
      {
        m_Player_PrimaryWeapon = PlayerInfo.Instance().PrimaryWeapon;
        m_Player_SecondaryWeapon = PlayerInfo.Instance().SecondaryWeapon;
        m_Player_DamageModifier = m_Player.DamageModifier;
        m_Player.DamageModifier = 0;
        ActionManager.ForceClearQueue(m_Player);
        ActionManager.QueueNext(m_Player, new Actions.Lock());
      }
      PlayerInfo.Instance().Actor = GameScenarioManager.Instance().SceneCamera;
      
      GameScenarioManager.Instance().IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(object[] param)
    {
      if (m_Player != null)
      {
        PlayerInfo.Instance().Actor = m_Player;
        PlayerInfo.Instance().PrimaryWeapon = m_Player_PrimaryWeapon;
        PlayerInfo.Instance().SecondaryWeapon = m_Player_SecondaryWeapon;
        m_Player.DamageModifier = m_Player_DamageModifier;
        ActionManager.ForceClearQueue(m_Player);
      }
      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam(object[] param)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_EnterCutscene");
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(1000, 300, -15000);
      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 600;
      GameScenarioManager.Instance().SceneCamera.Speed = 600;
      GameScenarioManager.Instance().CameraTargetActor = m_ADS;
    }


    public void Scene_Stage02_ApproachDeathStar(object[] param)
    {
      Scene_DeathStarCam(null);
      SoundManager.Instance().SetMusic("battle_1_1", false, 71250);

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 4.8f, "Rebel_Reposition");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Scene_Stage02_Spawn");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 9f, "Scene_ExitCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 11f, "Message.04");

      foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
      {
        ainfo.Strength = ainfo.MaxStrength;
      }

      foreach (ActorInfo ainfo in GameScenarioManager.Instance().EnemyShips.Values)
      {
        ainfo.Destroy();
      }

      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(10000, 400, 8000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-10000, -175, -12000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(8000, 400, 8000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-8000, -160, -10000);
    }

    public void Scene_ClearGroundObjects(object[] param)
    {
      foreach (ActorInfo a in m_GroundObjects.Values)
      {
        if (a.CreationState != CreationState.DISPOSED)
          a.Destroy();
      }
      m_GroundObjects.Clear();
    }

    public void Scene_Stage02_Spawn(object[] param)
    {
      GameScenarioManager.Instance().StageNumber = 2;
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(-550, -130, GameScenarioManager.Instance().MaxBounds.z - 1500);

      m_ADS_Surface = SpawnActor(Surface003_00ATI.Instance(), "surfacebig", "", ""
                      , 0, FactionInfo.Neutral, new TV_3DVECTOR(0, -175, 0), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

      
      for (int x = -5 ; x <= 5; x++ )
        for (int z = -5; z <= 5; z++)
          m_ADS_SurfaceParts.Add(SpawnActor(((x + z) % 2 == 1) ? (ActorTypeInfo)Surface001_00ATI.Instance() : Surface001_01ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Neutral, new TV_3DVECTOR(x * 4000, -173, z * 4000), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]));
      

      m_ADS.Destroy();
      m_AYavin.SetRotation(0, 0, 180);
      m_AYavin4.Destroy();
      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 450;
      GameScenarioManager.Instance().SceneCamera.Speed = 450;
      GameScenarioManager.Instance().CameraTargetActor = m_Player;

      //Empire_TIEWave(null);
      Empire_Towers01(null);
    }

    public void Scene_Stage03_Spawn(object[] param)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_EnterCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_Reposition");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Scene_ExitCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 6.5f, "Message.06");


      GameScenarioManager.Instance().StageNumber = 3;
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(600, 130, GameScenarioManager.Instance().MaxBounds.z - 1000);

      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 450;
      GameScenarioManager.Instance().SceneCamera.Speed = 450;
      GameScenarioManager.Instance().CameraTargetActor = m_Player;

      foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
      {
        ainfo.Strength += 0.35f * ainfo.MaxStrength;
      }

      Scene_ClearGroundObjects(null);
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "easy":
          Empire_TIEWave(new object[] { 4 });
          break;
        case "mental":
          Empire_TIEWave(new object[] { 8 });
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 43f, "Empire_TIEWave");
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 45f, "Message.06");
          break;
        case "hard":
          Empire_TIEWave(new object[] { 8 });
          break;
        case "normal":
        default:
          Empire_TIEWave(new object[] { 6 });
          break;
      }
      Empire_Towers02(null);
    }

    public void Scene_Stage04_Spawn(object[] param)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_EnterCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_Reposition");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Scene_ExitCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 6.5f, "Message.08");

      //m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(2000, 750, -9000), new TV_3DVECTOR(-5000, 1250, 6000), 20 });

      ActorCreationInfo acinfo;
      ActorInfo ainfo;
      ActorTypeInfo type = ImperialIATI.Instance();
      TV_3DVECTOR position = new TV_3DVECTOR(2000, 750, -8000);
      TV_3DVECTOR targetposition = new TV_3DVECTOR(-4000, 1050, 1000);
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
      ActionManager.QueueLast(ainfo, new Actions.HyperspaceOut());
      ActionManager.QueueLast(ainfo, new Actions.Delete());

      GameScenarioManager.Instance().EnemyShips.Add(ainfo.Key, ainfo);
      //GameScenarioManager.Instance().CriticalEnemies.Add(ainfo.Name.ToUpper() + "    " + ainfo.ID, ainfo);
      RegisterEvents(ainfo);
      ainfo.TickEvents.Add("Empire_SDSpawner");
      //ainfo.SetStateF("TIEspawnRemaining", 24);
      
      GameScenarioManager.Instance().StageNumber = 4;
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(1000, 30, -2000);

      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 750;
      GameScenarioManager.Instance().SceneCamera.Speed = 750;
      GameScenarioManager.Instance().CameraTargetActor = ainfo;

      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        a.Strength += 0.35f * a.MaxStrength;
      }

      Scene_ClearGroundObjects(null);
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "easy":
          ainfo.SetStateF("TIEspawnRemaining", 12);
          break;
        case "mental":
          //Empire_TIEWave(new object[] { 4 });
          ainfo.SetStateF("TIEspawnRemaining", 20);
          //GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 73f, "Empire_TIEWave");
          //GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 75f, "Message.06");
          break;
        case "hard":
          //Empire_TIEWave(new object[] { 2 });
          ainfo.SetStateF("TIEspawnRemaining", 16);
          break;
        case "normal":
        default:
          ainfo.SetStateF("TIEspawnRemaining", 14);
          break;
      }
      Empire_Towers03(null);
    }

    public void Scene_Stage05_Spawn(object[] param)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_EnterCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_Reposition");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Scene_ExitCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 6.5f, "Message.10");

      GameScenarioManager.Instance().StageNumber = 5;
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(550, -130, -1500);
      SoundManager.Instance().SetMusic("battle_1_2", true);

      foreach (ActorInfo a in m_ADS_SurfaceParts)
      {
        a.Destroy();
      }
      m_ADS_SurfaceParts.Clear();

      for (int x = -5; x <= 5; x++)
        for (int z = 0; z <= 3; z++)
        {
          m_ADS_SurfaceParts.Add(SpawnActor(((x + z) % 2 == 1) ? (ActorTypeInfo)Surface001_00ATI.Instance() : Surface001_01ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Neutral, new TV_3DVECTOR(x * 4000, -173, 2250 + z * 4000), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]));

          m_ADS_SurfaceParts.Add(SpawnActor(((x + -z) % 2 == 1) ? (ActorTypeInfo)Surface001_00ATI.Instance() : Surface001_01ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Neutral, new TV_3DVECTOR(x * 4000, -173, -2250 + -z * 4000), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]));
        }

      for (int x = -20; x <= 20; x++)
      {
        m_ADS_SurfaceParts.Add(SpawnActor((ActorTypeInfo)Surface002_99ATI.Instance(), "", "", ""
         , 0, FactionInfo.Neutral, new TV_3DVECTOR(x * 1000, -175, 0), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]));
      }

      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(7500, 300, 8000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-7500, -400, -12000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(7000, 300, 8000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-7000, -160, -10000);

      m_ADS_Surface.Destroy();
      Scene_ClearGroundObjects(null);

      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 450;
      GameScenarioManager.Instance().SceneCamera.Speed = 450;
      GameScenarioManager.Instance().CameraTargetActor = m_Player;

      Empire_Towers04(null);
    }

    public void Scene_Stage05b_Spawn(object[] param)
    {
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(5000000, -185, 1000);
      //GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(vader_distX - 1000, -400, -1000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(7000, -400, -1000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(500000, 300, 2000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(0, -400, -2000);
      
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        if (!a.IsPlayer())
        {
          a.ActorState = ActorState.FIXED;
          //a.Destroy();
        }
      }
      //m_Player.SetLocalPosition(7050, m_Player.GetLocalPosition())

      foreach (ActorInfo a in m_ADS_SurfaceParts)
      {
        a.Destroy();
      }
      m_ADS_SurfaceParts.Clear();
      Scene_ClearGroundObjects(null);
      Rebel_RemoveTorps(null);

      Scene_Stage05b_SpawnRoute(null);
    }

    public void Scene_Stage05b_SpawnRoute(object[] param)
    {


      ActorTypeInfo[] randtrenchtypes = new ActorTypeInfo[] { Surface002_00ATI.Instance()
                                                            , Surface002_01ATI.Instance()
                                                            , Surface002_02ATI.Instance()
                                                            , Surface002_03ATI.Instance()
                                                            , Surface002_04ATI.Instance()
                                                            , Surface002_05ATI.Instance()
                                                            , Surface002_06ATI.Instance()
                                                            , Surface002_07ATI.Instance()
                                                            , Surface002_08ATI.Instance()
                                                            , Surface002_09ATI.Instance()
                                                            , Surface002_10ATI.Instance()
                                                            , Surface002_11ATI.Instance()
                                                            };

      SpawnActor(SurfaceVentATI.Instance(), "", "", ""
         , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(target_distX + 500, -385 + 47, 0), new TV_3DVECTOR(0, 180, 0), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "mental":
           Trenches = new int[] { 13, 13, 13, 13, 13, -11, -11, 0, 0, 0
                                       , -11, 0, 0, -11, -11, 0, 0, -11, 0, 0
                                       , -11, -11, 0, -11, 0, -11, -11, 0, -11, -11
                                       , -11, 0, -11, -11, -11, -11, -11, -11, 0, -11
                                       , -11, -11, -11, -11, -11, 0, -11, -11, 0, -11
                                       , -11, 0, 0, 0, -11, -11, -11, -11, 0, -11
                                       , -11, 0, -11, -11, -11, 0, -11, -11, 0, 0
                                       , -11, 0, -11, -11, -11, 0, 0, -11, -11, 11
                                       , 11, 13, 13, 13, 13, 13, 13, 13, 13, 13
                                       , 13, 13, 13, 13, 13, 13, 13, 13, 13
                                       , 13, 13, 13, 13, 13, 13, 13, 13, 13
                                       , 13, 13, 13, 13, 12
                                       };
          break;
      }
    }

    ActorTypeInfo[] TrenchTypes = new ActorTypeInfo[] { Surface002_00ATI.Instance()
                                                        , Surface002_01ATI.Instance()
                                                        , Surface002_02ATI.Instance()
                                                        , Surface002_03ATI.Instance()
                                                        , Surface002_04ATI.Instance()
                                                        , Surface002_05ATI.Instance()
                                                        , Surface002_06ATI.Instance()
                                                        , Surface002_07ATI.Instance()
                                                        , Surface002_08ATI.Instance()
                                                        , Surface002_09ATI.Instance()
                                                        , Surface002_10ATI.Instance()
                                                        , Surface002_11ATI.Instance()
                                                        , Surface002_12ATI.Instance()
                                                        , Surface002_99ATI.Instance()
                                                        };
    
    private int[] Trenches = new int[] { 13, 13, 13, 13, 13, 0, 0, 0, 0, 0
                                       , 1, 0, 0, 2, 1, 0, 0, 2, 0, 0
                                       , 3, 1, 0, 4, 0, 3, 5, 0, 4, 1
                                       , 6, 0, 4, 2, 1, 7, 3, 2, 0, 6
                                       , 2, 3, 8, 7, 1, 0, 9, 2, 0, 5
                                       , 1, 0, 0, 0, 4, 6, 3, 2, 0, 5
                                       , 1, 0, 2, 4, 5, 0, 1, 9, 0, 0
                                       , 10, 0, 1, 3, 11, 0, 0, 5, 6, 2
                                       , 1, 13, 13, 13, 13, 13, 13, 13, 13, 13
                                       , 13, 13, 13, 13, 13, 13, 13, 13, 13
                                       , 13, 13, 13, 13, 13, 13, 13, 13, 13
                                       , 13, 13, 13, 13, 12
                                       };
    
    public void Scene_Stage05b_ContinuouslySpawnRoute(object[] param)
    {
      // x_position = 7000 + counter * 1000
      // counter = (int)(x_position / 1000 - 7)

      int counter = (int)(PlayerInfo.Instance().Position.x - 8000) / 1000 - 7;
      if (counter < 0)
        counter = 0;

      int lasttravelledcounter = (int)(last_target_distX - 2000) / 1000 - 7;
      if (lasttravelledcounter < 0)
        lasttravelledcounter = 0;

      for (int i = counter - 4; i < counter + 20; i++)
      {
        ActorInfo a = m_ADS_TrenchParts.GetItem(i);
        if (a != null)
        {
          if (i < lasttravelledcounter && GameScenarioManager.Instance().StageNumber == 5)
          {
            if (!(m_ADS_TrenchParts.GetItem(i).TypeInfo is Surface002_00ATI) || i < counter)
            {
              m_ADS_TrenchParts.GetItem(i).Destroy();
              m_ADS_TrenchParts.AddorUpdateItem(i, null);
              //m_ADS_TrenchParts.AddorUpdateItem(i, SpawnActor(TrenchTypes[Trenches[0]], "", "", ""
              //     , 0, FactionInfo.Neutral, new TV_3DVECTOR(7000 + i * 1000, -173, 0), new TV_3DVECTOR(0, 180, 0), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]));
            }
          }
        }

        a = m_ADS_TrenchParts.GetItem(i);
        if (a == null)
        {
          if (i < lasttravelledcounter)
          {
            m_ADS_TrenchParts.AddorUpdateItem(i, SpawnActor(TrenchTypes[Trenches[0]], "", "", ""
                               , 0, FactionInfo.Neutral, new TV_3DVECTOR(7000 + i * 1000, -173, 0), new TV_3DVECTOR(0, 180, 0), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]));
          }
          else if (i < Trenches.Length)
          {
            int trench = Trenches[i];
            if (trench < 0)
              trench = Engine.Instance().Random.Next(0, -trench + 1);

            m_ADS_TrenchParts.AddorUpdateItem(i, SpawnActor(TrenchTypes[trench], "", "", ""
                               , 0, FactionInfo.Neutral, new TV_3DVECTOR(7000 + i * 1000, -173, 0), new TV_3DVECTOR(0, 180, 0), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]));

            if (i < 100 && i > 0 && i % 35 == 0)
            {
              SpawnActor(Tower01ATI.Instance(), "", "", ""
                  , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 175), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

              SpawnActor(Tower01ATI.Instance(), "", "", ""
                  , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, -175), new TV_3DVECTOR(0, 180, 0), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
            }
            else if (i > 10 && i < 96 && i % 10 == 0 || (i > 50 && i < 55))
            {
              SpawnActor(Tower02ATI.Instance(), "", "", ""
                  , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000, 30 - 175 , 150), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

              SpawnActor(Tower02ATI.Instance(), "", "", ""
                  , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000, 30 - 175, -150), new TV_3DVECTOR(0, 180, 0), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
            }
            else if (i == 100)
            {
              SpawnActor(Tower03ATI.Instance(), "", "", ""
                  , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 150), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

              SpawnActor(Tower03ATI.Instance(), "", "", ""
                  , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, -150), new TV_3DVECTOR(0, 180, 0), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
            }

            switch (GameScenarioManager.Instance().Difficulty.ToLower())
            {
              case "hard":
              case "mental":
                if (Trenches[i] == 1)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    SpawnActor(Tower01ATI.Instance(), "", "", ""
                           , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
                  }
                  else if (i % 3 < 2)
                  {
                      SpawnActor(Tower02ATI.Instance(), "", "", ""
                             , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000 - 140, 30 - 390, 40), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
                  }
                  else if (i % 4 < 1)
                  {
                    SpawnActor(Tower03ATI.Instance(), "", "", ""
                            , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
                  }
                }
                if (Trenches[i] == 2)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    SpawnActor(Tower01ATI.Instance(), "", "", ""
                           , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
                  }
                  else if (i % 3 < 2)
                  {
                    SpawnActor(Tower02ATI.Instance(), "", "", ""
                           , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000 - 40, 30 - 390, -40), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
                  }
                  else if (i % 4 < 1)
                  {
                    SpawnActor(Tower03ATI.Instance(), "", "", ""
                           , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
                  }
                }
                break;
              case "normal":
              case "easy":
              default:

                break;
            }
          }
        }
      }
    }

    public void Scene_Stage06_SetPlayer(object[] param)
    {
      m_Player.SetLocalPosition(m_Player.GetPosition().x, -220, 0);
      m_Player.SetRotation(0, 90, 0);
      m_Player.XTurnAngle = 0;
      m_Player.YTurnAngle = 0;
    }

    public void Scene_Stage06_Vader(object[] param)
    {
      Scene_EnterCutscene(null);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7.9f, "Scene_Stage06_SetPlayer");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, "Scene_ExitCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, "Scene_Stage06_VaderAttack");

      GameScenarioManager.Instance().StageNumber = 6;
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(vader_distX - 2750, -225, 0);
      SoundManager.Instance().SetMusic("battle_1_3", true);
      m_Player.SetLocalPosition(vader_distX, -220, 0);
      m_Player.SetRotation(0, 90, 0);
      m_Player.XTurnAngle = 0;
      m_Player.YTurnAngle = 0;
      ActionManager.ForceClearQueue(m_Player);
      ActionManager.QueueNext(m_Player, new Actions.Lock());

      Scene_ClearGroundObjects(null);

      if (m_Vader != null)
      { m_Vader.Destroy(); }

      if (m_Falcon != null)
      { m_Falcon.Destroy(); }

      if (m_VaderEscort1 != null)
      { m_VaderEscort1.Destroy(); }

      if (m_VaderEscort2 != null)
      { m_VaderEscort2.Destroy(); }

      m_Vader = SpawnActor(TIE_X1_ATI.Instance(), "", "", ""
                          , 0, FactionInfo.Get("Empire"), new TV_3DVECTOR(vader_distX - 2000, 85, 0), new TV_3DVECTOR(-10, 90, 0)
                          , new ActionInfo[] { new Actions.Move(new TV_3DVECTOR(vader_distX + 2000, -250, 0), 400)
                                             , new Actions.Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, 0), 400)
                                             , new Actions.Wait(3) 
                                             , new Actions.AttackActor(m_Player, 1500, 1, false, 9999) }
                          , new Dictionary<string, ActorInfo>[0]);

      m_VaderEscort1 = SpawnActor(TIE_LN_ATI.Instance(), "", "", ""
                    , 0, FactionInfo.Get("Empire"), new TV_3DVECTOR(vader_distX - 2100, 85, 75), new TV_3DVECTOR(-10, 90, 0)
                    , new ActionInfo[] { new Actions.Move(new TV_3DVECTOR(vader_distX + 2000, -250, 75), 400)
                                       , new Actions.Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, 75), 400)
                                       , new Actions.Lock() }
                    , new Dictionary<string, ActorInfo>[0]);

      m_VaderEscort2 = SpawnActor(TIE_LN_ATI.Instance(), "", "", ""
                    , 0, FactionInfo.Get("Empire"), new TV_3DVECTOR(vader_distX - 2100, 85, -75), new TV_3DVECTOR(-10, 90, 0)
                    , new ActionInfo[] { new Actions.Move(new TV_3DVECTOR(vader_distX + 2000, -250, -75), 400)
                                       , new Actions.Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, -75), 400)
                                       , new Actions.Lock() }
                    , new Dictionary<string, ActorInfo>[0]);

      m_Vader.Weapons = new Dictionary<string, WeaponInfo>{ {"lsrb", new TIE_LN_DblLaserWeapon() }
                                                        , {"laser", new TIE_IN_DblLaserWeapon() }
                                                        };
      m_Vader.AIWeapons = new List<string> { "1:laser", "1:lsrb" };
      m_Vader.MaxSpeed = 400;
      m_Vader.MinSpeed = 400;
      m_Vader.CanEvade = false;
      m_Vader.CanRetaliate = false;
      m_VaderEscort1.MaxSpeed = 400;
      m_VaderEscort1.MinSpeed = 400;
      m_VaderEscort1.CanEvade = false;
      m_VaderEscort1.CanRetaliate = false;
      m_VaderEscort2.MaxSpeed = 400;
      m_VaderEscort2.MinSpeed = 400;
      m_VaderEscort2.CanEvade = false;
      m_VaderEscort2.CanRetaliate = false;
      m_Player.CanEvade = false;
      m_Player.CanRetaliate = false;

      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 425;
      GameScenarioManager.Instance().SceneCamera.Speed = 425;
      GameScenarioManager.Instance().CameraTargetActor = m_Player;
    }

    public void Scene_Stage06_VaderAttack(object[] param)
    {
      GameScenarioManager.Instance().SetGameStateB("Stage6VaderAttacking", true);

      if (m_Falcon != null)
      { m_Falcon.Destroy(); }

      if (m_Vader != null)
      {
        ActionManager.ForceClearQueue(m_Vader);
        ActionManager.QueueNext(m_Vader, new Actions.AttackActor(m_Player, -1, -1, false, 9999));
        ActionManager.QueueNext(m_Vader, new Actions.AttackActor(m_Player, -1, -1, false, 9999));
        ActionManager.QueueNext(m_Vader, new Actions.Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_Vader, new Actions.Lock());
      }

      if (m_VaderEscort1 != null)
      {
        ActionManager.ForceClearQueue(m_VaderEscort1);
        ActionManager.QueueNext(m_VaderEscort1, new Actions.AttackActor(m_Player, -1, -1, false, 9999));
        ActionManager.QueueNext(m_VaderEscort1, new Actions.AttackActor(m_Player, -1, -1, false, 9999));
        ActionManager.QueueNext(m_VaderEscort1, new Actions.Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_VaderEscort1, new Actions.Lock());
      }

      if (m_VaderEscort2 != null)
      {
        ActionManager.ForceClearQueue(m_VaderEscort2);
        ActionManager.QueueNext(m_VaderEscort2, new Actions.AttackActor(m_Player, -1, -1, false, 9999));
        ActionManager.QueueNext(m_VaderEscort2, new Actions.AttackActor(m_Player, -1, -1, false, 9999));
        ActionManager.QueueNext(m_VaderEscort2, new Actions.Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_VaderEscort2, new Actions.Lock());
      }
    }

    public void Scene_Stage06_VaderEnd(object[] param)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_EnterCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 8f, "Scene_ExitCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7.9f, "Scene_Stage06_SetPlayer");

      GameScenarioManager.Instance().StageNumber = 6;
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(vaderend_distX + 900, -365, 0);
      SoundManager.Instance().SetMusic("ds_end_1_1", true);
      m_Player.SetLocalPosition(vaderend_distX, -220, 0);
      m_Player.SetRotation(0, 90, 0);
      m_Player.XTurnAngle = 0;
      m_Player.YTurnAngle = 0;
      m_Vader.SetRotation(0, 90, 0);
      ActionManager.ForceClearQueue(m_Player);
      ActionManager.QueueNext(m_Player, new Actions.Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      ActionManager.QueueNext(m_Player, new Actions.Lock());


      m_Falcon = SpawnActor(FalconATI.Instance(), "", "", ""
                    , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(vaderend_distX + 2500, 185, 0), new TV_3DVECTOR(0, -90, 0)
                    , new ActionInfo[] { new Actions.Move(new TV_3DVECTOR(vaderend_distX + 1300, 5, 0), 500, -1, false)
                                       , new Actions.AttackActor(m_VaderEscort1, -1, -1, false, 9999)
                                       , new Actions.AttackActor(m_VaderEscort2, -1, -1, false, 9999)
                                       , new Actions.Move(new TV_3DVECTOR(vaderend_distX - 5300, 315, 0), 500, -1, false)
                                       , new Actions.Delete()
                                       }
                    , new Dictionary<string, ActorInfo>[0]);
      m_Falcon.CanEvade = false;
      m_Falcon.CanRetaliate = false;

      if (m_Vader != null)
      {
        ActionManager.ForceClearQueue(m_Vader);
        ActionManager.QueueNext(m_Vader, new Actions.Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_Vader, new Actions.Lock());
      }

      if (m_VaderEscort1 != null)
      {
        ActionManager.ForceClearQueue(m_VaderEscort1);
        ActionManager.QueueNext(m_VaderEscort1, new Actions.Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_VaderEscort1, new Actions.Lock());
      }

      if (m_VaderEscort2 != null)
      {
        ActionManager.ForceClearQueue(m_VaderEscort2);
        ActionManager.QueueNext(m_VaderEscort2, new Actions.Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_VaderEscort2, new Actions.Lock());
      }

      m_VaderEscort1.HitEvents.Add("Scene_Stage06_VaderFlee");

      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 25;
      GameScenarioManager.Instance().SceneCamera.Speed = 25;
      GameScenarioManager.Instance().CameraTargetActor = m_Falcon;
    }

    public void Scene_Stage06_VaderFlee(object[] param)
    {
      if (GameScenarioManager.Instance().GetGameStateB("Stage6VaderAttacking"))
      {
        GameScenarioManager.Instance().SetGameStateB("Stage6VaderAttacking", false);

        ActionManager.ForceClearQueue(m_Vader);
        m_Vader.ApplyZBalance = false;
        m_Vader.SetRotation(-30, 85, 5);
        //m_Vader.MaxSpeed = 800;
        m_Vader.TimedLife = 999;
        m_Vader.ActorState = ActorState.DYING;
        m_VaderEscort2.SetRotation(-5, 93, 0);
        m_VaderEscort2.ActorState = ActorState.DYING;
        m_VaderEscort1.ActorState = ActorState.DYING;
        //ActionManager.QueueNext(m_Vader, new Actions.Rotate(new TV_3DVECTOR(vaderend_distX + 7500, 750, 1), 200, 1, true));
        //ActionManager.QueueNext(m_Vader, new Actions.Wait(2.5f));
        //ActionManager.QueueNext(m_Vader, new Actions.Move(new TV_3DVECTOR(vaderend_distX - 6500, 500, 100), 400, -1, false));
        //ActionManager.QueueNext(m_Vader, new Actions.Lock());
        //ActionManager.QueueNext(m_Vader, new Actions.Wait(2));
        //ActionManager.QueueNext(m_Vader, new Actions.SelfDestruct());
      }
    }

    #endregion


    #region Text
    public void Message_01_EnemyShipsInbound(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Enemy signals are approaching your position.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_MoreEnemyShipsInbound(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Another enemy signal are en route to your position.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Clear(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Area cleared. Resume your attack on the Death Star.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_04_Target(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Eliminate all Radar Towers.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_05_Clear(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Area cleared. Proceed to the next sector.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_06_TIE(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Enemy fighters detected.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07_Clear(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Deflector Towers detected en route to the trench.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08_Target(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Destroy all Deflection Towers and Radar Towers.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_09_Clear(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: The path to the trench line is clear.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_10_BeginRun(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Luke, start your attack run.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_100_RebelBaseInRange(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is in range of the Death Star.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_101_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in ONE minute.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_102_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWO minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_103_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THREE minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_105_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIVE minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_110_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TEN minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_115_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIFTEEN minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_120_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_125_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY-FIVE minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_130_RebelBase(object[] param)
    {
      Screen2D.Instance().UpdateText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THIRTY minutes.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }
        
    #endregion

  }
}
