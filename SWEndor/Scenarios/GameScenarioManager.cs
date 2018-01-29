using MTV3D65;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace SWEndor
{
  public class GameScenarioManager
  {
    private static GameScenarioManager _instance;
    public static GameScenarioManager Instance()
    {
      if (_instance == null) { _instance = new GameScenarioManager(); }
      return _instance;
    }

    private GameScenarioManager()
    {
      StageNumber = 0;
      prevStageNumber = 0;

      ScenarioList.Add(new Scenarios.GSEndor());
      ScenarioList.Add(new Scenarios.GSYavin());
      ScenarioList.Add(new Scenarios.GSTIEAdvanced());
      ScenarioList.Add(new Scenarios.GSTestZone());
    }

    // To remove
    //public int EndorKey { get; private set; }
    //public int DeathStarKey { get; private set; }
    //public int SceneRoomKey { get; private set; }


    public List<GameScenarioBase> ScenarioList = new List<GameScenarioBase>();


    public string Difficulty { get; set; }
    public int StageNumber { get; set; }
    public int prevStageNumber { get; private set; }

    private Dictionary<string, float> GameStatesF = new Dictionary<string, float>();
    private Dictionary<string, string> GameStatesS = new Dictionary<string, string>();
    private Dictionary<string, bool> GameStatesB = new Dictionary<string, bool>();

    private Dictionary<float, string> GameEvents = new Dictionary<float, string>();
    public GameScenarioBase Scenario = null;
    public ActorInfo SceneCamera = null;
    public ActorInfo CameraTargetActor = null;
    public TV_3DVECTOR CameraTargetPoint = new TV_3DVECTOR();

    public TV_3DVECTOR MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
    public TV_3DVECTOR MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

    public int TIEWaves = 0;
    public int SDWaves = 0;

    public Dictionary<string, ActorInfo> AllyFighters = new Dictionary<string, ActorInfo>();
    public Dictionary<string, ActorInfo> AllyShips = new Dictionary<string, ActorInfo>();
    public Dictionary<string, ActorInfo> EnemyFighters = new Dictionary<string, ActorInfo>();
    public Dictionary<string, ActorInfo> EnemyShips = new Dictionary<string, ActorInfo>();

    public Dictionary<string, ActorInfo> CriticalAllies = new Dictionary<string, ActorInfo>();
    public Dictionary<string, ActorInfo> CriticalEnemies = new Dictionary<string, ActorInfo>();

    public string Line1Text = "";
    public string Line2Text = "";
    public string Line3Text = "";

    public TV_COLOR Line1Color = new TV_COLOR();
    public TV_COLOR Line2Color = new TV_COLOR();
    public TV_COLOR Line3Color = new TV_COLOR();

    public bool IsCutsceneMode = false;

    public void LoadMainMenu()
    {
      Game.Instance().IsPaused = false;
      Screen2D.Instance().ShowPage = true;
      Screen2D.Instance().CurrentPage = new UIPage_MainMenu();
      Scenario = new Scenarios.GSMainMenu();
      Scenario.Load(null, "");
    }

    public void LoadInitial()
    {
      //LoadFactions();
      //LoadScene();
      LoadInvisibleCam();
      PlayerInfo.Instance().Actor = SceneCamera;
      PlayerInfo.Instance().IsMovementControlsEnabled = false;
    }

    public int UpdateActorLists(Dictionary<string, ActorInfo> list)
    {
      int ret = 0;
      List<string> rm = new List<string>();
      foreach (KeyValuePair<string, ActorInfo> kvp in list)
      {
        if (kvp.Value.CreationState == CreationState.DISPOSED)
        {
          rm.Add(kvp.Key);
          if (kvp.Value != PlayerInfo.Instance().Actor)
          {
            ret++;
          }
        }
      }

      foreach (string rs in rm)
      {
        list.Remove(rs);
      }
      return ret;
    }

    public void Update()
    {
      if (SceneCamera != null) //&& SceneCamera.Mesh != null)
      {
        if (CameraTargetActor != null)
        {
          SceneCamera.LookAtPoint(CameraTargetActor.GetPosition(), true);
        }
        else
        {
          SceneCamera.LookAtPoint(CameraTargetPoint, true);
        }
      }

      UpdateActorLists(EnemyFighters);
      UpdateActorLists(EnemyShips);
      UpdateActorLists(CriticalAllies);
      UpdateActorLists(CriticalEnemies);

      if (Scenario != null)
        Scenario.GameTick();

      List<float> remove = new List<float>();
      float[] gekeys = new float[GameEvents.Count];
      GameEvents.Keys.CopyTo(gekeys, 0);
      for (int i = 0; i < gekeys.Length; i++)
      {
        float gekey = gekeys[i];
        GameEvent ge = GameEvent.GetEvent(GameEvents[gekey]);
        if (ge == null || ge.Method == null)
        {
          remove.Add(gekey);
        }
        else if (gekey < Game.Instance().GameTime)
        {
          remove.Add(gekey);
          ge.Method(null);
        }
      }

      foreach (float f in remove)
      {
        GameScenarioManager.Instance().GameEvents.Remove(f);
      }

      prevStageNumber = StageNumber;
    }

    public void LoadInvisibleCam()
    {
      if (SceneCamera != null)
      {
        SceneCamera.Destroy();
      }

      ActorCreationInfo camaci = new ActorCreationInfo(InvisibleCameraATI.Instance());
      camaci.CreationTime = Game.Instance().GameTime;
      camaci.InitialState = ActorState.NORMAL;
      camaci.Position = new TV_3DVECTOR(0, 0, 0);
      camaci.Rotation = new TV_3DVECTOR();
      SceneCamera = ActorInfo.Create(camaci);
      CameraTargetPoint = new TV_3DVECTOR(0, 0, 100);
    }

    public void Reset()
    {
      if (Scenario != null)
        Scenario.Unload();

      _instance = new GameScenarioManager();
      _instance.LoadInitial();
    }

    public void AddEvent(float time, string eventname)
    {
      while (GameEvents.ContainsKey(time))
      {
        time += 0.01f;
      }
      GameEvents.Add(time, eventname);
    }

    public void ClearEvents()
    {
      GameEvents.Clear();
    }

    #region GameStates
    public void ClearGameStates()
    {
      GameStatesF.Clear();
      GameStatesB.Clear();
      GameStatesS.Clear();
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

    public Dictionary<float, string> GetGameEvents()
    {
      return GameEvents;
    }
  }
}
