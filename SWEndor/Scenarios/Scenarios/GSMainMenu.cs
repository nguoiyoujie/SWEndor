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

      sceneid = Manager.Engine.Random.Next(0, 7);
      Manager.Engine.LandInfo.Enabled = false;
      Manager.Engine.AtmosphereInfo.LoadDefaults(true, true);
      Manager.Engine.AtmosphereInfo.SetPos_Sun(new TV_3DVECTOR(-1000, 250, 0));
      Manager.Engine.AtmosphereInfo.Enabled = true;
      Manager.Engine.AtmosphereInfo.ShowSun = true;
      Manager.Engine.AtmosphereInfo.ShowFlare = true;
    }

    public override void Launch()
    {
      base.Launch();
      if (Manager.GetGameStateB("in_menu"))
        return;

      Manager.SetGameStateB("in_menu", true);
      Manager.SceneCamera.SetLocalPosition(0, 0, 0);
      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 5000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -5000);
      Manager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 5000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -5000);

      Manager.Engine.PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;
      
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0f, Rebel_HyperspaceIn);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0f, Empire_TIEDefender);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.2f, Empire_TIEAvengers);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0f, Empire_StarDestroyer_01);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.5f, Rebel_BeginBattle);

      Manager.Engine.PlayerInfo.Lives = 2;
      Manager.Engine.PlayerInfo.ScorePerLife = 9999999;
      Manager.Engine.PlayerInfo.ScoreForNextLife = 9999999;

      //Manager.Engine.SoundManager.SetMusic("battle_1_1", false, 18600);
      //Manager.Engine.SoundManager.SetMusic("battle_1_1", false, 71250);
      //Manager.Engine.SoundManager.SetMusic("battle_1_1", false, 191000);
      //Manager.Engine.SoundManager.SetMusicLoop("battle_3_4");

      /*
      switch (sceneid)
      {
        // Endor reportorie
        case 0:
        default:
          Manager.Engine.SoundManager.SetMusic("credits_3_1");
          Manager.Engine.SoundManager.SetMusicLoop("battle_3_4");
          break;
        case 1:
          Manager.Engine.SoundManager.SetMusic("credits_3_1");
          Manager.Engine.SoundManager.SetMusicLoop("battle_3_3");
          break;
        case 2:
          Manager.Engine.SoundManager.SetMusic("credits_3_1");
          Manager.Engine.SoundManager.SetMusicLoop("battle_3_4");
          break;
        // Yavin reportorie
        case 3:
          Manager.Engine.SoundManager.SetMusic("credits_1_1");
          Manager.Engine.SoundManager.SetMusicLoop("battle_1_1", 18600);
          break;
        case 4:
          Manager.Engine.SoundManager.SetMusic("credits_1_1");
          Manager.Engine.SoundManager.SetMusicLoop("battle_1_4");
          break;
        // Hoth reportorie
        case 5:
          Manager.Engine.SoundManager.SetMusic("credits_2_1");
          Manager.Engine.SoundManager.SetMusicLoop("battle_2_1");
          break;
        case 6:
          Manager.Engine.SoundManager.SetMusic("credits_2_1");
          Manager.Engine.SoundManager.SetMusicLoop("battle_2_2");
          break;
      }
      */

      Manager.Engine.SoundManager.SetMusic("dynamic\\CHAL-IN", false, 1657);

      //Manager.Engine.SoundManager.SetMusic("trofix", false, 2142);
      //Manager.Engine.SoundManager.QueueMusic("trofix", 64286);
      //Manager.Engine.SoundManager.QueueMusic("rebfix", 2142);

      //Manager.Engine.SoundManager.QueueMusic("dynamic\\approach4", 0);
      //Manager.Engine.SoundManager.QueueMusic("dynamic\\approach4", 67935);
      //Manager.Engine.SoundManager.QueueMusic("dynamic\\wait", 0);
      //Manager.Engine.SoundManager.QueueMusic("dynamic\\rebel", 0);

      Manager.IsCutsceneMode = true;
    }
    public override void Unload()
    {
      base.Unload();
      //Manager.Engine.SoundManager.RemoveMusicSyncPoint("trofix");
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");

      MainAllyFaction.WingSpawnLimit = 12;
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
          acinfo = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("Endor"));
        }
        else if (sceneid <= 4)
        {
          acinfo = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("Yavin"));
        }
        else
        {
          acinfo = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("Hoth"));
        }

        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, -1200, 0);
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        acinfo.InitialScale = new TV_3DVECTOR(6, 6, 6);
        m_APlanet = ActorInfo.Create(this.GetEngine().ActorFactory, acinfo);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      if (Manager.CameraTargetActor == null || Manager.CameraTargetActor.CreationState != CreationState.ACTIVE)
      {
        List<int> list = new List<int>(MainEnemyFaction.GetWings());
        //List<int> list = new List<int>(MainAllyFaction.GetWings());
        if (list.Count > 0)
        {
          Manager.CameraTargetActor = this.GetEngine().ActorFactory.Get(list[Manager.Engine.Random.Next(0, list.Count)]);
        }
      }

      if (Manager.CameraTargetActor != null && Manager.CameraTargetActor.CreationState == CreationState.ACTIVE)
      {
        TV_3DVECTOR pos = Manager.CameraTargetActor.GetRelativePositionFUR(-300, 0, 0);
        Manager.SceneCamera.SetLocalPosition(pos.x, pos.y + 125, pos.z);
        Manager.CameraTargetActor.TypeInfo.ChaseCamera(Manager.SceneCamera);
        //Manager.CameraTargetActor.TypeInfo.ChaseCamera(Manager.CameraTargetActor);
      }

      if (Manager.GetGameStateB("in_battle"))
      {
        // TIE spawn
        if (TIESpawnTime < Manager.Engine.Game.GameTime)
        {
          if (MainEnemyFaction.GetWings().Count < 30)
          {
            TIESpawnTime = Manager.Engine.Game.GameTime + 10f;
            Empire_TIEWave_02(new object[] { 4 });
          }
        }

        // Rebel spawn
        if (RebelSpawnTime < Manager.Engine.Game.GameTime)
        {
          if (MainAllyFaction.GetWings().Count < 10)
          {
            RebelSpawnTime = Manager.Engine.Game.GameTime + 10f;
            Rebel_Wave(new object[] { 18 });
          }
        }

        if (MainAllyFaction.GetShips().Count < 3 && !Manager.GetGameStateB("rebels_fled"))
        {
          Manager.SetGameStateB("rebels_fled", true);
          Manager.AddEvent(Manager.Engine.Game.GameTime + 15f, Rebel_HyperspaceIn2);
          Rebel_HyperspaceOut(null);
        }
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_APlanet != null && m_APlanet.CreationState == CreationState.ACTIVE)
      {
        float x_en = Manager.Engine.PlayerInfo.Position.x / 1.2f;
        float y_en = (Manager.Engine.PlayerInfo.Position.y > 0) 
          ? (Manager.Engine.PlayerInfo.Position.y / 6f) - 9000.0f 
          : (Manager.Engine.PlayerInfo.Position.y / 2.5f) - 9000.0f;

        if (Manager.Engine.PlayerInfo.Position.z < -30000)
          y_en += (Manager.Engine.PlayerInfo.Position.z + 30000) * 25;

        float z_en = Manager.Engine.PlayerInfo.Position.z / 1.2f;
        m_APlanet.SetLocalPosition(x_en, y_en, z_en);
      }
    }

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      float creationTime = Manager.Engine.Game.GameTime;

      // Wings x24
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      for (int i = 0; i < 24; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Manager.Engine.Random.Next(-800, -80), Manager.Engine.Random.Next(-100, 100), Manager.Engine.Random.Next(-800, -150)));
        else
          positions.Add(new TV_3DVECTOR(Manager.Engine.Random.Next(80, 800), Manager.Engine.Random.Next(-100, 100), Manager.Engine.Random.Next(-800, -150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ActorTypeInfo[] atypes = new ActorTypeInfo[] { this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("X-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("X-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("A-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("A-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("Y-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("Y-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("B-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("B-Wing") };

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
          Actions = new ActionInfo[]{ new Move(new TV_3DVECTOR(v.x + Manager.Engine.Random.Next(-5, 5), v.y + Manager.Engine.Random.Next(-5, 5), -v.z - 750)
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
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Manager.Engine.Random.Next(-5, 5), v.y + Manager.Engine.Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = this.GetEngine().ActorTypeFactory.Get("Nebulon-B Frigate");

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
      positions.Add(new TV_3DVECTOR(100, 170, 7200));
      positions.Add(new TV_3DVECTOR(-1000, -60, 7700));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Manager.Engine.Random.Next(-5, 5), v.y + Manager.Engine.Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = this.GetEngine().ActorTypeFactory.Get("Corellian Corvette");

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
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Manager.Engine.Random.Next(-5, 5), v.y + Manager.Engine.Random.Next(-5, 5), -v.z - 6000);
        ActorTypeInfo at = this.GetEngine().ActorTypeFactory.Get("Transport");

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

    private void Rebel_HyperspaceOut(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null
          && actor.ActorState != ActorState.DYING 
          && actor.ActorState != ActorState.DEAD)
        {
          this.GetEngine().ActionManager.ForceClearQueue(actorID);
          this.GetEngine().ActionManager.QueueLast(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(18000, 0, -20000)
                                                , actor.MovementInfo.Speed
                                                , actor.TypeInfo.Move_CloseEnough));
          this.GetEngine().ActionManager.QueueLast(actorID, new HyperspaceOut());
          this.GetEngine().ActionManager.QueueLast(actorID, new Delete());
        }
      }
    }

    public void Rebel_HyperspaceIn2(object[] param)
    {
      ActorInfo ainfo;
      float creationTime = Manager.Engine.Game.GameTime;
      float creationDelay = 0.025f;
      TV_3DVECTOR position;
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      ActionInfo[] actions;
      FactionInfo faction = FactionInfo.Factory.Get("Rebels");
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(10000, 0, 10000);
      TV_3DVECTOR movedisp = new TV_3DVECTOR(3000, 0, 3000);
      ActorTypeInfo type;
      List<object[]> spawns = new List<object[]>();
      spawns.Add(new object[] { new TV_3DVECTOR(-4600, 150, 7300), this.GetEngine().ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-5000, 90, 7500), this.GetEngine().ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-5400, 150, 7700), this.GetEngine().ActorTypeFactory.Get("X-Wing") });
      spawns.Add(new object[] { new TV_3DVECTOR(-1600, -120, 6300), this.GetEngine().ActorTypeFactory.Get("Mon Calamari Capital Ship") });
      spawns.Add(new object[] { new TV_3DVECTOR(1400, -320, 8400), this.GetEngine().ActorTypeFactory.Get("Corellian Corvette") });
      spawns.Add(new object[] { new TV_3DVECTOR(-2400, 150, 6500), this.GetEngine().ActorTypeFactory.Get("Corellian Corvette") });

      foreach (object[] spawn in spawns)
      {
        type = (ActorTypeInfo)spawn[1];
        creationTime += creationDelay;
        position = (TV_3DVECTOR)spawn[0];
        if (type is ActorTypes.Groups.Fighter)
        {
          actions = new ActionInfo[] { new HyperspaceIn(position)
                                 , new Move(new TV_3DVECTOR(position.x + Manager.Engine.Random.Next(-5, 5), position.y + Manager.Engine.Random.Next(-5, 5), -position.z - 1500)
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
          Actions = actions
        }.Spawn(this);
      }
    }


    public void Rebel_Wave(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("Z-95")
                                                      , this.GetEngine().ActorTypeFactory.Get("X-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("X-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("A-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("A-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("Y-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("Y-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("B-Wing")
                                                      , this.GetEngine().ActorTypeFactory.Get("B-Wing") };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Manager.Engine.Random.Next(-2500, 2500);
        float fy = Manager.Engine.Random.Next(-500, 500);

        int n = k % tietypes.Length;

        new ActorSpawnInfo
        {
          Type = tietypes[n],
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Manager.Engine.Game.GameTime + t,
          Faction = MainAllyFaction,
          Position = new TV_3DVECTOR(fx, fy, Manager.MaxBounds.z + 1500),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Move(new TV_3DVECTOR(fx, fy, Manager.MaxBounds.z), tietypes[n].MaxSpeed)
                                            }
        }.Spawn(this);

        t += 1.5f;
      }
    }

    public void Rebel_BeginBattle(object[] param)
    {
      Manager.SetGameStateB("in_battle", true);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 10f, Empire_TIEWave_02);
    }

    public void Empire_TIEWave_02(object[] param)
    {
      int sets = 8;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 8;

      // TIEs
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { this.GetEngine().ActorTypeFactory.Get("TIE"), this.GetEngine().ActorTypeFactory.Get("TIE Interceptor") };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Manager.Engine.Random.Next(-2500, 2500);
        float fy = Manager.Engine.Random.Next(-500, 500);

        int n = Manager.Engine.Random.Next(0, tietypes.Length);
        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            new ActorSpawnInfo
            {
              Type = tietypes[n],
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 2500),
              Rotation = new TV_3DVECTOR(),
              Actions = new ActionInfo[] { new Wait(5) }
            }.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEDefender(object[] param)
    {
      // TID/D
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -100000);
      TV_3DVECTOR plus = new TV_3DVECTOR(3500, 500, 2750);
      float creationTime = Manager.Engine.Game.GameTime;

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
          Type = this.GetEngine().ActorTypeFactory.Get("TIE Defender"),
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

        ainfo.MovementInfo.MinSpeed = 0;
        ainfo.CombatInfo.DamageModifier = 0.1f;
      }
    }

    public void Empire_TIEAvengers(object[] param)
    {
      // TID/A
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -100000);
      TV_3DVECTOR plus = new TV_3DVECTOR(3500, 500, 2750);
      float creationTime = Manager.Engine.Game.GameTime;

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
          Type = this.GetEngine().ActorTypeFactory.Get("TIE Avenger"),
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

        ainfo.MovementInfo.MinSpeed = 0;
        ainfo.CombatInfo.DamageModifier = 0.1f;
      }
    }

    public void Empire_StarDestroyer_01(object[] param)
    {
      // SD
      ActorInfo ainfo;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(-22000, 0, -100000);
      float creationTime = Manager.Engine.Game.GameTime;

      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      List<float> createtime = new List<float>();

      positions.Add(new TV_3DVECTOR(-6000, 110, Manager.MinBounds.z - 500));
      createtime.Add(0);

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"),
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
                                     , new Move(new TV_3DVECTOR(v.x * 0.2f, v.y, -1000), this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed / 2)
                                     , new Rotate(new TV_3DVECTOR(-1600, -120, 6300), 0)
                                     , new Lock() }
        }.Spawn(this);
      }
    }
  }
}
