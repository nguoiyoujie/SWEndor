using MTV3D65;
using System.Collections.Generic;
using System;

namespace SWEndor.Scenarios
{
  public class GSMainMenu : GameScenarioBase
  {
    public GSMainMenu()
    {
      Name = "Main Menu";
    }

    private ActorInfo m_AScene = null;
    private ActorInfo m_APlanet = null;
    private int sceneid = Engine.Instance().Random.Next(0, 7);

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      if (GameScenarioManager.Instance().GetGameStateB("in_menu"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_menu", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(15000, 1500, 5000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-15000, -1500, -5000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(15000, 1500, 5000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-15000, -1500, -5000);

      PlayerInfo.Instance().CameraMode = CameraMode.FIRSTPERSON;

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_HyperspaceIn");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.5f, "Rebel_BeginBattle");

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7f, "Empire_StarDestroyer_01");

      PlayerInfo.Instance().Lives = 2;
      PlayerInfo.Instance().ScorePerLife = 9999999;
      PlayerInfo.Instance().ScoreForNextLife = 9999999;
      PlayerInfo.Instance().Score = new ScoreInfo();

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
        List<ActorInfo> list = new List<ActorInfo>(GameScenarioManager.Instance().AllyFighters.Values);
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
      }

      if (GameScenarioManager.Instance().GetGameStateB("in_battle"))
      {
        // TIE spawn
        if (TIESpawnTime < Game.Instance().GameTime)
        {
          if (GameScenarioManager.Instance().EnemyFighters.Count < 30)
          {
            TIESpawnTime = Game.Instance().GameTime + 10f;
            Empire_TIEWave_02(new object[] { 6 });
          }
        }

        // Rebel spawn
        if (RebelSpawnTime < Game.Instance().GameTime)
        {
          if (GameScenarioManager.Instance().AllyFighters.Count < 10)
          {
            RebelSpawnTime = Game.Instance().GameTime + 10f;
            Rebel_Wave(new object[] { 15 });
          }
        }

        if (GameScenarioManager.Instance().AllyShips.Count < 3 && !GameScenarioManager.Instance().GetGameStateB("rebels_fled"))
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
      if (m_AScene != null && m_AScene.CreationState == CreationState.ACTIVE)
      {
        m_AScene.SetLocalPosition(PlayerInfo.Instance().Position.x, PlayerInfo.Instance().Position.y, PlayerInfo.Instance().Position.z);
      }

    }

    public override void RegisterEvents()
    {
      base.RegisterEvents();
      GameEvent.RegisterEvent("Rebel_HyperspaceIn", Rebel_HyperspaceIn);
      GameEvent.RegisterEvent("Rebel_HyperspaceIn2", Rebel_HyperspaceIn2);
      GameEvent.RegisterEvent("Rebel_HyperspaceOut", Rebel_HyperspaceOut);
      GameEvent.RegisterEvent("Rebel_MC80Spawner", Rebel_MC80Spawner);
      GameEvent.RegisterEvent("Rebel_BeginBattle", Rebel_BeginBattle);
      GameEvent.RegisterEvent("Rebel_Wave", Rebel_Wave);

      GameEvent.RegisterEvent("Empire_TIEWave_02", Empire_TIEWave_02);
      GameEvent.RegisterEvent("Empire_StarDestroyer_01", Empire_StarDestroyer_01);
      GameEvent.RegisterEvent("Empire_SDSpawner", Empire_SDSpawner);

    }

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorCreationInfo acinfo;
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
        
        acinfo = new ActorCreationInfo(atypes[Engine.Instance().Random.Next(0, atypes.Length)]);
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        //creationTime += 0.05f;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        ainfo = ActorInfo.Create(acinfo);

        ActionManager.QueueLast(ainfo, new Actions.Move(new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 750)
                                                      , ainfo.MaxSpeed));
        
