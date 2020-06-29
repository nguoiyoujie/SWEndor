using Primrose.FileFormat.INI;

namespace SWEndor.FileFormat.INI
{
  public class CampaignFile : INIFile
  {
    // information
    [INIValue("General", "Name")]
    public string General_Name = "Untitled Custom Campaign";

    [INIValue("General", "Description")]
    public string General_Desc;

    [INIKeyList("Scenarios")]
    public string[] ScenarioPaths;

    public CampaignFile() { }

    public CampaignFile(string filepath) : base(filepath) { }
  }
}
