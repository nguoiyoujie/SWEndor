using MTV3D65;
using SWEndor.Actors;
using SWEndor.Actors.Types;
using SWEndor.AI.Actions;
using SWEndor.Sound;
using System.Collections.Generic;

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
                                             , Tower04ATI.Instance()
                                             , TowerGunSuperATI.Instance()
                                             };

      AllowedDifficulties = new List<string> { "normal"
                                              };
    }

    private ActorInfo m_Player = null;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
    }

    public override void Launch()
    {
      base.Launch();

      GameScenarioManager.Instance().SceneCamera.SetLocalPosition(0, 0, 0);
      GameScenarioManager.Instance().MaxBounds = new TV_3DVECTOR(2500, 200, 2500);
      GameScenarioManager.Instance().MinBounds = new TV_3DVECTOR(-2500, -200, -2500);
      GameScenarioManager.Instance().MaxAIBounds = new TV_3DVECTOR(2500, 200, 2500);
      GameScenarioManager.Instance().MinAIBounds = new TV_3DVECTOR(-2500, -200, -2500);

      PlayerCameraInfo.Instance().CameraMode = CameraMode.THIRDPERSON;

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

      MainAllyFaction = FactionInfo.Get("Rebels");
      MainEnemyFaction = FactionInfo.Get("Empire");
    }

    public override void LoadScene()
    {
      base.LoadScene();
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      //if (GameScenarioManager.Instance().GetGameStateB("in_battle"))
      //{
        if (StageNumber == 0)
        {
          StageNumber = 1;
        }

        if (MainEnemyFaction.Wings.Count < 10)
          GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime, "Empire_TIEWave_02");

      //}
    }

    private void CalibrateSceneObjects()
    {
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
      PlayerInfo.Instance().Actor = PlayerInfo.Instance().TempActor;

      if (PlayerInfo.Instance().Actor == null || PlayerInfo.Instance().Actor.CreationState == CreationState.DISPOSED)
      { 
        if (PlayerInfo.Instance().Lives > 0)
        {
          PlayerInfo.Instance().Lives--;

          ActorInfo ainfo = SpawnActor(PlayerInfo.Instance().ActorType
                                      , "(Player)"
                                      , ""
                                      , ""
                                      , Game.Instance().GameTime
                                      , MainAllyFaction
                                      , new TV_3DVECTOR(125, 0, 125)
                                      , new TV_3DVECTOR()
                                      , new ActionInfo[] { new Wait(5) }
                                      );

          PlayerInfo.Instance().Actor = ainfo;
        }
      }
      m_Player = PlayerInfo.Instance().Actor;
      PlayerInfo.Instance().IsMovementControlsEnabled = true;
    }

    public void Test_GiveControl(object[] param)
    {
      PlayerInfo.Instance().IsMovementControlsEnabled = true;
      GameScenarioManager.Instance().SetGameStateB("in_battle", true);
    }

    public void Test_Towers01(object[] param)
    {
      List<ActorTypeInfo> towers = new List<ActorTypeInfo> { Tower00ATI.Instance()
                                             , Tower01ATI.Instance()
                                             , Tower02ATI.Instance()
                                             , Tower03ATI.Instance()
                                             , Tower04ATI.Instance()
                                             };

      for (int x = -5; x <= 5; x++)
        for (int y = -5; y <= 5; y++)
          SpawnActor(towers[Engine.Instance().Random.Next(0, towers.Count)], "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(x * 500, 0, y * 500), new TV_3DVECTOR());
      /*
      SpawnActor(Tower03ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100, 0, 500), new TV_3DVECTOR());
               
      SpawnActor(Tower02ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100 + 1000, 0, 500), new TV_3DVECTOR());

      SpawnActor(Tower00ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100 - 1000, 0, 500), new TV_3DVECTOR());

      SpawnActor(Tower02ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100, 0, 500 + 1000), new TV_3DVECTOR());

      SpawnActor(Tower01ATI.Instance(), "", "", ""
               , 0, FactionInfo.Get("Rebels"), new TV_3DVECTOR(100, 0, 500 - 1000), new TV_3DVECTOR());
      */
    }
    #endregion

    public void Empire_TIEWave_02(object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIEs
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { XWingATI.Instance(), AWingATI.Instance() };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Instance().Random.Next(-2500, 2500);
        float fy = Engine.Instance().Random.Next(-500, 500);

        int n = Engine.Instance().Random.Next(0, tietypes.Length);
        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorInfo ainfo = SpawnActor(tietypes[n]
                            , ""
                            , ""
                            , ""
                            , Game.Instance().GameTime + t
                            , MainEnemyFaction
                            , new TV_3DVECTOR(fx + x * 100, fy + y * 100, GameScenarioManager.Instance().MaxBounds.z + 1500)
                            , new TV_3DVECTOR(0, 180, 0)
                            );
          }
        }
        t += 1.5f;
      }
      //GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 30f, "Empire_TIEWave_02");
    }
  }
}
