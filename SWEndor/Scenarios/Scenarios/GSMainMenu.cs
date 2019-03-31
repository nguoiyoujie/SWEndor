﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Sound;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSMainMenu : GameScenarioBase
  {
    public GSMainMenu()
    {
      Name = "Main Menu";
    }

    private ActorInfo m_APlanet = null;
    private int sceneid = Engine.Instance().Random.Next(0, 7);

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);

      LandInfo.Instance().Enabled = false;
      AtmosphereInfo.Instance().LoadDefaults(true, true);
      AtmosphereInfo.Instance().Enabled = true;
      AtmosphereInfo.Instance().ShowSun = true;
      AtmosphereInfo.Instance().ShowFlare = true;
      AtmosphereInfo.Instance().SetPos_Sun(new TV_3DVECTOR(-1000, 270, 0));
    }

    public override void Launch()
    {
      base.Launch();
      if (GameScenarioManager.Instance().GetGameStateB("in_menu"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_menu", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(15000, 1500, 5000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-15000, -1500, -5000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(15000, 1500, 5000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-15000, -1500, -5000);

      PlayerCameraInfo.Instance().CameraMode = CameraMode.FIRSTPERSON;

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, "Empire_Towers01");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_HyperspaceIn");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.5f, "Rebel_BeginBattle");

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7f, "Empire_StarDestroyer_01");

      PlayerInfo.Instance().Lives = 2;
      PlayerInfo.Instance().ScorePerLife = 9999999;
      PlayerInfo.Instance().ScoreForNextLife = 9999999;
      PlayerInfo.Instance().Score.Reset();

      //SoundManager.Instance().SetMusic("battle_1_1", false, 18600);
      //SoundManager.Instance().SetMusic("battle_1_1", false, 71250);
      //SoundManager.Instance().SetMusic("battle_1_1", false, 191000);
      //SoundManager.Instance().SetMusicLoop("battle_3_4");

      switch (sceneid)
      {
        // Endor reportorie
        case 0:
        default:
          SoundManager.Instance().SetMusic("credits_3_1");
          SoundManager.Instance().SetMusicLoop("battle_3_4");
          break;
        case 1:
          SoundManager.Instance().SetMusic("credits_3_1");
          SoundManager.Instance().SetMusicLoop("battle_3_3");
          break;
        case 2:
          SoundManager.Instance().SetMusic("credits_3_1");
          SoundManager.Instance().SetMusicLoop("battle_3_4");
          break;
        // Yavin reportorie
        case 3:
          SoundManager.Instance().SetMusic("credits_1_1");
          SoundManager.Instance().SetMusicLoop("battle_1_1", 18600);
          break;
        case 4:
          SoundManager.Instance().SetMusic("credits_1_1");
          SoundManager.Instance().SetMusicLoop("battle_1_4");
          break;
        // Hoth reportorie
        case 5:
          SoundManager.Instance().SetMusic("credits_2_1");
          SoundManager.Instance().SetMusicLoop("battle_2_1");
          break;
        case 6:
          SoundManager.Instance().SetMusic("credits_2_1");
          SoundManager.Instance().SetMusicLoop("battle_2_2");
          break;
      }

      GameScenarioManager.Instance().IsCutsceneMode = true;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.AddFaction("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      MainAllyFaction = FactionInfo.Get("Rebels");
      MainEnemyFaction = FactionInfo.Get("Empire");

      MainAllyFaction.WingSpawnLimit = 12;
      MainEnemyFaction.WingSpawnLimit = 28;
    }

    public override void LoadScene()
    {
      base.LoadScene();

      ActorCreationInfo acinfo = null;

      // Create Planet
      if (m_APlanet == null)
      {
        if (sceneid <= 2)
        {
          acinfo = new ActorCreationInfo(EndorATI.Instance());
        }
        else if (sceneid <= 4)
        {
          acinfo = new ActorCreationInfo(YavinATI.Instance());
        }
        else
        {
          acinfo = new ActorCreationInfo(HothATI.Instance());
        }

        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, -1200, 0);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        acinfo.InitialScale = new TV_3DVECTOR(6, 6, 6);
        m_APlanet = ActorInfo.Create(acinfo);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      if (GameScenarioManager.Instance().CameraTargetActor == null || GameScenarioManager.Instance().CameraTargetActor.CreationState != CreationState.ACTIVE)
      {
        List<ActorInfo> list = new List<ActorInfo>(MainAllyFaction.GetWings());
        if (list.Count > 0)
        {
          GameScenarioManager.Instance().CameraTargetActor = list[Engine.Instance().Random.Next(0, list.Count)];
        }
      }

      if (GameScenarioManager.Instance().CameraTargetActor != null && GameScenarioManager.Instance().CameraTargetActor.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR pos = GameScenarioManager.Instance().CameraTargetActor.GetRelativePositionFUR(-300, 0, 0);
        GameScenarioManager.Instance().SceneCamera.SetLocalPosition(pos.x, pos.y + 125, pos.z);
        GameScenarioManager.Instance().CameraTargetActor.TypeInfo.ChaseCamera(GameScenarioManager.Instance().SceneCamera);
        //GameScenarioManager.Instance().CameraTargetActor.TypeInfo.ChaseCamera(GameScenarioManager.Instance().CameraTargetActor);
      }

      if (GameScenarioManager.Instance().GetGameStateB("in_battle"))
      {
        // TIE spawn
        if (TIESpawnTime < Game.Instance().GameTime)
        {
          if (MainEnemyFaction.GetWings().Count < 30)
          {
            TIESpawnTime = Game.Instance().GameTime + 10f;
            Empire_TIEWave_02(new object[] { 4 });
          }
        }

        // Rebel spawn
        if (RebelSpawnTime < Game.Instance().GameTime)
        {
          if (MainAllyFaction.GetWings().Count < 10)
          {
            RebelSpawnTime = Game.Instance().GameTime + 10f;
            Rebel_Wave(new object[] { 15 });
          }
        }

        if (MainAllyFaction.GetShips().Count < 3 && !GameScenarioManager.Instance().GetGameStateB("rebels_fled"))
        {
          GameScenarioManager.Instance().SetGameStateB("rebels_fled", true);
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, "Rebel_HyperspaceIn2");
          Rebel_HyperspaceOut(null);
        }
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_APlanet != null && m_APlanet.CreationState == CreationState.ACTIVE)
      {
        float x_en = PlayerInfo.Instance().Position.x / 1.2f;
        float y_en = (PlayerInfo.Instance().Position.y > 0) ? (PlayerInfo.Instance().Position.y / 6f) - 9000.0f : (PlayerInfo.Instance().Position.y / 2.5f) - 9000.0f;
        float z_en = PlayerInfo.Instance().Position.z / 1.2f;
        m_APlanet.SetLocalPosition(x_en, y_en, z_en);
      }
    }

    public override void RegisterEvents()
    {
      base.RegisterEvents();
      GameEvent.RegisterEvent("Rebel_HyperspaceIn", Rebel_HyperspaceIn);
      GameEvent.RegisterEvent("Rebel_HyperspaceIn2", Rebel_HyperspaceIn2);
      GameEvent.RegisterEvent("Rebel_HyperspaceOut", Rebel_HyperspaceOut);
      GameEvent.RegisterEvent("Rebel_BeginBattle", Rebel_BeginBattle);
      GameEvent.RegisterEvent("Rebel_Wave", Rebel_Wave);

      GameEvent.RegisterEvent("Empire_TIEWave_02", Empire_TIEWave_02);
      GameEvent.RegisterEvent("Empire_StarDestroyer_01", Empire_StarDestroyer_01);
      GameEvent.RegisterEvent("Empire_Towers01", Empire_Towers01);
    }

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      float creationTime = Game.Instance().GameTime;

      // Wings x16
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      for (int i = 0; i < 16; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Engine.Instance().Random.Next(-800, -80), Engine.Instance().Random.Next(-100, 100), Engine.Instance().Random.Next(-800, -150)));
        else
          positions.Add(new TV_3DVECTOR(Engine.Instance().Random.Next(80, 800), Engine.Instance().Random.Next(-100, 100), Engine.Instance().Random.Next(-800, -150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ActorTypeInfo[] atypes = new ActorTypeInfo[] { XWingATI.Instance()
                                                      , XWingATI.Instance()
                                                      , XWingATI.Instance()
                                                      , AWingATI.Instance()
                                                      , AWingATI.Instance()
                                                      , YWingATI.Instance()
                                                      , YWingATI.Instance()
                                                      , BWingATI.Instance()
                                                      , BWingATI.Instance() };

        ActorTypeInfo at = atypes[Engine.Instance().Random.Next(0, atypes.Length)];
        ainfo = SpawnActor(at
                          , ""
                          , ""
                          , ""
                          , creationTime
                          , MainAllyFaction
                          , v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z)
                          , new TV_3DVECTOR(0, 180, 0)
                          , new ActionInfo[] { new Move(new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 750)
                                                      , at.MaxSpeed)
                                              }
                           );
              }

      // Nebulon x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-100, 220, 1200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = NebulonBATI.Instance();
        ainfo = SpawnActor(at
                        , ""
                        , ""
                        , ""
                        , creationTime
                        , MainAllyFaction
                        , v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z)
                        , new TV_3DVECTOR(0, 180, 0)
                        , new ActionInfo[] { new Move(nv, at.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MinSpeed)
                                           , new Lock()
                                            }
                         );
      }

      // Corellian x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(300, 370, -3200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = CorellianATI.Instance();
        ainfo = SpawnActor(at
                        , ""
                        , ""
                        , ""
                        , creationTime
                        , MainAllyFaction
                        , v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z)
                        , new TV_3DVECTOR(0, 180, 0)
                        , new ActionInfo[] { new Move(nv, at.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MinSpeed)
                                           , new Lock()
                                            }
                         );
      }

      // Transport x2
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-800, -120, 600));
      positions.Add(new TV_3DVECTOR(700, -220, -5600));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = TransportATI.Instance();
        ainfo = SpawnActor(at
                        , ""
                        , ""
                        , ""
                        , creationTime
                        , MainAllyFaction
                        , v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z)
                        , new TV_3DVECTOR(0, 180, 0)
                        , new ActionInfo[] { new Move(nv, at.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MinSpeed)
                                           , new Lock()
                                            }
                         );
      }
    }

    private void Rebel_HyperspaceOut(object[] param)
    {
      foreach (ActorInfo a in MainAllyFaction.GetShips())
      {
        if (a.ActorState != ActorState.DYING && a.ActorState != ActorState.DEAD)
        {
          ActionManager.ForceClearQueue(a);
          ActionManager.QueueLast(a, new Rotate(a.GetPosition() + new TV_3DVECTOR(8000, 0, -20000)
                                                , a.MovementInfo.Speed
                                                , a.TypeInfo.Move_CloseEnough));
          ActionManager.QueueLast(a, new HyperspaceOut());
          ActionManager.QueueLast(a, new Delete());
        }
      }
    }

    public void Rebel_HyperspaceIn2(object[] param)
    {
      ActorInfo ainfo;
      float creationTime = Game.Instance().GameTime;
      float creationDelay = 0.025f;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      ActionInfo[] actions;
      FactionInfo faction = FactionInfo.Get("Rebels");
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(10000, 0, 10000);
      TV_3DVECTOR movedisp = new TV_3DVECTOR(3000, 0, 3000);
      ActorTypeInfo type;
      List<object[]> spawns = new List<object[]>();
      spawns.Add(new object[] { new TV_3DVECTOR(-4600, 150, 7300), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-5000, 90, 7500), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-5400, 150, 7700), XWingATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, -120, 6300), MC90ATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(1400, -320, 8400), CorellianATI.Instance() });
      spawns.Add(new object[] { new TV_3DVECTOR(-2400, 150, 6500), CorellianATI.Instance() });

      foreach (object[] spawn in spawns)
      {
        type = (ActorTypeInfo)spawn[1];
        creationTime += creationDelay;
        position = (TV_3DVECTOR)spawn[0];
        if (type is FighterGroup)
        {
          actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };
        }
        else
        {
          actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Move(position + movedisp
                                                  , type.MaxSpeed
                                                  , type.Move_CloseEnough
                                                  , false)
                                 , new Rotate(position + movedisp
                                                    , type.MinSpeed
                                                    , type.Move_CloseEnough
                                                    , false)
                                 , new Lock()
                                 };
        }

        ainfo = SpawnActor(type
                         , ""
                         , ""
                         , ""
                         , creationTime
                         , faction
                         , position + hyperspaceInOffset
                         , rotation
                         , actions
                         );
      }
    }


    public void Rebel_Wave(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { XWingATI.Instance(), AWingATI.Instance(), XWingATI.Instance(), AWingATI.Instance(), BWingATI.Instance(), YWingATI.Instance() };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        int n = Engine.Instance().Random.Next(0, tietypes.Length);

        ActorInfo ainfo = SpawnActor(tietypes[n]
                        , ""
                        , ""
                        , ""
                        , Game.Instance().GameTime + t
                        , MainAllyFaction
                        , new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MaxBounds.z + 1500)
                        , new TV_3DVECTOR(0, 180, 0)
                        , new ActionInfo[] { new Move(new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MaxBounds.z), tietypes[n].MaxSpeed)
                                            }
                         );
        t += 1.5f;
      }
    }

    public void Rebel_BeginBattle(object[] param)
    {
      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Empire_TIEWave_02");
    }

    public void Empire_TIEWave_02(object[] param)
    {
      int sets = 8;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 8;

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
            ActorInfo ainfo = SpawnActor(tietypes[n]
                                        , ""
                                        , ""
                                        , ""
                                        , Game.Instance().GameTime + t
                                        , MainEnemyFaction
                                        , new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 2500)
                                        , new TV_3DVECTOR()
                                        , new ActionInfo[] { new Wait(5) }
                                        );
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_Towers01(object[] param)
    {
      if (!LandInfo.Instance().Enabled)
        return;

      float dist = 3000;

      for (int x = -2; x <= 2; x++)
      {
        for (int z = -2; z <= 2; z++)
        {
          SpawnActor(Tower04ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 90 + LandInfo.Instance().GetLandHeight(x * dist, z * dist), z * dist), new TV_3DVECTOR());

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist + 300, 30 + LandInfo.Instance().GetLandHeight(x * dist, z * dist), z * dist), new TV_3DVECTOR());

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist - 300, 30 + LandInfo.Instance().GetLandHeight(x * dist, z * dist), z * dist), new TV_3DVECTOR());

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 30 + LandInfo.Instance().GetLandHeight(x * dist, z * dist), z * dist + 300), new TV_3DVECTOR());

          SpawnActor(Tower02ATI.Instance(), "", "", ""
                   , 0, FactionInfo.Get("Empire_DeathStarDefenses"), new TV_3DVECTOR(x * dist, 30 + LandInfo.Instance().GetLandHeight(x * dist, z * dist), z * dist - 300), new TV_3DVECTOR());
        }
      }
    }

    public void Empire_StarDestroyer_01(object[] param)
    {
      // SD
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -10000);
      float creationTime = Game.Instance().GameTime;

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      List<float> createtime = new List<float>();

      positions.Add(new TV_3DVECTOR(-7000, 110, GameScenarioManager.Instance().MinBounds.z - 500));
      createtime.Add(0);


      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = SpawnActor(ImperialIATI.Instance()
                        , ""
                        , ""
                        , ""
                        , creationTime + createtime[i]
                        , MainEnemyFaction
                        , v + hyperspaceInOffset
                        , new TV_3DVECTOR()
                        , new ActionInfo[] { new HyperspaceIn(v)
                                           , new Move(new TV_3DVECTOR(v.x * 0.2f, v.y, -1000), ImperialIATI.Instance().MaxSpeed / 2)
                                           , new Rotate(new TV_3DVECTOR(-1600, -120, 6300), 0)
                                           , new Lock() }
                        );
        ainfo.SetSpawnerEnable(true);
      }
    }
  }
}
