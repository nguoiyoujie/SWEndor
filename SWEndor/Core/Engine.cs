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
using SWEndor.AI;

namespace SWEndor
{
  public class Engine
  {
    internal Engine() { }

    private GameForm Form;
    private IntPtr Handle;
    public float ScreenWidth { get; internal set;}
    public float ScreenHeight { get; internal set; }
    public Random Random { get; } = new Random();

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
    internal ActorInfo.Factory ActorFactory { get; private set; }
    internal ActorTypeInfo.Factory ActorTypeFactory { get; private set; }
    internal ActionManager ActionManager { get; private set; }

    // Engine pars to be loaded late
    // Requires ActorInfoType initialization
    internal GameScenarioManager GameScenarioManager { get; private set; }
    internal Scenarios.Scripting.Expressions.Context ScriptContext { get; private set; }

    public void Init()
    {
      Game = new Game(this);
      SoundManager = new SoundManager(this);
      PerfManager = new PerfManager(this);
      ActorFactory = new ActorInfo.Factory(this);
      ActorTypeFactory = new ActorTypeInfo.Factory(this);
      ActionManager = new ActionManager(this);
      PlayerInfo = new PlayerInfo(this);
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
      ActorTypeFactory.Initialise();

      Screen2D.LoadingTextLines.Add("Loading other actor definitions...");
      ActorTypeFactory.LoadFromINI(Globals.ActorTypeINIPath);

      Screen2D.LoadingTextLines.Add("Loading weapons...");
      WeaponFactory.LoadFromINI(Globals.WeaponStatINIPath);

      Screen2D.LoadingTextLines.Add("Loading sounds...");
      SoundManager.Initialize();
      SoundManager.Load();

      Screen2D.LoadingTextLines.Add("Loading dynamic music info...");
      SoundManager.Piece.Factory.LoadFromINI(Globals.DynamicMusicINIPath);

      AtmosphereInfo.LoadDefaults(true, true);
      LandInfo.LoadDefaults();

      ScriptContext = new Scenarios.Scripting.Expressions.SWContext(this);

      Screen2D.LoadingTextLines.Add("Loading scenario engine...");
      GameScenarioManager = new GameScenarioManager(this);
      GameScenarioManager.LoadInitial();

      Screen2D.LoadingTextLines.Add("Loading main menu...");
      GameScenarioManager.LoadMainMenu();
    }

    public void Process()
    {
      foreach (int actorID in ActorFactory.GetList())
      {
        ActorInfo a = ActorFactory.Get(actorID);
        if (a != null)
          a.Process();

        //if (a.IsPlayer())
        //  ProcessPlayerCamera();
      }
    }

    public void ProcessAI()
    {
      foreach (int actorID in ActorFactory.GetHoldingList())
      {
        ActorInfo a = ActorFactory.Get(actorID);
        if (a != null)
          a.ProcessAI();
      }
    }

    public void ProcessCollision()
    {
      foreach (int actorID in ActorFactory.GetHoldingList())
      {
        ActorInfo a = ActorFactory.Get(actorID);
        if (a != null)
          a.ProcessCollision();
      }
    }

    public void PreRender()
    {
      TrueVision.TVEngine.Clear();
      TrueVision.TVScene.FinalizeShadows();

      TrueVision.TVScreen2DText.Action_BeginText();
      string text = "";
      int i = 0;
      while (Screen2D.LoadingTextLines.Count > 20)
      {
        Screen2D.LoadingTextLines.RemoveAt(0);
      }

      while (i < Screen2D.LoadingTextLines.Count)
      {
        text += Screen2D.LoadingTextLines[i] + "\n";
        i++;
      }

      TrueVision.TVScreen2DText.TextureFont_DrawText(text, 40, 40, new TV_COLOR(1,1,1,1).GetIntColor(), Font.Factory.Get("Text_12").ID);
      TrueVision.TVScreen2DText.Action_EndText();

      Screen2D.Draw();
      TrueVision.TVEngine.RenderToScreen();
    }

    public void Render()
    {
      TrueVision.TVEngine.Clear();

      AtmosphereInfo.Render();
      TrueVision.TVScene.FinalizeShadows();
      LandInfo.Render();

      foreach (int actorID in ActorFactory.GetHoldingList())
      {
        ActorInfo a = ActorFactory.Get(actorID);
        if (a != null)
          a.Render();
      }

      using (new PerfElement("render_2D_draw"))
        Screen2D.Draw();

      using (new PerfElement("render_drawtoscreen"))
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

    public void Exit()
    {
      Game.Stop();
      Form.Exit();
      Dispose();
    }
  }
}