        GameScenarioManager.Instance().AllyFighters.Add(ainfo.Name + " " + i, ainfo);
        RegisterEvents(ainfo);
      }

      // Nebulon x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-100, 220, 1200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(NebulonBATI.Instance());
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        //creationTime += 0.05f;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        ainfo = ActorInfo.Create(acinfo);
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 6000);

        ActionManager.QueueLast(ainfo, new Actions.Move(nv, ainfo.MaxSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Rotate(nv - new TV_3DVECTOR(0, 0, 20000), ainfo.MinSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Lock());

        GameScenarioManager.Instance().AllyShips.Add(ainfo.Name + " " + i, ainfo);
        RegisterEvents(ainfo);
      }

      // Corellian x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(300, 370, -3200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(CorellianATI.Instance());
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        //creationTime += 0.05f;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        ainfo = ActorInfo.Create(acinfo);
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 6000);
        ActionManager.QueueLast(ainfo, new Actions.Move(nv, ainfo.MaxSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Rotate(nv - new TV_3DVECTOR(0, 0, 20000), ainfo.MinSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Lock());

        GameScenarioManager.Instance().AllyShips.Add(ainfo.Name + " " + i, ainfo);
        RegisterEvents(ainfo);
      }

      // Transport x2
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-800, -120, 600));
      positions.Add(new TV_3DVECTOR(700, -220, -5600));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(TransportATI.Instance());
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        //creationTime += 0.05f;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        ainfo = ActorInfo.Create(acinfo);
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 6000);
        ActionManager.QueueLast(ainfo, new Actions.Move(nv, ainfo.MaxSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Rotate(nv - new TV_3DVECTOR(0, 0, 20000), ainfo.MinSpeed));
        ActionManager.QueueLast(ainfo, new Actions.Lock());

        GameScenarioManager.Instance().AllyShips.Add(ainfo.Name + " " + i, ainfo);
        RegisterEvents(ainfo);
      }

      RebelFighterLimit = GameScenarioManager.Instance().AllyFighters.Count;
    }

    private void Rebel_HyperspaceOut(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
      {
        if (a.ActorState != ActorState.DYING && a.ActorState != ActorState.DEAD)
        {
          ActionManager.ForceClearQueue(a);
          ActionManager.QueueLast(a, new Actions.Rotate(a.GetPosition() + new TV_3DVECTOR(8000, 0, -20000)
                                                , a.MaxSpeed
                                                , a.TypeInfo.Move_CloseEnough));
          ActionManager.QueueLast(a, new Actions.HyperspaceOut());
          ActionManager.QueueLast(a, new Actions.Delete());
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
      Dictionary<string, ActorInfo>[] registries;
      FactionInfo faction = FactionInfo.Get("Rebels");
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(10000, 0, 10000);
      TV_3DVECTOR movedisp = new TV_3DVECTOR(3000, 0, 3000);
      string name = "";
      string registername = "";
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
                                 , new Actions.Move(new TV_3DVECTOR(position.x + Engine.Instance().Random.Next(-5, 5), position.y + Engine.Instance().Random.Next(-5, 5), -position.z - 1500)
                                                                  , type.MaxSpeed
                                                                  , type.Move_CloseEnough)
                                 };

          registries = new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().AllyFighters };
        }
        else
        {
          actions = new ActionInfo[] { new Actions.HyperspaceIn(position)
                                 , new Actions.Move(position + movedisp
                                                  , type.MaxSpeed
                                                  , type.Move_CloseEnough
                                                  , false)
                                 , new Actions.Rotate(position + movedisp
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
                         , ""
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


    public void Rebel_Wave(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      ActorCreationInfo aci;
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { XWingATI.Instance(), AWingATI.Instance(), XWingATI.Instance(), AWingATI.Instance(), BWingATI.Instance(), YWingATI.Instance() };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        int n = Engine.Instance().Random.Next(0, tietypes.Length);
        aci = new ActorCreationInfo(tietypes[n]);

        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Rebels");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, GameScenarioManager.Instance().MaxBounds.z + 1500);
        aci.Rotation = new TV_3DVECTOR(0, 180, 0);

        ActorInfo a = ActorInfo.Create(aci);
        ActionManager.QueueLast(a, new Actions.Move(new TV_3DVECTOR(aci.Position.x, aci.Position.y, GameScenarioManager.Instance().MaxBounds.z), a.MaxSpeed));

        GameScenarioManager.Instance().AllyFighters.Add(a.Key, a);
        RegisterEvents(a);

        t += 1.5f;
      }
    }

    public void Rebel_BeginBattle(object[] param)
    {
      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Empire_TIEWave_02");
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
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 2500);
            aci.Rotation = new TV_3DVECTOR();

            ActorInfo a = ActorInfo.Create(aci);
            if (GameScenarioManager.Instance().AllyFighters.Count > 0)
            {
              string[] rskeys = new string[GameScenarioManager.Instance().AllyFighters.Count];
              GameScenarioManager.Instance().AllyFighters.Keys.CopyTo(rskeys, 0);
              ActorInfo rs = GameScenarioManager.Instance().AllyFighters[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

              ActionManager.QueueLast(a, new Actions.Wait(5));
            }

            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
          }
        }
        t += 1.5f;
      }
    }


    public void Empire_StarDestroyer_01(object[] param)
    {
      GameScenarioManager.Instance().SDWaves++;

      // SD
      ActorCreationInfo acinfo;
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
        acinfo = new ActorCreationInfo(ImperialIATI.Instance());
        acinfo.Faction = FactionInfo.Get("Empire");
        acinfo.InitialState = ActorState.NORMAL;
        acinfo.CreationTime = creationTime + createtime[i];
        acinfo.Position = v + hyperspaceInOffset;
        acinfo.Rotation = new TV_3DVECTOR();
        ainfo = ActorInfo.Create(acinfo);

        ActionManager.QueueLast(ainfo, new Actions.HyperspaceIn(v));
        ActionManager.QueueLast(ainfo, new Actions.Move(new TV_3DVECTOR(v.x * 0.2f, v.y, -1000), ainfo.MaxSpeed));
        //ActionManager.QueueLast(ainfo, new Actions.Rotate(v + new TV_3DVECTOR(0, 0, 2500), ainfo.MinSpeed));
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
          TargetPosition = new TV_3DVECTOR(v.x * 0.2f, v.y, -1000)
        });
        ainfo.AI.Orders.Enqueue(new AIElement
        {
          AIType = AIType.ROTATE,
          TargetPosition = v + new TV_3DVECTOR(0, 0, 2500)
        });
        ainfo.AI.Orders.Enqueue(new AIElement { AIType = AIType.LOCK });
        */
        GameScenarioManager.Instance().EnemyShips.Add(ainfo.Key, ainfo);
        RegisterEvents(ainfo);
        ainfo.TickEvents.Add("Empire_SDSpawner");
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
  }
}
