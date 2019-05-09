using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Components;
using SWEndor.Actors.Data;
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
      AllowedWings = new List<ActorTypeInfo> { ActorTypeFactory.Get("Millennium Falcon")
                                              };

      AllowedDifficulties = new List<string> { "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private List<ShipSpawnEventArg> m_pendingSDspawnlist;
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
      PlayerInfo.Name = "Solo";

      m_pendingSDspawnlist = new List<ShipSpawnEventArg>();
    }

    public override void Unload()
    {
      base.Unload();

      m_pendingSDspawnlist = null;
    }

    public override void Launch()
    {
      base.Launch();

      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      cam.SetLocalPosition(0, 0, 0);

      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 9000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -20000);
      Manager.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 8000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -20000);

      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_HyperspaceIn);
      Manager.AddEvent(Game.GameTime + 0.1f, Empire_FirstWave);
      Manager.AddEvent(Game.GameTime + 5.5f, Rebel_MakePlayer);
      Manager.AddEvent(Game.GameTime + 5.5f, Message_01_Leaving);
      Manager.AddEvent(Game.GameTime + 9.5f, Rebel_GiveControl);
      Manager.AddEvent(Game.GameTime + 10f, Message_02_Conditions);
      Manager.AddEvent(Game.GameTime + 6.5f, Empire_FirstTIEWave);
      Manager.AddEvent(Game.GameTime + 18, Message_03_Turbolasers);
      Manager.AddEvent(Game.GameTime + 23, Message_04_TIEs);
      Manager.AddEvent(Game.GameTime + 28, Message_05);
      Manager.AddEvent(Game.GameTime + 45, Message_06);
      Manager.AddEvent(Game.GameTime + 49, Message_07);
      Manager.AddEvent(Game.GameTime + 52, Rebel_IonCannonSpawn);
      Manager.AddEvent(Game.GameTime + 56, Message_08);
      Manager.AddEvent(Game.GameTime + 61, Message_09);
      Manager.AddEvent(Game.GameTime + 66, Message_10);
      Manager.AddEvent(Game.GameTime + 71, Message_11);
      Manager.AddEvent(Game.GameTime + 97, Message_12);
      Manager.AddEvent(Game.GameTime + 100, Empire_SecondWave);

      PlayerInfo.Lives = 4;
      PlayerInfo.ScorePerLife = 1000000;
      PlayerInfo.ScoreForNextLife = 1000000;

      MakePlayer = Rebel_MakePlayer;

      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Manager.Line2Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      //Manager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      SoundManager.SetMusic("battle_2_1");
      SoundManager.SetMusicLoop("battle_2_2");

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
      ActorCreationInfo aci_Hoth = new ActorCreationInfo(ActorTypeFactory.Get("Hoth"))
      {
        CreationTime = -1,
        Position = new TV_3DVECTOR(0, 0, 36000),
        Rotation = new TV_3DVECTOR(-90, 0, 0),
        InitialScale =12
      };
      ActorInfo.Create(ActorFactory, aci_Hoth);
    }

    public override void GameTick()
    {
      base.GameTick();

      ActorInfo player = ActorFactory.Get(m_PlayerID);
      ActorInfo trn1 = ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = ActorFactory.Get(m_Transport3ID);
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
            Manager.AddEvent(Game.GameTime + 0.1f, Scene_02);
            Engine.ActorDataSet.CombatData[trn1.dataID].DamageModifier = 0;
            Engine.ActorDataSet.CombatData[trn2.dataID].DamageModifier = 0;
            Engine.ActorDataSet.CombatData[trn3.dataID].DamageModifier = 0;
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
            Screen2D.Box3D_Enable = true;
            Screen2D.Box3D_color = new TV_COLOR(1f, 1f, 0.3f, 1);
            Screen2D.Box3D_min = hyperspace_lane_min;
            Screen2D.Box3D_max = hyperspace_lane_max;
          }

          if (TIEsLeft < 1 && !Manager.GetGameStateB("Stage2End") && !player.IsOutOfBounds(hyperspace_lane_min, hyperspace_lane_max))
          {
            Manager.AddEvent(Game.GameTime + 0.1f, Scene_02b_LightspeedFail);
            Manager.SetGameStateB("Stage2End", true);
            Screen2D.Box3D_Enable = false;
          }
        }
      }

      if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.GetShips().Count < 7)
      {
        Manager.AddEvent(0, Empire_StarDestroyer_Spawn, m_pendingSDspawnlist[0]);
        m_pendingSDspawnlist.RemoveAt(0);
      }

      if (StageNumber == 1)
      {
        if (Manager.Scenario.TimeSinceLostWing < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
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

    public void Rebel_HyperspaceIn(GameEventArg arg)
    {
      ActorInfo ainfo;
      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      cam.SetLocalPosition(200, 350, Manager.MaxBounds.z - 1500);

      // Player Falcon
      ainfo = new ActorSpawnInfo
      {
        Type = PlayerInfo.ActorType,
        Name = "(Player)",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
        Position = new TV_3DVECTOR(0, 200, Manager.MaxBounds.z - 450),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);

      ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
      Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.1f;
      ainfo.HitEvents += Rebel_PlayerHit;
      Manager.CameraTargetActorID = ainfo.ID;
      PlayerInfo.TempActorID = ainfo.ID;

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
          Type = ActorTypeFactory.Get("X-Wing"),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Lock(), new Wait(3) },
          Registries = null
        }.Spawn(this);

        Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.85f;
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
          Type = ActorTypeFactory.Get("Transport"),
          Name = "",
          RegisterName = "",
          SidebarName = "TRANSPORT",
          SpawnTime = Game.GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Lock(), new Wait(3) },
          Registries = new string[] { "CriticalAllies" }
        }.Spawn(this);

        Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.6f;

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

        ainfo.ActorStateChangeEvents += Rebel_CriticalUnitLost;
      }
    }

    public void Rebel_IonCannonSpawn(GameEventArg arg)
    {
      TV_3DVECTOR position = new TV_3DVECTOR(0, 500, Manager.MaxBounds.z + 3000);
      TV_3DVECTOR rotation = new TV_3DVECTOR();
      if (MainEnemyFaction.Ships.Count > 0)
      {
        ActorInfo target = ActorFactory.Get(MainEnemyFaction.Ships[0]);
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
        Type = ActorTypeFactory.Get("Large Ion Laser"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = position,
        Rotation = rotation,
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);
    }

    public void Rebel_MakePlayer(GameEventArg arg)
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;

      if (PlayerInfo.Actor == null || PlayerInfo.Actor.CreationState == CreationState.DISPOSED)
      {
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;

          TV_3DVECTOR position = new TV_3DVECTOR();
          if (StageNumber == 1)
          {
            if (Manager.CriticalAllies.Count > 0)
            {
              ActorInfo crit = ActorFactory.Get(new List<int>(Manager.CriticalAllies.Values)[0]);
              position = crit.GetRelativePositionXYZ(0, -100, -1750);
            }
            else
              position = new TV_3DVECTOR(0, 300, Manager.MaxBounds.z - 850);
          }
          else if (StageNumber == 2)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR();
            int count = 0;
            foreach (int enID in MainEnemyFaction.GetShips())
            {
              ActorInfo en = ActorFactory.Get(enID);
              if (en != null)
              {
                pos += en.CoordData.Position;
                count++;
              }
            }
            if (count > 0)
              pos /= count;

            position = pos;
          }

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = PlayerInfo.ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
            Position = position,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock() },
            Registries = null
          }.Spawn(this);

          ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
          Engine.ActorDataSet.CombatData[ainfo.dataID].DamageModifier = 0.1f;
          ainfo.HitEvents += Rebel_PlayerHit;

          PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_PlayerID = PlayerInfo.ActorID;
    }

    public void Rebel_GiveControl(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          ActionManager.UnlockOne(actorID);
          actor.ActorState = ActorState.NORMAL;
          actor.MoveData.Speed = actor.MoveData.MaxSpeed;
        }
      }
      PlayerInfo.IsMovementControlsEnabled = true;

      Manager.SetGameStateB("in_battle", true);
    }

    public void Rebel_Delete(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        if (!ActorInfo.IsPlayer(Engine, actorID))
          ActorInfo.Kill(Engine, actorID);
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        if (!ActorInfo.IsPlayer(Engine, actorID))
          ActorInfo.Kill(Engine, actorID);
      }
    }

    public void Rebel_PlayerHit(GameEventArg arg)
    {
      if (arg is HitEventArg)
      {
        ActorInfo player = ActorFactory.Get(((HitEventArg)arg).VictimID);
        ActorInfo attacker = ActorFactory.Get(((HitEventArg)arg).ActorID);

        if (!Engine.MaskDataSet[attacker.ID].Has(ComponentMask.IS_DAMAGE))
        {
          CombatSystem.onNotify(Engine, player.ID, Actors.Components.CombatEventType.DAMAGE, attacker.TypeInfo.ImpactDamage);
          PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
        }
        else
        {
          TV_3DVECTOR rot = player.GetRotation();
          TV_3DVECTOR tgtrot = Utilities.GetRotation(attacker.GetRelativePositionFUR(-1000, 0, 0) - player.CoordData.PrevPosition);

          float chgy = tgtrot.y - rot.y;

          while (chgy < -180)
            chgy += 360;

          while (chgy > 180)
            chgy -= 360;

          if ((chgy < -90 || chgy > 90) && PlayerInfo.SecondaryWeapon != "rear")
          {
            CombatSystem.onNotify(Engine, player.ID, Actors.Components.CombatEventType.DAMAGE, 1);
            PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
          }
          else if ((chgy > -90 && chgy < 90) && PlayerInfo.SecondaryWeapon != "front")
          {
            CombatSystem.onNotify(Engine, player.ID, Actors.Components.CombatEventType.DAMAGE, 1);
            PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
          }
          else
          {
            PlayerCameraInfo.Shake = 2 * attacker.TypeInfo.ImpactDamage;
          }
        }
      }
    }

    public void Rebel_CriticalUnitLost(ActorStateChangeEventArg arg)
    {
        if (Manager.GetGameStateB("GameWon"))
          return;

        if (Manager.GetGameStateB("GameOver"))
          return;

        ActorInfo ainfo = ActorFactory.Get(((ActorStateChangeEventArg)arg).ActorID);

      if (ainfo.ActorState.IsDyingOrDead())
      {
        Manager.SetGameStateB("GameOver", true);
        Manager.IsCutsceneMode = true;

        ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
        if (cam == null || !(cam.TypeInfo is DeathCameraATI))
        {
          ActorCreationInfo camaci = new ActorCreationInfo(ActorTypeFactory.Get("Death Camera"));
          camaci.CreationTime = Game.GameTime;
          camaci.InitialState = ActorState.DYING;
          camaci.Position = ainfo.GetPosition();
          camaci.Rotation = new TV_3DVECTOR();

          ActorInfo a = ActorInfo.Create(ActorFactory, camaci);
          PlayerInfo.ActorID = a.ID;

          a.CameraSystemInfo.CamDeathCirclePeriod = ainfo.CameraSystemInfo.CamDeathCirclePeriod;
          a.CameraSystemInfo.CamDeathCircleRadius = ainfo.CameraSystemInfo.CamDeathCircleRadius;
          a.CameraSystemInfo.CamDeathCircleHeight = ainfo.CameraSystemInfo.CamDeathCircleHeight;

          if (ainfo.ActorState.IsDying())
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
            //Manager.AddEvent(Game.GameTime + 15, "Message.92");
            TimedLifeSystem.Activate(Engine, ainfo.ID, 2000f);
            Manager.AddEvent(Game.GameTime + 25, FadeOut);
          }
        }
        else
        {
          cam.SetLocalPosition(ainfo.GetPosition().x, ainfo.GetPosition().y, ainfo.GetPosition().z);
        }
      }
    }

    #endregion

    #region Empire spawns

    public void Empire_StarDestroyer_Spawn(GameEventArg arg)
    {
      if (arg is ShipSpawnEventArg)
      {
        ShipSpawnEventArg s = (ShipSpawnEventArg)arg;
        ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);

        ship.HuntWeight = 1;
        Engine.ActorDataSet.CombatData[ship.dataID].DamageModifier = 0.1f;
      }
    }

    public void Empire_FirstWave(GameEventArg arg)
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 999
                                                              , true
                                                              , null
                                                              );

      switch (Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-8000, -350, 0), new TV_3DVECTOR(-2000, -350, 7000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, IntegerEventArg.N2);
          break;
        case "hard":
        case "normal":
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(6500, 100, -2000), new TV_3DVECTOR(1100, 50, 6500)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));
          break;
        case "easy":
        default:
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-2000, -150, -4000), new TV_3DVECTOR(-1000, -150, 5000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          break;
      }
    }

    public void Empire_SecondWave(GameEventArg arg)
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                                        , MainEnemyFaction
                                                        , true
                                                        , new TV_3DVECTOR()
                                                        , 999
                                                        , true
                                                        , null
                                                        );

      switch (Difficulty.ToLower())
      {
        case "mental":
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N5);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, IntegerEventArg.N3);
          break;
        case "hard":
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(5500, -200, -8000), new TV_3DVECTOR(1500, -200, 2000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N4);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, IntegerEventArg.N2);
          break;
        case "easy":
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));
          break;
        case "normal":
        default:
          m_pendingSDspawnlist.Add(new ShipSpawnEventArg(sspawn
                                                        , new TV_3DVECTOR(-5500, 100, -8000), new TV_3DVECTOR(-1500, 100, 2000)
                                                        , new TV_3DVECTOR(0, 0, 0)
                                                        ));

          Manager.AddEvent(0, Empire_TIEWave, IntegerEventArg.N2);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, IntegerEventArg.N1);
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
          count = 4;
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, IntegerEventArg.N2);
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

            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = ActorTypeFactory.Get("TIE"),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.GameTime,
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

    public void Empire_TIEWave_TIEsvsShips(GameEventArg arg)
    {
      int sets = ((IntegerEventArg)arg).Num;
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -200, Manager.MinBounds.z - 2500), new TV_3DVECTOR(2500, 800, Manager.MinBounds.z - 2500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 15
                                                                          , TargetType.SHIP
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
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
      PlayerInfo.ActorID = Manager.SceneCameraID;

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

    public void Scene_02_Switch(GameEventArg arg)
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

    public void Scene_02(GameEventArg arg)
    {
      ActorInfo player = ActorFactory.Get(m_PlayerID);

      Manager.AddEvent(Game.GameTime + 0.1f, Scene_EnterCutscene);
      Manager.AddEvent(Game.GameTime + 7, Scene_ExitCutscene);
      Manager.AddEvent(Game.GameTime + 9, Rebel_GiveControl);
      Manager.AddEvent(Game.GameTime + 14, Rebel_Delete);
      Manager.AddEvent(Game.GameTime + 11, Message_13);
      Manager.AddEvent(Game.GameTime + 13, Scene_02_Switch);
      Manager.AddEvent(Game.GameTime + 15, Message_14);
      Manager.AddEvent(Game.GameTime + 25, Message_15);

      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      cam.SetLocalPosition(400, 130, -1800);
      cam.MoveData.MaxSpeed = 50;
      cam.MoveData.Speed = 50;
      Manager.CameraTargetActorID = m_PlayerID;
      PlayerInfo.TempActorID = m_PlayerID;

      int counter = 0;
      int x = 0;
      int xn = -1;
      int y = 0;
      int z = -1400;

      foreach (int actorID in MainEnemyFaction.GetWings())
      {
        ActorInfo.Kill(Engine, actorID);
      }

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          x = xn * Engine.Random.Next(200, 600);
          xn *= -1;
          y = Engine.Random.Next(-200, 200);
          z += 200;
          ActionManager.ForceClearQueue(actorID);
          ActionManager.QueueNext(actorID, new Wait(8 + 0.2f * counter));
          ActionManager.QueueNext(actorID, new HyperspaceOut());
          actor.SetLocalPosition(x, y, z);
          actor.SetLocalRotation(0, 180, 0);
          actor.MoveData.Speed = 450;
          actor.MoveData.ResetTurn();
          actor.CanRetaliate = false;
          actor.CanEvade = false;
          counter++;
        }
      }

      ActorInfo trn1 = ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = ActorFactory.Get(m_Transport3ID);

      Engine.ActorDataSet.CombatData[trn1.dataID].DamageModifier = 0;
      Engine.ActorDataSet.CombatData[trn2.dataID].DamageModifier = 0;
      Engine.ActorDataSet.CombatData[trn3.dataID].DamageModifier = 0;

      player.SetLocalPosition(0, 0, 500);

      ActionManager.ForceClearQueue(m_Transport1ID);
      ActionManager.QueueNext(m_Transport1ID, new Wait(8.5f));
      ActionManager.QueueNext(m_Transport1ID, new HyperspaceOut());
      trn1.MoveData.MaxSpeed = 400;
      trn1.MoveData.Speed = 400;

      ActionManager.ForceClearQueue(m_Transport2ID);
      ActionManager.QueueNext(m_Transport2ID, new Wait(8.8f));
      ActionManager.QueueNext(m_Transport2ID, new HyperspaceOut());
      trn2.MoveData.MaxSpeed = 400;
      trn2.MoveData.Speed = 400;

      ActionManager.ForceClearQueue(m_Transport3ID);
      ActionManager.QueueNext(m_Transport3ID, new Wait(9.1f));
      ActionManager.QueueNext(m_Transport3ID, new HyperspaceOut());
      trn3.MoveData.MaxSpeed = 400;
      trn3.MoveData.Speed = 400;

      player.MoveData.Speed = 400;
      player.MoveData.ResetTurn(); 
      ActionManager.ForceClearQueue(m_PlayerID);
      ActionManager.QueueNext(m_PlayerID, new Lock());
      

      int en_ship = 0;
      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (en_ship == 1)
          {
            actor.SetLocalPosition(0, -300, 2500);
            actor.SetLocalRotation(0, 180, 0);
            actor.MoveData.Speed = actor.MoveData.MaxSpeed * 0.25f;
            ActionManager.ForceClearQueue(actorID);
            ActionManager.QueueNext(actorID, new Wait(60));
            ActionManager.QueueNext(actorID, new Rotate(new TV_3DVECTOR(-2000, -300, 2000), 0, -1, false));
            ActionManager.QueueNext(actorID, new Lock());
          }
          else if (en_ship == 2)
          {
            actor.SetLocalPosition(3300, 150, 5500);
            actor.LookAtPoint(new TV_3DVECTOR(1400, 150, 1000));
            actor.MoveData.Speed = actor.MoveData.MaxSpeed * 0.25f;
            ActionManager.ForceClearQueue(actorID);
            ActionManager.QueueNext(actorID, new Wait(30));
            ActionManager.QueueNext(actorID, new Rotate(new TV_3DVECTOR(-32000, 150, 2000), 0, -1, false));
            ActionManager.QueueNext(actorID, new Lock());
          }
          else
          {
            ActorInfo.Kill(Engine, actorID);
          }
        }
        en_ship++;
      }

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("Imperial-I Star Destroyer"),
        Name = "",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = Game.GameTime + 9,
        Faction = MainEnemyFaction,
        Position = new TV_3DVECTOR(20000, -2000, -22000),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(2000, 100, -8000))
                                    , new Move(new TV_3DVECTOR(1000, 100, 2000), ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed * 0.25f, -1, false)
                                    , new Rotate(new TV_3DVECTOR(2000, 100, -9000), 0, -1, false)
                                    , new Lock() },
        Registries = null
      };
      ActorInfo newDest = asi.Spawn(this);
      Engine.ActorDataSet.CombatData[newDest.dataID].DamageModifier = 0.1f;

      asi.SpawnTime = Game.GameTime + 9.25f;
      asi.Position = new TV_3DVECTOR(20000, -2000, -25000);
      asi.Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(1500, -100, -10200))
                                            , new Move(new TV_3DVECTOR(-6500, -100, 4000), ActorTypeFactory.Get("Imperial-I Star Destroyer").MaxSpeed * 0.25f, -1, false)
                                            , new Rotate(new TV_3DVECTOR(2000, -100, -10200), 0, -1, false)
                                            , new Lock() };
      newDest = asi.Spawn(this);
      Engine.ActorDataSet.CombatData[newDest.dataID].DamageModifier = 0.1f;
    }

    public void Scene_02b_LightspeedFail(GameEventArg arg)
    {
      Manager.AddEvent(Game.GameTime + 1, Message_16);
      Manager.AddEvent(Game.GameTime + 5, Message_17);
      Manager.AddEvent(Game.GameTime + 12, Scene_02_ViolentShake);
      Manager.AddEvent(Game.GameTime + 15, Message_18);
      Manager.AddEvent(Game.GameTime + 18, Message_19);
      Manager.AddEvent(Game.GameTime + 23, Message_20);
      Manager.AddEvent(Game.GameTime + 28, Message_21);
      Manager.AddEvent(Game.GameTime + 33, Message_22);
      //Manager.AddEvent(Game.GameTime + 33, "Scene_03");

      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -30000);
      Manager.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -30000);

      SoundManager.SetMusic("battle_2_3");
      SoundManager.SetMusicLoop("battle_2_3");
    }

    public void Scene_02_ViolentShake(GameEventArg arg)
    {
      PlayerCameraInfo.Shake = 75;
    }

    #endregion

    #region Text
    public void Message_01_Leaving(GameEventArg arg)
    {
      Screen2D.MessageText("ECHO BASE: Escort the transports to the designated locations for hyperspace jump. ", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_Conditions(GameEventArg arg)
    {
      Screen2D.MessageText("ECHO BASE: All transports must survive.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Turbolasers(GameEventArg arg)
    {
      Screen2D.MessageText("TRANSPORT: The Heavy Turbolaser Towers on the Star Destroyers must be taken out.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_04_TIEs(GameEventArg arg)
    {
      Screen2D.MessageText("X-WING: We will need to worry about the TIEs too.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_05(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: We will figure something out.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_06(GameEventArg arg)
    {
      Screen2D.MessageText("ECHO BASE: Ion Control, standby.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07(GameEventArg arg)
    {
      Screen2D.MessageText("ECHO BASE: Fire.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: Here's our opportunity.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_09(GameEventArg arg)
    {
      Screen2D.MessageText("X-WING: We will take care of the fighters.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_10(GameEventArg arg)
    {
      Screen2D.MessageText("X-WING: We need someone to take out the Heavy Turbolasers.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_11(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: I can take care of that.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_12(GameEventArg arg)
    {
      Screen2D.MessageText("ECHO BASE: More Star Destroyers incoming.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_13(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: I see them. Two Star Destroyers here coming right at us.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_14(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: [Use the secondary weapon toggle to switch between front and rear deflector shields.]", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_15(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: We can still out-manuever them.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_16(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: Prepare to make the jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_17(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: Watch this!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_18(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: ...", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_19(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: I think we are in trouble.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_20(GameEventArg arg)
    {
      Screen2D.MessageText("C3PO: The hyperdrive modulator has been damaged, sir.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_21(GameEventArg arg)
    {
      Screen2D.MessageText("C3PO: It is impossible to jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_22(GameEventArg arg)
    {
      Screen2D.MessageText("SOLO: We are in trouble!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_23(GameEventArg arg)
    {
      Screen2D.MessageText("LEIA: Han, get up here!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }
    #endregion
  }
}
