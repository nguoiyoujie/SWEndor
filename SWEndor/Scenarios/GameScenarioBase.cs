using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.Sound;
using SWEndor.UI.Menu.Pages;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GameScenarioBase
  {
    public string Name = "Untitled Scenario";
    public string Description = "";
    public List<ActorTypeInfo> AllowedWings = new List<ActorTypeInfo> { XWingATI.Instance() };
    public List<string> AllowedDifficulties = new List<string> { "normal" };

    public string Difficulty { get; set; }
    public int StageNumber { get; set; }

    public DeathCamMode DeathCamMode = DeathCamMode.CIRCLE;
    private CameraMode[] m_PlayerCameraModes = new CameraMode[] { CameraMode.FIRSTPERSON };
    private int m_PlayerModeNum = 0;
    public CameraMode[] PlayerCameraModes
    {
      get { return m_PlayerCameraModes; }
      set
      {
        m_PlayerCameraModes = value;
        if (m_PlayerCameraModes == null || m_PlayerCameraModes.Length == 0)
          PlayerCameraInfo.Instance().CameraMode = CameraMode.FIRSTPERSON;
        else
        {
          for (int i = 0; i < m_PlayerCameraModes.Length; i++)
            if (m_PlayerCameraModes[i] == PlayerCameraInfo.Instance().CameraMode)
            {
              m_PlayerModeNum = i;
              return;
            }
          PlayerCameraInfo.Instance().CameraMode = m_PlayerCameraModes[0];
        }
      }
    }

    public CameraMode NextCameraMode()
    {
      if (m_PlayerCameraModes == null || m_PlayerCameraModes.Length == 0)
        return CameraMode.FIRSTPERSON;

      m_PlayerModeNum++;
      if (m_PlayerModeNum >= m_PlayerCameraModes.Length)
        m_PlayerModeNum = 0;

      return m_PlayerCameraModes[m_PlayerModeNum];
    }

    public CameraMode PrevCameraMode()
    {
      if (m_PlayerCameraModes == null || m_PlayerCameraModes.Length == 0)
        return CameraMode.FIRSTPERSON;

      m_PlayerModeNum--;
      if (m_PlayerModeNum < 0)
        m_PlayerModeNum = m_PlayerCameraModes.Length - 1;

      return m_PlayerCameraModes[m_PlayerModeNum];
    }

    // scripting
    public ActorInfo ActiveActor { get; set; }

    public float RebelSpawnTime = 0;
    public float TIESpawnTime = 0;
    public GameEvent MakePlayer;
    public float TimeSinceLostWing = -100;
    public float TimeSinceLostShip = -100;
    public float TimeSinceLostStructure = -100;

    public FactionInfo MainAllyFaction = FactionInfo.Neutral;
    public FactionInfo MainEnemyFaction = FactionInfo.Neutral;

    public virtual void Load(ActorTypeInfo wing, string difficulty)
    {
      Difficulty = difficulty;
      StageNumber = 0;
      PlayerInfo.Instance().ActorType = wing;
      //LandInfo.Instance().Enabled = false;
    }

    public virtual void Launch()
    {
      GameScenarioManager.Instance().Scenario = this;
      PlayerInfo.Instance().Score.Reset();
      LoadFactions();
      LoadScene();
    }

    public virtual void LoadFactions()
    {
      FactionInfo.Factory.Clear();
      MainAllyFaction = FactionInfo.Neutral;
      MainEnemyFaction = FactionInfo.Neutral;
    }

    public virtual void LoadScene()
    {
      Engine.Instance().TVGraphicEffect.FadeIn();
    }

    public virtual void GameTick()
    {
    }

    public virtual void Unload()
    {
      GameScenarioManager.Instance().Scenario = null;

      // Full reset
      foreach (ActorInfo a in ActorInfo.Factory.GetList())
      {
        if (a != null)
          a.Kill();
      }
      GameScenarioManager.Instance().ClearGameStates();
      GameScenarioManager.Instance().ClearEvents();
      Screen2D.Instance().ClearText();

      // clear sounds
      SoundManager.Instance().SetSoundStopAll();

      Screen2D.Instance().OverrideTargetingRadar = false;
      Screen2D.Instance().Box3D_Enable = false;

      LandInfo.Instance().Enabled = false;
    }

    public void FadeOut(params object[] param)
    {
      Engine.Instance().TVGraphicEffect.FadeOut();
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.01f, FadeInterim);
    }

    public void FadeInterim(params object[] param)
    {
      if (Engine.Instance().TVGraphicEffect.IsFadeFinished())
      {
        Engine.Instance().TVScreen2DImmediate.Action_Begin2D();
        Engine.Instance().TVScreen2DImmediate.Draw_FilledBox(0, 0, Engine.Instance().ScreenWidth, Engine.Instance().ScreenHeight, new TV_COLOR(0, 0, 0, 1).GetIntColor());
        Engine.Instance().TVScreen2DImmediate.Action_End2D();

        if (GameScenarioManager.Instance().GetGameStateB("GameOver"))
        {
          GameOver();
          return;
        }
        else if (GameScenarioManager.Instance().GetGameStateF("PlayCutsceneSequence", -1) != -1)
        {
          PlayCutsceneSequence(new object[] { GameScenarioManager.Instance().GetGameStateF("PlayCutsceneSequence", -1) });
          return;
        }
        else if (GameScenarioManager.Instance().GetGameStateB("GameWon"))
        {
          GameWonSequence();
          return;
        }

        MakePlayer?.Invoke(null);
        //GameScenarioManager.Instance().SetGameStateB("PendingPlayerSpawn", false);

        FadeIn();
        GameScenarioManager.Instance().IsCutsceneMode = false;
      }
      else
      {
         GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 0.01f, FadeInterim);
      }
    }

    public void FadeIn(params object[] param)
    {
      Engine.Instance().TVGraphicEffect.FadeIn();
    }

    public void GameOver(params object[] param)
    {
      Engine.Instance().TVGraphicEffect.FadeIn(2.5f);

      SoundManager.Instance().SetSoundStopAll();

      Screen2D.Instance().CurrentPage = new GameOver();
      Screen2D.Instance().ShowPage = true;
      Game.Instance().IsPaused = true;
      SoundManager.Instance().SetMusic("battle_3_2");
    }

    public void LostWing()
    {
      float t = Game.Instance().GameTime;
      if (TimeSinceLostWing > Game.Instance().GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.Instance().GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Game.Instance().GameTime)
        t = TimeSinceLostStructure;

      while (t < Game.Instance().GameTime + 3f)
      {
          GameScenarioManager.Instance().AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostWing = Game.Instance().GameTime + 3f;
    }

    public void LostShip()
    {
      float t = Game.Instance().GameTime;
      if (TimeSinceLostWing > Game.Instance().GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.Instance().GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Game.Instance().GameTime)
        t = TimeSinceLostStructure;

      while (t < Game.Instance().GameTime + 3f)
      {
        GameScenarioManager.Instance().AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = Game.Instance().GameTime + 3f;
    }

    public void LostStructure()
    {
      float t = Game.Instance().GameTime;
      if (TimeSinceLostWing > Game.Instance().GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.Instance().GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Game.Instance().GameTime)
        t = TimeSinceLostStructure;

      while (t < Game.Instance().GameTime + 3f)
      {
        GameScenarioManager.Instance().AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = Game.Instance().GameTime + 3f;
    }

    public void LostSound(object[] param)
    {
      if (!GameScenarioManager.Instance().IsCutsceneMode)
      {
        SoundManager.Instance().SetSound("beep-22", true);
      }
    }

    public Dictionary<string, ActorInfo> GetRegister(string key)
    {
      switch (key.ToLower())
      {
        //case "enemystructures":
        //  return GameScenarioManager.Instance().EnemyStructures;
        //case "allyfighters":
        //  return GameScenarioManager.Instance().AllyFighters;
        //case "allyships":
        //  return GameScenarioManager.Instance().AllyShips;
        //case "allystructures":
        //  return GameScenarioManager.Instance().AllyStructures;
        //case "enemyfighters":
        //  return GameScenarioManager.Instance().EnemyFighters;
        //case "enemyships":
        //  return GameScenarioManager.Instance().EnemyShips;

        case "criticalallies":
          return GameScenarioManager.Instance().CriticalAllies;
        case "criticalenemies":
          return GameScenarioManager.Instance().CriticalEnemies;
        default:
          return null;
      }
    }

    public void SpawnActor(params object[] param)
    {
      // Format: Object[]

      if (param == null 
        || param.GetLength(0) < 10
        || !(param[0] is ActorTypeInfo)
        || !(param[1] is string)
        || !(param[2] is string)
        || !(param[3] is string)
        || !(param[4] is float)
        || !(param[5] is FactionInfo)
        || !(param[6] is TV_3DVECTOR)
        || !(param[7] is TV_3DVECTOR)
        || !(param[8] is ActionInfo[])
        || !(param[9] is string[])
        )
        return;

      new ActorSpawnInfo
      {
        Type = (ActorTypeInfo)param[0],
        Name = (string)param[1],
        RegisterName = (string)param[2],
        SidebarName = (string)param[3],
        SpawnTime = (float)param[4],
        Faction = (FactionInfo)param[5],
        Position = (TV_3DVECTOR)param[6],
        Rotation = (TV_3DVECTOR)param[7],
        Actions = (ActionInfo[])param[8],
        Registries = (string[])param[9]
      }.Spawn(this);
    }

    public void RegisterEvents(ActorInfo actor)
    {
      actor.CreatedEvents += ProcessCreated;
      actor.DestroyedEvents += ProcessKilled;
      actor.HitEvents += ProcessHit;
      actor.ActorStateChangeEvents += ProcessStateChange;
      actor.TickEvents += ProcessTick; 
    }

    public virtual void ProcessCreated(params object[] param)
    {
    }

    public virtual void ProcessKilled(params object[] param)
    {
    }

    public virtual void ProcessPlayerDying(params object[] param)
    {
      ActorInfo ainfo = (ActorInfo)param[0];
      if (ainfo != null)
      {
        PlayerInfo.Instance().TempActor = ainfo;

        if (PlayerInfo.Instance().Actor.TypeInfo is DeathCameraATI)
          if (PlayerInfo.Instance().Actor.CreationState == CreationState.ACTIVE)
            PlayerInfo.Instance().Actor.SetLocalPosition(ainfo.GetLocalPosition().x, ainfo.GetLocalPosition().y, ainfo.GetLocalPosition().z);
      }
    }

    public virtual void ProcessPlayerKilled(params object[] param)
    {
      GameScenarioManager.Instance().IsCutsceneMode = true;
      GameScenarioManager.Instance().AddEvent(Game.Instance().GameTime + 3f, FadeOut);
      if (PlayerInfo.Instance().Lives == 0)
        GameScenarioManager.Instance().SetGameStateB("GameOver", true);
    }

    public virtual void ProcessTick(params object[] param)
    {
    }

    public virtual void ProcessStateChange(params object[] param)
    {
    }

    public virtual void ProcessHit(params object[] param)
    {
      ActorInfo av = (ActorInfo)param[0];
      ActorInfo aa = (ActorInfo)param[1];

      if (PlayerInfo.Instance().Actor != null
        && av.Faction != null
        && av.Faction.IsAlliedWith(PlayerInfo.Instance().Actor.Faction))
      {
        List<int> attackerfamily = aa.GetAllParents();
        foreach (int i in attackerfamily)
        {
          ActorInfo a = ActorInfo.Factory.GetExact(i);
          if (PlayerInfo.Instance().Actor == a)
          {
            Screen2D.Instance().MessageText(string.Format("{0}: {1}, watch your fire!", av.Name, PlayerInfo.Instance().Name)
                                                        , 5
                                                        , av.Faction.Color
                                                        , -1);
          }
        }
      }
    }

    public virtual void PlayCutsceneSequence(object[] param = null)
    {

    }

    public virtual void GameWonSequence(object[] param = null)
    {
      Engine.Instance().TVGraphicEffect.FadeIn(2.5f);

      SoundManager.Instance().SetSoundStopAll();

      Screen2D.Instance().CurrentPage = new GameWon();
      Screen2D.Instance().ShowPage = true;
      Game.Instance().IsPaused = true;
      SoundManager.Instance().SetMusic("finale_3_1");
      SoundManager.Instance().SetMusicLoop("credits_3_1");
    }
  }
}
