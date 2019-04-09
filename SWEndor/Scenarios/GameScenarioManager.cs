using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.UI;
using SWEndor.UI.Menu.Pages;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Scenarios
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
      ScenarioList.Add(new Scenarios.GSEndor());
      ScenarioList.Add(new Scenarios.GSYavin());
      ScenarioList.Add(new Scenarios.GSHoth());
      ScenarioList.Add(new Scenarios.GSTIEAdvanced());
      ScenarioList.Add(new Scenarios.GSTestZone());

      // Add scripted scenarios?
      if (Directory.Exists(Globals.CustomScenarioPath))
        foreach (string path in Directory.GetFiles(Globals.CustomScenarioPath, "*.scen"))
          ScenarioList.Add(new Scenarios.GSCustomScenario(path));
    }

    public List<GameScenarioBase> ScenarioList = new List<GameScenarioBase>();

    private Dictionary<string, float> GameStatesF = new Dictionary<string, float>();
    private Dictionary<string, string> GameStatesS = new Dictionary<string, string>();
    private Dictionary<string, bool> GameStatesB = new Dictionary<string, bool>();

    //private Dictionary<float, GameEvent> GameEvents = new Dictionary<float, GameEvent>();
    public GameScenarioBase Scenario = null;
    public ActorInfo SceneCamera = null;
    public ActorInfo CameraTargetActor = null;
    public TV_3DVECTOR CameraTargetPoint = new TV_3DVECTOR();

    public TV_3DVECTOR MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
    public TV_3DVECTOR MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

    // Actor Registers
    //public Dictionary<string, ActorInfo> AllyFighters = new Dictionary<string, ActorInfo>();
    //public Dictionary<string, ActorInfo> AllyShips = new Dictionary<string, ActorInfo>();
    //public Dictionary<string, ActorInfo> AllyStructures = new Dictionary<string, ActorInfo>();
    //public Dictionary<string, ActorInfo> EnemyFighters = new Dictionary<string, ActorInfo>();
    //public Dictionary<string, ActorInfo> EnemyShips = new Dictionary<string, ActorInfo>();
    public Dictionary<string, ActorInfo> EnemyStructures = new Dictionary<string, ActorInfo>();
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
      Screen2D.Instance().CurrentPage = new MainMenu();
      Scenario = new Scenarios.GSMainMenu();
      Scenario.Load(null, "");
      Scenario.Launch();
    }

    public void LoadInitial()
    {
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

          if (Scenario != null && Scenario.ActiveActor == kvp.Value)
            Scenario.ActiveActor = null;

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

      UpdateActorLists(CriticalAllies);
      UpdateActorLists(CriticalEnemies);

      if (Scenario != null)
        Scenario.GameTick();

      GameEventQueue.Process();
    }

    public void LoadInvisibleCam()
    {
      if (SceneCamera != null)
      {
        SceneCamera.Kill();
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

      PlayerInfo.Instance().Score.Reset();

      _instance = new GameScenarioManager();
      _instance.LoadInitial();
    }

    public void AddEvent(float time, GameEvent gevent)
    {
      GameEventQueue.Add(time, gevent);
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
