using System;
using MTV3D65;
using System.IO;
using Primrose.Primitives;

namespace SWEndor.Core
{
  public class TrueVision
  {
    public readonly Engine Engine;
    internal TrueVision(Engine engine) { Engine = engine; }

    private IntPtr Handle;

    // TrueVision parts
    internal TVEngine TVEngine { get; private set; }
    internal TVScene TVScene { get; private set; }
    internal TVMathLibrary TVMathLibrary { get; private set; }
    internal TVCameraFactory TVCameraFactory { get; private set; }
    internal TVTextureFactory TVTextureFactory { get; private set; }
    internal TVLightEngine TVLightEngine { get; private set; }
    internal TVMaterialFactory TVMaterialFactory { get; private set; }
    internal TVGraphicEffect TVGraphicEffect { get; private set; }
    internal TVScreen2DText TVScreen2DText { get; private set; }
    internal TVScreen2DImmediate TVScreen2DImmediate { get; private set; }
    internal TVParticleSystem TVParticleSystem { get; private set; }
    internal TVGlobals TVGlobals { get; private set; }

    public void Init(IntPtr handle)
    {
      if (Handle == null)
        throw new Exception("Engine is not attached to a window form!");
      Handle = handle;

      TVEngine = new TVEngine();
      TVGlobals = new TVGlobals();
      TVMathLibrary = new TVMathLibrary();
      TVScene = new TVScene();
      TVScreen2DImmediate = new TVScreen2DImmediate();
      TVScreen2DText = new TVScreen2DText();
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
      InitLights();
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
      TVScene = null;
      TVMathLibrary = null;
      TVGlobals = null;
      TVEngine.ReleaseAll();
      TVEngine = null;

      GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
    }

    private void InitEngine()
    {
      TVEngine.AllowMultithreading(true);
      //TVEngine.SetDebugMode(true, true);
      TVEngine.SetDebugFile(Path.Combine(Globals.LogPath, @"truevision_debug.log"));

      TVEngine.SetAntialiasing(true, CONST_TV_MULTISAMPLE_TYPE.TV_MULTISAMPLE_16_SAMPLES);
      TVEngine.DisplayFPS(true);
      //TVEngine.EnableProfiler(true);
      TVEngine.EnableSmoothTime(true);
      TVEngine.SetVSync(true);

      TVEngine.SetAngleSystem(CONST_TV_ANGLE.TV_ANGLE_DEGREE);
    }

    private void InitWindow()
    {
      if (Engine.Settings.FullScreenMode)
        TVEngine.Init3DFullscreen(Engine.Settings.ResolutionX, Engine.Settings.ResolutionY, 32, true, true, CONST_TV_DEPTHBUFFERFORMAT.TV_DEPTHBUFFER_BESTBUFFER, 1.0f, Handle);
      else
        TVEngine.Init3DWindowed(Handle, true);

      TVViewport port = TVEngine.GetViewport();
      Engine.ScreenWidth = port.GetWidth();
      Engine.ScreenHeight = port.GetHeight();
    }

    private void InitFonts()
    {
      Engine.FontFactory.Init(Engine);
    }

    private void InitScene()
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        TVScene.SetShadeMode(CONST_TV_SHADEMODE.TV_SHADEMODE_GOURAUD);
        TVScene.SetRenderMode(CONST_TV_RENDERMODE.TV_SOLID);
        TVScene.SetTextureFilter(CONST_TV_TEXTUREFILTER.TV_FILTER_ANISOTROPIC);
        TVScene.SetBackgroundColor(0f, 0f, 0f);
      }

    }

    private void InitShaders()
    {
    }

    private void InitLights()
    {
      // TO-DO: Shift this to 'Globals', then make this configuable
      TVLightEngine.SetGlobalAmbient(0.75f, 0.75f, 0.75f);

      int tv_light0 = TVLightEngine.CreateDirectionalLight(new TV_3DVECTOR(-1.0f, -0.5f, 0.2f), 1f, 1f, 1f, "GlobalLight0", 0.9f);
      int tv_light1 = TVLightEngine.CreateDirectionalLight(new TV_3DVECTOR(1.0f, 0.2f, 0f), 0.5f, 0.5f, 0.5f, "GlobalLight1", 0.9f);
      int tv_light2 = TVLightEngine.CreateDirectionalLight(new TV_3DVECTOR(-1.0f, 2.0f, -0.4f), 0.3f, 0.25f, 0.1f, "GlobalLight2", 0.9f);
      
      TVLightEngine.EnableLight(tv_light0, true);
      TVLightEngine.EnableLight(tv_light1, true);
      TVLightEngine.EnableLight(tv_light2, true);
      
      TVLightEngine.SetSpecularLighting(true);
    }
  }
}
