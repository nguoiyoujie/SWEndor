using MTV3D65;
using System;
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

    private ActorInfo m_AScene = null;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      PlayerInfo.Instance().Name = "Red Two";

      if (GameScenarioManager.Instance().GetGameStateB("in_game"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_game", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(15000, 1500, 15000);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-15000, -1500, -15000);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(10000, 1500, 10000);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-10000, -1500, -10000);
      PlayerInfo.Instance().Lives = 4;
      PlayerInfo.Instance().ScorePerLife = 200000;
      PlayerInfo.Instance().ScoreForNextLife = 200000;
      PlayerInfo.Instance().Score.Reset();

      MakePlayer = Rebel_MakePlayer;

      if (!GameScenarioManager.Instance().GetGameStateB("rebels_arrive"))
      {
        GameScenarioManager.Instance().SetGameStateB("rebels_arrive", true);

        SoundManager.Instance().SetMusic("battle_1_1");
        SoundManager.Instance().SetMusicLoop("battle_3_3");

        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_HyperspaceIn");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 2.5f, "Empire_Wave_2");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3.5f, "Rebel_MakePlayer");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Rebel_RemoveTorps");
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 7.5f, "Rebel_GiveControl");
      }

      GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line2Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);
      GameScenarioManager.Instance().Line3Color = new TV_COLOR(0.7f, 1f, 0.3f, 1);

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public override void RegisterEvents()
    {
      base.RegisterEvents();
      GameEvent.RegisterEvent("Rebel_HyperspaceIn", Rebel_HyperspaceIn);
      GameEvent.RegisterEvent("Rebel_MakePlayer", Rebel_MakePlayer);
      GameEvent.RegisterEvent("Rebel_RemoveTorps", Rebel_RemoveTorps);
      GameEvent.RegisterEvent("Rebel_GiveControl", Rebel_GiveControl);

      GameEvent.RegisterEvent("Empire_TIEAdvanced", Empire_TIEAdvanced);
      GameEvent.RegisterEvent("Empire_TIEAdvanced_Control_AttackFighter", Empire_TIEAdvanced_Control_AttackFighter);
      GameEvent.RegisterEvent("Empire_TIEAdvanced_Control_AttackShip", Empire_TIEAdvanced_Control_AttackShip);
      GameEvent.RegisterEvent("Empire_TIEAdv_Control_Master", Empire_TIEAdv_Control_Master);
      GameEvent.RegisterEvent("Empire_TIEAdv_Control_TargetFighter", Empire_TIEAdv_Control_TargetFighter);
      GameEvent.RegisterEvent("Empire_TIEAdv_Control_TargetShip", Empire_TIEAdv_Control_TargetShip);
      GameEvent.RegisterEvent("Empire_TIEBombers", Empire_TIEBombers);
      GameEvent.RegisterEvent("Empire_TIEWave_03", Empire_TIEDefender_Wave);
      GameEvent.RegisterEvent("Empire_Wave_2", Empire_Wave_2);
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      if (GameScenarioManager.Instance().Scenario.TimeSinceLostWing < Game.Instance().GameTime || Game.Instance().GameTime % 0.4f > 0.2f)
      {
        GameScenarioManager.Instance().Line1Text = string.Format("WINGS: {0}", GameScenarioManager.Instance().Scenario.RebelFighterLimit);
      }
      else
      {
        GameScenarioManager.Instance().Line1Text = string.Format("");
      }

      int tie_d = 0;
      int tie_sa = 0;
      foreach (ActorInfo a in GameScenarioManager.Instance().EnemyFighters.Values)
      {
        if (a.TypeInfo is TIE_D_ATI)
        {
          tie_d++;
        }
        else if (a.TypeInfo is TIE_sa_ATI)
        {
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
      if (m_AScene != null && m_AScene.CreationState == CreationState.ACTIVE)
      {
        m_AScene.SetLocalPosition(PlayerInfo.Instance().Position.x, PlayerInfo.Instance().Position.y, PlayerInfo.Instance().Position.z);
      }
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.AddFaction("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Rebels_Falcon", new TV_COLOR(0.8f, 0.8f, 0.8f, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire_Advanced", new TV_COLOR(0.4f, 0.8f, 0.4f, 1)).AutoAI = true;

      FactionInfo.Get("Rebels").Allies.Add(FactionInfo.Get("Rebels_Falcon"));
      FactionInfo.Get("Rebels_Falcon").Allies.Add(FactionInfo.Get("Rebels"));

      FactionInfo.Get("Empire").Allies.Add(FactionInfo.Get("Empire_Advanced"));
      FactionInfo.Get("Empire_Advanced").Allies.Add(FactionInfo.Get("Empire"));
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
    }


    #region Rebellion spawns

    public void Rebel_HyperspaceIn(object[] param)
    {
      ActorCreationInfo acinfo;
      ActorInfo ainfo;
      TV_3DVECTOR pos;
      TV_3DVECTOR hyperspaceInOffset = new TV_3DVECTOR(0, 0, 5000);
      float creationTime = Game.Instance().GameTime;

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(350, 100, 1300);

      // Player X-Wing
      pos = new TV_3DVECTOR(0, 0, -150);
      acinfo = new ActorCreationInfo(PlayerInfo.Instance().ActorType);
      acinfo.Name = "(Player)";
      acinfo.CreationTime = creationTime;
      acinfo.Faction = FactionInfo.Get("Rebels");
      acinfo.InitialState = ActorState.NORMAL;
      creationTime += 0.025f;
      acinfo.CreationTime = creationTime;
      acinfo.Position = pos + hyperspaceInOffset;
      acinfo.Rotation = new TV_3DVECTOR();
      ainfo = ActorInfo.Create(acinfo);
      GameScenarioManager.Instance().AllyFighters.Add(ainfo.Name, ainfo);
      RegisterEvents(ainfo);

      ActionManager.QueueNext(ainfo, new Actions.HyperspaceIn(pos));
      ActionManager.QueueNext(ainfo, new Actions.Lock());

      GameScenarioManager.Instance().CameraTargetActor = ainfo;

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

        acinfo = new ActorCreationInfo(atypes[Engine.Instance().Random.Next(0, atypes.Length)]);
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        creationTime += 0.025f;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + hyperspaceInOffset;
        acinfo.Rotation = new TV_3DVECTOR();
        ainfo = ActorInfo.Create(acinfo);

        ActionManager.QueueNext(ainfo, new Actions.HyperspaceIn(v));
        ActionManager.QueueNext(ainfo, new Actions.Lock());
        ActionManager.QueueNext(ainfo, new Actions.Wait(10));

        GameScenarioManager.Instance().AllyFighters.Add(ainfo.Name + " " + i, ainfo);
        RegisterEvents(ainfo);
      }

      // Corellian x1
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-100, 220, 200));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(CorellianATI.Instance());
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        creationTime += 0.025f;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + hyperspaceInOffset;
        acinfo.Rotation = new TV_3DVECTOR();
        ainfo = ActorInfo.Create(acinfo);

        ActionManager.QueueNext(ainfo, new Actions.HyperspaceIn(v));
        ActionManager.QueueNext(ainfo, new Actions.Lock());
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 8000);
        ActionManager.QueueNext(ainfo, new Actions.Move(nv
                                                      , ainfo.MaxSpeed));
        ActionManager.QueueNext(ainfo, new Actions.Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                      , ainfo.MinSpeed));
        ActionManager.QueueNext(ainfo, new Actions.Lock());

        GameScenarioManager.Instance().AllyShips.Add(ainfo.Name.ToUpper() + i, ainfo);
        GameScenarioManager.Instance().CriticalAllies.Add("CORELLIAN     " + i, ainfo);
        RegisterEvents(ainfo);
      }

      // Transport x2
      positions.Clear();
      positions.Add(new TV_3DVECTOR(-800, -120, 600));
      positions.Add(new TV_3DVECTOR(600, -320, 400));

      for (int i = 0; i < positions.Count; i++)
      {
        TV_3DVECTOR v = positions[i];
        acinfo = new ActorCreationInfo(TransportATI.Instance());
        acinfo.Faction = FactionInfo.Get("Rebels");
        acinfo.InitialState = ActorState.NORMAL;
        creationTime += 0.025f;
        acinfo.CreationTime = creationTime;
        acinfo.Position = v + hyperspaceInOffset;
        acinfo.Rotation = new TV_3DVECTOR();
        ainfo = ActorInfo.Create(acinfo);

        ActionManager.QueueNext(ainfo, new Actions.HyperspaceIn(v));
        ActionManager.QueueNext(ainfo, new Actions.Lock());
        TV_3DVECTOR nv = new TV_3DVECTOR(v.x + Engine.Instance().Random.Next(-5, 5), v.y + Engine.Instance().Random.Next(-5, 5), -v.z - 8000);
        ActionManager.QueueNext(ainfo, new Actions.Move(nv
                                                      , ainfo.MaxSpeed));
        ActionManager.QueueNext(ainfo, new Actions.Rotate(nv - new TV_3DVECTOR(0, 0, 20000)
                                                      , ainfo.MinSpeed));
        ActionManager.QueueNext(ainfo, new Actions.Lock());

        GameScenarioManager.Instance().AllyShips.Add(ainfo.Name.ToUpper() + "    " + i, ainfo);
        GameScenarioManager.Instance().CriticalAllies.Add(ainfo.Name.ToUpper() + "    " + i, ainfo);
        RegisterEvents(ainfo);
      }

      RebelFighterLimit = GameScenarioManager.Instance().AllyFighters.Count;
    }

    public void Rebel_RemoveTorps(object[] param)
    {
      foreach (ActorInfo ainfo in GameScenarioManager.Instance().AllyFighters.Values)
      {
        //ainfo.SetStateB("EnableTorp", false);
        //ainfo.SetStateB("EnablePhoton", false);
        //string secondarymode = ainfo.GetStateS("SecondaryWeaponModes");
        //secondarymode = secondarymode.Replace(",1torp", "");
        //secondarymode = secondarymode.Replace(",1photon", "");
        //ainfo.SetStateS("SecondaryWeaponModes", secondarymode);

        ainfo.SecondaryWeapons = new List<string> { "none" };
        ainfo.AIWeapons = new List<string> { "1:laser" };
      }
      PlayerInfo.Instance().ResetPrimaryWeapon();
      PlayerInfo.Instance().ResetSecondaryWeapon();
    }

    public void Rebel_MakePlayer(object[] param)
    {
      if (GameScenarioManager.Instance().AllyFighters.ContainsKey("(Player)"))
      {
        //GameScenarioManager.Instance().AllyFighters["(Player)"].Camera.Destroy();
        //GameScenarioManager.Instance().AllyFighters["(Player)"].Camera = PlayerInfo.Instance().Camera;
        //GameScenarioManager.Instance().SceneCamera.Camera = new TVCamera();
        PlayerInfo.Instance().Actor = GameScenarioManager.Instance().AllyFighters["(Player)"];
      }
      else
      {
        if (PlayerInfo.Instance().Lives > 0)
        {
          PlayerInfo.Instance().Lives--;
          ActorInfo rs = null;
          if (GameScenarioManager.Instance().AllyShips.Count > 0)
          {
            string[] rskeys = new string[GameScenarioManager.Instance().AllyShips.Count];
            GameScenarioManager.Instance().AllyShips.Keys.CopyTo(rskeys, 0);
            rs = GameScenarioManager.Instance().AllyShips[rskeys[0]];
          }
          if (rs != null)
          {
            ActorCreationInfo placi = new ActorCreationInfo(PlayerInfo.Instance().ActorType);
            placi.Name = "(Player)";
            placi.CreationTime = Game.Instance().GameTime;
            placi.Faction = FactionInfo.Get("Rebels");
            placi.InitialState = ActorState.NORMAL;
            placi.Position = rs.GetPosition() + new TV_3DVECTOR(0, -200, 2500);
            placi.Rotation = new TV_3DVECTOR();

            ActorInfo ainfo = ActorInfo.Create(placi);
            PlayerInfo.Instance().Actor = ainfo;
            GameScenarioManager.Instance().AllyFighters.Add("(Player)", PlayerInfo.Instance().Actor);
            RegisterEvents(ainfo);
          }
        }
      }
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Rebel_RemoveTorps");
    }

    public void Rebel_GiveControl(object[] param)
    {
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyFighters.Values)
      {
        ActionManager.Unlock(a);
        //a.AI.CurrentOrder.Complete = true;
        a.ActorState = ActorState.NORMAL;
        a.Speed = a.MaxSpeed;
      }
      foreach (ActorInfo a in GameScenarioManager.Instance().AllyShips.Values)
      {
        ActionManager.Unlock(a);
        a.ActorState = ActorState.NORMAL;
        a.Speed = a.MaxSpeed;
      }
      //PlayerInfo.Instance().PlayerAIEnabled = true;
      PlayerInfo.Instance().IsMovementControlsEnabled = true;

      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
      Empire_TIEAdvanced(null);
    }

    #endregion


    #region Empire spawns

    public void Empire_Wave_2(object[] param)
    {
      switch (GameScenarioManager.Instance().Difficulty.ToLower())
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
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Empire_TIEAdv_Control_TargetFighter");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 15f, "Empire_TIEAdv_Control_Master");
    }

    public void Empire_TIEDefender_Wave(object[] param)
    {
      int sets = 5;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets)) 
        sets = 5;

      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        aci = new ActorCreationInfo(TIE_D_ATI.Instance());
        aci.CreationTime = Game.Instance().GameTime + t;
        aci.Faction = FactionInfo.Get("Empire");
        aci.InitialState = ActorState.NORMAL;
        aci.Position = new TV_3DVECTOR(fx, fy, -22500);
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

        t += 0.5f;
      }
    }

    public void Empire_TIEBombers(object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIE Bombers only
      ActorCreationInfo aci;
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            aci = new ActorCreationInfo(TIE_sa_ATI.Instance());
            aci.CreationTime = Game.Instance().GameTime + t;
            aci.Faction = FactionInfo.Get("Empire");
            aci.InitialState = ActorState.NORMAL;
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, -22500);
            aci.Rotation = new TV_3DVECTOR();

            ActorInfo a = ActorInfo.Create(aci);

            string[] rskeys = new string[GameScenarioManager.Instance().AllyShips.Count];
            GameScenarioManager.Instance().AllyShips.Keys.CopyTo(rskeys, 0);
            ActorInfo rs = GameScenarioManager.Instance().AllyShips[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];

            ActionManager.QueueLast(a, new Actions.AttackActor(rs, -1, -1, false));
            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
          }
        }
        t += 1.5f;
      }
    }

    public void Empire_TIEAdv_Control_Master(object[] param)
    {
      double r = Engine.Instance().Random.NextDouble();

      if (r * (GameScenarioManager.Instance().AllyFighters.Count + GameScenarioManager.Instance().AllyShips.Count * 5) < GameScenarioManager.Instance().AllyFighters.Count)
      {
        Empire_TIEAdvanced_Control_AttackFighter(null);
      }
      else
      {
        Empire_TIEAdvanced_Control_AttackShip(null);
        GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 125f, "Empire_TIEAdv_Control_TargetShip");
      }
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 30f, "Empire_TIEAdv_Control_Master");
    }

    public void Empire_TIEAdvanced(object[] param)
    {
      // TIE Advanced x 1
      ActorCreationInfo aci;

      float fx = Engine.Instance().Random.Next(-2500, 2500);
      float fy = Engine.Instance().Random.Next(-500, 500);

      aci = new ActorCreationInfo(TIE_X1_ATI.Instance());
      aci.Name = "TIE Adv. X1   ";
      aci.CreationTime = Game.Instance().GameTime;
      aci.Faction = FactionInfo.Get("Empire_Advanced");
      aci.InitialState = ActorState.NORMAL;
      aci.Position = new TV_3DVECTOR(fx, fy, -22500);
      aci.Rotation = new TV_3DVECTOR();

      ActorInfo a = ActorInfo.Create(aci);

      //a.AI.Orders.Enqueue(new AIElement { AIType = AIType.IDLE });
      GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
      GameScenarioManager.Instance().CriticalEnemies.Add("TIE ADV. X1", a);
      RegisterEvents(a);

      Screen2D.Instance().UpdateText("The TIE Advanced X1 has arrived.", Game.Instance().GameTime + 5f, new TV_COLOR(1, 1, 1, 1));
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
      string key = "";
      if (param != null && param.GetLength(0) >= 1)
        key = param[0].ToString();

      foreach (ActorInfo a in GameScenarioManager.Instance().EnemyFighters.Values)
      {
        if (a.TypeInfo is TIE_X1_ATI)
        {
          ActorInfo rs = null;
          if (GameScenarioManager.Instance().AllyShips.ContainsKey(key))
          {
            rs = GameScenarioManager.Instance().AllyShips[key];
          }
          else if (GameScenarioManager.Instance().AllyShips.Count > 0)
          {
            string[] rskeys = new string[GameScenarioManager.Instance().AllyShips.Count];
            GameScenarioManager.Instance().AllyShips.Keys.CopyTo(rskeys, 0);
            rs = GameScenarioManager.Instance().AllyShips[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];
          }

          ActionManager.ForceClearQueue(a);
          if (rs != null)
            ActionManager.QueueLast(a, new Actions.AttackActor(rs));

          //a.AI.Orders.Enqueue(new AIElement { AIType = AIType.ATTACK_ACTOR, TargetActor = rs });
        }
      }
    }

    public void Empire_TIEAdvanced_Control_AttackFighter(object[] param)
    {
      string key = "";
      if (param != null && param.GetLength(0) >= 1)
        key = param[0].ToString();

      foreach (ActorInfo a in GameScenarioManager.Instance().EnemyFighters.Values)
      {
        if (a.TypeInfo is TIE_X1_ATI)
        {
          ActorInfo rs = null;
          if (GameScenarioManager.Instance().AllyFighters.ContainsKey(key))
          {
            rs = GameScenarioManager.Instance().AllyFighters[key];
          }
          else if (GameScenarioManager.Instance().AllyFighters.Count > 0)
          {
            string[] rskeys = new string[GameScenarioManager.Instance().AllyFighters.Count];
            GameScenarioManager.Instance().AllyFighters.Keys.CopyTo(rskeys, 0);
            rs = GameScenarioManager.Instance().AllyFighters[rskeys[Engine.Instance().Random.Next(0, rskeys.Length)]];
          }

          ActionManager.ForceClearQueue(a);
          if (rs != null)
            ActionManager.QueueLast(a, new Actions.AttackActor(rs));
        }
      }
    }

    #endregion
  }
}
