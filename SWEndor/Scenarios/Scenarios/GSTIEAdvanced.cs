using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Sound;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSTIEAdvanced : GameScenarioBase
  {
    public GSTIEAdvanced()
    {
      Name = "TIE Advanced Challenge";
      AllowedWings = new List<ActorTypeInfo> { XWingATI.Instance()
                                               , AWingATI.Instance()
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
      PlayerInfo.Instance().Name = "Red Two";
    }

    public override void Launch()
    {
      base.Launch();

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(15000, 1500, 15000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-15000, -1500, -15000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(10000, 1500, 10000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-10000, -1500, -10000);
      PlayerInfo.Instance().Lives = 4;
      PlayerInfo.Instance().ScorePerLife = 200000;
      PlayerInfo.Instance().ScoreForNextLife = 200000;

      MakePlayer = Rebel_MakePlayer;

      if (!GameScenarioManager.Instance().GetGameStateB("rebels_arrive"))
      {
        GameScenarioManager.Instance().SetGameStateB("rebels_arrive", true);

        SoundManager.Instance().SetMusic("battle_1_1");
        SoundManager.Instance().SetMusicLoop("battle_3_3");

        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, Rebel_HyperspaceIn);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 2.5f, Empire_Wave_2);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3.5f, Rebel_MakePlayer);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, Rebel_RemoveTorps);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7.5f, Rebel_GiveControl);
      }

      GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().GameTime || Game.Instance().GameTime % 0.2f > 0.1f)
      {
        GameScenarioManager.Instance().Line1Text = string.Format("WINGS: {0}", MainAllyFaction.GetWings().Count);
      }
      else
      {
        GameScenarioManager.Instance().Line1Text = string.Format("");
      }

      int tie_d = 0;
      int tie_sa = 0;
      foreach (int actorID in MainEnemyFaction.GetWings())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
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
        GameScenarioManager.Instance().Line2Text = string.Format("TIE/D: {0}", tie_d);
        if (tie_sa > 0)
        {
          GameScenarioManager.Instance().Line3Text = string.Format("TIE/SA:{0}", tie_sa);
        }
        else
        {
          GameScenarioManager.Instance().Line3Text = "";
        }
      }
      else if (tie_sa > 0)
      {
        GameScenarioManager.Instance().Line2Text = string.Format("TIE/SA:{0}", tie_sa);
        GameScenarioManager.Instance().Line3Text = "";
      }
      else
      {
        GameScenarioManager.Instance().Line2Text = "";
        GameScenarioManager.Instance().Line3Text = "";
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

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorInfo ainfo;
      TV_3DVECTOR pos;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, 5000);
      float creationTime = Game.Instance().GameTime;

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(350, 100, 1300);

      // Player X-Wing
      pos = new TV_3DVECTOR(0, 0, -150);

      creationTime += 0.025f;
      ainfo = new ActorSpawnInfo
      {
        Type = TIE_X1_ATI.Instance(),
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

      GameScenarioManager.Instance().CameraTargetActor = ainfo;
      PlayerInfo.Instance().TempActorID = ainfo.ID;

      // Wings x(45-1)
      List<TV_3DVECTOR> positions = new List<TV_3DVECTOR>();
      for (int i = 0; i < 24; i++)
      {
        if (i % 2 == 1)
          positions.Add(new TV_3DVECTOR(Engine.Instance().Random.Next(-1800, -80), Engine.Instance().Random.Next(-100, 100), Engine.Instance().Random.Next(-2400, 150)));
        else
          positions.Add(new TV_3DVECTOR(Engine.Instance().Random.Next(80, 1800), Engine.Instance().Random.Next(-100, 100), Engine.Instance().Random.Next(-2400, 150)));
      }

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        ActorTypeInfo[] atypes = new ActorTypeInfo[] { XWingATI.Instance()
                                                      , XWingATI.Instance()
                                                      , XWingATI.Instance()
                                                      , AWingATI.Instance()
                                                      , AWingATI.Instance()
                                                      };

        creationTime += 0.025f;

        ainfo = new ActorSpawnInfo
        {
          Type = atypes[Engine.Instance().Random.Next(0, atypes.Length)],
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
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 8000);

        ainfo = new ActorSpawnInfo
        {
          Type = CorellianATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "CORELLIAN",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + hyperspaceInOffset,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new HyperspaceIn(v)
                                           , new Lock()
                                           , new Move(nv, ainfo.MovementInfo.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), ainfo.MovementInfo.MinSpeed)
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
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 8000);

        ainfo = new ActorSpawnInfo
        {
          Type = TransportATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "TRANSPORT",
          SpawnTime = creationTime,
          Faction = MainAllyFaction,
          Position = v + hyperspaceInOffset,
          Rotation = new TV_3DVECTOR(),
          Actions = new ActionInfo[] { new HyperspaceIn(v)
                                           , new Lock()
                                           , new Move(nv, ainfo.MovementInfo.MaxSpeed)
                                           , new Rotate(nv - new TV_3DVECTOR(0, 0, 20000), ainfo.MovementInfo.MinSpeed)
                                           , new Lock() },
          Registries = new string[] { "CriticalAllies" }
        }.Spawn(this);
      }
    }

    public void Rebel_RemoveTorps(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetWings())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          actor.WeaponSystemInfo.SecondaryWeapons = new string[] { "none" };
          actor.WeaponSystemInfo.AIWeapons = new string[] { "1:laser" };
        }
      }
      PlayerInfo.Instance().ResetPrimaryWeapon();
      PlayerInfo.Instance().ResetSecondaryWeapon();
    }

    public void Rebel_MakePlayer(object[] param)
    {
      PlayerInfo.Instance().ActorID = PlayerInfo.Instance().TempActorID;

      if (PlayerInfo.Instance().Actor == null || PlayerInfo.Instance().Actor.CreationState == CreationState.DISPOSED)
      {
        if (PlayerInfo.Instance().Lives > 0)
        {
          PlayerInfo.Instance().Lives--;
          TV_3DVECTOR pos = new TV_3DVECTOR(0, -200, 500);
          if (MainAllyFaction.GetShips().Count > 0)
          {
            ActorInfo rs = ActorInfo.Factory.Get(MainAllyFaction.GetShips()[0]);
            if (rs != null)
              pos += rs.GetPosition();
          }

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = PlayerInfo.Instance().ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Game.Instance().GameTime,
            Faction = MainAllyFaction,
            Position = pos,
            Rotation = new TV_3DVECTOR(),
            Actions = null,
            Registries = null
          }.Spawn(this);

          PlayerInfo.Instance().ActorID = ainfo.ID;
        }
      }
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, Rebel_RemoveTorps);
    }

    public void Rebel_GiveControl(object[] param)
    {
      foreach (int actorID in MainAllyFaction.GetAll())
      {
        ActorInfo actor = ActorInfo.Factory.Get(actorID);
        if (actor != null)
        {
          ActionManager.UnlockOne(actorID);
          actor.ActorState = ActorState.NORMAL;
          actor.MovementInfo.Speed = actor.MovementInfo.MaxSpeed;
        }
      }

      PlayerInfo.Instance().IsMovementControlsEnabled = true;

      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
      Empire_TIEAdvanced(null);
    }

    #endregion


    #region Empire spawns

    public void Empire_Wave_2(object[] param)
    {
      switch (Difficulty.ToLower())
      {
        case "mental":
          Empire_TIEDefender_Wave(new object[] { 30 });
          Empire_TIEBombers(new object[] { 4 });
          break;
        case "hard":
          Empire_TIEDefender_Wave(new object[] { 21 });
          Empire_TIEBombers(new object[] { 2 });
          break;
        case "normal":
          Empire_TIEDefender_Wave(new object[] { 15 });
          Empire_TIEBombers(new object[] { 1 });
          break;
        case "easy":
        default:
          Empire_TIEDefender_Wave(new object[] { 8 });
          break;
      }
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, Empire_TIEAdv_Control_TargetFighter);
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, Empire_TIEAdv_Control_Master);
    }

    public void Empire_TIEDefender_Wave(object[] param)
    {
      int sets = 5;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets)) 
        sets = 5;

      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        ActionInfo[] actions = null;
        switch (Difficulty.ToLower())
        {
          case "mental":
          case "hard":
            actions = new ActionInfo[] { new Hunt(TargetType.FIGHTER)};
            break;
        }

        ActorInfo ainfo = new ActorSpawnInfo
        {
          Type = TIE_D_ATI.Instance(),
          Name = "",
          RegisterName = "",
          SidebarName = "",
          SpawnTime = Game.Instance().GameTime + t,
          Faction = MainEnemyFaction,
          Position = new TV_3DVECTOR(fx, fy, -22500),
          Rotation = new TV_3DVECTOR(),
          Actions = actions ,
          Registries = null
        }.Spawn(this);

        t += 0.5f;
      }
    }

    public void Empire_TIEBombers(object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE Bombers only
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
              Type = TIE_sa_ATI.Instance(),
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Game.Instance().GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, -22500),
              Rotation = new TV_3DVECTOR(),
              Actions = new ActionInfo[] { new Hunt(TargetType.SHIP) },
              Registries = null
            }.Spawn(this);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEAdv_Control_Master(object[] param)
    {
      double r = Engine.Instance().Random.NextDouble();

      if (r * (MainAllyFaction.GetWings().Count + MainAllyFaction.GetShips().Count * 5) < MainAllyFaction.GetWings().Count)
      {
        Empire_TIEAdvanced_Control_AttackFighter(null);
      }
      else
      {
        Empire_TIEAdvanced_Control_AttackShip(null);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 125f, Empire_TIEAdv_Control_TargetShip);
      }
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 30f, Empire_TIEAdv_Control_Master);
    }

    public void Empire_TIEAdvanced(object[] param)
    {
      // TIE Advanced x 1
      float fx = Engine.Instance().Random.Next(-2500, 2500);
      float fy = Engine.Instance().Random.Next(-500, 500);

      m_X1ID = new ActorSpawnInfo
      {
        Type = TIE_X1_ATI.Instance(),
        Name = "",
        RegisterName = "",
        SidebarName = "TIE ADV. X1",
        SpawnTime = Game.Instance().GameTime,
        Faction = FactionInfo.Factory.Get("Empire_Advanced"),
        Position = new TV_3DVECTOR(fx, fy, -22500),
        Rotation = new TV_3DVECTOR(),
        Actions = new ActionInfo[] { new Hunt(TargetType.SHIP) },
        Registries = new string[] { "CriticalEnemies" }
      }.Spawn(this).ID;

      Screen2D.Instance().MessageText("The TIE Advanced X1 has arrived.", 5, new TV_COLOR(1, 1, 1, 1));
    }

    public void Empire_TIEAdv_Control_TargetFighter(object[] param)
    {
      Empire_TIEAdvanced_Control_AttackFighter(null);
    }

    public void Empire_TIEAdv_Control_TargetShip(object[] param)
    {
      Empire_TIEAdvanced_Control_AttackShip(null);
    }

    public void Empire_TIEAdvanced_Control_AttackShip(object[] param)
    {
      ActionManager.ForceClearQueue(m_X1ID);
      ActionManager.QueueLast(m_X1ID, new Hunt(TargetType.SHIP));
    }

    public void Empire_TIEAdvanced_Control_AttackFighter(object[] param)
    {
      ActionManager.ForceClearQueue(m_X1ID);
      ActionManager.QueueLast(m_X1ID, new Hunt(TargetType.FIGHTER));
    }

    #endregion
  }
}
