using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI.Actions;
using SWEndor.Player;
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

      Globals.Engine.GameScenarioManager.SceneCamera.SetLocalPosition(0, 0, 0);
      Globals.Engine.GameScenarioManager.MaxBounds = new TV_3DVECTOR(2500, 200, 2500);
      Globals.Engine.GameScenarioManager.MinBounds = new TV_3DVECTOR(-2500, -200, -2500);
      Globals.Engine.GameScenarioManager.MaxAIBounds = new TV_3DVECTOR(2500, 200, 2500);
      Globals.Engine.GameScenarioManager.MinAIBounds = new TV_3DVECTOR(-2500, -200, -2500);

      PlayerCameraInfo.Instance().CameraMode = CameraMode.THIRDPERSON;

      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Test_SpawnPlayer);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.1f, Test_Towers01);
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 5f, Empire_TIEWave_02);

      Globals.Engine.PlayerInfo.Lives = 2;
      Globals.Engine.PlayerInfo.ScorePerLife = 1000000;
      Globals.Engine.PlayerInfo.ScoreForNextLife = 1000000;

      MakePlayer = Test_SpawnPlayer;
      
      Globals.Engine.GameScenarioManager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);

      Globals.Engine.SoundManager.SetMusic("battle_1_1");
      Globals.Engine.SoundManager.SetMusicLoop("battle_1_4");

      Globals.Engine.GameScenarioManager.IsCutsceneMode = false;
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new TV_COLOR(0.8f, 0, 0, 1)).AutoAI = true;
      FactionInfo.Factory.Add("Empire", new TV_COLOR(0, 0.8f, 0, 1)).AutoAI = true;

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");
    }

    public override void LoadScene()
    {
      base.LoadScene();
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      //if (Globals.Engine.GameScenarioManager.GetGameStateB("in_battle"))
      //{
        if (StageNumber == 0)
          StageNumber = 1;

        if (MainEnemyFaction.Wings.Count < 10)
          Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime, Empire_TIEWave_02);

      //}
    }

    private void CalibrateSceneObjects()
    {
    }

    #region Test events
    
    public void Test_SpawnPlayer(object[] param)
    {
      Globals.Engine.PlayerInfo.ActorID = Globals.Engine.PlayerInfo.TempActorID;

      if (Globals.Engine.PlayerInfo.Actor == null || Globals.Engine.PlayerInfo.Actor.CreationState == CreationState.DISPOSED)
      { 
        if (Globals.Engine.PlayerInfo.Lives > 0)
        {
          Globals.Engine.PlayerInfo.Lives--;

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = Globals.Engine.PlayerInfo.ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Globals.Engine.Game.GameTime,
            Faction = MainAllyFaction,
            Position = new TV_3DVECTOR(125, 0, 125),
            Rotation = new TV_3DVECTOR(),
            Actions = new ActionInfo[] { new Wait(5) },
            Registries = null
          }.Spawn(this);

          Globals.Engine.PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_Player = Globals.Engine.PlayerInfo.Actor;
      Globals.Engine.PlayerInfo.IsMovementControlsEnabled = true;
    }

    public void Test_GiveControl(object[] param)
    {
      Globals.Engine.PlayerInfo.IsMovementControlsEnabled = true;
      Globals.Engine.GameScenarioManager.SetGameStateB("in_battle", true);
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
        {
          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = towers[Globals.Engine.Random.Next(0, towers.Count)],
            Name = "",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Globals.Engine.Game.GameTime,
            Faction = MainAllyFaction,
            Position = new TV_3DVECTOR(x * 500, 0, y * 500),
            Rotation = new TV_3DVECTOR()
          }.Spawn(this);
        }
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
        float fx = Globals.Engine.Random.Next(-2500, 2500);
        float fy = Globals.Engine.Random.Next(-500, 500);

        int n = Globals.Engine.Random.Next(0, tietypes.Length);
        for (int x = 0; x <= 1; x++)
        {
          for (int y = 0; y <= 1; y++)
          {
            ActorInfo ainfo = new ActorSpawnInfo
            {
              Type = tietypes[n],
              Name = "",
              RegisterName = "",
              SidebarName = "",
              SpawnTime = Globals.Engine.Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Globals.Engine.GameScenarioManager.MaxBounds.z + 1500),
              Rotation = new TV_3DVECTOR(0, 180, 0)
            }.Spawn(this);
          }
        }
        t += 1.5f;
      }
      //Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 30f, "Empire_TIEWave_02");
    }
  }
}
