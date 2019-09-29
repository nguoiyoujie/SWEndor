using SWEndor.ActorTypes;
using SWEndor.FileFormat.INI;
using SWEndor.FileFormat.Scripting;
using SWEndor.Scenarios.Scripting;
using System.Collections.Generic;

namespace SWEndor.Scenarios
{
  public class GSCustomScenario : GameScenarioBase
  {
    public GSCustomScenario(GameScenarioManager manager, string masterfilepath) : base(manager)
    {
      FilePath = masterfilepath;
      INIFile f = new INIFile(masterfilepath);

      // [General]
      Name = f.GetStringValue("General", "Name", Name);
      PlayerName = f.GetStringValue("General", "PlayerName", PlayerName);

      Description = f.GetStringValue("General", "Description", Description).Replace('|', '\n');

      AllowedWings = new List<ActorTypeInfo>();
      foreach (string wing in f.GetStringList("General", "Wings", new string[0]))
        AllowedWings.Add(ActorTypeFactory.Get(wing.Trim()));

      AllowedDifficulties = new List<string>(f.GetStringList("General", "Difficulties", new string[] { "Normal" }));

      // [Script]
      ScriptPaths = f.GetStringList("Script", "Paths", new string[0]);

      Fn_load = f.GetStringValue("Script", "Fn_load", Fn_load);
      Fn_loadfaction = f.GetStringValue("Script", "Fn_loadfaction", Fn_loadfaction);
      Fn_loadscene = f.GetStringValue("Script", "Fn_loadscene", Fn_loadscene);
      Fn_makeplayer = f.GetStringValue("Script", "Fn_makeplayer", Fn_makeplayer);
      //Fn_calibratescene = f.GetStringValue("Script", "Fn_calibratescene", Fn_calibratescene);
      Fns_gametick = f.GetStringList("Script", "Fns_gametick", new string[0]);
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
    public string[] Fns_gametick;

    public void LoadScripts()
    {
      Script.Registry.Clear();
      Engine.ScriptContext.Reset();
      foreach (string scrfile in ScriptPaths)
        new ScriptFile(scrfile);
    }

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      LoadScripts();
      base.Load(wing, difficulty);

      PlayerInfo.Name = PlayerName;
      Screen2D.LoadingTextLines.Add("starting game");
      Screen2D.LoadingTextLines.RemoveAt(0);
    }

    public override void Unload()
    {
      base.Unload();
      Script.Registry.Clear();
      Engine.ScriptContext.Reset();
    }

    public override void Launch()
    {
      // after scripts
      base.Launch();
      MakePlayer = fn_MakePlayer;
      Script.Registry.Get(Fn_load)?.Run(Engine.ScriptContext);
    }

    private void fn_MakePlayer() { Script.Registry.Get(Fn_makeplayer)?.Run(Engine.ScriptContext); }

    public override void LoadFactions()
    {
      base.LoadFactions();
      Script.Registry.Get(Fn_loadfaction)?.Run(Engine.ScriptContext);
    }

    public override void LoadScene()
    {
      base.LoadScene();
      Script.Registry.Get(Fn_loadscene)?.Run(Engine.ScriptContext);
    }

    public override void GameTick()
    {
      base.GameTick();
      foreach (string s in Fns_gametick)
        Script.Registry.Get(s)?.Run(Engine.ScriptContext);
    }
  }
}
