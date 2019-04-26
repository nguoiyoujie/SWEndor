using MTV3D65;
using System.Collections.Generic;
using System;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using SWEndor.Weapons.Types;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.ActorTypes.Instances;

namespace SWEndor.Scenarios
{
  public class GSYavin : GameScenarioBase
  {
    public GSYavin(GameScenarioManager manager) : base(manager)
    {
      Name = "Battle of Yavin (WIP)";
      AllowedWings = new List<ActorTypeInfo> { this.GetEngine().ActorTypeFactory.Get("X-Wing"), this.GetEngine().ActorTypeFactory.Get("Millennium Falcon") };

      AllowedDifficulties = new List<string> { "easy"
                                               , "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private ActorInfo m_AYavin = null;
    private ActorInfo m_AYavin4 = null;
    private ActorInfo m_ADS = null;
    private ActorInfo m_ADS_Surface = null;
    private List<ActorInfo> m_ADS_SurfaceParts = new List<ActorInfo>();
    private ThreadSafeDictionary<int, ActorInfo> m_ADS_TrenchParts = new ThreadSafeDictionary<int, ActorInfo>();
    //private ActorInfo m_AStar = null;
    private List<object[]> m_pendingSDspawnlist = new List<object[]>();
    private Dictionary<string, ActorInfo> m_CriticalGroundObjects = new Dictionary<string, ActorInfo>();

    private float expiretime = 1800;
    private float target_distX = 118000;
    private float vader_distX = 91000;
    private float vaderend_distX = 107000;
    private float last_target_distX = 0;
    private float last_sound_distX = 0;
    List<string> names = new List<string>();

    private int m_PlayerID = -1;
    private int m_FalconID = -1;
    private int m_VaderID = -1;
    private int m_VaderEscort1ID = -1;
    private int m_VaderEscort2ID = -1;
    private float m_Player_DamageModifier = 1;
    private string m_Player_PrimaryWeapon = "";
    private string m_Player_SecondaryWeapon = "";

    private bool Stage5StartRun = false;
    private bool Stage5End = false;
    private bool Stage6VaderAttacking = false;
    private bool Stage6VaderEnd = false;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      Manager.Engine.PlayerInfo.Name = "Red Five";
    }

    public override void Launch()
    {
      base.Launch();

      Manager.SceneCamera.SetLocalPosition(0, 0, 0);
      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 10000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -15000);
      Manager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 10000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -15000);

