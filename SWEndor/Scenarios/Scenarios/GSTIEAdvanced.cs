using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI.Actions;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSTIEAdvanced : GameScenarioBase
  {
    public GSTIEAdvanced(GameScenarioManager manager) : base(manager)
    {
      Name = "TIE Advanced Challenge";
      AllowedWings = new List<ActorTypeInfo> { Engine.ActorTypeFactory.Get("X-Wing")
                                               , Engine.ActorTypeFactory.Get("A-Wing")
                                              };

      AllowedDifficulties = new List<string> { "easy"
                                               , "normal"
                                               , "hard"
                                               , "MENTAL"
                                              };
    }

    private int m_X1ID = -1;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Name = "Red Two";
    }

    public override void Launch()
    {
      base.Launch();

      Manager.MaxBounds = new TV_3DVECTOR(15000, 1500, 15000);
      Manager.MinBounds = new TV_3DVECTOR(-15000, -1500, -15000);
      Manager.MaxAIBounds = new TV_3DVECTOR(10000, 1500, 10000);
      Manager.MinAIBounds = new TV_3DVECTOR(-10000, -1500, -10000);
      PlayerInfo.Lives = 4;
      PlayerInfo.ScorePerLife = 200000;
      PlayerInfo.ScoreForNextLife = 200000;

      MakePlayer = Rebel_MakePlayer;

      if (!Manager.GetGameStateB("rebels_arrive"))
      {
        Manager.SetGameStateB("rebels_arrive", true);

        SoundManager.SetMusic("battle_1_1");
        SoundManager.SetMusicLoop("battle_3_3");

        Manager.AddEvent(Game.GameTime + 0.1f, Rebel_HyperspaceIn);
        Manager.AddEvent(Game.GameTime + 2.5f, Empire_Wave_2);
        Manager.AddEvent(Game.GameTime + 3.5f, Rebel_MakePlayer);
        Manager.AddEvent(Game.GameTime + 5f, Rebel_RemoveTorps);
        Manager.AddEvent(Game.GameTime + 7.5f, Rebel_GiveControl);
      }

      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      Manager.Line2Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      Manager.Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      Manager.IsCutsceneMode = false;
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      if (Manager.Scenario.TimeSinceLostWing < Game.GameTime || Game.GameTime % 0.2f > 0.1f)
      {
        Manager.Line1Text = string.Format("WINGS: {0}", MainAllyFaction.GetWings().Count);
      }
      else
      {
        Manager.Line1Text = string.Format("");
      }

      int tie_d = 0;
      int tie_sa = 0;
      foreach (int actorID in MainEnemyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          if (actor.TypeInfo is TIE_D_ATI)
            tie_d++;
          else if (actor.TypeInfo is TIE_sa_ATI)
            tie_sa++;
        }
      }

      if (tie_d > 0)
      {
        Manager.Line2Text = string.Format("TIE/D: {0}", tie_d);
        if (tie_sa > 0)
        {
          Manager.Line3Text = string.Format("TIE/SA:{0}", tie_sa);
        }
        else
        {
          Manager.Line3Text = "";
        }
      }
      else if (tie_sa > 0)
      {
        Manager.Line2Text = string.Format("TIE/SA:{0}", tie_sa);
        Manager.Line3Text = "";
      }
      else
      {
        Manager.Line2Text = "";
        Manager.Line3Text = "";
      }
    }


    private void CalibrateSceneObjects()
    {
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Rebels_Falcon", new TV_COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire_Advanced", new TV_COLOR(0.4f, 0.8f, 0.4f, 1)).AutoAI = true;

      FactionInfo.Factory.Get("Rebels").Allies.Add(FactionInfo.Factory.Get("Rebels_Falcon"));
      FactionInfo.Factory.Get("Rebels_Falcon").Allies.Add(FactionInfo.Factory.Get("Rebels"));

      FactionInfo.Factory.Get("Empire").Allies.Add(FactionInfo.Factory.Get("Empire_Advanced"));
      FactionInfo.Factory.Get("Empire_Advanced").Allies.Add(FactionInfo.Factory.Get("Empire"));

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");
    }

    public override void LoadScene()
    {
      base.LoadScene();
    }


    #region Rebellion spawns

    public void Rebel_HyperspaceIn(GameEventArg arg)
    {
      ActorInfo ainfo;
      TV_3DVECTOR pos;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, 5000);
      float creationTime = Game.GameTime;

      PlayerCameraInfo.Position = new TV_3DVECTOR(350, 100, 1300);

      // Player X-Wing
      pos = new TV_3DVECTOR(0, 0, -150);

      creationTime += 0.025f;
      ainfo = new ActorSpawnInfo
      {
        Type = Engine.ActorTypeFactory.Get("TIE Advanced X1"),
        Name = "(Player)",
        RegisterName = "",
        SidebarName = "",
        SpawnTime = creationTime,
        Faction = MainAllyFaction,
        Position = pos + hyperspaceInOffset,
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new HyperspaceIn(pos)
                                           , new Lock() },
        Registries = null
      }.Spawn(this);

      PlayerCameraInfo.LookAtActor = ainfo.ID;
      PlayerInfo.TempActorID = ainfo.ID;

      // Wings x(45-1)
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      for (int i = 0; i < 24; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(-1800, -80), Engine.Random.Next(-100, 100), Engine.Random.Next(-2400, 150)));
        else
          positions.Add(new TV_3DVECTOR(Engine.Random.Next(80, 1800), Engine.Random.Next(-100, 100), Engine.Random.Next(-2400, 150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        ActorTypeInfo[] atypes = new ActorTypeInfo[] { Engine.ActorTypeFactory.Get("X-Wing")
                                                      , Engine.ActorTypeFactory.Get("X-Wing")
                                                      , Engine.ActorTypeFactory.Get("X-Wing")
                                                      , Engine.ActorTypeFactory.Get("A-Wing")
                                                      , Engine.ActorTypeFactory.Get("A-Wing")
                                                      };

        creationTime += 0.025f;

        ainfo = new ActorSpawnInfo
        {
          Type = atypes[Engine.Random.Next(0, atypes.Length)],
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + hyperspaceInOffset,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new HyperspaceIn(v)
                                           , new Lock()
                                           , new Wait(15f) },
          Registries = null
        }.Spawn(this);
      }

      // Corellian x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-100, 220, 200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        creationTime += 0.025f;
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), -v.z - 8000);

        ainfo = new ActorSpawnInfo
        {
          Type = Engine.ActorTypeFactory.Get("Corellian Corvette"),
          Name = "",
          RegisterName = "",
          SidebarName = "CORELLIAN",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + hyperspaceInOffset,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new HyperspaceIn(v)
                                           , new Lock()
                                           , new Move(nv, ainfo.MoveData.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), ainfo.MoveData.MinSpeed)
                                           , new Lock() },
          Registries = new string[] { "CriticalAllies" }
        }.Spawn(this);
      }

      // Transport x2
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-800, -120, 600));
      positions.Add(new TV_3DVECTOR(600, -320, 400));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];

        creationTime += 0.025f;
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Random.Next(-5, 5), v.y + Engine.Random.Next(-5, 5), -v.z - 8000);

        ainfo = new ActorSpawnInfo
        {
          Type = Engine.ActorTypeFactory.Get("Transport"),
          Name = "",
          RegisterName = "",
          SidebarName = "TRANSPORT",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + hyperspaceInOffset,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new HyperspaceIn(v)
                                           , new Lock()
                                           , new Move(nv, ainfo.MoveData.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), ainfo.MoveData.MinSpeed)
                                           , new Lock() },
          Registries = new string[] { "CriticalAllies" }
        }.Spawn(this);
      }
    }

    public void Rebel_RemoveTorps(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          actor.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
          actor.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
        }
      }
      PlayerInfo.ResetPrimaryWeapon();
      PlayerInfo.ResetSecondaryWeapon();
    }

    public void Rebel_MakePlayer(GameEventArg arg)
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;

      if (PlayerInfo.Actor == null || PlayerInfo.Actor.CreationState == CreationState.DISPOSED)
      {
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;
          TV_3DVECTOR pos = new TV_3DVECTOR(0, -200, 500);
          if (MainAllyFaction.GetShips().Count > 0)
          {
            ActorInfo rs = Engine.ActorFactory.Get(MainAllyFaction.GetShips()[0]);
            if (rs != null)
              pos += rs.GetPosition();
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
            Rotation = new TV_3DVECTOR(),
            Actions = null,
            Registries = null
          }.Spawn(this);

          PlayerInfo.ActorID = ainfo.ID;
        }
      }
      Manager.AddEvent(Game.GameTime + 0.1f, Rebel_RemoveTorps);
    }

    public void Rebel_GiveControl(GameEventArg arg)
    {
      foreach (int actorID in MainAllyFaction.GetAll())
      {
        ActorInfo actor = Engine.ActorFactory.Get(actorID);
        if (actor != null)
        {
          Engine.ActionManager.UnlockOne(actorID);
          actor.ActorState = ActorState.NORMAL;
          actor.MoveData.Speed = actor.MoveData.MaxSpeed;
        }
      }

      PlayerInfo.IsMovementControlsEnabled = true;

      Manager.SetGameStateB("in_battle", true);
      Empire_TIEAdvanced(null);
    }

    #endregion


    #region Empire spawns

    public void Empire_Wave_2(GameEventArg arg)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
          Manager.AddEvent(0, Empire_TIEDefender_Wave, IntegerEventArg.N10);
          Manager.AddEvent(0, Empire_TIEBombers, IntegerEventArg.N4);
          break;
        case "hard":
          Manager.AddEvent(0, Empire_TIEDefender_Wave, IntegerEventArg.N7);
          Manager.AddEvent(0, Empire_TIEBombers, IntegerEventArg.N2);
          break;
        case "normal":
          Manager.AddEvent(0, Empire_TIEDefender_Wave, IntegerEventArg.N5);
          Manager.AddEvent(0, Empire_TIEBombers, IntegerEventArg.N1);
          break;
        case "easy":
        default:
          Manager.AddEvent(0, Empire_TIEDefender_Wave, IntegerEventArg.N8);
          break;
      }
      Manager.AddEvent(Game.GameTime + 5f, Empire_TIEAdv_Control_TargetFighter);
      Manager.AddEvent(Game.GameTime + 15f, Empire_TIEAdv_Control_Master);
    }

    public void Empire_TIEDefender_Wave(GameEventArg arg)
    {
      int sets = ((IntegerEventArg)arg).Num;
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, -22500), new TV_3DVECTOR(2500, 500, -22500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE Defender")
                                                                          , MainEnemyFaction
                                                                          , 3
                                                                          , 8
                                                                          , TargetType.FIGHTER
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VSHAPE
                                                                          , new TV_3DVECTOR()
                                                                          , 1000
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 0.5f, spawninfo);
    }

    public void Empire_TIEBombers(GameEventArg arg)
    {
      int sets = ((IntegerEventArg)arg).Num;
      GSFunctions.BoxInfo box = new GSFunctions.BoxInfo(new TV_3DVECTOR(-2500, -500, -22500), new TV_3DVECTOR(2500, 500, -22500));
      GSFunctions.SquadSpawnInfo spawninfo = new GSFunctions.SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("TIE Bomber")
                                                                          , MainEnemyFaction
                                                                          , 4
                                                                          , 18
                                                                          , TargetType.SHIP
                                                                          , false
                                                                          , GSFunctions.SquadFormation.VERTICAL_SQUARE
                                                                          , new TV_3DVECTOR()
                                                                          , 200
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, sets, box, 1.5f, spawninfo);
    }

    public void Empire_TIEAdv_Control_Master(GameEventArg arg)
    {
      double r = Engine.Random.NextDouble();

      if (r * (MainAllyFaction.GetWings().Count + MainAllyFaction.GetShips().Count * 5) < MainAllyFaction.GetWings().Count)
      {
        Empire_TIEAdvanced_Control_AttackFighter(null);
      }
      else
      {
        Empire_TIEAdvanced_Control_AttackShip(null);
        Manager.AddEvent(Game.GameTime + 125f, Empire_TIEAdv_Control_TargetShip);
      }
      Manager.AddEvent(Game.GameTime + 30f, Empire_TIEAdv_Control_Master);
    }

    public void Empire_TIEAdvanced(GameEventArg arg)
    {
      // TIE Advanced x 1
      float fx = Engine.Random.Next(-2500, 2500);
      float fy = Engine.Random.Next(-500, 500);

      m_X1ID = new ActorSpawnInfo
      {
        Type = Engine.ActorTypeFactory.Get("TIE Advanced X1"),
        Name = "",
        RegisterName = "",
        SidebarName = "TIE ADV. X1",
        SpawnTime = Game.GameTime,
        Faction = FactionInfo.Factory.Get("Empire_Advanced"),
        Position = new TV_3DVECTOR(fx, fy, -22500),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new Hunt(TargetType.SHIP) },
        Registries = new string[] { "CriticalEnemies" }
      }.Spawn(this).ID;

      Screen2D.MessageText("The TIE Advanced X1 has arrived.", 5, new TV_COLOR(1, 1, 1, 1));
    }

    public void Empire_TIEAdv_Control_TargetFighter(GameEventArg arg)
    {
      Empire_TIEAdvanced_Control_AttackFighter(null);
    }

    public void Empire_TIEAdv_Control_TargetShip(GameEventArg arg)
    {
      Empire_TIEAdvanced_Control_AttackShip(null);
    }

    public void Empire_TIEAdvanced_Control_AttackShip(GameEventArg arg)
    {
      Engine.ActionManager.ForceClearQueue(m_X1ID);
      Engine.ActionManager.QueueLast(m_X1ID, new Hunt(TargetType.SHIP));
    }

    public void Empire_TIEAdvanced_Control_AttackFighter(GameEventArg arg)
    {
      Engine.ActionManager.ForceClearQueue(m_X1ID);
      Engine.ActionManager.QueueLast(m_X1ID, new Hunt(TargetType.FIGHTER));
    }

    #endregion
  }
}
