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
      Info.Description = s.General_Desc.Replace('|', '\n');

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
      Engine.ScriptContext.Reset();
      foreach (string scrfile in ScriptPaths)
      {
        ScriptFile f = new ScriptFile(Path.Combine(Globals.CustomScenarioPath, scrfile.Trim()), Engine.ScriptContext);
        f.ScriptReadBegin = ReadScript;
        f.ReadFile();
      }
      Engine.ScriptContext.RunGlobalScript();
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
      Engine.ScriptContext.Reset();
    }

    public override void Launch()
    {
      // after scripts
      base.Launch();
      MakePlayer = fn_MakePlayer;
      Engine.ScriptContext.RunScript(Fn_load);
    }

    private void fn_MakePlayer() { Engine.ScriptContext.RunScript(Fn_makeplayer); }

    internal override void LoadFactions()
    {
      base.LoadFactions();
      Engine.ScriptContext.RunScript(Fn_loadfaction);
    }

    internal override void LoadScene()
    {
      base.LoadScene();
      Engine.ScriptContext.RunScript(Fn_loadscene);
    }

    public override void GameTick()
    {
      base.GameTick();
      foreach (string s in Fns_gametick)
        Engine.ScriptContext.RunScript(s);
    }
  }
}
