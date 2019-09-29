using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Primitives;
using SWEndor.Primitives.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SWEndor.Scenarios
{
  public class GSHoth : GameScenarioBase
  {
    public GSHoth(GameScenarioManager manager) : base(manager)
    {
      Name = "Escape from Hoth (WIP up to Stage 02) [Maintenance]";
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
    private int m_Player_PrimaryWeaponN;
    private int m_Player_SecondaryWeaponN;

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
      ActorFactory.Create(aci_Hoth);
    }

    public override void GameTick()
    {
      base.GameTick();

      ActorInfo player = ActorFactory.Get(m_PlayerID);
      ActorInfo trn1 = ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = ActorFactory.Get(m_Transport3ID);
      if (player != null
          && !player.IsDyingOrDead)
      {
        MainEnemyFaction.WingLimit = 6 + MainAllyFaction.WingCount * 2;
        if (MainEnemyFaction.WingLimit > 12)
          MainEnemyFaction.WingLimit = 12;

        if (StageNumber == 0)
        {
          StageNumber = 1;
        }
        else if (StageNumber == 1 && trn1 != null)
        {
          if (transport_currentZpos > trn1.GetGlobalPosition().z)
          {
            transport_currentZpos = trn1.GetGlobalPosition().z;
          }
          if (trn1.GetGlobalPosition().z < transport_hyperspaceZpos
            && !player.IsDyingOrDead
            && !Manager.GetGameStateB("Stage1End") 
            && !Manager.GetGameStateB("GameOver"))
          {
            Manager.AddEvent(Game.GameTime + 0.1f, Scene_02);
            trn1.SetArmor(DamageType.ALL, 0);
            trn2.SetArmor(DamageType.ALL, 0);
            trn3.SetArmor(DamageType.ALL, 0);
            Manager.SetGameStateB("Stage1End", true);
          }
        }
        else if (StageNumber == 2)
        {
          if (CurrentTIEs > MainEnemyFaction.WingCount)
          {
            TIEsLeft -= CurrentTIEs - MainEnemyFaction.WingCount;
          }
          CurrentTIEs = MainEnemyFaction.WingCount;

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

      if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.ShipCount < 7)
      {
        Manager.AddEvent(0, Empire_StarDestroyer_Spawn, m_pendingSDspawnlist[0]);
        m_pendingSDspawnlist.RemoveAt(0);
      }

      if (StageNumber == 1)
      {
        if (Manager.Scenario.TimeSinceLostWing < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
          Manager.Line1Text = "WINGS: {0}".F(MainAllyFaction.WingCount.ToString());
        else
          Manager.Line1Text = "";

        if (!Manager.GetGameStateB("Stage1End"))
          Manager.Line2Text = "DIST: {0}".F((transport_currentZpos - transport_hyperspaceZpos).ToString("00000"));
        else
          Manager.Line2Text = "";
      }
      else if (StageNumber == 2)
      {
        if (TIEsLeft > 0)
        {
          Manager.Line1Text = "Destroy TIEs";
          Manager.Line2Text = string.Format("TIEs: {0}", TIEsLeft.ToString());
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

    public void Rebel_HyperspaceIn()
    {
      ActorInfo ainfo;
      //ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      //cam.Position = new TV_3DVECTOR(200, 350, Manager.MaxBounds.z - 1500);
      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(200, 350, Manager.MaxBounds.z - 1500));

      // Player Falcon
      ainfo = new ActorSpawnInfo
      {
        Type = PlayerInfo.ActorType,
        Name = "(Player)",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
        Position = new TV_3DVECTOR(0, 200, Manager.MaxBounds.z - 450),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);

      //ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
      ainfo.SetArmor(DamageType.ALL, 0.1f);
      ainfo.HitEvents += Rebel_PlayerHit;
      PlayerCameraInfo.Look.SetTarget_LookAtActor(ainfo.ID);
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
          SidebarName = "",
          SpawnTime = Game.GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Lock(), new Wait(3) },
          Registries = null
        }.Spawn(this);

        ainfo.SetArmor(DamageType.ALL, 0.85f);
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
          SidebarName = "TRANSPORT",
          SpawnTime = Game.GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { new Lock(), new Wait(3) },
          Registries = new string[] { "CriticalAllies" }
        }.Spawn(this);

        ainfo.SetArmor(DamageType.ALL, 0.6f);

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

    public void Rebel_IonCannonSpawn()
    {
      TV_3DVECTOR position = new TV_3DVECTOR(0, 500, Manager.MaxBounds.z + 3000);
      TV_3DVECTOR rotation = new TV_3DVECTOR(0, 180, 0);
      if (MainEnemyFaction.ShipCount > 0)
      {
        ActorInfo target = ActorFactory.Get(MainEnemyFaction.GetShip(0));
        if (target != null)
        {
          TV_3DVECTOR dir = target.GetGlobalPosition() - position;
          rotation = dir.ConvertDirToRot();
        }
      }

      new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("Large Ion Laser"),
        Name = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = position,
        Rotation = rotation,
        Actions = new ActionInfo[] { new Lock() },
        Registries = null
      }.Spawn(this);
    }

    public void Rebel_MakePlayer()
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;

      if (PlayerInfo.Actor == null || PlayerInfo.Actor.Disposed)
      {
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;

          TV_3DVECTOR position = new TV_3DVECTOR();
          if (StageNumber == 1)
          {
            if (Manager.CriticalAllies.Count > 0)
            {
              ActorInfo crit = Manager.CriticalAllies.ToArray()[0];
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
            Type = PlayerInfo.ActorType,
            Name = "(Player)",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
            Position = position,
            Rotation = new TV_3DVECTOR(0, 180, 0),
            Actions = new ActionInfo[] { new Lock() },
            Registries = null
          }.Spawn(this);

          //ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
          ainfo.SetArmor(DamageType.ALL, 0.1f);
          ainfo.HitEvents += Rebel_PlayerHit;

          ainfo.SetPlayer();
        }
      }
      m_PlayerID = PlayerInfo.ActorID;
    }

    public void Rebel_GiveControl()
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
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

    public void Rebel_Delete()
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor.IsPlayer)
          actor.Delete();
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor.IsPlayer)
          actor.Delete();
      }
    }

    public void Rebel_PlayerHit(ActorInfo attacker, ActorInfo player)
    {

      /* DISABLED until new Shield implementation is found
      if (!Engine.MaskDataSet[attacker].Has(ComponentMask.IS_DAMAGE))
      {
        player.InflictDamage(attacker, attacker.TypeInfo.ImpactDamage, DamageType.NORMAL, player.GetGlobalPosition());
        //CombatSystem.onNotify(Engine, player, Actors.Components.CombatEventType.DAMAGE, attacker.TypeInfo.ImpactDamage);
        PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
      }
      else
      {
        TV_3DVECTOR rot = player.GetGlobalRotation();
        TV_3DVECTOR tgtrot = Utilities.GetRotation(attacker.GetRelativePositionFUR(-1000, 0, 0) - player.PrevPosition);

        float chgy = tgtrot.y - rot.y;

        while (chgy < -180)
          chgy += 360;

        while (chgy > 180)
          chgy -= 360;

        if ((chgy < -90 || chgy > 90) && PlayerInfo.SecondaryWeapon != "rear")
        {
          player.InflictDamage(attacker, 1, DamageType.NORMAL, player.GetGlobalPosition());
          //CombatSystem.onNotify(Engine, player, Actors.Components.CombatEventType.DAMAGE, 1);
          PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
        }
        else if ((chgy > -90 && chgy < 90) && PlayerInfo.SecondaryWeapon != "front")
        {
          player.InflictDamage(attacker, 1, DamageType.NORMAL, player.GetGlobalPosition());
          //CombatSystem.onNotify(Engine, player, Actors.Components.CombatEventType.DAMAGE, 1);
          PlayerInfo.FlashHit(PlayerInfo.StrengthColor);
        }
        else
        {
          PlayerCameraInfo.Shake(2 * attacker.TypeInfo.ImpactDamage);
        }
      }
      */
    }

    public void Rebel_CriticalUnitLost(ActorInfo actor, ActorState state)
    {
      if (Manager.GetGameStateB("GameWon"))
        return;

      if (Manager.GetGameStateB("GameOver"))
        return;

      if (actor.IsDyingOrDead)
      {
        Manager.SetGameStateB("GameOver", true);
        Manager.IsCutsceneMode = true;

        PlayerInfo.ActorID = -1;
        PlayerCameraInfo.Look.SetPosition_Actor(actor.ID);
        PlayerCameraInfo.Look.SetModeDeathCircle(actor.TypeInfo.DeathCamera);

        if (actor.IsDying)
        {
          actor.TickEvents += ProcessPlayerDying;
          actor.DestroyedEvents += ProcessPlayerKilled;
        }
        else
        {
          actor.DestroyedEvents += ProcessPlayerKilled;
        }

        if (actor.TypeInfo is TransportATI)
        {
          actor.DyingTimerSet(2000, true);
          Manager.AddEvent(Game.GameTime + 25, FadeOut);
        }
      }
    }

    #endregion

    #region Empire spawns

    public void Empire_StarDestroyer_Spawn(ShipSpawnEventArg s)
    {
      ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);

      ship.HuntWeight = 1;
      ship.SetArmor(DamageType.ALL, 0.1f);
    }

    public void Empire_FirstWave()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 999
                                                              , true
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

          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
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

    public void Empire_SecondWave()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                        , ActorTypeFactory.Get("Imperial-I Star Destroyer")
                                                        , MainEnemyFaction
                                                        , true
                                                        , new TV_3DVECTOR()
                                                        , 999
                                                        , true
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

          Manager.AddEvent(0, Empire_TIEWave, 5);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 3);
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

          Manager.AddEvent(0, Empire_TIEWave, 4);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
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

          Manager.AddEvent(0, Empire_TIEWave, 2);
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 1);
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
          count = 4;
          Manager.AddEvent(0, Empire_TIEWave_TIEsvsShips, 2);
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

    public void Empire_TIEWave_TIEsvsShips(int sets)
    {
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

    public void Scene_EnterCutscene()
    {
      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_PrimaryWeaponN = PlayerInfo.PrimaryWeaponN;
        m_Player_SecondaryWeaponN = PlayerInfo.SecondaryWeaponN;
        m_Player_DamageModifier = player.GetArmor(DamageType.ALL);
        player.SetArmor(DamageType.ALL, 0);
        player.ForceClearQueue();
        player.QueueNext(new Lock());
      }
      PlayerInfo.ActorID = -1; // Manager.SceneCameraID;

      Manager.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene()
    {
      ActorInfo player = ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        PlayerInfo.ActorID = m_PlayerID;
        PlayerInfo.PrimaryWeaponN = m_Player_PrimaryWeaponN;
        PlayerInfo.SecondaryWeaponN = m_Player_SecondaryWeaponN;

        player.SetArmor(DamageType.ALL, m_Player_DamageModifier);
        player.ForceClearQueue();
      }
      Manager.IsCutsceneMode = false;
    }

    public void Scene_02_Switch()
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

    public void Scene_02()
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

      //cam.MoveData.MaxSpeed = 50;
      //cam.MoveData.Speed = 50;
      PlayerCameraInfo.Look.SetPosition_Point(new TV_3DVECTOR(400, 130, -1800));
      //PlayerCameraInfo.LookAtPosition = new TV_3DVECTOR(200, 350, Manager.MaxBounds.z - 1500);

      PlayerCameraInfo.Look.SetTarget_LookAtActor(m_PlayerID);
      PlayerInfo.TempActorID = m_PlayerID;

      int counter = 0;
      int x = 0;
      int xn = -1;
      int y = 0;
      int z = -1400;

      foreach (int actorID in MainEnemyFaction.GetWings())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
          actor.Delete();
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
          actor.ForceClearQueue();
          actor.QueueNext(new Wait(8 + 0.2f * counter));
          actor.QueueNext(new HyperspaceOut());
          actor.Position = new TV_3DVECTOR(x, y, z);
          actor.Rotation = new TV_3DVECTOR(0, 180, 0);
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

      trn1.SetArmor(DamageType.ALL, 0);
      trn2.SetArmor(DamageType.ALL, 0);
      trn3.SetArmor(DamageType.ALL, 0);

      player.Position = new TV_3DVECTOR(0, 0, 500);

      trn1.ForceClearQueue();
      trn1.QueueNext(new Wait(8.5f));
      trn1.QueueNext(new HyperspaceOut());
      trn1.MoveData.MaxSpeed = 400;
      trn1.MoveData.Speed = 400;

      trn2.ForceClearQueue();
      trn2.QueueNext(new Wait(8.8f));
      trn2.QueueNext(new HyperspaceOut());
      trn2.MoveData.MaxSpeed = 400;
      trn2.MoveData.Speed = 400;

      trn3.ForceClearQueue();
      trn3.QueueNext(new Wait(9.1f));
      trn3.QueueNext(new HyperspaceOut());
      trn3.MoveData.MaxSpeed = 400;
      trn3.MoveData.Speed = 400;

      player.MoveData.Speed = 400;
      player.MoveData.ResetTurn();
      player.ForceClearQueue();
      player.QueueNext(new Lock());
      

      int en_ship = 0;
      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (en_ship == 1)
          {
            actor.Position = new TV_3DVECTOR(0, -300, 2500);
            actor.Rotation = new TV_3DVECTOR(0, 180, 0);
            actor.MoveData.Speed = actor.MoveData.MaxSpeed * 0.25f;
            actor.ForceClearQueue();
            actor.QueueNext(new Wait(60));
            actor.QueueNext(new Rotate(new TV_3DVECTOR(-2000, -300, 2000), 0, -1, false));
            actor.QueueNext(new Lock());
          }
          else if (en_ship == 2)
          {
            actor.Position = new TV_3DVECTOR(3300, 150, 5500);
            actor.LookAt(new TV_3DVECTOR(1400, 150, 1000));
            actor.MoveData.Speed = actor.MoveData.MaxSpeed * 0.25f;
            actor.ForceClearQueue();
            actor.QueueNext(new Wait(30));
            actor.QueueNext(new Rotate(new TV_3DVECTOR(-32000, 150, 2000), 0, -1, false));
            actor.QueueNext(new Lock());
          }
          else
          {
            actor.Delete();
          }
        }
        en_ship++;
      }

      ActorSpawnInfo asi = new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("Imperial-I Star Destroyer"),
        Name = "",
        SidebarName = "",
        SpawnTime = Game.GameTime + 9,
        Faction = MainEnemyFaction,
        Position = new TV_3DVECTOR(20000, -2000, -22000),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(2000, 100, -8000))
                                    , new Move(new TV_3DVECTOR(1000, 100, 2000), ActorTypeFactory.Get("Imperial-I Star Destroyer").MoveLimitData.MaxSpeed * 0.25f, -1, false)
                                    , new Rotate(new TV_3DVECTOR(2000, 100, -9000), 0, -1, false)
                                    , new Lock() },
        Registries = null
      };
      ActorInfo newDest = asi.Spawn(this);
      newDest.SetArmor(DamageType.ALL, 0.1f);

      asi.SpawnTime = Game.GameTime + 9.25f;
      asi.Position = new TV_3DVECTOR(20000, -2000, -25000);
      asi.Actions = new ActionInfo[] { new HyperspaceIn(new TV_3DVECTOR(1500, -100, -10200))
                                            , new Move(new TV_3DVECTOR(-6500, -100, 4000), ActorTypeFactory.Get("Imperial-I Star Destroyer").MoveLimitData.MaxSpeed * 0.25f, -1, false)
                                            , new Rotate(new TV_3DVECTOR(2000, -100, -10200), 0, -1, false)
                                            , new Lock() };
      newDest = asi.Spawn(this);
      newDest.SetArmor(DamageType.ALL, 0.1f);
    }

    public void Scene_02b_LightspeedFail()
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

    public void Scene_02_ViolentShake()
    {
      PlayerCameraInfo.Shake(75);
    }

    #endregion

    #region Text
    public void Message_01_Leaving()
    {
      Screen2D.MessageText("ECHO BASE: Escort the transports to the designated locations for hyperspace jump. ", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_02_Conditions()
    {
      Screen2D.MessageText("ECHO BASE: All transports must survive.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_03_Turbolasers()
    {
      Screen2D.MessageText("TRANSPORT: The Heavy Turbolaser Towers on the Star Destroyers must be taken out.", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }

    public void Message_04_TIEs()
    {
      Screen2D.MessageText("X-WING: We will need to worry about the TIEs too.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_05()
    {
      Screen2D.MessageText("SOLO: We will figure something out.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_06()
    {
      Screen2D.MessageText("ECHO BASE: Ion Control, standby.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_07()
    {
      Screen2D.MessageText("ECHO BASE: Fire.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_08()
    {
      Screen2D.MessageText("SOLO: Here's our opportunity.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_09()
    {
      Screen2D.MessageText("X-WING: We will take care of the fighters.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_10()
    {
      Screen2D.MessageText("X-WING: We need someone to take out the Heavy Turbolasers.", 5, new TV_COLOR(0.8f, 0, 0, 1));
    }

    public void Message_11()
    {
      Screen2D.MessageText("SOLO: I can take care of that.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_12()
    {
      Screen2D.MessageText("ECHO BASE: More Star Destroyers incoming.", 5, new TV_COLOR(0.6f, 0.6f, 0.9f, 1));
    }

    public void Message_13()
    {
      Screen2D.MessageText("SOLO: I see them. Two Star Destroyers here coming right at us.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_14()
    {
      Screen2D.MessageText("SOLO: [Use the secondary weapon toggle to switch between front and rear deflector shields.]", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_15()
    {
      Screen2D.MessageText("SOLO: We can still out-manuever them.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_16()
    {
      Screen2D.MessageText("SOLO: Prepare to make the jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_17()
    {
      Screen2D.MessageText("SOLO: Watch this!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_18()
    {
      Screen2D.MessageText("SOLO: ...", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_19()
    {
      Screen2D.MessageText("SOLO: I think we are in trouble.", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_20()
    {
      Screen2D.MessageText("C3PO: The hyperdrive modulator has been damaged, sir.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_21()
    {
      Screen2D.MessageText("C3PO: It is impossible to jump to lightspeed.", 5, new TV_COLOR(0.8f, 0.8f, 0.1f, 1));
    }

    public void Message_22()
    {
      Screen2D.MessageText("SOLO: We are in trouble!", 5, new TV_COLOR(0.8f, 0.8f, 0.9f, 1));
    }

    public void Message_23()
    {
      Screen2D.MessageText("LEIA: Han, get up here!", 5, new TV_COLOR(0.8f, 0.4f, 0.4f, 1));
    }
    #endregion
  }
}
