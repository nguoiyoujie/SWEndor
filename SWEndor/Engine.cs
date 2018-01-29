using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MTV3D65;
using System.IO;

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

    private Form fm_parent;
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
      if (fm_parent == null)
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
      InitFonts();
      InitScene();
      InitShaders();
      InitPhysics();
      InitLights();
      InitSounds();
    }

    public void Load()
    {
      Screen2D.Instance().LoadingTextLines.Add(Globals.LoadingFlavourTexts[Engine.Instance().Random.Next(0, Globals.LoadingFlavourTexts.Count)]);
      ActorTypeFactory.Instance().Initialise();
      Screen2D.Instance().LoadingTextLines.Add("Loading scenario engine...");
      GameScenarioManager.Instance().LoadInitial();
      Screen2D.Instance().LoadingTextLines.Add("Loading main menu...");
      GameScenarioManager.Instance().LoadMainMenu();
    }

    public void Process()
    {
      if (!Game.Instance().IsPaused)
      {
        Queue<ActorInfo> q = new Queue<ActorInfo>(ActorFactory.Instance().GetActorList());
        while (q.Count > 0)
        {
          q.Dequeue().Process();
        }
      }
    }

    public void ProcessAI()
    {
      if (!Game.Instance().IsPaused)
      {
        Queue<ActorInfo> q = new Queue<ActorInfo>(ActorFactory.Instance().GetActorList());
        while (q.Count > 0)
        {
          q.Dequeue().ProcessAI();
        }
      }
    }

    public void ProcessCollision()
    {
      if (!Game.Instance().IsPaused)
      {
        Queue<ActorInfo> q = new Queue<ActorInfo>(ActorFactory.Instance().GetActorList());
        while (q.Count > 0)
        {
          q.Dequeue().ProcessCollision();
        }
      }
    }

    public void ProcessPlayer()
    {
      if (!Game.Instance().IsPaused)
      {
        //process player last
        if (PlayerInfo.Instance().Actor != null)
        {
          //PlayerInfo.Instance().Actor.Process();
          //process player camera
          PlayerInfo.Instance().Actor.TypeInfo.ChaseCamera(PlayerInfo.Instance().Actor);
        }

        PlayerInfo.Instance().Update();
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

      TVScreen2DText.TextureFont_DrawText(text, 40, 40, new TV_COLOR(1,1,1,1).GetIntColor(), Screen2D.Instance().FontID12);
      TVScreen2DText.Action_EndText();

      Screen2D.Instance().Draw();
      tv_engine.RenderToScreen();
    }

    public void Render()
    {
      using (new PerfElement("render_clear"))
      {
        tv_engine.Clear();
        tv_scene.FinalizeShadows();
      }

      ActorInfo[] ainfo = ActorFactory.Instance().GetActorList();
      foreach (ActorInfo a in ainfo)
      {
        if (a != null)
          a.Render();
      }
      using (new PerfElement("render_2D_draw"))
      {
        Screen2D.Instance().Draw();
      }

      using (new PerfElement("render_drawtoscreen"))
      {
        tv_engine.RenderToScreen();
      }
    }

    public void LinkForm(Form f)
    {
      fm_parent = f;
    }

    private void InitEngine()
    {
      tv_engine.AllowMultithreading(true);
      //tv_engine.SetDebugMode(true, true);
      tv_engine.SetDebugFile(Path.Combine(Globals.DebugPath, @"truevision_debug.txt"));
      //tv_engine.Init3DFullscreen(800, 600);
      if (Settings.FullScreenMode)
      {
        tv_engine.Init3DFullscreen(Settings.ResolutionX, Settings.ResolutionY, 32, true, true, CONST_TV_DEPTHBUFFERFORMAT.TV_DEPTHBUFFER_BESTBUFFER, 1.0f, fm_parent.Handle);
      }
      else
      {
        tv_engine.Init3DWindowed(fm_parent.Handle, true);
      }

      tv_engine.DisplayFPS(true);
      //tv_engine.EnableProfiler(true);
      tv_engine.EnableSmoothTime(true);
      ScreenWidth = tv_engine.GetViewport().GetWidth();
      ScreenHeight = tv_engine.GetViewport().GetHeight();
      tv_engine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_DEGREE);
      tv_engine.GetViewport().SetAutoResize(true);

    }

    private void InitFonts()
    {
      Screen2D.Instance().FontID08 = TVScreen2DText.TextureFont_Create("Consolas08", "Consolas", 8, false, false, false, true);
      Screen2D.Instance().FontID10 = TVScreen2DText.TextureFont_Create("Consolas12", "Consolas", 10, true, false, false, true);
      Screen2D.Instance().FontID12 = TVScreen2DText.TextureFont_Create("Consolas12", "Consolas", 12, true, false, false, true);
      Screen2D.Instance().FontID14 = TVScreen2DText.TextureFont_Create("Consolas14", "Consolas", 14, true, false, false, true);
      Screen2D.Instance().FontID16 = TVScreen2DText.TextureFont_Create("Consolas16", "Consolas", 16, true, false, false, true);
      Screen2D.Instance().FontID24 = TVScreen2DText.TextureFont_Create("Consolas24", "Consolas", 24, true, false, false, true);
      Screen2D.Instance().FontID36 = TVScreen2DText.TextureFont_Create("Impact36", "Impact", 36, false, false, false, true);
      Screen2D.Instance().FontID48 = TVScreen2DText.TextureFont_Create("Impact48", "Impact", 48, false, false, false, true);
    }

    private void InitScene()
    {
      tv_scene.SetRenderMode(CONST_TV_RENDERMODE.TV_SOLID);
      tv_scene.SetBackgroundColor(0f, 0f, 0f);
    }

    private void InitShaders()
    {
      //tv_shader = tv_scene.CreateShader("common");
      //tv_shader.CreateFromEffectFile(Path.Combine(Globals.ShaderPath, "Common_shader"));
    }

    private void InitPhysics()
    {
      /*
      tv_physics.Initialize();
      tv_physics.SetSolverModel(CONST_TV_PHYSICS_SOLVER.TV_SOLVER_ADAPTIVE);
      tv_physics.SetFrictionModel(CONST_TV_PHYSICS_FRICTION.TV_FRICTION_ADAPTIVE);
      tv_physics.SetGlobalGravity(new TV_3DVECTOR(0, 0, 0));
      //tv_physics.SetGlobalGravity(new TV_3DVECTOR(0, -9.800908285f, 0));
      tv_physics.EnableCPUOptimizations(true);
      */
    }

    private void InitLights()
    {
      tv_lights.SetGlobalAmbient(0.75f, 0.75f, 0.75f);

      int tv_light0 = tv_lights.CreateDirectionalLight(new MTV3D65.TV_3DVECTOR(-1.0f, -0.5f, 0.2f), 1f, 1f, 1f, "GlobalLight0", 0.9f);
      int tv_light1 = tv_lights.CreateDirectionalLight(new MTV3D65.TV_3DVECTOR(1.0f, 0.2f, 0f), 0.5f, 0.5f, 0.5f, "GlobalLight1", 0.9f);
      int tv_light2 = tv_lights.CreateDirectionalLight(new MTV3D65.TV_3DVECTOR(-1.0f, 2.0f, -0.4f), 0.3f, 0.25f, 0.1f, "GlobalLight2", 0.9f);
      tv_lights.EnableLight(tv_light0, true);
      tv_lights.EnableLight(tv_light1, true);
      tv_lights.EnableLight(tv_light2, true);
      tv_lights.SetSpecularLighting(true);
    }

    private void InitSounds()
    {
      SoundManager.Instance().Initialize((int)fm_parent.Handle);
      SoundManager.Instance().Load();
    }

    public void Exit()
    {
      Game.Instance().Close();
    }
  }
}
