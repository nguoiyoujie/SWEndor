﻿using SWEndor.Actors;
using SWEndor.Actors.Components;
using System.Collections.Generic;
using System.IO;

namespace SWEndor.ActorTypes.Instances
{
  public class DeathStar2ATI : Groups.StaticScene
  {
    int[] texanimframes;

    internal DeathStar2ATI(Factory owner) : base(owner, "DeathStar2")
    {
      Explodes = new ExplodeInfo[]
      {
        new ExplodeInfo("ExpL02", 1, 1, ExplodeTrigger.ON_DEATH),
        new ExplodeInfo("ExpW02", 1, 1, ExplodeTrigger.ON_DEATH),
      };

      float size = 20000;

      SourceMesh = TrueVision.TVGlobals.GetMesh(Name);
      if (SourceMesh == null)
      {
        SourceMesh = TrueVision.TVScene.CreateMeshBuilder(Name);

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
        if (TrueVision.TVGlobals.GetTex(texname) == 0)
        {
          int texS = TrueVision.TVTextureFactory.LoadTexture(texpath);
          int texA = TrueVision.TVTextureFactory.LoadTexture(alphatexpath); // note we are loading alpha map as texture
          dstartex = TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texname);
        }

        if (TrueVision.TVGlobals.GetTex(texdname) == 0)
        {
          int texS = TrueVision.TVTextureFactory.LoadTexture(texdpath);
          int texA = TrueVision.TVTextureFactory.LoadTexture(alphatexpath);
          dstardtex = TrueVision.TVTextureFactory.AddAlphaChannel(texS, texA, texdname);
        }
        
        /*
        int dstartex = LoadAlphaTexture(texname, texpath, alphatexpath);
        int dstardtex = LoadAlphaTexture(texdname, texdpath, alphatexpath);
        */
        for (int i = 0; i <= 36; i++)
          frames.Add(death.Contains(i) ? dstardtex : dstartex);

        texanimframes = frames.ToArray();

        SourceMesh.AddWall(texanimframes[0], -size / 2, 0, size / 2, 0, size, -size / 2);
        //SourceMesh.CreateBox(size, size, 0.001f);
        SourceMesh.SetTexture(texanimframes[0]);
        SourceMesh.Enable(false);
        SourceMesh.SetCollisionEnable(false);
      }

      //AddOns = new AddOnInfo[] { new AddOnInfo("Death Star Laser Source", new TV_3DVECTOR(-1300, 2000, -0.04f * size), new TV_3DVECTOR(0, 0, 0), true) };
    }

    public override void Dying(ActorInfo ainfo)
    {
      base.Dying(ainfo);

      ainfo.DyingTimerSet(5, true);
      CombatSystem.Deactivate(Engine, ainfo);
    }

    public override void ProcessState(ActorInfo ainfo)
    {
      base.ProcessState(ainfo);
      if (ainfo.IsDying)
      {
        int k = texanimframes.Length - 1 - (int)(ainfo.CycleInfo.CycleTime / ainfo.CycleInfo.CyclePeriod * texanimframes.Length);
        if (k >= 0 && k < texanimframes.Length)
          ainfo.SetTexture(texanimframes[k]);
      }
    }
  }
}


