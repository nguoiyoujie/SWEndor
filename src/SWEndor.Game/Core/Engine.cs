using System;
using SWEndor.Game.Actors;
using SWEndor.Game.Scenarios;
using SWEndor.Game.Input;
using SWEndor.Game.Sound;
using SWEndor.Game.UI;
using SWEndor.Game.Weapons;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.Player;
using SWEndor.Game.UI.Forms;
using SWEndor.Game.AI.Squads;
using Primrose.Primitives;
using System.Threading;
using SWEndor.Game.ExplosionTypes;
using SWEndor.Game.Explosions;
using SWEndor.Game.Shaders;
using SWEndor.Game.Projectiles;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.UI.Menu.Pages;
using System.Threading.Tasks;
using Primrose.Primitives.Factories;
using MTV3D65;
using Primrose.Primitives.Geometry;
using SWEndor.Game.Models;
using System.Collections.Generic;
using SWEndor.Game.Particles;
using SWEndor.Game.ParticleTypes;
using SWEndor.Game.ActorTypes.Components;
using Primrose.Primitives.Extensions;
using SWEndor.Game.Scene;

namespace SWEndor.Game.Core
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
    internal Settings Settings { get; private set; }
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
    internal Factory<ParticleInfo, ParticleCreationInfo, ParticleTypeInfo> ParticleFactory { get; private set; }
    internal ActorTypeInfo.Factory ActorTypeFactory { get; private set; }
    internal ExplosionTypeInfo.Factory ExplosionTypeFactory { get; private set; }
    internal ProjectileTypeInfo.Factory ProjectileTypeFactory { get; private set; }
    internal ParticleTypeInfo.Factory ParticleTypeFactory { get; private set; }
    internal WeapRegistry WeaponRegistry { get; private set; }    
    internal Squadron.Factory SquadronFactory { get; private set; }
    internal ShaderInfo.Factory ShaderFactory { get; private set; }
    internal MeshEntityTable ActorMeshTable { get; private set; }
    internal MeshEntityTable ExplosionMeshTable { get; private set; }
    internal MeshEntityTable ProjectileMeshTable { get; private set; }
    internal List<MeshEntityTable> MeshTables { get; private set; }
    internal Registry<TVMesh> MeshRegistry { get; private set; }
    internal Registry<int> TextureRegistry { get; private set; }
    private int[] RenderMeshList { get; set; }
    private int[] RenderSortedMeshList { get; set; }


    // Engine parts to be loaded late
    // Requires ActorInfoType initialization
    internal ScenarioManager GameScenarioManager { get; private set; }
    internal Scenarios.Scripting.Context ScriptContext { get; private set; }

    public void Init()
    {
      Settings = new Settings();
      Game = new Session(this);
      SoundManager = new SoundManager();
      PerfManager = new PerfManager(this);
      ActorFactory = new Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo>(this, (e, f, n, i) => { return new ActorInfo(e, f, n, i); }, Globals.ActorLimit);
      ExplosionFactory = new Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo> (this, (e, f, n, i) => { return new ExplosionInfo(e, f, n, i); }, Globals.ActorLimit);
      ProjectileFactory = new Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo>(this, (e, f, n, i) => { return new ProjectileInfo(e, f, n, i); }, Globals.ActorLimit);
      ParticleFactory = new Factory<ParticleInfo, ParticleCreationInfo, ParticleTypeInfo>(this, (e, f, n, i) => { return new ParticleInfo(e, f, n, i); }, Globals.ActorLimit);
      ActorTypeFactory = new ActorTypeInfo.Factory(this);
      ExplosionTypeFactory = new ExplosionTypeInfo.Factory(this);
      ProjectileTypeFactory = new ProjectileTypeInfo.Factory(this);
      ParticleTypeFactory = new ParticleTypeInfo.Factory(this);
      WeaponRegistry = new WeapRegistry();
      SquadronFactory = new Squadron.Factory();
      ShaderFactory = new ShaderInfo.Factory(this);
      ActorMeshTable = new MeshEntityTable();
      ProjectileMeshTable = new MeshEntityTable();
      ExplosionMeshTable = new MeshEntityTable();
      MeshRegistry = new Registry<TVMesh>();
      TextureRegistry = new Registry<int>();
      PlayerInfo = new PlayerInfo(this);
      MeshTables = new List<MeshEntityTable> { ActorMeshTable, ProjectileMeshTable, ExplosionMeshTable };
      RenderMeshList = new int[1028];
      RenderSortedMeshList = new int[1028];

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
      Surfaces = new RenderSurfaces(this);
      Screen2D = new Screen2D(this);

      Surfaces.RenderOnce();
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
      // initialize
      Screen2D.CurrentPage = new LoadingGame(Screen2D);
      Screen2D.ShowPage = true;

      Screen2D.AppendLoadingText(Globals.LoadingFlavourTexts[Random.Next(0, Globals.LoadingFlavourTexts.Count)]);

      // Init MeshData before models
      MeshData.Init(this);
      ActorTypeFactory.RegisterBase();

      Task pload = Task.Factory.StartNew(ProjectileTypeFactory.Load);
      Task aload = Task.Factory.StartNew(ActorTypeFactory.Load);
      Task eload = Task.Factory.StartNew(ExplosionTypeFactory.Load);
      Task rload = Task.Factory.StartNew(ParticleTypeFactory.Load);

      Task.WaitAll(new Task[] { pload, aload, eload, rload });

      Screen2D.AppendLoadingText("Loading weapons...");
      WeaponRegistry.LoadFromINI(this);

      Screen2D.AppendLoadingText("Loading sounds...");
      SoundManager.Initialize();
      SoundManager.Load();

      Screen2D.AppendLoadingText("Loading dynamic music info...");
      SoundManager.Piece.Factory.LoadFromINI(SoundManager, Globals.DynamicMusicINIPath);

      AtmosphereInfo.LoadDefaults(true, true);
      LandInfo.LoadDefaults();

      ScriptContext = new Scenarios.Scripting.Context(this);

      // late ActorType bindings...
      ProjectileTypeFactory.Initialise();
      ActorTypeFactory.Initialise();
      ExplosionTypeFactory.Initialise();
      ParticleTypeFactory.Initialise();

      Screen2D.AppendLoadingText("Loading scenario engine...");
      GameScenarioManager = new ScenarioManager(this);

      // Preload copies of Actors (beware arbitary numbers)
      TV_3DVECTOR pos = new TV_3DVECTOR(10000, 100, 10000);
      foreach (KeyValuePair<string, ActorTypeInfo> atinfo in ActorTypeFactory)
      {
        for (int i = 0; i < 3; i++)
          ActorFactory.Create(new ActorCreationInfo(atinfo.Value)).Position = pos;
      }
      foreach (KeyValuePair<string, ExplosionTypeInfo> etinfo in ExplosionTypeFactory)
      {
        for (int i = 0; i < 5; i++)
          ExplosionFactory.Create(new ExplosionCreationInfo(etinfo.Value)).Position = pos;
      }
      foreach (KeyValuePair<string, ProjectileTypeInfo> ptinfo in ProjectileTypeFactory)
      {
        for (int i = 0; i < 10; i++)
          ProjectileFactory.Create(new ProjectileCreationInfo(ptinfo.Value)).Position = pos;
      }
      foreach (KeyValuePair<string, ParticleTypeInfo> ptinfo in ParticleTypeFactory)
      {
        for (int i = 0; i < 10; i++)
          ParticleFactory.Create(new ParticleCreationInfo(ptinfo.Value)).Position = pos;
      }
      ActorFactory.Reset();
      ExplosionFactory.Reset();
      ProjectileFactory.Reset();
      ParticleFactory.Reset();

      SoundManager.Clear();
      Screen2D.AppendLoadingText("Loading main menu...");
      GameScenarioManager.LoadMainMenu();
    }

    private readonly Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo>.EngineActionDelegate _process = ActorInfo.Process;
    private readonly Factory<ExplosionInfo, ExplosionCreationInfo, ExplosionTypeInfo>.EngineActionDelegate _processExp = ExplosionInfo.ProcessExp;
    private readonly Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo>.EngineActionDelegate _processProj = ProjectileInfo.Process;
    private readonly Factory<ParticleInfo, ParticleCreationInfo, ParticleTypeInfo>.EngineActionDelegate _processPart = ParticleInfo.Process;
    private readonly Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo>.EngineActionDelegate _processAI = ActorInfo.ProcessAI;
    private readonly Factory<ActorInfo, ActorCreationInfo, ActorTypeInfo>.EngineActionDelegate _processCollision = ActorInfo.ProcessCollision;
    private readonly Factory<ProjectileInfo, ProjectileCreationInfo, ProjectileTypeInfo>.EngineActionDelegate _processProjCollision = ProjectileInfo.ProcessCollision;

    // explicit delegate conversion and cache it on the class to avoid hidden allocations
    public void Process() { ActorFactory.DoEach(_process); }
    public void ProcessExpl() { ExplosionFactory.DoEach(_processExp); }
    public void ProcessProj() { ProjectileFactory.DoEach(_processProj); }
    public void ProcessPart() { ParticleFactory.DoEach(_processPart); }
    public void ProcessAI() { ActorFactory.StaggeredDoEach(2, ref Game.AITickCount, _processAI); }
    public void ProcessCollision() { ActorFactory.StaggeredDoEach(5, ref Game.CollisionTickCount, _processCollision); }
    public void ProcessProjCollision() { ProjectileFactory.StaggeredDoEach(5, ref Game.CollisionTickCount, _processProjCollision); }

    private int _actorTypes;
    private ActorTypeInfo _loadingType;
    private TVMesh _loadingMesh;
    public void PreRender()
    {
      using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
      {
        TrueVision.TVEngine.Clear();
        TrueVision.TVScene.FinalizeShadows();
      }

      // render wireframe
      if (_actorTypes != ActorTypeFactory.Count)
      {
        _actorTypes = ActorTypeFactory.Count;
        foreach (ActorTypeInfo atype in ActorTypeFactory.GetValues()) // allocation!
        {
          if ((_loadingType == null || _loadingType.AIData.TargetType.Has(TargetType.FIGHTER)) && atype.AIData.TargetType.Has(TargetType.FIGHTER))
          {
            _loadingType = atype;
          }
          else if (atype.AIData.TargetType.Has(TargetType.SHIP))
          {
            _loadingType = atype;
          }
        }
        if (_loadingType != null)
        {
          _loadingMesh = _loadingType.MeshData.SourceMesh;
        }
      }

      if (_loadingMesh != null)
      {
        TV_3DVECTOR r = new TV_3DVECTOR(15, 120, 0);
        TV_3DVECTOR spos = new TV_3DVECTOR();
        TV_3DVECTOR smin = new TV_3DVECTOR();
        TV_3DVECTOR smax = new TV_3DVECTOR();
        float srad = 0;
        _loadingMesh.GetBoundingSphere(ref spos, ref srad);
        _loadingMesh.GetBoundingBox(ref smax, ref smax);
        TV_3DVECTOR scenter = (smin + smax) / 2;
        Sphere sph = new Sphere(spos, srad);
        TVCamera c = TrueVision.TVScene.GetCamera();
        c.SetRotation(r.x, r.y, r.z);
        c.SetPosition(scenter.x, scenter.y, scenter.z);
        TV_3DVECTOR d2 = c.GetFrontPosition(-sph.R * (_loadingType.AIData.TargetType.Has(TargetType.SHIP) ? 1.5f : 4f));
        c.SetPosition(d2.x, d2.y, d2.z);

        TrueVision.TVScene.SetRenderMode(CONST_TV_RENDERMODE.TV_LINE);
        _loadingMesh.Render();
        TrueVision.TVScene.SetRenderMode(CONST_TV_RENDERMODE.TV_SOLID);
      }

      Screen2D.Draw();
      Screen2D.CurrentPage?.RenderTick();

      //using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        TrueVision.TVEngine.RenderToScreen();
    }

    public void Render()
    {
      Surfaces.Render();

      TrueVision.TVEngine.Clear();
      AtmosphereInfo.Render();
      //LandInfo.Render();

      int maxRenderOrder = GetMaxRenderOrder();
      if (maxRenderOrder > 0)
      {
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          TrueVision.TVScene.FinalizeShadows();
          RenderMeshes(0);
          TrueVision.TVScene.RenderAllParticleSystems();
          for (int i = 1; i <= maxRenderOrder; i++)
          {
            RenderMeshes(i);
          }
        }
      }
      else
      {
        // ok to render all
        using (ScopeCounters.AcquireWhenZero(ScopeGlobals.GLOBAL_TVSCENE))
        {
          TrueVision.TVScene.FinalizeShadows();
          TrueVision.TVScene.RenderAllMeshes(true);
          TrueVision.TVScene.RenderAllParticleSystems();
        }
      }

      Screen2D.Draw();
      Screen2D.CurrentPage?.RenderTick();

      TrueVision.TVEngine.RenderToScreen();
    }

    private int GetMaxRenderOrder()
    {
      int count = 0;
      foreach (MeshEntityTable table in MeshTables)
      {
        count = count.Max(table.GetMaxRenderOrder());
      }
      return count;
    }

    private int RenderMeshes(int meshRenderOrder)
    {
      //Array.Clear(RenderMeshList, 0, RenderMeshList.Length);
      int index = 0;
      foreach (MeshEntityTable table in MeshTables)
      {
        if (table.GetMaxRenderOrder() < meshRenderOrder)
          continue;

        foreach (var meshID in table.EnumerateKeys())
        {
          if (table[meshID].RenderOrder == meshRenderOrder && table.IsVisible(meshID))
          {
            if (index > RenderMeshList.Length)
            {
              // replace list
              var oldlist = RenderMeshList;
              RenderMeshList = new int[oldlist.Length * 2];
              RenderSortedMeshList = new int[RenderMeshList.Length];
              Array.Copy(oldlist, RenderMeshList, RenderMeshList.Length);
            }
            RenderMeshList[index++] = meshID;
          }
        }
      }
      if (index > 0)
      {
        TrueVision.TVScene.SortMeshList(index, RenderMeshList, RenderSortedMeshList);
        TrueVision.TVScene.RenderMeshList(index, RenderSortedMeshList);
      }
      return index;
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
      GameScenarioManager?.Scenario.Unload();
      Game.PrepExit();
      Form.Exit();
    }
  }
}
