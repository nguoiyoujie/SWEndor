using MTV3D65;
using System.Collections.Generic;
using System;

namespace SWEndor.Scenarios
{
  public class GSTestZone : GameScenarioBase
  {
    public GSTestZone()
    {
      Name = "Test Zone";
      AllowedWings = new List<ActorTypeInfo> { Tower00ATI.Instance()
                                             , Tower01ATI.Instance()
                                             , Tower02ATI.Instance()
                                             , Tower03ATI.Instance()
                                             };

      AllowedDifficulties = new List<string> { "normal"
                                              };
    }

    private ActorInfo m_AScene = null;

    private ActorInfo m_Player = null;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
      if (GameScenarioManager.Instance().GetGameStateB("in_game"))
        return;

      GameScenarioManager.Instance().SetGameStateB("in_game", true);
      RegisterEvents();
      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(2500, 200, 2500);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-2500, -200, -2500);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(2500, 200, 2500);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-2500, -200, -2500);

      PlayerInfo.Instance().CameraMode = CameraMode.THIRDPERSON;

      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Test_SpawnPlayer");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.1f, "Test_Towers01");
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 5f, "Empire_TIEWave_02");

      PlayerInfo.Instance().Lives = 2;
      PlayerInfo.Instance().ScorePerLife = 1000000;
      PlayerInfo.Instance().ScoreForNextLife = 1000000;
      PlayerInfo.Instance().Score.Reset();

      MakePlayer = Test_SpawnPlayer;
      
      GameScenarioManager.Instance().Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);

      SoundManager.Instance().SetMusic("battle_1_1");
      SoundManager.Instance().SetMusicLoop("battle_1_4");

      GameScenarioManager.Instance().IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.AddFaction("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.AddFaction("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;
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

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      if (GameScenarioManager.Instance().GetGameStateB("in_battle"))
      {
        if (GameScenarioManager.Instance().StageNumber == 0)
        {
          GameScenarioManager.Instance().StageNumber = 1;
        }
      }
    }

    private void CalibrateSceneObjects()
    {
      if (m_AScene != null && m_AScene.CreationState == CreationState.ACTIVE)
      {
        m_AScene.SetLocalPosition(PlayerInfo.Instance().Position.x, PlayerInfo.Instance().Position.y, PlayerInfo.Instance().Position.z);
      }
    }

    public override void RegisterEvents()
    {
      base.RegisterEvents();
      GameEvent.RegisterEvent("Test_SpawnPlayer", Test_SpawnPlayer);
      GameEvent.RegisterEvent("Test_GiveControl", Test_GiveControl);
      GameEvent.RegisterEvent("Test_Towers01", Test_Towers01);
      GameEvent.RegisterEvent("Empire_TIEWave_02", Empire_TIEWave_02);
    }

    #region Test events
    
    public void Test_SpawnPlayer(object[] param)
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
          placi.Faction = FactionInfo.Get("Rebels");
          placi.InitialState = ActorState.NORMAL;
          placi.Position = new TV_3DVECTOR(0, 0, -200);
          placi.Rotation = new TV_3DVECTOR(0, 0, 0);

          ActorInfo ainfo = ActorInfo.Create(placi);
          PlayerInfo.Instance().Actor = ainfo;
          GameScenarioManager.Instance().AllyFighters.Add("(Player)", PlayerInfo.Instance().Actor);
          RegisterEvents(ainfo);
        }
      }
      m_Player = PlayerInfo.Instance().Actor;
    }

    public void Test_GiveControl(object[] param)
    {
      PlayerInfo.Instance().IsMovementControlsEnabled = true;
      //PlayerInfo.Instance().PlayerAIEnabled = true;
      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
    }

    public void Test_Towers01(object[] param)
    {
      SpawnActor(Tower03ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100, 0, 500), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

      SpawnActor(Tower02ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100 + 1000, 0, 500), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

      SpawnActor(Tower00ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100 - 1000, 0, 500), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

      SpawnActor(Tower02ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100, 0, 500 + 1000), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);

      SpawnActor(Tower01ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100, 0, 500 - 1000), new TV_3DVECTOR(), new ActionInfo[0], new Dictionary<string, ActorInfo>[0]);
    }
    #endregion

    public void Empire_TIEWave_02(object[] param)
    {
      int sets = 5;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 5;

      // TIEs
      ActorCreationInfo aci;
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { TIE_LN_ATI.Instance(), TIE_IN_ATI.Instance() };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        int n = Engine.Instance().Random.Next(0, tietypes.Length);
        aci = new ActorCreationInfo(tietypes[n]);
        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            aci.CreationTime = Game.Instance().GameTime + t;
            aci.Faction = FactionInfo.Get("Empire");
            aci.InitialState = ActorState.NORMAL;
            aci.Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MaxBounds.z + 1500);
            aci.Rotation = new TV_3DVECTOR(0, 180, 0);

            ActorInfo a = ActorInfo.Create(aci);
            GameScenarioManager.Instance().EnemyFighters.Add(a.Key, a);
            RegisterEvents(a);
          }
        }
        t += 1.5f;
      }
    }
  }
}
