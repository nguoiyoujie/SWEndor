using SWEndor.FileFormat.INI;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Game.Scenarios
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
        CampaignFile c = new CampaignFile(path);

        Name = c.General_Name;
        Description = c.General_Desc;

        foreach (string s in c.ScenarioPaths)
          Scenarios.Add(new GSCustomScenario(Manager, Path.Combine(dir, s)));
      }
    }
  }
}
