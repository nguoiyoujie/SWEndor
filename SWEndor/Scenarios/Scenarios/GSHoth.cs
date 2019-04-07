using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Sound;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSHoth : GameScenarioBase
  {
    public GSHoth()
    {
      Name = "Escape from Hoth (WIP up to Stage 02)";
      AllowedWings = new List<ActorTypeInfo> { FalconATI.Instance()
                                              };

      AllowedDifficulties = new List<string> { "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private ActorInfo m_AHoth = null;

    private List<object[]> m_pendingSDspawnlist = new List<object[]>();
    private int CurrentTIEs = 0;
    private int TIEsLeft = 20;
    private TV_3DVECTOR hyperspace_lane_min = new TV_3DVECTOR(-4000, -1000, -20000);
    private TV_3DVECTOR hyperspace_lane_max = new TV_3DVECTOR(4000, 1000, -8500);

    private float transport_hyperspaceZpos = -1000;
    private float transport_currentZpos = 10000;
    private ActorInfo m_Transport1 = null;
    private ActorInfo m_Transport2 = null;
    private ActorInfo m_Transport3 = null;

    private ActorInfo m_Player = null;
    private float m_Player_DamageModifier = 1;
    private string m_Player_PrimaryWeapon = "";
    private string m_Player_SecondaryWeapon = "";

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Instance().Name = "Solo";
    }

    public override void Launch()
    {
      base.Launch();

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);

      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(15000, 1500, 9000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-15000, -1500, -20000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(15000, 1500, 8000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-15000, -1500, -20000);

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_HyperspaceIn");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Empire_FirstWave");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5.5f, "Rebel_MakePlayer");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5.5f, "Message.01");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 9.5f, "Rebel_GiveControl");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 10f, "Message.02");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 6.5f, "Empire_FirstTIEWave");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 18, "Message.03");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 23, "Message.04");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 28, "Message.05");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 45, "Message.06");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 49, "Message.07");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 52, "Rebel_IonCannonSpawn");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 56, "Message.08");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 61, "Message.09");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 66, "Message.10");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 71, "Message.11");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 97, "Message.12");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 100, "Empire_SecondWave");

      PlayerInfo.Instance().Lives = 4;
      PlayerInfo.Instance().ScorePerLife = 1000000;
      PlayerInfo.Instance().ScoreForNextLife = 1000000;

      MakePlayer = Rebel_MakePlayer;

      GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      //GameScenarioManager.Instance().Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      SoundManager.Instance().SetMusic("battle_2_1");
      SoundManager.Instance().SetMusicLoop("battle_2_2");

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Falcon", new TV_COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      FactionInfo.Factory.Get("Rebels").Allies.Add(FactionInfo.Factory.Get("Rebels_Falcon"));
      FactionInfo.Factory.Get("Rebels_Falcon").Allies.Add(FactionInfo.Factory.Get("Rebels"));

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");

      MainAllyFaction.WingLimitIncludesAllies = true;
    }

    public override void LoadScene()
    {
      base.LoadScene();

      // Create Hoth
      if (m_AHoth == null)
      {
        ActorCreationInfo aci_Hoth = new ActorCreationInfo(HothATI.Instance())
        {
          InitialState = ActorState.FIXED,
          CreationTime = -1,
          Position = new TV_3DVECTOR(0, 800, -18000),
          Rotation = new TV_3DVECTOR(-90, 0, 0),
          InitialScale = new TV_3DVECTOR(6, 6, 6)
        };
        m_AHoth = ActorInfo.Create(aci_Hoth);
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AHoth != null && m_AHoth.CreationState == CreationState.ACTIVE)
      {
        float x_y4 = PlayerInfo.Instance().Position.x / 5f;
        float y_y4 = PlayerInfo.Instance().Position.y / 2f;
        float z_y4 = (PlayerInfo.Instance().Position.z > 0) ? PlayerInfo.Instance().Position.z / 1.5f + 30000f : PlayerInfo.Instance().Position.z / 100f + 30000f;
        m_AHoth.SetLocalPosition(x_y4, y_y4, z_y4);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      if (m_Player != null && m_Player.ActorState != ActorState.DEAD && m_Player.ActorState != ActorState.DYING)
      {
        MainEnemyFaction.WingLimit = 6 + MainAllyFaction.GetWings().Count * 2;
        if (MainEnemyFaction.WingLimit > 12)
          MainEnemyFaction.WingLimit = 12;

        if (StageNumber == 0)
        {
          StageNumber = 1;
        }
        else if (StageNumber == 1 && m_Transport1 != null)
        {
          if (transport_currentZpos > m_Transport1.GetPosition().z)
          {
            transport_currentZpos = m_Transport1.GetPosition().z;
          }
          if (m_Transport1.GetPosition().z < transport_hyperspaceZpos && m_Player.ActorState == ActorState.NORMAL && !GameScenarioManager.Instance().GetGameStateB("Stage1End") && !GameScenarioManager.Instance().GetGameStateB("GameOver"))
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_02");
            m_Transport1.CombatInfo.DamageModifier = 0;
            m_Transport2.CombatInfo.DamageModifier = 0;
            m_Transport3.CombatInfo.DamageModifier = 0;
            GameScenarioManager.Instance().SetGameStateB("Stage1End", true);
          }
        }
        else if (StageNumber == 2)
        {
          if (CurrentTIEs > MainEnemyFaction.Wings.Count)
          {
            TIEsLeft -= CurrentTIEs - MainEnemyFaction.Wings.Count;
          }
          CurrentTIEs = MainEnemyFaction.Wings.Count;

          if (TIEsLeft < 1 && !GameScenarioManager.Instance().GetGameStateB("Stage2b"))
          {
            GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
            GameScenarioManager.Instance().Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
            GameScenarioManager.Instance().Line1Text = "Proceed to";
            GameScenarioManager.Instance().Line2Text = "Hyperspace Lane";
            GameScenarioManager.Instance().SetGameStateB("Stage2b", true);
            Screen2D.Instance().Box3D_Enable = true;
            Screen2D.Instance().Box3D_color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Screen2D.Instance().Box3D_min = hyperspace_lane_min;
            Screen2D.Instance().Box3D_max = hyperspace_lane_max;
          }

          if (TIEsLeft < 1 && !GameScenarioManager.Instance().GetGameStateB("Stage2End") && !m_Player.IsOutOfBounds(hyperspace_lane_min, hyperspace_lane_max))
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_02b_LightspeedFail");
            GameScenarioManager.Instance().SetGameStateB("Stage2End", true);
            Screen2D.Instance().Box3D_Enable = false;
          }
        }
      }

      if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.GetShips().Count < 7)
      {
        if (m_pendingSDspawnlist[0].Length > 0
        && (!(m_pendingSDspawnlist[0][0] is ImperialIATI) || MainEnemyFaction.GetShips().Count < 7)
        && (!(m_pendingSDspawnlist[0][0] is DevastatorATI) || MainEnemyFaction.GetShips().Count < 2))
        {
          Empire_StarDestroyer_Spawn(m_pendingSDspawnlist[0]);
          m_pendingSDspawnlist.RemoveAt(0);
        }
      }

      if (StageNumber == 1)
      {
        if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().GameTime || Game.Instance().GameTime % 0.2f > 0.1f)
          GameScenarioManager.Instance().Line1Text = string.Format("WINGS: {0}", MainAllyFaction.GetWings().Count);
        else
          GameScenarioManager.Instance().Line1Text = "";

        if (!GameScenarioManager.Instance().GetGameStateB("Stage1End"))
          GameScenarioManager.Instance().Line2Text = string.Format("DIST: {0:00000}", transport_currentZpos - transport_hyperspaceZpos);
        else
          GameScenarioManager.Instance().Line2Text = "";
      }
      else if (StageNumber == 2)
      {
        if (TIEsLeft > 0)
        {
          GameScenarioManager.Instance().Line1Text = "Destroy TIEs";
          GameScenarioManager.Instance().Line2Text = string.Format("TIEs: {0}", TIEsLeft);
        }
        else if (!GameScenarioManager.Instance().GetGameStateB("Stage2End"))
        {
        }
        else
        {
          GameScenarioManager.Instance().Line1Text = "";
          GameScenarioManager.Instance().Line2Text = "";
        }
      }
      else
      {
        GameScenarioManager.Instance().Line2Text = "";
      }
    }

    public override void RegisterEvents()
    {
      base.RegisterEvents();
      GameEvent.RegisterEvent("Rebel_HyperspaceIn", Rebel_HyperspaceIn);
      GameEvent.RegisterEvent("Rebel_IonCannonSpawn", Rebel_IonCannonSpawn);
      GameEvent.RegisterEvent("Rebel_MakePlayer", Rebel_MakePlayer);
      GameEvent.RegisterEvent("Rebel_GiveControl", Rebel_GiveControl);
      GameEvent.RegisterEvent("Rebel_CriticalUnitLost", Rebel_CriticalUnitLost);
      GameEvent.RegisterEvent("Rebel_VulnerableTransport", Rebel_VulnerableTransport);
      GameEvent.RegisterEvent("Rebel_Delete", Rebel_Delete);
      GameEvent.RegisterEvent("Rebel_PlayerHit", Rebel_PlayerHit);
      
      GameEvent.RegisterEvent("Empire_FirstWave", Empire_FirstWave);
      GameEvent.RegisterEvent("Empire_SecondWave", Empire_SecondWave);
      GameEvent.RegisterEvent("Empire_StarDestroyer_Spawn", Empire_StarDestroyer_Spawn);
      GameEvent.RegisterEvent("Empire_FirstTIEWave", Empire_FirstTIEWave);
      GameEvent.RegisterEvent("Empire_TIEWave", Empire_TIEWave);
      GameEvent.RegisterEvent("Empire_TIEWave_TIEsvsShips", Empire_TIEWave_TIEsvsShips);

      GameEvent.RegisterEvent("Scene_02", Scene_02);
      GameEvent.RegisterEvent("Scene_02_Switch", Scene_02_Switch);
      GameEvent.RegisterEvent("Scene_02b_LightspeedFail", Scene_02b_LightspeedFail);
      GameEvent.RegisterEvent("Scene_02_ViolentShake", Scene_02_ViolentShake);
      GameEvent.RegisterEvent("Scene_EnterCutscene", Scene_EnterCutscene);
      GameEvent.RegisterEvent("Scene_ExitCutscene", Scene_ExitCutscene);

      GameEvent.RegisterEvent("Message.01", Message_01_Leaving);
      GameEvent.RegisterEvent("Message.02", Message_02_Conditions);
      GameEvent.RegisterEvent("Message.03", Message_03_Turbolasers);
      GameEvent.RegisterEvent("Message.04", Message_04_TIEs);
      GameEvent.RegisterEvent("Message.05", Message_05);
      GameEvent.RegisterEvent("Message.06", Message_06);
      GameEvent.RegisterEvent("Message.07", Message_07);
      GameEvent.RegisterEvent("Message.08", Message_08);
      GameEvent.RegisterEvent("Message.09", Message_09);
      GameEvent.RegisterEvent("Message.10", Message_10);
      GameEvent.RegisterEvent("Message.11", Message_11);
      GameEvent.RegisterEvent("Message.12", Message_12);
      GameEvent.RegisterEvent("Message.13", Message_13);
      GameEvent.RegisterEvent("Message.14", Message_14);
      GameEvent.RegisterEvent("Message.15", Message_15);
      GameEvent.RegisterEvent("Message.16", Message_16);
      GameEvent.RegisterEvent("Message.17", Message_17);
      GameEvent.RegisterEvent("Message.18", Message_18);
      GameEvent.RegisterEvent("Message.19", Message_19);
      GameEvent.RegisterEvent("Message.20", Message_20);
      GameEvent.RegisterEvent("Message.21", Message_21);
      GameEvent.RegisterEvent("Message.22", Message_22);
      GameEvent.RegisterEvent("Message.23", Message_23);
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(200, 350, GameScenarioManager.Instance().MaxBounds.z - 1500);

      // Player Falcon
      ainfo = new ActorSpawnInfo
      {
        Type = PlayerInfo.Instance().ActorType,
        Name = "(Player)",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.Instance().GameTime,
        Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
        Position = new TV_3DVECTOR(0, 200, GameScenarioManager.Instance().MaxBounds.z - 450),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);

      ainfo.SecondaryWeapons = new string[] { "front", "rear" };
      ainfo.CombatInfo.DamageModifier = 0.1f;
      ainfo.HitEvents.Add("Rebel_PlayerHit");
      GameScenarioManager.Instance().CameraTargetActor = ainfo;
      PlayerInfo.Instance().TempActor = ainfo;

      // X-Wings x12
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 11; i++)
      {
        if (i % 4 == 1)
          positions.Add(new TV_3DVECTOR(-16 * (i + 1) - 250, 10 * i, GameScenarioManager.Instance().MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 2)
          positions.Add(new TV_3DVECTOR(10 * i, -16 * (i + 2) - 250, GameScenarioManager.Instance().MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 3)
          positions.Add(new TV_3DVECTOR(-10 * i, 16 * (i + 2) + 250, GameScenarioManager.Instance().MaxBounds.z - 1150 + 250 * i));
        else
          positions.Add(new TV_3DVECTOR(16 * (i + 2) + 250, 10 * i, GameScenarioManager.Instance().MaxBounds.z - 1150 + 250 * i));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = XWingATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Lock(), new Wait(3) },
          Registries = null
        }.Spawn(this);

        ainfo.CombatInfo.DamageModifier = 0.85f;
      }

      // Transport x3
      positions.Clear();
      positions.Add(new TV_3DVECTOR(250, -220, GameScenarioManager.Instance().MaxBounds.z + 20));
      positions.Add(new TV_3DVECTOR(-100, -20, GameScenarioManager.Instance().MaxBounds.z + 70));
      positions.Add(new TV_3DVECTOR(50, 270, GameScenarioManager.Instance().MaxBounds.z + 1020));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = TransportATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "TRANSPORT",
          SpawnTime = Game.Instance().GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Lock(), new Wait(3) },
          Registries = new string[] { "CriticalAllies" }
        }.Spawn(this);

        ainfo.CombatInfo.DamageModifier = 0.6f;

        switch (i)
        {
          case 0:
            m_Transport1 = ainfo;
            break;
          case 1:
            m_Transport2 = ainfo;
            break;
          case 2:
            m_Transport3 = ainfo;
            break;
        }

        ainfo.HitEvents.Add("Rebel_VulnerableTransport");
        ainfo.ActorStateChangeEvents.Add("Rebel_CriticalUnitLost");
      }
    }

    public void Rebel_VulnerableTransport(object[] param)
    {
      ActorInfo vic = (ActorInfo)param[0];
      ActorInfo att = (ActorInfo)param[1];
      if (att.TypeInfo is GreenAntiShipLaserATI)
      {
        vic.CombatInfo.Strength -= 1.5f;
      }
    }

    public void Rebel_IonCannonSpawn(object[] param)
    {
      TV_3DVECTOR position = new TV_3DVECTOR(0, 500, GameScenarioManager.Instance().MaxBounds.z + 3000);
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      if (MainEnemyFaction.Ships.Count > 0)
      {
        ActorInfo target = new List<ActorInfo>(MainEnemyFaction.Ships)[0];
        TV_3DVECTOR dir = target.GetPosition() - position;
        rotation = Utilities.GetRotation(dir);
      }
      else
      {
        rotation = new TV_3DVECTOR(0, 180, 0);
      }

      new ActorSpawnInfo
      {
        Type = BigIonLaserATI.Instance(),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.Instance().GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = position,
        Rotation = rotation,
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);
    }

    public void Rebel_MakePlayer(object[] param)
    {
      PlayerInfo.Instance().Actor = PlayerInfo.Instance().TempActor;

      if (PlayerInfo.Instance().Actor == null || PlayerInfo.Instance().Actor.CreationState == CreationState.DISPOSED)
      {
        if (PlayerInfo.Instance().Lives > 0)
        {
          PlayerInfo.Instance().Lives--;

          TV_3DVECTOR position = new TV_3DVECTOR();
          if (StageNumber == 1)
          {
            if (GameScenarioManager.Instance().CriticalAllies.Count > 0)
              position = new List<ActorInfo>(GameScenarioManager.Instance().CriticalAllies.Values)[0].GetRelativePositionXYZ(0, -100, -1750);
            else
              position = new TV_3DVECTOR(0, 300, GameScenarioManager.Instance().MaxBounds.z - 850);
          }
          else if (StageNumber == 2)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR();
            foreach (ActorInfo en in MainEnemyFaction.GetShips())
              pos += en.Position;
            pos /= MainEnemyFaction.GetShips().Count;

            position = pos;
          }

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = PlayerInfo.Instance().ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Game.Instance().GameTime,
            Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
            Position = position,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock() },
            Registries = null
          }.Spawn(this);

          ainfo.SecondaryWeapons = new string[] { "front", "rear" };
          ainfo.CombatInfo.DamageModifier = 0.1f;
          ainfo.HitEvents.Add("Rebel_PlayerHit");

          PlayerInfo.Instance().Actor = ainfo;
        }
      }
      m_Player = PlayerInfo.Instance().Actor;
    }

    public void Rebel_GiveControl(object[] param)
    {
      foreach (ActorInfo a in MainAllyFaction.GetWings())
      {
        ActionManager.Unlock(a);
        a.ActorState = ActorState.NORMAL;
        a.MovementInfo.Speed = a.MovementInfo.MaxSpeed;
      }
      PlayerInfo.Instance().IsMovementControlsEnabled = true;

      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
    }

    public void Rebel_Delete(object[] param)
    {
      foreach (ActorInfo a in MainAllyFaction.GetWings())
      {
        if (!a.IsPlayer())
        {
          a.Destroy();
        }
      }

      foreach (ActorInfo a in MainAllyFaction.GetShips())
      {
        if (!a.IsPlayer())
        {
          a.Destroy();
        }
      }
    }

    public void Rebel_PlayerHit(object[] param)
    {
      ActorInfo player = (ActorInfo)param[0];
      ActorInfo attacker = (ActorInfo)param[1];

      if (!attacker.TypeInfo.IsDamage) 
      {
        player.CombatInfo.Strength -= attacker.TypeInfo.ImpactDamage;
        PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
      }
      else
      {
        TV_3DVECTOR rot = player.GetRotation();
        TV_3DVECTOR tgtrot = Utilities.GetRotation(attacker.GetRelativePositionFUR(-1000, 0, 0) - player.PrevPosition);

        float chgy = tgtrot.y - rot.y;

        while (chgy < -180)
          chgy += 360;

        while (chgy > 180)
          chgy -= 360;

        if ((chgy < -90 || chgy > 90) && PlayerInfo.Instance().SecondaryWeapon != "rear")
        {
          player.CombatInfo.Strength -= 1;
          PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
        }
        else if ((chgy > -90 && chgy < 90) && PlayerInfo.Instance().SecondaryWeapon != "front")
        {
          player.CombatInfo.Strength -= 1;
          PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
        }
        else
        {

          PlayerCameraInfo.Instance().Shake = 2 * attacker.TypeInfo.ImpactDamage;
        }
      }
    }

    public void Rebel_CriticalUnitLost(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (GameScenarioManager.Instance().GetGameStateB("GameWon"))
        return;

      if (GameScenarioManager.Instance().GetGameStateB("GameOver"))
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
          PlayerInfo.Instance().Actor.CombatInfo.Strength = 0;

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

          if (ainfo.TypeInfo is TransportATI)
          {
            //GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15, "Message.92");
            ainfo.CombatInfo.TimedLife = 2000f;
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
        SidebarName = "",
        SpawnTime = Game.Instance().GameTime,
        Faction = MainEnemyFaction,
        Position = position + hyperspaceInOffset,
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new HyperspaceIn(position)
                                                    , new Move(targetposition, type.MaxSpeed)
                                                    , new Rotate(targetposition, type.MinSpeed, 360, false)
                                                    , new Lock()},
        Registries = null
      }.Spawn(this);

      ainfo.SetSpawnerEnable(true);
      if (ainfo.SpawnerInfo != null)
      {
        ainfo.SpawnerInfo.SpawnInterval = ainfo.SpawnerInfo.SpawnInterval * 3;
        if (param.GetLength(0) >= 4 && param[3] is int)
          ainfo.SpawnerInfo.SpawnsRemaining = (int)param[3];
      }
      ainfo.CombatInfo.DamageModifier = 0.1f;
    }

    public void Empire_FirstWave(object[] param)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-8000, -350, 0), new TV_3DVECTOR(-2000, -350, 7000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          Empire_TIEWave_TIEsvsShips(new object[] { 2 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          break;
        case "normal":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          break;
        case "easy":
        default:
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          break;
      }
    }

    public void Empire_SecondWave(object[] param)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 5 });
          Empire_TIEWave_TIEsvsShips(new object[] { 3 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 4 });
          Empire_TIEWave_TIEsvsShips(new object[] { 2 });
          break;
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 2 });
          Empire_TIEWave_TIEsvsShips(new object[] { 1 });
          break;
      }
      GameScenarioManager.Instance().SetGameStateB("Stage1BBegin", true);
    }

    public void Empire_FirstTIEWave(object[] param)
    {
      int count = 0;
      switch (Difficulty.ToLower())
      {
        case "mental":
          count = 4;
          Empire_TIEWave_TIEsvsShips(new object[] { 2 });
          break;
        case "hard":
          count = 2;
          break;
        case "normal":
          count = 0;
          break;
        case "easy":
        default:
          count = 0;
          break;
      }

      // TIEs
      for (int k = 1; k < count; k++)
      {
        float fx = Engine.Instance().Random.Next(-500, 500);
        float fy = Engine.Instance().Random.Next(-500, 0);
        float fz = Engine.Instance().Random.Next(-2500, 2500);

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

            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = TIE_LN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, fz - 2500 - k * 100),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
              Registries = null
            }.Spawn(this);
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
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-200, 800);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActionInfo[] actions = new ActionInfo[] { new Wait(15) };
            switch (Difficulty.ToLower())
            {
              case "mental":
              case "hard":
                actions = new ActionInfo[] { new Wait(15), new Hunt(TargetType.FIGHTER) };
                break;
            }

            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = TIE_LN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 5000),
              Rotation = new TV_3DVECTOR(),
              Actions = actions,
              Registries = null
            }.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEWave_TIEsvsShips(object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE only
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = TIE_LN_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MinBounds.z - 2500),
              Rotation = new TV_3DVECTOR(),
              Actions = new ActionInfo[] { new Wait(18)
                                              , new Hunt(TargetType.SHIP)
                                              },
              Registries = null
            }.Spawn(this);
          }
        }
        t += 1.5f;
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
        m_Player_DamageModifier = m_Player.CombatInfo.DamageModifier;
        m_Player.CombatInfo.DamageModifier = 0;
        ActionManager.ForceClearQueue(m_Player);
        ActionManager.QueueNext(m_Player, new Lock());
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
        m_Player.CombatInfo.DamageModifier = m_Player_DamageModifier;
        ActionManager.ForceClearQueue(m_Player);
      }
      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public void Scene_02_Switch(object[] param)
    {
      StageNumber = 2;
      GameScenarioManager.Instance().Line1Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      switch (Difficulty.ToLower())
      {
        case "mental":
          TIEsLeft = 50;
          break;
        case "hard":
          TIEsLeft = 40;
          break;
        case "easy":
          TIEsLeft = 16;
          break;
        case "normal":
        default:
          TIEsLeft = 25;
          break;
      }
      
    }

    public void Scene_02(object[] param)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_EnterCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7, "Scene_ExitCutscene");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 9, "Rebel_GiveControl");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 14, "Rebel_Delete");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 11, "Message.13");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 13, "Scene_02_Switch");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15, "Message.14");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 25, "Message.15");

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(400, 130, -1800);

      GameScenarioManager.Instance().SceneCamera.MovementInfo.MaxSpeed = 50;
      GameScenarioManager.Instance().SceneCamera.MovementInfo.Speed = 50;
      GameScenarioManager.Instance().CameraTargetActor = m_Player;
      PlayerInfo.Instance().TempActor = m_Player;

      int counter = 0;
      int x = 0;
      int xn = -1;
      int y = 0;
      int z = -1400;

      foreach (ActorInfo ainfo in MainEnemyFaction.GetWings())
        ainfo.Destroy();

      foreach (ActorInfo ainfo in MainAllyFaction.GetWings())
      {
        x = xn * Engine.Instance().Random.Next(200, 600);
        xn *= -1;
        y = Engine.Instance().Random.Next(-200, 200);
        z += 200;
        ActionManager.ForceClearQueue(ainfo);
        ActionManager.QueueNext(ainfo, new Wait(8 + 0.2f * counter));
        ActionManager.QueueNext(ainfo, new HyperspaceOut());
        ainfo.SetLocalPosition(x, y, z);
        ainfo.SetLocalRotation(0, 180, 0);
        ainfo.MovementInfo.Speed = 450;
        ainfo.MovementInfo.ResetTurn();
        ainfo.CanRetaliate = false;
        ainfo.CanEvade = false;
        counter++;
      }

      m_Transport1.CombatInfo.DamageModifier = 0;
      m_Transport2.CombatInfo.DamageModifier = 0;
      m_Transport3.CombatInfo.DamageModifier = 0;

      m_Player.SetLocalPosition(0, 0, 500);

      ActionManager.ForceClearQueue(m_Transport1);
      ActionManager.QueueNext(m_Transport1, new Wait(8.5f));
      ActionManager.QueueNext(m_Transport1, new HyperspaceOut());
      m_Transport1.MovementInfo.MaxSpeed = 400;
      m_Transport1.MovementInfo.Speed = 400;

      ActionManager.ForceClearQueue(m_Transport2);
      ActionManager.QueueNext(m_Transport2, new Wait(8.8f));
      ActionManager.QueueNext(m_Transport2, new HyperspaceOut());
      m_Transport2.MovementInfo.MaxSpeed = 400;
      m_Transport2.MovementInfo.Speed = 400;

      ActionManager.ForceClearQueue(m_Transport3);
      ActionManager.QueueNext(m_Transport3, new Wait(9.1f));
      ActionManager.QueueNext(m_Transport3, new HyperspaceOut());
      m_Transport3.MovementInfo.MaxSpeed = 400;
      m_Transport3.MovementInfo.Speed = 400;

      m_Player.MovementInfo.Speed = 400;
      m_Player.MovementInfo.ResetTurn(); 
      ActionManager.ForceClearQueue(m_Player);
      ActionManager.QueueNext(m_Player, new Lock());
      

      int en_ship = 0;
      foreach (ActorInfo ainfo in MainEnemyFaction.GetShips())
      {
        if (en_ship == 1)
        {
          ainfo.SetLocalPosition(0, -300, 2500);
          ainfo.SetLocalRotation(0, 180, 0);
          ainfo.MovementInfo.Speed = ainfo.MovementInfo.MaxSpeed * 0.25f;
          ActionManager.ForceClearQueue(ainfo);
          ActionManager.QueueNext(ainfo, new Wait(60));
          ActionManager.QueueNext(ainfo, new Rotate(new TV_3DVECTOR(-2000, -300, 2000), 0, -1, false));
          ActionManager.QueueNext(ainfo, new Lock());
        }
        else if (en_ship == 2)
        {
          ainfo.SetLocalPosition(3300, 150, 5500);
          ainfo.LookAtPoint(new TV_3DVECTOR(1400, 150, 1000));
          ainfo.MovementInfo.Speed = ainfo.MovementInfo.MaxSpeed * 0.25f;
          ActionManager.ForceClearQueue(ainfo);
          ActionManager.QueueNext(ainfo, new Wait(30));
          ActionManager.QueueNext(ainfo, new Rotate(new TV_3DVECTOR(-32000, 150, 2000), 0, -1, false));
          ActionManager.QueueNext(ainfo, new Lock());
        }
        else
        {
          ainfo.Destroy();
        }
        en_ship++;
      }

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ImperialIATI.Instance(),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.Instance().GameTime + 9,
        Faction = MainEnemyFaction,
        Position = new TV_3DVECTOR(20000, -2000, -22000),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(2000, 100, -8000))
                                    , new Move(new TV_3DVECTOR(1000, 100, 2000), ImperialIATI.Instance().MaxSpeed * 0.25f, -1, false)
                                    , new Rotate(new TV_3DVECTOR(2000, 100, -9000), 0, -1, false)
                                    , new Lock() },
        Registries = null
      };
      ActorInfo newDest = asi.Spawn(this);
      newDest.CombatInfo.DamageModifier = 0.1f;

      asi.SpawnTime = Game.Instance().GameTime + 9.25f;
      asi.Position = new TV_3DVECTOR(20000, -2000, -25000);
      asi.Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(1500, -100, -10200))
                                            , new Move(new TV_3DVECTOR(-6500, -100, 4000), ImperialIATI.Instance().MaxSpeed * 0.25f, -1, false)
                                            , new Rotate(new TV_3DVECTOR(2000, -100, -10200), 0, -1, false)
                                            , new Lock() };
      newDest = asi.Spawn(this);
      newDest.CombatInfo.DamageModifier = 0.1f;
    }

    public void Scene_02b_LightspeedFail(object[] param)
    {
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 1, "Message.16");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5, "Message.17");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 12, "Scene_02_ViolentShake");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15, "Message.18");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 18, "Message.19");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 23, "Message.20");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 28, "Message.21");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 33, "Message.22");
      //GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 33, "Scene_03");

      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-15000, -1500, -30000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-15000, -1500, -30000);

      SoundManager.Instance().SetMusic("battle_2_3");
      SoundManager.Instance().SetMusicLoop("battle_2_3");
    }

    public void Scene_02_ViolentShake(object[] param)
    {
      PlayerCameraInfo.Instance().Shake = 75;
    }

    #endregion

    #region Text
    public void Message_01_Leaving(object[] param)
    {
      Screen2D.Instance().MessageText("ECHO BASE: Escort the transports to the designated locations for hyperspace jump. ", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_Conditions(object[] param)
    {
      Screen2D.Instance().MessageText("ECHO BASE: All transports must survive.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Turbolasers(object[] param)
    {
      Screen2D.Instance().MessageText("TRANSPORT: The Heavy Turbolaser Towers on the Star Destroyers must be taken out.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_04_TIEs(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING: We will need to worry about the TIEs too.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_05(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: We will figure something out.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_06(object[] param)
    {
      Screen2D.Instance().MessageText("ECHO BASE: Ion Control, standby.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07(object[] param)
    {
      Screen2D.Instance().MessageText("ECHO BASE: Fire.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: Here's our opportunity.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_09(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING: We will take care of the fighters.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_10(object[] param)
    {
      Screen2D.Instance().MessageText("X-WING: We need someone to take out the Heavy Turbolasers.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_11(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: I can take care of that.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_12(object[] param)
    {
      Screen2D.Instance().MessageText("ECHO BASE: More Star Destroyers incoming.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_13(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: I see them. Two Star Destroyers here coming right at us.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_14(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: [Use the secondary weapon toggle to switch between front and rear deflector shields.]", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_15(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: We can still out-manuever them.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_16(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: Prepare to make the jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_17(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: Watch this!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_18(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: ...", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_19(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: I think we are in trouble.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_20(object[] param)
    {
      Screen2D.Instance().MessageText("C3PO: The hyperdrive modulator has been damaged, sir.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_21(object[] param)
    {
      Screen2D.Instance().MessageText("C3PO: It is impossible to jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_22(object[] param)
    {
      Screen2D.Instance().MessageText("SOLO: We are in trouble!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_23(object[] param)
    {
      Screen2D.Instance().MessageText("LEIA: Han, get up here!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }
    #endregion
  }
}
