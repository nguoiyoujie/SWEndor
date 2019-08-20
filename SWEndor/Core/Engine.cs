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
using SWEndor.UI.Forms;
using SWEndor.Actors.Data;
using System.Text;

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

    // Data
    //internal MemoryManager Memory { get; private set; }

    // Data sets
    internal MaskDataSet MaskDataSet { get; private set; }

    internal ActorDataSet ActorDataSet { get; private set; }
    //internal SysDataSet SysDataSet { get; private set; }
    internal MeshDataSet MeshDataSet { get; private set; }
    //internal TimedLifeDataSet TimedLifeDataSet { get; private set; }

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
    internal ActionManager ActionManager { get; private set; }

    // Engine pars to be loaded late
    // Requires ActorInfoType initialization
    internal GameScenarioManager GameScenarioManager { get; private set; }
    internal Scenarios.Scripting.Expressions.Context ScriptContext { get; private set; }

    public void Init()
    {
      //Memory = new MemoryManager(this);

      MaskDataSet = new MaskDataSet();
      ActorDataSet = new ActorDataSet();
      //SysDataSet = new SysDataSet(this);
      MeshDataSet = new MeshDataSet();
      //TimedLifeDataSet = new TimedLifeDataSet();

      Game = new Game(this);
      SoundManager = new SoundManager(this);
      PerfManager = new PerfManager(this);
      ActorFactory = new ActorInfo.Factory(this);
      ActorTypeFactory = new ActorTypeInfo.Factory(this);
      ActionManager = new ActionManager(this);
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
      ActorFactory.DoEach(ActorInfo.Process);
    }

    public void ProcessAI()
    {
      ActorFactory.DoEach(ActorInfo.ProcessAI);
    }

    public void ProcessCollision()
    {
      ActorFactory.DoEach(ActorInfo.ProcessCollision);
    }

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
      LandInfo.Render();

      ActorFactory.DoEach(ActorInfo.Render);
      //TrueVision.TVGraphicEffect.DrawGlow();
      //TrueVision.TVGraphicEffect.DrawDepthOfField();

      Screen2D.Draw();

      /*
      ActorInfo tgt = ActorFactory.Get(PlayerCameraInfo.LookActor);
      if (tgt != null && tgt.Active)
      {
        TV_3DVECTOR prostart = tgt.GetRelativePositionXYZ(0, 0, tgt.TypeInfo.max_dimensions.z + 10);
        TV_3DVECTOR proend0 = tgt.GetRelativePositionXYZ((float)Math.Sin(tgt.MoveData.YTurnAngle * Globals.PI / 180) * tgt.MoveData.Speed
                                                         , -(float)Math.Sin(tgt.MoveData.XTurnAngle * Globals.PI / 180) * tgt.MoveData.Speed
                                                         , tgt.TypeInfo.max_dimensions.z + 10 + ActorDataSet.CollisionData[tgt.dataID].ProspectiveCollisionScanDistance);
        //TrueVision.TVMathLibrary.MoveAroundPoint(tgt.GetPosition(), 1000, tgt.)

        TrueVision.TVScreen2DImmediate.Action_Begin2D();
        //Engine.TrueVision.TVScreen2DImmediate.Draw_Box3D(prostart, proend0, new TV_COLOR(1, 1, 1, 1).GetIntColor());
        TrueVision.TVScreen2DImmediate.Draw_Line3D(prostart.x, prostart.y, prostart.z, proend0.x, proend0.y, proend0.z, new TV_COLOR(1, 0, 0, 1).GetIntColor(), new TV_COLOR(1, 1, 0, 1).GetIntColor());
        //TrueVision.TVScreen2DImmediate.Draw_FilledBox(100, 100, 200, 200, new TV_COLOR(1, 1, 1, 1).GetIntColor());
        TrueVision.TVScreen2DImmediate.Action_End2D();

      }
      */

      TrueVision.TVEngine.RenderToScreen();
      //UpdateEffect();
    }

    /*
    public void UpdateEffect()
    {
      TrueVision.LaserRenderSurface.StartRender(false);
      ActorFactory.DoEach(ActorInfo.RenderGlow);
      TrueVision.LaserRenderSurface.EndRender();
      TrueVision.TVGraphicEffect.UpdateGlow();
    }
    */

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
