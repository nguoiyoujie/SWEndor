using System;
using MTV3D65;
using System.IO;
using SWEndor.Actors;
using SWEndor.Scenarios;
using SWEndor.Input;
using SWEndor.Sound;
using SWEndor.UI;
using SWEndor.Weapons;
using SWEndor.ActorTypes;
using SWEndor.Player;

namespace SWEndor
{
  public class Engine
  {
    internal Engine() { }

    private IntPtr Handle;
    public float ScreenWidth { get; private set;}
    public float ScreenHeight { get; private set; }
    public Random Random { get; private set; } = new Random();

    // Engine parts
    internal Game Game { get; private set; }
    internal SoundManager SoundManager { get; private set; }
    internal PerfManager PerfManager { get; private set; }
    internal PlayerInfo PlayerInfo { get; private set; }

    // TrueVision parts
    internal TVEngine TVEngine { get; private set; }
    internal TVScene TVScene { get; private set; }
    internal TVMathLibrary TVMathLibrary { get; private set; }
    internal TVCameraFactory TVCameraFactory { get; private set; }
    internal TVTextureFactory TVTextureFactory { get; private set; }
    internal TVLightEngine TVLightEngine { get; private set; }
    internal TVMaterialFactory TVMaterialFactory { get; private set; }
    internal TVGraphicEffect TVGraphicEffect { get; private set; }
    internal TVShader TVShader { get; private set; }
    internal TVScreen2DText TVScreen2DText { get; private set; }
    internal TVScreen2DImmediate TVScreen2DImmediate { get; private set; }
    internal TVPhysics TVPhysics { get; private set; }
    internal TVParticleSystem TVParticleSystem { get; private set; }
    internal TVGlobals TVGlobals { get; private set; }

    // Engine parts dependent on TrueVision
    internal InputManager InputManager { get; private set; }
    internal AtmosphereInfo AtmosphereInfo { get; private set; }
    internal LandInfo LandInfo { get; private set; }
    internal Screen2D Screen2D { get; private set; }

    // Engine pars to be loaded late
    // After ActorInfoType initialization
    internal GameScenarioManager GameScenarioManager { get; private set; }


    public void Init()
    {
      Game = new Game();
      SoundManager = new SoundManager();
      PerfManager = new PerfManager();
      PlayerInfo = new PlayerInfo();
    }

    public void InitTrueVision()
    {
      if (Handle == null)
        throw new Exception("Engine is not attached to a window form!");
      TVEngine = new TVEngine();
      TVGlobals = new TVGlobals();
      TVMathLibrary = new TVMathLibrary();
      TVScene = new TVScene();
      TVPhysics = new TVPhysics();
      TVScreen2DImmediate = new TVScreen2DImmediate();
      TVScreen2DText = new TVScreen2DText();
      TVShader = new TVShader();
      TVCameraFactory = new TVCameraFactory();
      TVLightEngine = new TVLightEngine();
      TVTextureFactory = new TVTextureFactory();
      TVMaterialFactory = new TVMaterialFactory();
      TVParticleSystem = new TVParticleSystem();
      TVGraphicEffect = new TVGraphicEffect();

      InitEngine();
      InitWindow();
      InitFonts();
      InitScene();
      InitShaders();
      InitPhysics();
      InitLights();
      InitSounds();

      InputManager = new InputManager();
      AtmosphereInfo = new AtmosphereInfo();
      LandInfo = new LandInfo();
      Screen2D = new Screen2D();
    }

    public void Dispose()
    {
      TVParticleSystem = null;
      TVGraphicEffect = null;
      TVMaterialFactory = null;
      TVTextureFactory = null;
      TVLightEngine = null;
      TVCameraFactory = null;
      TVScreen2DText = null;
      TVScreen2DImmediate = null;
      TVShader = null;
      TVPhysics = null;
      TVScene = null;
      TVMathLibrary = null;
      TVGlobals = null;
      SoundManager.Dispose();
      InputManager.Dispose();
      TVEngine.ReleaseAll();
      TVEngine = null;

      GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
    }

