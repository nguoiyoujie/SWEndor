using System;
using MTV3D65;
using SWEndor.Actors;
using SWEndor.Scenarios;
using SWEndor.Input;
using SWEndor.Sound;
using SWEndor.UI;
using SWEndor.Weapons;
using SWEndor.ActorTypes;
using SWEndor.Player;
using SWEndor.UI.Forms;
using SWEndor.Actors.Data;
using System.Text;
using SWEndor.AI.Squads;
using SWEndor.Primitives;

namespace SWEndor
{
  public class Engine
  {
    internal Engine() { }

    private GameForm Form;
    private IntPtr Handle;
    public int ScreenWidth { get; internal set;}
    public int ScreenHeight { get; internal set; }
    public Random Random { get; } = new Random();

    // Data sets
    internal MaskDataSet MaskDataSet { get; private set; }
    internal ActorDataSet ActorDataSet { get; private set; }

    // Engine parts
    internal Game Game { get; private set; }
    internal SoundManager SoundManager { get; private set; }
    internal PerfManager PerfManager { get; private set; }
    internal PlayerInfo PlayerInfo { get; private set; }
    internal PlayerCameraInfo PlayerCameraInfo { get; private set; }

    // TrueVision part
    internal TrueVision TrueVision { get; private set; }

    // Engine parts dependent on TrueVision
    internal InputManager InputManager { get; private set; } 
    internal AtmosphereInfo AtmosphereInfo { get; private set; }
    internal LandInfo LandInfo { get; private set; }
    internal Screen2D Screen2D { get; private set; }

    // Factories and Registries
    internal Font.Factory FontFactory { get; private set; }
    internal ActorInfo.Factory ActorFactory { get; private set; }
    internal ActorTypeInfo.Factory ActorTypeFactory { get; private set; }
    internal Squadron.Factory SquadronFactory { get; private set; }

    // Engine pars to be loaded late
    // Requires ActorInfoType initialization
    internal GameScenarioManager GameScenarioManager { get; private set; }
    internal Scenarios.Scripting.Expressions.Context ScriptContext { get; private set; }

    public void Init()
    {
      MaskDataSet = new MaskDataSet();
      ActorDataSet = new ActorDataSet();

      Game = new Game(this);
      SoundManager = new SoundManager(this);
      PerfManager = new PerfManager(this);
      ActorFactory = new ActorInfo.Factory(this);
      ActorTypeFactory = new ActorTypeInfo.Factory(this);
      SquadronFactory = new Squadron.Factory();
      PlayerInfo = new PlayerInfo(this);

      FontFactory = new Font.Factory();
    }

    public void InitTrueVision()
    {
      if (Handle == null)
        throw new Exception("Engine is not attached to a window form!");

      TrueVision = new TrueVision(this);
      TrueVision.Init(Handle);

      InputManager = new InputManager(this);
      PlayerCameraInfo = new PlayerCameraInfo(this);

      AtmosphereInfo = new AtmosphereInfo(this);
      LandInfo = new LandInfo(this);
      Screen2D = new Screen2D(this);
    }

    public void Dispose()
    {
      SoundManager.Dispose();
      InputManager.Dispose();
      TrueVision.Dispose();
      GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
    }

    public void Load()
    {
      Screen2D.LoadingTextLines.Add(Globals.LoadingFlavourTexts[Random.Next(0, Globals.LoadingFlavourTexts.Count)]);
      ActorTypeFactory.RegisterBase();

      
      //Screen2D.LoadingTextLines.Add("Loading other actor definitions...");
      //ActorTypeFactory.LoadFromINI(Globals.ActorTypeINIPath);

      Screen2D.LoadingTextLines.Add("Loading weapons...");
      WeaponFactory.LoadFromINI(Globals.WeaponStatINIPath);
      WeaponLoadoutFactory.LoadFromINI(Globals.WeaponLoadoutStatINIPath);

      Screen2D.LoadingTextLines.Add("Loading sounds...");
      SoundManager.Initialize();
      SoundManager.Load();

      Screen2D.LoadingTextLines.Add("Loading dynamic music info...");
      SoundManager.Piece.Factory.LoadFromINI(Globals.DynamicMusicINIPath);

      AtmosphereInfo.LoadDefaults(true, true);
      LandInfo.LoadDefaults();

      ScriptContext = new Scenarios.Scripting.Expressions.SWContext(this);

      // late ActorType bindings...
      ActorTypeFactory.Initialise();

      Screen2D.LoadingTextLines.Add("Loading scenario engine...");
      GameScenarioManager = new GameScenarioManager(this);
      GameScenarioManager.LoadInitial();

      Screen2D.LoadingTextLines.Add("Loading main menu...");
      GameScenarioManager.LoadMainMenu();
    }

    Action<Engine, ActorInfo> process = ActorInfo.Process;
    Action<Engine, ActorInfo> processAI = ActorInfo.ProcessAI;
    Action<Engine, ActorInfo> processCollision = ActorInfo.ProcessCollision;
    public void Process() { ActorFactory.DoEach(process); }
    public void ProcessAI() { ActorFactory.DoEach(processAI); } //may ParallelDoEach
    public void ProcessCollision() { ActorFactory.DoEach(processCollision); }

    public void PreRender()
    {
      TrueVision.TVEngine.Clear();
      TrueVision.TVScene.FinalizeShadows();

      TrueVision.TVScreen2DText.Action_BeginText();
      int i = 0;
      while (Screen2D.LoadingTextLines.Count > 20)
      {
        Screen2D.LoadingTextLines.RemoveAt(0);
      }

      StringBuilder text = new StringBuilder();
      while (i < Screen2D.LoadingTextLines.Count)
      {
        text.Append(Screen2D.LoadingTextLines[i]);
        text.Append("\n");
        i++;
      }

      TrueVision.TVScreen2DText.TextureFont_DrawText(text.ToString(), 40, 40, new TV_COLOR(1,1,1,1).GetIntColor(), FontFactory.Get(Font.T12).ID);
      TrueVision.TVScreen2DText.Action_EndText();

      Screen2D.Draw();
      TrueVision.TVEngine.RenderToScreen();
    }

    public void Render()
    {
      TrueVision.TVEngine.Clear();

      AtmosphereInfo.Render();
      TrueVision.TVScene.FinalizeShadows();
      //LandInfo.Render();

      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_RENDER))
        TrueVision.TVScene.RenderAllMeshes(true); //RenderAll(true);

      Screen2D.Draw();
      Screen2D.CurrentPage?.RenderTick();

      TrueVision.TVEngine.RenderToScreen();
    }

    public void LinkForm(GameForm form)
    {
      Form = form;
    }

    public void LinkHandle(IntPtr handle)
    {
      Handle = handle;
    }

    public void BeginExit()
    {
      Game.Stop();
    }

    public void Exit()
    {
      GameScenarioManager.Scenario.Unload();
      Game.PrepExit();
      //Game.Stop();
      Form.Exit();
      Dispose();
    }
  }
}
