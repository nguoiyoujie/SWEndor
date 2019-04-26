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
      AllowedWings = new List<ActorTypeInfo> { this.GetEngine().ActorTypeFactory.Get("Millennium Falcon")
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
      Manager.Engine.PlayerInfo.Name = "Solo";
    }

    public override void Launch()
    {
      base.Launch();

      Manager.SceneCamera.SetLocalPosition(0, 0, 0);

      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 9000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -20000);
      Manager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -20000);

      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Rebel_HyperspaceIn);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Empire_FirstWave);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 5.5f, Rebel_MakePlayer);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 5.5f, Message_01_Leaving);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 9.5f, Rebel_GiveControl);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 10f, Message_02_Conditions);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 6.5f, Empire_FirstTIEWave);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 18, Message_03_Turbolasers);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 23, Message_04_TIEs);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 28, Message_05);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 45, Message_06);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 49, Message_07);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 52, Rebel_IonCannonSpawn);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 56, Message_08);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 61, Message_09);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 66, Message_10);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 71, Message_11);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 97, Message_12);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 100, Empire_SecondWave);

      Manager.Engine.PlayerInfo.Lives = 4;
      Manager.Engine.PlayerInfo.ScorePerLife = 1000000;
      Manager.Engine.PlayerInfo.ScoreForNextLife = 1000000;

      MakePlayer = Rebel_MakePlayer;

      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Manager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      //Manager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      Manager.Engine.SoundManager.SetMusic("battle_2_1");
      Manager.Engine.SoundManager.SetMusicLoop("battle_2_2");

      Manager.IsCutsceneMode = false;
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
        ActorCreationInfo aci_Hoth = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("Hoth"))
        {
          CreationTime = -1,
          Position = new TV_3DVECTOR(0, 800, -18000),
          Rotation = new TV_3DVECTOR(-90, 0, 0),
          InitialScale = new TV_3DVECTOR(6, 6, 6)
        };
        m_AHoth = ActorInfo.Create(this.GetEngine().ActorFactory, aci_Hoth);
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AHoth != null && m_AHoth.CreationState == CreationState.ACTIVE)
      {
        float x_y4 = Manager.Engine.PlayerInfo.Position.x / 5f;
        float y_y4 = Manager.Engine.PlayerInfo.Position.y / 2f;
        float z_y4 = (Manager.Engine.PlayerInfo.Position.z > 0) ? Manager.Engine.PlayerInfo.Position.z / 1.5f + 30000f : Manager.Engine.PlayerInfo.Position.z / 100f + 30000f;
        m_AHoth.SetLocalPosition(x_y4, y_y4, z_y4);
      }
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);
      ActorInfo trn1 = this.GetEngine().ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = this.GetEngine().ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = this.GetEngine().ActorFactory.Get(m_Transport3ID);
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
            && !Manager.GetGameStateB("Stage1End") 
            && !Manager.GetGameStateB("GameOver"))
          {
            Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_02);
            trn1.CombatInfo.DamageModifier = 0;
            trn2.CombatInfo.DamageModifier = 0;
            trn3.CombatInfo.DamageModifier = 0;
            Manager.SetGameStateB("Stage1End", true);
          }
        }
        else if (StageNumber == 2)
        {
          if (CurrentTIEs > MainEnemyFaction.Wings.Count)
          {
            TIEsLeft -= CurrentTIEs - MainEnemyFaction.Wings.Count;
          }
          CurrentTIEs = MainEnemyFaction.Wings.Count;

          if (TIEsLeft < 1 && !Manager.GetGameStateB("Stage2b"))
          {
            Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Manager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Manager.Line1Text = "Proceed to";
            Manager.Line2Text = "Hyperspace Lane";
            Manager.SetGameStateB("Stage2b", true);
            Manager.Engine.Screen2D.Box3D_Enable = true;
            Manager.Engine.Screen2D.Box3D_color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Manager.Engine.Screen2D.Box3D_min = hyperspace_lane_min;
            Manager.Engine.Screen2D.Box3D_max = hyperspace_lane_max;
          }

          if (TIEsLeft < 1 && !Manager.GetGameStateB("Stage2End") && !player.IsOutOfBounds(hyperspace_lane_min, hyperspace_lane_max))
          {
            Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_02b_LightspeedFail);
            Manager.SetGameStateB("Stage2End", true);
            Manager.Engine.Screen2D.Box3D_Enable = false;
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
        if (Manager.Scenario.TimeSinceLostWing < Manager.Engine.Game.GameTime || Manager.Engine.Game.GameTime % 0.2f > 0.1f)
          Manager.Line1Text = string.Format("WINGS: {0}", MainAllyFaction.GetWings().Count);
        else
          Manager.Line1Text = "";

        if (!Manager.GetGameStateB("Stage1End"))
          Manager.Line2Text = string.Format("DIST: {0:00000}", transport_currentZpos - transport_hyperspaceZpos);
        else
          Manager.Line2Text = "";
      }
      else if (StageNumber == 2)
      {
        if (TIEsLeft > 0)
        {
          Manager.Line1Text = "Destroy TIEs";
          Manager.Line2Text = string.Format("TIEs: {0}", TIEsLeft);
        }
        else if (!Manager.GetGameStateB("Stage2End"))
        {
        }
        else
        {
          Manager.Line1Text = "";
          Manager.Line2Text = "";
        }
      }
      else
      {
        Manager.Line2Text = "";
      }
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      Manager.SceneCamera.SetLocalPosition(200, 350, Manager.MaxBounds.z - 1500);

      // Player Falcon
      ainfo = new ActorSpawnInfo
      {
        Type = Manager.Engine.PlayerInfo.ActorType,
        Name = "(Player)",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
        Position = new TV_3DVECTOR(0, 200, Manager.MaxBounds.z - 450),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);

      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
      ainfo.CombatInfo.DamageModifier = 0.1f;
      ainfo.HitEvents += Rebel_PlayerHit;
      Manager.CameraTargetActor = ainfo;
      Manager.Engine.PlayerInfo.TempActorID = ainfo.ID;

      // X-Wings x12
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 11; i++)
      {
        if (i % 4 == 1)
          positions.Add(new TV_3DVECTOR(-16 * (i + 1) - 250, 10 * i, Manager.MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 2)
          positions.Add(new TV_3DVECTOR(10 * i, -16 * (i + 2) - 250, Manager.MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 3)
          positions.Add(new TV_3DVECTOR(-10 * i, 16 * (i + 2) + 250, Manager.MaxBounds.z - 1150 + 250 * i));
        else
          positions.Add(new TV_3DVECTOR(16 * (i + 2) + 250, 10 * i, Manager.MaxBounds.z - 1150 + 250 * i));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = this.GetEngine().ActorTypeFactory.Get("X-Wing"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Manager.Engine.Game.GameTime,
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
      positions.Add(new TV_3DVECTOR(250, -220, Manager.MaxBounds.z + 20));
      positions.Add(new TV_3DVECTOR(-100, -20, Manager.MaxBounds.z + 70));
      positions.Add(new TV_3DVECTOR(50, 270, Manager.MaxBounds.z + 1020));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = this.GetEngine().ActorTypeFactory.Get("Transport"),
          Name = "",
          RegisterName = "",
          SidebarName = "TRANSPORT",
          SpawnTime = Manager.Engine.Game.GameTime,
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
      TV_3DVECTOR position = new TV_3DVECTOR(0, 500, Manager.MaxBounds.z + 3000);
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      if (MainEnemyFaction.Ships.Count > 0)
      {
        ActorInfo target = this.GetEngine().ActorFactory.Get(MainEnemyFaction.Ships[0]);
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
        Type = this.GetEngine().ActorTypeFactory.Get("Large Ion Laser"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = this.GetEngine().Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = position,
        Rotation = rotation,
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);
    }

    public void Rebel_MakePlayer(object[] param)
    {
      Manager.Engine.PlayerInfo.ActorID = Manager.Engine.PlayerInfo.TempActorID;

      if (Manager.Engine.PlayerInfo.Actor == null || Manager.Engine.PlayerInfo.Actor.CreationState == CreationState.DISPOSED)
      {
        if (Manager.Engine.PlayerInfo.Lives > 0)
        {
          Manager.Engine.PlayerInfo.Lives--;

          TV_3DVECTOR position = new TV_3DVECTOR();
          if (StageNumber == 1)
          {
            if (Manager.CriticalAllies.Count > 0)
              position = new List<ActorInfo>(Manager.CriticalAllies.Values)[0].GetRelativePositionXYZ(0, -100, -1750);
            else
              position = new TV_3DVECTOR(0, 300, Manager.MaxBounds.z - 850);
          }
          else if (StageNumber == 2)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR();
            int count = 0;
            foreach (int enID in MainEnemyFaction.GetShips())
            {
              ActorInfo en = this.GetEngine().ActorFactory.Get(enID);
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
            Type = Manager.Engine.PlayerInfo.ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Manager.Engine.Game.GameTime,
            Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
            Position = position,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock() },
            Registries = null
          }.Spawn(this);

          ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
          ainfo.CombatInfo.DamageModifier = 0.1f;
          ainfo.HitEvents += Rebel_PlayerHit;

          Manager.Engine.PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_PlayerID = Manager.Engine.PlayerInfo.ActorID;
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

    public void Rebel_Delete(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
          if (!actor.IsPlayer())
            actor.Kill();
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
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
        Manager.Engine.PlayerInfo.FlashHit(Manager.Engine.PlayerInfo.HealthColor);
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

        if ((chgy < -90 || chgy > 90) && Manager.Engine.PlayerInfo.SecondaryWeapon != "rear")
        {
          player.CombatInfo.Strength -= 1;
          Manager.Engine.PlayerInfo.FlashHit(Manager.Engine.PlayerInfo.HealthColor);
        }
        else if ((chgy > -90 && chgy < 90) && Manager.Engine.PlayerInfo.SecondaryWeapon != "front")
        {
          player.CombatInfo.Strength -= 1;
          Manager.Engine.PlayerInfo.FlashHit(Manager.Engine.PlayerInfo.HealthColor);
        }
        else
        {

          Manager.Engine.PlayerCameraInfo.Shake = 2 * attacker.TypeInfo.ImpactDamage;
        }
      }
    }

    public void Rebel_CriticalUnitLost(object[] param)
    {
      if (param.GetLength(0) < 2 || param[0] == null || param[1] == null)
        return;

      if (Manager.GetGameStateB("GameWon"))
        return;

      if (Manager.GetGameStateB("GameOver"))
        return;

      ActorInfo ainfo = (ActorInfo)param[0];
      ActorState state = (ActorState)param[1];

      if (ainfo.ActorState == ActorState.DYING || ainfo.ActorState == ActorState.DEAD)
      {
        Manager.SetGameStateB("GameOver", true);
        Manager.IsCutsceneMode = true;

        if (Manager.SceneCamera == null || !(Manager.SceneCamera.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(this.GetEngine().ActorTypeFactory.Get("Death Camera"));
          camaci.CreationTime = Manager.Engine.Game.GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = ainfo.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(this.GetEngine().ActorFactory, camaci);
          Manager.Engine.PlayerInfo.ActorID = a.ID;
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
            //Manager.AddEvent(Manager.Engine.Game.GameTime + 15, "Message.92");
            ainfo.CombatInfo.TimedLife = 2000f;
            Manager.AddEvent(Manager.Engine.Game.GameTime + 25, FadeOut);
          }
        }
        else
        {
          Manager.SceneCamera.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
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
        SpawnTime = Manager.Engine.Game.GameTime,
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
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-8000, -350, 0), new TV_3DVECTOR(-2000, -350, 7000) });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          Empire_TIEWave_TIEsvsShips(new object[] { 2 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          break;
        case "normal":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500) });
          break;
        case "easy":
        default:
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000) });
          break;
      }
    }

    public void Empire_SecondWave(object[] param)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 5 });
          Empire_TIEWave_TIEsvsShips(new object[] { 3 });
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000) });
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 4 });
          Empire_TIEWave_TIEsvsShips(new object[] { 2 });
          break;
        case "easy":
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new object[] { this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"), new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000) });
          Empire_TIEWave(new object[] { 2 });
          Empire_TIEWave_TIEsvsShips(new object[] { 1 });
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

            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = this.GetEngine().ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime,
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
              case "hard":
                actions = new ActionInfo[] { new Wait(15), new Hunt(TargetType.FIGHTER) };
                break;
            }

            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = this.GetEngine().ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 5000),
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
        float fx = Manager.Engine.Random.Next(-2500, 2500);
        float fy = Manager.Engine.Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = this.GetEngine().ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Manager.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MinBounds.z - 2500),
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

    public void Scene_02_Switch(object[] param)
    {
      StageNumber = 2;
      Manager.Line1Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      Manager.Line2Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
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
      ActorInfo player = this.GetEngine().ActorFactory.Get(m_PlayerID);

      Manager.AddEvent(Manager.Engine.Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 7, Scene_ExitCutscene);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 9, Rebel_GiveControl);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 14, Rebel_Delete);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 11, Message_13);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 13, Scene_02_Switch);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 15, Message_14);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 25, Message_15);

      Manager.SceneCamera.SetLocalPosition(400, 130, -1800);

      Manager.SceneCamera.MovementInfo.MaxSpeed = 50;
      Manager.SceneCamera.MovementInfo.Speed = 50;
      Manager.CameraTargetActor = player;
      Manager.Engine.PlayerInfo.TempActorID = m_PlayerID;

      int counter = 0;
      int x = 0;
      int xn = -1;
      int y = 0;
      int z = -1400;

      foreach (int actorID in MainEnemyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
          actor.Kill();
      }

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
        {
          x = xn * Manager.Engine.Random.Next(200, 600);
          xn *= -1;
          y = Manager.Engine.Random.Next(-200, 200);
          z += 200;
          this.GetEngine().ActionManager.ForceClearQueue(actorID);
          this.GetEngine().ActionManager.QueueNext(actorID, new Wait(8 + 0.2f * counter));
          this.GetEngine().ActionManager.QueueNext(actorID, new HyperspaceOut());
          actor.SetLocalPosition(x, y, z);
          actor.SetLocalRotation(0, 180, 0);
          actor.MovementInfo.Speed = 450;
          actor.MovementInfo.ResetTurn();
          actor.CanRetaliate = false;
          actor.CanEvade = false;
          counter++;
        }
      }

      ActorInfo trn1 = this.GetEngine().ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = this.GetEngine().ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = this.GetEngine().ActorFactory.Get(m_Transport3ID);

      trn1.CombatInfo.DamageModifier = 0;
      trn2.CombatInfo.DamageModifier = 0;
      trn3.CombatInfo.DamageModifier = 0;

      player.SetLocalPosition(0, 0, 500);

      this.GetEngine().ActionManager.ForceClearQueue(m_Transport1ID);
      this.GetEngine().ActionManager.QueueNext(m_Transport1ID, new Wait(8.5f));
      this.GetEngine().ActionManager.QueueNext(m_Transport1ID, new HyperspaceOut());
      trn1.MovementInfo.MaxSpeed = 400;
      trn1.MovementInfo.Speed = 400;

      this.GetEngine().ActionManager.ForceClearQueue(m_Transport2ID);
      this.GetEngine().ActionManager.QueueNext(m_Transport2ID, new Wait(8.8f));
      this.GetEngine().ActionManager.QueueNext(m_Transport2ID, new HyperspaceOut());
      trn2.MovementInfo.MaxSpeed = 400;
      trn2.MovementInfo.Speed = 400;

      this.GetEngine().ActionManager.ForceClearQueue(m_Transport3ID);
      this.GetEngine().ActionManager.QueueNext(m_Transport3ID, new Wait(9.1f));
      this.GetEngine().ActionManager.QueueNext(m_Transport3ID, new HyperspaceOut());
      trn3.MovementInfo.MaxSpeed = 400;
      trn3.MovementInfo.Speed = 400;

      player.MovementInfo.Speed = 400;
      player.MovementInfo.ResetTurn(); 
      this.GetEngine().ActionManager.ForceClearQueue(m_PlayerID);
      this.GetEngine().ActionManager.QueueNext(m_PlayerID, new Lock());
      

      int en_ship = 0;
      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = this.GetEngine().ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (en_ship == 1)
          {
            actor.SetLocalPosition(0, -300, 2500);
            actor.SetLocalRotation(0, 180, 0);
            actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed * 0.25f;
            this.GetEngine().ActionManager.ForceClearQueue(actorID);
            this.GetEngine().ActionManager.QueueNext(actorID, new Wait(60));
            this.GetEngine().ActionManager.QueueNext(actorID, new Rotate(new TV_3DVECTOR(-2000, -300, 2000), 0, -1, false));
            this.GetEngine().ActionManager.QueueNext(actorID, new Lock());
          }
          else if (en_ship == 2)
          {
            actor.SetLocalPosition(3300, 150, 5500);
            actor.LookAtPoint(new TV_3DVECTOR(1400, 150, 1000));
            actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed * 0.25f;
            this.GetEngine().ActionManager.ForceClearQueue(actorID);
            this.GetEngine().ActionManager.QueueNext(actorID, new Wait(30));
            this.GetEngine().ActionManager.QueueNext(actorID, new Rotate(new TV_3DVECTOR(-32000, 150, 2000), 0, -1, false));
            this.GetEngine().ActionManager.QueueNext(actorID, new Lock());
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
        Type = this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Manager.Engine.Game.GameTime + 9,
        Faction = MainEnemyFaction,
        Position = new TV_3DVECTOR(20000, -2000, -22000),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(2000, 100, -8000))
                                    , new Move(new TV_3DVECTOR(1000, 100, 2000), this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed * 0.25f, -1, false)
                                    , new Rotate(new TV_3DVECTOR(2000, 100, -9000), 0, -1, false)
                                    , new Lock() },
        Registries = null
      };
      ActorInfo newDest = asi.Spawn(this);
      newDest.CombatInfo.DamageModifier = 0.1f;

      asi.SpawnTime = Manager.Engine.Game.GameTime + 9.25f;
      asi.Position = new TV_3DVECTOR(20000, -2000, -25000);
      asi.Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(1500, -100, -10200))
                                            , new Move(new TV_3DVECTOR(-6500, -100, 4000), this.GetEngine().ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed * 0.25f, -1, false)
                                            , new Rotate(new TV_3DVECTOR(2000, -100, -10200), 0, -1, false)
                                            , new Lock() };
      newDest = asi.Spawn(this);
      newDest.CombatInfo.DamageModifier = 0.1f;
    }

    public void Scene_02b_LightspeedFail(object[] param)
    {
      Manager.AddEvent(Manager.Engine.Game.GameTime + 1, Message_16);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 5, Message_17);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 12, Scene_02_ViolentShake);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 15, Message_18);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 18, Message_19);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 23, Message_20);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 28, Message_21);
      Manager.AddEvent(Manager.Engine.Game.GameTime + 33, Message_22);
      //Manager.AddEvent(Manager.Engine.Game.GameTime + 33, "Scene_03");

      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -30000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -30000);

      Manager.Engine.SoundManager.SetMusic("battle_2_3");
      Manager.Engine.SoundManager.SetMusicLoop("battle_2_3");
    }

    public void Scene_02_ViolentShake(object[] param)
    {
      Manager.Engine.PlayerCameraInfo.Shake = 75;
    }

    #endregion

    #region Text
    public void Message_01_Leaving(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("ECHO BASE: Escort the transports to the designated locations for hyperspace jump. ", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_Conditions(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("ECHO BASE: All transports must survive.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Turbolasers(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("TRANSPORT: The Heavy Turbolaser Towers on the Star Destroyers must be taken out.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_04_TIEs(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("X-WING: We will need to worry about the TIEs too.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_05(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: We will figure something out.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_06(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("ECHO BASE: Ion Control, standby.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("ECHO BASE: Fire.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: Here's our opportunity.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_09(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("X-WING: We will take care of the fighters.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_10(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("X-WING: We need someone to take out the Heavy Turbolasers.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_11(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: I can take care of that.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_12(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("ECHO BASE: More Star Destroyers incoming.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_13(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: I see them. Two Star Destroyers here coming right at us.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_14(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: [Use the secondary weapon toggle to switch between front and rear deflector shields.]", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_15(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: We can still out-manuever them.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_16(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: Prepare to make the jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_17(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: Watch this!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_18(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: ...", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_19(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: I think we are in trouble.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_20(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("C3PO: The hyperdrive modulator has been damaged, sir.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_21(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("C3PO: It is impossible to jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_22(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("SOLO: We are in trouble!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_23(object[] param)
    {
      Manager.Engine.Screen2D.MessageText("LEIA: Han, get up here!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }
    #endregion
  }
}
