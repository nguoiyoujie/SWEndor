using SWEndor.ActorTypes;
using SWEndor.FileFormat.INI;
using SWEndor.FileFormat.Scripting;
using SWEndor.Player;
using SWEndor.Scenarios.Scripting;
using SWEndor.Scenarios.Scripting.Expressions;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSCustomScenario : GameScenarioBase
  {
    public GSCustomScenario(string masterfilepath)
    {
      FilePath = masterfilepath;
      INIFile f = new INIFile(masterfilepath);

      // [General]
      Name = f.GetStringValue("General", "Name", Name);
      PlayerName = f.GetStringValue("General", "PlayerName", PlayerName);

      Description = f.GetStringValue("General", "Description", Description).Replace('|', '\n');

      AllowedWings = new List<ActorTypeInfo>();
      foreach (string wing in f.GetStringList("General", "Wings", new string[0]))
        AllowedWings.Add(ActorTypeInfo.Factory.Get(wing.Trim()));

      AllowedDifficulties = new List<string>(f.GetStringList("General", "Difficulties", new string[] { "Normal" }));

      // [Script]
      ScriptPaths = f.GetStringList("Script", "Paths", new string[0]);

      Fn_load = f.GetStringValue("Script", "Fn_load", Fn_load);
      Fn_loadfaction = f.GetStringValue("Script", "Fn_loadfaction", Fn_loadfaction);
      Fn_loadscene = f.GetStringValue("Script", "Fn_loadscene", Fn_loadscene);
      Fn_makeplayer = f.GetStringValue("Script", "Fn_makeplayer", Fn_makeplayer);
      Fn_calibratescene = f.GetStringValue("Script", "Fn_calibratescene", Fn_calibratescene);
      Fns_gametick = new List<string>(f.GetStringList("Script", "Fns_gametick", new string[0]));
    }

    public readonly string PlayerName = "Pilot";
    public readonly string FilePath;
    public readonly string[] ScriptPaths;

    // major functions
    public string Fn_load = "load";
    public string Fn_loadfaction = "loadfaction";
    public string Fn_loadscene = "loadscene";
    public string Fn_makeplayer = "makeplayer";
    public string Fn_calibratescene = "calibratescene";
    public List<string> Fns_gametick = new List<string>();

    public void LoadScripts()
    {
      Script.Registry.Clear();
      SWContext.Instance.Reset();
      foreach (string scrfile in ScriptPaths)
      {
        ScriptFile f = new ScriptFile(scrfile);
      }
    }

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      LoadScripts();
      base.Load(wing, difficulty);

      Globals.Engine.PlayerInfo.Name = PlayerName;
      Globals.Engine.Screen2D.LoadingTextLines.Add("starting game");
      Globals.Engine.Screen2D.LoadingTextLines.RemoveAt(0);
    }

    public override void Unload()
    {
      base.Unload();
      Script.Registry.Clear();
      SWContext.Instance.Reset();
    }

    public override void Launch()
    {
      // after scripts
      base.Launch();

      Script scr = Script.Registry.Get(Fn_load);
      if (scr != null)
        scr.Run();

      MakePlayer = delegate (object[] ps)
      {
        Script smk = Script.Registry.Get(Fn_makeplayer);
        if (smk != null)
          smk.Run();
      };
    }

    public override void LoadFactions()
    {
      base.LoadFactions();

      Script scr = Script.Registry.Get(Fn_loadfaction);
      if (scr != null)
        scr.Run();
    }

    public override void LoadScene()
    {
      base.LoadScene();

      Script scr = Script.Registry.Get(Fn_loadscene);
      if (scr != null)
        scr.Run();
    }

    public override void GameTick()
    {
      base.GameTick();
      CalibrateSceneObjects();

      foreach (string s in Fns_gametick)
      {
        Script scr = Script.Registry.Get(s);
        if (scr != null)
          scr.Run();
      }
    }

    private void CalibrateSceneObjects()
    {
      Script scr = Script.Registry.Get(Fn_calibratescene);
      if (scr != null)
        scr.Run();
    }
  }
}
