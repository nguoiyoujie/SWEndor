using Primrose.Expressions;
using Primrose.Primitives.Extensions;
using SWEndor.ActorTypes;
using SWEndor.FileFormat.INI;
using SWEndor.Sound;
using System.IO;

namespace SWEndor.Scenarios
{
  public class GSCustomScenario : ScenarioBase
  {
    public GSCustomScenario(ScenarioManager manager, string masterfilepath) : base(manager)
    {
      FilePath = masterfilepath;
      string folderpath = Path.GetDirectoryName(masterfilepath);
      ScenarioFile s = new ScenarioFile(masterfilepath);

      Info.Name = s.General_Name;
      PlayerName = s.General_PlayerName;
      Info.Description = s.General_Desc;

      Info.AllowedWings = new ActorTypeInfo[s.General_Wings.Length];
      for (int i = 0; i < Info.AllowedWings.Length; i++)
        Info.AllowedWings[i] = Engine.ActorTypeFactory.Get(s.General_Wings[i].Trim());

      Info.AllowedDifficulties = s.General_Diffc;

      ScriptPaths = new string[s.Scripts_Paths.Length];
      for (int i = 0; i < ScriptPaths.Length; i++)
        ScriptPaths[i] = Path.Combine(folderpath, s.Scripts_Paths[i]);

      Fn_load = s.Bindings_Load;
      Fn_loadfaction = s.Bindings_LoadFaction;
      Fn_loadscene = s.Bindings_LoadScene;
      Fn_makeplayer = s.Bindings_MakePlayer;
      Fns_gametick = s.Bindings_Tick;

      Info.Music_Lose = s.Audio_Lose ?? MusicGlobals.DefaultLose;
      Info.Music_Win = s.Audio_Win ?? MusicGlobals.DefaultWin;
    }

    public readonly string PlayerName = "Pilot";
    public readonly string FilePath;
    public readonly string[] ScriptPaths;

    // major functions
    internal string Fn_load = "load";
    internal string Fn_loadfaction = "loadfaction";
    internal string Fn_loadscene = "loadscene";
    internal string Fn_makeplayer = "makeplayer";
    internal string[] Fns_gametick;

    internal void LoadScripts()
    {
      Script.Registry.Clear();
      Engine.ScriptContext.Reset();
      foreach (string scrfile in ScriptPaths)
      {
        ScriptFile f = new ScriptFile(Path.Combine(Globals.CustomScenarioPath, scrfile.Trim()));
        f.NewScriptEvent = ReadScript;
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
