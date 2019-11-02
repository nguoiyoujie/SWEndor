using MTV3D65;
using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.UI.Menu.Pages;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Scenarios
{
  public class GameScenarioManager
  {
    public readonly Engine Engine;

    internal GameScenarioManager(Engine engine)
    {
      Engine = engine;
      ScenarioList.Add(new GSEndor(this));
      ScenarioList.Add(new GSYavin(this));
      ScenarioList.Add(new GSHoth(this));
      ScenarioList.Add(new GSTIEAdvanced(this));
      ScenarioList.Add(new GSTestZone(this));

      // Add scripted scenarios?
      if (Directory.Exists(Globals.CustomScenarioPath))
        foreach (string path in Directory.GetFiles(Globals.CustomScenarioPath, "*.scen"))
          ScenarioList.Add(new GSCustomScenario(this, path));
    }

    public List<GameScenarioBase> ScenarioList = new List<GameScenarioBase>();

    private Dictionary<string, float> GameStatesF = new Dictionary<string, float>();
    private Dictionary<string, string> GameStatesS = new Dictionary<string, string>();
    private Dictionary<string, bool> GameStatesB = new Dictionary<string, bool>();

    public GameScenarioBase Scenario = null;

    public TV_3DVECTOR MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
    public TV_3DVECTOR MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

    public Octree_String Octree = new Octree_String(new TV_3DVECTOR(), 100000, 10);

    // Actor Registers
    public HashSet<ActorInfo> CriticalAllies = new HashSet<ActorInfo>();
    public HashSet<ActorInfo> CriticalEnemies = new HashSet<ActorInfo>();

    public string Line1Text = "";
    public string Line2Text = "";
    public string Line3Text = "";

    public COLOR Line1Color = ColorLocalization.Get(ColorLocalKeys.WHITE);
    public COLOR Line2Color = ColorLocalization.Get(ColorLocalKeys.WHITE);
    public COLOR Line3Color = ColorLocalization.Get(ColorLocalKeys.WHITE);

    public bool IsCutsceneMode = false;

    public void LoadMainMenu()
    {
      Engine.Game.IsPaused = false;
      Engine.Screen2D.ShowPage = true;
      Engine.Screen2D.CurrentPage = new MainMenu(Engine.Screen2D);
      Scenario = new GSMainMenu(this);
      Scenario.Load(null, "");
      Scenario.Launch();
    }

    public void LoadInitial()
    {
      Engine.PlayerInfo.Score.Reset();
      Engine.PlayerInfo.IsMovementControlsEnabled = false;
    }

    public void UpdateActorLists(HashSet<ActorInfo> list)
    {
      foreach (ActorInfo a in new List<ActorInfo>(list))
        if (a != null && a.DisposingOrDisposed)
          list.Remove(a);
    }

    public void Update()
    {
#if DEBUG
      if (Scenario == null)
        throw new InvalidOperationException("Unexpected null Scenario");
#endif

      if (Scenario.Launched)
        Scenario.GameTick();

      UpdateActorLists(CriticalAllies);
      UpdateActorLists(CriticalEnemies);

      GameEventQueue.Process(Engine);
    }

    public void Reset()
    {
      if (Scenario != null)
        Scenario.Unload();

      LoadInitial();
    }

    public void AddEvent(float time, GameEvent gevent)
    {
      GameEventQueue.Add(time, gevent);
    }

    public void AddEvent<T>(float time, GameEvent<T> gevent, T arg)
    {
      GameEventQueue.Add(time, gevent, arg);
    }

    public void AddEvent<T1, T2>(float time, GameEvent<T1, T2> gevent, T1 a1, T2 a2)
    {
      GameEventQueue.Add(time, gevent, a1, a2);
    }

    public void AddEvent<T1, T2, T3>(float time, GameEvent<T1, T2, T3> gevent, T1 a1, T2 a2, T3 a3)
    {
      GameEventQueue.Add(time, gevent, a1, a2, a3);
    }

    public void ClearEvents()
    {
      GameEventQueue.Clear();
    }

    #region GameStates
    public void ClearGameStates()
    {
      GameStatesF.Clear();
      GameStatesB.Clear();
      GameStatesS.Clear();
      CriticalAllies.Clear();
      CriticalEnemies.Clear();
    }

    public bool IsGameStateFDefined(string key)
    {
      return GameStatesF.ContainsKey(key);
    }

    public string[] GetGameStateFKeys()
    {
      string[] ret = new string[GameStatesF.Count];
      GameStatesF.Keys.CopyTo(ret, 0);
      return ret;
    }

    public void SetGameStateF(string key, float value)
    {
      if (GameStatesF.ContainsKey(key))
      {
        GameStatesF[key] = value;
      }
      else
      {
        GameStatesF.Add(key, value);
      }
    }

    public float GetGameStateF(string key, float defaultvalue = 0)
    {
      return (GameStatesF.ContainsKey(key)) ? GameStatesF[key] : defaultvalue;
    }

    public bool IsGameStateBDefined(string key)
    {
      return GameStatesB.ContainsKey(key);
    }

    public string[] GetGameStateBKeys()
    {
      string[] ret = new string[GameStatesB.Count];
      GameStatesB.Keys.CopyTo(ret, 0);
      return ret;
    }

    public void SetGameStateB(string key, bool value)
    {
      if (GameStatesB.ContainsKey(key))
      {
        GameStatesB[key] = value;
      }
      else
      {
        GameStatesB.Add(key, value);
      }
    }

    public bool GetGameStateB(string key, bool defaultvalue = false)
    {
      return (GameStatesB.ContainsKey(key)) ? GameStatesB[key] : defaultvalue;
    }

    public bool IsGameStateSDefined(string key)
    {
      return GameStatesS.ContainsKey(key);
    }

    public string[] GetGameStateSKeys()
    {
      string[] ret = new string[GameStatesS.Count];
      GameStatesS.Keys.CopyTo(ret, 0);
      return ret;
    }

    public void SetGameStateS(string key, string value)
    {
      if (GameStatesS.ContainsKey(key))
      {
        GameStatesS[key] = value;
      }
      else
      {
        GameStatesS.Add(key, value);
      }
    }

    public string GetGameStateS(string key, string defaultvalue = "")
    {
      return (GameStatesS.ContainsKey(key)) ? GameStatesS[key] : defaultvalue;
    }

    #endregion
  }
}
