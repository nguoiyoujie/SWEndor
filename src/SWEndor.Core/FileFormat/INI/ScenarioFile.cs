namespace SWEndor.FileFormat.INI
{
  public class ScenarioFile : INIFile
  {
    // information
    [INIValue("General", "Name")]
    public string General_Name = "Untitled Custom Scenario";

    [INIValue("General", "PlayerName")]
    public string General_PlayerName = "Pilot";

    [INIValue("General", "Description")]
    public string General_Desc;

    [INIValue("General", "Wings")]
    public string[] General_Wings;

    [INIValue("General", "Difficulties")]
    public string[] General_Diffc;

    [INIKeyList("Scripts")]
    public string[] Scripts_Paths;

    [INIValue("Bindings", "Fn_load")]
    public string Bindings_Load = "load";

    [INIValue("Bindings", "Fn_loadfaction")]
    public string Bindings_LoadFaction = "loadfaction";

    [INIValue("Bindings", "Fn_loadscene")]
    public string Bindings_LoadScene = "loadscene";

    [INIValue("Bindings", "Fn_makeplayer")]
    public string Bindings_MakePlayer = "makeplayer";

    [INIValue("Bindings", "Fns_gametick")]
    public string[] Bindings_Tick;

    [INIValue("Audio", "Win")]
    public string Audio_Win;

    [INIValue("Audio", "Lose")]
    public string Audio_Lose;


    public ScenarioFile(string filepath) : base(filepath)
    {
    }
  }
}
