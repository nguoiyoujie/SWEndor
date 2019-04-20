using System;
using System.Collections.Generic;
using MTV3D65;
using System.IO;
using SWEndor.Actors;
using SWEndor.Scenarios;
using SWEndor.Input;
using SWEndor.Sound;
using SWEndor.UI;
using SWEndor.Weapons;
using SWEndor.ActorTypes;

namespace SWEndor
{
  public class Engine
  {
    private static Engine _instance;
    public static Engine Instance()
    {
      if (_instance == null) { _instance = new Engine(); }
      return _instance;
    }

    private Engine()
    {
      Random = new Random();
    }

    private IntPtr Handle;
    public float ScreenWidth { get; private set;}
    public float ScreenHeight { get; private set; }
    public Random Random { get; private set; }

    private TVEngine tv_engine;
    private TVScene tv_scene;
    private TVMathLibrary tv_math;
    private TVCameraFactory tv_cameras;
    private TVTextureFactory tv_textures;
    private TVLightEngine tv_lights;
    private TVMaterialFactory tv_materials;
    private TVGraphicEffect tv_geffect;
    private TVShader tv_shader;
    private TVScreen2DText tv_scr2Dtext;
    private TVScreen2DImmediate tv_scr2Dimm;
    private TVPhysics tv_physics;
    private TVParticleSystem tv_particlesys;
    private TVGlobals tv_globals;

    public TVEngine TVEngine { get { return tv_engine; } }
    public TVScene TVScene { get { return tv_scene; } }
    public TVMathLibrary TVMathLibrary { get { return tv_math; } }
    public TVCameraFactory TVCameraFactory {  get { return tv_cameras; } }
    public TVTextureFactory TVTextureFactory { get { return tv_textures; } }
    public TVLightEngine TVLightEngine { get { return tv_lights; } }
    public TVMaterialFactory TVMaterialFactory { get { return tv_materials; } }
    public TVGraphicEffect TVGraphicEffect { get { return tv_geffect; } }
    public TVShader TVShader { get { return tv_shader; } }
    public TVScreen2DText TVScreen2DText { get { return tv_scr2Dtext; } }
    public TVScreen2DImmediate TVScreen2DImmediate { get { return tv_scr2Dimm; } }
    public TVPhysics TVPhysics { get { return tv_physics; } }
    public TVParticleSystem TVParticleSystem { get { return tv_particlesys; } }
    public TVGlobals TVGlobals { get { return tv_globals; } }

    public void Initialize()
    {
      if (Handle == null)
        throw new Exception("Engine is not attached to a window form!");
      tv_engine = new TVEngine();
      tv_globals = new TVGlobals();
      tv_math = new TVMathLibrary();
      tv_scene = new TVScene();
      tv_physics = new TVPhysics();
      tv_scr2Dimm = new TVScreen2DImmediate();
      tv_scr2Dtext = new TVScreen2DText();
      tv_shader = new TVShader();
      tv_cameras = new TVCameraFactory();
      tv_lights = new TVLightEngine();
      tv_textures = new TVTextureFactory();
      tv_materials = new TVMaterialFactory();
      tv_particlesys = new TVParticleSystem();
      tv_geffect = new TVGraphicEffect();
    }

    public void Dispose()
    {
      tv_particlesys = null;
      tv_geffect = null;
      tv_materials = null;
      tv_textures = null;
      tv_lights = null;
      tv_cameras = null;
      tv_scr2Dtext = null;
      tv_scr2Dimm = null;
      tv_shader = null;
      tv_physics = null;
      tv_scene = null;
      tv_math = null;
      tv_globals = null;
      SoundManager.Instance().Dispose();
      InputManager.Instance().Dispose();
      tv_engine.ReleaseAll();
      tv_engine = null;

      GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
    }

    public void InitializeComponents()
    {
      InitEngine();
      InitWindow();
      InitFonts();
      InitScene();
      InitShaders();
      InitPhysics();
      InitLights();
      InitSounds();
    }

    public void Load()
    {
      Screen2D.Instance().LoadingTextLines.Add(Globals.LoadingFlavourTexts[Random.Next(0, Globals.LoadingFlavourTexts.Count)]);
      ActorTypeInfo.Factory.Initialise();
      Screen2D.Instance().LoadingTextLines.Add("Loading other actortypes...");
      ActorTypeInfo.Factory.LoadFromINI(Globals.ActorTypeINIPath);
      Screen2D.Instance().LoadingTextLines.Add("Loading weapons...");
      WeaponFactory.LoadFromINI(Globals.WeaponStatINIPath);
      Screen2D.Instance().LoadingTextLines.Add("Loading scenario engine...");
      GameScenarioManager.Instance().LoadInitial();
      Screen2D.Instance().LoadingTextLines.Add("Loading main menu...");
      GameScenarioManager.Instance().LoadMainMenu();
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
      tv_engine.Clear();
      tv_scene.FinalizeShadows();

      TVScreen2DText.Action_BeginText();
      string text = "";
      int i = 0;
      while (Screen2D.Instance().LoadingTextLines.Count > 20)
      {
        Screen2D.Instance().LoadingTextLines.RemoveAt(0);
      }

      while (i < Screen2D.Instance().LoadingTextLines.Count)
      {
        text += Screen2D.Instance().LoadingTextLines[i] + "\n";
        i++;
      }

      TVScreen2DText.TextureFont_DrawText(text, 40, 40, new TV_COLOR(1,1,1,1).GetIntColor(), Font.Factory.Get("Text_12").ID);
      TVScreen2DText.Action_EndText();

      Screen2D.Instance().Draw();
      tv_engine.RenderToScreen();
    }

