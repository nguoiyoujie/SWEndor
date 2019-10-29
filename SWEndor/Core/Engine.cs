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
using System.Text;
using SWEndor.AI.Squads;
using SWEndor.Primitives;
using System.Threading;
using SWEndor.ExplosionTypes;
using SWEndor.Explosions;
using SWEndor.Shaders;
using SWEndor.Projectiles;
using SWEndor.ProjectileTypes;
using SWEndor.Primitives.Geometry;

namespace SWEndor.Core
{
  public class Engine
  {
    internal Engine() { }

    internal GameForm Form;
    private IntPtr Handle;
    public int ScreenWidth { get; internal set;}
    public int ScreenHeight { get; internal set; }
    public Random Random { get; } = new Random();

    // Engine parts
    internal Session Game { get; private set; }
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
    internal RenderSurfaces Surfaces { get; private set; }

    // Factories and Registries
    internal Font.Factory FontFactory { get; private set; }
    internal Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo> ActorFactory { get; private set; }
    internal Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo> ExplosionFactory { get; private set; }
    internal Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo> ProjectileFactory { get; private set; }
    internal ActorTypeInfo.Factory ActorTypeFactory { get; private set; }
    internal ExplosionTypeInfo.Factory ExplosionTypeFactory { get; private set; }
    internal ProjectileTypeInfo.Factory ProjectileTypeFactory { get; private set; }
    internal WeaponFactory WeaponFactory { get; private set; }
    internal WeaponLoadoutFactory WeaponLoadoutFactory { get; private set; }
    internal Squadron.Factory SquadronFactory { get; private set; }
    internal ShaderInfo.Factory ShaderFactory { get; private set; }

    // Engine pars to be loaded late
    // Requires ActorInfoType initialization
    internal GameScenarioManager GameScenarioManager { get; private set; }
    internal Scenarios.Scripting.Expressions.Context ScriptContext { get; private set; }

    public void Init()
    {
      Game = new Session(this);
      SoundManager = new SoundManager(this);
      PerfManager = new PerfManager(this);
      ActorFactory = new Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo>(this, (e, f, n, i) => { return new ActorInfo(e, f, n, i); }, Globals.ActorLimit);
      ExplosionFactory = new Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo> (this, (e, f, n, i) => { return new ExplosionInfo(e, f, n, i); }, Globals.ActorLimit);
      ProjectileFactory = new Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo>(this, (e, f, n, i) => { return new ProjectileInfo(e, f, n, i); }, Globals.ActorLimit);
      ActorTypeFactory = new ActorTypeInfo.Factory(this);
      ExplosionTypeFactory = new ExplosionTypeInfo.Factory(this);
      ProjectileTypeFactory = new ProjectileTypeInfo.Factory(this);
      WeaponFactory = new WeaponFactory();
      WeaponLoadoutFactory = new WeaponLoadoutFactory();
      SquadronFactory = new Squadron.Factory();
      ShaderFactory = new ShaderInfo.Factory(this);
      PlayerInfo = new PlayerInfo(this);

      FontFactory = new Font.Factory();
    }

    public void InitTV()
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
      Surfaces = new RenderSurfaces(this);

      ShaderFactory.Load();
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
      ProjectileTypeFactory.RegisterBase();
      ActorTypeFactory.RegisterBase();
      ExplosionTypeFactory.RegisterBase();

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
      ProjectileTypeFactory.Initialise();
      ActorTypeFactory.Initialise();
      ExplosionTypeFactory.Initialise();

      Screen2D.LoadingTextLines.Add("Loading scenario engine...");
      GameScenarioManager = new GameScenarioManager(this);
      GameScenarioManager.LoadInitial();

      Screen2D.LoadingTextLines.Add("Loading main menu...");
      GameScenarioManager.LoadMainMenu();
    }

    Action<Engine, ActorInfo> process = ActorInfo.Process;
    Action<Engine, ExplosionInfo> processExpl = ExplosionInfo.ProcessExp;
    Action<Engine, ProjectileInfo> processProj = ProjectileInfo.Process;
    Action<Engine, ActorInfo> processAI = ActorInfo.ProcessAI;
    Action<Engine, ActorInfo> processCollision = ActorInfo.ProcessCollision;
    Action<Engine, ProjectileInfo> processProjCollision = ProjectileInfo.ProcessCollision;

    public void Process() { ActorFactory.DoEach(process); }
    public void ProcessExpl() { ExplosionFactory.DoEach(processExpl); }
    public void ProcessProj() { ProjectileFactory.DoEach(processProj); }
    public void ProcessAI() { ActorFactory.StaggeredDoEach(2, processAI); }
    public void ProcessCollision() { ActorFactory.StaggeredDoEach(5, processCollision); }
    public void ProcessProjCollision() { ProjectileFactory.StaggeredDoEach(5, processProjCollision); }

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
      Surfaces.Render();

      TrueVision.TVEngine.Clear();

      AtmosphereInfo.Render();
      //LandInfo.Render();

      using (ScopeCounterManager.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        TrueVision.TVScene.FinalizeShadows();
        TrueVision.TVScene.RenderAllMeshes(true); //RenderAll(true);
      }

      // test
      //foreach (ActorInfo a in ActorFactory.GetAll())
      //{
      //ActorInfo a = PlayerInfo.TargetActor;
      //  if (a != null)
      //{
      //  GameScenarioManager.Octree.GetId(a.GetBoundingBox(false), ref a.OctID);
      //  Box b = GameScenarioManager.Octree.GetBox(a.OctID);
      //  TrueVision.TVScreen2DImmediate.Draw_Box3D(new TV_3DVECTOR(b.X.Min, b.Y.Min, b.Z.Min), new TV_3DVECTOR(b.X.Max, b.Y.Max, b.Z.Max), new TV_COLOR(1, 1, 0, 1).GetIntColor());
      //}
      //}

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
      Thread.Sleep(1500);
      GameScenarioManager.Scenario.Unload();
      Game.PrepExit();
      Form.Exit();
    }
  }
}
