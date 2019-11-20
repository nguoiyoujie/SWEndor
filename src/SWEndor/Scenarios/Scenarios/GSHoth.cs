using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Models;
using SWEndor.Player;
using Primrose.Primitives.Extensions;
using Primrose.Primitives.Geometry;
using System.Collections.Generic;
using System.Linq;
using SWEndor.Primitives.Extensions;

namespace SWEndor.Scenarios
{
  public class GSHoth : ScenarioBase
  {
    public GSHoth(ScenarioManager manager) : base(manager)
    {
      Info.Name = "Escape from Hoth (WIP up to Stage 02) [Maintenance]";
      Info.AllowedWings = new ActorTypeInfo[] { ActorTypeFactory.Get("FALC") };

      Info.AllowedDifficulties = new string[] { "normal"
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

    public FactionInfo MainAllyFaction = FactionInfo.Neutral;
    public FactionInfo MainEnemyFaction = FactionInfo.Neutral;

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

      State.MaxBounds = new TV_3DVECTOR(15000, 1500, 9000);
      State.MinBounds = new TV_3DVECTOR(-15000, -1500, -20000);
      State.MaxAIBounds = new TV_3DVECTOR(15000, 1500, 8000);
      State.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -20000);

      EventQueue.Add(Game.GameTime + 0.1f, Rebel_HyperspaceIn);
      EventQueue.Add(Game.GameTime + 0.1f, Empire_FirstWave);
      EventQueue.Add(Game.GameTime + 5.5f, Rebel_MakePlayer);
      EventQueue.Add(Game.GameTime + 5.5f, Message_01_Leaving);
      EventQueue.Add(Game.GameTime + 9.5f, Rebel_GiveControl);
      EventQueue.Add(Game.GameTime + 10f, Message_02_Conditions);
      EventQueue.Add(Game.GameTime + 6.5f, Empire_FirstTIEWave);
      EventQueue.Add(Game.GameTime + 18, Message_03_Turbolasers);
      EventQueue.Add(Game.GameTime + 23, Message_04_TIEs);
      EventQueue.Add(Game.GameTime + 28, Message_05);
      EventQueue.Add(Game.GameTime + 45, Message_06);
      EventQueue.Add(Game.GameTime + 49, Message_07);
      EventQueue.Add(Game.GameTime + 52, Rebel_IonCannonSpawn);
      EventQueue.Add(Game.GameTime + 56, Message_08);
      EventQueue.Add(Game.GameTime + 61, Message_09);
      EventQueue.Add(Game.GameTime + 66, Message_10);
      EventQueue.Add(Game.GameTime + 71, Message_11);
      EventQueue.Add(Game.GameTime + 97, Message_12);
      EventQueue.Add(Game.GameTime + 100, Empire_SecondWave);

      PlayerInfo.Lives = 4;
      PlayerInfo.ScorePerLife = 1000000;
      PlayerInfo.ScoreForNextLife = 1000000;

      MakePlayer = Rebel_MakePlayer;

      Screen2D.Line1.Color = new COLOR(1f, 1f, 0.3f, 1);
      Screen2D.Line2.Color = new COLOR(1f, 1f, 0.3f, 1);
      //Screen2D.Line3.Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      SoundManager.SetMusic("battle_2_1");
      SoundManager.SetMusicLoop("battle_2_2");

      State.IsCutsceneMode = false;
    }

    internal override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Falcon", new COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      FactionInfo.Factory.Get("Rebels").Allies.Add(FactionInfo.Factory.Get("Rebels_Falcon"));
      FactionInfo.Factory.Get("Rebels_Falcon").Allies.Add(FactionInfo.Factory.Get("Rebels"));

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");

      MainAllyFaction.WingLimitIncludesAllies = true;
    }

