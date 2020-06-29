using Primrose.Primitives.Factories;
using SWEndor.Actors;
using SWEndor.Core;
using Primrose.FileFormat.INI;
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

      CreateMainCampaign();
      LoadCampaigns();
    }

    public List<CampaignInfo> CampaignList = new List<CampaignInfo>();
    public List<ScenarioBase> ScenarioList = new List<ScenarioBase>();

    public ScenarioBase Scenario = null;
    public ScenarioBase DesignatedMainMenuScenario;

    public void CreateMainCampaign()
    {
      CampaignInfo c = new CampaignInfo(this);

      c.Name = "Main Campaigns";
      c.Description = "Play the main campaign.";

      c.Scenarios.Add(new GSEndor(this));
      //c.Scenarios.Add(new GSYavin(this));
      //c.Scenarios.Add(new GSHoth(this));
      //c.Scenarios.Add(new GSTIEAdvanced(this));
      //c.Scenarios.Add(new GSTestZone(this));

      CampaignList.Add(c);
    }

    public void LoadCampaigns()
    {
      if (Directory.Exists(Globals.CustomScenarioPath))
      {
        string fpath = Path.Combine(Globals.CustomScenarioPath, "campaigns.ini");
        if (File.Exists(fpath))
        {
          INIFile f = new INIFile(fpath);

          // [Campaigns]
          if (f.HasSection("Campaigns"))
            foreach (INIFile.INISection.INILine ln in f.GetSection("Campaigns").Lines)
              if (ln.HasKey)
                CampaignList.Add(new CampaignInfo(this, Path.Combine(Globals.CustomScenarioPath, ln.Key)));
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
    }
  }
}