    public void Load()
    {
      Screen2D.LoadingTextLines.Add(Globals.LoadingFlavourTexts[Random.Next(0, Globals.LoadingFlavourTexts.Count)]);
      ActorTypeInfo.Factory.Initialise();

      Screen2D.LoadingTextLines.Add("Loading other actor definitions...");
      ActorTypeInfo.Factory.LoadFromINI(Globals.ActorTypeINIPath);

      Screen2D.LoadingTextLines.Add("Loading weapons...");
      WeaponFactory.LoadFromINI(Globals.WeaponStatINIPath);

      Screen2D.LoadingTextLines.Add("Loading dynamic music info...");
      SoundManager.Piece.Factory.LoadFromINI(Globals.DynamicMusicINIPath);

      AtmosphereInfo.LoadDefaults(true, true);
      LandInfo.LoadDefaults();

      Screen2D.LoadingTextLines.Add("Loading scenario engine...");
      GameScenarioManager = new GameScenarioManager();
      GameScenarioManager.LoadInitial();

      Screen2D.LoadingTextLines.Add("Loading main menu...");
      GameScenarioManager.LoadMainMenu();
    }

    public void Process()
    {
      foreach (int actorID in ActorInfo.Factory.GetList())
      {
        ActorInfo a = ActorInfo.Factory.Get(actorID);
        if (a != null)
          a.Process();

        //if (a.IsPlayer())
        //  ProcessPlayerCamera();
      }
    }

    public void ProcessAI()
    {
      foreach (int actorID in ActorInfo.Factory.GetHoldingList())
      {
        ActorInfo a = ActorInfo.Factory.Get(actorID);
        if (a != null)
          a.ProcessAI();
      }
    }

    public void ProcessCollision()
    {
      foreach (int actorID in ActorInfo.Factory.GetHoldingList())
      {
        ActorInfo a = ActorInfo.Factory.Get(actorID);
        if (a != null)
          a.ProcessCollision();
      }
    }

    public void PreRender()
    {
      TVEngine.Clear();
      TVScene.FinalizeShadows();

      TVScreen2DText.Action_BeginText();
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

      TVScreen2DText.TextureFont_DrawText(text, 40, 40, new TV_COLOR(1,1,1,1).GetIntColor(), Font.Factory.Get("Text_12").ID);
      TVScreen2DText.Action_EndText();

      Screen2D.Draw();
      TVEngine.RenderToScreen();
    }

    public void Render()
    {
      TVEngine.Clear();

      AtmosphereInfo.Render();
      TVScene.FinalizeShadows();
      LandInfo.Render();

      foreach (int actorID in ActorInfo.Factory.GetHoldingList())
      {
        ActorInfo a = ActorInfo.Factory.Get(actorID);
        if (a != null)
          a.Render();
      }

      using (new PerfElement("render_2D_draw"))
        Screen2D.Draw();

      using (new PerfElement("render_drawtoscreen"))
        TVEngine.RenderToScreen();
    }

    public void LinkHandle(IntPtr handle)
    {
      Handle = handle;
    }

    private void InitEngine()
    {
      TVEngine.AllowMultithreading(true);
      //TVEngine.SetDebugMode(true, true);
      TVEngine.SetDebugFile(Path.Combine(Globals.DebugPath, @"truevision_debug.txt"));

      TVEngine.DisplayFPS(true);
      //TVEngine.EnableProfiler(true);
      TVEngine.EnableSmoothTime(true);
      TVEngine.SetVSync(true);

      TVEngine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_DEGREE);
    }

    private void InitWindow()
    {
      if (Settings.FullScreenMode)
        TVEngine.Init3DFullscreen(Settings.ResolutionX, Settings.ResolutionY, 32, true, true, CONST_TV_DEPTHBUFFERFORMAT.TV_DEPTHBUFFER_BESTBUFFER, 1.0f, Handle);
      else
        TVEngine.Init3DWindowed(Handle, true);

      //TVEngine.SwitchWindowed();

      TVViewport port = TVEngine.GetViewport();
      ScreenWidth = port.GetWidth();
      ScreenHeight = port.GetHeight();
      //TVEngine.GetViewport().SetAutoResize(true);
    }

    private void InitFonts()
    {
      Font.Factory.Create("Text_08", "Consolas", 8, false, false, false, true);
      Font.Factory.Create("Text_10", "Consolas", 10, true, false, false, true);
      Font.Factory.Create("Text_12", "Consolas", 12, true, false, false, true);
      Font.Factory.Create("Text_14", "Consolas", 14, true, false, false, true);
      Font.Factory.Create("Text_16", "Consolas", 16, true, false, false, true);
      Font.Factory.Create("Text_24", "Consolas", 24, true, false, false, true);
      Font.Factory.Create("Title_36", "Impact", 36, false, false, false, true);
      Font.Factory.Create("Title_48", "Impact", 48, false, false, false, true);
    }

