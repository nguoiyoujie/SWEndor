using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Player;
using SWEndor.Sound;
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
    private MoodStates m_mood = MoodStates.AMBIENT;
    public MoodStates Mood
    {
      get { return m_mood; }
      set
      {
        if (value < 0)
        {
          SoundManager.TriggerInterruptMood((int)value);
        }
        else
          m_mood = value;
      }
    }

    //public DeathCamMode DeathCamMode = DeathCamMode.CIRCLE;
    private CameraMode[] m_PlayerCameraModes = new CameraMode[] { CameraMode.FIRSTPERSON };
    private int m_PlayerModeNum = 0;
    public CameraMode[] PlayerCameraModes
    {
      get { return m_PlayerCameraModes; }
      set
      {
        m_PlayerCameraModes = value;
        if (m_PlayerCameraModes == null || m_PlayerCameraModes.Length == 0)
          PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;
        else
        {
          for (int i = 0; i < m_PlayerCameraModes.Length; i++)
            if (m_PlayerCameraModes[i] == PlayerCameraInfo.CameraMode)
            {
              m_PlayerModeNum = i;
              return;
            }
          PlayerCameraInfo.CameraMode = m_PlayerCameraModes[0];
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

    public Engine Engine { get { return Manager.Engine; } }
    public Session Game { get { return Engine.Game; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorTypeInfo.Factory ActorTypeFactory { get { return Engine.ActorTypeFactory; } }
    public SoundManager SoundManager { get { return Engine.SoundManager; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

    public GameScenarioBase(GameScenarioManager manager)
    {
      Manager = manager;
    }

    public virtual void Load(ActorTypeInfo wing, string difficulty)
    {
      Difficulty = difficulty;
      StageNumber = 0;
      PlayerInfo.ActorType = wing;
    }

    public virtual void Launch()
    {
      Manager.Scenario = this;
      Manager.IsCutsceneMode = false;
      PlayerInfo.Score.Reset();
      LoadFactions();
      LoadScene();
      Launched = true;
      PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;
      PlayerCameraInfo.Look.ResetPosition();
      PlayerCameraInfo.Look.ResetTarget();
    }

    public virtual void LoadFactions()
    {
      FactionInfo.Factory.Clear();
      MainAllyFaction = FactionInfo.Neutral;
      MainEnemyFaction = FactionInfo.Neutral;
    }

    public virtual void LoadScene()
    {
      TrueVision.TVGraphicEffect.FadeIn();
    }

    public virtual void GameTick()
    {
    }

    public virtual void Unload()
    {
      Launched = false;
      Manager.Scenario = null;
      Engine.PlayerInfo.ActorID = -1;
      Engine.PlayerInfo.TempActorID = -1;
      Engine.PlayerInfo.RequestSpawn = false;

      // Full reset
      Engine.ActorFactory.Reset();
      Engine.ExplosionFactory.Reset();

      Manager.ClearGameStates();
      Manager.ClearEvents();
      Engine.Screen2D.ClearText();

      Engine.Screen2D.OverrideTargetingRadar = false;
      Engine.Screen2D.Box3D_Enable = false;

      Engine.LandInfo.Enabled = false;
      Mood = MoodStates.AMBIENT;
      Engine.SoundManager.Clear();

      Game.GameTime = 0;
      Manager.IsCutsceneMode = false;

      // deleted many things, and this function is called when the game is not active. Probably safe to force GC
      GC.Collect();
    }

    public void FadeOut()
    {
      TrueVision.TVGraphicEffect.FadeOut();
      Manager.AddEvent(Game.GameTime + 0.01f, FadeInterim);
    }

    public void FadeInterim()
    {
      if (TrueVision.TVGraphicEffect.IsFadeFinished())
      {
        TrueVision.TVScreen2DImmediate.Action_Begin2D();
        TrueVision.TVScreen2DImmediate.Draw_FilledBox(0, 0, Engine.ScreenWidth, Engine.ScreenHeight, new TV_COLOR(0, 0, 0, 1).GetIntColor());
        TrueVision.TVScreen2DImmediate.Action_End2D();

        if (Manager.GetGameStateB("GameOver"))
        {
          GameOver();
          return;
        }
        //else if (Manager.GetGameStateF("PlayCutsceneSequence", -1) != -1)
        //{
        //  PlayCutsceneSequence(new object[] { Manager.GetGameStateF("PlayCutsceneSequence", -1) });
        //  return;
        //}
        else if (Manager.GetGameStateB("GameWon"))
        {
          GameWonSequence();
          return;
        }

        MakePlayer?.Invoke();

        FadeIn();
        Manager.IsCutsceneMode = false;
      }
      else
      {
         Manager.AddEvent(Game.GameTime + 0.01f, FadeInterim);
      }
    }

    public void FadeIn()
    {
      TrueVision.TVGraphicEffect.FadeIn();
    }

    public void GameOver()
    {
      TrueVision.TVGraphicEffect.FadeIn(2.5f);

      SoundManager.SetSoundStopAll();

      Screen2D.CurrentPage = new GameOver(Screen2D);
      Screen2D.ShowPage = true;
      Game.IsPaused = true;
      SoundManager.SetMusic("battle_3_2"); // make modifiable
    }

    public void LostWing()
    {
      float t = Game.GameTime;
      if (TimeSinceLostWing > Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < Game.GameTime + 3f)
      {
          Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostWing = Game.GameTime + 3f;
    }

    public void LostShip()
    {
      float t = Game.GameTime;
      if (TimeSinceLostWing > Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < Game.GameTime + 3f)
      {
        Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = Game.GameTime + 3f;
    }

    public void LostStructure()
    {
      float t = Game.GameTime;
      if (TimeSinceLostWing > Game.GameTime)
        t = TimeSinceLostWing;

      if (TimeSinceLostShip > Game.GameTime)
        t = TimeSinceLostShip;

      if (TimeSinceLostStructure > Game.GameTime)
        t = TimeSinceLostStructure;

      while (t < Game.GameTime + 3f)
      {
        Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = Game.GameTime + 3f;
    }

    public void LostSound()
    {
      if (!Manager.IsCutsceneMode)
      {
        SoundManager.SetSound(SoundGlobals.LostShip, true);
      }
    }

    public HashSet<ActorInfo> GetRegister(string key)
    {
      switch (key.ToLower())
      {
        case "criticalallies":
          return Manager.CriticalAllies;
        case "criticalenemies":
          return Manager.CriticalEnemies;
        default:
          return null;
      }
    }

    public void RegisterEvents(ActorInfo actor)
    {
     // actor.HitEvents += ProcessHit;
    }

    public virtual void ProcessPlayerDying(ActorInfo a)
    {
      if (a != null)
      {
        PlayerInfo.TempActorID = a.ID;

        //if (PlayerInfo.Actor.TypeInfo is DeathCameraATI)
        //  if (PlayerInfo.Actor.Active)
        //    PlayerInfo.Actor.Position = new TV_3DVECTOR(ainfo.Position.x, ainfo.Position.y, ainfo.Position.z);
      }
    }

    public virtual void ProcessPlayerKilled(ActorInfo a)
    {
      Manager.IsCutsceneMode = true;
      Manager.AddEvent(Game.GameTime + 3f, FadeOut);
      if (PlayerInfo.Lives == 0)
        Manager.SetGameStateB("GameOver", true);
    }

    public virtual void ProcessHit(ActorInfo aa, ActorInfo av)
    {
      /*
      if (PlayerInfo.Actor == aa.TopParent)
      {
        if (av.Faction.IsAlliedWith(PlayerInfo.Actor.Faction))
        {
          Screen2D.MessageText(string.Format("{0}: {1}, watch your fire!", av.Name, PlayerInfo.Name)
                                                      , 5
                                                      , av.Faction.Color
                                                      , -1);
        }
      }
      */
    }

    public virtual void PlayCutsceneSequence()
    {

    }

    public virtual void GameWonSequence()
    {
      TrueVision.TVGraphicEffect.FadeIn(2.5f);

      SoundManager.SetSoundStopAll();

      Screen2D.CurrentPage = new GameWon(Screen2D);
      Screen2D.ShowPage = true;
      Game.IsPaused = true;
      SoundManager.SetMusic("finale_3_1");
      SoundManager.SetMusicLoop("credits_3_1");
    }
  }
}
