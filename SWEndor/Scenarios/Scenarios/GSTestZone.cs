using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSTestZone : GameScenarioBase
  {
    public GSTestZone(GameScenarioManager manager) : base(manager)
    {
      Name = "Test Zone";
      AllowedWings = new List<ActorTypeInfo> { Engine.ActorTypeFactory.Get("Advanced Turbolaser Tower")
                                             , Engine.ActorTypeFactory.Get("Deflector Tower")
                                             , Engine.ActorTypeFactory.Get("Gun Tower")
                                             , Engine.ActorTypeFactory.Get("Radar Tower")
                                             , Engine.ActorTypeFactory.Get("Super Deflector Tower")
                                             , Engine.ActorTypeFactory.Get("Super Turbolaser Turret")
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

      ActorInfo cam = ActorFactory.Get(Manager.SceneCameraID);
      cam.SetLocalPosition(0, 0, 0);
      Manager.MaxBounds = new TV_3DVECTOR(2500, 200, 2500);
      Manager.MinBounds = new TV_3DVECTOR(-2500, -200, -2500);
      Manager.MaxAIBounds = new TV_3DVECTOR(2500, 200, 2500);
      Manager.MinAIBounds = new TV_3DVECTOR(-2500, -200, -2500);

      PlayerCameraInfo.CameraMode = CameraMode.THIRDPERSON;

      Manager.AddEvent(Game.GameTime + 0.1f, Test_SpawnPlayer);
      Manager.AddEvent(Game.GameTime + 0.1f, Test_Towers01);
      Manager.AddEvent(Game.GameTime + 5f, Empire_TIEWave_02);

      PlayerInfo.Lives = 2;
      PlayerInfo.ScorePerLife = 1000000;
      PlayerInfo.ScoreForNextLife = 1000000;

      MakePlayer = Test_SpawnPlayer;
      
      Manager.Line1Color = new TV_COLOR(1f, 1f, 0.3f, 1);

      SoundManager.SetMusic("battle_1_1");
      SoundManager.SetMusicLoop("battle_1_4");

      Manager.IsCutsceneMode = false;
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
      //if (Manager.GetGameStateB("in_battle"))
      //{
        if (StageNumber == 0)
          StageNumber = 1;

        if (MainEnemyFaction.Wings.Count < 10)
          Manager.AddEvent(Game.GameTime, Empire_TIEWave_02);

      //}
    }

    private void CalibrateSceneObjects()
    {
    }

    #region Test events
    
    public void Test_SpawnPlayer(params object[] param)
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;

      if (PlayerInfo.Actor == null || PlayerInfo.Actor.CreationState == CreationState.DISPOSED)
      { 
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = PlayerInfo.ActorType,
            Name = "(Player)",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = new TV_3DVECTOR(125, 0, 125),
            Rotation = new TV_3DVECTOR(),
            Actions = new ActionInfo[] { new Wait(5) },
            Registries = null
          }.Spawn(this);

          PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_Player = PlayerInfo.Actor;
      PlayerInfo.IsMovementControlsEnabled = true;
    }

    public void Test_GiveControl(params object[] param)
    {
      PlayerInfo.IsMovementControlsEnabled = true;
      Manager.SetGameStateB("in_battle", true);
    }

    public void Test_Towers01(params object[] param)
    {
      List<ActorTypeInfo> towers = new List<ActorTypeInfo> { Engine.ActorTypeFactory.Get("Advanced Turbolaser Tower")
                                             , Engine.ActorTypeFactory.Get("Deflector Tower")
                                             , Engine.ActorTypeFactory.Get("Gun Tower")
                                             , Engine.ActorTypeFactory.Get("Radar Tower")
                                             , Engine.ActorTypeFactory.Get("Super Deflector Tower")
                                             };

      for (int x = -5; x <= 5; x++)
        for (int y = -5; y <= 5; y++)
        {
          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = towers[Engine.Random.Next(0, towers.Count)],
            Name = "",
            RegisterName = "",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = new TV_3DVECTOR(x * 500, 0, y * 500),
            Rotation = new TV_3DVECTOR()
          }.Spawn(this);
        }
    }
    #endregion

    public void Empire_TIEWave_02(params object[] param)
    {
      int sets = 3;
      if (param != null && param.GetLength(0) >= 1 && !int.TryParse(param[0].ToString(), out sets))
        sets = 3;

      // TIEs
      ActorTypeInfo[] tietypes = new ActorTypeInfo[] { Engine.ActorTypeFactory.Get("X-Wing"), Engine.ActorTypeFactory.Get("A-Wing") };
      float t = 0;
      for (int k = 1; k < sets; k++)
      {
        float fx = Engine.Random.Next(-2500, 2500);
        float fy = Engine.Random.Next(-500, 500);

        int n = Engine.Random.Next(0, tietypes.Length);
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
              SpawnTime = Game.GameTime + t,
              Faction = MainEnemyFaction,
              Position = new TV_3DVECTOR(fx + x * 100, fy + y * 100, Manager.MaxBounds.z + 1500),
              Rotation = new TV_3DVECTOR(0, 180, 0)
            }.Spawn(this);
          }
        }
        t += 1.5f;
      }
      //Manager.AddEvent(Game.GameTime + 30f, "Empire_TIEWave_02");
    }
  }
}
