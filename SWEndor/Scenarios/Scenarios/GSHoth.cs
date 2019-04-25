using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI.Actions;
using SWEndor.Player;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSHoth : GameScenarioBase
  {
    public GSHoth(GameScenarioManager manager) : base(manager)
    {
      Name = "Escape from Hoth (WIP up to Stage 02)";
      AllowedWings = new List<ActorTypeInfo> { Manager.Engine.ActorTypeFactory.Get("Millennium Falcon")
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
    private int m_Transport1ID = -1;
    private int m_Transport2ID = -1;
    private int m_Transport3ID = -1;

    private int m_PlayerID = -1;
    private float m_Player_DamageModifier = 1;
    private string m_Player_PrimaryWeapon = "";
    private string m_Player_SecondaryWeapon = "";

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      Globals.Engine.PlayerInfo.Name = "Solo";
    }

    public override void Launch()
    {
      base.Launch();

      Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(0, 0, 0);

      Globals.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(15000, 1500, 9000);
      Globals.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(-15000, -1500, -20000);
      Globals.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 8000);
      Globals.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -20000);

      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Rebel_HyperspaceIn);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Empire_FirstWave);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5.5f, Rebel_MakePlayer);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5.5f, Message_01_Leaving);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 9.5f, Rebel_GiveControl);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 10f, Message_02_Conditions);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 6.5f, Empire_FirstTIEWave);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 18, Message_03_Turbolasers);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 23, Message_04_TIEs);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 28, Message_05);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 45, Message_06);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 49, Message_07);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 52, Rebel_IonCannonSpawn);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 56, Message_08);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 61, Message_09);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 66, Message_10);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 71, Message_11);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 97, Message_12);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 100, Empire_SecondWave);

      Globals.Engine.PlayerInfo.Lives = 4;
      Globals.Engine.PlayerInfo.ScorePerLife = 1000000;
      Globals.Engine.PlayerInfo.ScoreForNextLife = 1000000;

      MakePlayer = Rebel_MakePlayer;

      Globals.Engine.GameScenarioManager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Globals.Engine.GameScenarioManager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      //Globals.Engine.GameScenarioManager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      Globals.Engine.SoundManager.SetMusic("battle_2_1");
      Globals.Engine.SoundManager.SetMusicLoop("battle_2_2");

      Globals.Engine.GameScenarioManager.IsCutsceneMode = false;
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
        ActorCreationInfo aci_Hoth = new ActorCreationInfo(Manager.Engine.ActorTypeFactory.Get("Hoth"))
        {
          CreationTime = -1,
          Position = new TV_3DVECTOR(0, 800, -18000),
          Rotation = new TV_3DVECTOR(-90, 0, 0),
          InitialScale = new TV_3DVECTOR(6, 6, 6)
        };
        m_AHoth = ActorInfo.Create(Manager.Engine.ActorFactory, aci_Hoth);
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AHoth != null && m_AHoth.CreationState == CreationState.ACTIVE)
      {
        float x_y4 = Globals.Engine.PlayerInfo.Position.x / 5f;
        float y_y4 = Globals.Engine.PlayerInfo.Position.y / 2f;
        float z_y4 = (Globals.Engine.PlayerInfo.Position.z > 0) ? Globals.Engine.PlayerInfo.Position.z / 1.5f + 30000f : Globals.Engine.PlayerInfo.Position.z / 100f + 30000f;
        m_AHoth.SetLocalPosition(x_y4, y_y4, z_y4);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      ActorInfo player = Manager.Engine.ActorFactory.Get(m_PlayerID);
      ActorInfo trn1 = Manager.Engine.ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = Manager.Engine.ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = Manager.Engine.ActorFactory.Get(m_Transport3ID);
      if (player != null 
        && player.ActorState != ActorState.DEAD 
        && player.ActorState != ActorState.DYING)
      {
        MainEnemyFaction.WingLimit = 6 + MainAllyFaction.GetWings().Count * 2;
        if (MainEnemyFaction.WingLimit > 12)
          MainEnemyFaction.WingLimit = 12;

        if (StageNumber == 0)
        {
          StageNumber = 1;
        }
        else if (StageNumber == 1 && trn1 != null)
        {
          if (transport_currentZpos > trn1.GetPosition().z)
          {
            transport_currentZpos = trn1.GetPosition().z;
          }
          if (trn1.GetPosition().z < transport_hyperspaceZpos 
            && player.ActorState == ActorState.NORMAL 
            && !Globals.Engine.GameScenarioManager.GetGameStateB("Stage1End") 
            && !Globals.Engine.GameScenarioManager.GetGameStateB("GameOver"))
          {
            Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Scene_02);
            trn1.CombatInfo.DamageModifier = 0;
            trn2.CombatInfo.DamageModifier = 0;
            trn3.CombatInfo.DamageModifier = 0;
            Globals.Engine.GameScenarioManager.SetGameStateB("Stage1End", true);
          }
        }
        else if (StageNumber == 2)
        {
          if (CurrentTIEs > MainEnemyFaction.Wings.Count)
          {
            TIEsLeft -= CurrentTIEs - MainEnemyFaction.Wings.Count;
          }
          CurrentTIEs = MainEnemyFaction.Wings.Count;

          if (TIEsLeft < 1 && !Globals.Engine.GameScenarioManager.GetGameStateB("Stage2b"))
          {
            Globals.Engine.GameScenarioManager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Globals.Engine.GameScenarioManager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Globals.Engine.GameScenarioManager.Line1Text = "Proceed to";
            Globals.Engine.GameScenarioManager.Line2Text = "Hyperspace Lane";
            Globals.Engine.GameScenarioManager.SetGameStateB("Stage2b", true);
            Globals.Engine.Screen2D.Box3D_Enable = true;
            Globals.Engine.Screen2D.Box3D_color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Globals.Engine.Screen2D.Box3D_min = hyperspace_lane_min;
            Globals.Engine.Screen2D.Box3D_max = hyperspace_lane_max;
          }

          if (TIEsLeft < 1 && !Globals.Engine.GameScenarioManager.GetGameStateB("Stage2End") && !player.IsOutOfBounds(hyperspace_lane_min, hyperspace_lane_max))
          {
            Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Scene_02b_LightspeedFail);
            Globals.Engine.GameScenarioManager.SetGameStateB("Stage2End", true);
            Globals.Engine.Screen2D.Box3D_Enable = false;
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
        if (Globals.Engine.GameScenarioManager.Scenario.TimeSinceLostWing < Globals.Engine.Game.GameTime || Globals.Engine.Game.GameTime % 0.2f > 0.1f)
          Globals.Engine.GameScenarioManager.Line1Text = string.Format("WINGS: {0}", MainAllyFaction.GetWings().Count);
        else
          Globals.Engine.GameScenarioManager.Line1Text = "";

        if (!Globals.Engine.GameScenarioManager.GetGameStateB("Stage1End"))
          Globals.Engine.GameScenarioManager.Line2Text = string.Format("DIST: {0:00000}", transport_currentZpos - transport_hyperspaceZpos);
        else
          Globals.Engine.GameScenarioManager.Line2Text = "";
      }
      else if (StageNumber == 2)
      {
        if (TIEsLeft > 0)
        {
          Globals.Engine.GameScenarioManager.Line1Text = "Destroy TIEs";
          Globals.Engine.GameScenarioManager.Line2Text = string.Format("TIEs: {0}", TIEsLeft);
        }
        else if (!Globals.Engine.GameScenarioManager.GetGameStateB("Stage2End"))
        {
        }
        else
        {
          Globals.Engine.GameScenarioManager.Line1Text = "";
          Globals.Engine.GameScenarioManager.Line2Text = "";
        }
      }
      else
      {
        Globals.Engine.GameScenarioManager.Line2Text = "";
      }
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(200, 350, Globals.Engine.GameScenarioManager.MaxBounds.z - 1500);

      // Player Falcon
      ainfo = new ActorSpawnInfo
      {
        Type = Globals.Engine.PlayerInfo.ActorType,
        Name = "(Player)",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Globals.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
        Position = new TV_3DVECTOR(0, 200, Globals.Engine.GameScenarioManager.MaxBounds.z - 450),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);

      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
      ainfo.CombatInfo.DamageModifier = 0.1f;
      ainfo.HitEvents += Rebel_PlayerHit;
      Globals.Engine.GameScenarioManager.CameraTargetActor = ainfo;
      Globals.Engine.PlayerInfo.TempActorID = ainfo.ID;

      // X-Wings x12
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 11; i++)
      {
        if (i % 4 == 1)
          positions.Add(new TV_3DVECTOR(-16 * (i + 1) - 250, 10 * i, Globals.Engine.GameScenarioManager.MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 2)
          positions.Add(new TV_3DVECTOR(10 * i, -16 * (i + 2) - 250, Globals.Engine.GameScenarioManager.MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 3)
          positions.Add(new TV_3DVECTOR(-10 * i, 16 * (i + 2) + 250, Globals.Engine.GameScenarioManager.MaxBounds.z - 1150 + 250 * i));
        else
          positions.Add(new TV_3DVECTOR(16 * (i + 2) + 250, 10 * i, Globals.Engine.GameScenarioManager.MaxBounds.z - 1150 + 250 * i));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("X-Wing"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Globals.Engine.Game.GameTime,
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
      positions.Add(new TV_3DVECTOR(250, -220, Globals.Engine.GameScenarioManager.MaxBounds.z + 20));
      positions.Add(new TV_3DVECTOR(-100, -20, Globals.Engine.GameScenarioManager.MaxBounds.z + 70));
      positions.Add(new TV_3DVECTOR(50, 270, Globals.Engine.GameScenarioManager.MaxBounds.z + 1020));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = Manager.Engine.ActorTypeFactory.Get("Transport"),
          Name = "",
          RegisterName = "",
          SidebarName = "TRANSPORT",
          SpawnTime = Globals.Engine.Game.GameTime,
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
            m_Transport1ID = ainfo.ID;
            break;
          case 1:
            m_Transport2ID = ainfo.ID;
            break;
          case 2:
            m_Transport3ID = ainfo.ID;
            break;
        }

        ainfo.HitEvents += Rebel_VulnerableTransport;
        ainfo.ActorStateChangeEvents += Rebel_CriticalUnitLost;
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
      TV_3DVECTOR position = new TV_3DVECTOR(0, 500, Globals.Engine.GameScenarioManager.MaxBounds.z + 3000);
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      if (MainEnemyFaction.Ships.Count > 0)
      {
        ActorInfo target = Manager.Engine.ActorFactory.Get(MainEnemyFaction.Ships[0]);
        if (target != null)
        {
          TV_3DVECTOR dir = target.GetPosition() - position;
          rotation = Utilities.GetRotation(dir);
        }
      }
      else
      {
        rotation = new TV_3DVECTOR(0, 180, 0);
      }

      new ActorSpawnInfo
      {
        Type = Manager.Engine.ActorTypeFactory.Get("Large Ion Laser"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = position,
        Rotation = rotation,
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);
    }

    public void Rebel_MakePlayer(object[] param)
    {
      Globals.Engine.PlayerInfo.ActorID = Globals.Engine.PlayerInfo.TempActorID;

      if (Globals.Engine.PlayerInfo.Actor == null || Globals.Engine.PlayerInfo.Actor.CreationState == CreationState.DISPOSED)
      {
        if (Globals.Engine.PlayerInfo.Lives > 0)
        {
          Globals.Engine.PlayerInfo.Lives--;

          TV_3DVECTOR position = new TV_3DVECTOR();
          if (StageNumber == 1)
          {
            if (Globals.Engine.GameScenarioManager.CriticalAllies.Count > 0)
              position = new List<ActorInfo>(Globals.Engine.GameScenarioManager.CriticalAllies.Values)[0].GetRelativePositionXYZ(0, -100, -1750);
            else
              position = new TV_3DVECTOR(0, 300, Globals.Engine.GameScenarioManager.MaxBounds.z - 850);
          }
          else if (StageNumber == 2)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR();
            int count = 0;
            foreach (int enID in MainEnemyFaction.GetShips())
            {
              ActorInfo en = Manager.Engine.ActorFactory.Get(enID);
              if (en != null)
              {
                pos += en.Position;
                count++;
              }
            }
            if (count > 0)
              pos /= count;

            position = pos;
          }

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = Globals.Engine.PlayerInfo.ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Globals.Engine.Game.GameTime,
            Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
            Position = position,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock() },
            Registries = null
          }.Spawn(this);

          ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
          ainfo.CombatInfo.DamageModifier = 0.1f;
          ainfo.HitEvents += Rebel_PlayerHit;

          Globals.Engine.PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_PlayerID = Globals.Engine.PlayerInfo.ActorID;
    }

    public void Rebel_GiveControl(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          Manager.Engine.ActionManager.UnlockOne(actorID);
          actor.ActorState = ActorState.NORMAL;
          actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
        }
      }
      Globals.Engine.PlayerInfo.IsMovementControlsEnabled = true;

      Globals.Engine.GameScenarioManager.SetGameStateB("in_battle", true);
    }

    public void Rebel_Delete(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
          if (!actor.IsPlayer())
            actor.Kill();
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
          if (!actor.IsPlayer())
            actor.Kill();
      }
    }

    public void Rebel_PlayerHit(object[] param)
    {
      ActorInfo player = (ActorInfo)param[0];
      ActorInfo attacker = (ActorInfo)param[1];

      if (!attacker.TypeInfo.IsDamage) 
      {
        player.CombatInfo.Strength -= attacker.TypeInfo.ImpactDamage;
        Globals.Engine.PlayerInfo.FlashHit(Globals.Engine.PlayerInfo.HealthColor);
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

        if ((chgy < -90 || chgy > 90) && Globals.Engine.PlayerInfo.SecondaryWeapon != "rear")
        {
          player.CombatInfo.Strength -= 1;
          Globals.Engine.PlayerInfo.FlashHit(Globals.Engine.PlayerInfo.HealthColor);
        }
        else if ((chgy > -90 && chgy < 90) && Globals.Engine.PlayerInfo.SecondaryWeapon != "front")
        {
          player.CombatInfo.Strength -= 1;
          Globals.Engine.PlayerInfo.FlashHit(Globals.Engine.PlayerInfo.HealthColor);
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

      if (Globals.Engine.GameScenarioManager.GetGameStateB("GameWon"))
        return;

      if (Globals.Engine.GameScenarioManager.GetGameStateB("GameOver"))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];
      ActorState state = (ActorState)param[1];

      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        Globals.Engine.GameScenarioManager.SetGameStateB("GameOver", true);
        Globals.Engine.GameScenarioManager.IsCutsceneMode = true;

        if (Globals.Engine.GameScenarioManager.SceneCamera == null || !(Globals.Engine.GameScenarioManager.SceneCamera.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(Manager.Engine.ActorTypeFactory.Get("Death Camera"));
          camaci.CreationTime = Globals.Engine.Game.GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = ainfo.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(Manager.Engine.ActorFactory, camaci);
          Globals.Engine.PlayerInfo.ActorID = a.ID;
          a.CombatInfo.Strength = 0;

          a.CameraSystemInfo.CamDeathCirclePeriod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
          a.CameraSystemInfo.CamDeathCircleRadius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
          a.CameraSystemInfo.CamDeathCircleHeight = ainfo.CameraSystemInfo.CamDeathCircleHeight;

          if (ainfo.ActorState == ActorState.DYING)
          {
            ainfo.TickEvents += ProcessPlayerDying;
            ainfo.DestroyedEvents += ProcessPlayerKilled;
          }
          else
          {
            ainfo.DestroyedEvents += ProcessPlayerKilled;
          }

          if (ainfo.TypeInfo is TransportATI)
          {
            //Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15, "Message.92");
            ainfo.CombatInfo.TimedLife = 2000f;
            Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 25, FadeOut);
          }
        }
        else
        {
          Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
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
        SpawnTime = Globals.Engine.Game.GameTime,
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
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-8000, -350, 0), new TV_3DVECTOR(-2000, -350, 7000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          Empire_TIEWave_TIEsvsShips(new object[] { 2 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          break;
        case "normal":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          break;
        case "easy":
        default:
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          break;
      }
    }

    public void Empire_SecondWave(object[] param)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 5 });
          Empire_TIEWave_TIEsvsShips(new object[] { 3 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 4 });
          Empire_TIEWave_TIEsvsShips(new object[] { 2 });
          break;
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 2 });
          Empire_TIEWave_TIEsvsShips(new object[] { 1 });
          break;
      }
      Globals.Engine.GameScenarioManager.SetGameStateB("Stage1BBegin", true);
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
        float fx = Globals.Engine.Random.Next(-500, 500);
        float fy = Globals.Engine.Random.Next(-500, 0);
        float fz = Globals.Engine.Random.Next(-2500, 2500);

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
              Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime,
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-200, 800);

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
              Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MinBounds.z - 5000),
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = Manager.Engine.ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MinBounds.z - 2500),
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
      ActorInfo player = Manager.Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_PrimaryWeapon = Globals.Engine.PlayerInfo.PrimaryWeapon;
        m_Player_SecondaryWeapon = Globals.Engine.PlayerInfo.SecondaryWeapon;
        m_Player_DamageModifier = player.CombatInfo.DamageModifier;
        player.CombatInfo.DamageModifier = 0;
        Manager.Engine.ActionManager.ForceClearQueue(m_PlayerID);
        Manager.Engine.ActionManager.QueueNext(m_PlayerID, new Lock());
      }
      Globals.Engine.PlayerInfo.ActorID = Globals.Engine.GameScenarioManager.SceneCamera.ID;

      Globals.Engine.GameScenarioManager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene(object[] param)
    {
      ActorInfo player = Manager.Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        Globals.Engine.PlayerInfo.ActorID = m_PlayerID;
        Globals.Engine.PlayerInfo.PrimaryWeapon = m_Player_PrimaryWeapon;
        Globals.Engine.PlayerInfo.SecondaryWeapon = m_Player_SecondaryWeapon;
        player.CombatInfo.DamageModifier = m_Player_DamageModifier;
        Manager.Engine.ActionManager.ForceClearQueue(m_PlayerID);
      }
      Globals.Engine.GameScenarioManager.IsCutsceneMode = false;
    }

    public void Scene_02_Switch(object[] param)
    {
      StageNumber = 2;
      Globals.Engine.GameScenarioManager.Line1Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      Globals.Engine.GameScenarioManager.Line2Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
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
      ActorInfo player = Manager.Engine.ActorFactory.Get(m_PlayerID);

      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 7, Scene_ExitCutscene);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 9, Rebel_GiveControl);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 14, Rebel_Delete);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 11, Message_13);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 13, Scene_02_Switch);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15, Message_14);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 25, Message_15);

      Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(400, 130, -1800);

      Globals.Engine.GameScenarioManager.SceneCamera.MovementInfo.MaxSpeed = 50;
      Globals.Engine.GameScenarioManager.SceneCamera.MovementInfo.Speed = 50;
      Globals.Engine.GameScenarioManager.CameraTargetActor = player;
      Globals.Engine.PlayerInfo.TempActorID = m_PlayerID;

      int counter = 0;
      int x = 0;
      int xn = -1;
      int y = 0;
      int z = -1400;

      foreach (int actorID in MainEnemyFaction.GetWings())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
          actor.Kill();
      }

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          x = xn * Globals.Engine.Random.Next(200, 600);
          xn *= -1;
          y = Globals.Engine.Random.Next(-200, 200);
          z += 200;
          Manager.Engine.ActionManager.ForceClearQueue(actorID);
          Manager.Engine.ActionManager.QueueNext(actorID, new Wait(8 + 0.2f * counter));
          Manager.Engine.ActionManager.QueueNext(actorID, new HyperspaceOut());
          actor.SetLocalPosition(x, y, z);
          actor.SetLocalRotation(0, 180, 0);
          actor.MovementInfo.Speed = 450;
          actor.MovementInfo.ResetTurn();
          actor.CanRetaliate = false;
          actor.CanEvade = false;
          counter++;
        }
      }

      ActorInfo trn1 = Manager.Engine.ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = Manager.Engine.ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = Manager.Engine.ActorFactory.Get(m_Transport3ID);

      trn1.CombatInfo.DamageModifier = 0;
      trn2.CombatInfo.DamageModifier = 0;
      trn3.CombatInfo.DamageModifier = 0;

      player.SetLocalPosition(0, 0, 500);

      Manager.Engine.ActionManager.ForceClearQueue(m_Transport1ID);
      Manager.Engine.ActionManager.QueueNext(m_Transport1ID, new Wait(8.5f));
      Manager.Engine.ActionManager.QueueNext(m_Transport1ID, new HyperspaceOut());
      trn1.MovementInfo.MaxSpeed = 400;
      trn1.MovementInfo.Speed = 400;

      Manager.Engine.ActionManager.ForceClearQueue(m_Transport2ID);
      Manager.Engine.ActionManager.QueueNext(m_Transport2ID, new Wait(8.8f));
      Manager.Engine.ActionManager.QueueNext(m_Transport2ID, new HyperspaceOut());
      trn2.MovementInfo.MaxSpeed = 400;
      trn2.MovementInfo.Speed = 400;

      Manager.Engine.ActionManager.ForceClearQueue(m_Transport3ID);
      Manager.Engine.ActionManager.QueueNext(m_Transport3ID, new Wait(9.1f));
      Manager.Engine.ActionManager.QueueNext(m_Transport3ID, new HyperspaceOut());
      trn3.MovementInfo.MaxSpeed = 400;
      trn3.MovementInfo.Speed = 400;

      player.MovementInfo.Speed = 400;
      player.MovementInfo.ResetTurn(); 
      Manager.Engine.ActionManager.ForceClearQueue(m_PlayerID);
      Manager.Engine.ActionManager.QueueNext(m_PlayerID, new Lock());
      

      int en_ship = 0;
      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = Manager.Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (en_ship == 1)
          {
            actor.SetLocalPosition(0, -300, 2500);
            actor.SetLocalRotation(0, 180, 0);
            actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed * 0.25f;
            Manager.Engine.ActionManager.ForceClearQueue(actorID);
            Manager.Engine.ActionManager.QueueNext(actorID, new Wait(60));
            Manager.Engine.ActionManager.QueueNext(actorID, new Rotate(new TV_3DVECTOR(-2000, -300, 2000), 0, -1, false));
            Manager.Engine.ActionManager.QueueNext(actorID, new Lock());
          }
          else if (en_ship == 2)
          {
            actor.SetLocalPosition(3300, 150, 5500);
            actor.LookAtPoint(new TV_3DVECTOR(1400, 150, 1000));
            actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed * 0.25f;
            Manager.Engine.ActionManager.ForceClearQueue(actorID);
            Manager.Engine.ActionManager.QueueNext(actorID, new Wait(30));
            Manager.Engine.ActionManager.QueueNext(actorID, new Rotate(new TV_3DVECTOR(-32000, 150, 2000), 0, -1, false));
            Manager.Engine.ActionManager.QueueNext(actorID, new Lock());
          }
          else
          {
            actor.Kill();
          }
        }
        en_ship++;
      }

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Globals.Engine.Game.GameTime + 9,
        Faction = MainEnemyFaction,
        Position = new TV_3DVECTOR(20000, -2000, -22000),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(2000, 100, -8000))
                                    , new Move(new TV_3DVECTOR(1000, 100, 2000), Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed * 0.25f, -1, false)
                                    , new Rotate(new TV_3DVECTOR(2000, 100, -9000), 0, -1, false)
                                    , new Lock() },
        Registries = null
      };
      ActorInfo newDest = asi.Spawn(this);
      newDest.CombatInfo.DamageModifier = 0.1f;

      asi.SpawnTime = Globals.Engine.Game.GameTime + 9.25f;
      asi.Position = new TV_3DVECTOR(20000, -2000, -25000);
      asi.Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(1500, -100, -10200))
                                            , new Move(new TV_3DVECTOR(-6500, -100, 4000), Manager.Engine.ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed * 0.25f, -1, false)
                                            , new Rotate(new TV_3DVECTOR(2000, -100, -10200), 0, -1, false)
                                            , new Lock() };
      newDest = asi.Spawn(this);
      newDest.CombatInfo.DamageModifier = 0.1f;
    }

    public void Scene_02b_LightspeedFail(object[] param)
    {
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 1, Message_16);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5, Message_17);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 12, Scene_02_ViolentShake);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 15, Message_18);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 18, Message_19);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 23, Message_20);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 28, Message_21);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 33, Message_22);
      //Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 33, "Scene_03");

      Globals.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(-15000, -1500, -30000);
      Globals.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -30000);

      Globals.Engine.SoundManager.SetMusic("battle_2_3");
      Globals.Engine.SoundManager.SetMusicLoop("battle_2_3");
    }

    public void Scene_02_ViolentShake(object[] param)
    {
      PlayerCameraInfo.Instance().Shake = 75;
    }

    #endregion

    #region Text
    public void Message_01_Leaving(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("ECHO BASE: Escort the transports to the designated locations for hyperspace jump. ", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_Conditions(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("ECHO BASE: All transports must survive.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Turbolasers(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("TRANSPORT: The Heavy Turbolaser Towers on the Star Destroyers must be taken out.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_04_TIEs(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING: We will need to worry about the TIEs too.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_05(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: We will figure something out.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_06(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("ECHO BASE: Ion Control, standby.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("ECHO BASE: Fire.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: Here's our opportunity.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_09(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING: We will take care of the fighters.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_10(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("X-WING: We need someone to take out the Heavy Turbolasers.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_11(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: I can take care of that.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_12(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("ECHO BASE: More Star Destroyers incoming.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_13(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: I see them. Two Star Destroyers here coming right at us.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_14(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: [Use the secondary weapon toggle to switch between front and rear deflector shields.]", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_15(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: We can still out-manuever them.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_16(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: Prepare to make the jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_17(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: Watch this!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_18(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: ...", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_19(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: I think we are in trouble.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_20(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("C3PO: The hyperdrive modulator has been damaged, sir.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_21(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("C3PO: It is impossible to jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_22(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("SOLO: We are in trouble!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_23(object[] param)
    {
      Globals.Engine.Screen2D.MessageText("LEIA: Han, get up here!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }
    #endregion
  }
}