      Manager.Engine.PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;

      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Rebel_HyperspaceIn);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 3.5f, Rebel_MakePlayer);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 3f, Rebel_RemoveTorps);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 7.5f, Rebel_GiveControl);

      Manager.AddEvent(Manager.Engine.Game.GameTime + 6f, Message_01_EnemyShipsInbound);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 1f, Empire_FirstWave);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 8f, Empire_FirstTIEWave);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 9f, Rebel_YWingsAttackScan);

      switch (Difficulty.ToLower())
      {
        case "mental":
          expiretime = Manager.Engine.Game.GameTime + 1230;
          break;
        case "hard":
          expiretime = Manager.Engine.Game.GameTime + 1530;
          break;
        case "normal":
          expiretime = Manager.Engine.Game.GameTime + 1530;
          break;
        case "easy":
        default:
          expiretime = Manager.Engine.Game.GameTime + 1830;
          break;
      }

      Manager.AddEvent(expiretime, Message_100_RebelBaseInRange);

      if (expiretime - Manager.Engine.Game.GameTime > 60)
        Manager.AddEvent(expiretime - 60, Message_101_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 120)
        Manager.AddEvent(expiretime - 120, Message_102_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 180)
        Manager.AddEvent(expiretime - 180, Message_103_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 300)
        Manager.AddEvent(expiretime - 300, Message_105_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 600)
        Manager.AddEvent(expiretime - 600, Message_110_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 900)
        Manager.AddEvent(expiretime - 900, Message_115_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 1200)
        Manager.AddEvent(expiretime - 1200, Message_120_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 1500)
        Manager.AddEvent(expiretime - 1500, Message_125_RebelBase);

      if (expiretime - Manager.Engine.Game.GameTime > 1800)
        Manager.AddEvent(expiretime - 1800, Message_130_RebelBase);

      Manager.Engine.PlayerInfo.Lives = 4;
      Manager.Engine.PlayerInfo.ScorePerLife = 1000000;
      Manager.Engine.PlayerInfo.ScoreForNextLife = 1000000;

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

      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Manager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      //Manager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      Manager.Engine.SoundManager.SetMusic("battle_1_1");
      Manager.Engine.SoundManager.SetMusicLoop("battle_1_4");

      Manager.IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Gold", new TV_COLOR(0.8f, 0.3f, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire_DeathStarDefenses", new TV_COLOR(0.1f, 0.8f, 0, 1)).AutoAI = true;

      FactionInfo.Factory.Get("Rebels").Allies.Add(FactionInfo.Factory.Get("Rebels_Gold"));
      FactionInfo.Factory.Get("Rebels_Gold").Allies.Add(FactionInfo.Factory.Get("Rebels"));

      FactionInfo.Factory.Get("Empire").Allies.Add(FactionInfo.Factory.Get("Empire_DeathStarDefenses"));
      FactionInfo.Factory.Get("Empire_DeathStarDefenses").Allies.Add(FactionInfo.Factory.Get("Empire"));

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");

      switch (Difficulty.ToLower())
      {
        case "mental":
          MainEnemyFaction.WingSpawnLimit = 28;
          break;
        case "hard":
          MainEnemyFaction.WingSpawnLimit = 24;
          break;
        case "normal":
          MainEnemyFaction.WingSpawnLimit = 20;
          break;
        case "easy":
        default:
          MainEnemyFaction.WingSpawnLimit = 16;
          break;
      }
    }

    public override void LoadScene()
    {
      base.LoadScene();

      // Create Yavin
      if (m_AYavin == null)
      {
        ActorCreationInfo aci_Yavin = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("Yavin"))
        {
          Position = new TV_3DVECTOR(0, 0, 18000),
          Rotation = new TV_3DVECTOR(90, 90, 0),
          InitialScale = new TV_3DVECTOR(4, 4, 4)
        };
        m_AYavin = ActorInfo.Create(this.GetEngine().ActorFactory, aci_Yavin);
      }

      // Create Yavin 4
      if (m_AYavin4 == null)
      {
        ActorCreationInfo aci_Yavin4 = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("Yavin4"))
        {
          Position = new TV_3DVECTOR(0, 800, -18000),
          Rotation = new TV_3DVECTOR(0, 0, 0),
        };
        m_AYavin4 = ActorInfo.Create(this.GetEngine().ActorFactory, aci_Yavin4);
      }

      // Create DeathStar
      if (m_ADS == null)
      {
        ActorCreationInfo aci_DS = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("DeathStar"))
        {
          Position = new TV_3DVECTOR(0, 800, 28000),
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Faction = MainEnemyFaction
        };
        m_ADS = ActorInfo.Create(this.GetEngine().ActorFactory, aci_DS);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      if (Manager.GetGameStateB("in_battle"))
      {
        float enemystrength = 0;
        if (StageNumber == 0)
        {
          StageNumber = 1;
        }
        else if (StageNumber == 1)
        {
          enemystrength = MainEnemyFaction.GetWings().Count;
          foreach (int actorID in MainEnemyFaction.GetShips())
          {
            ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
            if (actor != null)
              enemystrength += actor.StrengthFrac * 100 + (actor.SpawnerInfo != null ? actor.SpawnerInfo.SpawnsRemaining : 0);
          }

          if (!Manager.GetGameStateB("Stage1B"))
          {
            if (enemystrength < 50)
            {
              Manager.AddEvent(Manager.Engine.Game.GameTime + 10f, Message_02_MoreEnemyShipsInbound);
              Manager.AddEvent(Manager.Engine.Game.GameTime + 13f, Empire_SecondWave);
              Manager.SetGameStateB("Stage1B", true);
            }
          }
          else if (Manager.GetGameStateB("Stage1BBegin"))
          {
            if (enemystrength == 0 && !Manager.GetGameStateB("Stage1BEnd"))
            {
              Manager.AddEvent(Manager.Engine.Game.GameTime + 1.5f, Message_03_Clear);
              Manager.AddEvent(Manager.Engine.Game.GameTime + 2f, Rebel_Forward);
              Manager.AddEvent(Manager.Engine.Game.GameTime + 10f, Scene_Stage02_ApproachDeathStar);
              Manager.SetGameStateB("Stage1BEnd", true);
            }
          }
        }
        else if (StageNumber == 2)
        {
          Manager.UpdateActorLists(m_CriticalGroundObjects);
          enemystrength = MainEnemyFaction.GetWings().Count;
          enemystrength += MainEnemyFaction.GetShips().Count;
          enemystrength += m_CriticalGroundObjects.Count;

          if (enemystrength == 0 && !Manager.GetGameStateB("Stage2End"))
          {
            Manager.AddEvent(Manager.Engine.Game.GameTime + 1.5f, Message_05_Clear);
            Manager.AddEvent(Manager.Engine.Game.GameTime + 2f, Rebel_Forward);
            Manager.AddEvent(Manager.Engine.Game.GameTime + 10f, Scene_Stage03_Spawn);
            Manager.SetGameStateB("Stage2End", true);
          }
        }
        else if (StageNumber == 3)
        {
          Manager.UpdateActorLists(m_CriticalGroundObjects);
          enemystrength = MainEnemyFaction.GetWings().Count;
          enemystrength += MainEnemyFaction.GetShips().Count;
          enemystrength += m_CriticalGroundObjects.Count;

          if (enemystrength == 0 && !Manager.GetGameStateB("Stage3End"))
          {
            Manager.AddEvent(Manager.Engine.Game.GameTime + 1.5f, Message_07_Clear);
            Manager.AddEvent(Manager.Engine.Game.GameTime + 2f, Rebel_Forward);
            Manager.AddEvent(Manager.Engine.Game.GameTime + 10f, Scene_Stage04_Spawn);
            Manager.SetGameStateB("Stage3End", true);
          }
        }
        else if (StageNumber == 4)
        {
          Manager.UpdateActorLists(m_CriticalGroundObjects);
          enemystrength = MainEnemyFaction.GetWings().Count;
          enemystrength += MainEnemyFaction.GetShips().Count;
          enemystrength += m_CriticalGroundObjects.Count;

          if (enemystrength == 0 && !Manager.GetGameStateB("Stage4End"))
          {
            Manager.AddEvent(Manager.Engine.Game.GameTime + 1.5f, Message_09_Clear);
            Manager.AddEvent(Manager.Engine.Game.GameTime + 2f, Rebel_Forward);
            Manager.AddEvent(Manager.Engine.Game.GameTime + 10f, Scene_Stage05_Spawn);
            Manager.SetGameStateB("Stage4End", true);
          }
        }
        else if (StageNumber == 5)
        {
          ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
          if (player != null)
          {
            player.CombatInfo.DamageModifier = 0.75f;

            /*
            if (Manager.Engine.Player.Actor != null && !Manager.GetGameStateB("Stage5StartRun"))
            { 
             this.GetEngine().ActionManager.ForceClearQueue(Manager.Engine.Player.Actor);
              if (Manager.Engine.Player.Actor.GetPosition().x > 250)
                this.GetEngine().ActionManager.QueueFirst(Manager.Engine.Player.Actor, new Move(new TV_3DVECTOR(0, 1500, 200), 1000, can_interrupt: false));
              else
                this.GetEngine().ActionManager.QueueNext(Manager.Engine.Player.Actor, new Move(new TV_3DVECTOR(Manager.MaxBounds.x - 500, -220, 0), 1000, can_interrupt: false));
            }
            else if (Manager.Engine.Player.Actor != null && Manager.GetGameStateB("Stage5StartRun") && (Manager.Engine.Player.Actor.CurrentAction is AttackActor || Manager.Engine.Player.Actor.CurrentAction is Wait))
            {
              this.GetEngine().ActionManager.ForceClearQueue(Manager.Engine.Player.Actor);
              this.GetEngine().ActionManager.QueueFirst(Manager.Engine.Player.Actor, new Move(new TV_3DVECTOR(Manager.Engine.Player.Actor.GetPosition().x + 2000, Manager.Engine.Player.Actor.GetPosition().y, Manager.Engine.Player.Actor.GetPosition().z), 1000, can_interrupt: false));
            }
            */

            if (player.GetPosition().x > Manager.MaxBounds.x - 500
             && player.GetPosition().y < -180
             && player.GetPosition().z < 120
             && player.GetPosition().z > -120
             && !Stage5StartRun)
            {
              Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_Stage05b_Spawn);
              Stage5StartRun = true;
              Manager.Engine.Screen2D.OverrideTargetingRadar = true;
            }
            else if (Stage5StartRun)
            {
              if (last_target_distX < player.GetPosition().x)
              {
                Manager.Engine.PlayerInfo.Score.Score += (player.GetPosition().x - last_target_distX) * 10;
                last_target_distX = player.GetPosition().x;
              }
              if (last_sound_distX < player.GetPosition().x && !Manager.IsCutsceneMode)
              {
                Manager.Engine.SoundManager.SetSound("button_3");
                last_sound_distX = player.GetPosition().x + 250;
              }
              Manager.Engine.Screen2D.TargetingRadar_text = string.Format("{0:00000000}", (target_distX - player.GetPosition().x) * 30);
              Scene_Stage05b_ContinuouslySpawnRoute(null);

              if (player.GetPosition().x > vader_distX 
                && player.ActorState == ActorState.NORMAL
                && !Stage5End)
              {
                Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_Stage06_Vader);
                Stage5End = true;
              }
            }
          }
        }
        else if (StageNumber == 6)
        {
          ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
          if (player != null)
          {
            if (!Stage6VaderEnd)
            {
              if (Manager.Engine.Screen2D.ShowRadar)
                player.CombatInfo.DamageModifier = 2.5f;
              else
                player.CombatInfo.DamageModifier = 1f;
            }

            if (player != null)
            {
              if (last_target_distX < player.GetPosition().x)
              {
                Manager.Engine.PlayerInfo.Score.Score += (player.GetPosition().x - last_target_distX) * 10;
                last_target_distX = player.GetPosition().x;
              }

              if (last_sound_distX < player.GetPosition().x && !Manager.IsCutsceneMode)
              {
                Manager.Engine.SoundManager.SetSound("button_3");
                last_sound_distX = player.GetPosition().x + 250;
              }

              Manager.Engine.Screen2D.TargetingRadar_text = string.Format("{0:00000000}", (target_distX - player.GetPosition().x) * 30);
              Scene_Stage05b_ContinuouslySpawnRoute(null);

              if (player.GetPosition().x > vaderend_distX 
                && player.ActorState == ActorState.NORMAL 
                && !Stage6VaderEnd)
              {
                Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_Stage06_VaderEnd);
                Stage6VaderEnd = true;
                Rebel_RemoveTorps(null);
              }

              ActorInfo vader = this.GetEngine().ActorFactory.Get(m_VaderID);
              ActorInfo vaderEscort1 = this.GetEngine().ActorFactory.Get(m_VaderEscort1ID);
              ActorInfo vaderEscort2 = this.GetEngine().ActorFactory.Get(m_VaderEscort2ID);

              if (vader != null 
                && Stage6VaderAttacking)
              {
                vader?.SetLocalPosition(player.GetPosition().x - 1700, -200, 0);
                vaderEscort1?.SetLocalPosition(player.GetPosition().x - 1700, -220, 75);
                vaderEscort2?.SetLocalPosition(player.GetPosition().x - 1700, -220, -75);
              }
            }
          }
        }
      }


      if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.GetShips().Count < 5)
      {
        Empire_StarDestroyer_Spawn(m_pendingSDspawnlist[0]);
        m_pendingSDspawnlist.RemoveAt(0);
      }

      if (!Stage5StartRun)
      {
        if (Manager.Scenario.TimeSinceLostWing < Manager.Engine.Game.GameTime || Manager.Engine.Game.GameTime % 0.2f > 0.1f)
        {
          Manager.Line1Text = string.Format("WINGS: {0}", MainAllyFaction.GetWings().Count);
        }
        else
        {
          Manager.Line1Text = string.Format("");
        }
      }
      else
      {
        Manager.Line1Text = Manager.Engine.Screen2D.TargetingRadar_text;
      }

      Manager.Line2Text = string.Format("TIME: {0:00}:{1:00}", (int)(expiretime - Manager.Engine.Game.GameTime) / 60, (int)(expiretime - Manager.Engine.Game.GameTime) % 60);
      if ((int)(expiretime - Manager.Engine.Game.GameTime) / 60 < 4)
      {
        Manager.Line2Color = new TV_COLOR(1, 0.3f, 0.3f, 1);
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AYavin != null && m_AYavin.CreationState == CreationState.ACTIVE)
      {
        if (StageNumber < 2)
        {
          float x_yv = (Manager.Engine.PlayerInfo.Position.y > 0) ? (Manager.Engine.PlayerInfo.Position.y / 6f) - 18000.0f : (Manager.Engine.PlayerInfo.Position.y / 2.5f) - 18000.0f;
          float y_yv = Manager.Engine.PlayerInfo.Position.x / 1.2f;
          float z_yv = Manager.Engine.PlayerInfo.Position.z / 1.2f;
          m_AYavin.SetLocalPosition(x_yv, y_yv, z_yv);
        }
        else
        {
          float x_yv = Manager.Engine.PlayerInfo.Position.x / 1.2f;
          float y_yv = 20000.0f;
          float z_yv = Manager.Engine.PlayerInfo.Position.z / 1.2f;
          m_AYavin.SetLocalPosition(x_yv, y_yv, z_yv);
        }
      }
      if (m_AYavin4 != null && m_AYavin4.CreationState == CreationState.ACTIVE)
      {
        float x_y4 = Manager.Engine.PlayerInfo.Position.x / 10f;
        float y_y4 = Manager.Engine.PlayerInfo.Position.y / 2f;
        float z_y4 = (Manager.Engine.PlayerInfo.Position.z > 0) ? Manager.Engine.PlayerInfo.Position.z / 1.5f + 30000f : Manager.Engine.PlayerInfo.Position.z / 100f + 30000f;
        m_AYavin4.SetLocalPosition(x_y4, y_y4, z_y4);
      }
      if (m_ADS != null && m_ADS.CreationState == CreationState.ACTIVE)
      {
        float x_ds = Manager.Engine.PlayerInfo.Position.x / 5f;
        float y_ds = (Manager.Engine.PlayerInfo.Position.y / 1.5f) + 3200.0f;
        float z_ds = (Manager.Engine.PlayerInfo.Position.z > 0) ? Manager.Engine.PlayerInfo.Position.z / 1.5f - 30000f : Manager.Engine.PlayerInfo.Position.z / 100f - 30000f;
        m_ADS.SetLocalPosition(x_ds, y_ds, z_ds);
      }
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn(object[] param)
    {
      //ActorCreationInfo acinfo;
      ActorInfo ainfo;

      Manager.SceneCamera.SetLocalPosition(150, 100, Manager.MaxBounds.z - 1000);

      // Player X-Wing
      ainfo = new ActorSpawnInfo
      {
        Type = Manager.Engine.PlayerInfo.ActorType,
        Name = "(Player)",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = MainAllyFaction,
        Position = new TV_3DVECTOR(0, 0, Manager.MaxBounds.z - 150),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock()
                                  , new Move(new TV_3DVECTOR(Manager.Engine.Random.Next(-5, 5), Manager.Engine.Random.Next(-5, 5), Manager.MaxBounds.z - 150 - 4500)
                                                    , Manager.Engine.PlayerInfo.ActorType.MaxSpeed)},
        Registries = null
      }.Spawn(this);

      Manager.CameraTargetActor = ainfo;
      Manager.Engine.PlayerInfo.TempActorID = ainfo.ID;

      // X-Wings x21, Y-Wing x8
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 29; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Manager.Engine.Random.Next(-2800, -80), Manager.Engine.Random.Next(-200, 200), Manager.MaxBounds.z + Manager.Engine.Random.Next(-2200, -150)));
        else
          positions.Add(new TV_3DVECTOR(Manager.Engine.Random.Next(80, 2800), Manager.Engine.Random.Next(-200, 200), Manager.MaxBounds.z + Manager.Engine.Random.Next(-2200, -150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        if (i < 21)
        {
          ainfo = new ActorSpawnInfo
          {
            Type = this.GetEngine().ActorTypeFactory.Get("X-Wing"),
            Name = names[i],
            RegisterName = "",
            SidebarName = names[i],
            SpawnTime = Manager.Engine.Game.GameTime,
            Faction = MainAllyFaction,
            Position = v,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock()
                                       , new Move(new TV_3DVECTOR(v.x + Manager.Engine.Random.Next(-5, 5), v.y + Manager.Engine.Random.Next(-5, 5), v.z - 4500)
                                       , this.GetEngine().ActorTypeFactory.Get("X-Wing").MaxSpeed)},
            Registries = null
          }.Spawn(this);

          ainfo.CombatInfo.DamageModifier = 0.75f;
        }
        else
        {
          ainfo = new ActorSpawnInfo
          {
            Type = this.GetEngine().ActorTypeFactory.Get("Y-Wing"),
            Name = names[i],
            RegisterName = "",
            SidebarName = names[i],
            SpawnTime = Manager.Engine.Game.GameTime,
            Faction = MainAllyFaction,
            Position = v,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock()
                                       , new Move(new TV_3DVECTOR(v.x + Manager.Engine.Random.Next(-5, 5), v.y + Manager.Engine.Random.Next(-5, 5), v.z - 4500)
                                       , this.GetEngine().ActorTypeFactory.Get("Y-Wing").MaxSpeed)},
            Registries = null
          }.Spawn(this);

          ainfo.CombatInfo.DamageModifier = 0.6f;
        }
      }
    }

    public void Rebel_RemoveTorps(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (actor.TypeInfo is YWingATI)
          {
            foreach (KeyValuePair<string, WeaponInfo> kvp in actor.WeaponSystemInfo.Weapons)
            {
              if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
              {
                kvp.Value.Ammo = 1;
                kvp.Value.MaxAmmo = 1;
              }
            }
          }
          else
          {
            foreach (KeyValuePair<string, WeaponInfo> kvp in actor.WeaponSystemInfo.Weapons)
            {
              if (kvp.Key.Contains("torp") || kvp.Key.Contains("ion"))
              {
                kvp.Value.Ammo = 0;
                kvp.Value.MaxAmmo = 0;
              }
            }
            actor.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
            actor.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
          }
        }
      }

      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        if (Stage6VaderEnd)
        {
          player.MovementInfo.MinSpeed = 400;
          player.MovementInfo.MaxSpeed = 400;
          player.CombatInfo.DamageModifier = 0.5f;
          player.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", new XWingTorpWeapon() }
                                                        , {"laser", new XWingLaserWeapon() }
                                                        };
          player.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
          player.WeaponSystemInfo.SecondaryWeapons = new string[] { "4:laser", "1:torp" };
          player.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:laser" };
        }
        else if (Stage5StartRun)
        {
          player.MovementInfo.MinSpeed = 400;
          player.MovementInfo.MaxSpeed = 400;
          //m_Player.DamageModifier = 0.5f;
        }
        else if (StageNumber > 1)
        {
          player.MovementInfo.MinSpeed = player.MovementInfo.MaxSpeed * 0.75f;
        }
      }
      Manager.Engine.PlayerInfo.ResetPrimaryWeapon();
      Manager.Engine.PlayerInfo.ResetSecondaryWeapon();
    }

    public void Rebel_YWingsAttackScan(object[] param)
    {
      if (MainEnemyFaction.GetShips().Count > 0)
      {
        foreach (int actorID in MainAllyFaction.GetWings())
        {
          ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
          if (actor != null)
          {
            if (actor.TypeInfo is YWingATI || actor.TypeInfo is BWingATI)
            {
              int rsID = MainEnemyFaction.GetShips()[Manager.Engine.Random.Next(0, MainEnemyFaction.GetShips().Count)];
              ActorInfo rs = this.GetEngine().ActorFactory.Get(actorID);
              {
                foreach (int i in rs.GetAllChildren(1))
                {
                  ActorInfo rc = this.GetEngine().ActorFactory.Get(i);
                  if (rc.RegenerationInfo.ParentRegenRate > 0)
                    if (Manager.Engine.Random.NextDouble() > 0.4f)
                      rsID = rc.ID;
                }
              }

              this.GetEngine().ActionManager.ClearQueue(actorID);
              this.GetEngine().ActionManager.QueueLast(actorID, new AttackActor(rsID, -1, -1, false));
            }
          }
        }
      }

      Manager.AddEvent(Manager.Engine.Game.GameTime + 5f, Rebel_YWingsAttackScan);
    }

    public void Rebel_MakePlayer(object[] param)
    {
      Manager.Engine.PlayerInfo.ActorID = Manager.Engine.PlayerInfo.TempActorID;
      if (Manager.Engine.PlayerInfo.Actor == null || Manager.Engine.PlayerInfo.Actor.CreationState == CreationState.DISPOSED)
      {
        if (Manager.Engine.PlayerInfo.Lives > 0)
        {
          Manager.Engine.PlayerInfo.Lives--;

          TV_3DVECTOR pos = new TV_3DVECTOR();
          TV_3DVECTOR rot = new TV_3DVECTOR();

          if (Stage5StartRun)
          {
            rot = new TV_3DVECTOR(0, 90, 0);

            if (StageNumber == 6)
            {
              pos = new TV_3DVECTOR(vader_distX - 5000, -200, 0);
              Stage6VaderEnd = false;
              Stage6VaderAttacking = false;
              Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_Stage06_Vader);
            }
            else
            {
              pos = new TV_3DVECTOR(last_target_distX - 5000, -200, 0);
            }
          }
          else
          {
            pos = new TV_3DVECTOR(0, 200, Manager.MaxBounds.z - 1000);
            rot = new TV_3DVECTOR(0, 180, 0);
          }

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = Manager.Engine.PlayerInfo.ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Manager.Engine.Game.GameTime,
            Faction = MainAllyFaction,
            Position = pos,
            Rotation = rot,
            Actions = null,
            Registries = null
          }.Spawn(this);

          Manager.Engine.PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_PlayerID = Manager.Engine.PlayerInfo.ActorID;
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Rebel_RemoveTorps);
    }

    public void Rebel_GiveControl(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
        {
          this.GetEngine().ActionManager.UnlockOne(actorID);
          actor.ActorState = ActorState.NORMAL;
          actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
        }
      }
      Manager.Engine.PlayerInfo.IsMovementControlsEnabled = true;
      Manager.SetGameStateB("in_battle", true);
    }

    public void Rebel_Forward(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
        {
          this.GetEngine().ActionManager.ForceClearQueue(actorID);
          this.GetEngine().ActionManager.QueueNext(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(0, 0, -20000), actor.MovementInfo.MaxSpeed));
          this.GetEngine().ActionManager.QueueNext(actorID, new Lock());
        }
      }
    }

    public void Rebel_Reposition(object[] param)
    {
      int sw = -1;
      float x = 0;
      float y = 0;
      float z = Manager.MaxBounds.z - 150 ;
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
        {
          this.GetEngine().ActionManager.ForceClearQueue(actorID);
          if (actor.Name == "(Player)")
          {
            actor.SetLocalPosition(0, 100, Manager.MaxBounds.z - 150);
            actor.SetLocalRotation(0, 180, 0);
            actor.MovementInfo.ResetTurn();
            actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
            this.GetEngine().ActionManager.QueueNext(actorID, new Wait(5));
          }
          else
          {
            x = Manager.Engine.Random.Next(80, 1800) * sw;
            y = Manager.Engine.Random.Next(-40, 100);
            z = Manager.MaxBounds.z + Manager.Engine.Random.Next(-1800, -150);
            sw = -sw;
            actor.SetLocalPosition(x, y, z);
            actor.SetLocalRotation(0, 180, 0);
            actor.MovementInfo.ResetTurn();
            actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
            this.GetEngine().ActionManager.QueueNext(actorID, new Wait(5));
          }
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

      ActorTypeInfo type = (ActorTypeInfo)param[0];
      TV_3DVECTOR position = (TV_3DVECTOR)param[1];
      TV_3DVECTOR targetposition = (TV_3DVECTOR)param[2];
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -25000);

      ActorInfo ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = "",
        RegisterName = "",
        SidebarName = type.Name.Substring(0, type.Name.IndexOf(' ')).ToUpper(),
        SpawnTime = Manager.Engine.Game.GameTime,
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
      }.Spawn(this);

      ainfo.SetSpawnerEnable(true);
      if (ainfo.SpawnerInfo != null)
        if (param.GetLength(0) >= 4 && param[3] is int)
          ainfo.SpawnerInfo.SpawnsRemaining = (int)param[3];
    }
    
    public void Empire_FirstWave(object[] param)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-5000, -150, 6000), 9 });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(1500, 100, -4000), new TV_3DVECTOR(3000, 150, 5500), 9 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-6000, -150, 7000), 6 });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(1500, 100, -4000), new TV_3DVECTOR(4000, 100, 5000), 6 });
          break;
        case "normal":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-2500, -150, 7000), 9 });
          break;
        case "easy":
        default:
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-2500, -150, 7000), 6 });
          break;
      }
    }

    public void Empire_SecondWave(object[] param)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000), 12 });
          Empire_TIEWave(new object[] { 4 });
          break;
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000), 8 });
          Empire_TIEWave(new object[] { 2 });
          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000), 10 });
          Empire_TIEWave(new object[] { 3 });
          break;
      }
      Manager.SetGameStateB("Stage1BBegin", true);
    }

    public void Empire_FirstTIEWave(object[] param)
    {
      int count = 0;
      switch (Difficulty.ToLower())
      {
        case "mental":
          count = 5;
          break;
        case "hard":
          count = 4;
          break;
        case "normal":
          count = 3;
          break;
        case "easy":
        default:
          count = 2;
          break;
      }

      // TIEs
      for (int k = 1; k < count; k++)
      {
        float fx = Manager.Engine.Random.Next(-500, 500);
        float fy = Manager.Engine.Random.Next(-500, 0);
        float fz = Manager.Engine.Random.Next(-2500, 2500);

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

            new ActorSpawnInfo
            {
              Type = this.GetEngine().ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, fz - 4000 - k * 100),
              Rotation = new TV_3DVECTOR(),
              Actions = actions
            }.Spawn(this);
          }
        }
      }
    }

    public void Empire_TIEWave(object[] param)
    {
      int sets = 15;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 15;

      // TIE Fighters only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Manager.Engine.Random.Next(-2500, 2500);
        float fy = Manager.Engine.Random.Next(-200, 800);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = new ActionInfo[] { new Wait(15) };
            switch (Difficulty.ToLower())
            {
              case "mental":
                actions = new ActionInfo[] { new Wait(15), new Hunt(TargetType.FIGHTER) };
                break;
            }

            new ActorSpawnInfo
            {
              Type = this.GetEngine().ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 5000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions
            }.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_Towers01(object[] param)
    {
      float dist = 2500;
      float height = Manager.MinBounds.y;

      for (int x = -2; x <= 2; x++)
        for (int z = -3; z <= -1; z++)
          Spawn_TowerRadarFormation(new TV_3DVECTOR(x * dist, 90 + height, z * dist));
    }

    public void Empire_Towers02(object[] param)
    {
      float dist = 2500;
      float height = Manager.MinBounds.y;

      for (int x = -3; x <= 3; x += 2)
        for (int z = -2; z <= -1; z++)
          Spawn_TowerRadarFormation(new TV_3DVECTOR(x * dist / 2, 90 + height, z * dist));
    }

    private void Spawn_TowerRadarFormation(TV_3DVECTOR position)
    {
      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("Radar Tower"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
        Position = position,
        Rotation = new TV_3DVECTOR()
      };

      ActorInfo a = asi.Spawn(this);
      m_CriticalGroundObjects.Add(a.Key, a);

      asi.Type = this.GetEngine().ActorTypeFactory.Get("Gun Tower");
      asi.Position = position + new TV_3DVECTOR(300, 0, 0);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(-300, 0, 0);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(0, 0, 300);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(0, 0, -300);
      asi.Spawn(this);
    }

    private void Spawn_TowerDeflectorFormation(TV_3DVECTOR position)
    {
      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("Deflector Tower"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
        Position = position,
        Rotation = new TV_3DVECTOR()
      };

      ActorInfo a = asi.Spawn(this);
      m_CriticalGroundObjects.Add(a.Key, a);

      asi.Type = this.GetEngine().ActorTypeFactory.Get("Gun Tower");
      asi.Position = position + new TV_3DVECTOR(500, 0, 0);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(-500, 0, 0);
      asi.Spawn(this);
    }


    public void Empire_Towers03(object[] param)
    {
      float dist = 2500;
      float height = Manager.MinBounds.y;

      for (int z = -3; z <= 0; z++)
      {
        int z0 = (z > 0) ? z : -z;
        for (int x = -z0; x <= z0; x++)
        {
          if (z == -2)
            Spawn_TowerDeflectorFormation(new TV_3DVECTOR(x * dist, 90 + height, z * dist));
          else
            Spawn_TowerRadarFormation(new TV_3DVECTOR(x * dist, 90 + height, z * dist));
        }
      }
    }

    private void Spawn_TrenchFormation(ActorTypeInfo type, TV_3DVECTOR position, int distance, int trench_slot = -1)
    {
      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = type,
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
        Position = position,
        Rotation = new TV_3DVECTOR()
      };
      asi.Position = position + new TV_3DVECTOR(0, 0, distance);
      ActorInfo ainfo = asi.Spawn(this);
      if (trench_slot >= 0)
        TrenchTurrets[trench_slot].Add(ainfo);

      asi.Position = position + new TV_3DVECTOR(0, 0, -distance);
      ainfo = asi.Spawn(this);
      if (trench_slot >= 0)
        TrenchTurrets[trench_slot].Add(ainfo);
    }

    public void Empire_Towers04(object[] param)
    {
      float dist = 1000;
      float height = -175;

      for (int x = -6; x <= 6; x++)
        Spawn_TrenchFormation(this.GetEngine().ActorTypeFactory.Get("Gun Tower"), new TV_3DVECTOR(x * dist, 30 + height, 0), 150);
    }

    #endregion

    #region Scene

    public void Scene_EnterCutscene(object[] param)
    {
      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_PrimaryWeapon = Manager.Engine.PlayerInfo.PrimaryWeapon;
        m_Player_SecondaryWeapon = Manager.Engine.PlayerInfo.SecondaryWeapon;
        m_Player_DamageModifier = player.CombatInfo.DamageModifier;
        player.CombatInfo.DamageModifier = 0;
        this.GetEngine().ActionManager.ForceClearQueue(m_PlayerID);
        this.GetEngine().ActionManager.QueueNext(m_PlayerID, new Lock());
      }
      Manager.Engine.PlayerInfo.ActorID = Manager.SceneCamera.ID;
      
      Manager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(object[] param)
    {
      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        Manager.Engine.PlayerInfo.ActorID = m_PlayerID;
        Manager.Engine.PlayerInfo.PrimaryWeapon = m_Player_PrimaryWeapon;
        Manager.Engine.PlayerInfo.SecondaryWeapon = m_Player_SecondaryWeapon;
        player.CombatInfo.DamageModifier = m_Player_DamageModifier;
        this.GetEngine().ActionManager.ForceClearQueue(m_PlayerID);
      }
      Manager.IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam(object[] param)
    {
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.SceneCamera.SetLocalPosition(1000, 300, -15000);
      Manager.SceneCamera.MovementInfo.MaxSpeed = 600;
      Manager.SceneCamera.MovementInfo.Speed = 600;
      Manager.CameraTargetActor = m_ADS;
    }


    public void Scene_Stage02_ApproachDeathStar(object[] param)
    {
      Scene_DeathStarCam(null);
      Manager.Engine.SoundManager.SetMusic("battle_1_1", false, 71250);

      Manager.AddEvent(Manager.Engine.Game.GameTime + 4.8f, Rebel_Reposition);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 5f, Scene_Stage02_Spawn);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 9f, Scene_ExitCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 11f, Message_04_Target);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
          actor.CombatInfo.Strength = actor.CombatInfo.MaxStrength;
      }

      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
          actor.Kill();
      }

      Manager.MaxBounds = new TV_3DVECTOR(10000, 400, 8000);
      Manager.MinBounds = new TV_3DVECTOR(-10000, -175, -12000);
      Manager.MaxAIBounds = new TV_3DVECTOR(8000, 400, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-8000, -160, -10000);
    }

    public void Scene_ClearGroundObjects(object[] param)
    {
      foreach (int actorID in MainEnemyFaction.GetStructures())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
          actor.Kill();
      }
    }

    public void Scene_Stage02_Spawn(object[] param)
    {
      StageNumber = 2;
      Rebel_RemoveTorps(null);

      Manager.SceneCamera.SetLocalPosition(-550, -130, Manager.MaxBounds.z - 1500);

      m_ADS_Surface = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("Surface003_00ATI"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      }.Spawn(this);

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("Surface003_00ATI"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      };

      for (int x = -5 ; x <= 5; x++ )
        for (int z = -5; z <= 5; z++)
        {
          asi.Type = ((x + z) % 2 == 1) ? (ActorTypeInfo)this.GetEngine().ActorTypeFactory.Get("Surface001_00ATI") : this.GetEngine().ActorTypeFactory.Get("Surface001_01ATI");
          asi.Position = new TV_3DVECTOR(x * 4000, -173, z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
        }

      m_ADS.Kill();
      m_AYavin.SetLocalRotation(0, 0, 180);
      m_AYavin4.Kill();
      Manager.SceneCamera.MovementInfo.MaxSpeed = 450;
      Manager.SceneCamera.MovementInfo.Speed = 450;

      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      Manager.CameraTargetActor = player;

      //Empire_TIEWave(null);
      Empire_Towers01(null);
    }

    public void Scene_Stage03_Spawn(object[] param)
    {
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Rebel_Reposition);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 5f, Scene_ExitCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 6.5f, Message_06_TIE);


      StageNumber = 3;
      Manager.SceneCamera.SetLocalPosition(600, 130, Manager.MaxBounds.z - 1000);

      Manager.SceneCamera.MovementInfo.MaxSpeed = 450;
      Manager.SceneCamera.MovementInfo.Speed = 450;
      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      Manager.CameraTargetActor = player;

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
          actor.CombatInfo.Strength += 0.35f * actor.CombatInfo.MaxStrength;
      }

      Scene_ClearGroundObjects(null);
      switch (Difficulty.ToLower())
      {
        case "easy":
          Empire_TIEWave(new object[] { 5 });
          break;
        case "mental":
          Empire_TIEWave(new object[] { 8 });
          Manager.AddEvent(Manager.Engine.Game.GameTime + 43f, Empire_TIEWave);
          Manager.AddEvent(Manager.Engine.Game.GameTime + 45f, Message_06_TIE);
          break;
        case "hard":
          Empire_TIEWave(new object[] { 7 });
          Manager.AddEvent(Manager.Engine.Game.GameTime + 43f, Empire_TIEWave);
          Manager.AddEvent(Manager.Engine.Game.GameTime + 45f, Message_06_TIE);
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
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Rebel_Reposition);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 5f, Scene_ExitCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 6.5f, Message_08_Target);

      ActorTypeInfo type = this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer");
      TV_3DVECTOR position = new TV_3DVECTOR(2000, 750, -8000);
      TV_3DVECTOR targetposition = new TV_3DVECTOR(-4000, 1050, 1000);
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -25000);

      ActorInfo ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = MainEnemyFaction,
        Position = position + hyperspaceInOffset,
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[]
                     {
                                   new HyperspaceIn(position)
                                   , new Move(targetposition, type.MaxSpeed)
                                   , new HyperspaceOut()
                                   , new Delete()
                     }
      }.Spawn(this);

      ainfo.SetSpawnerEnable(true);
      if (ainfo.SpawnerInfo != null)
          ainfo.SpawnerInfo.NextSpawnTime = Manager.Engine.Game.GameTime + 3f;

      StageNumber = 4;
      Manager.SceneCamera.SetLocalPosition(1000, 30, -2000);

      Manager.SceneCamera.MovementInfo.MaxSpeed = 750;
      Manager.SceneCamera.MovementInfo.Speed = 750;
      Manager.CameraTargetActor = ainfo;

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
          actor.CombatInfo.Strength += 0.35f * actor.CombatInfo.MaxStrength;
      }

      Scene_ClearGroundObjects(null);
      Empire_Towers03(null);
    }

    public void Scene_Stage05_Spawn(object[] param)
    {
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Rebel_Reposition);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 5f, Scene_ExitCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 6.5f, Message_10_BeginRun);

      StageNumber = 5;
      Manager.SceneCamera.SetLocalPosition(550, -130, -1500);
      Manager.Engine.SoundManager.SetMusic("battle_1_2", true);
      Manager.MaxAIBounds = Manager.MaxAIBounds + new TV_3DVECTOR(2500, 0, 0);

      foreach (ActorInfo a in m_ADS_SurfaceParts)
      {
        a.Kill();
      }
      m_ADS_SurfaceParts.Clear();

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("Surface003_00ATI"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      };

      for (int x = -5; x <= 5; x++)
        for (int z = 0; z <= 3; z++)
        {
          asi.Type = ((x + z) % 2 == 1) ? (ActorTypeInfo)this.GetEngine().ActorTypeFactory.Get("Surface001_00ATI") : this.GetEngine().ActorTypeFactory.Get("Surface001_01ATI");
          asi.Position = new TV_3DVECTOR(x * 4000, -173, 2250 + z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
          asi.Position = new TV_3DVECTOR(x * 4000, -173, -2250 + -z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
        }

      for (int x = -20; x <= 20; x++)
      {
        asi.Type = this.GetEngine().ActorTypeFactory.Get("Surface002_99ATI");
        asi.Position = new TV_3DVECTOR(x * 1000, -175, 0);
        m_ADS_SurfaceParts.Add(asi.Spawn(this));
      }

      Manager.MaxBounds = new TV_3DVECTOR(7500, 300, 8000);
      Manager.MinBounds = new TV_3DVECTOR(-7500, -400, -12000);
      Manager.MaxAIBounds = new TV_3DVECTOR(7000, 300, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-7000, -160, -10000);

      m_ADS_Surface.Kill();
      Scene_ClearGroundObjects(null);

      Manager.SceneCamera.MovementInfo.MaxSpeed = 450;
      Manager.SceneCamera.MovementInfo.Speed = 450;
      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      Manager.CameraTargetActor = player;

      Empire_Towers04(null);
    }

    public void Scene_Stage05b_Spawn(object[] param)
    {
      DeathCamMode = DeathCamMode.FOLLOW;

      Manager.MaxBounds = new TV_3DVECTOR(5000000, -185, 1000);
      //Manager.MinBounds = new TV_3DVECTOR(vader_distX - 1000, -400, -1000);
      Manager.MinBounds = new TV_3DVECTOR(7000, -400, -1000);
      Manager.MaxAIBounds = new TV_3DVECTOR(5000000, 300, 2000);
      Manager.MinAIBounds = new TV_3DVECTOR(0, -400, -2000);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (!actor.IsPlayer())
          {
            actor.Faction = FactionInfo.Neutral;
            actor.Kill();
          }
        }
      }
      //m_Player.SetLocalPosition(7050, m_Player.GetLocalPosition())

      foreach (ActorInfo a in m_ADS_SurfaceParts)
      {
        a.Kill();
      }
      m_ADS_SurfaceParts.Clear();
      Scene_ClearGroundObjects(null);
      Rebel_RemoveTorps(null);

      Scene_Stage05b_SpawnRoute(null);
    }

    public void Scene_Stage05b_SpawnRoute(object[] param)
    {
      new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("Thermal Exhaust Port"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = MainEnemyFaction,
        Position = new TV_3DVECTOR(target_distX + 500, -385 + 47, 0),
        Rotation = new TV_3DVECTOR(0, 180, 0)
      }.Spawn(this);

      switch (Difficulty.ToLower())
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

    string[] TrenchTypes = new string[] { "Surface002_00ATI"
                                        , "Surface002_01ATI"
                                        , "Surface002_02ATI"
                                        , "Surface002_03ATI"
                                        , "Surface002_04ATI"
                                        , "Surface002_05ATI"
                                        , "Surface002_06ATI"
                                        , "Surface002_07ATI"
                                        , "Surface002_08ATI"
                                        , "Surface002_09ATI"
                                        , "Surface002_10ATI"
                                        , "Surface002_11ATI"
                                        , "Surface002_12ATI"
                                        , "Surface002_99ATI"
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

    private List<ActorInfo>[] TrenchTurrets = null;


    public void Scene_Stage05b_ContinuouslySpawnRoute(object[] param)
    {
      if (TrenchTurrets == null)
      {
        TrenchTurrets = new List<ActorInfo>[Trenches.Length];
        for (int i = 0; i < Trenches.Length; i++)
          TrenchTurrets[i] = new List<ActorInfo>();
      }

      int counter = (int)(Manager.Engine.PlayerInfo.Position.x - 8000) / 1000 - 7;
      if (counter < 0)
        counter = 0;

      int lasttravelledcounter = (int)(last_target_distX - 2000) / 1000 - 7;
      if (lasttravelledcounter < 0)
        lasttravelledcounter = 0;

      for (int i = counter - 4; i < counter + 20; i++)
      {
        ActorInfo a = m_ADS_TrenchParts.Get(i);
        if (a != null)
        {
          if (i < lasttravelledcounter && StageNumber == 5)
          {
            Type t = this.GetEngine().ActorTypeFactory.Get(TrenchTypes[Trenches[0]]).GetType();
            if (!(m_ADS_TrenchParts.Get(i).TypeInfo.GetType() == t) || i < counter)
            {
              m_ADS_TrenchParts.Get(i).Kill();
              foreach (ActorInfo turret in TrenchTurrets[i])
                turret.Kill();
              TrenchTurrets[i].Clear();

              ActorSpawnInfo asi = new ActorSpawnInfo
              {
                Type = this.GetEngine().ActorTypeFactory.Get(TrenchTypes[Trenches[0]]),
                Name = "",
                RegisterName = "",
                SidebarName = "",
                SpawnTime = Manager.Engine.Game.GameTime,
                Faction = FactionInfo.Neutral,
                Position = new TV_3DVECTOR(7000 + i * 1000, -173, 0),
                Rotation = new TV_3DVECTOR(0, 180, 0)
              };

              m_ADS_TrenchParts.Put(i, asi.Spawn(this));
            }
          }
        }

        a = m_ADS_TrenchParts.Get(i);
        if (a == null)
        {
          if (i >= lasttravelledcounter && i < Trenches.Length)
          {
            int trench = Trenches[i];
            if (trench < 0)
              trench = Manager.Engine.Random.Next(0, -trench + 1);

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = this.GetEngine().ActorTypeFactory.Get(TrenchTypes[trench]),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime,
              Faction = FactionInfo.Neutral,
              Position = new TV_3DVECTOR(7000 + i * 1000, -173, 0),
              Rotation = new TV_3DVECTOR(0, 180, 0)
            };

            m_ADS_TrenchParts.Put(i, asi.Spawn(this));

            if (i < 100 && i > 0 && i % 35 == 0)
              Spawn_TrenchFormation(this.GetEngine().ActorTypeFactory.Get("Deflector Tower"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 0), 175, i);
            else if (i > 10 && i < 96 && i % 10 == 0 || (i > 50 && i < 55))
              Spawn_TrenchFormation(this.GetEngine().ActorTypeFactory.Get("Gun Tower"), new TV_3DVECTOR(7000 + i * 1000, 30 - 175, 0), 150, i);
            else if (i == 100)
              Spawn_TrenchFormation(this.GetEngine().ActorTypeFactory.Get("Radar Tower"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 0), 170, i);


            asi = new ActorSpawnInfo
            {
              Type = this.GetEngine().ActorTypeFactory.Get("Deflector Tower"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime,
              Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
              Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40),
              Rotation = new TV_3DVECTOR()
            };

            switch (Difficulty.ToLower())
            {
              case "hard":
              case "mental":
                if (trench == 1 || trench == 4 || trench == 7)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 30 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 2 || trench == 3 || trench == 6)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 30 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 5)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 30 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 9)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 30 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 10)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 340, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 340, 30 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = this.GetEngine().ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 340, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
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
      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.SetLocalPosition(player.GetPosition().x, -220, 0);
        player.SetLocalRotation(0, 90, 0);
        player.MovementInfo.ResetTurn();
      }
    }

    public void Scene_Stage06_Vader(object[] param)
    {
      Scene_EnterCutscene(null);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 7.9f, Scene_Stage06_SetPlayer);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 8f, Scene_ExitCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 8f, Scene_Stage06_VaderAttack);

      StageNumber = 6;
      Manager.SceneCamera.SetLocalPosition(vader_distX - 2750, -225, 0);
      Manager.Engine.SoundManager.SetMusic("battle_1_3", true);

      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.SetLocalPosition(vader_distX, -220, 0);
        player.SetLocalRotation(0, 90, 0);
        player.MovementInfo.ResetTurn();
        this.GetEngine().ActionManager.ForceClearQueue(m_PlayerID);
        this.GetEngine().ActionManager.QueueNext(m_PlayerID, new Lock());

        player.CanEvade = false;
        player.CanRetaliate = false;
      }
      Scene_ClearGroundObjects(null);

      ActorInfo vader = this.GetEngine().ActorFactory.Get(m_VaderID);
      ActorInfo vaderE1 = this.GetEngine().ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo vaderE2 = this.GetEngine().ActorFactory.Get(m_VaderEscort2ID);
      ActorInfo falcon = this.GetEngine().ActorFactory.Get(m_FalconID);
      vader?.Kill();
      vaderE1?.Kill();
      vaderE2?.Kill();
      falcon?.Kill();

      m_VaderID = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("TIE Advanced X1"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire"),
        Position = new TV_3DVECTOR(vader_distX - 2000, 85, 0),
        Rotation = new TV_3DVECTOR(-10, 90, 0),
        Actions = new ActionInfo[] { new Move(new TV_3DVECTOR(vader_distX + 2000, -250, 0), 400)
                                             , new Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, 0), 400)
                                             , new Wait(3)
                                             , new AttackActor(m_PlayerID, 1500, 1, false, 9999) },
        Registries = null
      }.Spawn(this).ID;

      m_VaderEscort1ID = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("TIE"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire"),
        Position = new TV_3DVECTOR(vader_distX - 2100, 85, 75),
        Rotation = new TV_3DVECTOR(-10, 90, 0),
        Actions = new ActionInfo[] {  new Move(new TV_3DVECTOR(vader_distX + 2000, -250, 75), 400)
                                       , new Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, 75), 400)
                                       , new Lock() },
        Registries = null
      }.Spawn(this).ID;

      m_VaderEscort2ID = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("TIE"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire"),
        Position = new TV_3DVECTOR(vader_distX - 2100, 85, -75),
        Rotation = new TV_3DVECTOR(-10, 90, 0),
        Actions = new ActionInfo[] { new Move(new TV_3DVECTOR(vader_distX + 2000, -250, -75), 400)
                                       , new Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, -75), 400)
                                       , new Lock() },
        Registries = null
      }.Spawn(this).ID;

      vader = this.GetEngine().ActorFactory.Get(m_VaderID);
      vaderE1 = this.GetEngine().ActorFactory.Get(m_VaderEscort1ID);
      vaderE2 = this.GetEngine().ActorFactory.Get(m_VaderEscort2ID);
      vader.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"lsrb", WeaponFactory.Get("TIED_LASR") }
                                                        , {"laser", WeaponFactory.Get("TIED_LASR") }
                                                        };
      vader.WeaponSystemInfo.AIWeapons = new string[] { "1:laser", "1:lsrb" };
      vader.MovementInfo.MaxSpeed = 400;
      vader.MovementInfo.MinSpeed = 400;
      vader.CanEvade = false;
      vader.CanRetaliate = false;

      vaderE1.MovementInfo.MaxSpeed = 400;
      vaderE1.MovementInfo.MinSpeed = 400;
      vaderE1.CanEvade = false;
      vaderE1.CanRetaliate = false;

      vaderE2.MovementInfo.MaxSpeed = 400;
      vaderE2.MovementInfo.MinSpeed = 400;
      vaderE2.CanEvade = false;
      vaderE2.CanRetaliate = false;

      Manager.SceneCamera.MovementInfo.MaxSpeed = 425;
      Manager.SceneCamera.MovementInfo.Speed = 425;
      Manager.CameraTargetActor = player;
    }

    public void Scene_Stage06_VaderAttack(object[] param)
    {
      Stage6VaderAttacking = true;

      ActorInfo falcon = this.GetEngine().ActorFactory.Get(m_FalconID);
      falcon?.Kill();

      this.GetEngine().ActionManager.ForceClearQueue(m_VaderID);
      this.GetEngine().ActionManager.QueueNext(m_VaderID, new AttackActor(m_PlayerID, -1, -1, false, 9999));
      this.GetEngine().ActionManager.QueueNext(m_VaderID, new AttackActor(m_PlayerID, -1, -1, false, 9999));
      this.GetEngine().ActionManager.QueueNext(m_VaderID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      this.GetEngine().ActionManager.QueueNext(m_VaderID, new Lock());

      this.GetEngine().ActionManager.ForceClearQueue(m_VaderEscort1ID);
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort1ID, new AttackActor(m_PlayerID, -1, -1, false, 9999));
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort1ID, new AttackActor(m_PlayerID, -1, -1, false, 9999));
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort1ID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort1ID, new Lock());

      this.GetEngine().ActionManager.ForceClearQueue(m_VaderEscort2ID);
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort2ID, new AttackActor(m_PlayerID, -1, -1, false, 9999));
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort2ID, new AttackActor(m_PlayerID, -1, -1, false, 9999));
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort2ID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      this.GetEngine().ActionManager.QueueNext(m_VaderEscort2ID, new Lock());
    }

    public void Scene_Stage06_VaderEnd(object[] param)
    {
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 8f, Scene_ExitCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 7.9f, Scene_Stage06_SetPlayer);
      Manager.Engine.AtmosphereInfo.SetPos_Sun(new TV_3DVECTOR(1000, 100, 0));
      Manager.Engine.AtmosphereInfo.ShowSun = true;

      StageNumber = 6;
      Manager.SceneCamera.SetLocalPosition(vaderend_distX + 900, -365, 0);
      Manager.Engine.SoundManager.SetMusic("ds_end_1_1", true);

      ActorInfo vader = this.GetEngine().ActorFactory.Get(m_VaderID);
      ActorInfo vaderE1 = this.GetEngine().ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo vaderE2 = this.GetEngine().ActorFactory.Get(m_VaderEscort2ID);
      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);

      player.SetLocalPosition(vaderend_distX, -220, 0);
      player.SetLocalRotation(0, 90, 0);
      player.MovementInfo.ResetTurn();
      this.GetEngine().ActionManager.ForceClearQueue(m_PlayerID);
      this.GetEngine().ActionManager.QueueNext(m_PlayerID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      this.GetEngine().ActionManager.QueueNext(m_PlayerID, new Lock());

      vader.SetLocalRotation(0, 90, 0);

      ActorInfo falcon = new ActorSpawnInfo
      {
        Type = this.GetEngine().ActorTypeFactory.Get("Millennium Falcon"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = new TV_3DVECTOR(vaderend_distX + 2500, 185, 0),
        Rotation = new TV_3DVECTOR(0, -90, 0),
        Actions = new ActionInfo[] { new Move(new TV_3DVECTOR(vaderend_distX + 1300, 5, 0), 500, -1, false)
                                       , new AttackActor(m_VaderEscort1ID, -1, -1, false, 9999)
                                       , new AttackActor(m_VaderEscort2ID, -1, -1, false, 9999)
                                       , new Move(new TV_3DVECTOR(vaderend_distX - 5300, 315, 0), 500, -1, false)
                                       , new Delete() }
      }.Spawn(this);

      falcon.CanEvade = false;
      falcon.CanRetaliate = false;
      m_FalconID = falcon.ID;

        this.GetEngine().ActionManager.ForceClearQueue(m_VaderID);
        this.GetEngine().ActionManager.QueueNext(m_VaderID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        this.GetEngine().ActionManager.QueueNext(m_VaderID, new Lock());

        this.GetEngine().ActionManager.ForceClearQueue(m_VaderEscort1ID);
        this.GetEngine().ActionManager.QueueNext(m_VaderEscort1ID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        this.GetEngine().ActionManager.QueueNext(m_VaderEscort1ID, new Lock());

        this.GetEngine().ActionManager.ForceClearQueue(m_VaderEscort2ID);
        this.GetEngine().ActionManager.QueueNext(m_VaderEscort2ID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        this.GetEngine().ActionManager.QueueNext(m_VaderEscort2ID, new Lock());

      vader.HitEvents += Scene_Stage06_VaderFlee;
      vaderE1.HitEvents += Scene_Stage06_VaderFlee;
      vaderE2.HitEvents += Scene_Stage06_VaderFlee;

      Manager.SceneCamera.MovementInfo.MaxSpeed = 25;
      Manager.SceneCamera.MovementInfo.Speed = 25;
      Manager.CameraTargetActor = falcon;
    }

    public void Scene_Stage06_VaderFlee(object[] param)
    {
      if (Stage6VaderAttacking)
      {
        Stage6VaderAttacking = false;

        ActorInfo vader = this.GetEngine().ActorFactory.Get(m_VaderID);
        ActorInfo vaderE1 = this.GetEngine().ActorFactory.Get(m_VaderEscort1ID);
        ActorInfo vaderE2 = this.GetEngine().ActorFactory.Get(m_VaderEscort2ID);

        this.GetEngine().ActionManager.ForceClearQueue(m_VaderID);
        vader.MovementInfo.ApplyZBalance = false;
        vader.SetLocalRotation(-30, 85, 5);
        vader.CombatInfo.TimedLife = 999;
        vader.ActorState = ActorState.DYING;
        vaderE2.SetLocalRotation(-5, 93, 0);
        vaderE2.ActorState = ActorState.DYING;
        vaderE1.ActorState = ActorState.DYING;
      }
    }

    #endregion


    #region Text
    public void Message_01_EnemyShipsInbound(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Enemy signals are approaching your position.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_MoreEnemyShipsInbound(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Another enemy signal are en route to your position.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Clear(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Area cleared. Resume your attack on the Death Star.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_04_Target(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Eliminate all Radar Towers.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_05_Clear(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Area cleared. Proceed to the next sector.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_06_TIE(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Enemy fighters detected.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07_Clear(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Deflector Towers detected en route to the trench.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08_Target(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Destroy all Deflection Towers and Radar Towers.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_09_Clear(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: The path to the trench line is clear.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_10_BeginRun(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Luke, start your attack run.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_100_RebelBaseInRange(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is in range of the Death Star.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_101_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in ONE minute.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_102_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWO minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_103_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THREE minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_105_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIVE minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_110_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TEN minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_115_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIFTEEN minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_120_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_125_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY-FIVE minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_130_RebelBase(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THIRTY minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    #endregion
  }
}
