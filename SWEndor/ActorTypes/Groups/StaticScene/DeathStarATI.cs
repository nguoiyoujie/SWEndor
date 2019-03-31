using MTV3D65;
using System.IO;

namespace SWEndor.Actors.Types
{
  public class DeathStarATI : StaticSceneGroup
  {
    private static DeathStarATI _instance;
    public static DeathStarATI Instance()
    {
      if (_instance == null) { _instance = new DeathStarATI(); }
      return _instance;
    }

    private DeathStarATI() : base("DeathStar")
    {
      float size = 20000;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        string texname = Path.Combine("deathstar", "deathstar.bmp");
        string alphatexname = Path.Combine("deathstar", "deathstaralpha.bmp"); 
        string texpath = Path.Combine(Globals.ImagePath, texname);
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname); // alphatexname
        int itex = LoadAlphaTexture(texname, texpath, alphatexpath);

        SourceMesh.CreateBox(size, size, 0.001f);
        SourceMesh.SetTexture(itex);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      AddOns = new AddOnInfo[] { new AddOnInfo("Death Star Laser Source", new TV_3DVECTOR(-0.13f * size, 0.2f * size, -0.04f * size), new TV_3DVECTOR(0, 0, 0), true) };
    }

    public override void Initialize(ActorInfo ainfo)
    {
      base.Initialize(ainfo);

      ainfo.ExplosionInfo.EnableDeathExplosion = true;
      ainfo.ExplosionInfo.DeathExplosionType = "ExplosionMega";
      ainfo.ExplosionInfo.DeathExplosionSize = 1;
    }

    public override void ProcessNewState(ActorInfo ainfo)
    {
      base.ProcessNewState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        ainfo.CombatInfo.OnTimedLife = true;
        ainfo.CombatInfo.TimedLife = 5f;
        ainfo.CombatInfo.IsCombatObject = false;
      }
      else if (ainfo.ActorState == ActorState.DEAD)
      {
        ActorCreationInfo acinfo = new ActorCreationInfo(ExplosionWaveMegaATI.Instance());
        acinfo.Position = ainfo.GetPosition();
        ActorInfo explwav = ActorInfo.Create(acinfo);
        explwav.Scale = new MTV3D65.TV_3DVECTOR(10, 10, 10);
      }
    }
  }
}


