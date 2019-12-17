using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using SWEndor.ActorTypes;
using SWEndor.FileFormat.INI;
using SWEndor.FileFormat.Scripting;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.Scenarios
{
  public class GSCustomScenario : ScenarioBase
  {
    public GSCustomScenario(ScenarioManager manager, string masterfilepath) : base(manager)
    {
      FilePath = masterfilepath;
      INIFile f = new INIFile(masterfilepath);

      // [General]
      Info.Name = f.GetString("General", "Name", "Untitled Custom Scenario");
      PlayerName = f.GetString("General", "PlayerName", PlayerName);
      Info.Description = f.GetString("General", "Description", "").Replace('|', '\n');

      List<ActorTypeInfo> allowedWings = new List<ActorTypeInfo>();
      foreach (string wing in f.GetStringArray("General", "Wings", new string[0]))
        allowedWings.Add(ActorTypeFactory.Get(wing.Trim()));
      Info.AllowedWings = allowedWings.ToArray();

      Info.AllowedDifficulties = f.GetStringArray("General", "Difficulties", new string[] { "Normal" });

      // [Script]
      ScriptPaths = f.GetStringArray("Script", "Paths", new string[0]);

      Fn_load = f.GetString("Script", "Fn_load", Fn_load);
      Fn_loadfaction = f.GetString("Script", "Fn_loadfaction", Fn_loadfaction);
      Fn_loadscene = f.GetString("Script", "Fn_loadscene", Fn_loadscene);
      Fn_makeplayer = f.GetString("Script", "Fn_makeplayer", Fn_makeplayer);
      Fns_gametick = f.GetStringArray("Script", "Fns_gametick", new string[0]);

      Info.Music_Lose = f.GetString("Audio", "Lose", Info.Music_Lose);
      Info.Music_Win = f.GetString("Audio", "Win", Info.Music_Win);
    }

    public readonly string PlayerName = "Pilot";
    public readonly string FilePath;
    public readonly string[] ScriptPaths;

    // major functions
    internal string Fn_load = "load";
    internal string Fn_loadfaction = "loadfaction";
    internal string Fn_loadscene = "loadscene";
    internal string Fn_makeplayer = "makeplayer";
    internal string Fn_calibratescene = "calibratescene";
    internal string[] Fns_gametick;

    internal void LoadScripts()
    {
      Script.Registry.Clear();
      Engine.ScriptContext.Reset();
      foreach (string scrfile in ScriptPaths)
      {
        ScriptFile f = new ScriptFile(Path.Combine(Globals.CustomScenarioPath, scrfile.Trim()));
        f.ScriptReadDelegate = ReadScript;
        f.ReadFile();
      }
      Script.Registry.Global.Run(Engine.ScriptContext);
    }

    public void ReadScript(string name)
    {
      Engine.Screen2D.LoadingTextLines.Add("loading script:".C(name));
      Engine.Screen2D.LoadingTextLines.RemoveAt(0);
    }
    
    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      LoadScripts();
      base.Load(wing, difficulty);

      PlayerInfo.Name = PlayerName;
      Engine.Screen2D.LoadingTextLines.Add("starting game");
      Engine.Screen2D.LoadingTextLines.RemoveAt(0);
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

    internal override void LoadFactions()
    {
      base.LoadFactions();
      Script.Registry.Get(Fn_loadfaction)?.Run(Engine.ScriptContext);
    }

    internal override void LoadScene()
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
