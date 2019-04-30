using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes;
using SWEndor.AI;
using SWEndor.Player;
using SWEndor.Sound;
using SWEndor.UI.Menu.Pages;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Scenarios
{
  public class GameScenarioManager
  {
    public readonly Engine Engine;
    public Game Game { get { return Engine.Game; } }
    public TrueVision TrueVision { get { return Engine.TrueVision; } }
    public ActorInfo.Factory ActorFactory { get { return Engine.ActorFactory; } }
    public ActorTypeInfo.Factory ActorTypeFactory { get { return Engine.ActorTypeFactory; } }
    public ActionManager ActionManager { get { return Engine.ActionManager; } }
    public SoundManager SoundManager { get { return Engine.SoundManager; } }
    public LandInfo LandInfo { get { return Engine.LandInfo; } }
    public AtmosphereInfo AtmosphereInfo { get { return Engine.AtmosphereInfo; } }
    public PlayerInfo PlayerInfo { get { return Engine.PlayerInfo; } }
    public PlayerCameraInfo PlayerCameraInfo { get { return Engine.PlayerCameraInfo; } }
    public Screen2D Screen2D { get { return Engine.Screen2D; } }
    public Scripting.Expressions.Context ScriptContext { get { return Engine.ScriptContext; } }

    internal GameScenarioManager(Engine engine)
    {
      Engine = engine;
      ScenarioList.Add(new Scenarios.GSEndor(this));
      ScenarioList.Add(new Scenarios.GSYavin(this));
      ScenarioList.Add(new Scenarios.GSHoth(this));
      ScenarioList.Add(new Scenarios.GSTIEAdvanced(this));
      ScenarioList.Add(new Scenarios.GSTestZone(this));

      // Add scripted scenarios?
      if (Directory.Exists(Globals.CustomScenarioPath))
        foreach (string path in Directory.GetFiles(Globals.CustomScenarioPath, "*.scen"))
          ScenarioList.Add(new Scenarios.GSCustomScenario(this, path));
    }

    public List<GameScenarioBase> ScenarioList = new List<GameScenarioBase>();

    private Dictionary<string, float> GameStatesF = new Dictionary<string, float>();
    private Dictionary<string, string> GameStatesS = new Dictionary<string, string>();
    private Dictionary<string, bool> GameStatesB = new Dictionary<string, bool>();

    //private Dictionary<float, GameEvent> GameEvents = new Dictionary<float, GameEvent>();
    public GameScenarioBase Scenario = null;
    public int SceneCameraID = -1;
    public int CameraTargetActorID = -1;
    public TV_3DVECTOR CameraTargetPoint = new TV_3DVECTOR();

    public TV_3DVECTOR MaxBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinBounds = new TV_3DVECTOR(-20000, -1500, -20000);
    public TV_3DVECTOR MaxAIBounds = new TV_3DVECTOR(20000, 1500, 20000);
    public TV_3DVECTOR MinAIBounds = new TV_3DVECTOR(-20000, -1500, -20000);

    // Actor Registers
    public Dictionary<string, int> CriticalAllies = new Dictionary<string, int>();
    public Dictionary<string, int> CriticalEnemies = new Dictionary<string, int>();

    public string Line1Text = "";
    public string Line2Text = "";
    public string Line3Text = "";

    public TV_COLOR Line1Color = new TV_COLOR();
    public TV_COLOR Line2Color = new TV_COLOR();
    public TV_COLOR Line3Color = new TV_COLOR();

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
      LoadInvisibleCam();
      PlayerInfo.ActorID = SceneCameraID;
      PlayerInfo.IsMovementControlsEnabled = false;
    }

    public int UpdateActorLists(Dictionary<string, int> list)
    {
      int ret = 0;
      List<string> rm = new List<string>();
      foreach (KeyValuePair<string, int> kvp in list)
      {
        ActorInfo actor = Engine.ActorFactory.Get(kvp.Value);
        if (actor != null && actor.CreationState == CreationState.DISPOSED)
        {
          rm.Add(kvp.Key);

          if (ActorInfo.IsPlayer(Engine, actor.ID))
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
      ActorInfo cam = Engine.ActorFactory.Get(SceneCameraID);
      ActorInfo tgt = Engine.ActorFactory.Get(CameraTargetActorID);
      if (cam != null)
      {
        if (tgt != null)
        {
          CameraTargetPoint = tgt.GetPosition();
        }
        cam.LookAtPoint(CameraTargetPoint, true);
      }

      UpdateActorLists(CriticalAllies);
      UpdateActorLists(CriticalEnemies);

      if (Scenario != null
        && Scenario.Launched)
        Scenario.GameTick();

      GameEventQueue.Process(Engine);
    }

    public void LoadInvisibleCam()
    {
      ActorCreationInfo camaci = new ActorCreationInfo(Engine.ActorTypeFactory.Get("Invisible Camera"));
      camaci.CreationTime = Engine.Game.GameTime;
      camaci.InitialState = ActorState.NORMAL;
      camaci.Position = new TV_3DVECTOR(0, 0, 0);
      camaci.Rotation = new TV_3DVECTOR();

      ActorInfo.Kill(Engine, SceneCameraID);
      SceneCameraID = ActorInfo.Create(Engine.ActorFactory, camaci).ID;
      CameraTargetPoint = new TV_3DVECTOR(0, 0, 100);
    }

    public void Reset()
    {
      if (Scenario != null)
        Scenario.Unload();

      PlayerInfo.Score.Reset();

      LoadInitial();

      //_instance = new GameScenarioManager();
      //_instance.LoadInitial();
    }

    public void AddEvent(float time, GameEvent gevent, params object[] param)
    {
      GameEventQueue.Add(time, gevent, param);
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
