using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Models;
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
      Engine.LandInfo.Enabled = false;
      Engine.AtmosphereInfo.LoadDefaults(true, true);
      Engine.AtmosphereInfo.SetPos_Sun(new TV_3DVECTOR(-1000, 250, 0));
      Engine.AtmosphereInfo.Enabled = true;
      Engine.AtmosphereInfo.ShowSun = true;
      Engine.AtmosphereInfo.ShowFlare = true;
    }

    public override void Launch()
    {
      base.Launch();
      if (Manager.GetGameStateB("in_menu"))
        return;

      Manager.SetGameStateB("in_menu", true);

      //ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      //cam.Position = new TV_3DVECTOR(100000, 0, 100000);
      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 5000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -5000);
      Manager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 10000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -10000);

      PlayerCameraInfo.CameraMode = CameraMode.CUSTOM;

      Empire_StarDestroyer_01();
      Empire_TIEDefender();
      Rebel_HyperspaceIn();
      Manager.AddEvent(Game.GameTime + 0.2f, Empire_TIEAvengers);
      Manager.AddEvent(Game.GameTime + 0.5f, Rebel_BeginBattle);
      Manager.AddEvent(Game.GameTime + 15, Empire_StarDestroyer_00);

      PlayerInfo.Lives = 2;
      PlayerInfo.ScorePerLife = 9999999;
      PlayerInfo.ScoreForNextLife = 9999999;
      
      Mood = MoodStates.AMBIENT;
      SoundManager.SetMusicDyn("TRO-IN");
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
          acinfo = new ActorCreationInfo(ActorTypeFactory.Get("ENDOR"));
        }
        else if (sceneid <= 4)
        {
          acinfo = new ActorCreationInfo(ActorTypeFactory.Get("YAVIN"));
        }
        else
        {
          acinfo = new ActorCreationInfo(ActorTypeFactory.Get("HOTH"));
        }

        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, -40000, 0);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        acinfo.InitialScale = 60;
        m_APlanet = Engine.ActorFactory.Create(acinfo);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      ActorInfo tgt = Engine.ActorFactory.Get(PlayerCameraInfo.Look.GetPosition_Actor());

      if (tgt == null || !tgt.Active)
      {
        List<int> list = new List<int>(MainEnemyFaction.GetWings());
        //list.AddRange(MainAllyFaction.GetWings());
        if (list.Count > 0)
          PlayerCameraInfo.Look.SetPosition_Actor(list[Engine.Random.Next(0, list.Count)]);
      }

      if (tgt != null && tgt.Active)
      {
        PlayerInfo.TempActorID = tgt.ID;
        PlayerCameraInfo.Look.SetPosition_Actor(tgt.ID, new TV_3DVECTOR(0, 25, 0), new TV_3DVECTOR(0, 0, -100));
        PlayerCameraInfo.Look.SetTarget_LookAtActor(tgt.ID, new TV_3DVECTOR(0, 30, 0), new TV_3DVECTOR(0, 0, 20000));
        PlayerCameraInfo.Look.SetRotationMult(0.2f);
      }

      if (Manager.GetGameStateB("in_battle"))
      {
        // TIE spawn
        if (TIESpawnTime < Game.GameTime)
        {
          if (MainEnemyFaction.WingCount < 12)
          {
            TIESpawnTime = Game.GameTime + 10f;
            Manager.AddEvent(0, Empire_TIEWave_01, 4);
          }
        }

        // Rebel spawn
        if (RebelSpawnTime < Game.GameTime)
        {
          if (MainAllyFaction.WingCount < 8)
          {
            RebelSpawnTime = Game.GameTime + 10f;
            Manager.AddEvent(0, Rebel_Wave);
          }
        }

        if (MainAllyFaction.ShipCount < 3 && !Manager.GetGameStateB("rebels_fled"))
        {
          Manager.SetGameStateB("rebels_fled", true);
          Manager.AddEvent(Game.GameTime + 15f, Rebel_HyperspaceIn2);
          Rebel_HyperspaceOut();
        }
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_APlanet != null && m_APlanet.Active)
      {
        float y_en = -40000;
        if (PlayerCameraInfo.Position.z < -30000)
          y_en += (PlayerCameraInfo.Position.z + 30000) * 50f;
        else if (PlayerCameraInfo.Position.z > 30000)
          y_en -= (PlayerCameraInfo.Position.z - 30000) * 50f;

        m_APlanet.Position = new TV_3DVECTOR(0, y_en, 0);
      }
    }

    public void Rebel_HyperspaceIn()
    {
      ActorInfo ainfo;
      float creationTime = Game.GameTime;

      // Wings x24
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      for (int i = 0; i < 15; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(-800, -80), Engine.Random.Next(-100, 100), Engine.Random.Next(2800, 4850)));
        else
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(80, 800), Engine.Random.Next(-100, 100), Engine.Random.Next(2800, 4850)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ActorTypeInfo[] atypes = new ActorTypeInfo[] { ActorTypeFactory.Get("Z95")
                                                      , ActorTypeFactory.Get("Z95")
                                                      , ActorTypeFactory.Get("Z95")
                                                      , ActorTypeFactory.Get("Z95")
                                                      , ActorTypeFactory.Get("XWING")
                                                      , ActorTypeFactory.Get("XWING")
                                                      , ActorTypeFactory.Get("AWING")
                                                      , ActorTypeFactory.Get("AWING")
                                                      , ActorTypeFactory.Get("YWING")
                                                      , ActorTypeFactory.Get("YWING")
                                                      , ActorTypeFactory.Get("BWING")
                                                      , ActorTypeFactory.Get("BWING") };

        ActorTypeInfo at = atypes[i % atypes.Length];
        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[]{ new Wait(18) }
        }.Spawn(this);

      }

      // Nebulon x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-100, 220, 1200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = ActorTypeFactory.Get("NEBL");

        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Move(nv, at.MoveLimitData.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MoveLimitData.MinSpeed)
                                           , new Lock()
                                            }
        }.Spawn(this);
        ainfo.SetSpawnerEnable(true);
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
        ActorTypeInfo at = ActorTypeFactory.Get("CORV");

        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Move(nv, at.MoveLimitData.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MoveLimitData.MinSpeed)
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
        ActorTypeInfo at = ActorTypeFactory.Get("TRAN");

        ainfo = new ActorSpawnInfo
        {
          Type = at,
          Name = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + new TV_3DVECTOR(0, 0, Manager.MaxBounds.z),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Move(nv, at.MoveLimitData.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), at.MoveLimitData.MinSpeed)
                                           , new Lock()
                                            }
        }.Spawn(this);
      }
    }

    private void Rebel_HyperspaceOut()
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null
          && !actor.IsDyingOrDead)
        {
          actor.ForceClearQueue();
          actor.QueueLast(new Rotate(actor.GetGlobalPosition() + new TV_3DVECTOR(18000, 0, -20000)
                                                , actor.MoveData.Speed
                                                , actor.TypeInfo.AIData.Move_CloseEnough));
          actor.QueueLast(new HyperspaceOut());
          actor.QueueLast(new Delete());
        }
      }
    }

    public void Rebel_HyperspaceIn2()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("MC90")
                                                        , MainAllyFaction
                                                        , true
                                                        , new TV_3DVECTOR(0, -135, 0)
                                                        , 90
                                                        , true
                                                        );

      List<ShipSpawnEventArg> SDspawnlist = new List<ShipSpawnEventArg>();
      sspawn.TypeInfo = ActorTypeFactory.Get("MC90");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(-1600, -120, 6300)
                                          , new TV_3DVECTOR(-5400, -120, 3300)
                                          , new TV_3DVECTOR(-1200, -120, -1000)
                                          ));

      //sspawn.TypeInfo = ActorTypeFactory.Get("MC80B");
      //SDspawnlist.Add(new ShipSpawnEventArg(sspawn
      //                                    , new TV_3DVECTOR(200, -620, 10300)
      //                                    , new TV_3DVECTOR(-2400, -620, 7300)
      //                                    , new TV_3DVECTOR(-1200, -420, 1000)
      //                                    ));

      sspawn.TypeInfo = ActorTypeFactory.Get("CORV");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(1400, -320, 8400)
                                          , new TV_3DVECTOR(-4400, -320, 5400)
                                          , new TV_3DVECTOR(-1200, -320, 1000)
                                          ));

      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(-2400, 350, 6500)
                                          , new TV_3DVECTOR(-1600, 350, 3500)
                                          , new TV_3DVECTOR(-1200, 350, -5000)
                                          ));

      sspawn.TypeInfo = ActorTypeFactory.Get("MCL");
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

      float delay = 0;
      foreach (ShipSpawnEventArg s in SDspawnlist)
      {
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, delay, s.Info);
        delay += 0.7f;
        if  (ship.TypeInfo == ActorTypeFactory.Get("MC90"))
          ship.SetArmor(DamageType.ALL, 0.1f);
      }

      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-4000, 500, 7300), new TV_3DVECTOR(-5500, 0, 8000));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("Z95")
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
      spawninfo.TypeInfo = ActorTypeFactory.Get("XWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("AWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("BWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("YWING");
      spawninfo.HuntTargetType = TargetType.SHIELDGENERATOR | TargetType.SHIP;
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);

      Manager.AddEvent(Game.GameTime + 25, Empire_StarDestroyer_02);
      Manager.AddEvent(Game.GameTime + 50, Rebel_HyperspaceIn3);
      Mood = MoodStates.ENEMY_SHIP_ARRIVED;
    }

    public void Rebel_HyperspaceIn3()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("MC90")
                                                        , MainAllyFaction
                                                        , true
                                                        , new TV_3DVECTOR(0, -100, 0)
                                                        , 90
                                                        , true
                                                        );

      List<ShipSpawnEventArg> SDspawnlist = new List<ShipSpawnEventArg>();
      sspawn.TypeInfo = ActorTypeFactory.Get("NEBL");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(7600, 420, 6300)
                                          , new TV_3DVECTOR(-1200, 420, -1300)
                                          , new TV_3DVECTOR(0, 420, 0)
                                          ));

      sspawn.TypeInfo = ActorTypeFactory.Get("CORV");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(4600, 320, 8300)
                                          , new TV_3DVECTOR(1600, 320, 2300)
                                          , new TV_3DVECTOR(0, 320, 0)
                                          ));

      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(9200, 250, 4300)
                                          , new TV_3DVECTOR(200, 250, -2300)
                                          , new TV_3DVECTOR(0, 250, 0)
                                          ));

      float delay = 0;
      foreach (ShipSpawnEventArg s in SDspawnlist)
      {
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, delay, s.Info);
        delay += 2.6f;
        ship.SetSpawnerEnable(true);
      }

      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-4000, 500, 7300), new TV_3DVECTOR(-5500, 0, 8000));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("Z95")
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
      spawninfo.TypeInfo = ActorTypeFactory.Get("XWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("AWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("BWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("YWING");
      spawninfo.HuntTargetType = TargetType.SHIELDGENERATOR | TargetType.SHIP;
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      Mood = MoodStates.ENEMY_SHIP_ARRIVED;
    }


    public void Rebel_Wave()
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -1000, Manager.MaxBounds.z + 1500), new TV_3DVECTOR(2500, 1000, -1500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("Z95")
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
      spawninfo.TypeInfo = ActorTypeFactory.Get("XWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("AWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("BWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("YWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      Mood = MoodStates.ENEMY_FIGHTER_ARRIVED;
    }

    public void Rebel_BeginBattle()
    {
      Manager.SetGameStateB("in_battle", true);
      RebelSpawnTime = Game.GameTime + 30;
      Manager.AddEvent(Game.GameTime + 10f, Empire_TIEWave_01, 8);
    }

    public void Empire_TIEWave_01(int sets)
    {
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

    public void Empire_TIEDefender()
    {
      // TID/D
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -500000);
      TV_3DVECTOR plus = new TV_3DVECTOR(3500, 500, 2750);

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      positions.Add(new TV_3DVECTOR(-10500, -1010, Manager.MinBounds.z - 7500));
      positions.Add(new TV_3DVECTOR(-10250, -910, Manager.MinBounds.z - 7600));
      positions.Add(new TV_3DVECTOR(-10750, -910, Manager.MinBounds.z - 7600));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIED"),
          Name = "",
          SidebarName = "",
          SpawnTime = 0,
          Faction = MainEnemyFaction,
          Position = v + hyperspaceInOffset + plus,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(7.5f)
                                     , new Rotate(v, 0, 0.5f)
                                     , new SetMood(MoodStates.AMBIENT_2, true)
                                     , new Wait(2.5f)
                                     , new HyperspaceOut()
                                     , new HyperspaceIn(v)
                                     , new SetMood(MoodStates.NEUTRAL_SHIP_ARRIVED, true)
                                     , new Rotate(new TV_3DVECTOR(0, 0, 0), 1)
                                     , new Wait(3f+10f)
                                     , new SetMood(MoodStates.AMBIENT_4, true)
                                     }
        }.Spawn(this);

        ainfo.MoveData.MinSpeed = 1;
        //Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.4f;
      }
    }

    public void Empire_TIEAvengers()
    {
      // TID/A
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -500000);
      TV_3DVECTOR plus = new TV_3DVECTOR(3500, 500, 2750);

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      positions.Add(new TV_3DVECTOR(-9500, -560, Manager.MinBounds.z - 6500));
      positions.Add(new TV_3DVECTOR(-9250, -660, Manager.MinBounds.z - 6600));
      positions.Add(new TV_3DVECTOR(-9750, -660, Manager.MinBounds.z - 6600));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TIEA"),
          Name = "",
          SidebarName = "",
          SpawnTime = 0,
          Faction = MainEnemyFaction,
          Position = v + hyperspaceInOffset + plus,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(7.5f)
                                     , new Rotate(v, 0, 0.5f)
                                     , new Wait(12.5f)
                                     , new HyperspaceOut()
                                     , new HyperspaceIn(v)
                                     , new SetMood(MoodStates.ALLY_FIGHTER_ARRIVED, true)
                                     , new SetMood(MoodStates.AMBIENT_3, true)
                                     , new Rotate(new TV_3DVECTOR(0, 0, 0), 40)
                                     , new Wait(3f)
                                     }
        }.Spawn(this);

        ainfo.MoveData.MinSpeed = 1;
        //Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.4f;
      }
    }

    public void Empire_StarDestroyer_00()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("IMPL")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 999
                                                              , true
                                                              );

      List<ShipSpawnEventArg> SDspawnlist = new List<ShipSpawnEventArg>();
      sspawn.TypeInfo = ActorTypeFactory.Get("VICT");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(1200, 850, -7000)
                                          , new TV_3DVECTOR(600, 720, -2000)
                                          , new TV_3DVECTOR(0, 500, 0)
                                          ));

      sspawn.TypeInfo = ActorTypeFactory.Get("ARQT");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(800, -100, -6800)
                                          , new TV_3DVECTOR(-1600, 350, -1200)
                                          , new TV_3DVECTOR(0, 350, 0)
                                          ));

      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(1600, 150, -7200)
                                          , new TV_3DVECTOR(2600, 150, -1700)
                                          , new TV_3DVECTOR(8000, 150, 2500)
                                          ));

      float delay = 0;
      foreach (ShipSpawnEventArg s in SDspawnlist)
      {
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, delay, s.Info);
        delay += 1.2f;
        ship.SetArmor(DamageType.ALL, 0.5f);
        foreach (ActorInfo a in ship.Children)
          a.SetArmor(DamageType.ALL, 0.25f);
      }
    }

    public void Empire_StarDestroyer_01()
    {
      // SD
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -500000);
      float creationTime = Game.GameTime;

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      List<float> createtime = new List<float>();

      positions.Add(new TV_3DVECTOR(-6000, 210, Manager.MinBounds.z - 500));
      createtime.Add(0);

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("IMPL"),
          Name = "",
          SidebarName = "",
          SpawnTime = creationTime + createtime[i],
          Faction = MainEnemyFaction,
          Position = v + hyperspaceInOffset,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new Wait(32f+10f)
                                     //, new HyperspaceIn(v + hyperspaceInOffset + new TV_3DVECTOR(0, 0, 100000))
                                     //, new HyperspaceIn(v + hyperspaceInOffset)
                                     , new HyperspaceOut()
                                     , new HyperspaceIn(v)
                                     , new SetMood(MoodStates.ALLY_SHIP_ARRIVED, true)
                                     , new SetMood(MoodStates.ENGAGEMENT, true)
                                     , new EnableSpawn(true)
                                     , new Move(new TV_3DVECTOR(v.x * 0.2f, v.y, -1000), ActorTypeFactory.Get("IMPL").MoveLimitData.MaxSpeed / 2)
                                     , new Rotate(new TV_3DVECTOR(-1600, -120, 6300), 0)
                                     , new Lock() }
        }.Spawn(this);

        ainfo.SpawnerInfo.SpawnsRemaining = 999;
        ainfo.SpawnerInfo.SpawnTypes = new string[] {
          "TIE",
          "TIE",
          "TIE",
          "TIE",
          "TIE",
          "TIE",
          "TIE",
          "TIE",
          "TIE",
          "TIE",
          "TIEI",
          "TIEI",
          "TIEI",
          "TIEI",
          "TIEI",
          "TIEI",
          "TIEI",
          "TIESA",
          "TIESA",
          "TIESA",
          "TIEA",
          "TIED",
        };

        ainfo.SetArmor(DamageType.ALL, 0.01f);
        foreach (ActorInfo a in ainfo.Children)
          a.SetArmor(DamageType.ALL, 0.001f);
      }
    }

    public void Empire_StarDestroyer_02()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("IMPL")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 999
                                                              , true
                                                              , -12
                                                              );

      List<ShipSpawnEventArg> SDspawnlist = new List<ShipSpawnEventArg>();
      sspawn.TypeInfo = ActorTypeFactory.Get("INTD");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(-5200, -500, -5000)
                                          , new TV_3DVECTOR(-3000, -500, 0)
                                          , new TV_3DVECTOR(0, -500, 500)
                                          ));

      sspawn.IntermissionMood = 0;
      sspawn.TypeInfo = ActorTypeFactory.Get("ACCL");
      SDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                          , new TV_3DVECTOR(-3200, -600, -4800)
                                          , new TV_3DVECTOR(3600, -600, 2200)
                                          , new TV_3DVECTOR(5000, -600, 5000)
                                          ));

      float delay = 0;
      foreach (ShipSpawnEventArg s in SDspawnlist)
      {
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, delay, s.Info);
        delay += 2.5f;
        ship.SetArmor(DamageType.ALL, 0.5f);
        foreach (ActorInfo a in ship.Children)
          a.SetArmor(DamageType.ALL, 0.1f);
      }
    }
  }
}
