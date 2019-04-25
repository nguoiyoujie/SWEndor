using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.ActorTypes.Instances;
using SWEndor.AI.Actions;
using SWEndor.Player;
using SWEndor.UI.Menu.Pages;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GameScenarioBase
  {
    public string Name = "Untitled Scenario";
    public string Description = "";
    public List<ActorTypeInfo> AllowedWings = new List<ActorTypeInfo>();
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

    public GameEvent MakePlayer;
    public float TimeSinceLostWing = -100;
    public float TimeSinceLostShip = -100;
    public float TimeSinceLostStructure = -100;
    public bool Launched = false;

    public FactionInfo MainAllyFaction = FactionInfo.Neutral;
    public FactionInfo MainEnemyFaction = FactionInfo.Neutral;

    public readonly GameScenarioManager Manager;

    public GameScenarioBase(GameScenarioManager manager)
    {
      Manager = manager;
    }

    public virtual void Load(ActorTypeInfo wing, string difficulty)
    {
      Difficulty = difficulty;
      StageNumber = 0;
      Globals.Engine.PlayerInfo.ActorType = wing;
    }

    public virtual void Launch()
    {
      Globals.Engine.GameScenarioManager.Scenario = this;
      Globals.Engine.PlayerInfo.Score.Reset();
      LoadFactions();
      LoadScene();
      Launched = true;
    }

    public virtual void LoadFactions()
    {
      FactionInfo.Factory.Clear();
      MainAllyFaction = FactionInfo.Neutral;
      MainEnemyFaction = FactionInfo.Neutral;
    }

    public virtual void LoadScene()
    {
      Globals.Engine.TrueVision.TVGraphicEffect.FadeIn();
    }

    public virtual void GameTick()
    {
    }

    public virtual void Unload()
    {
      Launched = false;
      Globals.Engine.GameScenarioManager.Scenario = null;

      // Full reset
      Manager.Engine.ActorFactory.Reset();

      Globals.Engine.GameScenarioManager.ClearGameStates();
      Globals.Engine.GameScenarioManager.ClearEvents();
      Globals.Engine.Screen2D.ClearText();
      Globals.Engine.PlayerInfo.RequestSpawn = false;

      // clear sounds
      Globals.Engine.SoundManager.SetSoundStopAll();

      Globals.Engine.Screen2D.OverrideTargetingRadar = false;
      Globals.Engine.Screen2D.Box3D_Enable = false;

      Globals.Engine.LandInfo.Enabled = false;

      // deleted many things, and this function is called when the game is not active. Probably safe to force GC
      GC.Collect();
    }

    public void FadeOut(params object[] param)
    {
      Globals.Engine.TrueVision.TVGraphicEffect.FadeOut();
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.01f, FadeInterim);
    }

    public void FadeInterim(params object[] param)
    {
      if (Globals.Engine.TrueVision.TVGraphicEffect.IsFadeFinished())
      {
        Globals.Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
        Globals.Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(0, 0, Globals.Engine.ScreenWidth, Globals.Engine.ScreenHeight, new TV_COLOR(0, 0, 0, 1).GetIntColor());
        Globals.Engine.TrueVision.TVScreen2DImmediate.Action_End2D();

        if (Globals.Engine.GameScenarioManager.GetGameStateB("GameOver"))
        {
          GameOver();
          return;
        }
        else if (Globals.Engine.GameScenarioManager.GetGameStateF("PlayCutsceneSequence", -1) != -1)
        {
          PlayCutsceneSequence(new object[] { Globals.Engine.GameScenarioManager.GetGameStateF("PlayCutsceneSequence", -1) });
          return;
        }
        else if (Globals.Engine.GameScenarioManager.GetGameStateB("GameWon"))
        {
          GameWonSequence();
          return;
        }

        MakePlayer?.Invoke(null);

        FadeIn();
        Globals.Engine.GameScenarioManager.IsCutsceneMode = false;
      }
      else
      {
         Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 0.01f, FadeInterim);
      }
    }

    public void FadeIn(params object[] param)
    {
      Globals.Engine.TrueVision.TVGraphicEffect.FadeIn();
    }

    public void GameOver(params object[] param)
    {
      Globals.Engine.TrueVision.TVGraphicEffect.FadeIn(2.5f);

      Globals.Engine.SoundManager.SetSoundStopAll();

      Globals.Engine.Screen2D.CurrentPage = new GameOver();
      Globals.Engine.Screen2D.ShowPage = true;
      Globals.Engine.Game.IsPaused = true;
      Globals.Engine.SoundManager.SetMusic("battle_3_2"); // make modifiable
    }

    public void LostWing()
    {
      float t = Globals.Engine.Game.GameTime;
      if (TimeSinceLostWing > Globals.Engine.Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Globals.Engine.Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Globals.Engine.Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < Globals.Engine.Game.GameTime + 3f)
      {
          Globals.Engine.GameScenarioManager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostWing = Globals.Engine.Game.GameTime + 3f;
    }

    public void LostShip()
    {
      float t = Globals.Engine.Game.GameTime;
      if (TimeSinceLostWing > Globals.Engine.Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Globals.Engine.Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Globals.Engine.Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < Globals.Engine.Game.GameTime + 3f)
      {
        Globals.Engine.GameScenarioManager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = Globals.Engine.Game.GameTime + 3f;
    }

    public void LostStructure()
    {
      float t = Globals.Engine.Game.GameTime;
      if (TimeSinceLostWing > Globals.Engine.Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Globals.Engine.Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Globals.Engine.Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < Globals.Engine.Game.GameTime + 3f)
      {
        Globals.Engine.GameScenarioManager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = Globals.Engine.Game.GameTime + 3f;
    }

    public void LostSound(object[] param)
    {
      if (!Globals.Engine.GameScenarioManager.IsCutsceneMode)
      {
        Globals.Engine.SoundManager.SetSound("beep-22", true);
      }
    }

    public Dictionary<string, ActorInfo> GetRegister(string key)
    {
      switch (key.ToLower())
      {
        //case "enemystructures":
        //  return Globals.Engine.GameScenarioManager.EnemyStructures;
        //case "allyfighters":
        //  return Globals.Engine.GameScenarioManager.AllyFighters;
        //case "allyships":
        //  return Globals.Engine.GameScenarioManager.AllyShips;
        //case "allystructures":
        //  return Globals.Engine.GameScenarioManager.AllyStructures;
        //case "enemyfighters":
        //  return Globals.Engine.GameScenarioManager.EnemyFighters;
        //case "enemyships":
        //  return Globals.Engine.GameScenarioManager.EnemyShips;

        case "criticalallies":
          return Globals.Engine.GameScenarioManager.CriticalAllies;
        case "criticalenemies":
          return Globals.Engine.GameScenarioManager.CriticalEnemies;
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
      ActorInfo ainfo = Manager.Engine.ActorFactory.Get((int)param[0]);
      if (ainfo != null)
      {
        Globals.Engine.PlayerInfo.TempActorID = ainfo.ID;

        if (Globals.Engine.PlayerInfo.Actor.TypeInfo is DeathCameraATI)
          if (Globals.Engine.PlayerInfo.Actor.CreationState == CreationState.ACTIVE)
            Globals.Engine.PlayerInfo.Actor.SetLocalPosition(ainfo.GetLocalPosition().x, ainfo.GetLocalPosition().y, ainfo.GetLocalPosition().z);
      }
    }

    public virtual void ProcessPlayerKilled(params object[] param)
    {
      Globals.Engine.GameScenarioManager.IsCutsceneMode = true;
      Globals.Engine.GameScenarioManager.AddEvent(Globals.Engine.Game.GameTime + 3f, FadeOut);
      if (Globals.Engine.PlayerInfo.Lives == 0)
        Globals.Engine.GameScenarioManager.SetGameStateB("GameOver", true);
    }

    public virtual void ProcessTick(params object[] param)
    {
    }

    public virtual void ProcessStateChange(params object[] param)
    {
    }

    public virtual void ProcessHit(params object[] param)
    {
      ActorInfo av = Manager.Engine.ActorFactory.Get((int)param[0]);
      ActorInfo aa = Manager.Engine.ActorFactory.Get((int)param[1]);

      if (Globals.Engine.PlayerInfo.Actor != null
        && av.Faction != null
        && av.Faction.IsAlliedWith(Globals.Engine.PlayerInfo.Actor.Faction))
      {
        List<int> attackerfamily = aa.GetAllParents();
        foreach (int i in attackerfamily)
        {
          ActorInfo a = Manager.Engine.ActorFactory.Get(i);
          if (Globals.Engine.PlayerInfo.Actor == a)
          {
            Globals.Engine.Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", av.Name, Globals.Engine.PlayerInfo.Name)
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
      Globals.Engine.TrueVision.TVGraphicEffect.FadeIn(2.5f);

      Globals.Engine.SoundManager.SetSoundStopAll();

      Globals.Engine.Screen2D.CurrentPage = new GameWon();
      Globals.Engine.Screen2D.ShowPage = true;
      Globals.Engine.Game.IsPaused = true;
      Globals.Engine.SoundManager.SetMusic("finale_3_1");
      Globals.Engine.SoundManager.SetMusicLoop("credits_3_1");
    }
  }
}