    private void InitScene()
    {
      TVScene.SetShadeMode(CONST_TV_SHADEMODE.TV_SHADEMODE_GOURAUD);
      TVScene.SetRenderMode(CONST_TV_RENDERMODE.TV_SOLID);
      TVScene.SetBackgroundColor(0f, 0f, 0f);
    }

    private void InitShaders()
    {
      //TVShader = TVScene.CreateShader("common");
      //TVShader.CreateFromEffectFile(Path.Combine(Globals.ShaderPath, "Common_shader"));
    }

    private void InitPhysics()
    {
      //TVPhysics.Initialize();
      //TVPhysics.SetSolverModel(CONST_TVPhysics_SOLVER.TV_SOLVER_ADAPTIVE);
      //TVPhysics.SetFrictionModel(CONST_TVPhysics_FRICTION.TV_FRICTION_ADAPTIVE);
      //TVPhysics.SetGlobalGravity(new TV_3DVECTOR(0, 0, 0));
      //TVPhysics.SetGlobalGravity(new TV_3DVECTOR(0, -9.800908285f, 0));
      //TVPhysics.EnableCPUOptimizations(true); 
    }

    private void InitLights()
    {
      TVLightEngine.SetGlobalAmbient(0.75f, 0.75f, 0.75f);

      int tv_light0 = TVLightEngine.CreateDirectionalLight(new TV_3DVECTOR(-1.0f, -0.5f, 0.2f), 1f, 1f, 1f, "GlobalLight0", 0.9f);
      int tv_light1 = TVLightEngine.CreateDirectionalLight(new TV_3DVECTOR(1.0f, 0.2f, 0f), 0.5f, 0.5f, 0.5f, "GlobalLight1", 0.9f);
      int tv_light2 = TVLightEngine.CreateDirectionalLight(new TV_3DVECTOR(-1.0f, 2.0f, -0.4f), 0.3f, 0.25f, 0.1f, "GlobalLight2", 0.9f);
      
      TVLightEngine.EnableLight(tv_light0, true);
      TVLightEngine.EnableLight(tv_light1, true);
      TVLightEngine.EnableLight(tv_light2, true);
      
      TVLightEngine.SetSpecularLighting(true);
    }

    private void InitSounds()
    {
      SoundManager.Initialize();
      SoundManager.Load();


      //2142 + 8572x
      for (uint i = 2142; i < 68571; i += 8572)
        SoundManager.AddMusicSyncPoint("trofix", "try", i);
      SoundManager.AddMusicSyncPoint("trofix", "try2", 68571);

      for (uint i = 2142; i < 123000; i += 4922)
        SoundManager.AddMusicSyncPoint("rebfix", "try", i);

      for (uint i = 2142; i < 123000; i += 4922)
        SoundManager.AddMusicSyncPoint("waitfix", "try", i);
      SoundManager.AddMusicSyncPoint("waitfix", "try2", 41922);
      SoundManager.AddMusicSyncPoint("waitfix", "try2", 44895);

      for (uint i = 2142; i < 123000; i += 4922)
        SoundManager.AddMusicSyncPoint("confix", "try", i);

      for (uint i = 2142; i < 123000; i += 4922)
        SoundManager.AddMusicSyncPoint("polfix", "try", i);

      SoundManager.AddMusicSyncPoint("dynamic\\S-EMP-SM", "", 5240);
      SoundManager.AddMusicSyncPoint("dynamic\\S-EMP-LG", "", 9030);
      //SoundManager.AddMusicSyncPoint("dynamic\\imperialfighterarrv", "", 4300);
      //SoundManager.AddMusicSyncPoint("dynamic\\imperialfighterlost", "", 5192);
      //SoundManager.AddMusicSyncPoint("dynamic\\rebelcapshiparrv", "", 7600);
      //SoundManager.AddMusicSyncPoint("dynamic\\rebelfighterarrv", "", 5800);



    }

    public void Exit()
    {
      Game.Close();
    }
  }
}
