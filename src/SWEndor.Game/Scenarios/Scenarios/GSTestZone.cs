using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Models;
using SWEndor.Game.Player;
using Primrose.Primitives.Geometry;
using System.Collections.Generic;

namespace SWEndor.Game.Scenarios
{
  public class GSTestZone : ScenarioBase
  {
    public GSTestZone(ScenarioManager manager) : base(manager)
    {
      Info.Name = "Tower Test Zone [Maintenance]";
      Info.AllowedWings = new ActorTypeInfo[] { Engine.ActorTypeFactory.Get("ADVT")
                                             , Engine.ActorTypeFactory.Get("DEFT")
                                             , Engine.ActorTypeFactory.Get("GUNT")
                                             , Engine.ActorTypeFactory.Get("RDRT")
                                             , Engine.ActorTypeFactory.Get("SUPT")
                                             , Engine.ActorTypeFactory.Get("SUPGUN")
                                             , Engine.ActorTypeFactory.Get("XQ1")
                                             };

      Info.AllowedDifficulties = new string[] { "normal"
                                              };
    }

    private ActorInfo m_Player = null;
    public FactionInfo MainAllyFaction = FactionInfo.Neutral;
    public FactionInfo MainEnemyFaction = FactionInfo.Neutral;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
    }

    public override void Launch()
    {
      base.Launch();

      State.MaxBounds = new TV_3DVECTOR(2500, 200, 2500);
      State.MinBounds = new TV_3DVECTOR(-2500, -200, -2500);
      State.MaxAIBounds = new TV_3DVECTOR(2500, 200, 2500);
      State.MinAIBounds = new TV_3DVECTOR(-2500, -200, -2500);

      PlayerCameraInfo.CameraMode = CameraMode.THIRDPERSON;

      EventQueue.Add(Game.GameTime + 0.1f, Test_SpawnPlayer);
      EventQueue.Add(Game.GameTime + 0.1f, Test_Towers01);
      EventQueue.Add(Game.GameTime + 5f, Test_EnemyWave);

      PlayerInfo.Lives = 2;
      PlayerInfo.ScorePerLife = 1000000;
      PlayerInfo.ScoreForNextLife = 1000000;

      MakePlayer = Test_SpawnPlayer;
      
      Screen2D.Line1.Color = new COLOR(1f, 1f, 0.3f, 1);

      SoundManager.SetMusic("battle_1_1");
      SoundManager.SetMusicLoop("battle_1_4");

      State.IsCutsceneMode = false;
    }

    internal override void LoadFactions()
    {
      base.LoadFactions();

      FactionInfo.Factory.Add("Rebels", new COLOR(0.8f, 0, 0, 1));
      FactionInfo.Factory.Add("Empire", new COLOR(0, 0.8f, 0, 1));

      MainAllyFaction = FactionInfo.Factory.Get("Rebels");
      MainEnemyFaction = FactionInfo.Factory.Get("Empire");
    }

    internal override void LoadScene()
    {
      base.LoadScene();
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();
      //if (GetGameStateB("in_battle"))
      //{
        if (State.StageNumber == 0)
        State.StageNumber = 1;

        if (MainEnemyFaction.WingCount < 10)
          EventQueue.Add(Game.GameTime, Test_EnemyWave);

      //}
    }

    private void CalibrateSceneObjects()
    {
    }

    #region Test events
    
    public void Test_SpawnPlayer()
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;

      if (PlayerInfo.Actor == null || PlayerInfo.Actor.Disposed)
      { 
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = PlayerInfo.ActorType,
            Name = "(Player)",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = new TV_3DVECTOR(125, 0, 125),
            Rotation = new TV_3DVECTOR(),
            Actions = new ActionInfo[] { Wait.GetOrCreate(5) },
            Registries = null
          }.Spawn(this);

          PlayerInfo.ActorID = ainfo.ID;
        }
      }
      m_Player = PlayerInfo.Actor;
      PlayerInfo.SystemLockMovement = false;
    }

    public void Test_GiveControl()
    {
      PlayerInfo.SystemLockMovement = false;
      State.SetGameStateB("in_battle", true);
    }

    public void Test_Towers01()
    {
      List<ActorTypeInfo> towers = new List<ActorTypeInfo> { Engine.ActorTypeFactory.Get("ADVT")
                                             , Engine.ActorTypeFactory.Get("DEFT")
                                             , Engine.ActorTypeFactory.Get("GUNT")
                                             , Engine.ActorTypeFactory.Get("RDRT")
                                             , Engine.ActorTypeFactory.Get("SUPT")
                                             };

      for (int x = -5; x <= 5; x++)
        for (int y = -5; y <= 5; y++)
        {
          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = towers[Engine.Random.Next(0, towers.Count)],
            Name = "",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = MainAllyFaction,
            Position = new TV_3DVECTOR(x * 500, 0, y * 500),
            Rotation = new TV_3DVECTOR()
          }.Spawn(this);
        }
    }
    #endregion

    public void Test_EnemyWave()
    {
      Box box = new Box(new TV_3DVECTOR(-2500, -500, State.MaxBounds.z + 1500), new TV_3DVECTOR(2500, 500, State.MaxBounds.z + 1500));
      SquadSpawnInfo spawninfo = new SquadSpawnInfo(null
                                                                          , ActorTypeFactory.Get("Z95")
                                                                          , MainEnemyFaction
                                                                          , 3
                                                                          , 18
                                                                          , TargetType.FIGHTER
                                                                          , true
                                                                          , SquadFormation.VSHAPE
                                                                          , new TV_3DVECTOR(0, 180, 0)
                                                                          , 400
                                                                          , null);

      GSFunctions.MultipleSquadron_Spawn(Engine, this, 2, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("XWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("YWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("AWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
      spawninfo.TypeInfo = ActorTypeFactory.Get("BWING");
      GSFunctions.MultipleSquadron_Spawn(Engine, this, 1, box, 1.5f, spawninfo);
    }
  }
}
