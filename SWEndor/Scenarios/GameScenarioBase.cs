﻿using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Primitives.Extensions;
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
    public bool InformLostWing = false;
    public bool InformLostShip = false;
    public bool InformLostStructure = false;
    public float TimeSinceLostWing = -100;
    public float TimeSinceLostShip = -100;
    public float TimeSinceLostStructure = -100;
    private float LastLostSoundTime = 0;
    public bool Launched = false;

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
      PlayerCameraInfo.SetPlayerLook(); 
    }

    public virtual void LoadFactions()
    {
      FactionInfo.Factory.Clear();
      //MainAllyFaction = FactionInfo.Neutral;
      //MainEnemyFaction = FactionInfo.Neutral;
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
      Engine.PlayerInfo.ActorID = -1;
      Engine.PlayerInfo.TempActorID = -1;
      Engine.PlayerInfo.RequestSpawn = false;

      // Full reset
      Engine.ProjectileFactory.Reset();
      Engine.ExplosionFactory.Reset();
      Engine.ActorFactory.Reset();

      Manager.ClearGameStates();
      Manager.ClearEvents();
      Engine.Screen2D.ClearText();

      Engine.Screen2D.OverrideTargetingRadar = false;
      Engine.Screen2D.Box3D_Enable = false;

      Engine.LandInfo.Enabled = false;

      Mood = MoodStates.AMBIENT;
      Engine.SoundManager.Clear();
      DistanceModel.Reset();

      // TO-DO: reset the live counters in a more graceful way
      AI.Actions.Hunt._count = 0;
      AI.Actions.AttackActor._count = 0;
      AI.Actions.AvoidCollisionRotate._count = 0;
      AI.Actions.AvoidCollisionWait._count = 0;
      AI.Actions.Evade._count = 0;
      AI.Actions.ProjectileAttackActor._count = 0;

      Game.GameTime = 0;
      Manager.IsCutsceneMode = false;

      InformLostWing = false;
      InformLostShip = false;
      InformLostStructure = false;
      TimeSinceLostWing = -100;
      TimeSinceLostShip = -100;
      TimeSinceLostStructure = -100;
      LastLostSoundTime = 0;

      Manager.Scenario = null;

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
        PlayerCameraInfo.SetPlayerLook();

        FadeIn();
        Manager.IsCutsceneMode = false;
      }
      else
      {
         Manager.AddEvent(Game.GameTime + 0.0001f, FadeInterim);
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
      SoundManager.SetMusic("battle_3_2"); // TO-DO: make configurable
    }

    public void LostWing()
    {
      float t = Game.GameTime;
      float t2 = Game.GameTime + 3;
      while (t < t2)
      {
        Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostWing = t2;
      LastLostSoundTime = t2;
    }

    public void LostShip()
    {
      float t = Game.GameTime;
      float t2 = Game.GameTime + 3;
      while (t < t2)
      {
        Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostShip = t2;
      LastLostSoundTime = t2;
    }

    public void LostStructure()
    {
      float t = Game.GameTime;
      float t2 = Game.GameTime + 3;
      while (t < t2)
      {
        Manager.AddEvent(t, LostSound);
        t += 0.2f;
      }
      TimeSinceLostStructure = t2;
      LastLostSoundTime = t2;
    }

    public void LostSound()
    {
      if (!Manager.IsCutsceneMode)
        SoundManager.SetSound(SoundGlobals.LostShip, true);
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

    public virtual void ProcessPlayerDying(ActorInfo a)
    {

    }

    public virtual void ProcessPlayerKilled(ActorInfo a)
    {
      Manager.IsCutsceneMode = true;
      Manager.AddEvent(Game.GameTime + 3f, FadeOut);
      if (PlayerInfo.Lives == 0)
        Manager.SetGameStateB("GameOver", true);
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
