using SWEndor.Actors;
using SWEndor.Core;
using SWEndor.FileFormat.INI;
using SWEndor.Primitives.Extensions;
using SWEndor.UI.Menu.Pages;
using System;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Scenarios
{
  public class ScenarioManager
  {
    public readonly Engine Engine;

    internal ScenarioManager(Engine engine)
    {
      Engine = engine;
      DesignatedMainMenuScenario = new GSMainMenu(this);

      ScenarioList.Add(new GSEndor(this));
      ScenarioList.Add(new GSYavin(this));
      ScenarioList.Add(new GSHoth(this));
      ScenarioList.Add(new GSTIEAdvanced(this));
      ScenarioList.Add(new GSTestZone(this));

      LoadCustomScenarios();
    }

    public List<ScenarioBase> ScenarioList = new List<ScenarioBase>();
    public ScenarioBase Scenario = null;
    public ScenarioBase DesignatedMainMenuScenario;

    public void LoadCustomScenarios()
    {
      if (Directory.Exists(Globals.CustomScenarioPath))
      {
        string fpath = Path.Combine(Globals.CustomScenarioPath, "scenarios.ini");
        if (File.Exists(fpath))
        {
          INIFile f = new INIFile(fpath);

          // [Scenarios]
          //List<string> paths = new List<string>();
          if (f.HasSection("Scenarios"))
            foreach (INIFile.INISection.INILine ln in f.GetSection("Scenarios").Lines)
              if (ln.HasKey)
                ScenarioList.Add(new GSCustomScenario(this, Path.Combine(Globals.CustomScenarioPath, ln.Key)));
        }
      }
    }



    public void LoadMainMenu()
    {
      Engine.Game.IsPaused = false;
      Engine.Screen2D.ShowPage = true;
      Engine.Screen2D.CurrentPage = new MainMenu(Engine.Screen2D);
      Scenario = DesignatedMainMenuScenario;
      Scenario.Load(null, "");
      Scenario.Launch();
    }

    public bool IsMainMenu { get { return Scenario == DesignatedMainMenuScenario; } }

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

      if (Scenario.State.Launched)
        Scenario.GameTick();

      Scenario.State.CriticalAllies.RemoveDisposed();
      Scenario.State.CriticalEnemies.RemoveDisposed();

      Scenario.EventQueue.Process(Engine);
    }

    public void Reset()
    {
      Scenario?.Unload();

      LoadInitial();
    }
  }
}
