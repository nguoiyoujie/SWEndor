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
  public class CampaignInfo
  {
    internal CampaignInfo(ScenarioManager manager)
    {
      Manager = manager;
    }

    internal CampaignInfo(ScenarioManager manager, string path)
    {
      Manager = manager;
      LoadCustomScenarios(path);
    }

    public ScenarioManager Manager;
    public string Name;
    public string Description;
    public List<ScenarioBase> Scenarios = new List<ScenarioBase>();

    public void LoadCustomScenarios(string path)
    {
      string dir = Path.GetDirectoryName(path);
      if (Directory.Exists(dir))
      {
        INIFile f = new INIFile(path);

        // [General]
        if (f.HasSection("General"))
        {
          Name = f.GetString("General", "Name", "");
          Description = f.GetString("General", "Description", "").Replace('|', '\n');
        }

        // [Scenarios]
        if (f.HasSection("Scenarios"))
          foreach (INIFile.INISection.INILine ln in f.GetSection("Scenarios").Lines)
            if (ln.HasKey)
              Scenarios.Add(new GSCustomScenario(Manager, Path.Combine(dir, ln.Key)));
      }
    }
  }
}