    internal override void LoadScene()
    {
      base.LoadScene();

      // Create Hoth
      ActorCreationInfo aci_Hoth = new ActorCreationInfo(ActorTypeFactory.Get("HOTH"))
      {
        CreationTime = -1,
        Position = new TV_3DVECTOR(0, 0, 36000),
        Rotation = new TV_3DVECTOR(-90, 0, 0),
        InitialScale =12
      };
      Engine.ActorFactory.Create(aci_Hoth);
    }

    public override void GameTick()
    {
      base.GameTick();

      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      ActorInfo trn1 = Engine.ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = Engine.ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = Engine.ActorFactory.Get(m_Transport3ID);
      if (player != null
          && !player.IsDyingOrDead)
      {
        MainEnemyFaction.WingSpawnLimit = 6 + MainAllyFaction.WingCount * 2;
        if (MainEnemyFaction.WingSpawnLimit > 12)
          MainEnemyFaction.WingSpawnLimit = 12;

        if (State.StageNumber == 0)
        {
          State.StageNumber = 1;
        }
        else if (State.StageNumber == 1 && trn1 != null)
        {
          if (transport_currentZpos > trn1.GetGlobalPosition().z)
          {
            transport_currentZpos = trn1.GetGlobalPosition().z;
          }
          if (trn1.GetGlobalPosition().z < transport_hyperspaceZpos
            && !player.IsDyingOrDead
            && !State.GetGameStateB("Stage1End") 
            && !State.GetGameStateB("GameOver"))
          {
            EventQueue.Add(Game.GameTime + 0.1f, Scene_02);
            trn1.SetArmor(DamageType.ALL, 0);
            trn2.SetArmor(DamageType.ALL, 0);
            trn3.SetArmor(DamageType.ALL, 0);
            State.SetGameStateB("Stage1End", true);
          }
        }
        else if (State.StageNumber == 2)
        {
          if (CurrentTIEs > MainEnemyFaction.WingCount)
          {
            TIEsLeft -= CurrentTIEs - MainEnemyFaction.WingCount;
          }
          CurrentTIEs = MainEnemyFaction.WingCount;

          if (TIEsLeft < 1 && !State.GetGameStateB("Stage2b"))
          {
            Screen2D.Line1.Color = new COLOR(1f, 1f, 0.3f, 1);
            Screen2D.Line2.Color = new COLOR(1f, 1f, 0.3f, 1);
            Screen2D.Line1.Text = "Proceed to";
            Screen2D.Line2.Text = "Hyperspace Lane";
            State.SetGameStateB("Stage2b", true);
            Screen2D.Box3D_Enable = true;
            Screen2D.Box3D_color = new COLOR(1f, 1f, 0.3f, 1);
            Screen2D.Box3D_min = hyperspace_lane_min;
            Screen2D.Box3D_max = hyperspace_lane_max;
          }

          if (TIEsLeft < 1 && !State.GetGameStateB("Stage2End") && !player.IsOutOfBounds(hyperspace_lane_min, hyperspace_lane_max))
          {
            EventQueue.Add(Game.GameTime + 0.1f, Scene_02b_LightspeedFail);
            State.SetGameStateB("Stage2End", true);
            Screen2D.Box3D_Enable = false;
          }
        }
      }

      if (m_pendingSDspawnlist.Count > 0 && MainEnemyFaction.ShipCount < 7)
      {
        EventQueue.Add(0, Empire_StarDestroyer_Spawn, m_pendingSDspawnlist[0]);
        m_pendingSDspawnlist.RemoveAt(0);
      }

      if (State.StageNumber == 1)
      {
        if (State.TimeSinceLostWing < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
          Screen2D.Line1.Text = "WINGS: {0}".F(MainAllyFaction.WingCount.ToString());
        else
          Screen2D.Line1.Text = "";

        if (!State.GetGameStateB("Stage1End"))
          Screen2D.Line2.Text = "DIST: {0}".F((transport_currentZpos - transport_hyperspaceZpos).ToString("00000"));
        else
          Screen2D.Line2.Text = "";
      }
      else if (State.StageNumber == 2)
      {
        if (TIEsLeft > 0)
        {
          Screen2D.Line1.Text = "Destroy TIEs";
          Screen2D.Line2.Text = string.Format("TIEs: {0}", TIEsLeft.ToString());
        }
        else if (!State.GetGameStateB("Stage2End"))
        {
        }
        else
        {
          Screen2D.Line1.Text = "";
          Screen2D.Line2.Text = "";
        }
      }
      else
      {
        Screen2D.Line2.Text = "";
      }
    }

    #region Rebellion spawns

    public void Rebel_HyperspaceIn()
    {
      ActorInfo ainfo;
      //ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      //cam.Position = new TV_3DVECTOR(200, 350, State.MaxBounds.z - 1500);
      PlayerCameraInfo.SceneLook.SetPosition_Point(new TV_3DVECTOR(200, 350, State.MaxBounds.z - 1500));

      // Player Falcon
      ainfo = new ActorSpawnInfo
      {
        Type = PlayerInfo.ActorType,
        Name = "(Player)",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels_Falcon"),
        Position = new TV_3DVECTOR(0, 200, State.MaxBounds.z - 450),
        Rotation = new TV_3DVECTOR(0, 180, 0),
        Actions = new ActionInfo[] { Lock.GetOrCreate() },
        Registries = null
      }.Spawn(this);

      //ainfo.WeaponSystemInfo.SecondaryWeapons = new string[] { "front", "rear" };
      ainfo.SetArmor(DamageType.ALL, 0.1f);
      ainfo.HitEvents += Rebel_PlayerHit;
      PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(ainfo.ID);
      PlayerCameraInfo.SetSceneLook();
      PlayerInfo.TempActorID = ainfo.ID;

      // X-Wings x12
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();

      for (int i = 0; i < 11; i++)
      {
        if (i % 4 == 1)
          positions.Add(new TV_3DVECTOR(-16 * (i + 1) - 250, 10 * i, State.MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 2)
          positions.Add(new TV_3DVECTOR(10 * i, -16 * (i + 2) - 250, State.MaxBounds.z - 1150 + 250 * i));
        else if (i % 4 == 3)
          positions.Add(new TV_3DVECTOR(-10 * i, 16 * (i + 2) + 250, State.MaxBounds.z - 1150 + 250 * i));
        else
          positions.Add(new TV_3DVECTOR(16 * (i + 2) + 250, 10 * i, State.MaxBounds.z - 1150 + 250 * i));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("XWING"),
          Name = "",
          SidebarName = "",
          SpawnTime = Game.GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { Lock.GetOrCreate(), Wait.GetOrCreate(3) },
          Registries = null
        }.Spawn(this);

        ainfo.SetArmor(DamageType.ALL, 0.85f);
      }

      // Transport x3
      positions.Clear();
      positions.Add(new TV_3DVECTOR(250, -220, State.MaxBounds.z + 20));
      positions.Add(new TV_3DVECTOR(-100, -20, State.MaxBounds.z + 70));
      positions.Add(new TV_3DVECTOR(50, 270, State.MaxBounds.z + 1020));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        ainfo = new ActorSpawnInfo
        {
          Type = ActorTypeFactory.Get("TRAN"),
          Name = "",
          SidebarName = "TRAN",
          SpawnTime = Game.GameTime,
          Faction = FactionInfo.Factory.Get("Rebels"),
          Position = v,
          Rotation = new TV_3DVECTOR(0, 180, 0),
          Actions = new ActionInfo[] { Lock.GetOrCreate(), Wait.GetOrCreate(3) },
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
      TV_3DVECTOR position = new TV_3DVECTOR(0, 500, State.MaxBounds.z + 3000);
      TV_3DVECTOR rotation = new TV_3DVECTOR(0, 180, 0);
      if (MainEnemyFaction.ShipCount > 0)
      {
        ActorInfo target = Engine.ActorFactory.Get(MainEnemyFaction.GetFirst(TargetType.SHIP));
        if (target != null)
        {
          TV_3DVECTOR dir = target.GetGlobalPosition() - position;
          rotation = dir.ConvertDirToRot(Engine.TrueVision.TVMathLibrary);
        }
      }

      // need to switch to ProjectileType
      new ActorSpawnInfo
      {
        Type = ActorTypeFactory.Get("LSR_IONBIG"),
        Name = "",
        SidebarName = "",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Rebels"),
        Position = position,
        Rotation = rotation,
        Actions = new ActionInfo[] { Lock.GetOrCreate() },
        Registries = null
      }.Spawn(this);
    }

    public void Rebel_MakePlayer()
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;
      PlayerCameraInfo.SetPlayerLook();

      if (PlayerInfo.Actor == null || PlayerInfo.Actor.Disposed)
      {
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;

          TV_3DVECTOR position = new TV_3DVECTOR();
          if (State.StageNumber == 1)
          {
            if (State.CriticalAllies.Count > 0)
            {
              ActorInfo crit = State.CriticalAllies.ToArray()[0];
              position = crit.GetRelativePositionXYZ(0, -100, -1750);
            }
            else
              position = new TV_3DVECTOR(0, 300, State.MaxBounds.z - 850);
          }
          else if (State.StageNumber == 2)
          {
            TV_3DVECTOR pos = new TV_3DVECTOR();
            int count = 0;
            foreach (int enID in MainEnemyFaction.GetShips())
            {
              ActorInfo en = Engine.ActorFactory.Get(enID);
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
            Actions = new ActionInfo[] { Lock.GetOrCreate() },
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
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.UnlockOne();
          actor.SetState_Normal();
          actor.MoveData.Speed = actor.MoveData.MaxSpeed;
        }
      }
      PlayerInfo.IsMovementControlsEnabled = true;

      State.SetGameStateB("in_battle", true);
    }

    public void Rebel_Delete()
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor.IsPlayer)
          actor.Delete();
      }

