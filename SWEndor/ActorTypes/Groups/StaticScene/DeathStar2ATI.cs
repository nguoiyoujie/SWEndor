using MTV3D65;
using SWEndor.Actors;
using SWEndor.ActorTypes.Components;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes
{
  public class DeathStar2ATI : StaticSceneGroup
  {
    private static DeathStar2ATI _instance;
    public static DeathStar2ATI Instance()
    {
      if (_instance == null) { _instance = new DeathStar2ATI(); }
      return _instance;
    }

    int[] texanimframes;

    private DeathStar2ATI() : base("DeathStar2")
    {
      OnTimedLife = false;
      TimedLife = 5;

      float size = 20000;

      SourceMesh = Engine.Instance().TVGlobals.GetMesh(Key);
      if (SourceMesh == null)
      {
        SourceMesh = Engine.Instance().TVScene.CreateMeshBuilder(Key);

        List<int> death = new List<int> { 1, 10, 15, 16, 20, 21, 22, 23, 27, 28, 30, 31, 34, 35, 36 };
        List<int> frames = new List<int>();

        string texname = Path.Combine("deathstar", "deathstar2.bmp");
        string texdname = Path.Combine("deathstar", "deathstar2b.jpg");
        string alphatexname = Path.Combine("deathstar", "deathstar2alpha.bmp");
        string texpath = Path.Combine(Globals.ImagePath, texname);
        string texdpath = Path.Combine(Globals.ImagePath, texdname);
        string alphatexpath = Path.Combine(Globals.ImagePath, alphatexname); // alphatexname

        
        int dstartex = 0;
        int dstardtex = 0;
        if (Engine.Instance().TVGlobals.GetTex(texname) == 0)
        {
          int texS = Engine.Instance().TVTextureFactory.LoadTexture(texpath);
          int texA = Engine.Instance().TVTextureFactory.LoadTexture(alphatexpath); // note we are loading alpha map as texture
          dstartex = Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }

        if (Engine.Instance().TVGlobals.GetTex(texdname) == 0)
        {
          int texS = Engine.Instance().TVTextureFactory.LoadTexture(texdpath);
          int texA = Engine.Instance().TVTextureFactory.LoadTexture(alphatexpath);
          dstardtex = Engine.Instance().TVTextureFactory.AddAlphaChannel(texS, texA, texdname);
        }
        
        /*
        int dstartex = LoadAlphaTexture(texname, texpath, alphatexpath);
        int dstardtex = LoadAlphaTexture(texdname, texdpath, alphatexpath);
        */
        for (int i = 0; i <= 36; i++)
          frames.Add(death.Contains(i) ? dstardtex : dstartex);

        texanimframes = frames.ToArray();

        SourceMesh.CreateBox(size, size, 0.001f);
        SourceMesh.SetTexture(texanimframes[0]);
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

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.ActorState == ActorState.DYING)
      {
        int k = texanimframes.Length - 1 - (int)(ainfo.CycleInfo.CycleTime / ainfo.CycleInfo.CyclePeriod * texanimframes.Length);
        if (k >= 0 && k < texanimframes.Length)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}


