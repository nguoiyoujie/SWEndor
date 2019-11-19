using MTV3D65;
using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Core;
using SWEndor.Models;
using SWEndor.Player;
using SWEndor.Sound;
using SWEndor.UI.Menu.Pages;
using System;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class ScenarioInfo
  {
    private static ActorTypeInfo[] _defaultAllowedWings = new ActorTypeInfo[0];
    private static string[] _defaultAllowedDifficulties = new string[] { "normal" };

    public string Name;
    public string Description;
    public ActorTypeInfo[] AllowedWings;
    public string[] AllowedDifficulties;

    public ScenarioInfo InitDefault()
    {
      Name = "Untitled Scenario";
      Description = "";
      AllowedWings = _defaultAllowedWings;
      AllowedDifficulties = _defaultAllowedDifficulties;
      return this;
    }
  }

  public class ScenarioStates
  {
    // Setup states
    public string Difficulty;
    public int StageNumber;
    public bool Launched = false;

    // Runtime states
    public bool IsCutsceneMode = false;

    public TV_3DVECTOR MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
    public TV_3DVECTOR MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

    public bool InformLostWing = false;
    public bool InformLostShip = false;
    public bool InformLostStructure = false;
    public float TimeSinceLostWing = -100;
    public float TimeSinceLostShip = -100;
    public float TimeSinceLostStructure = -100;
    internal float LastLostSoundTime = 0;

    public Registry<float> GameStatesF = new Registry<float>();
    public Registry<string> GameStatesS = new Registry<string>();
    public Registry<bool> GameStatesB = new Registry<bool>();
    public HashSet<ActorInfo> CriticalAllies = new HashSet<ActorInfo>();
    public HashSet<ActorInfo> CriticalEnemies = new HashSet<ActorInfo>();

    public Octree_String Octree = new Octree_String(new TV_3DVECTOR(), 100000, 10);



    public bool GetGameStateB(string key) { return GameStatesB.Get(key); }
    public float GetGameStateF(string key) { return GameStatesF.Get(key); }
    public string GetGameStateS(string key) { return GameStatesS.Get(key); }
    public bool GetGameStateB(string key, bool defaultValue) { return GameStatesB.Get(key, defaultValue); }
    public float GetGameStateF(string key, float defaultValue) { return GameStatesF.Get(key, defaultValue); }
    public string GetGameStateS(string key, string defaultValue) { return GameStatesS.Get(key, defaultValue); }
    public void SetGameStateB(string key, bool value) { GameStatesB.Put(key, value); }
    public void SetGameStateF(string key, float value) { GameStatesF.Put(key, value); }
    public void SetGameStateS(string key, string value) { GameStatesS.Put(key, value); }

    public void Reset()
    {
      MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
      MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
      MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
      MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

      InformLostWing = false;
      InformLostShip = false;
      InformLostStructure = false;
      TimeSinceLostWing = -100;
      TimeSinceLostShip = -100;
      TimeSinceLostStructure = -100;
      LastLostSoundTime = 0;

      GameStatesF.Clear();
      GameStatesB.Clear();
      GameStatesS.Clear();
      CriticalAllies.Clear();
      CriticalEnemies.Clear();
    }
  }

  public class ScenarioBase
  {
    public readonly ScenarioManager Manager;
    internal GameEventQueue EventQueue = new GameEventQueue();
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

    public ScenarioBase(ScenarioManager manager) { Manager = manager; }

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
    }

    internal virtual void LoadFactions()
    {
      FactionInfo.Factory.Clear();
    }

    internal virtual void LoadScene()
    {
      Engine.TrueVision.TVGraphicEffect.FadeIn();
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

      SoundManager.Mood = MoodStates.AMBIENT;
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
      Engine.TrueVision.TVGraphicEffect.FadeOut();
      EventQueue.Add(Game.GameTime + 0.01f, FadeInterim);
    }

    public void FadeInterim()
    {
      if (Engine.TrueVision.TVGraphicEffect.IsFadeFinished())
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
      Engine.TrueVision.TVGraphicEffect.FadeIn();
    }

    public void GameOver()
    {
      Engine.TrueVision.TVGraphicEffect.FadeIn(2.5f);

      SoundManager.SetSoundStopAll();

      Engine.Screen2D.CurrentPage = new GameOver(Engine.Screen2D);
      Engine.Screen2D.ShowPage = true;
      Game.IsPaused = true;
      SoundManager.SetMusic("battle_3_2"); // TO-DO: make configurable
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
        SoundManager.SetSound(SoundGlobals.LostShip, true);
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
      State.IsCutsceneMode = true;
      EventQueue.Add(Game.GameTime + 3f, FadeOut);
      if (PlayerInfo.Lives == 0)
        State.SetGameStateB("GameOver", true);
    }

    public virtual void GameWonSequence()
    {
      Engine.TrueVision.TVGraphicEffect.FadeIn(2.5f);

      SoundManager.SetSoundStopAll();

      Engine.Screen2D.CurrentPage = new GameWon(Engine.Screen2D);
      Engine.Screen2D.ShowPage = true;
      Game.IsPaused = true;
      SoundManager.SetMusic("finale_3_1");
      SoundManager.SetMusicLoop("credits_3_1");
    }
  }
}
