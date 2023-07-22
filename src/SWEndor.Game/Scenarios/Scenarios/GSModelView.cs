using MTV3D65;
using Primrose.Primitives;
using SWEndor.Game.Actors;
using SWEndor.Game.ActorTypes;
using SWEndor.Game.AI.Actions;
using SWEndor.Game.Core;
using SWEndor.Game.Explosions;
using SWEndor.Game.ExplosionTypes;
using SWEndor.Game.Player;
using SWEndor.Game.Projectiles;
using SWEndor.Game.ProjectileTypes;
using SWEndor.Game.UI.Menu.Pages;

namespace SWEndor.Game.Scenarios
{
  public class GSModelView : ScenarioBase
  {
    public GSModelView(ScenarioManager manager) : base(manager)
    {
      Info.Name = "Model Viewer";
      Info.AllowedWings = new ActorTypeInfo[] { Engine.ActorTypeFactory.Get("TIE") };
      Info.AllowedDifficulties = new string[] { "normal" };
    }

    private ActorInfo m_CurrentActor = null;
    private ExplosionInfo m_CurrentExplosion = null;
    private ProjectileInfo m_CurrentProjectile= null;
    public FactionInfo TestFaction = FactionInfo.Neutral;

    public override void Load(ActorTypeInfo wing, string difficulty)
    {
      base.Load(wing, difficulty);
    }

    public override void Launch()
    {
      base.Launch();

      State.MaxBounds = new TV_3DVECTOR(2500, 1000, 2500);
      State.MinBounds = new TV_3DVECTOR(-2500, -1000, -2500);
      State.MaxAIBounds = new TV_3DVECTOR(2500, 1000, 2500);
      State.MinAIBounds = new TV_3DVECTOR(-2500, -1000, -2500);

      PlayerInfo.Lives = 2;
      PlayerInfo.ScorePerLife = 1000000;
      PlayerInfo.ScoreForNextLife = 1000000;

      //MakePlayer = Test_SpawnPlayer;
      
      Screen2D.Line1.Color = new COLOR(1f, 1f, 0.3f, 1);

      SoundManager.SetMusicDyn("TRO-IN");

      State.IsCutsceneMode = false;

      PlayerCameraInfo.CameraMode = CameraMode.CUSTOM;
      PlayerCameraInfo.SceneLook.SetPosition_Point(new TV_3DVECTOR(500, 200, 2000));
      PlayerCameraInfo.SceneLook.SetTarget_LookAtPoint(new TV_3DVECTOR());
      PlayerCameraInfo.SceneLook.Update(Engine, PlayerCameraInfo.Camera, 0);
      PlayerCameraInfo.FreeLook.Position = PlayerCameraInfo.Position;
      PlayerCameraInfo.FreeLook.Rotation = PlayerCameraInfo.Rotation;
      PlayerCameraInfo.IsFreeLook = true;
    }

    internal override void LoadFactions()
    {
      base.LoadFactions();
      TestFaction = FactionInfo.Factory.Add("Test", new COLOR(0.6f, 0.6f, 0.6f, 1));
    }

    internal override void LoadScene()
    {
      base.LoadScene();
    }

    public override void GameTick()
    {
      base.GameTick();
      if (Screen2D.CurrentPage == null)
        Screen2D.CurrentPage = new ModelSelection(Screen2D);

      Screen2D.ShowPage = true;

      if (Screen2D.CurrentPage is ModelSelection msel)
      {
        object model = msel.GetModelType();
        if (model is ActorTypeInfo atype) 
        {
          if (m_CurrentExplosion != null)
          {
            ScopeCounters.ReleaseOne(m_CurrentExplosion.Scope);
            m_CurrentExplosion.Delete();
            m_CurrentExplosion = null;
          }
          if (m_CurrentProjectile != null)
          {
            ScopeCounters.ReleaseOne(m_CurrentProjectile.Scope);
            m_CurrentProjectile.Delete();
            m_CurrentProjectile = null;
          }

          if (m_CurrentActor == null || m_CurrentActor.TypeInfo != atype || m_CurrentActor.IsDead)
          {
            if (m_CurrentActor != null)
            {
              m_CurrentActor.Delete();
              ScopeCounters.ReleaseOne(m_CurrentActor.Scope);
            }
            m_CurrentActor = Engine.ActorFactory.Create(new ActorCreationInfo(atype) { FreeSpeed = true, InitialSpeed = 0});
            m_CurrentActor.MoveData.FreeSpeed = true;
            ScopeCounters.Acquire(m_CurrentActor.Scope);
          }
        }
        else if (model is ExplosionTypeInfo etype)
        {
          if (m_CurrentActor != null)
          {
            ScopeCounters.ReleaseOne(m_CurrentActor.Scope);
            m_CurrentActor.Delete();
            m_CurrentActor = null;
          }
          if (m_CurrentProjectile != null)
          {
            ScopeCounters.ReleaseOne(m_CurrentProjectile.Scope);
            m_CurrentProjectile.Delete();
            m_CurrentProjectile = null;
          }

          if (m_CurrentExplosion == null || m_CurrentExplosion.TypeInfo != etype || m_CurrentExplosion.IsDead)
          {
            if (m_CurrentExplosion != null)
            {
              m_CurrentExplosion.Delete();
              ScopeCounters.ReleaseOne(m_CurrentExplosion.Scope);
            }
            m_CurrentExplosion = Engine.ExplosionFactory.Create(new ExplosionCreationInfo(etype));
            ScopeCounters.Acquire(m_CurrentExplosion.Scope);
          }
        }
        else if(model is ProjectileTypeInfo ptype)
        {
          if (m_CurrentActor != null)
          {
            ScopeCounters.ReleaseOne(m_CurrentActor.Scope);
            m_CurrentActor.Delete();
            m_CurrentActor = null;
          }
          if (m_CurrentExplosion != null)
          {
            ScopeCounters.ReleaseOne(m_CurrentExplosion.Scope);
            m_CurrentExplosion.Delete();
            m_CurrentExplosion = null;
          }

          if (m_CurrentProjectile == null || m_CurrentProjectile.TypeInfo != ptype || m_CurrentProjectile.IsDead)
          {
            if (m_CurrentProjectile != null)
            {
              m_CurrentProjectile.Delete();
              ScopeCounters.ReleaseOne(m_CurrentProjectile.Scope);
            }
            m_CurrentProjectile = Engine.ProjectileFactory.Create(new ProjectileCreationInfo(ptype) { FreeSpeed = true, InitialSpeed = 0 });
            ScopeCounters.Acquire(m_CurrentProjectile.Scope);
          }
        }
      }
    }

    public void Test_SpawnPlayer()
    {
      PlayerInfo.ActorID = PlayerInfo.TempActorID;

      if (PlayerInfo.Actor == null || PlayerInfo.Actor.Disposed)
      { 
        if (PlayerInfo.Lives > 0)
        {
          PlayerInfo.Lives--;

          ActorInfo ainfo = new ActorSpawnInfo
          {
            Type = PlayerInfo.ActorType,
            Name = "(Player)",
            SidebarName = "",
            SpawnTime = Game.GameTime,
            Faction = FactionInfo.Neutral,
            Position = new TV_3DVECTOR(125, 0, 125),
            Rotation = new TV_3DVECTOR(),
            Actions = new ActionInfo[] { Wait.GetOrCreate(5) },
            Registries = null
          }.Spawn(this);

          PlayerInfo.ActorID = ainfo.ID;
        }
      }
      PlayerInfo.SystemLockMovement = false;
    }



  }
}
