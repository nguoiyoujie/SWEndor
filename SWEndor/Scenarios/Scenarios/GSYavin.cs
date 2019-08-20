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
using SWEndor.Actors.Data;
using SWEndor.Actors.Components;
using SWEndor.AI;

namespace SWEndor.Scenarios
{
  public class GSYavin : GameScenarioBase
  {
    public GSYavin(GameScenarioManager manager) : base(manager)
    {
      Name = "Battle of Yavin (WIP)";
      AllowedWings = new List<ActorTypeInfo> { ActorTypeFactory.Get("X-Wing"), ActorTypeFactory.Get("Millennium Falcon") };

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
    private Dictionary<string, int> m_CriticalGroundObjects;

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
      PlayerInfo.Name = "Red Five";

      m_ADS_SurfaceParts = new List<ActorInfo>();
      m_ADS_TrenchParts = new ThreadSafeDictionary<int, ActorInfo>();
      m_pendingSDspawnlist = new List<ShipSpawnEventArg>();
      m_CriticalGroundObjects = new Dictionary<string, int>();
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

      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Manager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      //Manager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      SoundManager.SetMusic("battle_1_1");
      SoundManager.SetMusicLoop("battle_1_4");

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
      ActorCreationInfo aci_Yavin = new ActorCreationInfo(ActorTypeFactory.Get("Yavin"))
      {
        Position = new TV_3DVECTOR(0, 0, 18000),
        Rotation = new TV_3DVECTOR(90, 90, 0),
        InitialScale = 4
      };
      m_AYavin = ActorFactory.Create(aci_Yavin);

      // Create Yavin 4
      ActorCreationInfo aci_Yavin4 = new ActorCreationInfo(ActorTypeFactory.Get("Yavin4"))
      {
        Position = new TV_3DVECTOR(0, 800, -18000),
        Rotation = new TV_3DVECTOR(0, 0, 0),
      };
      m_AYavin4 = ActorFactory.Create(aci_Yavin4);

      // Create DeathStar
      ActorCreationInfo aci_DS = new ActorCreationInfo(ActorTypeFactory.Get("DeathStar"))
      {
        Position = new TV_3DVECTOR(0, 800, 28000),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Faction = MainEnemyFaction
      };
      m_ADS = ActorFactory.Create(aci_DS);
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
            ActorInfo actor = ActorFactory.Get(actorID);
            if (actor != null)
              enemystrength += actor.HP_Frac * 100 + (actor.SpawnerInfo != null ? actor.SpawnerInfo.SpawnsRemaining : 0);
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
          enemystrength = MainEnemyFaction.GetWings().Count;
          enemystrength += MainEnemyFaction.GetShips().Count;
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
          enemystrength = MainEnemyFaction.GetWings().Count;
          enemystrength += MainEnemyFaction.GetShips().Count;
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
          enemystrength = MainEnemyFaction.GetWings().Count;
          enemystrength += MainEnemyFaction.GetShips().Count;
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
          ActorInfo player = ActorFactory.Get(m_PlayerID);
          if (player != null)
          {
            Engine.ActorDataSet.CombatData[player.dataID].DamageModifier = 0.75f;

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

            if (player.GetPosition().x > Manager.MaxBounds.x - 500
             && player.GetPosition().y < -180
             && player.GetPosition().z < 120
             && player.GetPosition().z > -120
             && !Stage5StartRun)
            {
              Manager.AddEvent(Game.GameTime + 0.1f, Scene_Stage05b_Spawn);
              Stage5StartRun = true;
              Screen2D.OverrideTargetingRadar = true;
            }
            else if (Stage5StartRun)
            {
              if (last_target_distX < player.GetPosition().x)
              {
                PlayerInfo.Score.AddDirect((player.GetPosition().x - last_target_distX) * 10);
                last_target_distX = player.GetPosition().x;
              }
              if (last_sound_distX < player.GetPosition().x && !Manager.IsCutsceneMode)
              {
                SoundManager.SetSound("button_3");
                last_sound_distX = player.GetPosition().x + 250;
              }
              Screen2D.TargetingRadar_text = string.Format("{0:00000000}", (target_distX - player.GetPosition().x) * 30);
              Scene_Stage05b_ContinuouslySpawnRoute(null);

              if (player.GetPosition().x > vader_distX 
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
          ActorInfo player = ActorFactory.Get(m_PlayerID);
          if (player != null)
          {
            if (!Stage6VaderEnd)
            {
              if (Screen2D.ShowRadar)
                Engine.ActorDataSet.CombatData[player.dataID].DamageModifier = 2.5f;
              else
                Engine.ActorDataSet.CombatData[player.dataID].DamageModifier = 1f;
            }

            if (player != null)
            {
              if (last_target_distX < player.GetPosition().x)
              {
                PlayerInfo.Score.AddDirect((player.GetPosition().x - last_target_distX) * 10);
                last_target_distX = player.GetPosition().x;
              }

              if (last_sound_distX < player.GetPosition().x && !Manager.IsCutsceneMode)
              {
                SoundManager.SetSound("button_3");
                last_sound_distX = player.GetPosition().x + 250;
              }

              Screen2D.TargetingRadar_text = string.Format("{0:00000000}", (target_distX - player.GetPosition().x) * 30);
              Scene_Stage05b_ContinuouslySpawnRoute(null);

              if (player.GetPosition().x > vaderend_distX 
                && !player.IsDyingOrDead
                && !Stage6VaderEnd)
              {
                Manager.AddEvent(Game.GameTime + 0.1f, Scene_Stage06_VaderEnd);
                Stage6VaderEnd = true;
                Rebel_RemoveTorps(null);
              }

              ActorInfo vader = ActorFactory.Get(m_VaderID);
              ActorInfo vaderEscort1 = ActorFactory.Get(m_VaderEscort1ID);
              ActorInfo vaderEscort2 = ActorFactory.Get(m_VaderEscort2ID);

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
        Manager.AddEvent(0, Empire_StarDestroyer_Spawn, m_pendingSDspawnlist[0]);
        m_pendingSDspawnlist.RemoveAt(0);
      }

      if (!Stage5StartRun)
      {
        if (Manager.Scenario.TimeSinceLostWing < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
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
        Manager.Line1Text = Screen2D.TargetingRadar_text;
      }

      Manager.Line2Text = string.Format("TIME: {0:00}:{1:00}", (int)(expiretime - Game.GameTime) / 60, (int)(expiretime - Game.GameTime) % 60);
      if ((int)(expiretime - Game.GameTime) / 60 < 4)
      {
        Manager.Line2Color = new TV_COLOR(1, 0.3f, 0.3f, 1);
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AYavin != null && m_AYavin.Active)
      {
        if (StageNumber < 2)
        {
          float x_yv = (PlayerInfo.Position.y > 0) ? (PlayerInfo.Position.y / 6f) - 18000.0f : (PlayerInfo.Position.y / 2.5f) - 18000.0f;
          float y_yv = PlayerInfo.Position.x / 1.2f;
          float z_yv = PlayerInfo.Position.z / 1.2f;
          m_AYavin.SetLocalPosition(x_yv, y_yv, z_yv);
        }
        else
        {
          float x_yv = PlayerInfo.Position.x / 1.2f;
          float y_yv = 20000.0f;
          float z_yv = PlayerInfo.Position.z / 1.2f;
          m_AYavin.SetLocalPosition(x_yv, y_yv, z_yv);
        }
      }
      if (m_AYavin4 != null && m_AYavin4.Active)
      {
        float x_y4 = PlayerInfo.Position.x / 10f;
        float y_y4 = PlayerInfo.Position.y / 2f;
        float z_y4 = (PlayerInfo.Position.z > 0) ? PlayerInfo.Position.z / 1.5f + 30000f : PlayerInfo.Position.z / 100f + 30000f;
        m_AYavin4.SetLocalPosition(x_y4, y_y4, z_y4);
      }
      if (m_ADS != null && m_ADS.Active)
      {
        float x_ds = PlayerInfo.Position.x / 5f;
        float y_ds = (PlayerInfo.Position.y / 1.5f) + 3200.0f;
        float z_ds = (PlayerInfo.Position.z > 0) ? PlayerInfo.Position.z / 1.5f - 30000f : PlayerInfo.Position.z / 100f - 30000f;
        m_ADS.SetLocalPosition(x_ds, y_ds, z_ds);
      }
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn(GameEventArg arg)
    {
      ActorInfo ainfo;

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(150, 100, Manager.MaxBounds.z - 1000));

      // Player X-Wing
      ainfo = new ActorSpawnInfo
      {
        Type = PlayerInfo.ActorType,
        Name = "(Player)",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = MainAllyFaction,
        Position = new TV_3DVECTOR(0, 0, Manager.MaxBounds.z - 150),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock()
                                  , new Move(new TV_3DVECTOR(Engine.Random.Next(-5, 5), Engine.Random.Next(-5, 5), Manager.MaxBounds.z - 150 - 4500)
                                                    , PlayerInfo.ActorType.MaxSpeed)},
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
            Type = ActorTypeFactory.Get("X-Wing"),
            Name = names[i],
            RegisterName = "",
            SidebarName = names[i],
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = v,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock()
                                       , new Move(new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), v.z - 4500)
                                       , ActorTypeFactory.Get("X-Wing").MaxSpeed)},
            Registries = null
          }.Spawn(this);

          Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.75f;
        }
        else
        {
          ainfo = new ActorSpawnInfo
          {
            Type = ActorTypeFactory.Get("Y-Wing"),
            Name = names[i],
            RegisterName = "",
            SidebarName = names[i],
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = v,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock()
                                       , new Move(new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), v.z - 4500)
                                       , ActorTypeFactory.Get("Y-Wing").MaxSpeed)},
            Registries = null
          }.Spawn(this);

          Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.6f;
        }
      }
    }

    public void Rebel_RemoveTorps(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
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

      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        if (Stage6VaderEnd)
        {
          player.MoveData.MinSpeed = 400;
          player.MoveData.MaxSpeed = 400;

          Engine.ActorDataSet.CombatData[player.dataID].DamageModifier = 0.5f;
          player.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"torp", WeaponFactory.Get("X_WG_TORP") }
                                                        , {"laser", WeaponFactory.Get("X_WG_LASR") }
                                                        };
          player.WeaponSystemInfo.PrimaryWeapons = new string[] { "1:laser", "2:laser", "4:laser" };
          player.WeaponSystemInfo.SecondaryWeapons = new string[] { "4:laser", "1:torp" };
          player.WeaponSystemInfo.AIWeapons = new string[] { "1:torp", "1:laser" };
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

    public void Rebel_YWingsAttackScan(GameEventArg arg)
    {
      if (MainEnemyFaction.GetShips().Count > 0)
      {
        foreach (int actorID in MainAllyFaction.GetWings())
        {
          ActorInfo actor = ActorFactory.Get(actorID);
          if (actor != null)
          {
            if (actor.TypeInfo is YWingATI || actor.TypeInfo is BWingATI)
            {
              int rsID = MainEnemyFaction.GetShips()[Engine.Random.Next(0, MainEnemyFaction.GetShips().Count)];
              ActorInfo rs = ActorFactory.Get(actorID);
              {
                foreach (ActorInfo a in rs.Relation.Children)
                {
                  if (Engine.ActorDataSet.RegenData[a.dataID].ParentRegenRate > 0)
                    if (Engine.Random.NextDouble() > 0.4f)
                      rsID = a.ID;
                }
              }

              actor.ClearQueue();
              actor.QueueLast(new AttackActor(rsID, -1, -1, false));
            }
          }
        }
      }

      Manager.AddEvent(Game.GameTime + 5f, Rebel_YWingsAttackScan);
    }

    public void Rebel_MakePlayer(GameEventArg arg)
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
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = pos,
            Rotation = rot,
            Actions = null,
            Registries = null
          }.Spawn(this);

          PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_PlayerID = PlayerInfo.ActorID;
      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_RemoveTorps);
    }

    public void Rebel_GiveControl(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.UnlockOne(actorID);
          actor.SetState_Normal();
          actor.MoveData.Speed = actor.MoveData.MaxSpeed;
        }
      }
      PlayerInfo.IsMovementControlsEnabled = true;
      Manager.SetGameStateB("in_battle", true);
    }

    public void Rebel_Forward(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueNext(actorID, new Rotate(actor.GetPosition() + new TV_3DVECTOR(0, 0, -20000), actor.MoveData.MaxSpeed));
          ActionManager.QueueNext(actorID, new Lock());
        }
      }
    }

    public void Rebel_Reposition(GameEventArg arg)
    {
      int sw = -1;
      float x = 0;
      float y = 0;
      float z = Manager.MaxBounds.z - 150 ;
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.ForceClearQueue(actorID);
          if (actor.Name == "(Player)")
          {
            actor.SetLocalPosition(0, 100, Manager.MaxBounds.z - 150);
            actor.SetLocalRotation(0, 180, 0);
            actor.MoveData.ResetTurn();
            actor.MoveData.Speed = actor.MoveData.MaxSpeed;
            ActionManager.QueueNext(actorID, new Wait(5));
          }
          else
          {
            x = Engine.Random.Next(80, 1800) * sw;
            y = Engine.Random.Next(-40, 100);
            z = Manager.MaxBounds.z + Engine.Random.Next(-1800, -150);
            sw = -sw;
            actor.SetLocalPosition(x, y, z);
            actor.SetLocalRotation(0, 180, 0);
            actor.MoveData.ResetTurn();
            actor.MoveData.Speed = actor.MoveData.MaxSpeed;
            ActionManager.QueueNext(actorID, new Wait(5));
          }
        }
      }
      Rebel_RemoveTorps(null);
    }



    #endregion


    #region Empire spawns

    public void Empire_StarDestroyer_Spawn(GameEventArg arg)
    {
      if (arg is ShipSpawnEventArg)
      {
        ShipSpawnEventArg s = (ShipSpawnEventArg)arg;
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);
      }
    }
    
    public void Empire_FirstWave(GameEventArg arg)
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 90
                                                              , true
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

    public void Empire_SecondWave(GameEventArg arg)
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                                        , MainEnemyFaction
                                                        , true
                                                        , new TV_3DVECTOR()
                                                        , 90
                                                        , true
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

          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N4);
          break;
        case "easy":
          sspawn.CarrierSpawns = 8;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N2);
          break;
        case "normal":
        default:
          sspawn.CarrierSpawns = 10;
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(1500, 0, -12000), new TV_3DVECTOR(1500, 0, 2000)
                                                        , new TV_3DVECTOR(0, 0, 99999)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N3);
          break;
      }
      Manager.SetGameStateB("Stage1BBegin", true);
    }

    public void Empire_FirstTIEWave(GameEventArg arg)
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
                actions = new ActionInfo[] { new Hunt(TargetType.FIGHTER) };
                break;
            }

            new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
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

    public void Empire_TIEWave(GameEventArg arg)
    {
      int sets = ((IntegerEventArg)arg).Num;
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

    public void Empire_Towers01(GameEventArg arg)
    {
      float dist = 2500;
      float height = Manager.MinBounds.y;

      for (int x = -2; x <= 2; x++)
        for (int z = -3; z <= -1; z++)
          Spawn_TowerRadarFormation(Engine, new TV_3DVECTOR(x * dist, 90 + height, z * dist));
    }

    public void Empire_Towers02(GameEventArg arg)
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
        Type = ActorTypeFactory.Get("Radar Tower"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
        Position = position,
        Rotation = new TV_3DVECTOR()
      };

      ActorInfo a = asi.Spawn(this);
      m_CriticalGroundObjects.Add(a.Key, a.ID);

      asi.Type = ActorTypeFactory.Get("Gun Tower");
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
        Type = ActorTypeFactory.Get("Deflector Tower"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_DeathStarDefenses"),
        Position = position,
        Rotation = new TV_3DVECTOR()
      };

      ActorInfo a = asi.Spawn(this);
      m_CriticalGroundObjects.Add(a.Key, a.ID);

      asi.Type = ActorTypeFactory.Get("Gun Tower");
      asi.Position = position + new TV_3DVECTOR(500, 0, 0);
      asi.Spawn(this);
      asi.Position = position + new TV_3DVECTOR(-500, 0, 0);
      asi.Spawn(this);
    }


    public void Empire_Towers03(GameEventArg arg)
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
        RegisterName = "",
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

    public void Empire_Towers04(GameEventArg arg)
    {
      float dist = 1000;
      float height = -175;

      for (int x = -6; x <= 6; x++)
        Spawn_TrenchFormation(ActorTypeFactory.Get("Gun Tower"), new TV_3DVECTOR(x * dist, 30 + height, 0), 150);
    }

    #endregion

    #region Scene

    public void Scene_EnterCutscene(GameEventArg arg)
    {
      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_PrimaryWeapon = PlayerInfo.PrimaryWeapon;
        m_Player_SecondaryWeapon = PlayerInfo.SecondaryWeapon;
        m_Player_DamageModifier = Engine.ActorDataSet.CombatData[player.dataID].DamageModifier;
        Engine.ActorDataSet.CombatData[player.dataID].DamageModifier = 0;
        ActionManager.ForceClearQueue(m_PlayerID);
        ActionManager.QueueNext(m_PlayerID, new Lock());
      }
      PlayerCameraInfo.Look.ResetPosition();
      //PlayerInfo.ActorID = Manager.SceneCameraID;
      
      Manager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(GameEventArg arg)
    {
      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        PlayerInfo.ActorID = m_PlayerID;
        PlayerInfo.PrimaryWeapon = m_Player_PrimaryWeapon;
        PlayerInfo.SecondaryWeapon = m_Player_SecondaryWeapon;
        Engine.ActorDataSet.CombatData[player.dataID].DamageModifier = m_Player_DamageModifier;
        ActionManager.ForceClearQueue(m_PlayerID);
      }
      Manager.IsCutsceneMode = false;
    }

    public void Scene_DeathStarCam(GameEventArg arg)
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(1000, 300, -15000));
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_ADS.ID);

      //cam.MoveData.MaxSpeed = 600;
      //cam.MoveData.Speed = 600;
    }


    public void Scene_Stage02_ApproachDeathStar(GameEventArg arg)
    {
      Scene_DeathStarCam(null);
      SoundManager.SetMusic("battle_1_1", false, 71250);

      Manager.AddEvent(Game.GameTime + 4.8f, Rebel_Reposition);
      Manager.AddEvent(Game.GameTime + 5f, Scene_Stage02_Spawn);
      Manager.AddEvent(Game.GameTime + 9f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 11f, Message_04_Target);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
          actor.InflictDamage(actor, -actor.MaxHP, DamageType.ALWAYS_100PERCENT);
      }

      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
          actor.Delete();
      }

      Manager.MaxBounds = new TV_3DVECTOR(10000, 400, 8000);
      Manager.MinBounds = new TV_3DVECTOR(-10000, -175, -12000);
      Manager.MaxAIBounds = new TV_3DVECTOR(8000, 400, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-8000, -160, -10000);
    }

    public void Scene_ClearGroundObjects(GameEventArg arg)
    {
      foreach (int actorID in MainEnemyFaction.GetStructures())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
          actor.Delete();
      }
    }

    public void Scene_Stage02_Spawn(GameEventArg arg)
    {
      StageNumber = 2;
      Rebel_RemoveTorps(null);

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(-550, -130, Manager.MaxBounds.z - 1500));

      m_ADS_Surface = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("Surface003_00ATI"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      }.Spawn(this);

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("Surface003_00ATI"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      };

      for (int x = -5 ; x <= 5; x++ )
        for (int z = -5; z <= 5; z++)
        {
          asi.Type = ((x + z) % 2 == 1) ? (ActorTypeInfo)ActorTypeFactory.Get("Surface001_00ATI") : ActorTypeFactory.Get("Surface001_01ATI");
          asi.Position = new TV_3DVECTOR(x * 4000, -173, z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
        }

      m_ADS.Delete();
      m_AYavin.SetLocalRotation(0, 0, 180);
      m_AYavin4.Delete();

      //cam.MoveData.MaxSpeed = 450;
      //cam.MoveData.Speed = 450;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);

      //Empire_TIEWave(null);
      Empire_Towers01(null);
    }

    public void Scene_Stage03_Spawn(GameEventArg arg)
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
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
          actor.InflictDamage(actor, 0.35f * -actor.MaxHP, DamageType.ALWAYS_100PERCENT);
      }

      Scene_ClearGroundObjects(null);
      switch (Difficulty.ToLower())
      {
        case "easy":
          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N5);
          break;
        case "mental":
          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N8);
          Manager.AddEvent(Game.GameTime + 43f, Empire_TIEWave, IntegerEventArg.N10);
          Manager.AddEvent(Game.GameTime + 45f, Message_06_TIE);
          break;
        case "hard":
          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N7);
          Manager.AddEvent(Game.GameTime + 43f, Empire_TIEWave, IntegerEventArg.N10);
          Manager.AddEvent(Game.GameTime + 45f, Message_06_TIE);
          break;
        case "normal":
        default:
          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N6);
          break;
      }
      Empire_Towers02(null);
    }

    public void Scene_Stage04_Spawn(GameEventArg arg)
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_Reposition);
      Manager.AddEvent(Game.GameTime + 5f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 6.5f, Message_08_Target);

      ActorTypeInfo type = ActorTypeFactory.Get("Imperial-I Star Destroyer");
      TV_3DVECTOR position = new TV_3DVECTOR(2000, 750, -8000);
      TV_3DVECTOR targetposition = new TV_3DVECTOR(-4000, 1050, 1000);
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, -25000);

      ActorInfo ainfo = new ActorSpawnInfo
      {
        Type = type,
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
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
          ainfo.SpawnerInfo.NextSpawnTime = Game.GameTime + 3f;

      StageNumber = 4;
      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(1000, 30, -2000));
      //cam.MoveData.MaxSpeed = 750;
      //cam.MoveData.Speed = 750;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
          actor.InflictDamage(actor, 0.35f * -actor.MaxHP, DamageType.ALWAYS_100PERCENT);
      }

      Scene_ClearGroundObjects(null);
      Empire_Towers03(null);
    }

    public void Scene_Stage05_Spawn(GameEventArg arg)
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
        Type = ActorTypeFactory.Get("Surface003_00ATI"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Neutral,
        Position = new TV_3DVECTOR(0, -175, 0),
        Rotation = new TV_3DVECTOR()
      };

      for (int x = -5; x <= 5; x++)
        for (int z = 0; z <= 3; z++)
        {
          asi.Type = ((x + z) % 2 == 1) ? (ActorTypeInfo)ActorTypeFactory.Get("Surface001_00ATI") : ActorTypeFactory.Get("Surface001_01ATI");
          asi.Position = new TV_3DVECTOR(x * 4000, -173, 2250 + z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
          asi.Position = new TV_3DVECTOR(x * 4000, -173, -2250 + -z * 4000);
          m_ADS_SurfaceParts.Add(asi.Spawn(this));
        }

      for (int x = -20; x <= 20; x++)
      {
        asi.Type = ActorTypeFactory.Get("Surface002_99ATI");
        asi.Position = new TV_3DVECTOR(x * 1000, -175, 0);
        m_ADS_SurfaceParts.Add(asi.Spawn(this));
      }

      Manager.MaxBounds = new TV_3DVECTOR(7500, 300, 8000);
      Manager.MinBounds = new TV_3DVECTOR(-7500, -400, -12000);
      Manager.MaxAIBounds = new TV_3DVECTOR(7000, 300, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-7000, -160, -10000);

      m_ADS_Surface.Delete();
      Scene_ClearGroundObjects(null);

      //cam.MoveData.MaxSpeed = 450;
      //cam.MoveData.Speed = 450;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);

      Empire_Towers04(null);
    }

    public void Scene_Stage05b_Spawn(GameEventArg arg)
    {
      //DeathCamMode = DeathCamMode.FOLLOW;

      Manager.MaxBounds = new TV_3DVECTOR(5000000, -185, 1000);
      //Manager.MinBounds = new TV_3DVECTOR(vader_distX - 1000, -400, -1000);
      Manager.MinBounds = new TV_3DVECTOR(7000, -400, -1000);
      Manager.MaxAIBounds = new TV_3DVECTOR(5000000, 300, 2000);
      Manager.MinAIBounds = new TV_3DVECTOR(0, -400, -2000);

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (!actor.IsPlayer)
          {
            actor.Faction = FactionInfo.Neutral;
            actor.Delete();
          }
        }
      }
      //m_Player.SetLocalPosition(7050, m_Player.GetLocalPosition())

      foreach (ActorInfo a in m_ADS_SurfaceParts)
      {
        a.Delete();
      }
      m_ADS_SurfaceParts.Clear();
      Scene_ClearGroundObjects(null);
      Rebel_RemoveTorps(null);

      Scene_Stage05b_SpawnRoute(null);
    }

    public void Scene_Stage05b_SpawnRoute(GameEventArg arg)
    {
      new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("Thermal Exhaust Port"),
        Name = "",
        RegisterName = "",
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


    public void Scene_Stage05b_ContinuouslySpawnRoute(GameEventArg arg)
    {
      if (TrenchTurrets == null)
      {
        TrenchTurrets = new List<ActorInfo>[Trenches.Length];
        for (int i = 0; i < Trenches.Length; i++)
          TrenchTurrets[i] = new List<ActorInfo>();
      }

      int counter = (int)(PlayerInfo.Position.x - 8000) / 1000 - 7;
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
                RegisterName = "",
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
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.GameTime,
              Faction = FactionInfo.Neutral,
              Position = new TV_3DVECTOR(7000 + i * 1000, -173, 0),
              Rotation = new TV_3DVECTOR(0, 180, 0)
            };

            m_ADS_TrenchParts.Put(i, asi.Spawn(this));

            if (i < 100 && i > 0 && i % 35 == 0)
              Spawn_TrenchFormation(ActorTypeFactory.Get("Deflector Tower"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 0), 175, i);
            else if (i > 10 && i < 96 && i % 10 == 0 || (i > 50 && i < 55))
              Spawn_TrenchFormation(ActorTypeFactory.Get("Gun Tower"), new TV_3DVECTOR(7000 + i * 1000, 30 - 175, 0), 150, i);
            else if (i == 100)
              Spawn_TrenchFormation(ActorTypeFactory.Get("Radar Tower"), new TV_3DVECTOR(7000 + i * 1000, 90 - 175, 0), 170, i);


            asi = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("Deflector Tower"),
              Name = "",
              RegisterName = "",
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
                    asi.Type = ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 30 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 140, 90 - 390, 40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 2 || trench == 3 || trench == 6)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 30 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 40, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 5)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 30 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 320, 90 - 390, -40);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 9)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 30 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("Radar Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 260, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                }
                else if (trench == 10)
                {
                  if (i > 50 && i % 5 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Deflector Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 340, 90 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 3 < 2)
                  {
                    asi.Type = ActorTypeFactory.Get("Gun Tower");
                    asi.Position = new TV_3DVECTOR(7000 + i * 1000 - 340, 30 - 175, 0);
                    TrenchTurrets[i].Add(asi.Spawn(this));
                  }
                  else if (i % 4 < 1)
                  {
                    asi.Type = ActorTypeFactory.Get("Radar Tower");
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

    public void Scene_Stage06_SetPlayer(GameEventArg arg)
    {
      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.SetLocalPosition(player.GetPosition().x, -220, 0);
        player.SetLocalRotation(0, 90, 0);
        player.MoveData.ResetTurn();
      }
    }

    public void Scene_Stage06_Vader(GameEventArg arg)
    {
      Scene_EnterCutscene(null);
      Manager.AddEvent(Game.GameTime + 7.9f, Scene_Stage06_SetPlayer);
      Manager.AddEvent(Game.GameTime + 8f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 8f, Scene_Stage06_VaderAttack);

      StageNumber = 6;
      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(vader_distX - 2750, -225, 0));
      SoundManager.SetMusic("battle_1_3", true);

      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        player.SetLocalPosition(vader_distX, -220, 0);
        player.SetLocalRotation(0, 90, 0);
        player.MoveData.ResetTurn();
        ActionManager.ForceClearQueue(m_PlayerID);
        ActionManager.QueueNext(m_PlayerID, new Lock());

        player.CanEvade = false;
        player.CanRetaliate = false;
      }
      Scene_ClearGroundObjects(null);

      ActorInfo m_Vader = ActorFactory.Get(m_VaderID);
      ActorInfo m_VaderEscort1 = ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo m_VaderEscort2 = ActorFactory.Get(m_VaderEscort2ID);
      ActorInfo m_Falcon = ActorFactory.Get(m_FalconID);

      m_Vader?.Delete();
      m_VaderEscort1?.Delete();
      m_VaderEscort2?.Delete();
      m_Falcon?.Delete();

      m_VaderID = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("TIE Advanced X1"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
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
        Type = ActorTypeFactory.Get("TIE"),
        Name = "",
        RegisterName = "",
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
        RegisterName = "",
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

      ActorInfo vader = ActorFactory.Get(m_VaderID);
      ActorInfo vaderE1 = ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo vaderE2 = ActorFactory.Get(m_VaderEscort2ID);

      vader.WeaponSystemInfo.Weapons = new Dictionary<string, WeaponInfo>{ {"lsrb", WeaponFactory.Get("TIED_LASR") }
                                                        , {"laser", WeaponFactory.Get("TIED_LASR") }
                                                        };
      vader.WeaponSystemInfo.AIWeapons = new string[] { "1:laser", "1:lsrb" };
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

    public void Scene_Stage06_VaderAttack(GameEventArg arg)
    {
      Stage6VaderAttacking = true;

      ActorInfo m_Falcon = ActorFactory.Get(m_FalconID);
      m_Falcon.Delete();

      ActorInfo m_Vader = ActorFactory.Get(m_VaderID);
      m_Vader.ForceClearQueue();
      m_Vader.QueueNext(new AttackActor(m_PlayerID, -1, -1, false, 9999));
      m_Vader.QueueNext(new AttackActor(m_PlayerID, -1, -1, false, 9999));
      m_Vader.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      m_Vader.QueueNext(new Lock());

      ActorInfo m_VaderE1 = ActorFactory.Get(m_VaderEscort1ID);
      m_VaderE1.ForceClearQueue();
      m_VaderE1.QueueNext(new AttackActor(m_PlayerID, -1, -1, false, 9999));
      m_VaderE1.QueueNext(new AttackActor(m_PlayerID, -1, -1, false, 9999));
      m_VaderE1.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      m_VaderE1.QueueNext(new Lock());

      ActorInfo m_VaderE2 = ActorFactory.Get(m_VaderEscort2ID);
      m_VaderE2.ForceClearQueue();
      m_VaderE2.QueueNext(new AttackActor(m_PlayerID, -1, -1, false, 9999));
      m_VaderE2.QueueNext(new AttackActor(m_PlayerID, -1, -1, false, 9999));
      m_VaderE2.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      m_VaderE2.QueueNext(new Lock());
    }

    public void Scene_Stage06_VaderEnd(GameEventArg arg)
    {
      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Game.GameTime + 8f, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 7.9f, Scene_Stage06_SetPlayer);
      AtmosphereInfo.SetPos_Sun(new TV_3DVECTOR(1000, 100, 0));
      AtmosphereInfo.ShowSun = true;

      StageNumber = 6;

      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(vaderend_distX + 900, -365, 0));
      SoundManager.SetMusic("ds_end_1_1", true);

      ActorInfo vader = ActorFactory.Get(m_VaderID);
      ActorInfo vaderE1 = ActorFactory.Get(m_VaderEscort1ID);
      ActorInfo vaderE2 = ActorFactory.Get(m_VaderEscort2ID);
      ActorInfo player = ActorFactory.Get(m_PlayerID);

      player.SetLocalPosition(vaderend_distX, -220, 0);
      player.SetLocalRotation(0, 90, 0);
      player.MoveData.ResetTurn();
      player.ForceClearQueue();
      player.QueueNext(new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
      player.QueueNext(new Lock());

      vader.SetLocalRotation(0, 90, 0);

      ActorInfo falcon = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("Millennium Falcon"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
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

        ActionManager.ForceClearQueue(m_VaderID);
        ActionManager.QueueNext(m_VaderID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_VaderID, new Lock());

        ActionManager.ForceClearQueue(m_VaderEscort1ID);
        ActionManager.QueueNext(m_VaderEscort1ID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_VaderEscort1ID, new Lock());

        ActionManager.ForceClearQueue(m_VaderEscort2ID);
        ActionManager.QueueNext(m_VaderEscort2ID, new Rotate(new TV_3DVECTOR(vader_distX + 50000, -220, 0), 400));
        ActionManager.QueueNext(m_VaderEscort2ID, new Lock());

      vader.HitEvents += Scene_Stage06_VaderFlee;
      vaderE1.HitEvents += Scene_Stage06_VaderFlee;
      vaderE2.HitEvents += Scene_Stage06_VaderFlee;

      //cam.MoveData.MaxSpeed = 25;
      //cam.MoveData.Speed = 25;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(falcon.ID);
    }

    public void Scene_Stage06_VaderFlee(GameEventArg arg)
    {
      if (Stage6VaderAttacking)
      {
        Stage6VaderAttacking = false;

        ActorInfo vader = ActorFactory.Get(m_VaderID);
        ActorInfo vaderE1 = ActorFactory.Get(m_VaderEscort1ID);
        ActorInfo vaderE2 = ActorFactory.Get(m_VaderEscort2ID);

        vader.ForceClearQueue();
        vader.MoveData.ApplyZBalance = false;
        vader.SetLocalRotation(-30, 85, 5);

        vader.DyingTimer.Set(2000, true);

        vader.SetState_Dying();
        vaderE2.SetLocalRotation(-5, 93, 0);
        vaderE2.SetState_Dying();
        vaderE1.SetState_Dying();
      }
    }

    #endregion


    #region Text
    public void Message_01_EnemyShipsInbound(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Enemy signals are approaching your position.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_MoreEnemyShipsInbound(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Another enemy signal are en route to your position.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Clear(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Area cleared. Resume your attack on the Death Star.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_04_Target(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Eliminate all Radar Towers.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_05_Clear(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Area cleared. Proceed to the next sector.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_06_TIE(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Enemy fighters detected.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07_Clear(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Deflector Towers detected en route to the trench.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08_Target(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Destroy all Deflection Towers and Radar Towers.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_09_Clear(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: The path to the trench line is clear.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_10_BeginRun(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Luke, start your attack run.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_100_RebelBaseInRange(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is in range of the Death Star.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_101_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in ONE minute.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_102_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWO minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_103_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THREE minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_105_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIVE minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_110_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TEN minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_115_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in FIFTEEN minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_120_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_125_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in TWENTY-FIVE minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_130_RebelBase(GameEventArg arg)
    {
      Screen2D.MessageText("MASSASSI OUTPOST: Our Rebel Base is will be in range in THIRTY minutes.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    #endregion
  }
}