    public void Render()
    {
      tv_engine.Clear();

      AtmosphereInfo.Instance().Render();
      tv_scene.FinalizeShadows();
      LandInfo.Instance().Render();

      foreach (int actorID in ActorInfo.Factory.GetHoldingList())
      {
        ActorInfo a = ActorInfo.Factory.Get(actorID);
        if (a != null)
          a.Render();
      }

      using (new PerfElement("render_2D_draw"))
        Screen2D.Instance().Draw();

      using (new PerfElement("render_drawtoscreen"))
        tv_engine.RenderToScreen();
    }

    public void LinkHandle(IntPtr handle)
    {
      Handle = handle;
    }

    private void InitEngine()
    {
      tv_engine.AllowMultithreading(true);
      //tv_engine.SetDebugMode(true, true);
      tv_engine.SetDebugFile(Path.Combine(Globals.DebugPath, @"truevision_debug.txt"));

      tv_engine.DisplayFPS(true);
      //tv_engine.EnableProfiler(true);
      tv_engine.EnableSmoothTime(true);
      tv_engine.SetVSync(true);

      tv_engine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_DEGREE);
    }

    private void InitWindow()
    {
      if (Settings.FullScreenMode)
        tv_engine.Init3DFullscreen(Settings.ResolutionX, Settings.ResolutionY, 32, true, true, CONST_TV_DEPTHBUFFERFORMAT.TV_DEPTHBUFFER_BESTBUFFER, 1.0f, Handle);
      else
        tv_engine.Init3DWindowed(Handle, true);

      //tv_engine.SwitchWindowed();

      TVViewport port = tv_engine.GetViewport();
      ScreenWidth = port.GetWidth();
      ScreenHeight = port.GetHeight();
      //tv_engine.GetViewport().SetAutoResize(true);
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
      tv_scene.SetShadeMode(CONST_TV_SHADEMODE.TV_SHADEMODE_GOURAUD);
      tv_scene.SetRenderMode(CONST_TV_RENDERMODE.TV_SOLID);
      tv_scene.SetBackgroundColor(0f, 0f, 0f);

      AtmosphereInfo.Instance().LoadDefaults(true, true);
      LandInfo.Instance().LoadDefaults();
    }

    private void InitShaders()
    {
      //tv_shader = tv_scene.CreateShader("common");
      //tv_shader.CreateFromEffectFile(Path.Combine(Globals.ShaderPath, "Common_shader"));
    }

    private void InitPhysics()
    {
      //tv_physics.Initialize();
      //tv_physics.SetSolverModel(CONST_TV_PHYSICS_SOLVER.TV_SOLVER_ADAPTIVE);
      //tv_physics.SetFrictionModel(CONST_TV_PHYSICS_FRICTION.TV_FRICTION_ADAPTIVE);
      //tv_physics.SetGlobalGravity(new TV_3DVECTOR(0, 0, 0));
      //tv_physics.SetGlobalGravity(new TV_3DVECTOR(0, -9.800908285f, 0));
      //tv_physics.EnableCPUOptimizations(true); 
    }

    private void InitLights()
    {
      tv_lights.SetGlobalAmbient(0.75f, 0.75f, 0.75f);

      int tv_light0 = tv_lights.CreateDirectionalLight(new TV_3DVECTOR(-1.0f, -0.5f, 0.2f), 1f, 1f, 1f, "GlobalLight0", 0.9f);
      int tv_light1 = tv_lights.CreateDirectionalLight(new TV_3DVECTOR(1.0f, 0.2f, 0f), 0.5f, 0.5f, 0.5f, "GlobalLight1", 0.9f);
      int tv_light2 = tv_lights.CreateDirectionalLight(new TV_3DVECTOR(-1.0f, 2.0f, -0.4f), 0.3f, 0.25f, 0.1f, "GlobalLight2", 0.9f);
      
      tv_lights.EnableLight(tv_light0, true);
      tv_lights.EnableLight(tv_light1, true);
      tv_lights.EnableLight(tv_light2, true);
      
      tv_lights.SetSpecularLighting(true);
    }

    private void InitSounds()
    {
      SoundManager.Instance().Initialize();
      SoundManager.Instance().Load();

      SoundManager.Instance().AddMusicSyncPoint("trofix", "try", 10714);
      SoundManager.Instance().AddMusicSyncPoint("trofix", "try2", 68571);
    }

    public void Exit()
    {
      Game.Instance().Close();
    }
  }
}
