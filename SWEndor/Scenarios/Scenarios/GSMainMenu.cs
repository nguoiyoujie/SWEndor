using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSMainMenu : GameScenarioBase
  {
    public GSMainMenu(GameScenarioManager manager) : base(manager)
    {
      Name = "Main Menu";
    }

    private ActorInfo m_APlanet = null;
    private int sceneid;
    private float RebelSpawnTime = 0;
    private float TIESpawnTime = 0;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);

      sceneid = Engine.Random.Next(0, 7);
      LandInfo.Enabled = false;
      AtmosphereInfo.LoadDefaults(true, true);
      AtmosphereInfo.SetPos_Sun(new TV_3DVECTOR(-1000, 250, 0));
      AtmosphereInfo.Enabled = true;
      AtmosphereInfo.ShowSun = true;
      AtmosphereInfo.ShowFlare = true;
    }

    public override void Launch()
    {
      base.Launch();
      if (Manager.GetGameStateB("in_menu"))
        return;

      Manager.SetGameStateB("in_menu", true);

      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      cam.SetLocalPosition(100000, 0, 100000);
      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 5000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -5000);
      Manager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 5000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -5000);

      PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;

      Manager.AddEvent(Game.GameTime, Empire_StarDestroyer_01);
      Manager.AddEvent(Game.GameTime, Rebel_HyperspaceIn);
      Manager.AddEvent(Game.GameTime, Empire_TIEDefender);
      Manager.AddEvent(Game.GameTime + 0.2f, Empire_TIEAvengers);
      Manager.AddEvent(Game.GameTime + 0.5f, Rebel_BeginBattle);
      Manager.AddEvent(Game.GameTime + 15, Empire_StarDestroyer_00);

      PlayerInfo.Lives = 2;
      PlayerInfo.ScorePerLife = 9999999;
      PlayerInfo.ScoreForNextLife = 9999999;

      //SoundManager.SetMusic("battle_1_1", false, 18600);
      //SoundManager.SetMusic("battle_1_1", false, 71250);
      //SoundManager.SetMusic("battle_1_1", false, 191000);
      //SoundManager.SetMusicLoop("battle_3_4");

      /*
      switch (sceneid)
      {
        // Endor reportorie
        case 0:
        default:
          SoundManager.SetMusic("credits_3_1");
          SoundManager.SetMusicLoop("battle_3_4");
          break;
        case 1:
          SoundManager.SetMusic("credits_3_1");
          SoundManager.SetMusicLoop("battle_3_3");
          break;
        case 2:
          SoundManager.SetMusic("credits_3_1");
          SoundManager.SetMusicLoop("battle_3_4");
          break;
        // Yavin reportorie
        case 3:
          SoundManager.SetMusic("credits_1_1");
          SoundManager.SetMusicLoop("battle_1_1", 18600);
          break;
        case 4:
          SoundManager.SetMusic("credits_1_1");
          SoundManager.SetMusicLoop("battle_1_4");
          break;
        // Hoth reportorie
        case 5:
          SoundManager.SetMusic("credits_2_1");
          SoundManager.SetMusicLoop("battle_2_1");
          break;
        case 6:
          SoundManager.SetMusic("credits_2_1");
          SoundManager.SetMusicLoop("battle_2_2");
          break;
      }
      */

      SoundManager.SetMusic("dynamic\\CONF-01", false, 1657);
      Manager.IsCutsceneMode = true;
    }
    public override void Unload()
    {
      base.Unload();
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");

      MainAllyFaction.WingSpawnLimit = 24;
      MainEnemyFaction.WingSpawnLimit = 28;
    }

    public override void LoadScene()
    {
      base.LoadScene();

      ActorCreationInfo acinfo;

      // Create Planet
      if (m_APlanet == null)
      {
        if (sceneid <= 2)
        {
          acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Endor"));
        }
        else if (sceneid <= 4)
        {
          acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Yavin"));
        }
        else
        {
          acinfo = new ActorCreationInfo(ActorTypeFactory.Get("Hoth"));
        }

        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, -40000, 0);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        acinfo.InitialScale = 60;
        m_APlanet = ActorInfo.Create(ActorFactory, acinfo);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      ActorInfo tgt = ActorFactory.Get(Manager.CameraTargetActorID);

      if (tgt == null || tgt.CreationState != CreationState.ACTIVE)
      {
        List<int> list = new List<int>(MainEnemyFaction.GetWings());
        //List<int> list = new List<int>(MainAllyFaction.GetWings());
        if (list.Count > 0)
        {
          Manager.CameraTargetActorID = list[Engine.Random.Next(0, list.Count)];
        }
      }

      if (tgt != null && tgt.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR pos = tgt.GetRelativePositionFUR(-300, 0, 0);
        cam.SetLocalPosition(pos.x, pos.y + 125, pos.z);
        tgt.TypeInfo.ChaseCamera(cam);
        //Manager.CameraTargetActor.TypeInfo.ChaseCamera(Manager.CameraTargetActor);
      }

      if (Manager.GetGameStateB("in_battle"))
      {
        // TIE spawn
        if (TIESpawnTime < Game.GameTime)
        {
          if (MainEnemyFaction.GetWings().Count < 12)
          {
            TIESpawnTime = Game.GameTime + 10f;
            Manager.AddEvent(0, Empire_TIEWave_01, IntegerEventArg.N4);
          }
        }

        // Rebel spawn
        if (RebelSpawnTime < Game.GameTime)
        {
          if (MainAllyFaction.GetWings().Count < 8)
          {
            RebelSpawnTime = Game.GameTime + 10f;
            Manager.AddEvent(0, Rebel_Wave);
          }
        }

        if (MainAllyFaction.GetShips().Count < 3 && !Manager.GetGameStateB("rebels_fled"))
        {
          Manager.SetGameStateB("rebels_fled", true);
          Manager.AddEvent(Game.GameTime + 15f, Rebel_HyperspaceIn2);
          Rebel_HyperspaceOut(null);
        }
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_APlanet != null && m_APlanet.CreationState == CreationState.ACTIVE)
      {
        float y_en = -40000;
        if (PlayerInfo.Position.z < -30000)
          y_en += (PlayerInfo.Position.z + 30000) * 100f;

        m_APlanet.SetLocalPosition(0, y_en, 0);
      }
    }

    public void Rebel_HyperspaceIn(GameEventArg arg)
    {
      ActorInfo ainfo;
      float creationTime = Game.GameTime;

      // Wings x24
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      for (int i = 0; i < 24; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(-800, -80), Engine.Random.Next(-100, 100), Engine.Random.Next(-800, -150)));
        else
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(80, 800), Engine.Random.Next(-100, 100), Engine.Random.Next(-800, -150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ActorTypeInfo[] atypes = new ActorTypeInfo[] { ActorTypeFactory.Get("Z-95")
                                                      , ActorTypeFactory.Get("Z-95")
                                                      , ActorTypeFactory.Get("Z-95")
                                                      , ActorTypeFactory.Get("Z-95")
                                                      , ActorTypeFactory.Get("X-Wing")
                                                      , ActorTypeFactory.Get("X-Wing")
                                                      , ActorTypeFactory.Get("A-Wing")
                                                      , ActorTypeFactory.Get("A-Wing")
                                                      , ActorTypeFactory.Get("Y-Wing")
                                                      , ActorTypeFactory.Get("Y-Wing")
                                                      , ActorTypeFactory.Get("B-Wing")
                                                      , ActorTypeFactory.Get("B-Wing") };

        ActorTypeInfo at = atypes[i % atypes.Length];
        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[]{ new Move(new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), -v.z - 750)
                                                      , at.MaxSpeed)
                                                      }
        }.Spawn(this);

      }

      // Nebulon x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-100, 220, 1200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = ActorTypeFactory.Get("Nebulon-B Frigate");

        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Move(nv, at.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MinSpeed)
                                           , new Lock()
                                            }
        }.Spawn(this);
      }

      // Corellian x3
      positions.Clear();
      positions.Add(new TV_3DVECTOR(300, 370, -3200));
      positions.Add(new TV_3DVECTOR(125, 170, 7200));
      positions.Add(new TV_3DVECTOR(-1000, -60, 7700));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = ActorTypeFactory.Get("Corellian Corvette");

        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Move(nv, at.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MinSpeed)
                                           , new Lock()
                                            }
        }.Spawn(this);
      }

      // Transport x4
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-800, -120, 600));
      positions.Add(new TV_3DVECTOR(700, -220, 5600));
      positions.Add(new TV_3DVECTOR(-100, -220, 6400));
      positions.Add(new TV_3DVECTOR(1200, 160, 6200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = ActorTypeFactory.Get("Transport");

        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Move(nv, at.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MinSpeed)
                                           , new Lock()
                                            }
        }.Spawn(this);
      }
    }

    private void Rebel_HyperspaceOut(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null
          && actor.ActorState != ActorState.DYING 
          && actor.ActorState != ActorState.DEAD)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(18000, 0, -20000)
                                                , actor.MoveData.Speed
                                                , actor.TypeInfo.Move_CloseEnough));
          ActionManager.QueueLast(actorID, new HyperspaceOut());
          ActionManager.QueueLast(actorID, new Delete());
        }
      }
    }

    public void Rebel_HyperspaceIn2(GameEventArg arg)
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("Mon Calamari Capital Ship")
                                                        , MainAllyFaction
                                                        , true
                                                        , new TV_3DVECTOR(0, -135, 0)
                                                        , 90
                                                        , true
                                                        , null
                                                        );

      List<ShipSpawnEventArg> SDspawnlist = new List<ShipSpawnEventArg>();
      sspawn.TypeInfo = ActorTypeFactory.Get("Mon Calamari Capital Ship");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(-1600, -120, 6300)
                                          , new TV_3DVECTOR(1400, -120, 9300)
                                          , new TV_3DVECTOR(-1200, -120, -1000)
                                          ));

      sspawn.TypeInfo = ActorTypeFactory.Get("Corellian Corvette");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(1400, -320, 8400)
                                          , new TV_3DVECTOR(4400, -320, 11400)
                                          , new TV_3DVECTOR(-1200, -320, 1000)
                                          ));

      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(-2400, 350, 6500)
                                          , new TV_3DVECTOR(1600, 350, 9500)
                                          , new TV_3DVECTOR(-1200, -250, -5000)
                                          ));

      sspawn.TypeInfo = ActorTypeFactory.Get("Mon Calamari Light Cruiser");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(6600, -220, 7300)
                                          , new TV_3DVECTOR(4600, -220, 4300)
                                          , new TV_3DVECTOR(-1200, -220, -1000)
                                          ));

      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(8200, 120, 5300)
                                          , new TV_3DVECTOR(5200, 120, 2300)
                                          , new TV_3DVECTOR(-1200, 120, -1000)
                                          ));

      foreach (ShipSpawnEventArg s in SDspawnlist)
      {
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);
      }

      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-4000, 500, 7300), new TV_3DVECTOR(-5500, 0, 8000));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("Z-95")
                                                                          , MainAllyFaction
                                                                          , 3
                                                                          , 3
                                                                          , TargetType.FIGHTER
                                                                          , true
                                                                          , GSFunctions.SquadFormation.VSHAPE
                                                                          , new TV_3DVECTOR(0, -135, 0)
                                                                          , 400
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, 2, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("X-Wing");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("A-Wing");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("B-Wing");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("Y-Wing");
      spawninfo.HuntTargetType = TargetType.SHIELDGENERATOR | TargetType.SHIP;
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
    }


    public void Rebel_Wave(GameEventArg arg)
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -1000, Manager.MaxBounds.z + 1500), new TV_3DVECTOR(2500, 1000, -1500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("Z-95")
                                                                          , MainAllyFaction
                                                                          , 3
                                                                          , 3
                                                                          , TargetType.FIGHTER
                                                                          , true
                                                                          , GSFunctions.SquadFormation.VSHAPE
                                                                          , new TV_3DVECTOR(0, 180, 0)
                                                                          , 400
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, 2, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("X-Wing");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("A-Wing");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("B-Wing");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("Y-Wing");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
    }

    public void Rebel_BeginBattle(GameEventArg arg)
    {
      Manager.SetGameStateB("in_battle", true);
      Manager.AddEvent(Game.GameTime + 10f, Empire_TIEWave_01, IntegerEventArg.N8);
    }

    public void Empire_TIEWave_01(GameEventArg arg)
    {
      int sets = ((IntegerEventArg)arg).Num;
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, Manager.MinBounds.z - 8000), new TV_3DVECTOR(2500, 500, Manager.MinBounds.z - 9000));
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
    }

    public void Empire_TIEDefender(GameEventArg arg)
    {
      // TID/D
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -100000);
      TV_3DVECTOR plus = new TV_3DVECTOR(3500, 500, 2750);
      float creationTime = Game.GameTime;

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      List<float> createtime = new List<float>();

      positions.Add(new TV_3DVECTOR(-10500, -1010, Manager.MinBounds.z - 7500));
      positions.Add(new TV_3DVECTOR(-10250, -910, Manager.MinBounds.z - 7600));
      positions.Add(new TV_3DVECTOR(-10750, -910, Manager.MinBounds.z - 7600));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE Defender"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainEnemyFaction,
          Position = v + hyperspaceInOffset + plus,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(7.5f)
                                     , new Rotate(v, 0)
                                     , new Wait(2.5f)
                                     , new HyperspaceIn(v)
                                     , new Rotate(new TV_3DVECTOR(0, 0, 0), 1)
                                     , new Wait(3f+10f)}
        }.Spawn(this);

        ainfo.MoveData.MinSpeed = 0;
        Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.1f;
      }
    }

    public void Empire_TIEAvengers(GameEventArg arg)
    {
      // TID/A
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -100000);
      TV_3DVECTOR plus = new TV_3DVECTOR(3500, 500, 2750);
      float creationTime = Game.GameTime;

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      List<float> createtime = new List<float>();

      positions.Add(new TV_3DVECTOR(-9500, -560, Manager.MinBounds.z - 6500));
      positions.Add(new TV_3DVECTOR(-9250, -660, Manager.MinBounds.z - 6600));
      positions.Add(new TV_3DVECTOR(-9750, -660, Manager.MinBounds.z - 6600));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIE Avenger"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainEnemyFaction,
          Position = v + hyperspaceInOffset + plus,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(7.5f)
                                     , new Rotate(v, 0)
                                     , new Wait(12.5f)
                                     , new HyperspaceIn(v)
                                     , new Rotate(new TV_3DVECTOR(0, 0, 0), 40)
                                     , new Wait(3f)
                                     }
        }.Spawn(this);

        ainfo.MoveData.MinSpeed = 0;
        Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.1f;
      }
    }

    public void Empire_StarDestroyer_00(GameEventArg arg)
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 90
                                                              , true
                                                              , null
                                                              );

      List<ShipSpawnEventArg> SDspawnlist = new List<ShipSpawnEventArg>();
      sspawn.TypeInfo = ActorTypeFactory.Get("Victory-I Star Destroyer");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(1200, 850, -7000)
                                          , new TV_3DVECTOR(600, 720, -2000)
                                          , new TV_3DVECTOR(0, 500, 0)
                                          ));

      sspawn.TypeInfo = ActorTypeFactory.Get("Arquitens Light Cruiser");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(800, -100, -6800)
                                          , new TV_3DVECTOR(-1600, -100, -1200)
                                          , new TV_3DVECTOR(0, 0, 0)
                                          ));

      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(1600, 150, -7200)
                                          , new TV_3DVECTOR(2600, 150, -1700)
                                          , new TV_3DVECTOR(8000, 150, 2500)
                                          ));

      foreach (ShipSpawnEventArg s in SDspawnlist)
      {
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);
      }
    }

    public void Empire_StarDestroyer_01(GameEventArg arg)
    {
      // SD
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -100000);
      float creationTime = Game.GameTime;

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      List<float> createtime = new List<float>();

      positions.Add(new TV_3DVECTOR(-6000, 110, Manager.MinBounds.z - 500));
      createtime.Add(0);

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("Imperial-I Star Destroyer"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime + createtime[i],
          Faction = MainEnemyFaction,
          Position = v + hyperspaceInOffset,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(32f+10f)
                                     //, new HyperspaceIn(v + hyperspaceInOffset + new TV_3DVECTOR(0, 0, 100000))
                                     //, new HyperspaceIn(v + hyperspaceInOffset)
                                     , new HyperspaceIn(v)
                                     , new EnableSpawn(true)
                                     , new Move(new TV_3DVECTOR(v.x * 0.2f, v.y, -1000), ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed / 2)
                                     , new Rotate(new TV_3DVECTOR(-1600, -120, 6300), 0)
                                     , new Lock() }
        }.Spawn(this);

        Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.001f;
      }
    }
  }
}
