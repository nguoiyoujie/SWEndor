using SWEndor.Actors;
using SWEndor.Core;
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

    public List<ScenarioBase> ScenarioList = new List<ScenarioBase>();
    public ScenarioBase Scenario = null;

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
