using MTV3D65;
using System.Collections.Generic;
using System;

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

    private ActorInfo m_AScene = null;
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

      if (GameScenarioManager.Instance().GetGameStateB("in_game"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_game", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);

      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(15000, 1500, 8000);
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
      PlayerInfo.Instance().Score = new ScoreInfo();

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

      FactionInfo.AddFaction("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Rebels_Falcon", new TV_COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      FactionInfo.Get("Rebels").Allies.Add(FactionInfo.Get("Rebels_Falcon"));
      FactionInfo.Get("Rebels_Falcon").Allies.Add(FactionInfo.Get("Rebels"));
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

      // Create Hoth
      if (m_AHoth == null)
      {
        acinfo = new ActorCreationInfo(HothATI.Instance());
        acinfo.InitialState = ActorState.FIXED;
        acinfo.CreationTime = -1;
        acinfo.Position = new TV_3DVECTOR(0, 800, -18000);
        acinfo.Rotation = new TV_3DVECTOR(-90, 0, 0);
        acinfo.InitialScale = new TV_3DVECTOR(6, 6, 6);
        m_AHoth = ActorInfo.Create(acinfo);
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
        if (GameScenarioManager.Instance().StageNumber == 0)
        {
          GameScenarioManager.Instance().StageNumber = 1;
        }
        else if (GameScenarioManager.Instance().StageNumber == 1 && m_Transport1 != null)
        {
          if (transport_currentZpos > m_Transport1.GetPosition().z)
          {
            transport_currentZpos = m_Transport1.GetPosition().z;
          }
          if (m_Transport1.GetPosition().z < transport_hyperspaceZpos && m_Player.ActorState == ActorState.NORMAL && !GameScenarioManager.Instance().GetGameStateB("Stage1End") && !GameScenarioManager.Instance().GetGameStateB("GameOver"))
          {
            GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Scene_02");
            m_Transport1.DamageModifier = 0;
            m_Transport2.DamageModifier = 0;
            m_Transport3.DamageModifier = 0;
            GameScenarioManager.Instance().SetGameStateB("Stage1End", true);
          }
        }
        else if (GameScenarioManager.Instance().StageNumber == 2)
        {
          if (CurrentTIEs > GameScenarioManager.Instance().EnemyFighters.Count)
          {
            TIEsLeft -= CurrentTIEs - GameScenarioManager.Instance().EnemyFighters.Count;
          }
          CurrentTIEs = GameScenarioManager.Instance().EnemyFighters.Count;

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

      if (m_pendingSDspawnlist.Count > 0 && GameScenarioManager.Instance().EnemyShips.Count < 7)
      {
        if (m_pendingSDspawnlist[0].Length > 0
        && (!(m_pendingSDspawnlist[0][0] is ImperialIATI) || GameScenarioManager.Instance().EnemyShips.Count < 7)
        && (!(m_pendingSDspawnlist[0][0] is DevastatorATI) || GameScenarioManager.Instance().EnemyShips.Count < 2))
        {
          Empire_StarDestroyer_Spawn(m_pendingSDspawnlist[0]);
          m_pendingSDspawnlist.RemoveAt(0);
        }
      }

      if (GameScenarioManager.Instance().StageNumber == 1)
      {
        if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().GameTime || Game.Instance().GameTime % 0.4f > 0.2f)
          GameScenarioManager.Instance().Line1Text = string.Format("WINGS: {0}", GameScenarioManager.Instance().Scenario.RebelFighterLimit);
        else
          GameScenarioManager.Instance().Line1Text = "";

        if (!GameScenarioManager.Instance().GetGameStateB("Stage1End"))
          GameScenarioManager.Instance().Line2Text = string.Format("DIST: {0:00000}", transport_currentZpos - transport_hyperspaceZpos);
        else
          GameScenarioManager.Instance().Line2Text = "";
      }
      else if (GameScenarioManager.Instance().StageNumber == 2)
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
      GameEvent.RegisterEvent("Empire_SDSpawner", Empire_SDSpawner);

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
      ActorCreationInfo acinfo;
      ActorInfo ainfo;

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(200, -100, GameScenarioManager.Instance().MaxBounds.z - 2000);

      // Player Falcon
      acinfo = new ActorCreationInfo(PlayerInfo.Instance().ActorType);
      acinfo.Name = "(Player)";
      acinfo.Faction = FactionInfo.Get("Rebels_Falcon");
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = Game.Instance().GameTime;
      acinfo.Position = new TV_3DVECTOR(0, 0, GameScenarioManager.Instance().MaxBounds.z - 550);
      acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
      ainfo = ActorInfo.Create(acinfo);
      GameScenarioManager.Instance().AllyFighters.Add(ainfo.Name, ainfo);
      RegisterEvents(ainfo);
      ainfo.SecondaryWeapons = new List<string> { "front", "rear" };
      ainfo.DamageModifier = 0.1f;
      ainfo.HitEvents.Add("Rebel_PlayerHit");

      ActionManager.QueueLast(ainfo, new Actions.Lock());
      ActionManager.QueueLast(ainfo, new Actions.Move(new TV_3DVECTOR(acinfo.Position.x + Engine.Instance().Random.Next(-5, 5), acinfo.Position.y + Engine.Instance().Random.Next(-5, 5), acinfo.Position.z - 4500)
                                                    , ainfo.MaxSpeed));

      GameScenarioManager.Instance().CameraTargetActor = ainfo;

      // X-Wings x12
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 11; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(-160 * (i + 1), 10 * i, GameScenarioManager.Instance().MaxBounds.z - 1150 + 250 * i));
        else
          positions.Add(new TV_3DVECTOR(160 * (i + 2), 10 * i, GameScenarioManager.Instance().MaxBounds.z - 1150 + 250 * i));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(XWingATI.Instance());
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        acinfo.CreationTime = Game.Instance().GameTime;
        acinfo.Position = v;
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        ainfo = ActorInfo.Create(acinfo);
        ainfo.DamageModifier = 0.75f;

        ActionManager.QueueLast(ainfo, new Actions.Lock());
        ActionManager.QueueLast(ainfo, new Actions.Wait(3));

        GameScenarioManager.Instance().AllyFighters.Add(ainfo.Key, ainfo);
        RegisterEvents(ainfo);
      }

      // Transport x3
      positions.Clear();
      positions.Add(new TV_3DVECTOR(650, -220, GameScenarioManager.Instance().MaxBounds.z + 1020));
      positions.Add(new TV_3DVECTOR(-500, -20, GameScenarioManager.Instance().MaxBounds.z + 1070));
      positions.Add(new TV_3DVECTOR(50, 270, GameScenarioManager.Instance().MaxBounds.z + 2020));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(TransportATI.Instance());
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        acinfo.CreationTime = Game.Instance().GameTime;
        acinfo.Position = v;
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
        ainfo = ActorInfo.Create(acinfo);
        ainfo.SideBarName = "TRANSPORT";
        ainfo.DamageModifier = 0.6f;

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

        ActionManager.QueueLast(ainfo, new Actions.Lock());

        GameScenarioManager.Instance().AllyShips.Add(ainfo.Key, ainfo);
        GameScenarioManager.Instance().CriticalAllies.Add(ainfo.Key, ainfo);
        RegisterEvents(ainfo);

        ainfo.HitEvents.Add("Rebel_VulnerableTransport");
        ainfo.ActorStateChangeEvents.Add("Rebel_CriticalUnitLost");
      }

      RebelFighterLimit = GameScenarioManager.Instance().AllyFighters.Count;
    }

    public void Rebel_VulnerableTransport(object[] param)
    {
      ActorInfo vic = (ActorInfo)param[0];
      ActorInfo att = (ActorInfo)param[1];
      if (att.TypeInfo is GreenAntiShipLaserATI)
      {
        vic.Strength -= 1.5f;
      }
    }

    public void Rebel_IonCannonSpawn(object[] param)
    {
      ActorCreationInfo acinfo;
      ActorInfo ainfo;

      // Player Falcon
      acinfo = new ActorCreationInfo(BigIonLaserATI.Instance());
      acinfo.Faction = FactionInfo.Get("Rebels");
      acinfo.InitialState = ActorState.NORMAL;
      acinfo.CreationTime = Game.Instance().GameTime;
      acinfo.Position = new TV_3DVECTOR(0, 500, GameScenarioManager.Instance().MaxBounds.z + 3000);

      if (GameScenarioManager.Instance().EnemyShips.Count > 0)
      {
        ActorInfo target = new List<ActorInfo>(GameScenarioManager.Instance().EnemyShips.Values)[0];
        TV_3DVECTOR dir = new TV_3DVECTOR();
        dir = target.GetPosition() - acinfo.Position;

        acinfo.Rotation = Utilities.GetRotation(dir);
      }
      else
      {
        acinfo.Rotation = new TV_3DVECTOR(0, 180, 0);
      }

      ainfo = ActorInfo.Create(acinfo);
      RegisterEvents(ainfo);

      ActionManager.QueueLast(ainfo, new Actions.Lock());

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
          placi.Faction = FactionInfo.Get("Rebels_Falcon");
          placi.InitialState = ActorState.NORMAL;
          placi.Rotation = new TV_3DVECTOR(0, 180, 0);

          if (GameScenarioManager.Instance().StageNumber == 1)
          {
            if (GameScenarioManager.Instance().CriticalAllies.Count > 0)
            {
              ActorInfo target = new List<ActorInfo>(GameScenarioManager.Instance().CriticalAllies.Values)[0];
              TV_3DVECTOR dir = new TV_3DVECTOR();
              placi.Position = target.GetRelativePositionXYZ(0, -100, -1750);
            }
            else
            {
              placi.Position = new TV_3DVECTOR(0, 300, GameScenarioManager.Instance().MaxBounds.z - 550);
            }
          }
          else
          {
            placi.Position = new TV_3DVECTOR(3500, -400, 0);
            placi.Rotation = new TV_3DVECTOR(0, -90, 0);
          }


          ActorInfo ainfo = ActorInfo.Create(placi);
          ainfo.SecondaryWeapons = new List<string> { "front", "rear" };
          ainfo.DamageModifier = 0.1f;
          ainfo.HitEvents.Add("Rebel_PlayerHit");

          PlayerInfo.Instance().Actor = ainfo;
          GameScenarioManager.Instance().AllyFighters.Add("(Player)", PlayerInfo.Instance().Actor);
          RegisterEvents(ainfo);
        }
      }
      m_Player = PlayerInfo.Instance().Actor;
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

    public void Rebel_Delete(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        if (!a.IsPlayer())
        {
          a.Destroy();
        }
      }

      foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
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
        player.Strength -= attacker.TypeInfo.ImpactDamage;
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
          player.Strength -= 1;
          PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
        }
        else if ((chgy > -90 && chgy < 90) && PlayerInfo.Instance().SecondaryWeapon != "front")
        {
          player.Strength -= 1;
          PlayerInfo.Instance().FlashHit(PlayerInfo.Instance().HealthColor);
        }
        else
        {
          PlayerInfo.Instance().shake_displacement = 2 * attacker.TypeInfo.ImpactDamage;
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
          PlayerInfo.Instance().Actor.Strength = 0;

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
            ainfo.TimedLife = 2000f;
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
      ActionManager.QueueLast(ainfo, new Actions.Rotate(targetposition, ainfo.MinSpeed, 360, false));
      ActionManager.QueueLast(ainfo, new Actions.Lock());

      GameScenarioManager.Instance().EnemyShips.Add(ainfo.Key, ainfo);
      //GameScenarioManager.Instance().CriticalEnemies.Add("IMPERIAL I STAR DESTROYER" + ainfo.ID, ainfo);
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
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-8000, -350, 0), new TV_3DVECTOR(-2000, -350, 7000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
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
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 5 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 4 });
          break;
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { ImperialIATI.Instance(), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 2 });
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
          count = 4;
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
      int fighters = 6 + GameScenarioManager.Instance().AllyFighters.Count;
      if (fighters > 12)
        fighters = 12;

      // spawner deployment logic
      if (ainfo.ActorState != ActorState.DEAD
          && ainfo.ActorState != ActorState.DYING
          && ainfo.IsStateFDefined("TIEspawnCooldown")
          && ainfo.GetStateF("TIEspawnCooldown") < Game.Instance().GameTime
          && GameScenarioManager.Instance().EnemyFighters.Count < fighters)
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

    public void Scene_02_Switch(object[] param)
    {
      GameScenarioManager.Instance().StageNumber = 2;
      GameScenarioManager.Instance().Line1Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
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

      GameScenarioManager.Instance().SceneCamera.MaxSpeed = 50;
      GameScenarioManager.Instance().SceneCamera.Speed = 50;
      GameScenarioManager.Instance().CameraTargetActor = m_Player;

      int counter = 0;
      int x = 0;
      int xn = -1;
      int y = 0;
      int z = -1400;

      foreach (ActorInfo ainfo in GameScenarioManager.Instance().EnemyFighters.Values)
      {
        ainfo.Destroy();
      }

      foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
      {
        x = xn * Engine.Instance().Random.Next(200, 600);
        xn *= -1;
        y = Engine.Instance().Random.Next(-200, 200);
        z += 200;
        ActionManager.ForceClearQueue(ainfo);
        ActionManager.QueueNext(ainfo, new Actions.Wait(8 + 0.2f * counter));
        ActionManager.QueueNext(ainfo, new Actions.HyperspaceOut());
        ainfo.SetLocalPosition(x, y, z);
        ainfo.SetRotation(0, 180, 0);
        ainfo.Speed = 450;
        ainfo.XTurnAngle = 0;
        ainfo.YTurnAngle = 0;
        ainfo.CanRetaliate = false;
        ainfo.CanEvade = false;
        counter++;
      }

      m_Transport1.DamageModifier = 0;
      m_Transport2.DamageModifier = 0;
      m_Transport3.DamageModifier = 0;

      m_Player.SetLocalPosition(0, 0, 500);

      ActionManager.ForceClearQueue(m_Transport1);
      ActionManager.QueueNext(m_Transport1, new Actions.Wait(8.5f));
      ActionManager.QueueNext(m_Transport1, new Actions.HyperspaceOut());
      m_Transport1.MaxSpeed = 400;
      m_Transport1.Speed = 400;

      ActionManager.ForceClearQueue(m_Transport2);
      ActionManager.QueueNext(m_Transport2, new Actions.Wait(8.8f));
      ActionManager.QueueNext(m_Transport2, new Actions.HyperspaceOut());
      m_Transport2.MaxSpeed = 400;
      m_Transport2.Speed = 400;

      ActionManager.ForceClearQueue(m_Transport3);
      ActionManager.QueueNext(m_Transport3, new Actions.Wait(9.1f));
      ActionManager.QueueNext(m_Transport3, new Actions.HyperspaceOut());
      m_Transport3.MaxSpeed = 400;
      m_Transport3.Speed = 400;

      m_Player.Speed = 400;
      m_Player.XTurnAngle = 0;
      m_Player.YTurnAngle = 0;
      ActionManager.ForceClearQueue(m_Player);
      ActionManager.QueueNext(m_Player, new Actions.Lock());
      

      int en_ship = 0;
      foreach (ActorInfo ainfo in GameScenarioManager.Instance().EnemyShips.Values)
      {
        if (en_ship == 1)
        {
          ainfo.SetLocalPosition(0, -300, 2500);
          ainfo.SetRotation(0, 180, 0);
          ainfo.Speed = ainfo.MaxSpeed;
          ActionManager.ForceClearQueue(ainfo);
          ActionManager.QueueNext(ainfo, new Actions.Wait(60));
          ActionManager.QueueNext(ainfo, new Actions.Rotate(new TV_3DVECTOR(-2000, -300, 2000), 0, -1, false));
          ActionManager.QueueNext(ainfo, new Actions.Lock());
        }
        else if (en_ship == 2)
        {
          ainfo.SetLocalPosition(3300, 150, 5500);
          ainfo.LookAtPoint(new TV_3DVECTOR(1400, 150, 1000));
          ainfo.Speed = ainfo.MaxSpeed;
          ActionManager.ForceClearQueue(ainfo);
          ActionManager.QueueNext(ainfo, new Actions.Wait(30));
          ActionManager.QueueNext(ainfo, new Actions.Rotate(new TV_3DVECTOR(-32000, 150, 2000), 0, -1, false));
          ActionManager.QueueNext(ainfo, new Actions.Lock());
        }
        else
        {
          ainfo.Destroy();
        }
        en_ship++;
      }

      SpawnActor(ImperialIATI.Instance()
               , ""
               , ""
               , ""
               , Game.Instance().GameTime + 10
               , FactionInfo.Get("Empire")
               , new TV_3DVECTOR(20000, -2000, -22000)
               , new TV_3DVECTOR()
               , new ActionInfo[] { new Actions.HyperspaceIn(new TV_3DVECTOR(2000, 100, -8000))
                                  , new Actions.Move(new TV_3DVECTOR(-2000, 100, 2000), ImperialIATI.Instance().MaxSpeed, -1, false)
                                  , new Actions.Rotate(new TV_3DVECTOR(2000, 100, -9000), 0, -1, false)
                                  , new Actions.Lock() }
               , new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().EnemyShips });

      SpawnActor(ImperialIATI.Instance()
               , ""
               , ""
               , ""
               , Game.Instance().GameTime + 10.5f
               , FactionInfo.Get("Empire")
               , new TV_3DVECTOR(20000, -2000, -25000)
               , new TV_3DVECTOR()
               , new ActionInfo[] { new Actions.HyperspaceIn(new TV_3DVECTOR(1500, -100, -10200))
                                  , new Actions.Move(new TV_3DVECTOR(-4500, -100, 4000), ImperialIATI.Instance().MaxSpeed, -1, false)
                                  , new Actions.Rotate(new TV_3DVECTOR(2000, -100, -10200), 0, -1, false)
                                  , new Actions.Lock() }
               , new Dictionary<string, ActorInfo>[] { GameScenarioManager.Instance().EnemyShips });
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
      PlayerInfo.Instance().shake_displacement = 75;
    }

    #endregion

    #region Text
    public void Message_01_Leaving(object[] param)
    {
      Screen2D.Instance().UpdateText("ECHO BASE: Escort the transports to the designated locations for hyperspace jump. ", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_Conditions(object[] param)
    {
      Screen2D.Instance().UpdateText("ECHO BASE: All transports must survive.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Turbolasers(object[] param)
    {
      Screen2D.Instance().UpdateText("TRANSPORT: The Heavy Turbolaser Towers on the Star Destroyers must be taken out.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_04_TIEs(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING: We will need to worry about the TIEs too.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_05(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: We will figure something out.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_06(object[] param)
    {
      Screen2D.Instance().UpdateText("ECHO BASE: Ion Control, standby.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07(object[] param)
    {
      Screen2D.Instance().UpdateText("ECHO BASE: Fire.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: Here's our opportunity.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_09(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING: We will take care of the fighters.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_10(object[] param)
    {
      Screen2D.Instance().UpdateText("X-WING: We need someone to take out the Heavy Turbolasers.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_11(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: I can take care of that.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_12(object[] param)
    {
      Screen2D.Instance().UpdateText("ECHO BASE: More Star Destroyers incoming.", Game.Instance().GameTime + 5f, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_13(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: I see them. Two Star Destroyers here coming right at us.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_14(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: [Use the secondary weapon toggle to switch between front and rear deflector shields.]", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_15(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: We can still out-manuever them.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_16(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: Prepare to make the jump to lightspeed.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_17(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: Watch this!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_18(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: ...", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_19(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: I think we are in trouble.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_20(object[] param)
    {
      Screen2D.Instance().UpdateText("C3PO: The hyperdrive modulator has been damaged, sir.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_21(object[] param)
    {
      Screen2D.Instance().UpdateText("C3PO: It is impossible to jump to lightspeed.", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_22(object[] param)
    {
      Screen2D.Instance().UpdateText("SOLO: We are in trouble!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_23(object[] param)
    {
      Screen2D.Instance().UpdateText("LEIA: Han, get up here!", Game.Instance().GameTime + 5f, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }
    #endregion
  }
}
