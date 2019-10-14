using MTV3D65;
using System.Collections.Generic;
using System;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Weapons;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI;
using SWEndor.ActorTypes.Components;
using SWEndor.Sound;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Scenarios
{
  public class GSYavin : GameScenarioBase
  {
    public GSYavin(GameScenarioManager manager) : base(manager)
    {
      Name = "Battle of Yavin (WIP) [Maintenance]";
      AllowedWings = new List<ActorTypeInfo> { ActorTypeFactory.Get("XWING"), ActorTypeFactory.Get("FALC") };

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
    private List<ActorInfo> m_ADS_SurfaceParts;
    private ThreadSafeDictionary<int, ActorInfo> m_ADS_TrenchParts;
    //private ActorInfo m_AStar = null;
    private List<ShipSpawnEventArg> m_pendingSDspawnlist;
    private HashSet<ActorInfo> m_CriticalGroundObjects;

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
    private int m_Player_PrimaryWeapon;
    private int m_Player_SecondaryWeapon;

    private bool Stage5StartRun = false;
    private bool Stage5End = false;
    private bool Stage6VaderAttacking = false;
    private bool Stage6VaderEnd = false;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Name = "Red Five";

      m_ADS_SurfaceParts = new List<ActorInfo>();
      m_ADS_TrenchParts = new ThreadSafeDictionary<int, ActorInfo>();
      m_pendingSDspawnlist = new List<ShipSpawnEventArg>();
      m_CriticalGroundObjects = new HashSet<ActorInfo>();
    }

    public override void Unload()
    {
      base.Unload();

      m_AYavin = null;
      m_AYavin4 = null;
      m_ADS = null;
      m_ADS_Surface = null;
      m_ADS_SurfaceParts = null;
      m_ADS_TrenchParts = null;
      m_pendingSDspawnlist = null;
      m_CriticalGroundObjects = null;
    }

    public override void Launch()
    {
      base.Launch();

      PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;

      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 10000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -15000);
      Manager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 10000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -15000);

      PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;

      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_HyperspaceIn);
      Manager.AddEvent(Game.GameTime + 3.5f, Rebel_MakePlayer);
      Manager.AddEvent(Game.GameTime + 3f, Rebel_RemoveTorps);
      Manager.AddEvent(Game.GameTime + 7.5f, Rebel_GiveControl);

      Manager.AddEvent(Game.GameTime + 6f, Message_01_EnemyShipsInbound);
      Manager.AddEvent(Game.GameTime + 1f, Empire_FirstWave);
      Manager.AddEvent(Game.GameTime + 8f, Empire_FirstTIEWave);
      Manager.AddEvent(Game.GameTime + 9f, Rebel_YWingsAttackScan);

      switch (Difficulty.ToLower())
      {
        case "mental":
          expiretime = Game.GameTime + 1230;
          break;
        case "hard":
          expiretime = Game.GameTime + 1530;
          break;
        case "normal":
          expiretime = Game.GameTime + 1530;
          break;
        case "easy":
        default:
          expiretime = Game.GameTime + 1830;
          break;
      }

      Manager.AddEvent(expiretime, Message_100_RebelBaseInRange);

      if (expiretime - Game.GameTime > 60)
        Manager.AddEvent(expiretime - 60, Message_101_RebelBase);

      if (expiretime - Game.GameTime > 120)
        Manager.AddEvent(expiretime - 120, Message_102_RebelBase);

      if (expiretime - Game.GameTime > 180)
        Manager.AddEvent(expiretime - 180, Message_103_RebelBase);

      if (expiretime - Game.GameTime > 300)
        Manager.AddEvent(expiretime - 300, Message_105_RebelBase);

      if (expiretime - Game.GameTime > 600)
        Manager.AddEvent(expiretime - 600, Message_110_RebelBase);

      if (expiretime - Game.GameTime > 900)
        Manager.AddEvent(expiretime - 900, Message_115_RebelBase);

      if (expiretime - Game.GameTime > 1200)
        Manager.AddEvent(expiretime - 1200, Message_120_RebelBase);

      if (expiretime - Game.GameTime > 1500)
        Manager.AddEvent(expiretime - 1500, Message_125_RebelBase);

      if (expiretime - Game.GameTime > 1800)
        Manager.AddEvent(expiretime - 1800, Message_130_RebelBase);

      PlayerInfo.Lives = 4;
      PlayerInfo.ScorePerLife = 1000000;
      PlayerInfo.ScoreForNextLife = 1000000;

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

      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1).GetIntColor();
      Manager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1).GetIntColor();
      //Manager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      SoundManager.SetMusic("battle_1_1");
      SoundManager.SetMusicLoop("battle_1_4");

      Manager.IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1).GetIntColor()).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Gold", new TV_COLOR(0.8f, 0.3f, 0, 1).GetIntColor()).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1).GetIntColor()).AutoAI = true;
      FactionInfo.Factory.Add("Empire_DeathStarDefenses", new TV_COLOR(0.1f, 0.8f, 0, 1).GetIntColor()).AutoAI = true;

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
      ActorCreationInfo aci_Yavin = new ActorCreationInfo(ActorTypeFactory.Get("YAVIN"))
      {
        Position = new TV_3DVECTOR(0, 0, 18000),
        Rotation = new TV_3DVECTOR(90, 90, 0),
        InitialScale = 4
      };
      m_AYavin = Engine.ActorFactory.Create(aci_Yavin);

      // Create Yavin 4
      ActorCreationInfo aci_Yavin4 = new ActorCreationInfo(ActorTypeFactory.Get("YAVIN4"))
      {
        Position = new TV_3DVECTOR(0, 800, -18000),
        Rotation = new TV_3DVECTOR(0, 0, 0),
      };
      m_AYavin4 = Engine.ActorFactory.Create(aci_Yavin4);

      // Create DeathStar
      ActorCreationInfo aci_DS = new ActorCreationInfo(ActorTypeFactory.Get("DSTAR"))
      {
        Position = new TV_3DVECTOR(0, 800, 28000),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Faction = MainEnemyFaction
      };
      m_ADS = Engine.ActorFactory.Create(aci_DS);
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
          enemystrength = MainEnemyFaction.WingCount;
          foreach (int actorID in MainEnemyFaction.GetShips())
          {
            ActorInfo actor = Engine.ActorFactory.Get(actorID);
            if (actor != null)
              enemystrength += actor.HP_Frac * 100 + (actor.SpawnerInfo.SpawnsRemaining);
          }

          if (!Manager.GetGameStateB("Stage1B"))
          {
            if (enemystrength < 50)
            {
              Manager.AddEvent(Game.GameTime + 10f, Message_02_MoreEnemyShipsInbound);
              Manager.AddEvent(Game.GameTime + 13f, Empire_SecondWave);
              Manager.SetGameStateB("Stage1B", true);
            }
          }
          else if (Manager.GetGameStateB("Stage1BBegin"))
          {
            if (enemystrength == 0 && !Manager.GetGameStateB("Stage1BEnd"))
            {
              Manager.AddEvent(Game.GameTime + 1.5f, Message_03_Clear);
              Manager.AddEvent(Game.GameTime + 2f, Rebel_Forward);
              Manager.AddEvent(Game.GameTime + 10f, Scene_Stage02_ApproachDeathStar);
              Manager.SetGameStateB("Stage1BEnd", true);
            }
          }
        }
        else if (StageNumber == 2)
        {
          Manager.UpdateActorLists(m_CriticalGroundObjects);
          enemystrength = MainEnemyFaction.WingCount;
          enemystrength += MainEnemyFaction.ShipCount;
          enemystrength += m_CriticalGroundObjects.Count;

          if (enemystrength == 0 && !Manager.GetGameStateB("Stage2End"))
          {
            Manager.AddEvent(Game.GameTime + 1.5f, Message_05_Clear);
            Manager.AddEvent(Game.GameTime + 2f, Rebel_Forward);
            Manager.AddEvent(Game.GameTime + 10f, Scene_Stage03_Spawn);
            Manager.SetGameStateB("Stage2End", true);
          }
        }
        else if (StageNumber == 3)
        {
          Manager.UpdateActorLists(m_CriticalGroundObjects);
          enemystrength = MainEnemyFaction.WingCount;
          enemystrength += MainEnemyFaction.ShipCount;
          enemystrength += m_CriticalGroundObjects.Count;

          if (enemystrength == 0 && !Manager.GetGameStateB("Stage3End"))
          {
            Manager.AddEvent(Game.GameTime + 1.5f, Message_07_Clear);
            Manager.AddEvent(Game.GameTime + 2f, Rebel_Forward);
            Manager.AddEvent(Game.GameTime + 10f, Scene_Stage04_Spawn);
            Manager.SetGameStateB("Stage3End", true);
          }
        }
        else if (StageNumber == 4)
        {
          Manager.UpdateActorLists(m_CriticalGroundObjects);
          enemystrength = MainEnemyFaction.WingCount;
          enemystrength += MainEnemyFaction.ShipCount;
          enemystrength += m_CriticalGroundObjects.Count;

          if (enemystrength == 0 && !Manager.GetGameStateB("Stage4End"))
          {
            Manager.AddEvent(Game.GameTime + 1.5f, Message_09_Clear);
            Manager.AddEvent(Game.GameTime + 2f, Rebel_Forward);
            Manager.AddEvent(Game.GameTime + 10f, Scene_Stage05_Spawn);
            Manager.SetGameStateB("Stage4End", true);
          }
        }
        else if (StageNumber == 5)
        {
          ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
          if (player != null)
          {
            player.SetArmor(DamageType.ALL, 0.75f);

            /*
            if (Player.Actor != null && !Manager.GetGameStateB("Stage5StartRun"))
            { 
             ActionManager.ForceClearQueue(Player.Actor);
              if (Player.Actor.GetPosition().x > 250)
                ActionManager.QueueFirst(Player.Actor, new Move(new TV_3DVECTOR(0, 1500, 200), 1000, can_interrupt: false));
              else
                ActionManager.QueueNext(Player.Actor, new Move(new TV_3DVECTOR(Manager.MaxBounds.x - 500, -220, 0), 1000, can_interrupt: false));
            }
            else if (Player.Actor != null && Manager.GetGameStateB("Stage5StartRun") && (Player.Actor.CurrentAction is AttackActor || Player.Actor.CurrentAction is Wait))
            {
              ActionManager.ForceClearQueue(Player.Actor);
              ActionManager.QueueFirst(Player.Actor, new Move(new TV_3DVECTOR(Player.Actor.GetPosition().x + 2000, Player.Actor.GetPosition().y, Player.Actor.GetPosition().z), 1000, can_interrupt: false));
            }
            */

            if (player.GetGlobalPosition().x > Manager.MaxBounds.x - 500
             && player.GetGlobalPosition().y < -180
             && player.GetGlobalPosition().z < 120
             && player.GetGlobalPosition().z > -120
             && !Stage5StartRun)
            {
              Manager.AddEvent(Game.GameTime + 0.1f, Scene_Stage05b_Spawn);
              Stage5StartRun = true;
              Screen2D.OverrideTargetingRadar = true;
            }
            else if (Stage5StartRun)
            {
              if (last_target_distX < player.GetGlobalPosition().x)
              {
                PlayerInfo.Score.AddDirect((player.GetGlobalPosition().x - last_target_distX) * 10);
                last_target_distX = player.GetGlobalPosition().x;
              }
              if (last_sound_distX < player.GetGlobalPosition().x && !Manager.IsCutsceneMode)
              {
                SoundManager.SetSound(SoundGlobals.Button3);
                last_sound_distX = player.GetGlobalPosition().x + 250;
              }
              Screen2D.TargetingRadar_text = string.Format("{0:00000000}", (target_distX - player.GetGlobalPosition().x) * 30);
              Scene_Stage05b_ContinuouslySpawnRoute();

              if (player.GetGlobalPosition().x > vader_distX 
                && !player.IsDyingOrDead
                && !Stage5End)
              {
                Manager.AddEvent(Game.GameTime + 0.1f, Scene_Stage06_Vader);
                Stage5End = true;
              }
            }
          }
        }
        else if (StageNumber == 6)
        {
          ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
          if (player != null)
          {
            if (!Stage6VaderEnd)
            {
              if (Screen2D.ShowRadar)
                player.SetArmor(DamageType.ALL, 2.5f);
              else
                player.SetArmor(DamageType.ALL, 1);
            }

            if (player != null)
            {
              if (last_target_distX < player.GetGlobalPosition().x)
              {
                PlayerInfo.Score.AddDirect((player.GetGlobalPosition().x - last_target_distX) * 10);
                last_target_distX = player.GetGlobalPosition().x;
              }

              if (last_sound_distX < player.GetGlobalPosition().x && !Manager.IsCutsceneMode)
              {
                SoundManager.SetSound(SoundGlobals.Button3);
                last_sound_distX = player.GetGlobalPosition().x + 250;
              }

              Screen2D.TargetingRadar_text = ((target_distX - player.GetGlobalPosition().x) * 30).ToString("00000000");
              Scene_Stage05b_ContinuouslySpawnRoute();

              if (player.GetGlobalPosition().x > vaderend_distX 
                && !player.IsDyingOrDead
                && !Stage6VaderEnd)
              {
                Manager.AddEvent(Game.GameTime + 0.1f, Scene_Stage06_VaderEnd);
                Stage6VaderEnd = true;
                Rebel_RemoveTorps();
              }

              ActorInfo vader = Engine.ActorFactory.Get(m_VaderID);
              ActorInfo vaderE1 = Engine.ActorFactory.Get(m_VaderEscort1ID);
              ActorInfo vaderE2 = Engine.ActorFactory.Get(m_VaderEscort2ID);

              if (Stage6VaderAttacking)
              {
                if (vader != null)
                vader.Position = new TV_3DVECTOR(player.GetGlobalPosition().x - 1700, -200, 0);

                if (vaderE1 != null)
                  vaderE1.Position = new TV_3DVECTOR(player.GetGlobalPosition().x - 1700, -220, 75);

                if (vaderE2 != null)
                  vaderE2.Position = new TV_3DVECTOR(player.GetGlobalPosition().x - 1700, -220, -75);
              }
            }
          }
        }
      }
      
      if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.ShipCount < 5)
      {
        Manager.AddEvent(0, Empire_StarDestroyer_Spawn, m_pendingSDspawnlist[0]);
        m_pendingSDspawnlist.RemoveAt(0);
      }

      if (!Stage5StartRun)
      {
        if (Manager.Scenario.TimeSinceLostWing < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
        {
          Manager.Line1Text = "WINGS: {0}".F(MainAllyFaction.WingCount);
        }
        else
        {
          Manager.Line1Text = "";
        }
      }
      else
      {
        Manager.Line1Text = Screen2D.TargetingRadar_text;
      }

      Manager.Line2Text = "TIME: {0:00}:{1:00}".F((int)(expiretime - Game.GameTime) / 60, (int)(expiretime - Game.GameTime) % 60);
      if ((int)(expiretime - Game.GameTime) / 60 < 4)
      {
        Manager.Line2Color = new TV_COLOR(1, 0.3f, 0.3f, 1).GetIntColor();
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AYavin != null && m_AYavin.Active)
      {
        if (StageNumber < 2)
        {
          float x_yv = (PlayerCameraInfo.Position.y > 0) ? (PlayerCameraInfo.Position.y / 6f) - 18000.0f : (PlayerCameraInfo.Position.y / 2.5f) - 18000.0f;
          float y_yv = PlayerCameraInfo.Position.x / 1.2f;
          float z_yv = PlayerCameraInfo.Position.z / 1.2f;
          m_AYavin.Position = new TV_3DVECTOR(x_yv, y_yv, z_yv);
        }
        else
        {
          float x_yv = PlayerCameraInfo.Position.x / 1.2f;
          float y_yv = 20000.0f;
          float z_yv = PlayerCameraInfo.Position.z / 1.2f;
          m_AYavin.Position = new TV_3DVECTOR(x_yv, y_yv, z_yv);
        }
      }
      if (m_AYavin4 != null && m_AYavin4.Active)
      {
        float x_y4 = PlayerCameraInfo.Position.x / 10f;
        float y_y4 = PlayerCameraInfo.Position.y / 2f;
        float z_y4 = (PlayerCameraInfo.Position.z > 0) ? PlayerCameraInfo.Position.z / 1.5f + 30000f : PlayerCameraInfo.Position.z / 100f + 30000f;
        m_AYavin4.Position = new TV_3DVECTOR(x_y4, y_y4, z_y4);
      }
      if (m_ADS != null && m_ADS.Active)
      {
        float x_ds = PlayerCameraInfo.Position.x / 5f;
        float y_ds = (PlayerCameraInfo.Position.y / 1.5f) + 3200.0f;
        float z_ds = (PlayerCameraInfo.Position.z > 0) ? PlayerCameraInfo.Position.z / 1.5f - 30000f : PlayerCameraInfo.Position.z / 100f - 30000f;
        m_ADS.Position = new TV_3DVECTOR(x_ds, y_ds, z_ds);
      }
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn()
    {
      ActorInfo ainfo;

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(150, 100, Manager.MaxBounds.z - 1000));

      // Player X-Wing
      ainfo = new ActorSpawnInfo
      {
        Type = PlayerInfo.ActorType,
        Name = "(Player)",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = MainAllyFaction,
        Position = new TV_3DVECTOR(0, 0, Manager.MaxBounds.z - 150),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock()
                                  , new Move(new TV_3DVECTOR(Engine.Random.Next(-5, 5), Engine.Random.Next(-5, 5), Manager.MaxBounds.z - 150 - 4500)
                                                    , PlayerInfo.ActorType.MoveLimitData.MaxSpeed)},
        Registries = null
      }.Spawn(this);

      PlayerCameraInfo.Look.SetTarget_LookAtActor(ainfo.ID);
      PlayerInfo.TempActorID = ainfo.ID;

      // X-Wings x21, Y-Wing x8
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 29; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(-2800, -80), Engine.Random.Next(-200, 200), Manager.MaxBounds.z + Engine.Random.Next(-2200, -150)));
        else
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(80, 2800), Engine.Random.Next(-200, 200), Manager.MaxBounds.z + Engine.Random.Next(-2200, -150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        if (i < 21)
        {
          ainfo = new ActorSpawnInfo
          {
            Type = ActorTypeFactory.Get("XWING"),
            Name = names[i],
    
            SidebarName = names[i],
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = v,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock()
                                       , new Move(new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), v.z - 4500)
                                       , ActorTypeFactory.Get("XWING").MoveLimitData.MaxSpeed)},
            Registries = null
          }.Spawn(this);

          ainfo.SetArmor(DamageType.ALL, 0.75f);
        }
        else
        {
          ainfo = new ActorSpawnInfo
          {
            Type = ActorTypeFactory.Get("YWING"),
            Name = names[i],
    
            SidebarName = names[i],
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = v,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock()
                                       , new Move(new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), v.z - 4500)
                                       , ActorTypeFactory.Get("YWING").MoveLimitData.MaxSpeed)},
            Registries = null
          }.Spawn(this);

          ainfo.SetArmor(DamageType.ALL, 0.6f);
        }
      }
    }

    public void Rebel_RemoveTorps()
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (actor.TypeInfo is YWingATI)
          {
            foreach (WeaponInfo w in actor.WeaponDefinitions.Weapons)
            {
              if (w.Type == WeaponType.TORPEDO)
              {
                w.Ammo = 1;
                w.MaxAmmo = 1;
              }
            }
          }
          else
          {
            foreach (WeaponInfo w in actor.WeaponDefinitions.Weapons)
            {
              if (w.Type == WeaponType.TORPEDO || w.Type == WeaponType.ION)
              {
                w.Ammo = 0;
                w.MaxAmmo = 0;
              }
            }
          }
        }
      }

      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        if (Stage6VaderEnd)
        {
          player.MoveData.MinSpeed = 400;
          player.MoveData.MaxSpeed = 400;

          player.SetArmor(DamageType.ALL, 0.5f);

          player.WeaponDefinitions.Reset();

          UnfixedWeaponData prew = new UnfixedWeaponData();
          prew.InsertLoadout(Engine.WeaponLoadoutFactory, "X_WG_LASR");
          prew.InsertLoadout(Engine.WeaponLoadoutFactory, "X_WG_TORP");
          player.WeaponDefinitions.Load(Engine.WeaponFactory, prew);
        }
        else if (Stage5StartRun)
        {
          player.MoveData.MinSpeed = 400;
          player.MoveData.MaxSpeed = 400;
          //m_Player.DamageModifier = 0.5f;
        }
        else if (StageNumber > 1)
        {
          player.MoveData.MinSpeed = player.MoveData.MaxSpeed * 0.75f;
        }
      }
      PlayerInfo.ResetPrimaryWeapon();
      PlayerInfo.ResetSecondaryWeapon();
    }

    public void Rebel_YWingsAttackScan()
    {
      if (MainEnemyFaction.ShipCount > 0)
      {
        foreach (int actorID in MainAllyFaction.GetWings())
        {
          ActorInfo actor = Engine.ActorFactory.Get(actorID);
          if (actor != null)
          {
            if (actor.TypeInfo is YWingATI || actor.TypeInfo is BWingATI)
            {
              int rsID = MainEnemyFaction.GetShip(Engine.Random.Next(0, MainEnemyFaction.ShipCount));
              ActorInfo rs = Engine.ActorFactory.Get(actorID);
              {
                foreach (ActorInfo c in rs.Children)
                {
                  if (c.TypeInfo.AIData.TargetType.Has(TargetType.SHIELDGENERATOR))
                    if (Engine.Random.NextDouble() > 0.4f)
                      rsID = c.ID;
                }
              }

              actor.ClearQueue();
              actor.QueueLast(AttackActor.GetOrCreate(rsID, -1, -1, false));
            }
          }
        }
      }

      Manager.AddEvent(Game.GameTime + 5f, Rebel_YWingsAttackScan);
    }

    public void Rebel_MakePlayer()
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;
      if (PlayerInfo.Actor == null || PlayerInfo.Actor.Disposed)
      {
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;

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
              Manager.AddEvent(Game.GameTime + 0.1f, Scene_Stage06_Vader);
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
            Type = PlayerInfo.ActorType,
            Name = "(Player)",
    
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = pos,
            Rotation = rot,
            Actions = null,
            Registries = null
          }.Spawn(this);

          ainfo.SetPlayer();
        }
      }
      m_PlayerID = PlayerInfo.ActorID;
      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_RemoveTorps);
    }

    public void Rebel_GiveControl()
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.UnlockOne();
          actor.SetState_Normal();
          actor.MoveData.Speed = actor.MoveData.MaxSpeed;
        }
      }
      PlayerInfo.IsMovementControlsEnabled = true;
      Manager.SetGameStateB("in_battle", true);
    }

    public void Rebel_Forward()
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.ForceClearQueue();
          actor.QueueNext(new Rotate(actor.GetGlobalPosition() + new TV_3DVECTOR(0, 0, -20000), actor.MoveData.MaxSpeed));
          actor.QueueNext(new Lock());
        }
      }
    }

    public void Rebel_Reposition()
    {
      int sw = -1;
      float x = 0;
      float y = 0;
      float z = Manager.MaxBounds.z - 150 ;
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.ForceClearQueue();
          if (actor.Name == "(Player)")
          {
            actor.Position = new TV_3DVECTOR(0, 100, Manager.MaxBounds.z - 150);
            actor.Rotation = new TV_3DVECTOR(0, 180, 0);
            actor.MoveData.ResetTurn();
            actor.MoveData.Speed = actor.MoveData.MaxSpeed;
            actor.QueueNext(new Wait(5));
          }
          else
          {
            x = Engine.Random.Next(80, 1800) * sw;
            y = Engine.Random.Next(-40, 100);
            z = Manager.MaxBounds.z + Engine.Random.Next(-1800, -150);
            sw = -sw;
            actor.Position = new TV_3DVECTOR(x, y, z);
            actor.Rotation = new TV_3DVECTOR(0, 180, 0);
            actor.MoveData.ResetTurn();
            actor.MoveData.Speed = actor.MoveData.MaxSpeed;
            actor.QueueNext(new Wait(5));
          }
        }
      }
      Rebel_RemoveTorps();
    }



    #endregion


    #region Empire spawns

    public void Empire_StarDestroyer_Spawn(ShipSpawnEventArg s)
    {
      ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);
    }
    
    public void Empire_FirstWave()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("IMPL")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 90
                                                              , true
                                                              , 0
                                                              , new string[] { "CriticalEnemies" }
                                                              );

      switch (Difficulty.ToLower())
      {
        case "mental":

          sspawn.CarrierSpawns = 9;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-5000, -150, 6000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 100, -4000), new TV_3DVECTOR(3000, 150, 5500)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "hard":
          sspawn.CarrierSpawns = 6;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-6000, -150, 7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 100, -4000), new TV_3DVECTOR(4000, 150, 5000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          break;
        case "normal":
          sspawn.CarrierSpawns = 9;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-2500, -150, 7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));
          break;
        case "easy":
        default:
          sspawn.CarrierSpawns = 6;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -150, -2000), new TV_3DVECTOR(-2500, -150, 7000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        )); break;
      }
    }

    public void Empire_SecondWave()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("IMPL")
                                                        , MainEnemyFaction
                                                        , true
                                                        , new TV_3DVECTOR()
                                                        , 90
                                                        , true
                                                        , 0
                                                        , new string[] { "CriticalEnemies" }
                                                        );

      switch (Difficulty.ToLower())
      {
        case "mental":
        case "hard":
          sspawn.CarrierSpawns = 12;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        )); 

          Manager.AddEvent(0, Empire_TIEWave, 4);
          break;
        case "easy":
          sspawn.CarrierSpawns = 8;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave, 2);
          break;
        case "normal":
        default:
          sspawn.CarrierSpawns = 10;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave, 3);
          break;
      }
      Manager.SetGameStateB("Stage1BBegin", true);
    }

    public void Empire_FirstTIEWave()
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
        float fx = Engine.Random.Next(-500, 500);
        float fy = Engine.Random.Next(-500, 0);
        float fz = Engine.Random.Next(-2500, 2500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = null;
            switch (Difficulty.ToLower())
            {
              case "mental":
              case "hard":
                actions = new ActionInfo[] { Hunt.GetOrCreate(TargetType.FIGHTER) };
                break;
            }

            new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
      
              SidebarName = "",
              SpawnTime = Game.GameTime,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, fz - 4000 - k * 100),
              Rotation = new TV_3DVECTOR(),
              Actions = actions
            }.Spawn(this);
          }
        }
      }
    }

    public void Empire_TIEWave(int sets)
    {
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -200, Manager.MinBounds.z - 5000), new TV_3DVECTOR(2500, 800, Manager.MinBounds.z - 5000));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 15
                                                                          , TargetType.FIGHTER
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
    }

    public void Empire_Towers01()
    {
      float dist = 2500;
      float height = Manager.MinBounds.y;

      for (int x = -2; x <= 2; x++)
        for (int z = -3; z <= -1; z++)
          Spawn_TowerRadarFormation(Engine, new TV_3DVECTOR(x * dist, 90 + height, z * dist));
    }

    public void Empire_Towers02()
    {
      float dist = 2500;
      float height = Manager.MinBounds.y;

      for (int x = -3; x <= 3; x += 2)
        for (int z = -2; z <= -1; z++)
          Spawn_TowerRadarFormation(Engine, new TV_3DVECTOR(x * dist / 2, 90 + height, z * dist));
    }

    private void Spawn_TowerRadarFormation(Engine Engine, TV_3DVECTOR position)
    {
      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("RDRT"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
        Position = position,
        Rotation = new TV_3DVECTOR()
      };

      ActorInfo a = asi.Spawn(this);
      m_CriticalGroundObjects.Add(a);

      asi.Type = ActorTypeFactory.Get("GUNT");
      asi.Position = position + new TV_3DVECTOR(300, 0, 0);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(-300, 0, 0);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(0, 0, 300);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(0, 0, -300);
      asi.Spawn(this);
    }

    private void Spawn_TowerDeflectorFormation(Engine Engine, TV_3DVECTOR position)
    {
      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("DEFT"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
        Position = position,
        Rotation = new TV_3DVECTOR()
      };

      ActorInfo a = asi.Spawn(this);
      m_CriticalGroundObjects.Add(a);

      asi.Type = ActorTypeFactory.Get("GUNT");
      asi.Position = position + new TV_3DVECTOR(500, 0, 0);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(-500, 0, 0);
      asi.Spawn(this);
    }


    public void Empire_Towers03()
    {
      float dist = 2500;
      float height = Manager.MinBounds.y;

      for (int z = -3; z <= 0; z++)
      {
        int z0 = (z > 0) ? z : -z;
        for (int x = -z0; x <= z0; x++)
        {
          if (z == -2)
            Spawn_TowerDeflectorFormation(Engine, new TV_3DVECTOR(x * dist, 90 + height, z * dist));
          else
            Spawn_TowerRadarFormation(Engine, new TV_3DVECTOR(x * dist, 90 + height, z * dist));
        }
      }
    }

    private void Spawn_TrenchFormation(ActorTypeInfo type, TV_3DVECTOR position, int distance, int trench_slot = -1)
    {
      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = type,
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
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

    public void Empire_Towers04()
    {
      float dist = 1000;
      float height = -175;

      for (int x = -6; x <= 6; x++)
        Spawn_TrenchFormation(ActorTypeFactory.Get("GUNT"), new TV_3DVECTOR(x * dist, 30 + height, 0), 150);
    }

    #endregion

    #region Scene

    public void Scene_EnterCutscene()
    {
      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_PrimaryWeapon = PlayerInfo.PrimaryWeaponN;
        m_Player_SecondaryWeapon = PlayerInfo.SecondaryWeaponN;
        m_Player_DamageModifier = player.GetArmor(DamageType.NORMAL);
        player.SetArmor(DamageType.ALL, 0);
        player.ForceClearQueue();
        player.QueueNext(new Lock());
      }
      PlayerCameraInfo.Look.ResetPosition();
      //PlayerInfo.ActorID = Manager.SceneCameraID;
      
      Manager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene()
    {
      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        PlayerInfo.ActorID = m_PlayerID;
        PlayerInfo.PrimaryWeaponN = m_Player_PrimaryWeapon;
        PlayerInfo.SecondaryWeaponN = m_Player_SecondaryWeapon;
        player.SetArmor(DamageType.ALL, m_Player_DamageModifier);
        player.ForceClearQueue();
      }
      Manager.IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam()
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(1000, 300, -15000));
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_ADS.ID);

      //cam.MoveData.MaxSpeed = 600;
      //cam.MoveData.Speed = 600;
    }


    public void Scene_Stage02_ApproachDeathStar()
    {
      Scene_DeathStarCam();
      SoundManager.SetMusic("battle_1_1", false, 71250);

      Manager.AddEvent(Game.GameTime + 4.8f, Rebel_Reposition);
      Manager.AddEvent(Game.GameTime + 5f, Scene_Stage02_Spawn);
      Manager.AddEvent(Game.GameTime + 9f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 11f, Message_04_Target);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
          actor.InflictDamage(-actor.MaxHP, DamageType.ALWAYS_100PERCENT);
      }

      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
          actor.Delete();
      }

      Manager.MaxBounds = new TV_3DVECTOR(10000, 400, 8000);
      Manager.MinBounds = new TV_3DVECTOR(-10000, -175, -12000);
      Manager.MaxAIBounds = new TV_3DVECTOR(8000, 400, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-8000, -160, -10000);
    }

    public void Scene_ClearGroundObjects()
    {
      foreach (int actorID in MainEnemyFaction.GetStructures())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
          actor.Delete();
      }
    }

    public void Scene_Stage02_Spawn()
    {
      StageNumber = 2;
      Rebel_RemoveTorps();

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(-550, -130, Manager.MaxBounds.z - 1500));

      m_ADS_Surface = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("SURF00300"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      }.Spawn(this);

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("SURF00300"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      };

      for (int x = -5 ; x <= 5; x++ )
        for (int z = -5; z <= 5; z++)
        {
          asi.Type = ((x + z) % 2 == 1) ? (ActorTypeInfo)ActorTypeFactory.Get("SURF00100") : ActorTypeFactory.Get("SURF00101");
          asi.Position = new TV_3DVECTOR(x * 4000, -173, z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
        }

      m_ADS.Delete();
      m_AYavin.Rotation = new TV_3DVECTOR(0, 0, 180);
      m_AYavin4.Delete();

      //cam.MoveData.MaxSpeed = 450;
      //cam.MoveData.Speed = 450;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);

      //Empire_TIEWave();
      Empire_Towers01();
    }

    public void Scene_Stage03_Spawn()
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_Reposition);
      Manager.AddEvent(Game.GameTime + 5f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 6.5f, Message_06_TIE);


      StageNumber = 3;

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(600, 130, Manager.MaxBounds.z - 1000));
      //cam.MoveData.MaxSpeed = 450;
      //cam.MoveData.Speed = 450;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
          actor.InflictDamage(0.35f * -actor.MaxHP, DamageType.ALWAYS_100PERCENT);
      }

      Scene_ClearGroundObjects();
      switch (Difficulty.ToLower())
      {
        case "easy":
          Manager.AddEvent(0, Empire_TIEWave, 5);
          break;
        case "mental":
          Manager.AddEvent(0, Empire_TIEWave, 8);
          Manager.AddEvent(Game.GameTime + 43f, Empire_TIEWave, 10);
          Manager.AddEvent(Game.GameTime + 45f, Message_06_TIE);
          break;
        case "hard":
          Manager.AddEvent(0, Empire_TIEWave, 7);
          Manager.AddEvent(Game.GameTime + 43f, Empire_TIEWave, 10);
          Manager.AddEvent(Game.GameTime + 45f, Message_06_TIE);
          break;
        case "normal":
        default:
          Manager.AddEvent(0, Empire_TIEWave, 6);
          break;
      }
      Empire_Towers02();
    }

    public void Scene_Stage04_Spawn()
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_Reposition);
      Manager.AddEvent(Game.GameTime + 5f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 6.5f, Message_08_Target);

      ActorTypeInfo type = ActorTypeFactory.Get("IMPL");
      TV_3DVECTOR position = new TV_3DVECTOR(2000, 750, -8000);
      TV_3DVECTOR targetposition = new TV_3DVECTOR(-4000, 1050, 1000);
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -25000);

      ActorInfo ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = MainEnemyFaction,
        Position = position + hyperspaceInOffset,
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[]
                     {
                                   new HyperspaceIn(position)
                                   , new Move(targetposition, type.MoveLimitData.MaxSpeed)
                                   , new HyperspaceOut()
                                   , new Delete()
                     }
      }.Spawn(this);

      ainfo.SetSpawnerEnable(true);
      ainfo.SpawnerInfo.SpawnMoveTime = Game.GameTime + 3f;

      StageNumber = 4;
      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(1000, 30, -2000));
      //cam.MoveData.MaxSpeed = 750;
      //cam.MoveData.Speed = 750;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
          actor.InflictDamage(0.35f * -actor.MaxHP, DamageType.ALWAYS_100PERCENT);
      }

      Scene_ClearGroundObjects();
      Empire_Towers03();
    }

    public void Scene_Stage05_Spawn()
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_Reposition);
      Manager.AddEvent(Game.GameTime + 5f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 6.5f, Message_10_BeginRun);

      StageNumber = 5;

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(550, -130, -1500));
      SoundManager.SetMusic("battle_1_2", true);
      Manager.MaxAIBounds = Manager.MaxAIBounds + new TV_3DVECTOR(2500, 0, 0);

      foreach (ActorInfo a in m_ADS_SurfaceParts)
      {
        a.Delete();
      }
      m_ADS_SurfaceParts.Clear();

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("SURF00300"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      };

      for (int x = -5; x <= 5; x++)
        for (int z = 0; z <= 3; z++)
        {
          asi.Type = ((x + z) % 2 == 1) ? (ActorTypeInfo)ActorTypeFactory.Get("SURF00100") : ActorTypeFactory.Get("SURF00101");
          asi.Position = new TV_3DVECTOR(x * 4000, -173, 2250 + z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
          asi.Position = new TV_3DVECTOR(x * 4000, -173, -2250 + -z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
        }

      for (int x = -20; x <= 20; x++)
      {
        asi.Type = ActorTypeFactory.Get("SURF00299");
        asi.Position = new TV_3DVECTOR(x * 1000, -175, 0);
        m_ADS_SurfaceParts.Add(asi.Spawn(this));
      }

      Manager.MaxBounds = new TV_3DVECTOR(7500, 300, 8000);
      Manager.MinBounds = new TV_3DVECTOR(-7500, -400, -12000);
      Manager.MaxAIBounds = new TV_3DVECTOR(7000, 300, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-7000, -160, -10000);

      m_ADS_Surface.Delete();
      Scene_ClearGroundObjects();

      //cam.MoveData.MaxSpeed = 450;
      //cam.MoveData.Speed = 450;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);

      Empire_Towers04();
    }

    public void Scene_Stage05b_Spawn()
    {
      //DeathCamMode = DeathCamMode.FOLLOW;

      Manager.MaxBounds = new TV_3DVECTOR(5000000, -185, 1000);
      //Manager.MinBounds = new TV_3DVECTOR(vader_distX - 1000, -400, -1000);
      Manager.MinBounds = new TV_3DVECTOR(7000, -400, -1000);
      Manager.MaxAIBounds = new TV_3DVECTOR(5000000, 300, 2000);
      Manager.MinAIBounds = new TV_3DVECTOR(0, -400, -2000);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (!actor.IsPlayer)
          {
            actor.Faction = FactionInfo.Neutral;
            actor.Delete();
          }
        }
      }
      //m_Player.Position = new TV_3DVECTOR(7050, m_Player.Position)

      foreach (ActorInfo a in m_ADS_SurfaceParts)
      {
        a.Delete();
      }
      m_ADS_SurfaceParts.Clear();
      Scene_ClearGroundObjects();
      Rebel_RemoveTorps();

      Scene_Stage05b_SpawnRoute();
    }

    public void Scene_Stage05b_SpawnRoute()
    {
      new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("VENT"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
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

    string[] TrenchTypes = new string[] { "SURF00200"
                                        , "SURF00201"
                                        , "SURF00202"
                                        , "SURF00203"
                                        , "SURF00204"
                                        , "SURF00205"
                                        , "SURF00206"
                                        , "SURF00207"
                                        , "SURF00208"
                                        , "SURF00209"
                                        , "SURF00210"
                                        , "SURF00211"
                                        , "SURF00212"
                                        , "SURF00299"
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


    public void Scene_Stage05b_ContinuouslySpawnRoute()
    {
      if (TrenchTurrets == null)
      {
        TrenchTurrets = new List<ActorInfo>[Trenches.Length];
        for (int i = 0; i < Trenches.Length; i++)
          TrenchTurrets[i] = new List<ActorInfo>();
      }

      int counter = (int)(PlayerCameraInfo.Position.x - 8000) / 1000 - 7;
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
            Type t = ActorTypeFactory.Get(TrenchTypes[Trenches[0]]).GetType();
            if (!(m_ADS_TrenchParts.Get(i).TypeInfo.GetType() == t) || i < counter)
            {
              m_ADS_TrenchParts.Get(i).Delete();
              foreach (ActorInfo turret in TrenchTurrets[i])
                turret.Delete();
              TrenchTurrets[i].Clear();

              ActorSpawnInfo asi = new ActorSpawnInfo
              {
                Type = ActorTypeFactory.Get(TrenchTypes[Trenches[0]]),
                Name = "",
        
                SidebarName = "",
                SpawnTime = Game.GameTime,
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
              trench = Engine.Random.Next(0, -trench + 1);

            ActorSpawnInfo asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get(TrenchTypes[trench]),
              Name = "",
      
              SidebarName = "",
              SpawnTime = Game.GameTime,
              Faction = FactionInfo.Neutral,
              Position = new TV_3DVECTOR(7000 + i * 1000, -173, 0),
              Rotation = new TV_3DVECTOR(0, 180, 0)
            };

            m_ADS_TrenchParts.Put(i, asi.Spawn(this));

            if (i < 100 && i > 0 && i % 35 == 0)
              Spawn_TrenchFormation(ActorTypeFactory.Get("DEFT"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 0), 175, i);
            else if (i > 10 && i < 96 && i % 10 == 0 || (i > 50 && i < 55))
              Spawn_TrenchFormation(ActorTypeFactory.Get("GUNT"), new TV_3DVECTOR(7000 + i * 1000, 30 - 175, 0), 150, i);
            else if (i == 100)
              Spawn_TrenchFormation(ActorTypeFactory.Get("RDRT"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 0), 170, i);


            asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("DEFT"),
              Name = "",
      
              SidebarName = "",
              SpawnTime = Game.GameTime,
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
                    asi.Type = ActorTypeFactory.Get("DEFT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("GUNT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 30 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("RDRT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 2 || trench == 3 || trench == 6)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("DEFT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("GUNT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 30 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("RDRT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 5)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("DEFT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("GUNT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 30 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("RDRT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 9)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("DEFT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("GUNT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 30 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("RDRT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 10)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("DEFT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 340, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("GUNT");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 340, 30 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("RDRT");
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

    public void Scene_Stage06_SetPlayer()
    {
      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.Position = new TV_3DVECTOR(player.GetGlobalPosition().x, -220, 0);
        player.Rotation = new TV_3DVECTOR(0, 90, 0);
        player.MoveData.ResetTurn();
      }
    }

    public void Scene_Stage06_Vader()
    {
      Scene_EnterCutscene();
      Manager.AddEvent(Game.GameTime + 7.9f, Scene_Stage06_SetPlayer);
      Manager.AddEvent(Game.GameTime + 8f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 8f, Scene_Stage06_VaderAttack);

      StageNumber = 6;
      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(vader_distX - 2750, -225, 0));
      SoundManager.SetMusic("battle_1_3", true);

      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.Position = new TV_3DVECTOR(vader_distX, -220, 0);
        player.Rotation = new TV_3DVECTOR(0, 90, 0);
        player.MoveData.ResetTurn();
        player.ForceClearQueue();
        player.QueueNext(new Lock());

        player.CanEvade = false;
        player.CanRetaliate = false;
      }
      Scene_ClearGroundObjects();

      ActorInfo m_Vader = Engine.ActorFactory.Get(m_VaderID);
      ActorInfo m_VaderEscort1 = Engine.ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo m_VaderEscort2 = Engine.ActorFactory.Get(m_VaderEscort2ID);
      ActorInfo m_Falcon = Engine.ActorFactory.Get(m_FalconID);

      m_Vader?.Delete();
      m_VaderEscort1?.Delete();
      m_VaderEscort2?.Delete();
      m_Falcon?.Delete();

      m_VaderID = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("TIEX"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire"),
        Position = new TV_3DVECTOR(vader_distX - 2000, 85, 0),
        Rotation = new TV_3DVECTOR(-10, 90, 0),
        Actions = new ActionInfo[] { new Move(new TV_3DVECTOR(vader_distX + 2000, -250, 0), 400)
                                             , new Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, 0), 400)
                                             , new Wait(3)
                                             , AttackActor.GetOrCreate(m_PlayerID, 1500, 1, false, 9999) },
        Registries = null
      }.Spawn(this).ID;

      m_VaderEscort1ID = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("TIE"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
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
        Type = ActorTypeFactory.Get("TIE"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire"),
        Position = new TV_3DVECTOR(vader_distX - 2100, 85, -75),
        Rotation = new TV_3DVECTOR(-10, 90, 0),
        Actions = new ActionInfo[] { new Move(new TV_3DVECTOR(vader_distX + 2000, -250, -75), 400)
                                       , new Rotate(new TV_3DVECTOR(vader_distX + 10000, -250, -75), 400)
                                       , new Lock() },
        Registries = null
      }.Spawn(this).ID;

      ActorInfo vader = Engine.ActorFactory.Get(m_VaderID);
      ActorInfo vaderE1 = Engine.ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo vaderE2 = Engine.ActorFactory.Get(m_VaderEscort2ID);

      UnfixedWeaponData prew = new UnfixedWeaponData();
      prew.InsertLoadout(Engine.WeaponLoadoutFactory, "TIED_LASR");
      prew.InsertLoadout(Engine.WeaponLoadoutFactory, "TIED_LASR");
      vader.WeaponDefinitions.Load(Engine.WeaponFactory, prew);

      vader.MoveData.MaxSpeed = 400;
      vader.MoveData.MinSpeed = 400;
      vader.CanEvade = false;
      vader.CanRetaliate = false;

      vaderE1.MoveData.MaxSpeed = 400;
      vaderE1.MoveData.MinSpeed = 400;
      vaderE1.CanEvade = false;
      vaderE1.CanRetaliate = false;

      vaderE2.MoveData.MaxSpeed = 400;
      vaderE2.MoveData.MinSpeed = 400;
      vaderE2.CanEvade = false;
      vaderE2.CanRetaliate = false;

      //cam.MoveData.MaxSpeed = 425;
      //cam.MoveData.Speed = 425;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(player.ID);
    }

    public void Scene_Stage06_VaderAttack()
    {
      Stage6VaderAttacking = true;

      ActorInfo m_Falcon = Engine.ActorFactory.Get(m_FalconID);
      m_Falcon.Delete();

      ActorInfo m_Vader = Engine.ActorFactory.Get(m_VaderID);
      m_Vader.ForceClearQueue();
      m_Vader.QueueNext(AttackActor.GetOrCreate(m_PlayerID, -1, -1, false, 9999));
      m_Vader.QueueNext(AttackActor.GetOrCreate(m_PlayerID, -1, -1, false, 9999));
      m_Vader.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      m_Vader.QueueNext(new Lock());

      ActorInfo m_VaderE1 = Engine.ActorFactory.Get(m_VaderEscort1ID);
      m_VaderE1.ForceClearQueue();
      m_VaderE1.QueueNext(AttackActor.GetOrCreate(m_PlayerID, -1, -1, false, 9999));
      m_VaderE1.QueueNext(AttackActor.GetOrCreate(m_PlayerID, -1, -1, false, 9999));
      m_VaderE1.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      m_VaderE1.QueueNext(new Lock());

      ActorInfo m_VaderE2 = Engine.ActorFactory.Get(m_VaderEscort2ID);
      m_VaderE2.ForceClearQueue();
      m_VaderE2.QueueNext(AttackActor.GetOrCreate(m_PlayerID, -1, -1, false, 9999));
      m_VaderE2.QueueNext(AttackActor.GetOrCreate(m_PlayerID, -1, -1, false, 9999));
      m_VaderE2.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      m_VaderE2.QueueNext(new Lock());
    }

    public void Scene_Stage06_VaderEnd()
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Game.GameTime + 8f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 7.9f, Scene_Stage06_SetPlayer);
      Engine.AtmosphereInfo.SetPos_Sun(new TV_3DVECTOR(1000, 100, 0));
      Engine.AtmosphereInfo.ShowSun = true;

      StageNumber = 6;

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(vaderend_distX + 900, -365, 0));
      SoundManager.SetMusic("ds_end_1_1", true);

      ActorInfo vader = Engine.ActorFactory.Get(m_VaderID);
      ActorInfo vaderE1 = Engine.ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo vaderE2 = Engine.ActorFactory.Get(m_VaderEscort2ID);
      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);

      player.Position = new TV_3DVECTOR(vaderend_distX, -220, 0);
      player.Rotation = new TV_3DVECTOR(0, 90, 0);
      player.MoveData.ResetTurn();
      player.ForceClearQueue();
      player.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      player.QueueNext(new Lock());

      vader.Rotation = new TV_3DVECTOR(0, 90, 0);

      ActorInfo falcon = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("FALC"),
        Name = "",

        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = new TV_3DVECTOR(vaderend_distX + 2500, 185, 0),
        Rotation = new TV_3DVECTOR(0, -90, 0),
        Actions = new ActionInfo[] { new Move(new TV_3DVECTOR(vaderend_distX + 1300, 5, 0), 500, -1, false)
                                       , AttackActor.GetOrCreate(m_VaderEscort1ID, -1, -1, false, 9999)
                                       , AttackActor.GetOrCreate(m_VaderEscort2ID, -1, -1, false, 9999)
                                       , new Move(new TV_3DVECTOR(vaderend_distX - 5300, 315, 0), 500, -1, false)
                                       , new Delete() }
      }.Spawn(this);

      falcon.CanEvade = false;
      falcon.CanRetaliate = false;
      m_FalconID = falcon.ID;

      vader.ForceClearQueue();
      vader.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      vader.QueueNext(new Lock());

      vaderE1.ForceClearQueue();
      vaderE1.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      vaderE1.QueueNext(new Lock());

      vaderE2.ForceClearQueue();
      vaderE2.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      vaderE2.QueueNext(new Lock());

      vader.HitEvents += Scene_Stage06_VaderFlee;
      vaderE1.HitEvents += Scene_Stage06_VaderFlee;
      vaderE2.HitEvents += Scene_Stage06_VaderFlee;

      //cam.MoveData.MaxSpeed = 25;
      //cam.MoveData.Speed = 25;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(falcon.ID);
    }

    public void Scene_Stage06_VaderFlee(ActorInfo aa, ActorInfo av)
    {
      if (Stage6VaderAttacking)
      {
        Stage6VaderAttacking = false;

        ActorInfo vader = Engine.ActorFactory.Get(m_VaderID);
        ActorInfo vaderE1 = Engine.ActorFactory.Get(m_VaderEscort1ID);
        ActorInfo vaderE2 = Engine.ActorFactory.Get(m_VaderEscort2ID);

        vader.ForceClearQueue();
        vader.MoveData.ApplyZBalance = false;
        vader.Rotation = new TV_3DVECTOR(-30, 85, 5);

        vader.DyingTimerSet(2000, true);

        vader.SetState_Dying();
        vaderE2.Rotation = new TV_3DVECTOR(-5, 93, 0);
        vaderE2.SetState_Dying();
        vaderE1.SetState_Dying();
      }
    }

    #endregion


    #region Text
    int color_outpost = new TV_COLOR(0.6f, 0.6f, 0.9f, 1).GetIntColor();

    public void Message_01_EnemyShipsInbound()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Enemy signals are approaching your position.", 5, color_outpost);
    }

    public void Message_02_MoreEnemyShipsInbound()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Another enemy signal are en route to your position.", 5, color_outpost);
    }

    public void Message_03_Clear()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Area cleared. Resume your attack on the Death Star.", 5, color_outpost);
    }

    public void Message_04_Target()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Eliminate all Radar Towers.", 5, color_outpost);
    }

    public void Message_05_Clear()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Area cleared. Proceed to the next sector.", 5, color_outpost);
    }

    public void Message_06_TIE()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Enemy fighters detected.", 5, color_outpost);
    }

    public void Message_07_Clear()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Deflector Towers detected en route to the trench.", 5, color_outpost);
    }

    public void Message_08_Target()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Destroy all Deflection Towers and Radar Towers.", 5, color_outpost);
    }

    public void Message_09_Clear()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: The path to the trench line is clear.", 5, color_outpost);
    }

    public void Message_10_BeginRun()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Luke, start your attack run.", 5, color_outpost);
    }

    public void Message_100_RebelBaseInRange()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is in range of the Death Star.", 5, color_outpost);
    }

    public void Message_101_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in ONE minute.", 5, color_outpost);
    }

    public void Message_102_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWO minutes.", 5, color_outpost);
    }

    public void Message_103_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THREE minutes.", 5, color_outpost);
    }

    public void Message_105_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIVE minutes.", 5, color_outpost);
    }

    public void Message_110_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TEN minutes.", 5, color_outpost);
    }

    public void Message_115_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIFTEEN minutes.", 5, color_outpost);
    }

    public void Message_120_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY minutes.", 5, color_outpost);
    }

    public void Message_125_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY-FIVE minutes.", 5, color_outpost);
    }

    public void Message_130_RebelBase()
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THIRTY minutes.", 5, color_outpost);
    }

    #endregion
  }
}
