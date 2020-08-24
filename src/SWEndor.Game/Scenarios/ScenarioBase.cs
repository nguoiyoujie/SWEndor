using MTV3D65;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.Core;
using SWEndor.Game.Models;
using SWEndor.Game.Player;
using SWEndor.Game.Sound;
using SWEndor.Game.UI.Menu.Pages;
using System;
using System.Collections.Generic;

namespace SWEndor.Game.Scenarios
{
  public class ScenarioBase
  {
    public readonly ScenarioManager Manager;
    internal GameEventQueue EventQueue;
    public ScenarioInfo Info = new ScenarioInfo().InitDefault();
    public ScenarioStates State = new ScenarioStates();

    public Engine Engine { get { return Manager.Engine; } }
    public Session Game { get { return Engine.Game; } }
    public ActorTypeInfo.Factory ActorTypeFactory { get { return Engine.ActorTypeFactory; } }
    public SoundManager SoundManager { get { return Engine.SoundManager; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }

    public GameEvent MakePlayer;

    public ScenarioBase(ScenarioManager manager)
    {
      Manager = manager;
      EventQueue = new GameEventQueue(manager.Engine);
    }

    public virtual void Load(ActorTypeInfo wing, string difficulty)
    {
      State.Difficulty = difficulty;
      State.StageNumber = 0;
      PlayerInfo.ActorType = wing;
    }

    public virtual void Launch()
    {
      Manager.Scenario = this;
      State.IsCutsceneMode = false;
      PlayerInfo.Score.Reset();
      LoadFactions();
      LoadScene();
      State.Launched = true;
      PlayerCameraInfo.CameraMode = CameraMode.FIRSTPERSON;
      PlayerCameraInfo.SetPlayerLook();
      PlayerInfo.PlayerLockMovement = false;
      PlayerInfo.SystemLockMovement = true;
    }

    internal virtual void LoadFactions()
    {
      FactionInfo.Factory.Clear();
    }

    internal virtual void LoadScene()
    {
      FadeIn();
    }

    public virtual void GameTick()
    {
    }

    public virtual void Unload()
    {
      State.Launched = false;
      Engine.PlayerInfo.ActorID = -1;
      Engine.PlayerInfo.TempActorID = -1;
      Engine.PlayerInfo.RequestSpawn = false;

      // Full reset
      Engine.ProjectileFactory.Reset();
      Engine.ExplosionFactory.Reset();
      Engine.ActorFactory.Reset();

      State.Reset();
      EventQueue.Clear();
      Engine.Screen2D.ClearText();

      Engine.Screen2D.OverrideTargetingRadar = false;
      Engine.Screen2D.Box3D_Enable = false;

      Engine.LandInfo.Enabled = false;

      SoundManager.SetMood(MoodState.AMBIENT);
      Engine.SoundManager.Clear();
      DistanceModel.Reset();
      PlayerCameraInfo.Shake(0);

      Game.GameTime = 0;
      Game.GameFrame = 0;
      Game.CollisionTickCount = 0;
      Game.AITickCount = 0;
      State.IsCutsceneMode = false;
      
      Manager.Scenario = null;

      // deleted many things, and this function is called when the game is not active. Probably safe to force GC
      GC.Collect();
    }

    public void FadeOut()
    {
      Engine.PlayerCameraInfo.FadeOut();
      EventQueue.Add(Game.GameTime + 0.01f, FadeInterim);
    }

    public void FadeInterim()
    {
      if (!Engine.PlayerCameraInfo.IsFadingOut)
      {
        Engine.TrueVision.TVScreen2DImmediate.Action_Begin2D();
        Engine.TrueVision.TVScreen2DImmediate.Draw_FilledBox(0, 0, Engine.ScreenWidth, Engine.ScreenHeight, new TV_COLOR(0, 0, 0, 1).GetIntColor());
        Engine.TrueVision.TVScreen2DImmediate.Action_End2D();

        if (State.GetGameStateB("GameOver"))
        {
          GameOver();
          return;
        }
        //else if (Manager.GetGameStateF("PlayCutsceneSequence", -1) != -1)
        //{
        //  PlayCutsceneSequence(new object[] { Manager.GetGameStateF("PlayCutsceneSequence", -1) });
        //  return;
        //}
        else if (State.GetGameStateB("GameWon"))
        {
          GameWonSequence();
          return;
        }

        MakePlayer?.Invoke();
        PlayerCameraInfo.SetPlayerLook();

        FadeIn();
        State.IsCutsceneMode = false;
      }
      else
      {
         EventQueue.Add(Game.GameTime + 0.0001f, FadeInterim);
      }
    }

    public void FadeIn()
    {
      Engine.PlayerCameraInfo.FadeIn();
    }

    public void GameOver()
    {
      Engine.PlayerCameraInfo.FadeIn(2.5f);

      SoundManager.Clear();

      Engine.Screen2D.CurrentPage = new GameOver(Engine.Screen2D);
      Engine.Screen2D.ShowPage = true;
      Game.IsPaused = true;
      SoundManager.SetMusic(Info.Music_Lose);
    }

    internal void LostWing()
    {
      float t = Game.GameTime;
      float t2 = Game.GameTime + 3;
      while (t < t2)
      {
        EventQueue.Add(t, LostSound);
        t += 0.2f;
      }
      State.TimeSinceLostWing = t2;
      State.LastLostSoundTime = t2;
    }

    internal void LostShip()
    {
      float t = Game.GameTime;
      float t2 = Game.GameTime + 3;
      while (t < t2)
      {
        EventQueue.Add(t, LostSound);
        t += 0.2f;
      }
      State.TimeSinceLostShip = t2;
      State.LastLostSoundTime = t2;
    }

    internal void LostStructure()
    {
      float t = Game.GameTime;
      float t2 = Game.GameTime + 3;
      while (t < t2)
      {
        EventQueue.Add(t, LostSound);
        t += 0.2f;
      }
      State.TimeSinceLostStructure = t2;
      State.LastLostSoundTime = t2;
    }

    private void LostSound()
    {
      if (!State.IsCutsceneMode)
        SoundManager.SetSound(SoundGlobals.LostShip);
    }

    public HashSet<ActorInfo> GetRegister(string key)
    {
      switch (key.ToLower())
      {
        case "criticalallies":
          return State.CriticalAllies;
        case "criticalenemies":
          return State.CriticalEnemies;
        default:
          return null;
      }
    }

    public virtual void ProcessPlayerDying(ActorInfo a)
    {

    }

    public virtual void ProcessPlayerKilled(ActorInfo a)
    {
      State.IsCutsceneMode = false;
      EventQueue.Add(Game.GameTime + 3f, FadeOut);
      if (PlayerInfo.Lives == 0)
        State.SetGameStateB("GameOver", true);
    }

    public virtual void GameWonSequence()
    {
      Engine.PlayerCameraInfo.FadeIn(2.5f);

      SoundManager.Clear();

      Engine.Screen2D.CurrentPage = new GameWon(Engine.Screen2D);
      Engine.Screen2D.ShowPage = true;
      Game.IsPaused = true;
      SoundManager.SetMusic(Info.Music_Win);
    }
  }
}