      foreach (int actorID in MainAllyFaction.GetShips())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor.IsPlayer)
          actor.Delete();
      }
    }

    public void Rebel_PlayerHit(ActorInfo player)
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
      if (State.GetGameStateB("GameWon"))
        return;

      if (State.GetGameStateB("GameOver"))
        return;

      if (actor.IsDyingOrDead)
      {
        State.SetGameStateB("GameOver", true);
        State.IsCutsceneMode = true;

        PlayerInfo.ActorID = -1;

        if (actor.IsDying)
        {
          actor.TickEvents += ProcessPlayerDying;
          actor.DestroyedEvents += ProcessPlayerKilled;
          PlayerCameraInfo.DeathLook.SetPosition_Actor(actor.ID, actor.TypeInfo.DeathCamera);
        }
        else
        {
          actor.DestroyedEvents += ProcessPlayerKilled;
          PlayerCameraInfo.DeathLook.SetPosition_Point(actor.GetGlobalPosition(), actor.TypeInfo.DeathCamera);
        }
        PlayerCameraInfo.SetDeathLook();

        if (actor.TypeInfo is TransportATI)
        {
          actor.DyingTimerSet(2000, true);
          EventQueue.Add(Game.GameTime + 25, FadeOut);
        }
      }
    }

    #endregion

    #region Empire spawns

    public void Empire_StarDestroyer_Spawn(ShipSpawnEventArg s)
    {
      ActorInfo ship = GSFunctions.Ship_Spawn(Engine, this, s.Position, s.TargetPosition, s.FacingPosition, 0, s.Info);

      ship.AI.HuntWeight = 1;
      ship.SetArmor(DamageType.ALL, 0.1f);
    }

    public void Empire_FirstWave()
    {
      GSFunctions.ShipSpawnInfo sspawn = new GSFunctions.ShipSpawnInfo(null
                                                              , ActorTypeFactory.Get("IMPL")
                                                              , MainEnemyFaction
                                                              , true
                                                              , new TV_3DVECTOR()
                                                              , 999
                                                              , true
                                                              );

      switch (State.Difficulty.ToLower())
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

          EventQueue.Add(0, Empire_TIEWave_TIEsvsShips, 2);
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
                                                        , ActorTypeFactory.Get("IMPL")
                                                        , MainEnemyFaction
                                                        , true
                                                        , new TV_3DVECTOR()
                                                        , 999
                                                        , true
                                                        );

      switch (State.Difficulty.ToLower())
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

          EventQueue.Add(0, Empire_TIEWave, 5);
          EventQueue.Add(0, Empire_TIEWave_TIEsvsShips, 3);
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

          EventQueue.Add(0, Empire_TIEWave, 4);
          EventQueue.Add(0, Empire_TIEWave_TIEsvsShips, 2);
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

          EventQueue.Add(0, Empire_TIEWave, 2);
          EventQueue.Add(0, Empire_TIEWave_TIEsvsShips, 1);
          break;
      }
      State.SetGameStateB("Stage1BBegin", true);
    }

    public void Empire_FirstTIEWave()
    {
      int count = 0;
      switch (State.Difficulty.ToLower())
      {
        case "mental":
          count = 4;
          EventQueue.Add(0, Empire_TIEWave_TIEsvsShips, 2);
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
            switch (State.Difficulty.ToLower())
            {
              case "mental":
              case "hard":
                actions = new ActionInfo[] { Hunt.GetOrCreate(TargetType.FIGHTER) };
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
      Box box = new Box(new TV_3DVECTOR(-2500, -200, State.MinBounds.z - 5000), new TV_3DVECTOR(2500, 800, State.MinBounds.z - 5000));
      SquadSpawnInfo spawninfo = new SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 15
                                                                          , TargetType.FIGHTER
                                                                          , false
                                                                          , SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
    }

    public void Empire_TIEWave_TIEsvsShips(int sets)
    {
      Box box = new Box(new TV_3DVECTOR(-2500, -200, State.MinBounds.z - 2500), new TV_3DVECTOR(2500, 800, State.MinBounds.z - 2500));
      SquadSpawnInfo spawninfo = new SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 15
                                                                          , TargetType.SHIP
                                                                          , false
                                                                          , SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
    }

    #endregion

    #region Scene

    public void Scene_EnterCutscene()
    {
      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        m_Player_PrimaryWeaponN = PlayerInfo.PrimaryWeaponN;
        m_Player_SecondaryWeaponN = PlayerInfo.SecondaryWeaponN;
        m_Player_DamageModifier = player.GetArmor(DamageType.ALL);
        player.SetArmor(DamageType.ALL, 0);
        player.ForceClearQueue();
        player.QueueNext(Lock.GetOrCreate());
      }
      PlayerInfo.ActorID = -1; // Manager.SceneCameraID;

      State.IsCutsceneMode = true;
    }

    public void Scene_ExitCutscene()
    {
      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);
      if (player != null)
      {
        PlayerInfo.ActorID = m_PlayerID;
        PlayerInfo.PrimaryWeaponN = m_Player_PrimaryWeaponN;
        PlayerInfo.SecondaryWeaponN = m_Player_SecondaryWeaponN;

        player.SetArmor(DamageType.ALL, m_Player_DamageModifier);
        player.ForceClearQueue();
      }
      PlayerCameraInfo.SetPlayerLook();
      State.IsCutsceneMode = false;
    }

    public void Scene_02_Switch()
    {
      State.StageNumber = 2;
      Screen2D.Line1.Color = new COLOR(0.7f, 1f, 0.3f, 1);
      Screen2D.Line2.Color = new COLOR(0.7f, 1f, 0.3f, 1);
      switch (State.Difficulty.ToLower())
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
      ActorInfo player = Engine.ActorFactory.Get(m_PlayerID);

      EventQueue.Add(Game.GameTime + 0.1f, Scene_EnterCutscene);
      EventQueue.Add(Game.GameTime + 7, Scene_ExitCutscene);
      EventQueue.Add(Game.GameTime + 9, Rebel_GiveControl);
      EventQueue.Add(Game.GameTime + 14, Rebel_Delete);
      EventQueue.Add(Game.GameTime + 11, Message_13);
      EventQueue.Add(Game.GameTime + 13, Scene_02_Switch);
      EventQueue.Add(Game.GameTime + 15, Message_14);
      EventQueue.Add(Game.GameTime + 25, Message_15);

      //cam.MoveData.MaxSpeed = 50;
      //cam.MoveData.Speed = 50;
      PlayerCameraInfo.SceneLook.SetPosition_Point(new TV_3DVECTOR(400, 130, -1800));
      //PlayerCameraInfo.LookAtPosition = new TV_3DVECTOR(200, 350, State.MaxBounds.z - 1500);

      PlayerCameraInfo.SceneLook.SetTarget_LookAtActor(m_PlayerID);
      PlayerCameraInfo.SetSceneLook();

      PlayerInfo.TempActorID = m_PlayerID;

      int counter = 0;
      int x = 0;
      int xn = -1;
      int y = 0;
      int z = -1400;

      foreach (int actorID in MainEnemyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
          actor.Delete();
      }

      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          x = xn * Engine.Random.Next(200, 600);
          xn *= -1;
          y = Engine.Random.Next(-200, 200);
          z += 200;
          actor.ForceClearQueue();
          actor.QueueNext(Wait.GetOrCreate(8 + 0.2f * counter));
          actor.QueueNext(HyperspaceOut.GetOrCreate());
          actor.Position = new TV_3DVECTOR(x, y, z);
          actor.Rotation = new TV_3DVECTOR(0, 180, 0);
          actor.MoveData.Speed = 450;
          actor.MoveData.ResetTurn();
          actor.AI.CanRetaliate = false;
          actor.AI.CanEvade = false;
          counter++;
        }
      }

      ActorInfo trn1 = Engine.ActorFactory.Get(m_Transport1ID);
      ActorInfo trn2 = Engine.ActorFactory.Get(m_Transport2ID);
      ActorInfo trn3 = Engine.ActorFactory.Get(m_Transport3ID);

      trn1.SetArmor(DamageType.ALL, 0);
      trn2.SetArmor(DamageType.ALL, 0);
      trn3.SetArmor(DamageType.ALL, 0);

      player.Position = new TV_3DVECTOR(0, 0, 500);

      trn1.ForceClearQueue();
      trn1.QueueNext(Wait.GetOrCreate(8.5f));
      trn1.QueueNext(HyperspaceOut.GetOrCreate());
      trn1.MoveData.MaxSpeed = 400;
      trn1.MoveData.Speed = 400;

      trn2.ForceClearQueue();
      trn2.QueueNext(Wait.GetOrCreate(8.8f));
      trn2.QueueNext(HyperspaceOut.GetOrCreate());
      trn2.MoveData.MaxSpeed = 400;
      trn2.MoveData.Speed = 400;

      trn3.ForceClearQueue();
      trn3.QueueNext(Wait.GetOrCreate(9.1f));
      trn3.QueueNext(HyperspaceOut.GetOrCreate());
      trn3.MoveData.MaxSpeed = 400;
      trn3.MoveData.Speed = 400;

      player.MoveData.Speed = 400;
      player.MoveData.ResetTurn();
      player.ForceClearQueue();
      player.QueueNext(Lock.GetOrCreate());
      

      int en_ship = 0;
      foreach (int actorID in MainEnemyFaction.GetShips())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (en_ship == 1)
          {
            actor.Position = new TV_3DVECTOR(0, -300, 2500);
            actor.Rotation = new TV_3DVECTOR(0, 180, 0);
            actor.MoveData.Speed = actor.MoveData.MaxSpeed * 0.25f;
            actor.ForceClearQueue();
            actor.QueueNext(Wait.GetOrCreate(60));
            actor.QueueNext(Rotate.GetOrCreate(new TV_3DVECTOR(-2000, -300, 2000), 0, -1, false));
            actor.QueueNext(Lock.GetOrCreate());
          }
          else if (en_ship == 2)
          {
            actor.Position = new TV_3DVECTOR(3300, 150, 5500);
            actor.LookAt(new TV_3DVECTOR(1400, 150, 1000));
            actor.MoveData.Speed = actor.MoveData.MaxSpeed * 0.25f;
            actor.ForceClearQueue();
            actor.QueueNext(Wait.GetOrCreate(30));
            actor.QueueNext(Rotate.GetOrCreate(new TV_3DVECTOR(-32000, 150, 2000), 0, -1, false));
            actor.QueueNext(Lock.GetOrCreate());
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
        Type = ActorTypeFactory.Get("IMPL"),
        Name = "",
        SidebarName = "",
        SpawnTime = Game.GameTime + 9,
        Faction = MainEnemyFaction,
        Position = new TV_3DVECTOR(20000, -2000, -22000),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { HyperspaceIn.GetOrCreate(new TV_3DVECTOR(2000, 100, -8000))
                                    , Move.GetOrCreate(new TV_3DVECTOR(1000, 100, 2000), ActorTypeFactory.Get("IMPL").MoveLimitData.MaxSpeed * 0.25f, -1, false)
                                    , Rotate.GetOrCreate(new TV_3DVECTOR(2000, 100, -9000), 0, -1, false)
                                    , Lock.GetOrCreate() },
        Registries = null
      };
      ActorInfo newDest = asi.Spawn(this);
      newDest.SetArmor(DamageType.ALL, 0.1f);

      asi.SpawnTime = Game.GameTime + 9.25f;
      asi.Position = new TV_3DVECTOR(20000, -2000, -25000);
      asi.Actions = new ActionInfo[] { HyperspaceIn.GetOrCreate(new TV_3DVECTOR(1500, -100, -10200))
                                            , Move.GetOrCreate(new TV_3DVECTOR(-6500, -100, 4000), ActorTypeFactory.Get("IMPL").MoveLimitData.MaxSpeed * 0.25f, -1, false)
                                            , Rotate.GetOrCreate(new TV_3DVECTOR(2000, -100, -10200), 0, -1, false)
                                            , Lock.GetOrCreate() };
      newDest = asi.Spawn(this);
      newDest.SetArmor(DamageType.ALL, 0.1f);
    }

    public void Scene_02b_LightspeedFail()
    {
      EventQueue.Add(Game.GameTime + 1, Message_16);
      EventQueue.Add(Game.GameTime + 5, Message_17);
      EventQueue.Add(Game.GameTime + 12, Scene_02_ViolentShake);
      EventQueue.Add(Game.GameTime + 15, Message_18);
      EventQueue.Add(Game.GameTime + 18, Message_19);
      EventQueue.Add(Game.GameTime + 23, Message_20);
      EventQueue.Add(Game.GameTime + 28, Message_21);
      EventQueue.Add(Game.GameTime + 33, Message_22);
      //EventQueue.Add(Game.GameTime + 33, "Scene_03");

      State.MinBounds = new TV_3DVECTOR(-15000, -1500, -30000);
      State.MinAIBounds = new TV_3DVECTOR(-15000, -1500, -30000);

      SoundManager.SetMusic("battle_2_3");
      SoundManager.SetMusicLoop("battle_2_3");
    }

    public void Scene_02_ViolentShake()
    {
      PlayerCameraInfo.Shake(75);
    }

    #endregion

    #region Text
    COLOR color_echobase = new COLOR(0.6f, 0.6f, 0.9f, 1);
    COLOR color_transport = new COLOR(0.8f, 0.4f, 0.4f, 1);
    COLOR color_xwing = new COLOR(0.8f, 0, 0, 1);
    COLOR color_solo = new COLOR(0.8f, 0.8f, 0.9f, 1);
    COLOR color_3cpo = new COLOR(0.8f, 0.8f, 0.1f, 1);

    public void Message_01_Leaving()
    {
      Screen2D.MessageText("ECHO BASE: Escort the transports to the designated locations for hyperspace jump. ", 5, color_echobase);
    }

    public void Message_02_Conditions()
    {
      Screen2D.MessageText("ECHO BASE: All transports must survive.", 5, color_echobase);
    }

    public void Message_03_Turbolasers()
    {
      Screen2D.MessageText("TRANSPORT: The Heavy Turbolaser Towers on the Star Destroyers must be taken out.", 5, color_transport);
    }

    public void Message_04_TIEs()
    {
      Screen2D.MessageText("X-WING: We will need to worry about the TIEs too.", 5, color_xwing);
    }

    public void Message_05()
    {
      Screen2D.MessageText("SOLO: We will figure something out.", 5, color_solo);
    }

    public void Message_06()
    {
      Screen2D.MessageText("ECHO BASE: Ion Control, standby.", 5, color_echobase);
    }

    public void Message_07()
    {
      Screen2D.MessageText("ECHO BASE: Fire.", 5, color_echobase);
    }

    public void Message_08()
    {
      Screen2D.MessageText("SOLO: Here's our opportunity.", 5, color_solo);
    }

    public void Message_09()
    {
      Screen2D.MessageText("X-WING: We will take care of the fighters.", 5, color_xwing);
    }

    public void Message_10()
    {
      Screen2D.MessageText("X-WING: We need someone to take out the Heavy Turbolasers.", 5, color_xwing);
    }

    public void Message_11()
    {
      Screen2D.MessageText("SOLO: I can take care of that.", 5, color_solo);
    }

    public void Message_12()
    {
      Screen2D.MessageText("ECHO BASE: More Star Destroyers incoming.", 5, color_echobase);
    }

    public void Message_13()
    {
      Screen2D.MessageText("SOLO: I see them. Two Star Destroyers here coming right at us.", 5, color_solo);
    }

    public void Message_14()
    {
      Screen2D.MessageText("SOLO: [Use the secondary weapon toggle to switch between front and rear deflector shields.]", 5, color_solo);
    }

    public void Message_15()
    {
      Screen2D.MessageText("SOLO: We can still out-manuever them.", 5, color_solo);
    }

    public void Message_16()
    {
      Screen2D.MessageText("SOLO: Prepare to make the jump to lightspeed.", 5, color_solo);
    }

    public void Message_17()
    {
      Screen2D.MessageText("SOLO: Watch this!", 5, color_solo);
    }

    public void Message_18()
    {
      Screen2D.MessageText("SOLO: ...", 5, color_solo);
    }

    public void Message_19()
    {
      Screen2D.MessageText("SOLO: I think we are in trouble.", 5, color_solo);
    }

    public void Message_20()
    {
      Screen2D.MessageText("C3PO: The hyperdrive modulator has been damaged, sir.", 5, color_3cpo);
    }

    public void Message_21()
    {
      Screen2D.MessageText("C3PO: It is impossible to jump to lightspeed.", 5, color_3cpo);
    }

    public void Message_22()
    {
      Screen2D.MessageText("SOLO: We are in trouble!", 5, color_solo);
    }

    public void Message_23()
    {
      Screen2D.MessageText("LEIA: Han, get up here!", 5, color_transport);
    }
    #endregion
  }
}
